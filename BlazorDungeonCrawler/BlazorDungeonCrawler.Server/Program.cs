using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Server.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<DungeonManager>();
builder.Services.AddDbContextFactory<DungeonDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BlazorDungeonCrawler")));

builder.Services.AddCors(options => {
    options.AddPolicy(name: "AllowAnyOriginMethodHeader", builder =>
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

//Logging
builder.Logging.ClearProviders();           // remove all previous logging providers
builder.Logging.AddConsole();               // add logging to console
builder.Logging.AddDebug();                 // add logging to debug window

// report logging to application insights
builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = "InstrumentationKey=8090da4c-d1ed-4233-aca7-1cdea70a4a3c;IngestionEndpoint=https://uksouth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://uksouth.livediagnostics.monitor.azure.com/",
        configureApplicationInsightsLoggerOptions: (options) => { }
    );

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAnyOriginMethodHeader");
app.UseResponseCompression();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();