using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Seeders
{
    public static class CampaignSeeder
    {
        public static async Task SeedAsync(ECommerceDbContext context)
        {
            if (await context.Campaigns.AnyAsync()) return;

            // Kategorileri bul
            var telefonCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Telefon");
            var erkekParfumCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Erkek Parfüm");
            var kadinParfumCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Kadın Parfüm");
            var laptopCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Laptop");

            // Laptop için bir ürün seç (ilk laptop)
            var sampleLaptop = await context.Products
                .Where(p => p.Category.Name == "Laptop")
                .FirstOrDefaultAsync();

            var campaigns = new List<Campaign>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "500 TL Üzeri Bedava Kargo",
                    Description = "500 TL ve üzeri alışverişlerinizde kargo ücretsiz! Standart kargo ücreti 50 TL'dir.",
                    RuleType = "FreeShipping",
                    IsActive = true,
                    DiscountRate = 50m, // Standart kargo ücreti
                    MinAmount = 500m,    // Ücretsiz kargo için gereken minimum sepet tutarı
                    EndDate = DateTime.UtcNow.AddYears(5)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung Telefonlara %20 İndirim",
                    Description = "Samsung markalı tüm telefonlarda %20 indirim fırsatı!",
                    RuleType = "BrandDiscount",
                    IsActive = true,
                    Brand = "Samsung",
                    CategoryId = telefonCategory?.Id.ToString(),
                    DiscountRate = 20m,
                    EndDate = DateTime.UtcNow.AddMonths(3)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Erkek Parfümlerinde %10 İndirim",
                    Description = "Tüm erkek parfümlerinde %10 indirim!",
                    RuleType = "CategoryDiscount",
                    IsActive = true,
                    CategoryId = erkekParfumCategory?.Id.ToString(),
                    DiscountRate = 10m,
                    EndDate = DateTime.UtcNow.AddMonths(2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Kadın Parfümlerinde %10 İndirim",
                    Description = "Tüm kadın parfümlerinde %10 indirim!",
                    RuleType = "CategoryDiscount",
                    IsActive = true,
                    CategoryId = kadinParfumCategory?.Id.ToString(),
                    DiscountRate = 10m,
                    EndDate = DateTime.UtcNow.AddMonths(2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = sampleLaptop != null ? $"{sampleLaptop.Brand} {sampleLaptop.Name} %20 İndirimli!" : "Seçili Laptop'ta %20 İndirim",
                    Description = sampleLaptop != null ? $"{sampleLaptop.Brand} {sampleLaptop.Name} modelinde özel fırsat! %20 indirim!" : "Seçili laptop modelinde %20 indirim!",
                    RuleType = "SelectedProductsDiscount",
                    IsActive = true,
                    ProductId = sampleLaptop?.Id.ToString(),
                    DiscountRate = 20m,
                    EndDate = DateTime.UtcNow.AddMonths(1)
                }
            };

            await context.Campaigns.AddRangeAsync(campaigns);
            await context.SaveChangesAsync();
            Console.WriteLine($"[Seed Data] Toplam {campaigns.Count} adet kampanya başarıyla tohumlandı.");
        }
    }
}
