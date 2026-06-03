import sys

with open(r"D:\Kadir\Projeler\ECommerce\ECommerce.Persistence\DataSeeder.cs", "r", encoding="utf-8") as f:
    lines = f.readlines()

top = lines[:9]
bottom = lines[117:]

new_content = """        public static async Task SeedCategoriesAsync(ECommerceDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categoriesTree = new Dictionary<string, List<string>>
            {
                { "Elektronik", new List<string> { "Telefon", "Laptop", "Kulaklık", "Mouse", "Klavye" } },
                { "Kozmetik", new List<string> { "Parfüm", "Ruj", "Cilt Bakım" } },
                { "Ayakkabı", new List<string> { "Spor", "Gündelik", "Topuklu" } },
                { "Kadın", new List<string> { "Giyim", "Aksesuar & Çanta", "Takı" } },
                { "Erkek", new List<string> { "Giyim", "Saat", "Cüzdan" } },
                { "Süpermarket", new List<string> { "Gıda", "Temizlik", "Kişisel Bakım" } },
                { "Spor & Outdoor", new List<string> { "Fitness", "Kamp", "Bisiklet" } }
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

            var productMap = new Dictionary<string, List<(string Name, float Price)>>
            {
                { "Telefon", new List<(string, float)> { 
                    ("iPhone 15 Pro Max", 75000f), ("Samsung Galaxy S24 Ultra", 65000f), 
                    ("iPhone 14 Pro", 55000f), ("Xiaomi 14 Pro", 45000f), 
                    ("Samsung Galaxy A54", 15000f) 
                }},
                { "Laptop", new List<(string, float)> { 
                    ("MacBook Pro M3 Max", 125000f), ("MacBook Air M2", 35000f), 
                    ("Dell XPS 15", 60000f), ("Asus ROG Zephyrus G14", 55000f), 
                    ("Lenovo Legion 5", 42000f) 
                }},
                { "Kulaklık", new List<(string, float)> { 
                    ("Sony WH-1000XM5", 12000f), ("AirPods Pro 2", 8500f), 
                    ("Sennheiser Momentum 4", 11500f), ("Bose QuietComfort Ultra", 14000f), 
                    ("Jabra Elite 8 Active", 6500f) 
                }},
                { "Mouse", new List<(string, float)> { 
                    ("Logitech MX Master 3S", 3500f), ("Razer DeathAdder V3 Pro", 4500f), 
                    ("SteelSeries Aerox 5", 2800f), ("Logitech G Pro X Superlight", 4800f), 
                    ("Corsair Katar Pro", 900f) 
                }},
                { "Klavye", new List<(string, float)> { 
                    ("Corsair K100 RGB", 7500f), ("Logitech G915 TKL", 6500f), 
                    ("SteelSeries Apex Pro", 8000f), ("Razer Huntsman V2", 6000f), 
                    ("Keychron K2", 3500f) 
                }},

                { "Parfüm", new List<(string, float)> { 
                    ("Tom Ford Black Orchid 100ml", 8500f), ("Dior Sauvage Elixir 60ml", 6500f), 
                    ("Chanel Bleu De Chanel 100ml", 5500f), ("Yves Saint Laurent Libre 90ml", 6000f), 
                    ("Versace Eros 100ml", 3500f) 
                }},
                { "Ruj", new List<(string, float)> { 
                    ("MAC Ruby Woo Matte", 950f), ("Dior Rouge 999", 1600f), 
                    ("Charlotte Tilbury Pillow Talk", 1400f), ("YSL Rouge Pur Couture", 1500f), 
                    ("Maybelline SuperStay Matte Ink", 350f) 
                }},
                { "Cilt Bakım", new List<(string, float)> { 
                    ("La Roche-Posay Effaclar Jel", 650f), ("Cerave Nemlendirici Krem", 550f), 
                    ("The Ordinary Niacinamide", 450f), ("Vichy Mineral 89 Serum", 950f), 
                    ("Estee Lauder Advanced Night Repair", 3500f) 
                }},

                { "Spor", new List<(string, float)> { 
                    ("Nike Air Zoom Pegasus 40", 4500f), ("Adidas Ultraboost Light", 6000f), 
                    ("Puma Deviate Nitro 2", 5200f), ("Under Armour HOVR", 4800f), 
                    ("Asics Gel-Kayano 30", 6500f) 
                }},
                { "Gündelik", new List<(string, float)> { 
                    ("Nike Air Force 1 '07", 4200f), ("Vans Old Skool Siyah", 2500f), 
                    ("Converse Chuck Taylor All Star", 2800f), ("Adidas Stan Smith", 3800f), 
                    ("New Balance 550", 4500f) 
                }},
                { "Topuklu", new List<(string, float)> { 
                    ("Derimod Siyah Stiletto", 1800f), ("Aldo Platform Topuklu", 2600f), 
                    ("Nine West Nude Topuklu", 2200f), ("Kemal Tanca Deri Abiye Ayakkabı", 3200f), 
                    ("Elle İnce Bantlı Topuklu", 1950f) 
                }},

                { "Giyim", new List<(string, float)> { 
                    ("Basic Tişört", 350f), ("Kot Pantolon", 850f), 
                    ("Keten Gömlek", 600f), ("Mevsimlik Mont", 1500f), 
                    ("Spor Ceket", 1200f) 
                }},
                { "Aksesuar & Çanta", new List<(string, float)> { 
                    ("Guess Siyah Çapraz Çanta", 3800f), ("Vakko Monogram Omuz Çantası", 8500f), 
                    ("Beymen Club Keten Çanta", 2500f), ("Mango Deri Kemer", 450f), 
                    ("Zara Portföy Cüzdan", 650f) 
                }},
                { "Takı", new List<(string, float)> { 
                    ("Pandora Moments Gümüş Bileklik", 2800f), ("Atasay 14 Ayar Altın Kolye", 7500f), 
                    ("Zen Pırlanta Tektaş Yüzük", 35000f), ("Swarovski Iconic Swan Küpe", 3200f), 
                    ("So Chic Çelik Halhal", 550f) 
                }},

                { "Saat", new List<(string, float)> { 
                    ("Seiko 5 Sports Automatic", 12000f), ("Tissot PRX Powermatic 80", 25000f), 
                    ("Casio G-Shock", 4500f), ("Fossil Gen 6 Smartwatch", 7500f), 
                    ("Tommy Hilfiger Deri Kordon Saat", 4200f) 
                }},
                { "Cüzdan", new List<(string, float)> { 
                    ("Kemal Tanca Hakiki Deri Cüzdan", 1400f), ("Derimod Siyah Kartlık", 650f), 
                    ("Tommy Hilfiger Erkek Cüzdan", 2100f), ("Vakko Logo Baskılı Deri Cüzdan", 2800f), 
                    ("Pierre Cardin Deri Kartlık", 950f) 
                }},

                { "Gıda", new List<(string, float)> { 
                    ("Torku Toz Şeker 5 kg", 160f), ("Çaykur Rize Turist Çayı 1 kg", 155f), 
                    ("Yudum Ayçiçek Yağı 5 lt", 240f), ("Nutella Kakaolu Krem Çikolata 750g", 145f), 
                    ("Dardanel Ton Balığı 3x75g", 125f) 
                }},
                { "Temizlik", new List<(string, float)> { 
                    ("Ariel Dağ Esintisi Toz Deterjan 7 kg", 340f), ("Fairy Platinum Kapsül 60'lı", 480f), 
                    ("Domestos Çamaşır Suyu 3.2 lt", 135f), ("Papia İpek Özlü Tuvalet Kağıdı 32'li", 310f), 
                    ("Vernel Max Yumuşatıcı 1440ml", 110f) 
                }},
                { "Kişisel Bakım", new List<(string, float)> { 
                    ("Oral-B Pro 3 Şarjlı Diş Fırçası", 1200f), ("Colgate Optic White Diş Macunu", 95f), 
                    ("Gillette Fusion 5 ProGlide Yedek Bıçak", 450f), ("Clear Men Kepeğe Karşı Şampuan 500ml", 135f), 
                    ("Dove Güzellik Sabunu 4x90g", 85f) 
                }},

                { "Fitness", new List<(string, float)> { 
                    ("Decathlon Domyos 10 kg Dambıl Seti", 750f), ("Voit Kalın Pilates Matı", 450f), 
                    ("Nike Pro Antrenman Eldiveni", 550f), ("Delta 3'lü Direnç Bandı Seti", 280f), 
                    ("Adler Sayaçlı Atlama İpi", 150f) 
                }},
                { "Kamp", new List<(string, float)> { 
                    ("Quechua Arpenaz 3 Kişilik Kamp Çadırı", 3200f), ("Stanley Klasik Termos 1 Litre", 2100f), 
                    ("Nurgaz Portatif Kamp Ocağı", 550f), ("Husky Mumya Tipi Uyku Tulumu", 3800f), 
                    ("Decathlon Katlanır Kamp Sandalyesi", 450f) 
                }},
                { "Bisiklet", new List<(string, float)> { 
                    ("Kron XC100 29 Jant Dağ Bisikleti", 9500f), ("Bianchi Touring Şehir Bisikleti", 10500f), 
                    ("Salcano Hector 26 Jant Dağ Bisikleti", 7500f), ("Bisan Katlanabilir Bisiklet", 8200f), 
                    ("Shimano Profesyonel Bisiklet Kaskı", 1400f) 
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
                            Name = pd.Name,
                            Price = pd.Price,
                            Stock = random.Next(10, 200),
                            CategoryId = category.Id
                        };
                        
                        if (lookupName == "Giyim")
                        {
                            var parent = await context.Categories.FindAsync(category.ParentCategoryId);
                            product.Name = $"{parent?.Name} {pd.Name}";
                        }
                        
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
        }\n"""

with open(r"D:\Kadir\Projeler\ECommerce\ECommerce.Persistence\DataSeeder.cs", "w", encoding="utf-8") as f:
    f.writelines(top)
    f.write(new_content)
    f.writelines(bottom)
