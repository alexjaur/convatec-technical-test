namespace Logistics.Application.Clients.ShipmentClient
{
    public record ShipmentStatusDto
    (
        int Id,
        int UserId,
        List<ShipmentProductDto> Products,
        decimal TotalAmount,
        string Status,
        DateTime OrderDate,
        DateTime? DeliveryDate
    );
}
