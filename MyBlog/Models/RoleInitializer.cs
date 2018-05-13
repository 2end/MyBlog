using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class RoleInitializer
    {
		public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			string adminEmail = "my@gmail.com";
			string password = "Lopik@12";
			if (await roleManager.FindByNameAsync("admin") == null)
			{
				await roleManager.CreateAsync(new IdentityRole("admin"));
			}
			if (await roleManager.FindByNameAsync("user") == null)
			{
				await roleManager.CreateAsync(new IdentityRole("user"));
			}
			if (await userManager.FindByEmailAsync(adminEmail) == null)
			{
				User admin = new User
				{
					UserName = "Admin",
					Email = adminEmail,
					EmailConfirmed = true
				};
				IdentityResult result = await userManager.CreateAsync(admin, password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, "admin");
				}
			}
		}
    }
}
