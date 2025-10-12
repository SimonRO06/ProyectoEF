// ====== Config ======
const API_BASE = "http://localhost:5000/api"; // <-- AJUSTA a tu backend real
const ORDENES_URL = `${API_BASE}/ordenesservicio`; // endpoint para ordenes
const CLIENTES_URL = `${API_BASE}/clientes`;
const VEHICULOS_BY_CLIENTE = (clienteId) => `${API_BASE}/clientes/${clienteId}/vehiculos`; // o /api/vehiculos?clienteId=
const USUARIOS_URL = `${API_BASE}/usuarios`; // para traer mecánicos (filtrar por rol)
const REPUESTOS_URL = `${API_BASE}/repuestos`;

// DOM
const clienteSelect = document.getElementById("clienteSelect");
const vehiculoSelect = document.getElementById("vehiculoSelect");
const mecanicoSelect = document.getElementById("mecanicoSelect");
const tipoServicio = document.getElementById("tipoServicio");
const descripcion = document.getElementById("descripcion");
const fechaEstimada = document.getElementById("fechaEstimada");
const kilometrajeInput = document.getElementById("kilometrajeInput");

const repuestoSelect = document.getElementById("repuestoSelect");
const repuestoCantidad = document.getElementById("repuestoCantidad");
const addRepuestoBtn = document.getElementById("addRepuestoBtn");
const repuestosList = document.getElementById("repuestosList");

const ordenForm = document.getElementById("ordenForm");
const ordenesTableBody = document.querySelector("#ordenesTable tbody");
const logoutBtn = document.getElementById("logoutBtn");

// Estado local
let repuestosDisponibles = []; // lista completa de repuestos desde backend
let repuestosAgregados = [];   // { id, nombre, cantidad, precioUnitario }

// ====== Init ======
document.addEventListener("DOMContentLoaded", () => {
  cargarClientes();
  cargarMecanicos();
  cargarRepuestos();
  cargarOrdenesRecientes();
});

// ====== Cargar clientes ======
async function cargarClientes() {
  try {
    const res = await fetch(CLIENTES_URL);
    if (!res.ok) throw new Error("No se pudieron cargar clientes");
    const clientes = await res.json();
    clienteSelect.innerHTML = `<option value="">-- Seleccione cliente --</option>`;
    clientes.forEach(c => {
      clienteSelect.innerHTML += `<option value="${c.id}">${c.nombre} (${c.correo ?? ''})</option>`;
    });
  } catch (err) {
    console.error(err);
    clienteSelect.innerHTML = `<option value="">Error cargando clientes</option>`;
  }
}

// ====== Cuando cambia cliente: cargar vehículos ======
clienteSelect.addEventListener("change", async (e) => {
  const clientId = e.target.value;
  vehiculoSelect.innerHTML = `<option value="">Cargando vehículos...</option>`;
  if (!clientId) {
    vehiculoSelect.innerHTML = `<option value="">Seleccione un cliente primero</option>`;
    return;
  }

  try {
    const res = await fetch(VEHICULOS_BY_CLIENTE(clientId));
    if (!res.ok) throw new Error("Error cargando vehículos");
    const vehiculos = await res.json();
    vehiculoSelect.innerHTML = `<option value="">-- Seleccione vehículo --</option>`;
    vehiculos.forEach(v => {
      const label = `${v.modelo?.nombre ?? ''} - ${v.año} - ${v.numeroSerie}`;
      vehiculoSelect.innerHTML += `<option value="${v.id}">${label}</option>`;
    });
  } catch (err) {
    console.error(err);
    vehiculoSelect.innerHTML = `<option value="">Error cargando vehículos</option>`;
  }
});

// ====== Cargar mecánicos (usuarios con role=Mecanico) ======
async function cargarMecanicos() {
  try {
    // Suponemos: GET /api/usuarios?role=Mecanico o backend devuelve todos y filtramos
    const res = await fetch(`${USUARIOS_URL}`);
    if (!res.ok) throw new Error("Error cargando usuarios");
    const usuarios = await res.json();
    // Filtrar por rol numérico o string: intentamos ambos
    const mecanicos = usuarios.filter(u => u.rol === 2 || u.rol === "Mecanico" || (u.roleName && u.roleName.toLowerCase().includes("mecanic")));
    mecanicoSelect.innerHTML = `<option value="">-- Seleccione mecánico --</option>`;
    mecanicos.forEach(m => {
      mecanicoSelect.innerHTML += `<option value="${m.id}">${m.nombre}</option>`;
    });
  } catch (err) {
    console.error(err);
    mecanicoSelect.innerHTML = `<option value="">Error cargando mecánicos</option>`;
  }
}

// ====== Cargar repuestos ======
async function cargarRepuestos() {
  try {
    const res = await fetch(REPUESTOS_URL);
    if (!res.ok) throw new Error("Error cargando repuestos");
    repuestosDisponibles = await res.json();
    repuestoSelect.innerHTML = `<option value="">-- Seleccione repuesto --</option>`;
    repuestosDisponibles.forEach(r => {
      repuestoSelect.innerHTML += `<option value="${r.id}" data-stock="${r.cantidadStock}" data-precio="${r.precioUnitario}">${r.codigo} — ${r.descripcion} (Stock: ${r.cantidadStock})</option>`;
    });
  } catch (err) {
    console.error(err);
    repuestoSelect.innerHTML = `<option value="">Error cargando repuestos</option>`;
  }
}

