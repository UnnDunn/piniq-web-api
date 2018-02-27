using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Pinball.Web.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string adminUsername, string adminSeedPW)
        {
            await EnsureUser(serviceProvider, adminUsername, adminSeedPW);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userName, string userPW)
        {
            var userManager = (UserManager<ApplicationUser>)serviceProvider.GetService(typeof(UserManager<ApplicationUser>));

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser() { UserName = userName };
                await userManager.CreateAsync(user, userPW);
            }

            return user.Id;
        }
    }
}