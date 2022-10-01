using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Server.HttpSys;
using MyWishly.App.Models.Options;
using MyWishly.App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureAppConfiguration(b =>
{
    //Connect to your App Config Store using the connection string
    b.AddAzureAppConfiguration(builder.Configuration.GetConnectionString("AppConfig"));
});

builder.Services.AddOptions();
builder.Services.Configure<Connections>(builder.Configuration.GetSection(nameof(Connections)));
builder.Services.Configure<Cryptography>(builder.Configuration.GetSection(nameof(Cryptography)));

builder.Services.AddAzureAppConfiguration();

builder.Services.AddSingleton<ICryptographyService, CryptographyService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                    options.LoginPath = "/home/login";
                    options.LogoutPath = "/home/logout";
                });

builder.Services.AddMvc();

builder.Services.AddHsts(o =>
{
    o.MaxAge = TimeSpan.FromDays(180);
    o.Preload = true;
    o.IncludeSubDomains = true;
});

var app = builder.Build();

app.UseAzureAppConfiguration();
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.Strict
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
