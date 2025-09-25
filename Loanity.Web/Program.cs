
using Loanity.Web.Controllers.Auth;
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
    //client.BaseAddress = new Uri("http://localhost:5253/"); // use your API’s actual port
    client.BaseAddress = new Uri("http://10.130.56.53:5253/"); // use your API’s actual port

});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionTimerByMin); //  1 auto logout after 1 minute
        options.SlidingExpiration = true;
        options.LoginPath = "/Login";

        // This makes it expire when browser closes
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "LoanityAuth";
        options.Cookie.MaxAge = null; // <= Makes the cookie non-persistent!
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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
