// Brings in your domain entity definitions like Equipment, EquipmentCategory, etc.
using Loanity.Domain.Entities;

// Brings in enum values like EquipmentStatus.Available, etc.
using Loanity.Domain.Statuses;

// Gives you access to LoanityDbContext (your real database context)
using Loanity.Infrastructure;

// Core EF stuff like DbContextOptionsBuilder, etc.
using Microsoft.EntityFrameworkCore;

//  THIS IS THE MOST IMPORTANT: Needed for mocking the database with .UseInMemoryDatabase()
using Microsoft.EntityFrameworkCore.InMemory;

// xUnit is the testing framework you're using
using Xunit;

// System features used for LINQ querying
using System.Linq;
using System.Threading.Tasks;

namespace Loanity.Tests
{
    public class EquipmentTests
    {
        //  1. This creates a fake/mock DB context with InMemoryDatabase (not MSSQL)
        private LoanityDbContext GetInMemoryDbContext()
        {
            // This builds options for your DbContext to use an in-memory database
            var options = new DbContextOptionsBuilder<LoanityDbContext>()
                .UseInMemoryDatabase(databaseName: "LoanityTestDb") // ← name doesn't matter much here
                .Options;

            // Create the mock DB context using those options
            var context = new LoanityDbContext(options);

            //  2. Add one EquipmentCategory to mock DB so FK relation doesn't fail
            context.EquipmentCategories.Add(new EquipmentCategory
            {
                Id = 99,
                Name = "TestCategory"
            });

            //  3. Add one Equipment record to be used in test
            context.Equipment.Add(new Equipment
            {
                Id = 1,
                Name = "Test Device",
                CategoryId = 99,               // FK to above category
                QrCode = "QR001",              // Used to find this device later in test
                SerialNumber = "SN001",
                Status = EquipmentStatus.Available
            });

            //  4. Save the above "mock" data into the in-memory DB
            context.SaveChanges();

            return context;
        }

        //  This is your test: "Should get Equipment by QR"
        [Fact]
        public async Task Should_Get_Equipment_By_QR()
        {
            //  Arrange: Get the fake/mock DB context with seeded data
            var context = GetInMemoryDbContext();

            //  Act: Try to find the Equipment where QrCode == "QR001"
            var equipment = await context.Equipment.FirstOrDefaultAsync(e => e.QrCode == "QR001");

            //  Assert: Confirm the equipment was found
            Assert.NotNull(equipment);                     // Test fails if no equipment found
            Assert.Equal("Test Device", equipment.Name);   // Test fails if wrong name
        }
    }
}

