const API_URL = "http://localhost:5000/api/marcas"; // Ajusta a tu endpoint real

const tabla = document.getElementById("tablaMarcas").querySelector("tbody");
const modal = document.getElementById("modalMarca");
const form = document.getElementById("formMarca");
const btnAgregar = document.getElementById("btnAgregar");
const btnCancelar = document.getElementById("btnCancelar");
const modalTitulo = document.getElementById("modalTitulo");
const logoutBtn = document.getElementById("logoutBtn");

let editando = false;
let idActual = null;

// 🔄 Cargar marcas
async function cargarMarcas() {
  try {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error("Error al obtener marcas");
    const data = await res.json();

    tabla.innerHTML = "";
    data.forEach(m => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${m.nombre}</td>
        <td>${m.pais}</td>
        <td>
          <button class="btn-editar" onclick="editarMarca('${m.id}')">
            <i class="fas fa-edit"></i>
          </button>
          <button class="btn-eliminar" onclick="eliminarMarca('${m.id}')">
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

// ➕ Abrir modal
btnAgregar.addEventListener("click", () => {
  form.reset();
  editando = false;
  idActual = null;
  modalTitulo.textContent = "Agregar Marca";
  modal.classList.remove("hidden");
});

// ❌ Cancelar modal
btnCancelar.addEventListener("click", () => modal.classList.add("hidden"));

// 💾 Guardar marca
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const marca = {
    nombre: document.getElementById("nombre").value.trim(),
    pais: document.getElementById("pais").value.trim()
  };

  try {
    const url = editando ? `${API_URL}/${idActual}` : API_URL;
    const method = editando ? "PUT" : "POST";

    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(marca)
    });

    if (!res.ok) throw new Error("Error al guardar marca");

    await cargarMarcas();
    modal.classList.add("hidden");
  } catch (error) {
    console.error(error);
  }
});

// ✏️ Editar marca
async function editarMarca(id) {
  try {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("Marca no encontrada");
    const m = await res.json();

    idActual = m.id;
    editando = true;
    modalTitulo.textContent = "Editar Marca";

    document.getElementById("nombre").value = m.nombre;
    document.getElementById("pais").value = m.pais;

    modal.classList.remove("hidden");
  } catch (error) {
    console.error(error);
  }
}

// 🗑️ Eliminar marca
async function eliminarMarca(id) {
  if (!confirm("¿Seguro que deseas eliminar esta marca?")) return;
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar marca");
    await cargarMarcas();
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
cargarMarcas();
