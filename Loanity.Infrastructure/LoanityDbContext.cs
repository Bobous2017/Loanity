using Loanity.Domain.Entities;
using Loanity.Domain.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Loanity.Infrastructure
{
    public class LoanityDbContext : DbContext
    {
        public LoanityDbContext(DbContextOptions<LoanityDbContext> options) : base(options) { }
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<EquipmentCategory> EquipmentCategories => Set<EquipmentCategory>();
        public DbSet<Equipment> Equipment => Set<Equipment>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<LoanItem> LoanItems => Set<LoanItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Role>().HasKey(x => x.Id);
            b.Entity<Role>().HasIndex(x => x.Name).IsUnique();

            b.Entity<User>().HasKey(x => x.Id);
            b.Entity<User>().HasIndex(x => x.Email).IsUnique();
            b.Entity<User>().HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);

            b.Entity<EquipmentCategory>().HasKey(x => x.Id);
            b.Entity<EquipmentCategory>().HasIndex(x => x.Name).IsUnique();

            b.Entity<Equipment>().HasKey(x => x.Id);
            b.Entity<Equipment>().HasIndex(x => x.SerialNumber).IsUnique();
            b.Entity<Equipment>().HasIndex(x => x.QrCode).IsUnique();
            b.Entity<Equipment>()
              .Property(x => x.Status).HasConversion<string>();
            b.Entity<Equipment>()
              .HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);


            b.Entity<Reservation>()
                .Property(x => x.EquipmentId)
                .HasColumnName("EquipmentId");

            b.Entity<Reservation>()
                .Property(x => x.LoanId)
                .HasColumnName("LoanId");

            b.Entity<Reservation>()
                .Property(x => x.StartAt)
                .HasColumnName("StartAt");

            b.Entity<Reservation>()
                .Property(x => x.EndAt)
                .HasColumnName("EndAt");

            b.Entity<Reservation>()
            .Property(x => x.UserId)
            .HasColumnName("UserId");

            b.Entity<Reservation>()
                .Property(x => x.EquipmentId)
                .HasColumnName("EquipmentId");

            b.Entity<Reservation>()
                .Property(x => x.LoanId)
                .HasColumnName("LoanId");

            b.Entity<Reservation>()
                .Property(x => x.StartAt)
                .HasColumnName("StartAt");

            b.Entity<Reservation>()
                .Property(x => x.EndAt)
                .HasColumnName("EndAt");

            b.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion<string>();


            b.Entity<Reservation>()
            .HasOne(r => r.Loan)
            .WithMany()
            .HasForeignKey(r => r.LoanId)
            .IsRequired(false);



            b.Entity<Loan>().HasKey(x => x.Id);
            b.Entity<Loan>()
              .Property(x => x.Status).HasConversion<string>();

            // explicit navigation mapping — prevents EF from creating a shadow FK (UserId1)
            b.Entity<Loan>()
              .HasOne(l => l.User)           // Loan.User navigation
              .WithMany(u => u.Loans)       // User.Loans inverse
              .HasForeignKey(l => l.UserId) // use existing UserId column
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Loan>()
              .HasCheckConstraint("CK_Loans_DueAfterStart", "DueAt > StartAt");

            // Junction with composite PK
            b.Entity<LoanItem>().HasKey(li => new { li.LoanId, li.EquipmentId });
            b.Entity<LoanItem>()
              .HasOne(li => li.Loan).WithMany(l => l.Items).HasForeignKey(li => li.LoanId);
            b.Entity<LoanItem>()
              .HasOne(li => li.Equipment).WithMany(e => e.LoanItems).HasForeignKey(li => li.EquipmentId);


            // --- Seed Roles ---
            b.Entity<Role>().HasData(new Role { Id = 1, Name = "Admin" }, new Role { Id = 2, Name = "User" });

            // --- Seed Categories ---
            b.Entity<EquipmentCategory>().HasData(
                new EquipmentCategory { Id = 1, Name = "Laptop" },
                new EquipmentCategory { Id = 2, Name = "Tablet" }
            );

            // --- Seed Equipment ---
            b.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Dell XPS 13", SerialNumber = "SN123", QrCode = "QR123", Status = EquipmentStatus.Available, CategoryId = 1 },
                new Equipment { Id = 2, Name = "iPad Pro", SerialNumber = "SN124", QrCode = "QR124", Status = EquipmentStatus.Available, CategoryId = 2 }
            );

            // --- Seed Users ---
            b.Entity<User>().HasData(
             new User
             {
                 Id = 1,
                 FirstName = "Alice",
                 LastName = "Admin",
                 Email = "alice@example.com",
                 UserName = "admin",
                 PassWord = "admin", // (or hashed later)
                 RfidChip = "123456",
                 RoleId = 1
             },
             new User
             {
                 Id = 2,
                 FirstName = "Bob",
                 LastName = "Borrower",
                 Email = "bob@example.com",
                 UserName = "bob",
                 PassWord = "bob2017", // (or hashed later)
                 RfidChip = null,
                 RoleId = 2
             }
         );

        }
    }
}
