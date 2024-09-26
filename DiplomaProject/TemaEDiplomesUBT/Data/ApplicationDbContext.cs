using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Models;

namespace TemaEDiplomesUBT.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<DayOff> DayOff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Stock entity
            modelBuilder.Entity<Stock>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(s => s.WarehouseId);

            // Configure Sale entity
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.SaleId);

            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleDetails)
                .WithOne(sd => sd.Sale)
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SaleDetail entity
            modelBuilder.Entity<SaleDetail>()
                .HasKey(sd => sd.SaleDetailId);

            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product)
                .WithMany()
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleDetail>()
                .HasOne(sd => sd.Warehouse)
                .WithMany()
                .HasForeignKey(sd => sd.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Purchase entity
            modelBuilder.Entity<Purchase>()
                .HasKey(p => p.PurchaseId);

            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.PurchaseDetails)
                .WithOne(pd => pd.Purchase)
                .HasForeignKey(pd => pd.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseDetail entity
            modelBuilder.Entity<PurchaseDetail>()
                .HasKey(pd => pd.PurchaseDetailId);

            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Product)
                .WithMany()
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Warehouse)
                .WithMany()
                .HasForeignKey(pd => pd.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Customer entity
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Sales)
                .WithOne(s => s.Customer)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment entity
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Sale)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Supplier entity
            modelBuilder.Entity<Supplier>()
                .HasKey(s => s.SupplierId);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Purchases)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Debts)
                .WithOne(d => d.Supplier)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Debt entity
            modelBuilder.Entity<Debt>()
                .HasKey(d => d.DebtId);

            //modelBuilder.Entity<Debt>()
            //   .HasOne(d => d.Purchase)
            //   .WithMany()  
            //   .HasForeignKey(d => d.PurchaseId)
            //   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Debt>()
                .HasOne(d => d.Supplier)
                .WithMany(s => s.Debts)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
