using Microsoft.AspNetCore.ResponseCompression;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Server.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<DungeonManager>();
builder.Services.AddSingleton<DungeonDbContext>();

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