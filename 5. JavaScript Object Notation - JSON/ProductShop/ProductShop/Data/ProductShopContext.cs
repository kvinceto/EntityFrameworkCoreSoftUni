using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShop.Models;
namespace ProductShop.Data
{
    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {
        }

        public ProductShopContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<CategoryProduct> CategoriesProducts { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasColumnType("nvarchar(200)");
            });



            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(x => new { x.CategoryId, x.ProductId });
                entity
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CategoriesProducts)
                .HasForeignKey(cp => cp.ProductId);
                entity
                .HasOne(cp => cp.Category)
                .WithMany(c => c.CategoriesProducts)
                .HasForeignKey(cp => cp.CategoryId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.BuyerId).IsRequired(false);
                entity.Property(p => p.SellerId).IsRequired();
                entity
                .HasOne(p => p.Seller)
                .WithMany(u => u.ProductsSold)
                .HasForeignKey(p => p.SellerId);
                entity
                .HasOne(p => p.Buyer)
                .WithMany(u => u.ProductsBought)
                .HasForeignKey(p => p.BuyerId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FirstName).IsRequired(false).HasColumnType("nvarchar(200)");
                entity.Property(u => u.LastName).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(u => u.Age).IsRequired(false);
            });
        }
    }
}
