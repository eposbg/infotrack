using RatingTracker.Application.Services;
using RatingTracker.Domain.Settings;
using RatingTracker.Infrastructure.ScraperServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(); 

builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<IScraperFactory, ScraperFactory>();

builder.Services.AddHttpClient<GoogleScraperService>();
builder.Services.AddHttpClient<BingScraperService>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

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



var app = builder.Build();

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