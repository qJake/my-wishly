using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using MyWishly.App.Models.Options;
using MyWishly.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(b =>
{
    b.AddAzureAppConfiguration(options =>
    {
        options.Connect(builder.Configuration.GetConnectionString("AppConfig"))
               .Select(KeyFilter.Any, LabelFilter.Null)
               .Select(KeyFilter.Any, builder.Environment.EnvironmentName);
    });
});

builder.Services.AddOptions();
builder.Services.Configure<Connections>(builder.Configuration.GetSection(nameof(Connections)));
builder.Services.Configure<Cryptography>(builder.Configuration.GetSection(nameof(Cryptography)));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));

builder.Services.AddAzureAppConfiguration();

builder.Services.AddSingleton<ICryptographyService, CryptographyService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IItemsService, ItemsService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IMailService, MailService>();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
