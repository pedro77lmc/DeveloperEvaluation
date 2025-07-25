using DeveloperStore.Sales.Domain.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infra.Repositories
{
    public class SalesDbContext : DbContext
    {
        // CORREÇÃO: Use Microsoft.EntityFrameworkCore.DbSet
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        // Método para acessar Sales (opcional)
        public DbSet<Sale> GetSales()
        {
            return Sales;
        }

        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sale Configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                // Customer as owned entity (External Identity pattern)
                entity.OwnsOne(e => e.Customer, customer =>
                {
                    customer.Property(c => c.Name).IsRequired().HasMaxLength(200);
                    customer.Property(c => c.Email).IsRequired().HasMaxLength(200);
                    customer.Property(c => c.Document).IsRequired().HasMaxLength(20);
                });

                // Branch as owned entity (External Identity pattern)
                entity.OwnsOne(e => e.Branch, branch =>
                {
                    branch.Property(b => b.Name).IsRequired().HasMaxLength(200);
                    branch.Property(b => b.Address).IsRequired().HasMaxLength(500);
                    branch.Property(b => b.City).IsRequired().HasMaxLength(100);
                    branch.Property(b => b.State).IsRequired().HasMaxLength(2);
                });

                // One-to-many relationship with SaleItems
                entity.HasMany<SaleItem>("_items")
                      .WithOne()
                      .HasForeignKey("SaleId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SaleItem Configuration
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.DiscountPercentage).HasPrecision(5, 4);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                // Product as owned entity (External Identity pattern)
                entity.OwnsOne(e => e.Product, product =>
                {
                    product.Property(p => p.Name).IsRequired().HasMaxLength(200);
                    product.Property(p => p.Description).HasMaxLength(1000);
                    product.Property(p => p.Category).IsRequired().HasMaxLength(100);
                    product.Property(p => p.Sku).IsRequired().HasMaxLength(50);
                });
            });
        }
    }
}