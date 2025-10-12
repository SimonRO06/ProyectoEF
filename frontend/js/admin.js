// ====== CONFIGURACIÓN GENERAL ======
const API_BASE = "https://localhost:5001/api"; // cambia el puerto si tu backend usa otro
const token = localStorage.getItem("token");

if (!token) {
  // Si no hay token, redirige al login
  window.location.href = "login.html";
}

// ====== REFERENCIAS DEL DOM ======
const totalUsuariosEl = document.getElementById("totalUsuarios");
const ordenesActivasEl = document.getElementById("ordenesActivas");
const facturacionTotalEl = document.getElementById("facturacionTotal");
const logoutBtn = document.getElementById("logoutBtn");

// ====== EVENTOS ======
logoutBtn.addEventListener("click", () => {
  localStorage.removeItem("token");
  window.location.href = "login.html";
});

// ====== FUNCIONES DE PETICIÓN ======
async function fetchData(url) {
  try {
    const res = await fetch(url, {
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      }
    });

    if (!res.ok) throw new Error(`Error HTTP ${res.status}`);
    return await res.json();
  } catch (err) {
    console.error("Error al obtener datos:", err.message);
    return null;
  }
}

// ====== FUNCIONES DE DASHBOARD ======
async function cargarDatosDashboard() {
  // 1. Total de usuarios
  const usuarios = await fetchData(`${API_BASE}/usuarios`);
  if (usuarios) totalUsuariosEl.textContent = usuarios.length;
  else totalUsuariosEl.textContent = "Error";

  // 2. Órdenes activas
  const ordenes = await fetchData(`${API_BASE}/ordenesServicio`);
  if (ordenes) {
    const activas = ordenes.filter(o => o.estado === "EnProceso" || o.estado === "Pendiente");
    ordenesActivasEl.textContent = activas.length;
  } else {
    ordenesActivasEl.textContent = "Error";
  }

  // 3. Facturación total
  const facturas = await fetchData(`${API_BASE}/facturas`);
  if (facturas) {
    const total = facturas.reduce((sum, f) => sum + (f.total || 0), 0);
    facturacionTotalEl.textContent = `$${total.toLocaleString()}`;
  } else {
    facturacionTotalEl.textContent = "$0";
  }
}

// ====== ACCIONES ======
function abrirUsuarios() {
  window.location.href = "usuarios.html";
}

function abrirReportes() {
  window.location.href = "reportes.html";
}

// ====== INICIALIZAR ======
cargarDatosDashboard();
