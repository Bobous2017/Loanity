
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register HttpClient for API calls
builder.Services.AddHttpClient("LoanityApi", client =>
{
    client.BaseAddress = new Uri("http://10.130.56.53:5253/"); // use your API’s actual port



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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
