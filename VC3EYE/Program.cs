using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VC3EYE;
using VC3EYE.Data;
using VC3EYE.Entities;
using VC3EYE.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.Configure<CookiePolicyOptions>(options => {
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "UserLoginCookie";
    options.LoginPath = "/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddDbContext<Vc3eyeContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("VC3EYEDB")), ServiceLifetime.Singleton);

builder.Services.AddScoped<LogsManager>();
builder.Services.AddScoped<Service>();
builder.Services.AddScoped<ServiceManager>();
builder.Services.AddScoped<ServicesThread>();
builder.Services.AddScoped<ServiceController>();
builder.Services.AddScoped<IndexModel>();

builder.Services.AddHostedService<ServicesThread>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
