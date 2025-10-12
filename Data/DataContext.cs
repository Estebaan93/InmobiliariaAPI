using Microsoft.EntityFrameworkCore;
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Data
{
  public class InmobiliariaContext: DbContext
  {
    public InmobiliariaContext(DbContextOptions<InmobiliariaContext> options) : base(options) { }
    public DbSet<Propietario> Propietarios { get; set; }
  }
}