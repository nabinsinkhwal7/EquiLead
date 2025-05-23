using EquidCMS.Controllers;
using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var connectionString = builder.Configuration.GetConnectionString("EquiDB");
builder.Services.AddDbContext<EquiDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddScoped<EmailService>();
builder.Services.Configure<EmailService>(builder.Configuration.GetSection("EmailSettings"));

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(240);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHostedService<NotificationScheduler>(); // add for notification scheduler
builder.Services.AddScoped<NotificationController>();
builder.Services.AddHttpClient<JobScraperService>();


// Configure file upload limit (set max file size for multipart uploads)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600;  // 100 MB limit (in bytes), adjust as necessary
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

}
app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Place UseCors between UseRouting and UseEndpoints
app.UseCors("CorsPolicy");

// Ensure session comes after CORS and before authorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=landingpagenw}/{id?}");

app.Run();