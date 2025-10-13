const logoutBtn = document.getElementById("logoutBtn");

// URLs de la API basadas en tu backend
const API_BASE = "http://localhost:5000/api";
const API_URLS = {
  clientes: `${API_BASE}/clientes`,
  vehiculos: `${API_BASE}/vehiculos`, 
  ordenes: `${API_BASE}/ordenesservicio`,
  repuestos: `${API_BASE}/repuestos`,
  facturas: `${API_BASE}/facturas`
};

// 🚀 Inicialización
document.addEventListener("DOMContentLoaded", () => {
  console.log("🚀 Inicializando Dashboard...");
  if (!verificarAutenticacion()) return;
  cargarUsuario();
  cargarEstadisticas();
  cargarOrdenesRecientes();
  cargarStockBajo();
});

// 🔐 Verificar autenticación
function verificarAutenticacion() {
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('userRole');
  
  if (!token) {
    window.location.href = 'login.html';
    return false;
  }
  
  if (userRole !== 'Administrador') {
    alert('No tienes permisos para acceder al panel de administración');
    window.location.href = 'login.html';
    return false;
  }
  
  return true;
}

// 👤 Cargar información del usuario
function cargarUsuario() {
  const userName = localStorage.getItem('userName');
  if (userName) {
    document.getElementById('userName').textContent = userName;
  }
}

