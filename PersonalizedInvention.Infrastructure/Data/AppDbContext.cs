using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Enums;

namespace PersonalizedInvention.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Decimal precision ────────────────────────────────────────
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // ── Let PostgreSQL set timestamps — static from EF's view ✅ ──
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.AddedAt)
                .HasDefaultValueSql("NOW()");

            // ── Relationships ─────────────────────────────────────────────
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CartItems)
                .WithOne(ci => ci.User)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ── Seed data — ALL values are static hardcoded ✅ ───────────
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Custom Engraved Mug",
                    Description = "Personalized ceramic mug with your name or message.",
                    Price = 199.99m,
                    Stock = 50,
                    Category = "Drinkware",
                    ImageUrl = "https://placehold.co/400x400?text=Mug",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                },
                new Product
                {
                    Id = 2,
                    Name = "Photo Canvas Print",
                    Description = "High-quality canvas print from your photo.",
                    Price = 599.99m,
                    Stock = 30,
                    Category = "Art",
                    ImageUrl = "https://placehold.co/400x400?text=Canvas",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                },
                new Product
                {
                    Id = 3,
                    Name = "Personalized Phone Case",
                    Description = "Custom printed phone case with your design.",
                    Price = 299.99m,
                    Stock = 100,
                    Category = "Accessories",
                    ImageUrl = "https://placehold.co/400x400?text=PhoneCase",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                },
                new Product
                {
                    Id = 4,
                    Name = "Custom T-Shirt",
                    Description = "Premium cotton t-shirt with your design or text.",
                    Price = 449.99m,
                    Stock = 0,
                    Category = "Clothing",
                    ImageUrl = "https://placehold.co/400x400?text=TShirt",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                },
                new Product
                {
                    Id = 5,
                    Name = "Engraved Keychain",
                    Description = "Stainless steel keychain with custom engraving.",
                    Price = 129.99m,
                    Stock = 75,
                    Category = "Accessories",
                    ImageUrl = "https://placehold.co/400x400?text=Keychain",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                },
                new Product
                {
                    Id = 6,
                    Name = "Personalised Notebook",
                    Description = "Hardcover notebook with your name on the cover.",
                    Price = 249.99m,
                    Stock = 40,
                    Category = "Stationery",
                    ImageUrl = "https://placehold.co/400x400?text=Notebook",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ hardcoded
                }
            );
        }
    }
}
