const API_URL = "http://localhost:5000/api/repuestos";

const tabla = document.getElementById("tablaRepuestos").querySelector("tbody");
const modal = document.getElementById("modalRepuesto");
const form = document.getElementById("formRepuesto");
const btnAgregar = document.getElementById("btnAgregar");
const btnCancelar = document.getElementById("btnCancelar");
const modalTitulo = document.getElementById("modalTitulo");
const logoutBtn = document.getElementById("logoutBtn");

let editando = false;
let idActual = null;

// 🔄 Cargar repuestos - VERSIÓN CORREGIDA
async function cargarRepuestos() {
  try {
    console.log("🔄 Cargando repuestos...");
    
    const res = await fetch("http://localhost:5000/api/repuestos/all");
    if (!res.ok) throw new Error("Error al obtener los repuestos");
    const data = await res.json();

    console.log("✅ Repuestos cargados:", data);

    tabla.innerHTML = "";
    
    if (data.length === 0) {
      tabla.innerHTML = `
        <tr>
          <td colspan="5" style="text-align: center; color: #666;">
            No hay repuestos registrados. ¡Agrega el primero!
          </td>
        </tr>
      `;
      return;
    }

    data.forEach(rep => {
      const repuestoId = rep.id || rep._id;
      console.log(`🔧 Creando fila para repuesto: ${rep.codigo} con ID: ${repuestoId}`);
      
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${rep.codigo || rep.nombre || 'N/A'}</td>
        <td>${rep.descripcion || 'Sin descripción'}</td>
        <td>$${(rep.precioUnitario || rep.precio || 0).toFixed(2)}</td>
        <td class="${(rep.cantidadStock || rep.cantidad || 0) <= 5 ? 'stock-bajo' : 'stock-normal'}">
          ${rep.cantidadStock || rep.cantidad || 0}
        </td>
        <td class="acciones">
          <button class="btn-editar" onclick="editarRepuesto('${repuestoId}')" title="Editar repuesto">
            <i class="fas fa-edit"></i> Editar
          </button>
          <button class="btn-eliminar" onclick="eliminarRepuesto('${repuestoId}')" title="Eliminar repuesto">
            <i class="fas fa-trash"></i> Eliminar
          </button>
        </td>
      `;
      tabla.appendChild(fila);
    });
    
    console.log("✅ Tabla de repuestos cargada correctamente");
    
  } catch (error) {
    console.error("❌ Error cargando repuestos:", error);
    tabla.innerHTML = `
      <tr>
        <td colspan="5" style="text-align: center; color: red;">
          Error al cargar repuestos: ${error.message}
        </td>
      </tr>
    `;
  }
}

// ➕ Abrir modal nuevo - MEJORADO
btnAgregar.addEventListener("click", () => {
  console.log("➕ Abriendo modal para nuevo repuesto");
  
  form.reset();
  editando = false;
  idActual = null;
  modalTitulo.textContent = "Agregar Repuesto";
  modal.classList.remove("hidden");
});

// ❌ Cancelar modal
btnCancelar.addEventListener("click", () => {
  console.log("❌ Cerrando modal");
  modal.classList.add("hidden");
});

// 💾 Guardar repuesto - MEJORADO
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const repuesto = {
    codigo: document.getElementById("codigo").value.trim(),
    descripcion: document.getElementById("descripcion").value.trim(),
    precioUnitario: parseFloat(document.getElementById("precioUnitario").value),
    cantidadStock: parseInt(document.getElementById("cantidadStock").value)
  };

  console.log("💾 Guardando repuesto:", repuesto);

  // Validaciones básicas
  if (!repuesto.codigo || !repuesto.descripcion) {
    alert("❌ El código y descripción son obligatorios");
    return;
  }

  if (repuesto.precioUnitario <= 0 || repuesto.cantidadStock < 0) {
    alert("❌ El precio debe ser mayor a 0 y la cantidad no puede ser negativa");
    return;
  }

  try {
    const url = editando ? `${API_URL}/${idActual}` : API_URL;
    const method = editando ? "PUT" : "POST";

    console.log(`📤 Enviando ${method} a:`, url);

    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(repuesto)
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(errorText || "Error al guardar repuesto");
    }

    await cargarRepuestos();
    modal.classList.add("hidden");
    alert(editando ? "✅ Repuesto actualizado correctamente" : "✅ Repuesto creado correctamente");
    
  } catch (error) {
    console.error("❌ Error guardando repuesto:", error);
    alert("❌ Error: " + error.message);
  }
});

// ✏️ Editar repuesto - VERSIÓN CORREGIDA
async function editarRepuesto(id) {
  try {
    console.log("✏️ Solicitando edición del repuesto ID:", id);
    
    const res = await fetch(`${API_URL}/${id}`);
    
    if (!res.ok) throw new Error("No se encontró el repuesto");
    const rep = await res.json();

    console.log("📥 Repuesto cargado para editar:", rep);

    // 🔧 CORRECCIÓN: Usar el ID correcto
    idActual = rep.id || rep._id;
    editando = true;
    modalTitulo.textContent = "Editar Repuesto";

    // 🔧 CORRECCIÓN: Asignar valores correctamente
    document.getElementById("codigo").value = rep.codigo || "";
    document.getElementById("descripcion").value = rep.descripcion || "";
    document.getElementById("precioUnitario").value = rep.precioUnitario || rep.precio || "";
    document.getElementById("cantidadStock").value = rep.cantidadStock || rep.cantidad || "";

    modal.classList.remove("hidden");
    console.log("✅ Modal de edición abierto correctamente");
    
  } catch (error) {
    console.error("❌ Error editando repuesto:", error);
    alert("❌ Error al cargar repuesto para editar: " + error.message);
  }
}

// 🗑️ Eliminar repuesto - MEJORADO
async function eliminarRepuesto(id) {
  if (!confirm("¿Seguro que deseas eliminar este repuesto?")) return;
  
  try {
    console.log("🗑️ Eliminando repuesto ID:", id);
    
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar repuesto");
    
    await cargarRepuestos();
    alert("✅ Repuesto eliminado correctamente");
    
  } catch (error) {
    console.error("❌ Error eliminando repuesto:", error);
    alert("❌ Error al eliminar repuesto");
  }
}

// 🚪 Cerrar sesión
logoutBtn.addEventListener("click", () => {
  console.log("🚪 Cerrando sesión...");
  localStorage.clear();
  window.location.href = "login.html";
});

// 🚀 Inicialización
document.addEventListener("DOMContentLoaded", () => {
  console.log("🚀 Inicializando página de repuestos...");
  cargarRepuestos();
});