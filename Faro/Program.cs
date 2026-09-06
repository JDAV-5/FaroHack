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

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "ConnectionStrings:DefaultConnection no está configurado."
    );
}

builder.Services.AddDbContext<FaroDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


// ==========================================
// Services
// ==========================================

builder.Services.AddScoped<IAuthService, AuthService>();


// ==========================================
// JWT
// ==========================================

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception(
        "Jwt:Key no está configurado."
    );
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new Exception(
        "Jwt:Issuer no está configurado."
    );
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception(
        "Jwt:Audience no está configurado."
    );
}

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
// CORS
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("FaroCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ==========================================
// Swagger
// ==========================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// ==========================================
// HTTP Pipeline
// ==========================================

// Swagger habilitado también en producción
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("FaroCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();