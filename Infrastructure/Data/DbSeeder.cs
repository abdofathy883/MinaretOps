using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<MinaretOpsDbContext>();

            // Seed Roles
            var roles = new[] { 
                "Admin", "AccountManager", "GraphicDesigner", "GraphicDesignerTeamLeader",
                "ContentCreator", "ContentCreatorTeamLeader", "AdsSpecialest", "SEOSpecialest",
                "WebDeveloper", "VideoEditor", "Finance", "SalesRep"
            };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed SuperAdmin
            string adminEmail = "abdofathy883@gmail.com";
            string password = "Aa123#";

            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingUser == null)
            {
                var admin = new ApplicationUser
                {
                    FirstName = "Abdo",
                    LastName = "Fathy",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "01028128912",
                    PhoneNumberConfirmed = true,
                    City = "Minya",
                    Street = "12345",
                    PaymentNumber = "01028128912",
                    NID = "29909272402873"
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    throw new Exception($"Failed to create Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            if (!await context.Currencies.AnyAsync())
            {
                var defaultCurrency = new Currency
                {
                    Code = "EGP",
                    Name = "Egyptian Pound",
                    DecimalPlaces = 2
                };
                await context.Currencies.AddAsync(defaultCurrency);
                await context.SaveChangesAsync();
                var existingUnifiedVault = await context.Vaults
                    .FirstOrDefaultAsync(v => v.VaultType == VaultType.Unified && v.CurrencyId == defaultCurrency.Id);

                if (existingUnifiedVault == null)
                {
                    var unifiedVault = new Vault
                    {
                        VaultType = VaultType.Unified,
                        BranchId = null, // Unified vaults don't belong to a branch
                        CurrencyId = defaultCurrency.Id
                    };
                    await context.Vaults.AddAsync(unifiedVault);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
