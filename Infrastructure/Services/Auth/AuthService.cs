using Core.DTOs.AuthDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly TaskHelperService helperService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJWTServices jwtServices;
        private readonly MediaUploadService mediaUploadService;
        public AuthService(
            MinaretOpsDbContext _context,
            TaskHelperService _helperService,
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            IJWTServices _jwtServices,
            MediaUploadService service
            )
        {
            dbContext = _context;
            helperService = _helperService;
            userManager = _userManager;
            roleManager = _roleManager;
            jwtServices = _jwtServices;
            mediaUploadService = service;
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
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                Roles = userManager.GetRolesAsync(u).Result.ToList()
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
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                City = user.City,
                Street = user.Street,
                NID = user.NID,
                PaymentNumber = user.PaymentNumber,
                DateOfHiring = user.DateOfHiring,
                Roles = userManager.GetRolesAsync(user).Result.ToList()
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
            authDTO.Email = user.Email ?? string.Empty;
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

            //var imageUploadResult = await mediaUploadService.UploadImageWithPath(newUser.ProfilePicture, $"{newUser.FirstName} {newUser.LastName}");

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
                    return FailResult("Failed To Add New User");

                await userManager.AddToRoleAsync(user, newUser.Role.ToString());

                var authDTO = new AuthResponseDTO
                {
                    IsAuthenticated = true,
                    Message = "تم تسجيل حساب جديد بنجاح"
                };

                var emailPayload = new
                {
                    To = user.Email,
                    Subject = "Welcome On Board",
                    Template = "EmployeeOnBoarding",
                    Replacements = new Dictionary<string, string>
                    {
                        {"EmpName", $"{user.FirstName} {user.LastName}" },
                        {"EmpEmail", $"{user.Email}" },
                        {"EmpRole", string.Join(", ", await userManager.GetRolesAsync(user)) }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "New User Email", emailPayload);
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return authDTO;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ToggleVisibilityAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);

            user.IsDeleted = !user.IsDeleted;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new NotImplementedOperationException("فشل في تحديث حالة المستخدم");
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
                throw new InvalidObjectException("بيانات الحساب غير مكتملة");
            var user = await GetUserOrThrow(updatedUser.Id);

            if (user.FirstName != updatedUser.FirstName && !string.IsNullOrWhiteSpace(updatedUser.FirstName)) 
                user.FirstName = updatedUser.FirstName.Trim();
            if (user.LastName != updatedUser.LastName && !string.IsNullOrWhiteSpace(updatedUser.LastName))
                user.LastName = updatedUser.LastName.Trim();
            if (user.City != updatedUser.City && !string.IsNullOrWhiteSpace(updatedUser.City))
                user.City = updatedUser.City.Trim();
            if (user.Street != updatedUser.Street && !string.IsNullOrWhiteSpace(updatedUser.Street))
                user.Street = updatedUser.Street.Trim();
            if (user.PaymentNumber != updatedUser.PaymentNumber && !string.IsNullOrWhiteSpace(updatedUser.PaymentNumber))
                user.PaymentNumber = updatedUser.PaymentNumber.Trim();

            if (user.Email != updatedUser.Email && !string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                await userManager.SetEmailAsync(user, updatedUser.Email.Trim());
                await userManager.UpdateNormalizedEmailAsync(user);
                await userManager.SetUserNameAsync(user, updatedUser.Email.Split("@")[0].Trim());

                user.EmailConfirmed = true;
            }
            if (user.PhoneNumber != updatedUser.PhoneNumber && !string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
            {
                user.PhoneNumber = updatedUser.PhoneNumber.Trim();
                user.PhoneNumberConfirmed = true;
            }
            var userRoles = await userManager.GetRolesAsync(user);
            //if (!string.IsNullOrWhiteSpace(updatedUser.Role)
            //    && !userRoles.Contains(updatedUser.Role))
            //{
            //    await userManager.RemoveFromRolesAsync(user, userRoles);
            //    await userManager.AddToRoleAsync(user, updatedUser.Role.ToString());
            //}

            // before calling userManager.UpdateAsync(user)

            if (!string.IsNullOrWhiteSpace(updatedUser.Role))
            {
                var newRole = updatedUser.Role.Trim();

                // ensure role exists in the role store
                if (!await roleManager.RoleExistsAsync(newRole))
                {
                    throw new InvalidOperationException($"Role '{newRole}' does not exist.");
                }

                // case-insensitive check whether user already has the role
                var hasRole = userRoles.Any(r => r.Equals(newRole, StringComparison.OrdinalIgnoreCase));
                if (!hasRole)
                {
                    // Remove existing roles (if any) and verify result
                    if (userRoles.Any())
                    {
                        var removeResult = await userManager.RemoveFromRolesAsync(user, userRoles);
                        if (!removeResult.Succeeded)
                        {
                            var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                            throw new InvalidOperationException($"Failed to remove existing roles: {errors}");
                        }
                    }

                    // Add new role and verify
                    var addResult = await userManager.AddToRoleAsync(user, newRole);
                    if (!addResult.Succeeded)
                    {
                        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to add role '{newRole}': {errors}");
                    }
                }
            }

            //if (!string.IsNullOrWhiteSpace(updatedUser.Role))
            //{
            //    // Use case-insensitive comparison to check if user already has the role
            //    var normalizedNewRole = updatedUser.Role.Trim();
            //    var hasRole = userRoles.Any(r => r.Equals(normalizedNewRole, StringComparison.OrdinalIgnoreCase));

            //    if (!hasRole)
            //    {
            //        // Remove all existing roles first
            //        if (userRoles.Any())
            //        {
            //            await userManager.RemoveFromRolesAsync(user, userRoles);
            //            // Refresh roles after removal to ensure they're actually removed
            //            userRoles = await userManager.GetRolesAsync(user);
            //        }

            //        // Add the new role
            //        var addRoleResult = await userManager.AddToRoleAsync(user, normalizedNewRole);
            //        if (!addRoleResult.Succeeded)
            //        {
            //            // Log or handle the error if needed
            //            throw new InvalidOperationException($"Failed to add role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            //        }
            //}
            //}
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                var emailPayload = new
                {
                    To = user.Email,
                    Subject = "Profile Update Successfully",
                    Template = "ProfileUpdate",
                    Replacements = new Dictionary<string, string>
                    {
                        {"EmpFullName", $"{user.FirstName} {user.LastName}" },
                        {"EmpEmail", $"{user.Email}" },
                        {"TimeStamp", $"{DateTime.UtcNow}" }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "Profile Updates Email", emailPayload);
                await dbContext.SaveChangesAsync();
            }

            return new AuthResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Street = user.Street,
                PaymentNumber = user.PaymentNumber,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
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
                    Message = "تعذر تحديث كلمة المرور"
                };
            }

            var result = await userManager.ChangePasswordAsync(user, passwordDTO.OldPassword, passwordDTO.NewPassword);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    IsAuthenticated = false,
                    Message = "تعذر تحديث كلمة المرور"
                };
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                var emailPayload = new
                {
                    To = user.Email,
                    Subject = "Change Password Confirmation",
                    Template = "ChangePasswordConfirmation",
                    Replacements = new Dictionary<string, string>
                    {
                        {"EmpFullName", $"{user.FirstName} {user.LastName}" },
                        {"EmpEmail", $"{user.Email}" },
                        {"TimeStamp", $"{DateTime.UtcNow}" }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "Profile Updates Email", emailPayload);
                await dbContext.SaveChangesAsync();
            }

            return new AuthResponseDTO
            {
                IsAuthenticated = true,
                Message = "تم تحديث كلمة المرور"
            };
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);

            await userManager.DeleteAsync(user);
            return true;
        }

        public async Task<string> RequestResetPasswordByAdminAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://internal.theminaretagency.com/reset-password?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
            if (!string.IsNullOrEmpty(user.Email))
            {
                var emailPayload = new
                {
                    To = user.Email,
                    Subject = "Reset Your Password",
                    Template = "RequestResetPassword",
                    Replacements = new Dictionary<string, string>
                    {
                        { "EmployeeName", $"{user.FirstName} {user.LastName}" },
                        { "EmployeeEmail", user.Email },
                        { "TimeStamp", $"{DateTime.UtcNow}" },
                        { "ResetLink", resetLink }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "Profile Updates Email", emailPayload);
                await dbContext.SaveChangesAsync();
            }
            return resetLink;
        }
        public async Task ResetPAsswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await GetUserOrThrow(resetPasswordDTO.UserId);
            var result = await userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}