using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES CONFIGURATION

// --------------------------------------
// Controllers (API endpoints)
// --------------------------------------
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });


// --------------------------------------
// Database (Azure SQL / SQL Server via EF Core)
// --------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// --------------------------------------
// JWT Configuration
// --------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Convert secret key into byte array for signing tokens
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Register JWT settings for dependency injection
builder.Services.Configure<JwtSettings>(jwtSettings);


// --------------------------------------
// Authentication (JWT Bearer)
// --------------------------------------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


// --------------------------------------
// Dependency Injection (Services)
// --------------------------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<RateLimitService>();


// --------------------------------------
// Swagger (API Documentation)
// --------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payments API",
        Version = "v1"
    });

    // JWT Authentication support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// --------------------------------------
// CORS Policy (Frontend Access)
// --------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://payments-rc.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion


var app = builder.Build();

#region MIDDLEWARE PIPELINE

// --------------------------------------
// Swagger (ONLY in Development)
// --------------------------------------

    app.UseSwagger();
    app.UseSwaggerUI();


// --------------------------------------
// Global Exception + Security Handling
// --------------------------------------

// ⚠️ DO NOT use DeveloperExceptionPage in production
// app.UseDeveloperExceptionPage(); ❌ removed for production safety

app.UseHsts();
app.UseHttpsRedirection();


// --------------------------------------
// CORS (must come BEFORE auth)
// --------------------------------------
app.UseCors("AllowReactApp");


// --------------------------------------
// Custom Middleware
// --------------------------------------
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();


// --------------------------------------
// Authentication & Authorization
// IMPORTANT ORDER:
// Authentication FIRST → Authorization SECOND
// --------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// --------------------------------------
// Map Controllers (API routes)
// --------------------------------------
app.MapControllers();

#endregion

app.Run();