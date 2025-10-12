const API_URL = "http://localhost:5191/api/Usuarios"; // Ajusta la ruta real del backend

const tabla = document.getElementById("tablaUsuarios").querySelector("tbody");
const modal = document.getElementById("modalUsuario");
const form = document.getElementById("formUsuario");
const btnAgregar = document.getElementById("btnAgregar");
const btnCancelar = document.getElementById("btnCancelar");
const modalTitulo = document.getElementById("modalTitulo");
const logoutBtn = document.getElementById("logoutBtn");

let editando = false;
let idActual = null;

// 🔄 Cargar usuarios
async function cargarUsuarios() {
  try {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error("Error al obtener usuarios");
    const data = await res.json();

    tabla.innerHTML = "";
    data.forEach(u => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${u.nombre}</td>
        <td>${u.email}</td>
        <td>${u.rol}</td>
        <td>
          <button class="btn-editar" onclick="editarUsuario('${u.id}')">
            <i class="fas fa-edit"></i>
          </button>
          <button class="btn-eliminar" onclick="eliminarUsuario('${u.id}')">
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
  modalTitulo.textContent = "Agregar Usuario";
  modal.classList.remove("hidden");
});

// ❌ Cancelar modal
btnCancelar.addEventListener("click", () => modal.classList.add("hidden"));

// 💾 Guardar usuario
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const usuario = {
    nombre: document.getElementById("nombre").value.trim(),
    email: document.getElementById("email").value.trim(),
    password: document.getElementById("password").value.trim(),
    rol: document.getElementById("rol").value
  };

  try {
    const url = editando ? `${API_URL}/${idActual}` : API_URL;
    const method = editando ? "PUT" : "POST";

    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(usuario)
    });

    if (!res.ok) throw new Error("Error al guardar usuario");

    await cargarUsuarios();
    modal.classList.add("hidden");
  } catch (error) {
    console.error(error);
  }
});

// ✏️ Editar usuario
async function editarUsuario(id) {
  try {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("Usuario no encontrado");
    const u = await res.json();

    idActual = u.id;
    editando = true;
    modalTitulo.textContent = "Editar Usuario";

    document.getElementById("nombre").value = u.nombre;
    document.getElementById("email").value = u.email;
    document.getElementById("password").value = "";
    document.getElementById("rol").value = u.rol;

    modal.classList.remove("hidden");
  } catch (error) {
    console.error(error);
  }
}

// 🗑️ Eliminar usuario
async function eliminarUsuario(id) {
  if (!confirm("¿Seguro que deseas eliminar este usuario?")) return;
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar usuario");
    await cargarUsuarios();
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
cargarUsuarios();
