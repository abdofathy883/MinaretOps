using AutoMapper;
using Core.DTOs.AuthDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJWTServices jwtServices;
        private readonly IEmailService emailService;
        public AuthService(
            MinaretOpsDbContext _context,
            UserManager<ApplicationUser> _userManager,
            IJWTServices _jwtServices,
            IEmailService email
            )
        {
            dbContext = _context;
            userManager = _userManager;
            jwtServices = _jwtServices;
            emailService = email;
        }
        public async Task<List<AuthResponseDTO>> GetAllUsersAsync()
        {
            var users = await userManager.Users.ToListAsync()
                ?? throw new InvalidObjectException("لا يوجد مستخدمين");

            return users.Select(u => new AuthResponseDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Roles = userManager.GetRolesAsync(u).Result.ToList(),
            }).ToList();
        }
        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ConcurrencyStamp = user.ConcurrencyStamp,
                City = user.City,
                Street = user.Street,
                NID = user.NID,
                PaymentNumber = user.PaymentNumber,
                DateOfHiring = user.DateOfHiring,
                Roles = userManager.GetRolesAsync(user).Result.ToList(),
            };
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO login)
        {
            var authDTO = new AuthResponseDTO();
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == login.PhoneNumber.Trim());
            if (user is null || !await userManager.CheckPasswordAsync(user, login.Password.Trim()))
            {
                authDTO.IsAuthenticated = false;
                authDTO.Message = "لا يوجد حساب بهذه البيانات";
                return authDTO;
            }

            if (user.IsDeleted)
            {
                authDTO.IsAuthenticated = false;
                authDTO.Message = "هذا الحساب غير موجود";
                return authDTO;
            }

            var roles = await userManager.GetRolesAsync(user);
            authDTO.Id = user.Id;
            authDTO.FirstName = user.FirstName;
            authDTO.LastName = user.LastName;
            authDTO.Email = user.Email;
            authDTO.UserName = user.UserName;
            authDTO.PhoneNumber = user.PhoneNumber ?? string.Empty;
            authDTO.Roles = roles.ToList();
            authDTO.IsAuthenticated = true;
            authDTO.Token = await jwtServices.GenerateAccessTokenAsync(user);
            authDTO.ConcurrencyStamp = user.ConcurrencyStamp;
            authDTO.City = user.City;
            authDTO.Street = user.Street;
            authDTO.NID = user.NID;
            authDTO.PaymentNumber = user.PaymentNumber;
            authDTO.DateOfHiring = user.DateOfHiring;

            if (user.RefreshTokens.Any(u => u.IsActive))
            {
                var ActiveRefreshToken = user.RefreshTokens.First(t => t.IsActive);
                authDTO.RefreshToken = ActiveRefreshToken.Token;
                authDTO.RefreshTokenExpiration = ActiveRefreshToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = await jwtServices.GenerateRefreshTokenAsync();
                authDTO.RefreshToken = RefreshToken.Token;
                authDTO.RefreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await userManager.UpdateAsync(user);
            }

            authDTO.Message = "تم تسجيل الدخول بنجاح";
            return authDTO;
        }
        public async Task<AuthResponseDTO> RegisterUserAsync(RegisterUserDTO newUser)
        {
            var validateErrors = await ValidateRegisterAsync(newUser);
            if (validateErrors is not null && validateErrors.Count > 0)
                return FailResult(string.Join(", ", validateErrors));

            var user = new ApplicationUser
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                UserName = newUser.Email.Split("@")[0],
                Email = newUser.Email,
                EmailConfirmed = true,
                PhoneNumber = newUser.PhoneNumber,
                PhoneNumberConfirmed = true,
                City = newUser.City,
                Street = newUser.Street,
                NID = newUser.NID,
                PaymentNumber = newUser.PaymentNumber,
                DateOfHiring = newUser.DateOfHiring
            };

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await userManager.CreateAsync(user, newUser.Password);

                if (!result.Succeeded)
                {
                    return FailResult(string.Join(", ", validateErrors));
                }

                await userManager.AddToRoleAsync(user, newUser.Role.ToString());

                var authDTO = new AuthResponseDTO
                {
                    IsAuthenticated = true,
                    Message = "تم تسجيل حساب جديد بنجاح"
                };

                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    {"Name", $"{user.FirstName} {user.LastName}" },
                    {"Email", $"{user.Email}" },
                    {"Role", $"{userManager.GetRolesAsync(user)}" }

                };

                await emailService.SendEmailWithTemplateAsync(user.Email, "Welcome On Board", "EmployeeOnBoarding", replacements);
                await dbTransaction.CommitAsync();
                return authDTO;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return FailResult($"حدث خطأ أثناء التسجيل: {ex.Message}");
            }
        }
        public async Task<bool> ToggleVisibilityAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);

            user.IsDeleted = !user.IsDeleted;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidObjectException("فشل في تحديث حالة المستخدم");
            }
            return user.IsDeleted;
        }
        public static AuthResponseDTO FailResult(string message)
        {
            return new AuthResponseDTO
            {
                IsAuthenticated = false,
                Message = message
            };
        }
        public async Task<List<string>> ValidateRegisterAsync(RegisterUserDTO registerDTO)
        {
            var errors = new List<string>();

            // Email Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                errors.Add("بريد الكتروني غير صالح");
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                errors.Add("هذا الايميل موجود بالفعل");
            }

            //Password Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                errors.Add("الرقم السري مطلوب");
            }
            else if (registerDTO.Password.Length < 6)
            {
                errors.Add("الرقم السري يجب ان يكون 6 احرف على الاقل");
            }

            //Phone 
            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف مطلوب");
            }

            //************** Phone Unique Validation
            if (await userManager.Users.AnyAsync(u => u.PhoneNumber == registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف موجود بالفعل");
            }

            //Name
            if (string.IsNullOrWhiteSpace(registerDTO.FirstName))
            {
                errors.Add("الاسم الاول مطلوب");
            }
            if (string.IsNullOrWhiteSpace(registerDTO.LastName))
            {
                errors.Add("الاسم الاخير مطلوب");
            }

            return errors;
        }
        private async Task<ApplicationUser> GetUserOrThrow(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user ?? throw new InvalidObjectException("لم يتم العثور على المستخدم");
        }
        public async Task<AuthResponseDTO> UpdateUserAsync(UpdateUserDTO updatedUser)
        {
            if (updatedUser is null)
                throw new Exception();
            var user = await GetUserOrThrow(updatedUser.Id);

            if (!string.IsNullOrWhiteSpace(updatedUser.FirstName)) 
                user.FirstName = updatedUser.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.LastName))
                user.LastName = updatedUser.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.City))
                user.City = updatedUser.City.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.Street))
                user.Street = updatedUser.Street.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.PaymentNumber))
                user.PaymentNumber = updatedUser.PaymentNumber.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                user.Email = updatedUser.Email.Trim();
                user.NormalizedEmail = userManager.NormalizeEmail(updatedUser.Email);
                user.EmailConfirmed = true;
            }
            if (!string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
            {
                user.PhoneNumber = updatedUser.PhoneNumber.Trim();
                user.PhoneNumberConfirmed = true;
            }
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            
            return new AuthResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Street = user.Street,
                PaymentNumber = user.PaymentNumber,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                NID = user.NID,
                DateOfHiring = user.DateOfHiring,
                IsAuthenticated = true,
                Message = "تم تحديث بيانات الحساب بنجاح",
            };
        }

        public async Task<AuthResponseDTO> ChangePasswordAsync(ChangePasswordDTO passwordDTO)
        {
            var user = await GetUserOrThrow(passwordDTO.Id);

            if (passwordDTO.ConfirmNewPassword != passwordDTO.NewPassword)
            {
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = "تعذر تحديث الرقم السري"
                };
            }

            var result = await userManager.ChangePasswordAsync(user, passwordDTO.OldPassword, passwordDTO.NewPassword);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = "تعذر تحديث الرقم السري"
                };
            }

            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Message = "تم تحديث الرقم السري"
            };
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);

            await userManager.DeleteAsync(user);
            return true;
        }
    }
}
