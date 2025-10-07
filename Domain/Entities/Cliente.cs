using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entidades;

namespace Domain.Entities;

public class Cliente
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = null!;
    public string Telefono { get; private set; } = null!;
    public string Correo { get; private set; } = null!;
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new HashSet<Vehiculo>();
    private Cliente() { }
    public Cliente(string nombre, string telefono, string correo)
    { Nombre = nombre; Telefono = telefono; Correo = correo; }
    
    }