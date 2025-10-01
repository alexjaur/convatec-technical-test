using System.Net.Http.Json;
using Polly;
using Polly.Retry;
using Logistics.Application.Clients.ShipmentClient;

namespace Logistics.Infrastructure.Clients.ShipmentClient
{
    public class FakeStoreClient(IHttpClientFactory httpClientFactory) : IShipmentClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(FakeStoreClient));

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retry = Policy
          .Handle<HttpRequestException>()
          .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
          .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * i));

        public async Task<ShipmentStatusDto?> GetStatusAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var httpResponseMessage = await _retry.ExecuteAsync(() => _httpClient.GetAsync($"/orders/{orderId}", cancellationToken));

            httpResponseMessage.EnsureSuccessStatusCode();

            var shipmentStatusDto = await httpResponseMessage.Content.ReadFromJsonAsync<ShipmentStatusDto>(cancellationToken: cancellationToken);
            
            return shipmentStatusDto;
        }
    }
}
