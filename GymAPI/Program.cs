using GymAPI.Repositories;
using GymAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using GymAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

//ESTE ES EL LOCAL
// 🔹 Configuración JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings no está configurado");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// 🔹 Registrar JwtSettings como servicio
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 🔹 Configurar EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// 🔹 Obtener la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("GymappDB");

// 🔹 Registrar los repositorios con la cadena de conexión
builder.Services.AddScoped<IUsuarioRepository>(provider =>
    new UsuarioRepository(connectionString));

builder.Services.AddScoped<IEntrenamientoRepository>(provider =>
    new EntrenamientoRepository(connectionString));

builder.Services.AddScoped<IEjercicioRepository>(provider =>
    new EjercicioRepository(connectionString));

builder.Services.AddScoped<IEntrenamientoEjercicioRepository>(provider =>
    new EntrenamientoEjercicioRepository(connectionString));

builder.Services.AddScoped<IComentarioRepository>(provider =>
    new ComentarioRepository(connectionString));

builder.Services.Configure<GoogleAuthSettings>(
    builder.Configuration.GetSection("Authentication:Google"));
    




// Registrar el servicio de MedidaCorporal
builder.Services.AddScoped<IMedidaCorporalService, MedidaCorporalService>();

// Registrar el repositorio de MedidaCorporal
builder.Services.AddScoped<IMedidaCorporalRepository>(provider =>
    new MedidaCorporalRepository(connectionString));




// Añadir estas líneas después de registrar los demás repositorios
builder.Services.AddScoped<IFavoritoRepository>(provider =>
    new FavoritoRepository(connectionString));

// Añadir esta línea después de registrar los demás servicios
builder.Services.AddScoped<IFavoritoService, FavoritoService>();





// 🔹 Registrar los servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEntrenamientoService, EntrenamientoService>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();
builder.Services.AddScoped<IEntrenamientoEjercicioService, EntrenamientoEjercicioService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔹 Configurar Swagger para autenticación - SIEMPRE habilitado
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "GymAPI", 
        Version = "v1",
        Description = "API para aplicación de entrenamiento personal" 
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token en el formato: Bearer {token}"
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

// 🔹 Configurar CORS para permitir Vue - Corregido para evitar el error con AllowCredentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowVueApp");

// 🔹 Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymAPI v1");
    // Opcional: Hacer que Swagger UI sea la página de inicio
    c.RoutePrefix = string.Empty;
});

// Solo redirigir a HTTPS en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 🔹 Añadir middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();