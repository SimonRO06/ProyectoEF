const API_URL = "http://localhost:5191/api/Repuestos"; // 🔧 Ajusta al puerto real de tu backend

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
async function cargarRepuestos() {
  try {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error("Error al obtener los repuestos");
    const data = await res.json();

    tabla.innerHTML = "";
    data.forEach(rep => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${rep.nombre}</td>
        <td>${rep.descripcion}</td>
        <td>$${rep.precio.toFixed(2)}</td>
        <td>${rep.cantidad}</td>
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
    console.error(error);
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
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const repuesto = {
    nombre: document.getElementById("nombre").value.trim(),
    descripcion: document.getElementById("descripcion").value.trim(),
    precio: parseFloat(document.getElementById("precio").value),
    cantidad: parseInt(document.getElementById("cantidad").value)
  };

  try {
    const url = editando ? `${API_URL}/${idActual}` : API_URL;
    const method = editando ? "PUT" : "POST";

    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(repuesto)
    });

    if (!res.ok) throw new Error("Error al guardar repuesto");

    await cargarRepuestos();
    modal.classList.add("hidden");
  } catch (error) {
    console.error(error);
  }
});

// ✏️ Editar repuesto
async function editarRepuesto(id) {
  try {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("No se encontró el repuesto");
    const rep = await res.json();

    idActual = rep.id;
    editando = true;
    modalTitulo.textContent = "Editar Repuesto";

    document.getElementById("nombre").value = rep.nombre;
    document.getElementById("descripcion").value = rep.descripcion;
    document.getElementById("precio").value = rep.precio;
    document.getElementById("cantidad").value = rep.cantidad;

    modal.classList.remove("hidden");
  } catch (error) {
    console.error(error);
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
