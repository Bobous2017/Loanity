using Loanity.Web.Controllers.Auth;
using Loanity.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
var sessionTimer = SessionTimer.SessionTimeoutSeconds;
var sessionTimerByMin = sessionTimer / 60;
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(sessionTimerByMin);  // 5 min
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Register HttpClient for API calls
builder.Services.AddHttpClient("LoanityApi", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://10.130.56.53:5253/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<NotificationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionTimerByMin); //  1 auto logout after 1 minute
        options.SlidingExpiration = true;
        options.LoginPath = "/Login";

        // This makes it expire when browser closes
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "LoanityAuth";
        options.Cookie.MaxAge = null; // <= Makes the cookie non-persistent!
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                // let API calls get a 401 (no HTML redirect)
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }

                // otherwise go to the session-expired page
                var returnUrl = Uri.EscapeDataString(ctx.Request.Path + ctx.Request.QueryString);
                ctx.Response.Redirect($"/session-expired?returnUrl={returnUrl}");
                return Task.CompletedTask;
            }
        };

    });


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
