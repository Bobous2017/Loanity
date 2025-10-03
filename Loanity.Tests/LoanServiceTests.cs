using Loanity.Domain.Entities;
using Loanity.Domain.IServices;
using Loanity.Domain.Statuses;
using Loanity.Infrastructure.Services;
using Loanity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Tests
{
    public class LoanServiceTests
    {
        private LoanityDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LoanityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // new DB every time
                .Options;

            var context = new LoanityDbContext(options);

            // Seed required data
            var user = new User
            {
                Id = 1,
                Email = "student@example.com",
                FirstName = "Claus",
                LastName = "Steen",
                UserName = "claussteen",
                RoleId = 2
            };

            var category = new EquipmentCategory { Id = 1, Name = "Laptops" };

            var equipment = new Equipment
            {
                Id = 1,
                Name = "Test Laptop",
                QrCode = "QR001",
                SerialNumber = "SN001",
                CategoryId = 1,
                Status = EquipmentStatus.Available
            };

            context.Users.Add(user);
            context.EquipmentCategories.Add(category);
            context.Equipment.Add(equipment);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Should_Create_Loan_By_Scan_When_Equipment_Available()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null))
                     .Returns(Task.CompletedTask);

            var service = new LoanService(context, emailMock.Object);

            // Act
            var dueAt = DateTime.UtcNow.AddDays(3);
            var loan = await service.CreateLoanFromScanAsync(1, "QR001", dueAt);

            // Assert
            Assert.NotNull(loan);
            Assert.Equal(1, loan.UserId);
            Assert.Single(loan.Items);
            Assert.Equal("QR001", loan.Items.First().Equipment.QrCode);
            Assert.Equal(EquipmentStatus.Loaned, context.Equipment.First().Status);

            // Make sure email was called
            emailMock.Verify(e => e.SendAsync(
                "student@example.com",
                It.Is<string>(s => s.Contains("Loan Created")),
                It.IsAny<string>(),
                It.IsAny<System.IO.MemoryStream>(),
                "loan_qr.png"
            ), Times.Once);
        }
        [Fact]
        public async Task Should_Return_Equipment_By_Scan()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var equipment = new Equipment
            {
                Id = 2, // Different from the one seeded in GetInMemoryDbContext()
                Name = "Returnable Laptop",
                QrCode = "QR002",
                SerialNumber = "SN002",
                CategoryId = 1,
                Status = EquipmentStatus.Loaned
            };
            var loan = new Loan
            {
                Id = 1,
                UserId = 1,

                StartAt = DateTime.UtcNow.AddDays(-1),
                DueAt = DateTime.UtcNow.AddDays(2),
                Status = LoanStatus.Active,
                Items = new List<LoanItem>()
            };
            loan.Items.Add(new LoanItem
            {
                LoanId = 1,
                EquipmentId = 1,
                Loan = loan,
                Equipment = equipment
            });

            // Seed
            context.Equipment.Add(equipment);
            context.Loans.Add(loan);
            context.SaveChanges();

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null))
                     .Returns(Task.CompletedTask);

            var service = new LoanService(context, emailMock.Object);

            // Act
            var result = await service.ReturnByScanAsync(1, "QR002");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(LoanStatus.Closed, result.Status);
            Assert.NotNull(result.ReturnedAt);
            Assert.Equal(EquipmentStatus.Available, equipment.Status);

            // Optional: verify email
            emailMock.Verify(e => e.SendAsync(
                "student@example.com",
                It.Is<string>(s => s.Contains("Return Confirmed")),
                It.IsAny<string>(),
                null,
                null
            ), Times.Once);
        }

    }
}
