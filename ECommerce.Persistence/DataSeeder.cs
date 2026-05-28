using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedCategoriesAsync(ECommerceDbContext context)
        {
            if (context.Categories.Any()) return;

            // 1. Ana Kategoriler
            var elektronik = new Category { Id = Guid.NewGuid(), Name = "Elektronik" };
            var kozmetik = new Category { Id = Guid.NewGuid(), Name = "Kozmetik" };
            var ayakkabi = new Category { Id = Guid.NewGuid(), Name = "Ayakkabı" };
            var kadin = new Category { Id = Guid.NewGuid(), Name = "Kadın" };
            var erkek = new Category { Id = Guid.NewGuid(), Name = "Erkek" };
            var superMarket = new Category { Id = Guid.NewGuid(), Name = "Süpermarket" };
            var sporOutdoor = new Category { Id = Guid.NewGuid(), Name = "Spor & Outdoor" };

            // 2. Elektronik Alt Kategorileri
            var telefon = new Category { Id = Guid.NewGuid(), Name = "Telefon", ParentCategoryId = elektronik.Id };
            var laptop = new Category { Id = Guid.NewGuid(), Name = "Laptop", ParentCategoryId = elektronik.Id };
            var kulaklik = new Category { Id = Guid.NewGuid(), Name = "Kulaklık", ParentCategoryId = elektronik.Id };
            var mouse = new Category { Id = Guid.NewGuid(), Name = "Mouse", ParentCategoryId = elektronik.Id };
            var klavye = new Category { Id = Guid.NewGuid(), Name = "Klavye", ParentCategoryId = elektronik.Id };

            // 3. Kozmetik Alt Kategorileri
            var parfum = new Category { Id = Guid.NewGuid(), Name = "Parfüm", ParentCategoryId = kozmetik.Id };
            var ruj = new Category { Id = Guid.NewGuid(), Name = "Ruj", ParentCategoryId = kozmetik.Id };
            var ciltBakim = new Category { Id = Guid.NewGuid(), Name = "Cilt Bakım", ParentCategoryId = kozmetik.Id };

            // 4. Ayakkabı Alt Kategorileri
            var spor = new Category { Id = Guid.NewGuid(), Name = "Spor", ParentCategoryId = ayakkabi.Id };
            var gundelik = new Category { Id = Guid.NewGuid(), Name = "Gündelik", ParentCategoryId = ayakkabi.Id };
            var topuklu = new Category { Id = Guid.NewGuid(), Name = "Topuklu Ayakkabı", ParentCategoryId = ayakkabi.Id };

            // 5. Kadın Alt Kategorileri
            var kadinGiyim = new Category { Id = Guid.NewGuid(), Name = "Giyim", ParentCategoryId = kadin.Id };
            var kadinCanta = new Category { Id = Guid.NewGuid(), Name = "Aksesuar & Çanta", ParentCategoryId = kadin.Id };

            // 6. Erkek Alt Kategorileri
            var erkekGiyim = new Category { Id = Guid.NewGuid(), Name = "Giyim", ParentCategoryId = erkek.Id };
            var erkekSaat = new Category { Id = Guid.NewGuid(), Name = "Saat & Aksesuar", ParentCategoryId = erkek.Id };

            var categories = new List<Category>
            {
                elektronik, kozmetik, ayakkabi, kadin, erkek, superMarket, sporOutdoor,
                telefon, laptop, kulaklik, mouse, klavye,
                parfum, ruj, ciltBakim,
                spor, gundelik, topuklu,
                kadinGiyim, kadinCanta,
                erkekGiyim, erkekSaat
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine("[Seed Data] Kategoriler başarıyla tohumlandı.");
        }

        public static async Task SeedProductsAsync(ECommerceDbContext context)
        {
            var telefonCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Telefon");
            var laptopCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Laptop");
            var kulaklikCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Kulaklık");
            var mouseCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Mouse");
            var klavyeCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Klavye");
            var parfumCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Parfüm");
            var sporCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Spor");

            if (context.Products.Any())
            {
                // Var olan ürünlerin CategoryId'si boşsa eşleştir
                var existingProducts = await context.Products.Where(p => p.CategoryId == null).ToListAsync();
                if (existingProducts.Any())
                {
                    foreach (var p in existingProducts)
                    {
                        if (p.Name.Contains("iPhone") || p.Name.Contains("Galaxy"))
                            p.CategoryId = telefonCat?.Id;
                        else if (p.Name.Contains("MacBook") || p.Name.Contains("Dell") || p.Name.Contains("XPS"))
                            p.CategoryId = laptopCat?.Id;
                        else if (p.Name.Contains("Sony") || p.Name.Contains("WH-"))
                            p.CategoryId = kulaklikCat?.Id;
                        else if (p.Name.Contains("Logitech") || p.Name.Contains("MX"))
                            p.CategoryId = mouseCat?.Id;
                        else if (p.Name.Contains("Corsair") || p.Name.Contains("K100"))
                            p.CategoryId = klavyeCat?.Id;
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("[Seed Data] Mevcut ürünler ilgili kategorilerle eşleştirildi.");
                }
                return;
            }

            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "iPhone 15 Pro", Stock = 50, Price = 59999, CategoryId = telefonCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Samsung Galaxy S24", Stock = 75, Price = 44999, CategoryId = telefonCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "MacBook Air M3", Stock = 30, Price = 49999, CategoryId = laptopCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Sony WH-1000XM5", Stock = 100, Price = 9999, CategoryId = kulaklikCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Logitech MX Master 3S", Stock = 150, Price = 2999, CategoryId = mouseCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Corsair K100 Keyboard", Stock = 80, Price = 6999, CategoryId = klavyeCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Dell XPS 15", Stock = 25, Price = 42999, CategoryId = laptopCat?.Id },
                
                // Cosmetics & Shoes
                new() { Id = Guid.NewGuid(), Name = "Chanel Bleu De Chanel EDP 100 ml", Stock = 40, Price = 4500, CategoryId = parfumCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Nike Air Max Pulse Spor Ayakkabı", Stock = 65, Price = 5200, CategoryId = sporCat?.Id },
                new() { Id = Guid.NewGuid(), Name = "Adidas Ultraboost Light", Stock = 55, Price = 6100, CategoryId = sporCat?.Id }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine("[Seed Data] Ürünler başarıyla tohumlandı.");
        }

        public static async Task SeedRolesAndUsersAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ECommerceDbContext context)
        {
            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new AppRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
                await roleManager.CreateAsync(adminRole);
                Console.WriteLine("[Seed Data] Admin rolü oluşturuldu.");
            }

            // Create User role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("User"))
            {
                var userRole = new AppRole { Id = Guid.NewGuid().ToString(), Name = "User" };
                await roleManager.CreateAsync(userRole);
                Console.WriteLine("[Seed Data] User rolü oluşturuldu.");
            }

            // Create Editor role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Editor"))
            {
                var editorRole = new AppRole { Id = Guid.NewGuid().ToString(), Name = "Editor" };
                await roleManager.CreateAsync(editorRole);
                Console.WriteLine("[Seed Data] Editor rolü oluşturuldu.");
            }

            // Assign "User" role to all existing users who have no roles (Fix for existing users)
            var usersWithoutRoles = userManager.Users.ToList();
            foreach (var user in usersWithoutRoles)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    await userManager.AddToRoleAsync(user, "User");
                    Console.WriteLine($"[Seed Data] '{user.UserName}' kullanıcısına varsayılan 'User' rolü atandı.");
                }
            }

            // Find or create admin user
            var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    NameSurname = "Admin User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin+1234");
                if (result.Succeeded)
                {
                    Console.WriteLine("[Seed Data] Admin kullanıcısı oluşturuldu.");
                }
            }

            // Ensure admin user has Admin role
            if (adminUser != null)
            {
                var userRoles = await userManager.GetRolesAsync(adminUser);
                if (!userRoles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("[Seed Data] Admin kullanıcısına Admin rolü tanımlandı.");
                }
            }

            // Link "User" role to necessary order endpoints
            try 
            {
                var userRole = await roleManager.FindByNameAsync("User");
                if (userRole != null)
                {
                    var ordersMenu = await context.Menus.FirstOrDefaultAsync(m => m.Name == "Orders");
                    if (ordersMenu == null)
                    {
                        ordersMenu = new Menu { Id = Guid.NewGuid(), Name = "Orders" };
                        await context.Menus.AddAsync(ordersMenu);
                        await context.SaveChangesAsync();
                    }

                    var seedRequiredEndpoints = new List<(string Code, string ActionType, string HttpType, string Definition)>
                    {
                        ("POST.Writing.CreateOrder", "Writing", "POST", "Create Order"),
                        ("GET.Reading.GetOrdersByUser", "Reading", "GET", "Get Orders By User"),
                        ("GET.Reading.GetOrderById", "Reading", "GET", "Get Order By Id")
                    };

                    foreach (var seed in seedRequiredEndpoints)
                    {
                        var endpoint = await context.Endpoints.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == seed.Code);
                        if (endpoint == null)
                        {
                            endpoint = new Endpoint
                            {
                                Id = Guid.NewGuid(),
                                Code = seed.Code,
                                ActionType = seed.ActionType,
                                HttpType = seed.HttpType,
                                Definition = seed.Definition,
                                Menu = ordersMenu
                            };
                            await context.Endpoints.AddAsync(endpoint);
                            await context.SaveChangesAsync();
                        }

                        if (!endpoint.Roles.Any(r => r.Name == "User"))
                        {
                            endpoint.Roles.Add(userRole);
                            Console.WriteLine($"[Seed Data] '{endpoint.Code}' yetkisi 'User' rolüne tanımlandı.");
                        }
                    }
                    await context.SaveChangesAsync();
                }
            } 
            catch (Exception ex) 
            {
                Console.WriteLine($"[Seed Data] User yetki eşleştirme sırasında hata: {ex.Message}");
            }

            // Link "Editor" role to necessary product endpoints
            try 
            {
                var editorRole = await roleManager.FindByNameAsync("Editor");
                if (editorRole != null)
                {
                    var productsMenu = await context.Menus.FirstOrDefaultAsync(m => m.Name == "Products");
                    if (productsMenu == null)
                    {
                        productsMenu = new Menu { Id = Guid.NewGuid(), Name = "Products" };
                        await context.Menus.AddAsync(productsMenu);
                        await context.SaveChangesAsync();
                    }

                    var editorRequiredEndpoints = new List<(string Code, string ActionType, string HttpType, string Definition)>
                    {
                        ("POST.Writing.CreateProduct", "Writing", "POST", "Create Product"),
                        ("PUT.Updating.UpdateProduct", "Updating", "PUT", "Update Product"),
                        ("DELETE.Deleting.DeleteProduct", "Deleting", "DELETE", "Delete Product"),
                        ("POST.Writing.UploadProductImage", "Writing", "POST", "Upload Product Image"),
                        ("DELETE.Deleting.DeleteProductImage", "Deleting", "DELETE", "Delete Product Image"),
                        ("GET.Updating.ChangeShowcaseImage", "Updating", "GET", "Change Showcase Image")
                    };

                    foreach (var seed in editorRequiredEndpoints)
                    {
                        var endpoint = await context.Endpoints.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == seed.Code);
                        if (endpoint == null)
                        {
                            endpoint = new Endpoint
                            {
                                Id = Guid.NewGuid(),
                                Code = seed.Code,
                                ActionType = seed.ActionType,
                                HttpType = seed.HttpType,
                                Definition = seed.Definition,
                                Menu = productsMenu
                            };
                            await context.Endpoints.AddAsync(endpoint);
                            await context.SaveChangesAsync();
                        }

                        if (!endpoint.Roles.Any(r => r.Name == "Editor"))
                        {
                            endpoint.Roles.Add(editorRole);
                            Console.WriteLine($"[Seed Data] '{endpoint.Code}' yetkisi 'Editor' rolüne tanımlandı.");
                        }
                    }
                    await context.SaveChangesAsync();
                }
            } 
            catch (Exception ex) 
            {
                Console.WriteLine($"[Seed Data] Editor yetki eşleştirme sırasında hata: {ex.Message}");
            }
        }
    }
}
