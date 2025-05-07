using Microsoft.EntityFrameworkCore;
using RatingTracker.Application.Services;
using RatingTracker.Domain.Repositories;
using RatingTracker.Domain.Settings;
using RatingTracker.Infrastructure.Data;
using RatingTracker.Infrastructure.Data.Repositories;
using RatingTracker.Infrastructure.ScraperServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

// Services
builder.Services.AddTransient<IRankingService, RankingService>();

// Scraping Services
builder.Services.AddTransient<IScraperFactory, ScraperFactory>();
builder.Services.AddHttpClient<GoogleScraperService>();
builder.Services.AddHttpClient<BingScraperService>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

// Repositories
builder.Services.AddTransient<IRankingRepository, RankingRepository>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<SearchEngineOptions>(builder.Configuration.GetSection("SearchEngines"));


builder.Services.AddDbContext<RankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RankingDbContext>();
    db.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RankingDbContext>();
    db.Database.Migrate(); 
    DbInitializer.Seed(db); 
}

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAngularDev");

app.MapControllers();

app.Run();