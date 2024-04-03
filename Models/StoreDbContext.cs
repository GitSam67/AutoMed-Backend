using Microsoft.EntityFrameworkCore;


namespace AutoMed_Backend.Models
{
    public class StoreDbContext: DbContext
    {
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<CashBalance> CashBalance { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<StoreOwner> StoreOwners { get; set; }

        public StoreDbContext()
        {

        }

        // DI Resolve
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>()
           .HasKey(i => i.InventoryId);

            modelBuilder.Entity<Medicine>()
                .HasKey(m => m.MedicineId);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.MedicineId)
                .IsRequired();

            // Define the foreign key relationship
            modelBuilder.Entity<Inventory>()
                .HasOne<Medicine>()
                .WithMany()
                .HasForeignKey(i => i.MedicineId);

            modelBuilder.Entity<Inventory>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(i => i.BranchId);


            modelBuilder.Entity<Orders>()
            .HasKey(s => s.OrderId);

            modelBuilder.Entity<Orders>()
                .Property(s => s.CustomerId)
                .IsRequired();

            modelBuilder.Entity<Orders>()
                .HasMany(s => s.Medicines)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "OrdersMedicine",
                    j => j.HasOne<Medicine>().WithMany().HasForeignKey("MedicineId"),
                    j => j.HasOne<Orders>().WithMany().HasForeignKey("OrderId")
                );

            modelBuilder.Entity<Orders>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<Orders>()
                .Property(s => s.PurchaseTime)
                .HasColumnType("datetime");

            modelBuilder.Entity<Orders>()
                .Property(s => s.TotalBill)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Orders>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(s => s.BranchId);

            modelBuilder.Entity<CashBalance>()
                .HasKey(c => c.id);

            modelBuilder.Entity<CashBalance>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(s => s.BranchId);

            modelBuilder.Entity<Branch>()
                .HasKey(i => i.BranchId);

            modelBuilder.Entity<StoreOwner>()
                .HasKey(i => i.OwnerId);

            modelBuilder.Entity<StoreOwner>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(s => s.BranchId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
