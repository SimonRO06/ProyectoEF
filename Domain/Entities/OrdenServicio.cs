using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Auth;
using Domain.Enums;

namespace Domain.Entities;
public class OrdenServicio
{

    public Guid Id { get; private set; } = Guid.NewGuid();
        public TipoServicio TipoServicio { get; private set; }
        public DateTime FechaIngreso { get; private set; } = DateTime.UtcNow;
        public DateTime FechaEstimadaEntrega { get; private set; }
        public Estado Estado { get; private set; }
http://localhost:5000/api/ordenes
        public int UserMemberId { get; set; }
        public virtual UserMember? UserMember { get; set; }
        public Guid VehiculoId { get; set; }
        public virtual Vehiculo? Vehiculo { get; set; }

        public virtual ICollection<DetalleOrden> DetallesOrdenes { get; set; } = new HashSet<DetalleOrden>();
        public virtual ICollection<Factura> Facturas { get; set; } = new HashSet<Factura>();

        private OrdenServicio() { }
    public OrdenServicio(TipoServicio tipoServicio, DateTime fechaIngreso, DateTime fechaEstimadaEntrega, Estado estado, int userMemberId, Guid vehiculoId)
    {
        TipoServicio = tipoServicio;
        FechaIngreso = fechaIngreso;
        FechaEstimadaEntrega = fechaEstimadaEntrega;
        Estado = estado;
        UserMemberId = userMemberId;
        VehiculoId = vehiculoId;
    }
        
    public void Update(TipoServicio tipoServicio, DateTime fechaIngreso, DateTime fechaEstimadaEntrega, Estado estado)
    {
        TipoServicio = tipoServicio;
        FechaIngreso = fechaIngreso;
        FechaEstimadaEntrega = fechaEstimadaEntrega;
        Estado = estado;
    }

}