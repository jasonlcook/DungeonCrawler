using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Server.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<AccessTokenManager>();
builder.Services.AddSingleton<DungeonManager>();

builder.Services.AddDbContextFactory<DungeonDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BlazorDungeonCrawler")));

builder.Services.AddLocalization(options => {
    options.ResourcesPath = "Resources";
});

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
    configureTelemetryConfiguration: (config) => {
        config.ConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
    },
    configureApplicationInsightsLoggerOptions: (options) => { }
);

WebApplication? app = builder.Build();

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

//app.MapControllerRoute(name: "accesstoken", pattern: "api/{controller=AccessToken}");
//app.MapControllerRoute(name: "default", pattern: "api/{controller=Dungeon}");


app.MapFallbackToPage("/_Host");

app.Run();