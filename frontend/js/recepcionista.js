const API_BASE = "https://localhost:5001/api";
const token = localStorage.getItem("token");

if (!token) {
  window.location.href = "login.html";
}

const btnClientes = document.getElementById("btnClientes");
const btnOrdenes = document.getElementById("btnOrdenes");
const btnRegistrarOrden = document.getElementById("btnRegistrarOrden");
const contenido = document.getElementById("contenido");
const logoutBtn = document.getElementById("logoutBtn");

logoutBtn.addEventListener("click", () => {
  localStorage.removeItem("token");
  window.location.href = "login.html";
});

btnClientes.addEventListener("click", async () => {
  contenido.innerHTML = "<h3>Cargando clientes...</h3>";
  const res = await fetch(`${API_BASE}/clientes`, {
    headers: { "Authorization": `Bearer ${token}` }
  });
  const clientes = await res.json();
  renderClientes(clientes);
});

btnOrdenes.addEventListener("click", async () => {
  contenido.innerHTML = "<h3>Cargando órdenes...</h3>";
  const res = await fetch(`${API_BASE}/ordenesServicio`, {
    headers: { "Authorization": `Bearer ${token}` }
  });
  const ordenes = await res.json();
  renderOrdenes(ordenes);
});

btnRegistrarOrden.addEventListener("click", () => {
  contenido.innerHTML = `
    <h3>Registrar Nueva Orden</h3>
    <form id="formOrden">
      <input type="text" id="clienteId" placeholder="ID del Cliente" required><br>
      <input type="text" id="vehiculoId" placeholder="ID del Vehículo" required><br>
      <textarea id="descripcion" placeholder="Descripción del problema" required></textarea><br>
      <button type="submit">Registrar</button>
    </form>
  `;

  const form = document.getElementById("formOrden");
  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const nuevaOrden = {
      clienteId: form.clienteId.value,
      vehiculoId: form.vehiculoId.value,
      descripcion: form.descripcion.value
    };

    const res = await fetch(`${API_BASE}/ordenesServicio`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(nuevaOrden)
    });

    if (res.ok) {
      alert("Orden registrada exitosamente");
      btnOrdenes.click();
    } else {
      alert("Error al registrar la orden");
    }
  });
});

function renderClientes(clientes) {
  contenido.innerHTML = `
    <h3>Clientes Registrados</h3>
    <table>
      <thead><tr><th>ID</th><th>Nombre</th><th>Teléfono</th></tr></thead>
      <tbody>
        ${clientes.map(c => `
          <tr><td>${c.id}</td><td>${c.nombre}</td><td>${c.telefono}</td></tr>
        `).join("")}
      </tbody>
    </table>
  `;
}

function renderOrdenes(ordenes) {
  contenido.innerHTML = `
    <h3>Órdenes Activas</h3>
    <table>
      <thead><tr><th>ID</th><th>Cliente</th><th>Vehículo</th><th>Estado</th></tr></thead>
      <tbody>
        ${ordenes.map(o => `
          <tr><td>${o.id}</td><td>${o.cliente?.nombre}</td><td>${o.vehiculo?.modelo}</td><td>${o.estado}</td></tr>
        `).join("")}
      </tbody>
    </table>
  `;
}
