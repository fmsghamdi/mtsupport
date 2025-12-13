using Microsoft.AspNetCore.Identity;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. إنشاء الأدوار (Roles) إذا لم تكن موجودة
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. إنشاء حساب المدير (Admin)
        var adminEmail = "admin@mabat.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            // كلمة المرور يجب أن تكون قوية (حرف كبير، صغير، رقم، رمز)
            var createPowerUser = await userManager.CreateAsync(admin, "Mabat@2025");

            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}