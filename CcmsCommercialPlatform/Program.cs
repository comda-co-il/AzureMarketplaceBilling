using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Services;
using CcmsCommercialPlatform.Api.Models.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CCMS Commercial Platform API", Version = "v1" });
});

// Configure database provider (SQLite or SqlServer)
// Uses DefaultConnection from appsettings - Development uses SQLite, Production uses SqlServer
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SQLite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Ensure Development environment uses SQLite connection string
// This workaround handles cases where appsettings.Development.json connection string override doesn't work
if (builder.Environment.IsDevelopment() && databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
{
    // Override connection string for Development if it's still pointing to SQL Server
    if (connectionString != null && connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase))
    {
        connectionString = "Data Source=marketplace_billing.db";
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Configure services based on demo mode
var isDemo = builder.Configuration.GetValue<bool>("IsDemo");
if (isDemo)
{
    builder.Services.AddScoped<IAzureMarketplaceClient, DemoAzureMarketplaceClient>();
}
else
{
    builder.Services.AddScoped<IAzureMarketplaceClient, AzureMarketplaceClient>();
}

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IMeteredBillingService, MeteredBillingService>();
builder.Services.AddScoped<IIaCRunnerService, IaCRunnerService>();
builder.Services.AddScoped<IMarketplaceSubscriptionService, MarketplaceSubscriptionService>();

// Configure Email Service
builder.Services.Configure<MailSenderOptions>(builder.Configuration.GetSection("Mail"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Add HttpClient for external API calls
builder.Services.AddHttpClient();

// Add named HttpClient for Azure Marketplace API
builder.Services.AddHttpClient("AzureMarketplace", client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add named HttpClient for Claude IaC Runner API
builder.Services.AddHttpClient("IaCRunner", client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

// Serve static files - in Development, serve from ClientApp/dist for hot reload
// In Production, serve from wwwroot (files copied during publish)
if (app.Environment.IsDevelopment())
{
    string clientAppDistPath = Path.Combine(app.Environment.ContentRootPath, "ClientApp", "dist");
    if (Directory.Exists(clientAppDistPath))
    {
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientAppDistPath)
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientAppDistPath)
        });
    }
    else
    {
        // Fallback to wwwroot if dist doesn't exist
        app.UseDefaultFiles();
        app.UseStaticFiles();
    }
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseAuthorization();
app.MapControllers();

// Fallback to index.html for SPA routing
if (app.Environment.IsDevelopment())
{
    string clientAppDistPath = Path.Combine(app.Environment.ContentRootPath, "ClientApp", "dist");
    if (Directory.Exists(clientAppDistPath))
    {
        app.MapFallback(async context =>
        {
            string indexPath = Path.Combine(clientAppDistPath, "index.html");
            if (File.Exists(indexPath))
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(indexPath);
            }
        });
    }
    else
    {
        app.MapFallbackToFile("index.html");
    }
}
else
{
    app.MapFallbackToFile("index.html");
}

app.Run();
