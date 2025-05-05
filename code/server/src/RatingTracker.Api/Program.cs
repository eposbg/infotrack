using RatingTracker.Application.Services;
using RatingTracker.Infrastructure.ScraperServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(); // instead of AddSwaggerGen

builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<IScraperFactory, ScraperFactory>();

builder.Services.AddHttpClient<GoogleScraperService>();
builder.Services.AddHttpClient<BingScraperService>();
builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // 👈 adjust for your frontend port
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
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