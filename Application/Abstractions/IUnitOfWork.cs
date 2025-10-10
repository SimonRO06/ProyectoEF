using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Auth;

namespace Application.Abstractions;
public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    IDetalleOrdenRepository DetallesOrdenes { get; }
    IFacturaRepository Facturas { get; }
    IMarcaRepository Marcas { get; }
    IModeloRepository Modelos { get; }
    IOrdenServicioRepository OrdenesServicios { get; }
    IPagoRepository Pagos { get; }
    IRepuestoRepository Repuestos { get; }
    IUserMemberService UserMembers { get; }
    IVehiculoRepository Vehiculos { get; }
    IUserMemberRolService UserMemberRoles { get; }
    IRolService Roles { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}