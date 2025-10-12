const API_BASE = "https://localhost:5001/api";
const token = localStorage.getItem("token");

if (!token) {
  window.location.href = "login.html";
}

const listaOrdenes = document.getElementById("listaOrdenes");
const logoutBtn = document.getElementById("logoutBtn");

logoutBtn.addEventListener("click", () => {
  localStorage.removeItem("token");
  window.location.href = "login.html";
});

async function cargarOrdenes() {
  try {
    const res = await fetch(`${API_BASE}/ordenesServicio/asignadas`, {
      headers: { "Authorization": `Bearer ${token}` }
    });
    const ordenes = await res.json();
    renderOrdenes(ordenes);
  } catch (err) {
    listaOrdenes.innerHTML = `<p>Error al cargar órdenes.</p>`;
  }
}

function renderOrdenes(ordenes) {
  if (!ordenes.length) {
    listaOrdenes.innerHTML = `<p>No tienes órdenes asignadas.</p>`;
    return;
  }

  listaOrdenes.innerHTML = ordenes.map(o => `
    <div class="orden">
      <h3>Orden #${o.id}</h3>
      <p><b>Cliente:</b> ${o.cliente?.nombre}</p>
      <p><b>Vehículo:</b> ${o.vehiculo?.modelo}</p>
      <p><b>Descripción:</b> ${o.descripcion}</p>
      <p><b>Estado:</b> ${o.estado}</p>
      <button class="actualizar" onclick="actualizarEstado('${o.id}')">Marcar como completada</button>
    </div>
  `).join("");
}

async function actualizarEstado(id) {
  const confirmar = confirm("¿Deseas marcar esta orden como completada?");
  if (!confirmar) return;

  try {
    const res = await fetch(`${API_BASE}/ordenesServicio/${id}/estado`, {
      method: "PUT",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ estado: "Completada" })
    });

    if (res.ok) {
      alert("Orden actualizada con éxito");
      cargarOrdenes();
    } else {
      alert("Error al actualizar el estado");
    }
  } catch (err) {
    alert("Error de conexión con el servidor");
  }
}

cargarOrdenes();
