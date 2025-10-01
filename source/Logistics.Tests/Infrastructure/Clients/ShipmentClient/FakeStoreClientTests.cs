using System.Net;
using System.Text;
using System.Text.Json;
using Logistics.Infrastructure.Clients.ShipmentClient;
using Moq;
namespace Logistics.Tests.Infrastructure.Clients.ShipmentClient
{
    public class FakeStoreClientTests
    {
        [Fact]
        public async Task GetStatusAsync_ReturnsShipmentStatusDto_WhenResponseIsSuccessful()
        {
            var json = JsonSerializer.Serialize(new
            {
                Id = 1,
                UserId = 99,
                Products = new[]
                {
                new { ProductId = 10, Quantity = 2 },
                new { ProductId = 11, Quantity = 1 }
            },
                TotalAmount = 49.99m,
                Status = "Delivered",
                OrderDate = DateTime.UtcNow.AddDays(-3),
                DeliveryDate = DateTime.UtcNow
            });

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new MockHttpMessageHandler(mockResponse);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://fakestore.test")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(f => f.CreateClient(nameof(FakeStoreClient))).Returns(httpClient);

            var client = new FakeStoreClient(httpClientFactory.Object);

            // Act
            var result = await client.GetStatusAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("Delivered", result.Status);
            Assert.Equal(2, result.Products.Count);
            Assert.Equal(10, result.Products[0].ProductId);
            Assert.Equal(2, result.Products[0].Quantity);
        }
    }

}
