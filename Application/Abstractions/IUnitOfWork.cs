using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Abstractions;
public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    IDetalleOrdenRepository DetallesOrdenes { get; }
    IFacturaRepository Facturas { get; }
    IMarcaRepository Marcas { get; }
    IOrdenServicioRepository OrdenesServicios { get; }
    IPagoRepository Pagos { get; }
    IRepuestoRepository Repuestos { get; }
    IUsuarioRepository Usuarios { get; }
    IVehiculoRepository Vehiculos { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChanges(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}