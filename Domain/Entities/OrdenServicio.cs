using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entidades;
using Domain.Enums;

namespace Domain.Entities;
public class OrdenServicio
{
    private TipoServicio tipoServicio;
    private Estado estado;

    public Guid Id { get; private set; } = Guid.NewGuid();
    public TipoServicio TipoServicio { get; private set; }
    public DateTime FechaIngreso { get; private set; } = DateTime.UtcNow;
    public DateTime FechaEstimadaEntrega { get; private set; }
    public Estado Estado { get; private set; }

    public int? UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public int? VehiculoId { get; set; }
    public virtual Vehiculo? Vehiculo { get; set; }

    public virtual ICollection<DetalleOrden> DetallesOrdenes { get; set; } = new HashSet<DetalleOrden>();
    public virtual ICollection<Factura> Facturas { get; set; } = new HashSet<Factura>();
    private OrdenServicio(TipoServicio tipoServicio)
    {
        this.tipoServicio = tipoServicio;
    }
    public OrdenServicio(TipoServicio tipo_servicio, DateTime fecha_ingreso, Estado estado1, DateTime fecha_estimada_entrega, Estado estado)
    { TipoServicio = tipo_servicio; FechaIngreso = fecha_ingreso; FechaEstimadaEntrega = fecha_estimada_entrega; Estado = estado; }

    public OrdenServicio(TipoServicio tipoServicio, DateTime fechaEstimadaEntrega, Estado estado, Guid usuarioId, Guid vehiculoId)
    {
        this.tipoServicio = tipoServicio;
        FechaEstimadaEntrega = fechaEstimadaEntrega;
        this.estado = estado;
    }
}