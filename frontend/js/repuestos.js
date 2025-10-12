const API_URL = "http://localhost:5000/api/repuestos"; // 🔧 Ajusta al puerto real de tu backend

const tabla = document.getElementById("tablaRepuestos").querySelector("tbody");
const modal = document.getElementById("modalRepuesto");
const form = document.getElementById("formRepuesto");
const btnAgregar = document.getElementById("btnAgregar");
const btnCancelar = document.getElementById("btnCancelar");
const modalTitulo = document.getElementById("modalTitulo");
const logoutBtn = document.getElementById("logoutBtn");

let editando = false;
let idActual = null;

// 🔄 Cargar repuestos
// 🔄 Cargar repuestos - MEJORADA
async function cargarRepuestos() {
  try {
    const res = await fetch("http://localhost:5000/api/repuestos/all");
    if (!res.ok) throw new Error("Error al obtener los repuestos");
    const data = await res.json();

    console.log("Repuestos cargados:", data); // Para debug

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
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${rep.codigo || rep.nombre || 'N/A'}</td>
        <td>${rep.descripcion || 'Sin descripción'}</td>
        <td>$${(rep.precioUnitario || rep.precio || 0).toFixed(2)}</td>
        <td>${rep.cantidadStock || rep.cantidad || 0}</td>
        <td>
          <button class="btn-editar" onclick="editarRepuesto('${rep.id}')">
            <i class="fas fa-edit"></i>
          </button>
          <button class="btn-eliminar" onclick="eliminarRepuesto('${rep.id}')">
            <i class="fas fa-trash"></i>
          </button>
        </td>
      `;
      tabla.appendChild(fila);
    });
  } catch (error) {
    console.error("Error cargando repuestos:", error);
    tabla.innerHTML = `
      <tr>
        <td colspan="5" style="text-align: center; color: red;">
          Error al cargar repuestos: ${error.message}
        </td>
      </tr>
    `;
  }
}

// ➕ Abrir modal nuevo
btnAgregar.addEventListener("click", () => {
  form.reset();
  editando = false;
  idActual = null;
  modalTitulo.textContent = "Agregar Repuesto";
  modal.classList.remove("hidden");
});

// ❌ Cancelar modal
btnCancelar.addEventListener("click", () => modal.classList.add("hidden"));

// 💾 Guardar repuesto
// 💾 Guardar repuesto - Para HTML actualizado
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const repuesto = {
    codigo: document.getElementById("codigo").value.trim(),
    descripcion: document.getElementById("descripcion").value.trim(),
    precioUnitario: parseFloat(document.getElementById("precioUnitario").value),
    cantidadStock: parseInt(document.getElementById("cantidadStock").value)
  };

  console.log("Enviando repuesto:", repuesto);

  try {
    const url = editando ? `${API_URL}/${idActual}` : API_URL;
    const method = editando ? "PUT" : "POST";

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
    alert(editando ? "Repuesto actualizado correctamente ✅" : "Repuesto creado correctamente ✅");
  } catch (error) {
    console.error("Error guardando repuesto:", error);
    alert("Error: " + error.message);
  }
});

// ✏️ Editar repuesto - Para HTML actualizado
async function editarRepuesto(id) {
  try {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("No se encontró el repuesto");
    const rep = await res.json();

    console.log("Repuesto cargado para editar:", rep);

    idActual = rep.id;
    editando = true;
    modalTitulo.textContent = "Editar Repuesto";

    document.getElementById("codigo").value = rep.codigo || "";
    document.getElementById("descripcion").value = rep.descripcion || "";
    document.getElementById("precioUnitario").value = rep.precioUnitario || "";
    document.getElementById("cantidadStock").value = rep.cantidadStock || "";

    modal.classList.remove("hidden");
  } catch (error) {
    console.error("Error editando repuesto:", error);
    alert("Error al cargar repuesto para editar");
  }
}

// 🗑️ Eliminar repuesto
async function eliminarRepuesto(id) {
  if (!confirm("¿Seguro que deseas eliminar este repuesto?")) return;
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar repuesto");
    await cargarRepuestos();
  } catch (error) {
    console.error(error);
  }
}

// 🚪 Cerrar sesión
logoutBtn.addEventListener("click", () => {
  localStorage.clear();
  window.location.href = "login.html";
});

// 🚀 Inicialización
cargarRepuestos();
