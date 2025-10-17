//Program.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Repositories;
using InmobiliariaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




// CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
  policy.AllowAnyOrigin()   // Permite cualquier dominio
          .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE, etc.
          .AllowAnyHeader()); // Permite cualquier encabezado
});



//  CONFIGURACION GENERAL
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//  CONEXION A MySQL
builder.Services.AddDbContext<InmobiliariaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    )
);


//  INYECCION DE DEPENDENCIAS
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<JwtService>();


//  CONFIGURACION JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(key)
  };
});

builder.Services.AddAuthorization();


//  CONSTRUCCION APP
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}



// MIDDLEWARES
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();