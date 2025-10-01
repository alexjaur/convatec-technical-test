namespace Logistics.Application.Clients.ShipmentClient
{
    public interface IShipmentClient
    {
        Task<ShipmentStatusDto?> GetStatusAsync(int orderId, CancellationToken cancellationToken = default);
    }
}
