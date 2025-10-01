using Logistics.Application.Services;
using Logistics.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.Web.Controllers.Api;

[Route("api/transporters")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TransportersApiController(ITransporterService transporterService, ILogger<TransportersApiController> logger) : ControllerBase
{
    [HttpGet]
    public Task<List<Transporter>> Get(CancellationToken cancellationToken = default) 
        => transporterService.GetAllAsync(cancellationToken);

    [HttpGet("{id:int}")]
    public Task<Transporter?> Get(int id, CancellationToken cancellationToken = default) 
        => transporterService.GetAsync(id, cancellationToken);

    [HttpPost]
    public Task<Transporter> Post([FromBody] Transporter transporter, CancellationToken cancellationToken = default) 
        => transporterService.CreateAsync(transporter, cancellationToken);

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Transporter transporter, CancellationToken cancellationToken = default)
    {
        transporter.Id = id;
        var updated = await transporterService.UpdateAsync(transporter, cancellationToken);

        return updated is not null 
            ? Ok(updated)
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await transporterService.DeleteAsync(id, cancellationToken);

        return deleted
            ? NoContent()
            : NotFound();
    }
}
