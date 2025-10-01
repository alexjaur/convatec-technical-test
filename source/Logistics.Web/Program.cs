using Logistics.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
builder.Services.AddInternalDependencies(builder.Configuration);
builder.Services.AddControllersWithViews();

// add Auth
var azureAdApiInstance = builder.Configuration["AzureAdApi:Instance"];
var azureAdApiTenantId = builder.Configuration["AzureAdApi:TenantId"];
var azureAdApiAudience = builder.Configuration["AzureAdApi:Audience"];
var azureAdApiSwaggerClientId = builder.Configuration["AzureAdApi:SwaggerClientId"];
var azureAdApiScopes = $"api://{azureAdApiAudience}/access_as_user";
var azureAdApiBaseUrl = $"{azureAdApiInstance}/{azureAdApiTenantId}";

var azureAdWebAppInstance = builder.Configuration["AzureAdWebApp:Instance"];
var azureAdWebAppTenantId = builder.Configuration["AzureAdWebApp:TenantId"];
var azureAdWebAppUserFlow = builder.Configuration["AzureAdWebApp:UserFlow"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Audience = azureAdApiAudience;
        options.Authority = $"{azureAdApiBaseUrl}/v2.0/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{azureAdApiBaseUrl}/v2.0"
        };
    })
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAdWebApp", options);

        options.Authority = $"{azureAdWebAppInstance}/{azureAdWebAppTenantId}/v2.0/";

        options.ResponseType = "code";
        options.SaveTokens = true;

        options.Events ??= new OpenIdConnectEvents();
        options.Events.OnRedirectToIdentityProvider = ctx =>
        {
            ctx.ProtocolMessage.Parameters["p"] = azureAdWebAppUserFlow;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToIdentityProviderForSignOut = ctx =>
        {
            ctx.ProtocolMessage.Parameters["p"] = azureAdWebAppUserFlow;
            return Task.CompletedTask;
        };
    });

// add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Logistics.Api", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{azureAdApiBaseUrl}/oauth2/v2.0/authorize?p=user-flow-convatec"),
                TokenUrl = new Uri($"{azureAdApiBaseUrl}/oauth2/v2.0/token?p=user-flow-convatec"),
                Scopes = new Dictionary<string, string>
                {
                    { azureAdApiScopes, "Access Logistics API as a user" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { azureAdApiScopes }
        }
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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Logistics.Api v1");

    // configure OAuth2 for "Authorize" button
    c.OAuthClientId(azureAdApiSwaggerClientId);
    c.OAuthUsePkce();
    c.OAuthScopes(azureAdApiScopes);
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transporters}/{action=Index}/{id?}")
    .WithStaticAssets();

// API routes
app.MapControllers();

app.Run();
