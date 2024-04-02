using Aspirations.ApiService.Endpoints;
using Aspirations.ApiService.Storage;
using Aspirations.ApiService.Storage.Repositories;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Aspirations API"
    });
});

// Add services to the container.
builder.Services
    .AddProblemDetails()
    .AddDbContext<ApiContext>()
    .AddScoped<IQuoteRepository, QuoteRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints()
    .MapQuotesEndpoints();

app.Run();