using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Medicina.Models;
using Medicina.Services;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.Extensions.Options;
using Medicina.Controllers;
using MongoDB.Driver;
using Medicina.Repository;



var builder = WebApplication.CreateBuilder(args);


var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSwaggerGen();


// Agregar Identity con MongoDB
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        builder.Configuration["MongoDbSettings:ConnectionString"],
        builder.Configuration["MongoDbSettings:DatabaseName"]
    )
    .AddDefaultTokenProviders();

// Agregar controladores para la Web API (sin MVC)
builder.Services.AddControllers();


// Configurar JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Reducir la desviación de tiempo para tokens
    };
});

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("desarrollo",
           builder =>
           {
               builder
                   .AllowAnyOrigin() // Permitir cualquier origen
                   .AllowAnyMethod() // Permitir cualquier método (GET, POST, PUT, DELETE, etc.)
                   .AllowAnyHeader(); // Permitir cualquier encabezado
           });
});


builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<AgendaRepository>();
builder.Services.AddScoped<CitaRepository>();
builder.Services.AddScoped<PacienteRepository>();
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("desarrollo");

// Habilitar autenticación y autorización
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();