// 📊 Cargar estadísticas principales desde tu base de datos
async function cargarEstadisticas() {
  try {
    const token = localStorage.getItem('token');
    
    console.log("📊 Cargando estadísticas...");
    
    // Obtener todos los clientes
    const clientesRes = await fetch(API_URLS.clientes, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    let totalClientes = 0;
    if (clientesRes.ok) {
      const clientes = await clientesRes.json();
      totalClientes = clientes.length;
      console.log(`👥 Clientes encontrados: ${totalClientes}`);
    }

    // Obtener todos los vehículos
    const vehiculosRes = await fetch(API_URLS.vehiculos, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    let totalVehiculos = 0;
    if (vehiculosRes.ok) {
      const vehiculos = await vehiculosRes.json();
      totalVehiculos = vehiculos.length;
      console.log(`🚗 Vehículos encontrados: ${totalVehiculos}`);
    }

    // Obtener todas las órdenes de servicio
    const ordenesRes = await fetch(API_URLS.ordenes, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    let ordenesActivas = 0;
    if (ordenesRes.ok) {
      const todasOrdenes = await ordenesRes.json();
      ordenesActivas = todasOrdenes.filter(orden => 
        orden.estado === 'Pendiente' || orden.estado === 'EnProgreso'
      ).length;
      console.log(`📋 Órdenes activas: ${ordenesActivas}`);
    }

    // Obtener todas las facturas para calcular ingresos del mes
    const facturasRes = await fetch(API_URLS.facturas, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    let ingresosMes = 0;
    if (facturasRes.ok) {
      const todasFacturas = await facturasRes.json();
      const mesActual = new Date().getMonth();
      const añoActual = new Date().getFullYear();
      
      ingresosMes = todasFacturas
        .filter(factura => {
          if (!factura.fechaEmision) return false;
          const fechaFactura = new Date(factura.fechaEmision);
          return fechaFactura.getMonth() === mesActual && 
                 fechaFactura.getFullYear() === añoActual;
        })
        .reduce((total, factura) => total + (factura.total || 0), 0);
      
      console.log(`💰 Ingresos del mes: $${ingresosMes}`);
    }

    // Actualizar UI con datos reales de tu base de datos
    document.getElementById('totalClientes').textContent = totalClientes;
    document.getElementById('totalVehiculos').textContent = totalVehiculos;
    document.getElementById('ordenesActivas').textContent = ordenesActivas;
    document.getElementById('ingresosMes').textContent = `$${formatearNumero(ingresosMes)}`;

    console.log("✅ Estadísticas cargadas correctamente");

  } catch (error) {
    console.error("❌ Error cargando estadísticas:", error);
    
    // Fallback en caso de error
    document.getElementById('totalClientes').textContent = "0";
    document.getElementById('totalVehiculos').textContent = "0";
    document.getElementById('ordenesActivas').textContent = "0";
    document.getElementById('ingresosMes').textContent = "$0";
    
    mostrarError("Error al cargar estadísticas: " + error.message);
  }
}

// 📋 Cargar órdenes recientes desde tu base de datos
async function cargarOrdenesRecientes() {
  try {
    const token = localStorage.getItem('token');
    const tbody = document.querySelector('#tablaOrdenesRecientes tbody');
    
    console.log("📋 Cargando órdenes recientes...");
    
    const res = await fetch(API_URLS.ordenes, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!res.ok) {
      throw new Error(`Error ${res.status} al cargar órdenes`);
    }

    const todasOrdenes = await res.json();
    console.log(`📦 Total de órdenes encontradas: ${todasOrdenes.length}`);
    
    // Ordenar por fecha más reciente y tomar las últimas 5
    const ordenesRecientes = todasOrdenes
      .sort((a, b) => new Date(b.fechaIngreso) - new Date(a.fechaIngreso))
      .slice(0, 5);

    if (ordenesRecientes.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="5" style="text-align: center; color: #666;">
            No hay órdenes de servicio registradas
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = ordenesRecientes.map(orden => {
      // Formatear datos según tus entidades
      const ordenId = orden.id ? orden.id.substring(0, 8).toUpperCase() : 'N/A';
      
      // Acceder a los datos relacionados según tu estructura de entidades
      const clienteNombre = orden.vehiculo?.cliente?.nombre || 'Cliente no asignado';
      const vehiculoInfo = orden.vehiculo ? 
        `${orden.vehiculo.modelo?.marca?.nombre || ''} ${orden.vehiculo.modelo?.nombre || ''}`.trim() : 
        'Vehículo no asignado';
      
      const fechaFormateada = orden.fechaIngreso ? 
        new Date(orden.fechaIngreso).toLocaleDateString('es-ES') : 'N/A';

      const estadoClase = orden.estado ? 
        `estado-${orden.estado.toLowerCase().replace(' ', '')}` : 'estado-pendiente';

      const estadoTexto = obtenerTextoEstado(orden.estado);

      return `
        <tr>
          <td><strong>${ordenId}</strong></td>
          <td>${clienteNombre}</td>
          <td>${vehiculoInfo}</td>
          <td>
            <span class="estado-badge ${estadoClase}">
              ${estadoTexto}
            </span>
          </td>
          <td>${fechaFormateada}</td>
        </tr>
      `;
    }).join('');

    console.log("✅ Órdenes recientes cargadas:", ordenesRecientes.length);

  } catch (error) {
    console.error("❌ Error cargando órdenes recientes:", error);
    
    const tbody = document.querySelector('#tablaOrdenesRecientes tbody');
    tbody.innerHTML = `
      <tr>
        <td colspan="5" style="text-align: center; color: red;">
          Error al cargar órdenes: ${error.message}
        </td>
      </tr>
    `;
  }
}

// 📦 Cargar repuestos con stock bajo desde tu base de datos
async function cargarStockBajo() {
  try {
    const token = localStorage.getItem('token');
    const tbody = document.querySelector('#tablaStockBajo tbody');
    
    console.log("📦 Cargando repuestos con stock bajo...");
    
    const res = await fetch(`${API_URLS.repuestos}/all`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!res.ok) {
      throw new Error(`Error ${res.status} al cargar repuestos`);
    }

    const todosRepuestos = await res.json();
    console.log(`🔧 Total de repuestos encontrados: ${todosRepuestos.length}`);
    
    // Filtrar repuestos con stock bajo (<= 5 unidades) según tu entidad Repuesto
    const stockBajo = todosRepuestos.filter(repuesto => {
      const stockActual = repuesto.cantidadStock || 0;
      return stockActual <= 5;
    });

    if (stockBajo.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="4" style="text-align: center; color: #059669;">
            ✅ Todo el stock está en niveles normales
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = stockBajo.map(repuesto => {
      const stockActual = repuesto.cantidadStock || 0;
      const stockMinimo = 5; // Puedes hacer esto configurable
      const estado = stockActual === 0 ? 'Crítico' : 
                    stockActual <= 2 ? 'Muy Bajo' : 'Bajo';
      const estadoClase = stockActual === 0 ? 'estado-critico' : 
                         stockActual <= 2 ? 'estado-muybajo' : 'estado-bajo';

      return `
        <tr>
          <td>
            <strong>${repuesto.codigo || 'Sin código'}</strong>
            <div style="font-size: 12px; color: #666;">${repuesto.descripcion || 'Sin descripción'}</div>
          </td>
          <td>${stockActual}</td>
          <td>${stockMinimo}</td>
          <td>
            <span class="estado-badge ${estadoClase}">
              ${estado}
            </span>
          </td>
        </tr>
      `;
    }).join('');

    console.log("✅ Stock bajo cargado:", stockBajo.length);

  } catch (error) {
    console.error("❌ Error cargando stock bajo:", error);
    
    const tbody = document.querySelector('#tablaStockBajo tbody');
    tbody.innerHTML = `
      <tr>
        <td colspan="4" style="text-align: center; color: red;">
          Error al cargar stock: ${error.message}
        </td>
      </tr>
    `;
  }
}

// 🛠️ Funciones auxiliares

function obtenerTextoEstado(estado) {
  const estados = {
    'Pendiente': 'Pendiente',
    'EnProgreso': 'En Progreso', 
    'Finalizado': 'Finalizado',
    'Cancelado': 'Cancelado'
  };
  return estados[estado] || estado;
}

function formatearNumero(numero) {
  return new Intl.NumberFormat('es-ES', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(numero);
}

function mostrarError(mensaje) {
  // Puedes implementar un sistema de notificaciones más elegante
  console.error("💥 Error:", mensaje);
}

// 🚪 Cerrar sesión
logoutBtn.addEventListener("click", () => {
  console.log("🚪 Cerrando sesión...");
  localStorage.clear();
  window.location.href = "login.html";
});

// 🔄 Auto-actualización cada 2 minutos
setInterval(() => {
  if (document.visibilityState === 'visible') {
    console.log("🔄 Actualizando dashboard...");
    cargarEstadisticas();
    cargarOrdenesRecientes();
    cargarStockBajo();
  }
}, 120000);