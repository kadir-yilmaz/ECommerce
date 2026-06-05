using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Seeders
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(ECommerceDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categoriesTree = new Dictionary<string, List<string>>
            {
                { "Elektronik", new List<string> { "Telefon", "Laptop", "Kulaklık", "Mouse", "Klavye" } },
                { "Kozmetik", new List<string> { "Ruj", "Cilt Bakım" } },
                { "Süpermarket", new List<string> { "Gıda", "Temizlik", "Kişisel Bakım" } },
                { "Spor & Outdoor", new List<string> { "Fitness", "Kamp", "Bisiklet" } },
                { "Kadın", new List<string> { "Kadın Giyim", "Kadın Aksesuar & Çanta", "Kadın Takı", "Kadın Ayakkabı", "Kadın Gözlük", "Kadın Parfüm" } },
                { "Erkek", new List<string> { "Erkek Giyim", "Erkek Saat", "Erkek Cüzdan", "Erkek Ayakkabı", "Erkek Gözlük", "Erkek Parfüm" } }
            };

            foreach (var kvp in categoriesTree)
            {
                var rootCategory = new Category { Id = Guid.NewGuid(), Name = kvp.Key };
                await context.Categories.AddAsync(rootCategory);

                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    var subName = kvp.Value[i];
                    var subCategory = new Category 
                    { 
                        Id = Guid.NewGuid(), 
                        Name = subName, 
                        ParentCategoryId = rootCategory.Id,
                        ShowOnHomepage = (i == 0) // Her ana kategorinin ilk alt kategorisini vitrine ekle
                    };
                    await context.Categories.AddAsync(subCategory);
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("[Seed Data] 7 Ana Kategori ve 21 Alt Kategori başarıyla tohumlandı.");
        }
    }
}
