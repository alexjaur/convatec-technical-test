using Logistics.Application.Clients.ShipmentClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Web.Controllers.Api
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController(IShipmentClient shipmentClient) : ControllerBase
    {
        [HttpGet("{orderId:int}/status")]
        public async Task<IActionResult> GetStatus(int orderId, CancellationToken cancellationToken)
        {
            var status = await shipmentClient.GetStatusAsync(orderId, cancellationToken);

            return status is not null 
                ? Ok(status) 
                : NotFound();
        }
    }
}
