using RatingTracker.Application.Services;
using RatingTracker.Infrastructure.SearchEngineCrawler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(); // instead of AddSwaggerGen

builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<ISearchEngineCrawlerFactory, SearchEngineCrawlerFactory>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();