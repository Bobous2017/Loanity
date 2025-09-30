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


    }
}
