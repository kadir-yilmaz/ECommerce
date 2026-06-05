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
            if (await context.Categories.AnyAsync()) return;

            var categoriesTree = new Dictionary<string, List<string>>
            {
                { "Elektronik", new List<string> { "Telefon", "Laptop", "Kulaklık", "Mouse", "Klavye" } },
                { "Kozmetik", new List<string> { "Ruj", "Cilt Bakım" } },
                { "Süpermarket", new List<string> { "Gıda", "Temizlik", "Kişisel Bakım" } },
                { "Spor & Outdoor", new List<string> { "Fitness", "Kamp", "Bisiklet" } },
                { "Kadın", new List<string> { "Giyim", "Aksesuar & Çanta", "Takı", "Ayakkabı", "Gözlük", "Parfüm" } },
                { "Erkek", new List<string> { "Giyim", "Saat", "Cüzdan", "Ayakkabı", "Gözlük", "Parfüm" } }
            };

            foreach (var kvp in categoriesTree)
            {
                var rootCategory = new Category { Id = Guid.NewGuid(), Name = kvp.Key };
                await context.Categories.AddAsync(rootCategory);

                foreach (var subName in kvp.Value)
                {
                    var subCategory = new Category { Id = Guid.NewGuid(), Name = subName, ParentCategoryId = rootCategory.Id };
                    await context.Categories.AddAsync(subCategory);
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("[Seed Data] 7 Ana Kategori ve 21 Alt Kategori başarıyla tohumlandı.");
        }

        public static async Task SeedProductsAsync(ECommerceDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var productsToSeed = new List<Product>();
            var random = new Random(42);

            var subCats = await context.Categories.Where(c => c.ParentCategoryId != null).ToListAsync();

            var productMap = new Dictionary<string, List<(string Brand, string Name, float Price)>>
            {
                { "Telefon", new List<(string, string, float)> { 
                    ("Apple", "iPhone 15 Pro Max", 75000f), ("Samsung", "Galaxy S24 Ultra", 65000f), 
                    ("Apple", "iPhone 14 Pro", 55000f), ("Xiaomi", "14 Pro", 45000f), 
                    ("Samsung", "Galaxy A54", 15000f) 
                }},
                { "Laptop", new List<(string, string, float)> { 
                    ("Apple", "MacBook Pro M3 Max", 125000f), ("Apple", "MacBook Air M2", 35000f), 
                    ("Dell", "XPS 15", 60000f), ("Asus", "ROG Zephyrus G14", 55000f), 
                    ("Lenovo", "Legion 5", 42000f) 
                }},
                { "Kulaklık", new List<(string, string, float)> { 
                    ("Sony", "WH-1000XM5", 12000f), ("Apple", "AirPods Pro 2", 8500f), 
                    ("Sennheiser", "Momentum 4", 11500f), ("Bose", "QuietComfort Ultra", 14000f), 
                    ("Jabra", "Elite 8 Active", 6500f) 
                }},
                { "Mouse", new List<(string, string, float)> { 
                    ("Logitech", "MX Master 3S", 3500f), ("Razer", "DeathAdder V3 Pro", 4500f), 
                    ("SteelSeries", "Aerox 5", 2800f), ("Logitech", "G Pro X Superlight", 4800f), 
                    ("Corsair", "Katar Pro", 900f) 
                }},
                { "Klavye", new List<(string, string, float)> { 
                    ("Corsair", "K100 RGB", 7500f), ("Logitech", "G915 TKL", 6500f), 
                    ("SteelSeries", "Apex Pro", 8000f), ("Razer", "Huntsman V2", 6000f), 
                    ("Keychron", "K2", 3500f) 
                }},

                { "Cilt Bakım", new List<(string, string, float)> { 
                    ("La Roche-Posay", "Effaclar Jel", 650f), ("Cerave", "Nemlendirici Krem", 550f), 
                    ("The Ordinary", "Niacinamide", 450f), ("Vichy", "Mineral 89 Serum", 950f), 
                    ("Estee Lauder", "Advanced Night Repair", 3500f) 
                }},

                { "Gıda", new List<(string, string, float)> { 
                    ("Torku", "Toz Şeker 5 kg", 160f), ("Çaykur", "Rize Turist Çayı 1 kg", 155f), 
                    ("Yudum", "Ayçiçek Yağı 5 lt", 240f), ("Nutella", "Kakaolu Krem Çikolata 750g", 145f), 
                    ("Dardanel", "Ton Balığı 3x75g", 125f) 
                }},
                { "Temizlik", new List<(string, string, float)> { 
                    ("Ariel", "Dağ Esintisi Toz Deterjan 7 kg", 340f), ("Fairy", "Platinum Kapsül 60'lı", 480f), 
                    ("Domestos", "Çamaşır Suyu 3.2 lt", 135f), ("Papia", "İpek Özlü Tuvalet Kağıdı 32'li", 310f), 
                    ("Vernel", "Max Yumuşatıcı 1440ml", 110f) 
                }},
                { "Kişisel Bakım", new List<(string, string, float)> { 
                    ("Oral-B", "Pro 3 Şarjlı Diş Fırçası", 1200f), ("Colgate", "Optic White Diş Macunu", 95f), 
                    ("Gillette", "Fusion 5 ProGlide Yedek Bıçak", 450f), ("Clear Men", "Kepeğe Karşı Şampuan 500ml", 135f), 
                    ("Dove", "Güzellik Sabunu 4x90g", 85f) 
                }},

                { "Fitness", new List<(string, string, float)> { 
                    ("Decathlon", "Domyos 10 kg Dambıl Seti", 750f), ("Voit", "Kalın Pilates Matı", 450f), 
                    ("Nike", "Pro Antrenman Eldiveni", 550f), ("Delta", "3'lü Direnç Bandı Seti", 280f), 
                    ("Adler", "Sayaçlı Atlama İpi", 150f) 
                }},
                { "Kamp", new List<(string, string, float)> { 
                    ("Quechua", "Arpenaz 3 Kişilik Kamp Çadırı", 3200f), ("Stanley", "Klasik Termos 1 Litre", 2100f), 
                    ("Nurgaz", "Portatif Kamp Ocağı", 550f), ("Husky", "Mumya Tipi Uyku Tulumu", 3800f), 
                    ("Decathlon", "Katlanır Kamp Sandalyesi", 450f) 
                }},
                { "Bisiklet", new List<(string, string, float)> { 
                    ("Kron", "XC100 29 Jant Dağ Bisikleti", 9500f), ("Bianchi", "Touring Şehir Bisikleti", 10500f), 
                    ("Salcano", "Hector 26 Jant Dağ Bisikleti", 7500f), ("Bisan", "Katlanabilir Bisiklet", 8200f), 
                    ("Shimano", "Profesyonel Bisiklet Kaskı", 1400f) 
                }},

                // Kadın Kategorileri
                { "Kadın Giyim", new List<(string, string, float)> { 
                    ("Generic", "Basic Tişört", 350f), ("Generic", "Kot Pantolon", 850f), 
                    ("Generic", "Keten Gömlek", 600f), ("Generic", "Mevsimlik Mont", 1500f), 
                    ("Generic", "Spor Ceket", 1200f) 
                }},
                { "Kadın Aksesuar & Çanta", new List<(string, string, float)> { 
                    ("Guess", "Siyah Çapraz Çanta", 3800f), ("Vakko", "Monogram Omuz Çantası", 8500f), 
                    ("Beymen Club", "Keten Çanta", 2500f), ("Mango", "Deri Kemer", 450f), 
                    ("Zara", "Portföy Cüzdan", 650f) 
                }},
                { "Kadın Takı", new List<(string, string, float)> { 
                    ("Pandora", "Moments Gümüş Bileklik", 2800f), ("Atasay", "14 Ayar Altın Kolye", 7500f), 
                    ("Zen", "Pırlanta Tektaş Yüzük", 35000f), ("Swarovski", "Iconic Swan Küpe", 3200f), 
                    ("So Chic", "Çelik Halhal", 550f) 
                }},
                { "Kadın Ayakkabı", new List<(string, string, float)> { 
                    ("Nike", "Air Zoom Pegasus 40", 4500f), ("Adidas", "Ultraboost Light", 6000f), 
                    ("Nike", "Air Force 1 '07", 4200f), ("Vans", "Old Skool Siyah", 2500f), 
                    ("Derimod", "Siyah Stiletto", 1800f), ("Aldo", "Platform Topuklu", 2600f), 
                    ("Nine West", "Nude Topuklu", 2200f), ("Kemal Tanca", "Deri Abiye Ayakkabı", 3200f), 
                    ("Elle", "İnce Bantlı Topuklu", 1950f) 
                }},
                { "Kadın Gözlük", new List<(string, string, float)> { 
                    ("Ray-Ban", "Wayfarer Kadın", 3500f), ("Tom Ford", "Jennifer TF8", 6500f), 
                    ("Gucci", "Kadın Gözlük", 7200f), ("Dior", "DiorSoStellaire1 Kadın", 5800f), 
                    ("Oakley", "Frogskins Kadın", 2800f) 
                }},
                { "Kadın Parfüm", new List<(string, string, float)> { 
                    ("Chanel", "No5 100ml", 9500f), ("Dior", "Miss Dior Eau de Parfum 100ml", 7500f), 
                    ("Lancôme", "La Vie Est Belle 100ml", 6800f), ("Marc Jacobs", "Daisy 100ml", 5200f), 
                    ("Yves Saint Laurent", "Mon Paris 100ml", 6800f) 
                }},

                // Erkek Kategorileri
                { "Erkek Giyim", new List<(string, string, float)> { 
                    ("Generic", "Basic Tişört", 350f), ("Generic", "Kot Pantolon", 850f), 
                    ("Generic", "Keten Gömlek", 600f), ("Generic", "Mevsimlik Mont", 1500f), 
                    ("Generic", "Spor Ceket", 1200f) 
                }},
                { "Erkek Saat", new List<(string, string, float)> { 
                    ("Seiko", "5 Sports Automatic", 12000f), ("Tissot", "PRX Powermatic 80", 25000f), 
                    ("Casio", "G-Shock", 4500f), ("Fossil", "Gen 6 Smartwatch", 7500f), 
                    ("Tommy Hilfiger", "Deri Kordon Saat", 4200f) 
                }},
                { "Erkek Cüzdan", new List<(string, string, float)> { 
                    ("Kemal Tanca", "Hakiki Deri Cüzdan", 1400f), ("Derimod", "Siyah Kartlık", 650f), 
                    ("Tommy Hilfiger", "Erkek Cüzdan", 2100f), ("Vakko", "Logo Baskılı Deri Cüzdan", 2800f), 
                    ("Pierre Cardin", "Deri Kartlık", 950f) 
                }},
                { "Erkek Ayakkabı", new List<(string, string, float)> { 
                    ("Nike", "Air Zoom Pegasus 40", 4500f), ("Adidas", "Ultraboost Light", 6000f), 
                    ("Puma", "Deviate Nitro 2", 5200f), ("Under Armour", "HOVR", 4800f), 
                    ("Asics", "Gel-Kayano 30", 6500f), ("Nike", "Air Force 1 '07", 4200f), 
                    ("Converse", "Chuck Taylor All Star", 2800f), ("Adidas", "Stan Smith", 3800f), 
                    ("New Balance", "550", 4500f) 
                }},
                { "Erkek Gözlük", new List<(string, string, float)> { 
                    ("Ray-Ban", "Clubmaster Erkek", 3800f), ("Tom Ford", "James Bond", 6800f), 
                    ("Gucci", "Erkek Gözlük", 7500f), ("Dior", "Homme Erkek", 6200f), 
                    ("Oakley", "Holbrook Erkek", 3200f) 
                }},
                { "Erkek Parfüm", new List<(string, string, float)> { 
                    ("Dior", "Sauvage Eau de Toilette 100ml", 7000f), ("Versace", "Eros 100ml", 6200f), 
                    ("Hugo Boss", "Bottled 100ml", 5500f), ("Giorgio Armani", "Acqua di Gio 100ml", 5800f), 
                    ("Chanel", "Bleu de Chanel 100ml", 7500f) 
                }}
            };

            foreach (var category in subCats)
            {
                var lookupName = category.Name;

                if (productMap.TryGetValue(lookupName, out var productDefs))
                {
                    foreach (var pd in productDefs)
                    {
                        var product = new Product
                        {
                            Id = Guid.NewGuid(),
                            Brand = pd.Brand,
                            Name = pd.Name,
                            Price = pd.Price,
                            Stock = random.Next(10, 200),
                            CategoryId = category.Id
                        };
                        
                        productsToSeed.Add(product);
                    }
                }
            }

            if (productsToSeed.Any())
            {
                await context.Products.AddRangeAsync(productsToSeed);
                await context.SaveChangesAsync();
                Console.WriteLine($"[Seed Data] Toplam {productsToSeed.Count} adet ürün başarıyla tohumlandı.");
            }
        }

        public static async Task SeedRolesAndUsersAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ECommerceDbContext context,
            string adminEmail,
            string adminPassword)
        {
            const string legacyAdminEmail = "admin@gmail.com";
            adminEmail = string.IsNullOrWhiteSpace(adminEmail) ? legacyAdminEmail : adminEmail.Trim();
            adminPassword = string.IsNullOrWhiteSpace(adminPassword) ? "Admin+1234" : adminPassword;
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

            // Find or create configured admin user
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            var legacyAdminUser = string.Equals(adminEmail, legacyAdminEmail, StringComparison.OrdinalIgnoreCase)
                ? null
                : await userManager.FindByEmailAsync(legacyAdminEmail);

            if (adminUser == null && legacyAdminUser != null)
            {
                adminUser = legacyAdminUser;
                adminUser.EmailConfirmed = true;
                adminUser.NameSurname = "Kadir Yilmaz";

                await userManager.SetEmailAsync(adminUser, adminEmail);
                await userManager.SetUserNameAsync(adminUser, adminEmail);
                adminUser.PasswordHash = userManager.PasswordHasher.HashPassword(adminUser, adminPassword);
                await userManager.UpdateAsync(adminUser);

                Console.WriteLine($"[Seed Data] Legacy admin kullanicisi '{legacyAdminEmail}' -> '{adminEmail}' olarak guncellendi ve sifresi guncellendi.");
            }
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    NameSurname = "Kadir Yilmaz",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
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
