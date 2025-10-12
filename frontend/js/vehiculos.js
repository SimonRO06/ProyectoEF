const VEHICULOS_API = "http://localhost:5000/api/vehiculos";
const CLIENTES_API = "http://localhost:5000/api/clientes/all"; // 🔧 Nueva URL para clientes
const MODELOS_API = "http://localhost:5000/api/modelos/all";   // 🔧 Nueva URL para modelos

// Referencias del DOM
const modal = document.getElementById("modalVehiculo");
const nuevoVehiculoBtn = document.getElementById("nuevoVehiculoBtn");
const cancelarBtn = document.getElementById("cancelarBtn");
const vehiculoForm = document.getElementById("vehiculoForm");
const vehiculosTableBody = document.getElementById("vehiculosTableBody");
const logoutBtn = document.getElementById("logoutBtn");

// Mostrar modal
nuevoVehiculoBtn.addEventListener("click", () => {
  modal.classList.remove("hidden");
  console.log("📝 Abriendo modal para nuevo vehículo");
});

// Cerrar modal
cancelarBtn.addEventListener("click", () => {
  modal.classList.add("hidden");
  console.log("❌ Modal cerrado");
});

// Cargar vehículos
async function cargarVehiculos() {
  try {
    console.log("🔄 Cargando vehículos...");
    const res = await fetch("http://localhost:5000/api/vehiculos/all");
    
    if (!res.ok) {
      throw new Error(`Error ${res.status}: ${res.statusText}`);
    }
    
    const data = await res.json();
    console.log("🚗 Vehículos cargados:", data);

    vehiculosTableBody.innerHTML = "";
    
    if (data.length === 0) {
      vehiculosTableBody.innerHTML = `
        <tr>
          <td colspan="7" style="text-align: center; color: #666;">
            No hay vehículos registrados. ¡Agrega el primero!
          </td>
        </tr>
      `;
      return;
    }

    data.forEach(v => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${v.id || 'N/A'}</td>
        <td>${v.año || 'N/A'}</td>
        <td>${v.numeroSerie || 'N/A'}</td>
        <td>${v.kilometraje || 0} km</td>
        <td>${v.cliente?.nombre || v.clienteId || '—'}</td>
        <td>${v.modelo?.nombre || v.modeloId || '—'}</td>
        <td>
          <button class="btn-editar" onclick="editarVehiculo('${v.id}')" title="Editar vehículo">
            <i class="fas fa-edit"></i> Editar
          </button>
          <button class="btn-eliminar" onclick="eliminarVehiculo('${v.id}')" title="Eliminar vehículo">
            <i class="fas fa-trash"></i> Eliminar
          </button>
        </td>
      `;
      vehiculosTableBody.appendChild(fila);
    });
  } catch (error) {
    console.error("❌ Error cargando vehículos:", error);
    vehiculosTableBody.innerHTML = `
      <tr>
        <td colspan="7" style="text-align: center; color: red;">
          Error al cargar vehículos: ${error.message}
        </td>
      </tr>
    `;
  }
}

// Cargar clientes
async function cargarClientes() {
  try {
    console.log("🔄 Cargando clientes...");
    const res = await fetch(CLIENTES_API);
    
    if (!res.ok) {
      throw new Error(`Error ${res.status}: ${res.statusText}`);
    }
    
    const clientes = await res.json();
    const select = document.getElementById("cliente");

    console.log("👥 Clientes cargados:", clientes);

    // Limpiar select excepto la primera opción
    select.innerHTML = '<option value="">Seleccione un cliente</option>';
    
    clientes.forEach(c => {
      const option = document.createElement("option");
      option.value = c.id;
      option.textContent = `${c.nombre} - ${c.telefono}`; // Mostrar nombre y teléfono
      select.appendChild(option);
    });
  } catch (error) {
    console.error("❌ Error cargando clientes:", error);
    const select = document.getElementById("cliente");
    select.innerHTML = '<option value="">Error al cargar clientes</option>';
  }
}

// Cargar modelos
async function cargarModelos() {
  try {
    console.log("🔄 Cargando modelos...");
    const res = await fetch(MODELOS_API);
    
    if (!res.ok) {
      throw new Error(`Error ${res.status}: ${res.statusText}`);
    }
    
    const modelos = await res.json();
    const select = document.getElementById("modelo");

    console.log("🚙 Modelos cargados:", modelos);

    // Limpiar select excepto la primera opción
    select.innerHTML = '<option value="">Seleccione un modelo</option>';
    
    modelos.forEach(m => {
      const option = document.createElement("option");
      option.value = m.id;
      option.textContent = m.nombre;
      select.appendChild(option);
    });
  } catch (error) {
    console.error("❌ Error cargando modelos:", error);
    const select = document.getElementById("modelo");
    select.innerHTML = '<option value="">Error al cargar modelos</option>';
  }
}

// Guardar nuevo vehículo
vehiculoForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  console.log("💾 Guardando nuevo vehículo...");

  const nuevoVehiculo = {
    año: parseInt(document.getElementById("año").value),
    numeroSerie: document.getElementById("numeroSerie").value.trim(),
    kilometraje: parseInt(document.getElementById("kilometraje").value),
    clienteId: document.getElementById("cliente").value,
    modeloId: document.getElementById("modelo").value
  };

  console.log("📦 Datos del vehículo:", nuevoVehiculo);

  // Validaciones
  if (!nuevoVehiculo.clienteId || !nuevoVehiculo.modeloId) {
    alert("❌ Por favor selecciona un cliente y un modelo");
    return;
  }

  if (nuevoVehiculo.año < 1900 || nuevoVehiculo.año > new Date().getFullYear() + 1) {
    alert("❌ El año debe ser válido");
    return;
  }

  try {
    const res = await fetch(VEHICULOS_API, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(nuevoVehiculo)
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(errorText || "Error al guardar vehículo");
    }

    const resultado = await res.json();
    console.log("✅ Vehículo guardado:", resultado);

    modal.classList.add("hidden");
    vehiculoForm.reset();
    
    // Recargar la lista
    await cargarVehiculos();
    
    alert("✅ Vehículo registrado correctamente");
    
  } catch (error) {
    console.error("❌ Error guardando vehículo:", error);
    alert("❌ Error: " + error.message);
  }
});

// Editar vehículo (función básica - puedes expandirla)
async function editarVehiculo(id) {
  try {
    console.log("✏️ Editando vehículo:", id);
    
    // Cargar datos del vehículo
    const res = await fetch(`${VEHICULOS_API}/${id}`);
    if (!res.ok) throw new Error("No se encontró el vehículo");
    
    const vehiculo = await res.json();
    console.log("📋 Vehículo a editar:", vehiculo);

    // Llenar el formulario con los datos existentes
    document.getElementById("año").value = vehiculo.año || "";
    document.getElementById("numeroSerie").value = vehiculo.numeroSerie || "";
    document.getElementById("kilometraje").value = vehiculo.kilometraje || "";
    
    // Establecer cliente y modelo (necesitarías guardar el ID actual para edición)
    // Esto es más complejo y requeriría modificar el formulario para manejar edición
    
    alert("✏️ Funcionalidad de edición en desarrollo. Vehículo: " + vehiculo.numeroSerie);
    
  } catch (error) {
    console.error("❌ Error editando vehículo:", error);
    alert("❌ Error al cargar vehículo para editar: " + error.message);
  }
}

// Eliminar vehículo
async function eliminarVehiculo(id) {
  if (!confirm("¿Seguro que deseas eliminar este vehículo?")) return;
  
  try {
    console.log("🗑️ Eliminando vehículo:", id);
    const res = await fetch(`${VEHICULOS_API}/${id}`, { method: "DELETE" });
    
    if (!res.ok) {
      throw new Error("Error al eliminar vehículo");
    }
    
    await cargarVehiculos();
    alert("✅ Vehículo eliminado correctamente");
  } catch (error) {
    console.error("❌ Error eliminando vehículo:", error);
    alert("❌ Error al eliminar vehículo");
  }
}

// Cerrar sesión
logoutBtn.addEventListener("click", () => {
  localStorage.clear();
  window.location.href = "login.html";
});

// Inicialización
document.addEventListener("DOMContentLoaded", () => {
  console.log("🚀 Inicializando módulo de vehículos...");
  cargarVehiculos();
  cargarClientes();
  cargarModelos();
});