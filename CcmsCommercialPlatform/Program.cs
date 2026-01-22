using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CCMS Commercial Platform API", Version = "v1" });
});

// Configure database provider (SQLite or SqlServer)
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SQLite";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
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

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
