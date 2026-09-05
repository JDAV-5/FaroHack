using System.Text;
using Faro.DataAccess;
using Faro.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// Controllers
// ==========================================

builder.Services.AddControllers();


// ==========================================
// PostgreSQL
// ==========================================

builder.Services.AddDbContext<FaroDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});


// ==========================================
// Services
// ==========================================

builder.Services.AddScoped<IAuthService, AuthService>();


// ==========================================
// JWT
// ==========================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("Jwt:Key no está configurado.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();


// ==========================================
// Swagger
// ==========================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// ==========================================
// HTTP Pipeline
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// IMPORTANTE:
// Authentication debe ir antes de Authorization

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();