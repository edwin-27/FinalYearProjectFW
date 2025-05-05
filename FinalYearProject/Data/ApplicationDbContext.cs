using FinalYearProject.Models;
using FinalYearProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinalYearProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

        {

            
        }

        public DbSet<Category> Category {  get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Customer> Customers { get; set; }


        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany()
                .HasForeignKey(c => c.ParentCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryCode)
                .HasPrincipalKey(c => c.Code);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(var => var.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(var => var.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductCategory>()
               .HasOne(pc => pc.Category)
               .WithMany()
               .HasForeignKey(pc => pc.CategoryCode)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Basket>()
                .HasMany(ba => ba.BasketItems)
                .WithOne(bi => bi.Basket)
                .HasForeignKey(bi => bi.BasketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BasketItem>()
                .HasOne(bi => bi.ProductVariant)
                .WithMany()
                .HasForeignKey(bi => bi.ProductVariantId);

            modelBuilder.Entity<Product>()
                .Property(p => p.AvgRating)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                 .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId);


            base.OnModelCreating(modelBuilder);
            
        }

    }
}
