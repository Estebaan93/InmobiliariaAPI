//Program.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Repositories;
using InmobiliariaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

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
builder.Services.AddScoped<IInmuebleRepositorio, InmuebleRepositorio>();
builder.Services.AddScoped<IContratoRepositorio, ContratoRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>(); //

builder.Services.AddScoped<JwtService>();


//  CONFIGURACION JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{

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

  //Evento para invalidar token al cambiar la contraseña
  options.Events = new JwtBearerEvents
  {
    OnTokenValidated = async context =>
    {
      try
      {
        // Obtenemos el contexto de BD y datos del token
        var db = context.HttpContext.RequestServices.GetRequiredService<InmobiliariaContext>();
        var userId = int.Parse(context.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
        var lastChangeClaim = context.Principal.FindFirst("LastPasswordChange")?.Value;

        if (DateTime.TryParse(lastChangeClaim, out var tokenIssuedTime))
        {
          var user = await db.Propietarios.FindAsync(userId);
          // Si el usuario cambió la contraseña después de emitirse el token → invalida
          if (user != null && user.UltimoCambioPassword > tokenIssuedTime)
          {
            context.Fail("Token expirado por cambio de contraseña.");
          }
        }
      }
      catch
      {
        // Si algo falla en la validación personalizada, también invalida por seguridad
        context.Fail("Error al validar el token.");
      }
    }
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

//Habilitamos carga de img desde /wwwroot
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
