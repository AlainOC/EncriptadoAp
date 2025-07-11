using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using EncriptadoApi.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Encriptado API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT como: Bearer {su token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Agregar autenticación JWT
var claveJwt = "super_clave_jwt_12345_1234567890_segura";
var emisor = "encriptadoapi";
var audiencia = "encriptadoapi_usuarios";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = emisor,
            ValidAudience = audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt))
        };
    });

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo",
        policy => policy.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EncriptadoApi.Repositories.MensajeRepository>();
builder.Services.AddScoped<EncriptadoApi.Services.MensajeService>();

var app = builder.Build();

// Middleware para restringir acceso a Swagger UI solo a la IP de la escuela
app.Use(async (context, next) =>
{
    var swaggerPaths = new[] { "/swagger", "/swagger/index.html", "/swagger/v1/swagger.json" };
    var isSwagger = swaggerPaths.Any(p => context.Request.Path.StartsWithSegments(p));
    string remoteIp = context.Connection.RemoteIpAddress?.ToString();

    // Si hay un header X-Forwarded-For, úsalo
    if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
    {
        remoteIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0]?.Trim();
    }

    if (isSwagger && remoteIp != "187.155.101.200")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Acceso denegado: solo permitido desde la IP de la escuela.");
        return;
    }
    await next();
});

// Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("PermitirTodo");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
