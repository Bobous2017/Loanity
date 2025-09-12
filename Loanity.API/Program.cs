using Loanity.Domain;
using Loanity.Domain.IServices;
using Loanity.Infrastructure;
using Loanity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with SQLite
builder.Services.AddDbContext<LoanityDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LoanityDb")));

builder.Services.AddHttpClient("LoanityApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5253/"); // use your API’s actual port


});



// Register your services for DI
builder.Services.AddScoped<ILoanService, LoanService>();

// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
