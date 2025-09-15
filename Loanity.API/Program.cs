using Loanity.Domain;
using Loanity.Domain.IServices;
using Loanity.Infrastructure;
using Loanity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with SQLite
//builder.Services.AddDbContext<LoanityDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("LoanityDb")));

// Configure Sql Server
builder.Services.AddDbContext<LoanityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoanityDb")));


builder.Services.AddHttpClient("LoanityApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5253/"); // use your API’s actual port


});

// Register your services for Dependency Injection
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IReservationService, ReservationService>(); // Step 4



// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loanity API v1");
    });
}


// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
