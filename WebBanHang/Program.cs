using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Stripe;
using WebBanHang.Data;
using WebBanHang.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Load your ML.NET model
string modelPath = Path.Combine(builder.Environment.ContentRootPath, "RecommendationProduct.mlnet");
MLContext mlContext = new MLContext();
ITransformer mlModel = mlContext.Model.Load(modelPath, out DataViewSchema modelSchema);

// Register the ML.NET model as a singleton service
builder.Services.AddSingleton(mlModel);

// Stripe configuration
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(9999999999);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/KhachHang/Login";
    options.AccessDeniedPath = "/AcessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();  // This will show detailed error messages
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
app.UseDeveloperExceptionPage();
//app.UseMigrationsEndPoint();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add UseSession() here

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure your endpoints
#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
});

app.Run();

