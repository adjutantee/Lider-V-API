using Lider_V_APIServices.Models;
using Microsoft.AspNetCore.Identity;

namespace Lider_V_APIServices.Services.Interfaces
{
    public class IdentityInitializer
    {
        public static void Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminEmail = "admin@gmail.com";
            var adminPassword = "_Admin1";
            var adminName = "Админ";
            var adminLastName = "Админ";

            if (roleManager.FindByNameAsync(Constants.AdminRoleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(Constants.AdminRoleName)).Wait();
            }
            if (roleManager.FindByNameAsync(Constants.UserRoleName).Result == null)
            {
                roleManager.CreateAsync(new IdentityRole(Constants.UserRoleName)).Wait();
            }
            if (userManager.FindByNameAsync(adminEmail).Result == null)
            {
                var admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    UserFirstName = adminName,
                    UserLastName = adminLastName,
                };

                var result = userManager.CreateAsync(admin, adminPassword).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, Constants.AdminRoleName).Wait();
                }
            }
        }

        //public static async Task Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        //{
        //    var adminEmail = "admin@gmail.com";
        //    var adminPassword = "_Admin1";
        //    var adminName = "Админ";
        //    var adminLastName = "Админ";

        //    if (await roleManager.FindByNameAsync(Constants.AdminRoleName) == null)
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(Constants.AdminRoleName));
        //    }
        //    if (await roleManager.FindByNameAsync(Constants.UserRoleName) == null)
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(Constants.UserRoleName));
        //    }

        //    if (await userManager.FindByNameAsync(adminEmail) == null)
        //    {
        //        var admin = new User
        //        {
        //            Email = adminEmail,
        //            UserName = adminEmail,
        //            UserFirstName = adminName,
        //            UserLastName = adminLastName,
        //        };

        //        var result = await userManager.CreateAsync(admin, adminPassword);

        //        if (result.Succeeded)
        //        {
        //            await userManager.AddToRoleAsync(admin, Constants.AdminRoleName);
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                Console.WriteLine($"Error: {error.Description}");
        //            }
        //        }
        //    }
        //}
    }
}
