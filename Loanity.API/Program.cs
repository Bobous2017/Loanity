using Loanity.Domain;
using Loanity.Domain.IServices;
using Loanity.Infrastructure;
using Loanity.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Configuration; // Add this at the top

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings from config
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // only for development!
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtConfig["Audience"],
        ValidateLifetime = true
    };
});

// Configure Sql Server
builder.Services.AddDbContext<LoanityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoanityDb")));


builder.Services.AddHttpClient("LoanityApi", client =>
{
    var baseAddress = System.Configuration.ConfigurationManager.AppSettings["ApiBaseAddress"];
    client.BaseAddress = new Uri(baseAddress);
});

// Register your services for Dependency Injection
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IReservationService, ReservationService>(); // Step 4
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();


// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//  Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5191")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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
app.UseCors("AllowFrontend"); // Enable CORS
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
