using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using ECommerce.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Persistence
{
    public static class DataSeeder
    {
        public static Task SeedCategoriesAsync(ECommerceDbContext context)
            => CategorySeeder.SeedAsync(context);

        public static Task SeedProductsAsync(ECommerceDbContext context)
            => ProductSeeder.SeedAsync(context);

        public static Task SeedCampaignsAsync(ECommerceDbContext context)
            => CampaignSeeder.SeedAsync(context);

        public static Task SeedCouponsAndRewardsAsync(ECommerceDbContext context)
            => CouponAndRewardSeeder.SeedAsync(context);

        public static Task SeedRolesAndUsersAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ECommerceDbContext context,
            string adminEmail,
            string adminPassword)
            => IdentitySeeder.SeedAsync(userManager, roleManager, context, adminEmail, adminPassword);
    }
}
