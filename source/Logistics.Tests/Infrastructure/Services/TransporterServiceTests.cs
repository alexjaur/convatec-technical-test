using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Logistics.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Tests.Infrastructure.Services
{
    public class TransporterServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllTransporters()
        {
            // Arrange: configurar contexto en memoria
            var options = new DbContextOptionsBuilder<LogisticsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new LogisticsDbContext(options);

            context.Transporters.AddRange(
                new Transporter
                {
                    Id = 1,
                    Name = "Trans A",
                    Document = "123456789",
                    Email = "transa@example.com",
                    Phone = "555-1234"
                },
                new Transporter
                {
                    Id = 2,
                    Name = "Trans B",
                    Document = "987654321",
                    Email = "transb@example.com",
                    Phone = "555-5678"
                }
            );
            context.SaveChanges();

            TransporterService service = new TransporterService(context);

            // Act: ejecutar método
            var result = await service.GetAllAsync();

            // Assert: validar resultado
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Name == "Trans A");
            Assert.Contains(result, t => t.Name == "Trans B");
        }
    }
}
