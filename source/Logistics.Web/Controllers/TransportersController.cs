using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Logistics.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Logistics.Application.Services;
using Logistics.Domain.Entities;

namespace Logistics.Web.Controllers;

public class TransportersController(ITransporterService transporterService, ILogger<TransportersController> logger) : Controller
{
    public async Task<IActionResult> Index() 
        => View(await transporterService.GetAllAsync());

    public IActionResult Create() 
        => View(new Transporter());

    [HttpPost]
    public async Task<IActionResult> Create(Transporter transporter)
    {
        if (!ModelState.IsValid)
        {
            return View(transporter);
        }

        await transporterService.CreateAsync(transporter);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var transporter = await transporterService.GetAsync(id);

        return transporter is not null 
            ? View(transporter) 
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Transporter transporter)
    {
        if (!ModelState.IsValid)
        {
            return View(transporter);
        }

        var updated = await transporterService.UpdateAsync(transporter);

        return updated is not null 
            ? RedirectToAction(nameof(Index)) 
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await transporterService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
