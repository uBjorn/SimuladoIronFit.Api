using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimuladoIronFit.Api.Data;
using SimuladoIronFit.Api.Security;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var adminEmail = builder.Configuration["Admin:Email"]?.Trim();
var adminPassword = builder.Configuration["Admin:Password"];

if (string.IsNullOrWhiteSpace(adminEmail) ||
    string.IsNullOrWhiteSpace(adminPassword))
{
    throw new InvalidOperationException(
        "Configure Admin:Email e Admin:Password."
    );
}

if (adminPassword.Length < 12)
{
    throw new InvalidOperationException(
        "A senha administrativa deve ter pelo menos 12 caracteres."
    );
}

var passwordHasher = new PasswordHasher<AdminCredenciais>();

var admin = new AdminCredenciais
{
    Email = adminEmail
};

admin.PasswordHash = passwordHasher.HashPassword(
    admin,
    adminPassword
);

builder.Services.AddSingleton(admin);

builder.Services.AddSingleton<IPasswordHasher<AdminCredenciais>>(
    passwordHasher
);

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Key com pelo menos 32 bytes."
    );
}

if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Configure Jwt:Issuer e Jwt:Audience."
    );
}

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
});

builder.Services.AddAuthorization();

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
