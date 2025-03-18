using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CatDogBearMicroservice.Models;
using CatDogBearMicroservice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<PictureService>();
builder.Services.AddDbContext<PictureDbContext>(options =>
    options.UseInMemoryDatabase("CatDogBearDb")); // Use in-memory database
builder.Services.AddAuthorization();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Set the URLs the application should listen on
builder.WebHost.UseKestrel(serverOptions =>
    serverOptions.Listen(System.Net.IPAddress.Parse("0.0.0.0"), 80) // HTTP port
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();