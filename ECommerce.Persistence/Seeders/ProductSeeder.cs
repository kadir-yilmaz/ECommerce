using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Seeders
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(ECommerceDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var productsToSeed = new List<Product>();
            var random = new Random(42);

            var subCats = await context.Categories
                .Where(c => c.ParentCategoryId != null)
                .ToListAsync();

            var productMap = BuildProductMap();
            int seededPhonesCount = 0;

            foreach (var category in subCats)
            {
                if (productMap.TryGetValue(category.Name, out var productDefs))
                {
                    foreach (var pd in productDefs)
                    {
                        bool showOnHomepage = false;
                        if (category.Name == "Telefon" && seededPhonesCount < 10)
                        {
                            showOnHomepage = true;
                            seededPhonesCount++;
                        }

                        productsToSeed.Add(new Product
                        {
                            Id = Guid.NewGuid(),
                            Brand = pd.Brand,
                            Name = pd.Name,
                            Price = pd.Price,
                            Stock = random.Next(10, 200),
                            CategoryId = category.Id,
                            ShowOnHomepage = showOnHomepage
                        });
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

        private static Dictionary<string, List<(string Brand, string Name, float Price)>> BuildProductMap()
        {
            return new Dictionary<string, List<(string Brand, string Name, float Price)>>
            {
                // ═══════════════════════════════════════════════════════════════
                // ELEKTRONİK
                // ═══════════════════════════════════════════════════════════════

                { "Telefon", new()
                {
                    ("Apple", "iPhone 15 Pro Max 256GB", 74999f),
                    ("Apple", "iPhone 15 Pro 128GB", 64999f),
                    ("Apple", "iPhone 15 128GB", 49999f),
                    ("Apple", "iPhone 14 128GB", 37999f),
                    ("Samsung", "Galaxy S24 Ultra 256GB", 69999f),
                    ("Samsung", "Galaxy S24+ 256GB", 54999f),
                    ("Samsung", "Galaxy S24 128GB", 44999f),
                    ("Samsung", "Galaxy A55 128GB", 16999f),
                    ("Samsung", "Galaxy A35 128GB", 12999f),
                    ("Samsung", "Galaxy A15 64GB", 7499f),
                    ("Xiaomi", "14 Ultra 512GB", 49999f),
                    ("Xiaomi", "14 Pro 256GB", 39999f),
                    ("Xiaomi", "Redmi Note 13 Pro+ 256GB", 14999f),
                    ("Xiaomi", "Redmi Note 13 128GB", 9999f),
                    ("Google", "Pixel 8 Pro 128GB", 39999f),
                    ("Google", "Pixel 8 128GB", 29999f),
                    ("OnePlus", "12 256GB", 34999f),
                    ("Oppo", "Reno 11 Pro 256GB", 22999f),
                    ("Huawei", "P60 Pro 256GB", 32999f),
                    ("Nothing", "Phone 2 256GB", 24999f),
                }},

                { "Laptop", new()
                {
                    ("Apple", "MacBook Pro 16\" M3 Max 36GB", 129999f),
                    ("Apple", "MacBook Pro 14\" M3 Pro 18GB", 79999f),
                    ("Apple", "MacBook Air 15\" M3 16GB", 54999f),
                    ("Apple", "MacBook Air 13\" M3 8GB", 42999f),
                    ("Dell", "XPS 15 i7 16GB 512GB", 62999f),
                    ("Dell", "XPS 13 Plus i7 16GB", 44999f),
                    ("Dell", "Inspiron 16 i5 8GB", 24999f),
                    ("Lenovo", "ThinkPad X1 Carbon Gen 11 i7", 72999f),
                    ("Lenovo", "Legion Pro 5 RTX 4070", 54999f),
                    ("Lenovo", "IdeaPad Slim 5 Ryzen 5", 22999f),
                    ("Asus", "ROG Zephyrus G16 RTX 4080", 69999f),
                    ("Asus", "ROG Strix G15 RTX 4060", 44999f),
                    ("Asus", "ZenBook 14 OLED Ryzen 7", 34999f),
                    ("Asus", "VivoBook 15 i5 8GB", 17999f),
                    ("HP", "Spectre x360 14 i7 OLED", 52999f),
                    ("HP", "Pavilion 15 i5 8GB", 19999f),
                    ("MSI", "Raider GE78 HX RTX 4090", 89999f),
                    ("MSI", "Katana 15 RTX 4060", 34999f),
                    ("Acer", "Predator Helios 16 RTX 4070", 49999f),
                    ("Huawei", "MateBook X Pro i7 16GB", 44999f),
                }},

                { "Kulaklık", new()
                {
                    ("Sony", "WH-1000XM5 Wireless ANC", 12999f),
                    ("Sony", "WF-1000XM5 TWS ANC", 10999f),
                    ("Sony", "WH-1000XM4 Wireless ANC", 8999f),
                    ("Apple", "AirPods Pro 2 USB-C", 9499f),
                    ("Apple", "AirPods 3. Nesil", 6499f),
                    ("Apple", "AirPods Max USB-C", 18999f),
                    ("Bose", "QuietComfort Ultra Headphones", 14999f),
                    ("Bose", "QuietComfort 45", 9999f),
                    ("Sennheiser", "Momentum 4 Wireless", 11999f),
                    ("Sennheiser", "HD 660S2 Open-Back", 15999f),
                    ("JBL", "Tour One M2 ANC", 7499f),
                    ("JBL", "Tune 770NC Wireless", 3499f),
                    ("JBL", "Live Pro 2 TWS", 4999f),
                    ("Samsung", "Galaxy Buds2 Pro", 5999f),
                    ("Jabra", "Elite 10 TWS ANC", 8999f),
                    ("Marshall", "Major IV Bluetooth", 4499f),
                    ("Marshall", "Monitor II ANC", 7999f),
                    ("Beats", "Studio Pro Wireless", 9999f),
                    ("HyperX", "Cloud III Wireless Gaming", 4999f),
                    ("Razer", "BlackShark V2 Pro Wireless", 5499f),
                }},

                { "Mouse", new()
                {
                    ("Logitech", "MX Master 3S Wireless", 3799f),
                    ("Logitech", "G Pro X Superlight 2", 5499f),
                    ("Logitech", "G502 X Plus Wireless", 5999f),
                    ("Logitech", "G305 Lightspeed", 1499f),
                    ("Razer", "DeathAdder V3 Pro Wireless", 4999f),
                    ("Razer", "Viper V3 Pro Wireless", 6499f),
                    ("Razer", "Basilisk V3 Wired", 2499f),
                    ("Razer", "Orochi V2 Bluetooth", 1999f),
                    ("SteelSeries", "Aerox 5 Wireless", 3499f),
                    ("SteelSeries", "Prime Wireless", 3999f),
                    ("Corsair", "Dark Core RGB Pro SE", 3299f),
                    ("Corsair", "Katar Pro XT Ultra-Light", 999f),
                    ("Zowie", "EC2-CW Wireless Esports", 3999f),
                    ("Zowie", "FK2-C Wired Esports", 2499f),
                    ("Glorious", "Model O 2 Wireless", 3299f),
                    ("Pulsar", "X2V2 Wireless", 3199f),
                    ("Endgame Gear", "OP1we Wireless", 2799f),
                    ("HyperX", "Pulsefire Haste 2 Wireless", 2199f),
                    ("Fnatic", "Bolt Wireless", 2999f),
                    ("Lamzu", "Atlantis Mini Wireless", 3499f),
                }},

                { "Klavye", new()
                {
                    ("Corsair", "K100 RGB OPX Optical", 7999f),
                    ("Corsair", "K70 RGB Pro Cherry MX", 5499f),
                    ("Corsair", "K65 Plus Wireless 75%", 4999f),
                    ("Logitech", "G915 X TKL Wireless", 7499f),
                    ("Logitech", "G Pro X TKL Lightspeed", 5999f),
                    ("Logitech", "G413 SE Mechanical", 1799f),
                    ("Razer", "Huntsman V3 Pro Analog", 8999f),
                    ("Razer", "BlackWidow V4 75%", 6499f),
                    ("Razer", "Ornata V3 Mecha-Membrane", 2999f),
                    ("SteelSeries", "Apex Pro TKL Wireless", 7999f),
                    ("SteelSeries", "Apex 3 TKL RGB", 2499f),
                    ("Keychron", "Q1 Max QMK/VIA 75%", 5999f),
                    ("Keychron", "K8 Pro TKL Wireless", 3999f),
                    ("Keychron", "V1 QMK 75% Wired", 2499f),
                    ("HyperX", "Alloy Origins 65 Compact", 3499f),
                    ("HyperX", "Alloy Rise 75 Wireless", 4999f),
                    ("Ducky", "One 3 TKL Cherry MX", 4499f),
                    ("ASUS ROG", "Azoth Wireless 75%", 6999f),
                    ("Wooting", "60HE Analog Rapid Trigger", 5499f),
                    ("Cherry", "MX Board 3.0S RGB", 2999f),
                }},

                // ═══════════════════════════════════════════════════════════════
                // KOZMETİK
                // ═══════════════════════════════════════════════════════════════

                { "Ruj", new()
                {
                    ("MAC", "Matte Lipstick Ruby Woo", 850f),
                    ("MAC", "Retro Matte Liquid Lip", 950f),
                    ("MAC", "Lustreglass Sheer Shine", 750f),
                    ("Maybelline", "SuperStay Matte Ink", 350f),
                    ("Maybelline", "Color Sensational Satin", 280f),
                    ("Maybelline", "Lifter Gloss", 320f),
                    ("L'Oréal Paris", "Color Riche Satin", 420f),
                    ("L'Oréal Paris", "Infaillible Matte Resistance", 450f),
                    ("NYX", "Lip Lingerie XXL Liquid", 380f),
                    ("NYX", "Soft Matte Lip Cream", 320f),
                    ("Dior", "Rouge Dior Satin 999", 2200f),
                    ("Dior", "Addict Lip Glow", 1800f),
                    ("Charlotte Tilbury", "Matte Revolution Pillow Talk", 1500f),
                    ("Charlotte Tilbury", "Lip Cheat Lip Liner", 1100f),
                    ("Chanel", "Rouge Allure Velvet", 2400f),
                    ("YSL", "Rouge Pur Couture", 1900f),
                    ("NARS", "Powermatte Lipstick", 1350f),
                    ("Bobbi Brown", "Crushed Lip Color", 1200f),
                    ("Huda Beauty", "Power Bullet Matte", 1100f),
                    ("Rare Beauty", "Soft Pinch Tinted Lip Oil", 950f),
                }},

                { "Cilt Bakım", new()
                {
                    ("La Roche-Posay", "Effaclar Duo+ Bakım Kremi", 750f),
                    ("La Roche-Posay", "Anthelios UVMune SPF50+ 50ml", 650f),
                    ("CeraVe", "Nemlendirici Krem 473ml", 580f),
                    ("CeraVe", "Foaming Cleanser 236ml", 420f),
                    ("The Ordinary", "Niacinamide 10% + Zinc 1%", 350f),
                    ("The Ordinary", "AHA 30% + BHA 2% Peeling", 380f),
                    ("The Ordinary", "Hyaluronic Acid 2% + B5", 320f),
                    ("Vichy", "Minéral 89 Hyalüronik Asit Serum 50ml", 950f),
                    ("Vichy", "Normaderm Phytosolution Jel 50ml", 680f),
                    ("Estée Lauder", "Advanced Night Repair Serum 50ml", 3800f),
                    ("Clinique", "Dramatically Different Jel 125ml", 1200f),
                    ("Clinique", "Moisture Surge 72-Hour 75ml", 1450f),
                    ("Bioderma", "Sensibio H2O Misel Su 500ml", 480f),
                    ("Bioderma", "Sébium Global Bakım Kremi", 550f),
                    ("Avène", "Cicalfate+ Onarıcı Krem 40ml", 520f),
                    ("Neutrogena", "Hydro Boost Aqua Jel Krem", 380f),
                    ("Kiehl's", "Ultra Facial Cream 50ml", 1600f),
                    ("Paula's Choice", "2% BHA Liquid Exfoliant", 850f),
                    ("Garnier", "C Vitamini Parlak Aydınlatıcı Serum", 280f),
                    ("Nivea", "Q10 Anti-Wrinkle Power Krem 50ml", 320f),
                }},

                // ═══════════════════════════════════════════════════════════════
                // SÜPERMARKET
                // ═══════════════════════════════════════════════════════════════

                { "Gıda", new()
                {
                    ("Torku", "Toz Şeker 5 kg", 165f),
                    ("Çaykur", "Rize Turist Çayı 1 kg", 155f),
                    ("Yudum", "Ayçiçek Yağı 5 lt", 245f),
                    ("Nutella", "Kakaolu Fındık Kreması 750g", 149f),
                    ("Dardanel", "Ton Balığı 3x75g", 125f),
                    ("Barilla", "Spaghetti No.5 500g", 55f),
                    ("Ülker", "Çikolatalı Gofret 5'li Paket", 85f),
                    ("Eti", "Burçak Bisküvi 3'lü Paket", 45f),
                    ("Pınar", "Tam Yağlı Süt 1 lt 4'lü", 110f),
                    ("Sana", "Margarin Kase 250g", 55f),
                    ("Doğuş", "Filiz Çay 1 kg", 135f),
                    ("Nescafé", "Gold Kahve 200g", 350f),
                    ("Bağdat", "Pul Biber 1 kg", 195f),
                    ("Knorr", "Kremalı Mantar Çorbası 12'li", 180f),
                    ("Tadım", "Kavrulmuş Ay Çekirdeği 200g", 65f),
                    ("Kellogg's", "Corn Flakes 500g", 95f),
                    ("Heinz", "Ketçap 460g", 75f),
                    ("Komili", "Riviera Zeytinyağı 2 lt", 320f),
                    ("Filiz", "Burgu Makarna 500g", 42f),
                    ("İçim", "Beyaz Peynir 600g", 135f),
                }},

                { "Temizlik", new()
                {
                    ("Ariel", "Dağ Esintisi Toz Deterjan 7 kg", 340f),
                    ("Ariel", "Oxi Sıvı Çamaşır Deterjanı 4 lt", 380f),
                    ("Persil", "Power Jel Sıvı Deterjan 4 lt", 350f),
                    ("Fairy", "Platinum Bulaşık Kapsülü 60'lı", 480f),
                    ("Fairy", "Elde Yıkama Sıvısı 1350ml", 95f),
                    ("Domestos", "Çamaşır Suyu 3.2 lt", 135f),
                    ("Domestos", "Power Fresh WC Blok 3'lü", 85f),
                    ("Papia", "İpek Özlü Tuvalet Kağıdı 32'li", 310f),
                    ("Selpak", "Kağıt Havlu 12'li", 240f),
                    ("Vernel", "Max Konsantre Yumuşatıcı 1440ml", 115f),
                    ("Vanish", "Oxi Action Leke Çıkarıcı 1 kg", 195f),
                    ("Ace", "Jel Çamaşır Suyu 4 lt", 155f),
                    ("Cif", "Krem Temizleyici 750ml", 65f),
                    ("Mr. Muscle", "Mutfak Temizleyici Sprey 750ml", 85f),
                    ("Marc", "Sıvı Çamaşır Deterjanı 4 lt", 290f),
                    ("Finish", "Quantum Max Bulaşık Kapsülü 80'li", 520f),
                    ("Yumoş", "Extra Konsantre Yumuşatıcı 1440ml", 105f),
                    ("Solo", "Tuvalet Kağıdı 48'li Ekonomik", 360f),
                    ("Tursil", "Toz Deterjan 6 kg", 280f),
                    ("Bingo", "Yüzey Temizleyici 2.5 lt", 75f),
                }},

                { "Kişisel Bakım", new()
                {
                    ("Oral-B", "Pro 3 3000 Şarjlı Diş Fırçası", 1200f),
                    ("Oral-B", "Vitality D100 Diş Fırçası", 650f),
                    ("Colgate", "Optic White Diş Macunu 75ml", 95f),
                    ("Colgate", "Total Profesyonel Beyazlık 75ml", 85f),
                    ("Gillette", "Fusion 5 ProGlide Yedek Bıçak 8'li", 480f),
                    ("Gillette", "Mach3 Tıraş Makinesi + 2 Yedek", 320f),
                    ("Clear Men", "Kepeğe Karşı Şampuan 600ml", 135f),
                    ("Head & Shoulders", "Classic Clean Şampuan 500ml", 125f),
                    ("Dove", "Güzellik Sabunu 4x90g", 85f),
                    ("Dove", "Nemlendirici Duş Jeli 450ml", 115f),
                    ("Rexona", "Men Invisible Deodorant 150ml", 95f),
                    ("Nivea", "Men Silver Protect Roll-On 50ml", 85f),
                    ("Philips", "OneBlade Pro Face QP6551", 1800f),
                    ("Braun", "Series 5 51-M1200s Tıraş Makinesi", 3500f),
                    ("Elseve", "Mucizevi Yağ Şampuan 450ml", 120f),
                    ("Pantene", "Saç Dökülmesine Karşı Şampuan 500ml", 115f),
                    ("Signal", "White Now Diş Macunu 75ml", 75f),
                    ("Listerine", "Cool Mint Ağız Bakım Suyu 500ml", 95f),
                    ("Wella", "Koleston Saç Boyası", 135f),
                    ("Johnson's", "Baby Şampuan 750ml", 120f),
                }},

                // ═══════════════════════════════════════════════════════════════
                // SPOR & OUTDOOR
                // ═══════════════════════════════════════════════════════════════

                { "Fitness", new()
                {
                    ("Decathlon", "Domyos 10 kg Dambıl Seti", 750f),
                    ("Decathlon", "Domyos 20 kg Halter Seti", 1200f),
                    ("Nike", "Pro Antrenman Eldiveni", 550f),
                    ("Nike", "Fundamental Yoga Matı 5mm", 850f),
                    ("Nike", "Metcon 9 Antrenman Ayakkabısı", 4800f),
                    ("Adidas", "Performance Ağırlık Eldiveni", 480f),
                    ("Under Armour", "Training Mat 6mm", 950f),
                    ("Voit", "Kalın Pilates Matı 15mm", 450f),
                    ("Delta", "3'lü Direnç Bandı Seti", 280f),
                    ("Delta", "20 kg Ayarlanabilir Dambıl", 1800f),
                    ("Kettler", "Spin Bike Racer S", 12500f),
                    ("Theraband", "Profesyonel Direnç Bandı", 450f),
                    ("Adler", "Sayaçlı Atlama İpi", 150f),
                    ("TRX", "Suspension Training Pro Kit", 3500f),
                    ("Harbinger", "Power Ağırlık Kemeri Deri", 1200f),
                    ("Mueller", "Diz Desteği Pro Level", 650f),
                    ("Everlast", "Boks Eldiveni Powerlock 12oz", 1400f),
                    ("Reebok", "Step Board Profesyonel", 1800f),
                    ("Polar", "Ignite 3 Spor Saati", 6500f),
                    ("Gymball", "Anti-Burst Pilates Topu 65cm", 280f),
                }},

                { "Kamp", new()
                {
                    ("Quechua", "Arpenaz 4.1 Fresh&Black Çadır", 4500f),
                    ("Quechua", "2 Seconds Easy 3 Kişilik Çadır", 3200f),
                    ("Coleman", "Sundome 4 Kişilik Çadır", 5500f),
                    ("Stanley", "Classic Legendary Termos 1 lt", 2100f),
                    ("Stanley", "IceFlow Flip Straw 890ml", 1400f),
                    ("Nurgaz", "Portatif Kamp Ocağı NG-505", 550f),
                    ("Campingaz", "Party Grill 400 CV", 2800f),
                    ("Husky", "Mumya Tipi Uyku Tulumu -10°C", 3800f),
                    ("Naturehike", "Ultralight Uyku Tulumu CW300", 2200f),
                    ("Decathlon", "Katlanır Kamp Sandalyesi Basic", 450f),
                    ("Helinox", "Chair One Kamp Sandalyesi", 3500f),
                    ("Petzl", "Actik Core Şarjlı Kafa Lambası", 1200f),
                    ("Victorinox", "SwissChamp Çakı 33 Fonksiyon", 2800f),
                    ("Jack Wolfskin", "Highland Trail 65lt Sırt Çantası", 4500f),
                    ("Osprey", "Atmos AG 65 Sırt Çantası", 7500f),
                    ("Nalgene", "Wide Mouth 1lt Tritan Matara", 450f),
                    ("Jetboil", "Flash Kamp Ocağı Sistemi", 3200f),
                    ("Sea to Summit", "Ultralight Hamak Set", 1800f),
                    ("MSR", "Trail Mini Solo Cook Set", 1400f),
                    ("Therm-a-Rest", "ProLite Plus Şişme Mat", 2800f),
                }},

                { "Bisiklet", new()
                {
                    ("Kron", "XC100 29 Jant Dağ Bisikleti", 9500f),
                    ("Kron", "XC150 27.5 Jant MTB", 7800f),
                    ("Bianchi", "Touring Şehir Bisikleti", 10500f),
                    ("Bianchi", "Impulso Allroad GRX 600", 28000f),
                    ("Salcano", "Hector 26 Jant Dağ Bisikleti", 7500f),
                    ("Salcano", "NG750 29 Jant MTB Deore", 11000f),
                    ("Bisan", "MTS 4600 29 Jant MTB", 6500f),
                    ("Bisan", "CRX 8600 Yol Bisikleti Claris", 15000f),
                    ("Trek", "Marlin 7 29er Dağ Bisikleti", 22000f),
                    ("Giant", "Talon 2 29er MTB", 18000f),
                    ("Specialized", "Rockhopper Sport 29", 19500f),
                    ("Cannondale", "Trail 5 29er", 21000f),
                    ("Scott", "Scale 970 Karbon MTB", 16500f),
                    ("Shimano", "Lazer Genesis MIPS Kask", 1400f),
                    ("Shimano", "Deore XT BR-M8120 Fren Seti", 2200f),
                    ("Garmin", "Edge 540 GPS Bisiklet Bilgisayarı", 8500f),
                    ("Kryptonite", "Evolution Mini-7 U-Lock", 1200f),
                    ("Topeak", "Mini 20 Pro Tamir Seti", 650f),
                    ("Continental", "Grand Prix 5000 700x25c", 950f),
                    ("Lezyne", "Mega Drive 1800i Ön Işık", 2400f),
                }},

                // ═══════════════════════════════════════════════════════════════
                // KADIN
                // ═══════════════════════════════════════════════════════════════

                { "Kadın Giyim", new()
                {
                    ("Zara", "Oversize Basic Tişört", 450f),
                    ("Zara", "Crop Top Saten Bluz", 650f),
                    ("Mango", "Saten Uzun Kollu Gömlek", 1200f),
                    ("Mango", "Wide Leg Keten Pantolon", 1400f),
                    ("H&M", "Skinny High Waist Jean", 800f),
                    ("H&M", "Örme Oversize Hırka", 950f),
                    ("Koton", "Büzgülü Mini Elbise", 750f),
                    ("Koton", "Kuşaklı Trençkot", 2200f),
                    ("LC Waikiki", "Keten Blazer Ceket", 900f),
                    ("LC Waikiki", "Palazzo Pantolon", 550f),
                    ("Massimo Dutti", "100% İpek Gömlek", 2800f),
                    ("Massimo Dutti", "Kaşmir Boğazlı Kazak", 3500f),
                    ("Stradivarius", "Mom Fit Denim Pantolon", 850f),
                    ("Bershka", "Kısa Denim Ceket", 1100f),
                    ("Pull & Bear", "Kapüşonlu Oversize Sweatshirt", 750f),
                    ("Beymen Club", "Pilise Midi Etek", 1800f),
                    ("İpekyol", "A-Line Midi Elbise", 2400f),
                    ("Network", "Krep Blazer Ceket", 3200f),
                    ("Twist", "Asimetrik Saten Bluz", 1600f),
                    ("Vakko", "Yün Karışımlı Uzun Palto", 8500f),
                }},

                { "Kadın Aksesuar & Çanta", new()
                {
                    ("Guess", "Siyah Logo Çapraz Çanta", 3800f),
                    ("Guess", "Meridian Logo Tote Bag", 4500f),
                    ("Vakko", "Monogram Omuz Çantası", 8500f),
                    ("Vakko", "Mini Clutch Abiye Çanta", 3200f),
                    ("Michael Kors", "Jet Set Medium Tote", 7500f),
                    ("Michael Kors", "Bradshaw Crossbody Çanta", 5200f),
                    ("Coach", "Tabby Shoulder Bag 26", 9800f),
                    ("Beymen Club", "Hasır Plaj Çantası", 2500f),
                    ("Tommy Hilfiger", "Poppy Tote Çanta", 3800f),
                    ("Calvin Klein", "CK Must Medium Çanta", 4200f),
                    ("Pinko", "Love Bag One Simply", 8500f),
                    ("Derimod", "Hakiki Deri El Çantası", 2200f),
                    ("Longchamp", "Le Pliage Original L Tote", 4800f),
                    ("Furla", "Metropolis Mini Crossbody", 7500f),
                    ("Ted Baker", "Icon Tote Büyük Boy", 3500f),
                    ("Kate Spade", "Sam Icon Medium Satchel", 6200f),
                    ("Aldo", "Zincir Askılı Mini Çanta", 1800f),
                    ("Mango", "Deri Tokalı Kemer", 450f),
                    ("Zara", "Örgü Kumaş Kemer", 350f),
                    ("Fendi", "Baguette Mini Nappa", 35000f),
                }},

                { "Kadın Takı", new()
                {
                    ("Pandora", "Moments Gümüş Bileklik", 2800f),
                    ("Pandora", "Timeless Pavé Yüzük", 1800f),
                    ("Pandora", "Signature I-D Kolye", 2200f),
                    ("Atasay", "14 Ayar Altın İnce Kolye", 7500f),
                    ("Atasay", "Pırlanta Tektaş Yüzük 0.10ct", 12000f),
                    ("Zen", "Pırlanta Tektaş Yüzük 0.30ct", 35000f),
                    ("Zen", "14 Ayar Altın Zincir Kolye", 8500f),
                    ("Swarovski", "Iconic Swan Kristal Küpe", 3200f),
                    ("Swarovski", "Angelic Bileklik Beyaz Altın", 3800f),
                    ("Swarovski", "Attract Kristal Kolye", 2800f),
                    ("Tiffany & Co.", "Return to Tiffany Bileklik", 15000f),
                    ("So Chic", "Çelik Halhal Ayak Zinciri", 550f),
                    ("So Chic", "Minimal Çelik Küpe Seti 3'lü", 750f),
                    ("Altınbaş", "22 Ayar Burma Bilezik 10g", 28000f),
                    ("Goldstore", "Pırlantalı Tektaş Kolye 0.20ct", 18000f),
                    ("Daniel Wellington", "Classic Bracelet Rose Gold", 1200f),
                    ("Tous", "Bear Gümüş Kolye", 3200f),
                    ("Bvlgari", "B.zero1 Ring 18K", 28000f),
                    ("Thomas Sabo", "Charm Club Bileklik", 2800f),
                    ("Julie Vos", "Fleur-de-Lis Küpe Altın Kaplama", 4500f),
                }},

                { "Kadın Ayakkabı", new()
                {
                    ("Nike", "Air Zoom Pegasus 40 Kadın", 4500f),
                    ("Nike", "Air Force 1 '07 Kadın", 4200f),
                    ("Adidas", "Ultraboost Light Kadın", 6000f),
                    ("Adidas", "Stan Smith Kadın", 3800f),
                    ("Vans", "Old Skool Siyah Kadın", 2500f),
                    ("Converse", "Chuck Taylor All Star Kadın", 2800f),
                    ("New Balance", "530 Kadın Beyaz", 4200f),
                    ("Derimod", "Siyah Stiletto 8cm", 1800f),
                    ("Aldo", "Platform Topuklu Sandalet", 2600f),
                    ("Nine West", "Nude Topuklu Ayakkabı", 2200f),
                    ("Kemal Tanca", "Deri Abiye Ayakkabı", 3200f),
                    ("Elle", "İnce Bantlı Topuklu Sandalet", 1950f),
                    ("Hotiç", "Topuklu Tek Bant Sandalet", 2400f),
                    ("İnci", "Deri Babet Ayakkabı", 1200f),
                    ("Puma", "Carina 2.0 Kadın Sneaker", 2800f),
                    ("Skechers", "Arch Fit Kadın Yürüyüş", 3200f),
                    ("Dr. Martens", "1460 Smooth Kadın Bot", 6500f),
                    ("Birkenstock", "Arizona EVA Terlik", 1800f),
                    ("Steve Madden", "Maxima Platform Sneaker", 4500f),
                    ("UGG", "Classic Mini II Kadın Bot", 5800f),
                }},

                { "Kadın Gözlük", new()
                {
                    ("Ray-Ban", "Wayfarer RB2140 Kadın", 3500f),
                    ("Ray-Ban", "Aviator Classic RB3025", 3800f),
                    ("Ray-Ban", "Round Metal RB3447", 3200f),
                    ("Tom Ford", "Jennifer TF8 Kadın", 6500f),
                    ("Tom Ford", "Whitney TF9 Kadın", 7200f),
                    ("Gucci", "GG0036SN Kadın Kare", 7200f),
                    ("Gucci", "GG0034SN Oversized Kadın", 8500f),
                    ("Dior", "DiorSoStellaire1 Kadın", 5800f),
                    ("Dior", "Lady Dior Studs Kadın", 6200f),
                    ("Prada", "PR 01OS Kadın Minimal", 6800f),
                    ("Versace", "VE4361 Kadın Güneş Gözlüğü", 5500f),
                    ("Dolce & Gabbana", "DG4386 Kadın", 5200f),
                    ("Chanel", "CH5414 Kare Güneş Gözlüğü", 8500f),
                    ("Burberry", "BE4216 Kadın Kare", 4500f),
                    ("Oakley", "Frogskins Kadın", 2800f),
                    ("Maui Jim", "Peahi Polarize", 4200f),
                    ("Carrera", "Champion 65 Kadın", 2200f),
                    ("Persol", "PO3019S İtalyan Klasik", 4800f),
                    ("Celine", "CL40061I Triomphe Kadın", 7500f),
                    ("Jimmy Choo", "Auri/G/S Kadın", 5200f),
                }},

                { "Kadın Parfüm", new()
                {
                    ("Chanel", "No.5 Eau de Parfum 100ml", 9500f),
                    ("Chanel", "Coco Mademoiselle EDP 100ml", 8500f),
                    ("Dior", "Miss Dior Eau de Parfum 100ml", 7500f),
                    ("Dior", "J'adore Eau de Parfum 100ml", 8200f),
                    ("Lancôme", "La Vie Est Belle EDP 100ml", 6800f),
                    ("Lancôme", "Idôle Eau de Parfum 75ml", 5800f),
                    ("Marc Jacobs", "Daisy Eau de Toilette 100ml", 5200f),
                    ("YSL", "Mon Paris Eau de Parfum 90ml", 6800f),
                    ("YSL", "Libre Eau de Parfum 90ml", 7200f),
                    ("Versace", "Bright Crystal EDT 90ml", 4200f),
                    ("Dolce & Gabbana", "Light Blue EDT 100ml", 4800f),
                    ("Gucci", "Bloom Eau de Parfum 100ml", 6500f),
                    ("Narciso Rodriguez", "For Her EDP 100ml", 5800f),
                    ("Carolina Herrera", "Good Girl EDP 80ml", 6200f),
                    ("Prada", "Candy Eau de Parfum 80ml", 5500f),
                    ("Viktor & Rolf", "Flowerbomb EDP 100ml", 7800f),
                    ("Givenchy", "L'Interdit EDP 80ml", 5500f),
                    ("Burberry", "Her Eau de Parfum 100ml", 4800f),
                    ("Tom Ford", "Black Orchid EDP 50ml", 7500f),
                    ("Jo Malone", "English Pear & Freesia 100ml", 6200f),
                }},

                // ═══════════════════════════════════════════════════════════════
                // ERKEK
                // ═══════════════════════════════════════════════════════════════

                { "Erkek Giyim", new()
                {
                    ("Zara", "Slim Fit Basic Tişört", 350f),
                    ("Zara", "Oversize Denim Ceket", 1400f),
                    ("Mango", "Regular Fit Keten Gömlek", 950f),
                    ("Mango", "Slim Fit Chino Pantolon", 1100f),
                    ("H&M", "Regular Fit Denim Jean", 800f),
                    ("H&M", "Bomber Ceket", 1200f),
                    ("Koton", "Basic Polo Yaka Tişört", 450f),
                    ("Koton", "Slim Fit Takım Elbise", 3500f),
                    ("LC Waikiki", "Kapüşonlu Sweatshirt", 550f),
                    ("LC Waikiki", "Kargo Pantolon", 650f),
                    ("Massimo Dutti", "İtalyan Yaka Keten Gömlek", 2200f),
                    ("Massimo Dutti", "Yün Karışım Blazer Ceket", 4500f),
                    ("Beymen Club", "Slim Fit Oxford Gömlek", 1800f),
                    ("Network", "Erkek Trençkot", 3800f),
                    ("DS Damat", "Slim Fit Takım Elbise", 6500f),
                    ("Altınyıldız Classics", "V-Yaka Yün Kazak", 1200f),
                    ("Lacoste", "Classic Fit L.12.12 Polo", 2800f),
                    ("Tommy Hilfiger", "1985 Regular Pique Polo", 2200f),
                    ("Polo Ralph Lauren", "Custom Fit Oxford Gömlek", 3500f),
                    ("Jack & Jones", "Suni Deri Biker Ceket", 4200f),
                }},

                { "Erkek Saat", new()
                {
                    ("Seiko", "5 Sports SRPD55 Automatic", 12000f),
                    ("Seiko", "Presage Cocktail Time SRPB43", 18000f),
                    ("Tissot", "PRX Powermatic 80 Blue", 25000f),
                    ("Tissot", "Gentleman Powermatic 80 Silicium", 22000f),
                    ("Casio", "G-Shock GA-2100-1A1 CasiOak", 4500f),
                    ("Casio", "Edifice EQB-1200 Chronograph", 5200f),
                    ("Casio", "G-Shock Mudman GW-9500", 7500f),
                    ("Fossil", "Gen 6 Smartwatch Kahverengi", 7500f),
                    ("Tommy Hilfiger", "Deri Kordon Chronograph", 4200f),
                    ("Tommy Hilfiger", "Mesh Kordon Mavi Kadran", 3800f),
                    ("Orient", "Bambino Version 2 FAC00005W0", 8500f),
                    ("Citizen", "Eco-Drive Promaster Diver", 9500f),
                    ("Hamilton", "Khaki Field Automatic 38mm", 28000f),
                    ("Swatch", "Sistem51 Otomatik", 4800f),
                    ("Daniel Wellington", "Classic 40mm DW00100007", 3200f),
                    ("Emporio Armani", "AR11179 Chronograph", 8500f),
                    ("Hugo Boss", "Ocean Edition Chronograph", 7200f),
                    ("Bulova", "Marine Star 98A227 Automatic", 11000f),
                    ("Tag Heuer", "Formula 1 Quartz WAZ1110", 38000f),
                    ("Omega", "Speedmaster Moonwatch Manual", 185000f),
                }},

                { "Erkek Cüzdan", new()
                {
                    ("Kemal Tanca", "Hakiki Deri Cüzdan Siyah", 1400f),
                    ("Kemal Tanca", "Deri Kartlık Kahverengi", 850f),
                    ("Derimod", "Siyah Deri Cüzdan RFID", 1200f),
                    ("Derimod", "Bozuk Para Bölmeli Cüzdan", 650f),
                    ("Tommy Hilfiger", "Johnson CC And Coin Cüzdan", 2100f),
                    ("Tommy Hilfiger", "Eton Mini CC Kartlık", 1600f),
                    ("Vakko", "Logo Baskılı Deri Cüzdan", 2800f),
                    ("Vakko", "Saffiano Deri Kartlık", 1400f),
                    ("Pierre Cardin", "Hakiki Deri Kartlık", 950f),
                    ("Pierre Cardin", "RFID Korumalı Deri Cüzdan", 1400f),
                    ("Calvin Klein", "CK Must Bifold Cüzdan", 2400f),
                    ("Calvin Klein", "Warmth Kartlık Siyah", 1800f),
                    ("Fossil", "Derrick RFID Bifold Cüzdan", 2200f),
                    ("Michael Kors", "Harrison Billfold Cüzdan", 3200f),
                    ("Hugo Boss", "Signature Deri Cüzdan", 3800f),
                    ("Polo Ralph Lauren", "Pebble Leather Cüzdan", 3200f),
                    ("Montblanc", "Meisterstück 6cc Kartlık", 6500f),
                    ("Secrid", "Miniwallet Original RFID", 1800f),
                    ("Bellroy", "Note Sleeve Slim Cüzdan", 2400f),
                    ("Samsonite", "RFID Hakiki Deri Cüzdan", 1600f),
                }},

                { "Erkek Ayakkabı", new()
                {
                    ("Nike", "Air Zoom Pegasus 40 Erkek", 4500f),
                    ("Nike", "Air Force 1 '07 Erkek", 4200f),
                    ("Nike", "Air Max 90 Erkek", 5200f),
                    ("Adidas", "Ultraboost Light Erkek", 6000f),
                    ("Adidas", "Stan Smith Erkek", 3800f),
                    ("Adidas", "Samba OG Erkek Beyaz", 4200f),
                    ("Puma", "Deviate Nitro 2 Erkek", 5200f),
                    ("Under Armour", "HOVR Phantom 3 Erkek", 4800f),
                    ("Asics", "Gel-Kayano 30 Erkek", 6500f),
                    ("New Balance", "550 Erkek Beyaz Yeşil", 4500f),
                    ("New Balance", "574 Core Erkek", 3800f),
                    ("Converse", "Chuck Taylor All Star Erkek", 2800f),
                    ("Vans", "Old Skool Erkek Siyah", 2500f),
                    ("Hoka", "Clifton 9 Erkek Koşu", 5800f),
                    ("Salomon", "Speedcross 6 Trail Koşu", 5500f),
                    ("Timberland", "Premium 6 Inch Waterproof Bot", 6500f),
                    ("Dr. Martens", "1460 Smooth Erkek Bot", 6800f),
                    ("Skechers", "Arch Fit Erkek Yürüyüş", 3200f),
                    ("Clarks", "Desert Boot Erkek Süet", 4200f),
                    ("Greyder", "Hakiki Deri Klasik Ayakkabı", 2200f),
                }},

                { "Erkek Gözlük", new()
                {
                    ("Ray-Ban", "Clubmaster RB3016 Erkek", 3800f),
                    ("Ray-Ban", "Aviator RB3025 Gold", 4200f),
                    ("Ray-Ban", "New Wayfarer RB2132 Erkek", 3500f),
                    ("Tom Ford", "James Bond TF108 Erkek", 6800f),
                    ("Tom Ford", "Henry FT0248 Erkek Kare", 7200f),
                    ("Gucci", "GG0061S Pilot Erkek", 7500f),
                    ("Gucci", "GG1263S Navigator Erkek", 6800f),
                    ("Dior", "DiorBlackSuit S2I Erkek", 6200f),
                    ("Oakley", "Holbrook OO9102 Erkek", 3200f),
                    ("Oakley", "Sutro OO9406 Sport", 4500f),
                    ("Persol", "PO0714 Steve McQueen Katlanır", 5800f),
                    ("Prada", "PR 17WS Erkek Kare", 6500f),
                    ("Versace", "VE2150Q Erkek Pilot", 5800f),
                    ("Hugo Boss", "BOSS 1451/S Erkek Pilot", 3500f),
                    ("Carrera", "1047/S Erkek Sport", 2500f),
                    ("Police", "SPLF61 Erkek Navigator", 2800f),
                    ("Polaroid", "PLD 2129/S Polarize", 1200f),
                    ("Maui Jim", "Peahi MJ202 Polarize", 4200f),
                    ("Emporio Armani", "EA4129 Erkek Kare", 3200f),
                    ("Porsche Design", "P8478 Değiştirilebilir Cam", 8500f),
                }},

                { "Erkek Parfüm", new()
                {
                    ("Dior", "Sauvage Eau de Toilette 100ml", 7000f),
                    ("Dior", "Sauvage Eau de Parfum 100ml", 7800f),
                    ("Chanel", "Bleu de Chanel EDP 100ml", 7500f),
                    ("Chanel", "Allure Homme Sport EDT 100ml", 6800f),
                    ("Versace", "Eros Eau de Toilette 100ml", 6200f),
                    ("Versace", "Dylan Blue Pour Homme EDT 100ml", 5500f),
                    ("Hugo Boss", "Bottled Eau de Parfum 100ml", 5500f),
                    ("Hugo Boss", "Boss The Scent EDT 100ml", 5200f),
                    ("Giorgio Armani", "Acqua di Gio EDT 100ml", 5800f),
                    ("Giorgio Armani", "Code Eau de Parfum 110ml", 6500f),
                    ("Paco Rabanne", "1 Million EDT 100ml", 5200f),
                    ("Paco Rabanne", "Invictus EDT 100ml", 5000f),
                    ("Jean Paul Gaultier", "Le Male EDT 125ml", 5500f),
                    ("YSL", "La Nuit de L'Homme EDT 100ml", 5800f),
                    ("Tom Ford", "Oud Wood EDP 50ml", 12000f),
                    ("Dolce & Gabbana", "The One EDP 100ml", 5500f),
                    ("Calvin Klein", "Eternity EDT 100ml", 3200f),
                    ("Montblanc", "Explorer EDP 100ml", 3800f),
                    ("Bvlgari", "Man in Black EDP 100ml", 4800f),
                    ("Creed", "Aventus Eau de Parfum 100ml", 28000f),
                }},
            };
        }
    }
}
