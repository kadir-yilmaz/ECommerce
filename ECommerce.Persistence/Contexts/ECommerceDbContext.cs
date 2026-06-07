using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Contexts
{
    public class ECommerceDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ECommerceDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CompletedOrder> CompletedOrders { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<CampaignImageFile> CampaignImageFiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
        public DbSet<OrderDiscount> OrderDiscounts { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }
        public DbSet<RewardRule> RewardRules { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserRefreshToken>()
                .HasOne(urt => urt.User)
                .WithMany(u => u.UserRefreshTokens)
                .HasForeignKey(urt => urt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasKey(b => b.Id);

            builder.Entity<Order>()
                .HasIndex(o => o.OrderCode)
                .IsUnique();

            builder.Entity<Order>()
                .HasMany(o => o.OrderDiscounts)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // A user can accumulate historical baskets through orders.
            builder.Entity<AppUser>()
                .HasMany(u => u.Baskets)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Basket>()
                .HasOne(b => b.Order)
                .WithOne(o => o.Basket)
                .HasForeignKey<Order>(b => b.Id);

            builder.Entity<Order>()
                .HasOne(o => o.CompletedOrder)
                .WithOne(c => c.Order)
                .HasForeignKey<CompletedOrder>(c => c.OrderId);

            builder.Entity<InvoiceFile>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Campaign>()
                .Property(c => c.DiscountRate)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Campaign>()
                .Property(c => c.MinAmount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DiscountCoupon>()
                .Property(dc => dc.DiscountValue)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DiscountCoupon>()
                .Property(dc => dc.MinCartAmount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<DiscountCoupon>()
                .Property(dc => dc.MaxDiscountAmount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderDiscount>()
                .Property(od => od.DiscountAmount)
                .HasColumnType("decimal(18, 2)");

            // UserCoupon ilişkileri
            builder.Entity<UserCoupon>()
                .HasOne(uc => uc.DiscountCoupon)
                .WithMany(dc => dc.UserCoupons)
                .HasForeignKey(uc => uc.DiscountCouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCoupon>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCoupons)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Aynı kullanıcı aynı kupona birden fazla kez atanamaz
            builder.Entity<UserCoupon>()
                .HasIndex(uc => new { uc.UserId, uc.DiscountCouponId })
                .IsUnique();

            // RewardRule decimal precision
            builder.Entity<RewardRule>()
                .Property(r => r.MinTotalSpent)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<RewardRule>()
                .Property(r => r.RewardDiscountValue)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<RewardRule>()
                .Property(r => r.RewardMaxDiscountAmount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<RewardRule>()
                .Property(r => r.CouponMinCartAmount)
                .HasColumnType("decimal(18, 2)");

            // Address configuration
            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Address>()
                .HasIndex(a => new { a.UserId, a.AddressType, a.IsDefault });

            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ChangeTracker : Entityler üzerinden yapılan değişiklerin ya da yeni eklenen verinin yakalanmasını sağlayan propertydir. Update operasyonlarında Track edilen verileri yakalayıp elde etmemizi sağlar.

            var datas = ChangeTracker
                 .Entries<BaseEntity>();

            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,
                    _ => DateTime.UtcNow
                };
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
