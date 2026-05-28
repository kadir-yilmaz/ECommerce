using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedProductsAsync(ECommerceDbContext context)
        {
            if (context.Products.Any()) return;

            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "iPhone 15 Pro", Stock = 50, Price = 59999 },
                new() { Id = Guid.NewGuid(), Name = "Samsung Galaxy S24", Stock = 75, Price = 44999 },
                new() { Id = Guid.NewGuid(), Name = "MacBook Air M3", Stock = 30, Price = 49999 },
                new() { Id = Guid.NewGuid(), Name = "Sony WH-1000XM5", Stock = 100, Price = 9999 },
                new() { Id = Guid.NewGuid(), Name = "iPad Pro 12.9", Stock = 40, Price = 39999 },
                new() { Id = Guid.NewGuid(), Name = "Apple Watch Ultra 2", Stock = 60, Price = 27999 },
                new() { Id = Guid.NewGuid(), Name = "Dell XPS 15", Stock = 25, Price = 42999 },
                new() { Id = Guid.NewGuid(), Name = "Logitech MX Master 3S", Stock = 150, Price = 2999 },
                new() { Id = Guid.NewGuid(), Name = "Samsung 4K Monitor 32\"", Stock = 35, Price = 12999 },
                new() { Id = Guid.NewGuid(), Name = "Corsair K100 Keyboard", Stock = 80, Price = 6999 }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
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
