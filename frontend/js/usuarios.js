const tabla = document.getElementById("tablaUsuarios").querySelector("tbody");
const modal = document.getElementById("modalUsuario");
const form = document.getElementById("formUsuario");
const btnAgregar = document.getElementById("btnAgregar");
const btnCancelar = document.getElementById("btnCancelar");
const modalTitulo = document.getElementById("modalTitulo");
const logoutBtn = document.getElementById("logoutBtn");

// 🔧 CORRECCIÓN: URL correcta para usuarios
const API_URL = "http://localhost:5000/api/users";

let editando = false;
let idActual = null;

// 🔄 Cargar usuarios
async function cargarUsuarios() {
  try {
    const res = await fetch("http://localhost:5000/api/users/all");
    
    if (!res.ok) {
      const contentType = res.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const errorData = await res.json();
        throw new Error(errorData.message || "Error al obtener usuarios");
      } else {
        const errorText = await res.text();
        throw new Error(errorText || "Error al obtener usuarios");
      }
    }
    
    const data = await res.json();

    tabla.innerHTML = "";
    data.forEach(u => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${u.username || u.nombre}</td>
        <td>${u.email}</td>
        <td>${u.role || u.rol}</td>
        <td>
          <button class="btn-editar" onclick="editarUsuario('${u._id || u.id}')">
            <i class="fas fa-edit"></i>
          </button>
          <button class="btn-eliminar" onclick="eliminarUsuario('${u._id || u.id}')">
            <i class="fas fa-trash"></i>
          </button>
        </td>
      `;
      tabla.appendChild(fila);
    });
  } catch (error) {
    console.error("Error cargando usuarios:", error);
    tabla.innerHTML = `<tr><td colspan="4" style="text-align: center; color: red;">Error al cargar usuarios: ${error.message}</td></tr>`;
  }
}

// ➕ Abrir modal - CORREGIDO
btnAgregar.addEventListener("click", () => {
  // En lugar de form.reset(), establecer valores por defecto manualmente
  document.getElementById("nombre").value = "";
  document.getElementById("email").value = "";
  document.getElementById("password").value = "";
  document.getElementById("rol").value = "Administrador"; // ✅ ESTABLECER VALOR POR DEFECTO
  
  clearErrorMessages();
  editando = false;
  idActual = null;
  modalTitulo.textContent = "Agregar Usuario";
  modal.classList.remove("hidden");
});

// ❌ Cancelar modal
btnCancelar.addEventListener("click", () => {
  modal.classList.add("hidden");
  clearErrorMessages();
});

// 💾 Guardar usuario - CON DEBUG MEJORADO
// 💾 Guardar usuario - MEJORADO
form.addEventListener("submit", async (e) => {
  e.preventDefault();
  
  clearErrorMessages();

  const usuario = {
    username: document.getElementById("nombre").value.trim(),
    email: document.getElementById("email").value.trim(),
    password: document.getElementById("password").value.trim(),
    role: document.getElementById("rol").value
  };

  console.log("Datos del formulario:", usuario);

  // VALIDACIONES
  let valid = true;

  if (!usuario.username) {
    showErrorMessage("nombre", "El nombre de usuario es obligatorio.");
    valid = false;
  }

  if (!usuario.email) {
    showErrorMessage("email", "El correo electrónico es obligatorio.");
    valid = false;
  } else if (!validateEmail(usuario.email)) {
    showErrorMessage("email", "Por favor, ingrese un correo electrónico válido.");
    valid = false;
  }

  if (!editando && !usuario.password) {
    showErrorMessage("password", "La contraseña es obligatoria.");
    valid = false;
  }

  // 🔧 VALIDACIÓN MEJORADA: Asegurar que el rol no sea null
  if (!usuario.role) {
    // Si el rol está vacío, usar "Administrador" por defecto
    usuario.role = "Administrador";
    console.log("Rol estaba vacío, se estableció a:", usuario.role);
  }

  if (!valid) return;

  try {
    let url, method;
    
    if (editando) {
      url = `${API_URL}/${idActual}`;
      method = "PUT";
      
      if (!usuario.password) {
        delete usuario.password;
      }
    } else {
      url = `${API_URL}/register`;
      method = "POST";
    }

    console.log("Enviando datos:", usuario);

    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(usuario)
    });

    const contentType = res.headers.get('content-type');
    
    if (!res.ok) {
      if (contentType && contentType.includes('application/json')) {
        const errorData = await res.json();
        throw new Error(errorData.message || errorData.error || "Error al guardar usuario");
      } else {
        const errorText = await res.text();
        throw new Error(errorText || "Error al guardar usuario");
      }
    }

    let result;
    if (contentType && contentType.includes('application/json')) {
      result = await res.json();
    } else {
      const text = await res.text();
      console.log("Respuesta del servidor:", text);
      result = { message: text };
    }
    
    await cargarUsuarios();
    modal.classList.add("hidden");
    alert(editando ? "Usuario actualizado correctamente ✅" : "Usuario creado correctamente ✅");
    
  } catch (error) {
    console.error("Error guardando usuario:", error);
    alert("Error: " + error.message);
  }
});

// ✏️ Editar usuario
// ✏️ Editar usuario - CORREGIDO
async function editarUsuario(id) {
  try {
    clearErrorMessages();
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error("Usuario no encontrado");
    const u = await res.json();

    console.log("Usuario cargado para editar:", u);

    idActual = u._id || u.id;
    editando = true;
    modalTitulo.textContent = "Editar Usuario";

    document.getElementById("nombre").value = u.username || u.nombre || "";
    document.getElementById("email").value = u.email || "";
    document.getElementById("password").value = "";
    
    // 🔧 CORREGIDO: Manejar null específicamente y usar "Administrador" como default
    let rolValue;
    if (u.role === null || u.role === undefined) {
      rolValue = "Administrador"; // Valor por defecto que coincide con tu select
    } else {
      rolValue = u.role;
    }
    
    // También verificar si el valor existe en las opciones del select
    const rolSelect = document.getElementById("rol");
    const optionExists = Array.from(rolSelect.options).some(option => option.value === rolValue);
    
    if (!optionExists) {
      rolValue = "Administrador"; // Fallback si el valor no existe
    }
    
    document.getElementById("rol").value = rolValue;
    console.log("Rol establecido a:", rolValue);

    modal.classList.remove("hidden");
  } catch (error) {
    console.error("Error editando usuario:", error);
    alert("Error al cargar usuario para editar");
  }
}

// 🗑️ Eliminar usuario
async function eliminarUsuario(id) {
  if (!confirm("¿Seguro que deseas eliminar este usuario?")) return;
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar usuario");
    await cargarUsuarios();
    alert("Usuario eliminado correctamente ✅");
  } catch (error) {
    console.error("Error eliminando usuario:", error);
    alert("Error al eliminar usuario");
  }
}

// 🚪 Cerrar sesión
logoutBtn.addEventListener("click", () => {
  localStorage.clear();
  window.location.href = "login.html";
});

// FUNCIONES DE VALIDACIÓN
function validateEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

function showErrorMessage(fieldId, message) {
  const field = document.getElementById(fieldId);
  const errorMessage = document.createElement('div');
  errorMessage.classList.add('error-message');
  errorMessage.style.color = 'red';
  errorMessage.style.fontSize = '0.8rem';
  errorMessage.style.marginTop = '5px';
  errorMessage.textContent = message;
  field.parentNode.appendChild(errorMessage);
}

function clearErrorMessages() {
  const errorMessages = document.querySelectorAll('.error-message');
  errorMessages.forEach(msg => msg.remove());
}

// 🚀 Inicialización
document.addEventListener("DOMContentLoaded", () => {
  cargarUsuarios();
});