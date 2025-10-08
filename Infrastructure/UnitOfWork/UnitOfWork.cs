using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Abstractions.Auth;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories.Auth;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IClienteRepository? _clienteRepository;
    private IDetalleOrdenRepository? _detalleOrdenRepository;
    private IFacturaRepository? _facturaRepository;
    private IMarcaRepository? _marcaRepository;
    private IOrdenServicioRepository? _ordenServicioRepository;
    private IPagoRepository? _pagoRepository;
    private IRepuestoRepository? _repuestoRepository;
    private IUsuarioRepository? _usuarioRepository;
    private IVehiculoRepository? _vehiculoRepository;
    private IModeloRepository? _modeloRepository;
    private IRolService? _rolService;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public Task<int> SaveChanges(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
    public IClienteRepository Clientes => _clienteRepository ??= new ClienteRepository(_context);
    public IDetalleOrdenRepository DetallesOrdenes => _detalleOrdenRepository ??= new DetalleOrdenRepository(_context);
    public IFacturaRepository Facturas => _facturaRepository ??= new FacturaRepository(_context);
    public IMarcaRepository Marcas => _marcaRepository ??= new MarcaRepository(_context);
    public IOrdenServicioRepository OrdenesServicios => _ordenServicioRepository ??= new OrdenServicioRepository(_context);
    public IPagoRepository Pagos => _pagoRepository ??= new PagoRepository(_context);
    public IRepuestoRepository Repuestos => _repuestoRepository ??= new RepuestoRepository(_context);
    public IVehiculoRepository Vehiculos => _vehiculoRepository ??= new VehiculoRepository(_context);
    public IModeloRepository Modelos => _modeloRepository ??= new ModeloRepository(_context);
    public IUsuarioRepository Usuarios => _userService ??= new UsuarioRepository(_context);
    public IRolService Roles => _rolService ??= new RolRepository(_context);
    public IUserMemberRolService UserMemberRoles => throw new NotImplementedException();
}