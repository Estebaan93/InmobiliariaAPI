using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IPagoRepositorio
  {
    IEnumerable<Pago> ObtenerPorContrato(int idContrato);
  }
}