// ====== Añadir repuesto a la lista ======
addRepuestoBtn.addEventListener("click", () => {
  const repuestoId = repuestoSelect.value;
  const qty = parseInt(repuestoCantidad.value || "0", 10);
  if (!repuestoId) return alert("Seleccione un repuesto.");
  if (!qty || qty <= 0) return alert("Cantidad inválida.");

  const opt = repuestoSelect.querySelector(`option[value="${repuestoId}"]`);
  const stock = parseInt(opt.dataset.stock || "0", 10);
  const precio = parseFloat(opt.dataset.precio || "0");

  if (qty > stock) return alert("Cantidad mayor al stock disponible.");

  // Si ya está agregado, sumar cantidades (y validar stock)
  const existente = repuestosAgregados.find(r => r.id === repuestoId);
  if (existente) {
    if (existente.cantidad + qty > stock) return alert("Excede stock disponible.");
    existente.cantidad += qty;
  } else {
    // buscar nombre
    const texto = opt.textContent;
    const nombre = texto.split("—")[1]?.split("(Stock")[0]?.trim() ?? texto;
    repuestosAgregados.push({ id: repuestoId, nombre, cantidad: qty, precioUnitario: precio });
  }

  renderRepuestosAgregados();
});

// ====== Renderizar repuestos agregados ======
function renderRepuestosAgregados() {
  repuestosList.innerHTML = "";
  repuestosAgregados.forEach((r, idx) => {
    const row = document.createElement("div");
    row.className = "repuesto-row";
    row.innerHTML = `
      <div class="label">${r.nombre}</div>
      <div class="qty">x ${r.cantidad}</div>
      <div class="precio">$${(r.precioUnitario * r.cantidad).toFixed(2)}</div>
      <button class="remove" data-idx="${idx}" title="Eliminar"><i class="fas fa-trash"></i></button>
    `;
    repuestosList.appendChild(row);
  });

  // attach remove handlers
  repuestosList.querySelectorAll(".remove").forEach(btn => {
    btn.addEventListener("click", (e) => {
      const idx = parseInt(e.currentTarget.dataset.idx, 10);
      repuestosAgregados.splice(idx, 1);
      renderRepuestosAgregados();
    });
  });
}

// ====== Crear orden (submit) ======
ordenForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  // Validaciones simples
  if (!clienteSelect.value) return alert("Seleccione cliente.");
  if (!vehiculoSelect.value) return alert("Seleccione vehículo.");
  if (!tipoServicio.value) return alert("Seleccione tipo de servicio.");
  if (!mecanicoSelect.value) return alert("Seleccione un mecánico.");
  if (!fechaEstimada.value) return alert("Selecione fecha estimada.");
  if (!kilometrajeInput.value) return alert("Ingrese kilometraje.");

  // Construir payload acorde a tu DTO/DTOs backend
  const detalles = repuestosAgregados.map(r => ({
    repuestoId: r.id,
    cantidad: r.cantidad,
    costoUnitario: r.precioUnitario
  }));

  const payload = {
    tipoServicio: parseInt(tipoServicio.value, 10),       // enum numeric
    fechaIngreso: new Date().toISOString(),               // o dejar que backend lo genere
    fechaEstimadaEntrega: new Date(fechaEstimada.value).toISOString(),
    estado: 0, // Pendiente (enum Estado), si backend espera string cambia
    userMemberId: mecanicoSelect.value,                  // id del mecánico (puede ser int/string)
    vehiculoId: vehiculoSelect.value,
    detallesOrden: detalles,
    descripcion: descripcion.value,
    kilometraje: parseInt(kilometrajeInput.value, 10)
  };

  try {
    const res = await fetch(ORDENES_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(text || "Error creando orden");
    }

    alert("Orden creada correctamente ✅");
    ordenForm.reset();
    repuestosAgregados = [];
    renderRepuestosAgregados();
    cargarOrdenesRecientes();
  } catch (err) {
    console.error(err);
    alert("Error al crear orden: " + (err.message || err));
  }
});

// ====== Cargar órdenes recientes ======
async function cargarOrdenesRecientes() {
  try {
    const res = await fetch(ORDENES_URL + "?pageSize=10"); // si tu API acepta paginado
    if (!res.ok) throw new Error("Error cargando órdenes");
    const data = await res.json();
    ordenesTableBody.innerHTML = "";
    data.forEach(o => {
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${o.id}</td>
        <td>${o.vehiculo?.modelo?.nombre ?? ''} ${o.vehiculo?.numeroSerie ?? ''}</td>
        <td>${o.vehiculo?.cliente?.nombre ?? ''}</td>
        <td>${tipoServicioText(o.tipoServicio)}</td>
        <td>${o.userMember?.nombre ?? ''}</td>
        <td>${estadoText(o.estado)}</td>
        <td>${(new Date(o.fechaIngreso)).toLocaleDateString()}</td>
      `;
      ordenesTableBody.appendChild(tr);
    });
  } catch (err) {
    console.error(err);
    ordenesTableBody.innerHTML = `<tr><td colspan="7">No se pudieron cargar órdenes.</td></tr>`;
  }
}

// ====== Helpers para enums (asegura correspondencia con backend) ======
function tipoServicioText(v) {
  const map = { 1: "Preventivo", 2: "Reparación", 3: "Diagnóstico" };
  return map[v] ?? v;
}
function estadoText(v) {
  const map = { 0: "Pendiente", 1: "En Progreso", 2: "Finalizado" };
  return map[v] ?? v;
}

// ====== Logout ======
logoutBtn.addEventListener("click", () => {
  localStorage.clear();
  window.location.href = "../index.html";
});
