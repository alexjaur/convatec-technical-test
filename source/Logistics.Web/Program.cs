using Logistics.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
builder.Services.AddInternalDependencies(builder.Configuration);
builder.Services.AddControllersWithViews();

// add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Logistics API",
        Version = "v1",
        Description = "Documentación de mi Logistics API"
    });
});

var app = builder.Build();

// configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    // the default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// for easy testing of API endpoints
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapStaticAssets();

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transporters}/{action=Index}/{id?}")
    .WithStaticAssets();

// API routes
app.MapControllers();

app.Run();
