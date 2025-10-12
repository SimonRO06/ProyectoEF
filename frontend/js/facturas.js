// ==========================
// AutoTallerManager - Facturas
// ==========================

// 📦 URL base del backend (ajústala si cambia tu API)
const API_URL = "http://localhost:5062/api/facturas"; // <-- asegúrate que tu backend use esta ruta

// 🔹 Mapeo del Enum Método de Pago (igual que en tu backend)
const MetodoPago = {
  0: "Efectivo",
  1: "Tarjeta",
  2: "Transferencia",
  3: "Crédito"
};

// ==========================
// Cargar Facturas
// ==========================
document.addEventListener("DOMContentLoaded", async () => {
  await cargarFacturas();

  // Buscar factura
  const searchInput = document.getElementById("searchFactura");
  searchInput.addEventListener("input", filtrarFacturas);

  // Botón cerrar modal
  document.getElementById("closeModal").addEventListener("click", cerrarModal);

  // Logout
  document.getElementById("logoutBtn").addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.href = "login.html";
  });
});

// ==========================
// Función: Cargar facturas
// ==========================
async function cargarFacturas() {
  try {
    const response = await fetch(API_URL);
    if (!response.ok) throw new Error("Error al obtener las facturas");
    const facturas = await response.json();
    renderFacturas(facturas);
  } catch (error) {
    console.error("❌ Error cargando facturas:", error);
  }
}

// ==========================
// Renderizar facturas en tabla
// ==========================
function renderFacturas(facturas) {
  const tbody = document.querySelector("#facturasTable tbody");
  tbody.innerHTML = "";

  if (facturas.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="7" style="text-align:center; color:#64748b;">No hay facturas registradas</td>
      </tr>`;
    return;
  }

  facturas.forEach(f => {
    const tr = document.createElement("tr");

    tr.innerHTML = `
      <td>${f.id}</td>
      <td>${formatearFecha(f.fechaEmision)}</td>
      <td>$${f.subtotal.toFixed(2)}</td>
      <td>$${f.impuestos.toFixed(2)}</td>
      <td><strong>$${f.total.toFixed(2)}</strong></td>
      <td>${MetodoPago[f.metodoPago] ?? "Desconocido"}</td>
      <td>
        <button class="btn-ver" onclick="verDetalle('${f.id}')"><i class="fas fa-eye"></i></button>
        <button class="btn-eliminar" onclick="eliminarFactura('${f.id}')"><i class="fas fa-trash"></i></button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

// ==========================
// Filtrar facturas
// ==========================
function filtrarFacturas(e) {
  const filtro = e.target.value.toLowerCase();
  const filas = document.querySelectorAll("#facturasTable tbody tr");

  filas.forEach(fila => {
    const textoFila = fila.innerText.toLowerCase();
    fila.style.display = textoFila.includes(filtro) ? "" : "none";
  });
}

// ==========================
// Ver detalle de factura
// ==========================
async function verDetalle(id) {
  try {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error("Error al obtener detalle de factura");

    const factura = await response.json();
    mostrarDetalleModal(factura);
  } catch (error) {
    console.error("❌ Error obteniendo detalle:", error);
  }
}

function mostrarDetalleModal(factura) {
  const modal = document.getElementById("detalleModal");
  const detalleDiv = document.getElementById("detalleFacturaContent");

  detalleDiv.innerHTML = `
    <p><strong>ID:</strong> ${factura.id}</p>
    <p><strong>Fecha:</strong> ${formatearFecha(factura.fechaEmision)}</p>
    <p><strong>Subtotal:</strong> $${factura.subtotal.toFixed(2)}</p>
    <p><strong>Impuestos:</strong> $${factura.impuestos.toFixed(2)}</p>
    <p><strong>Total:</strong> <strong style="color:#04bd7d;">$${factura.total.toFixed(2)}</strong></p>
    <p><strong>Método de Pago:</strong> ${MetodoPago[factura.metodoPago] ?? "Desconocido"}</p>

    <h3>Detalles de Pago:</h3>
    <table>
      <thead>
        <tr>
          <th>Descripción</th>
          <th>Cantidad</th>
          <th>Precio</th>
        </tr>
      </thead>
      <tbody>
        ${factura.detalles && factura.detalles.length > 0
          ? factura.detalles.map(d => `
              <tr>
                <td>${d.descripcion}</td>
                <td>${d.cantidad}</td>
                <td>$${d.precio.toFixed(2)}</td>
              </tr>
            `).join("")
          : `<tr><td colspan="3" style="text-align:center;">Sin detalles</td></tr>`
        }
      </tbody>
    </table>
  `;

  modal.classList.remove("hidden");
}

function cerrarModal() {
  document.getElementById("detalleModal").classList.add("hidden");
}

// ==========================
// Eliminar factura
// ==========================
async function eliminarFactura(id) {
  if (!confirm("¿Seguro que deseas eliminar esta factura?")) return;

  try {
    const response = await fetch(`${API_URL}/${id}`, {
      method: "DELETE"
    });

    if (!response.ok) throw new Error("Error al eliminar la factura");

    alert("Factura eliminada con éxito ✅");
    await cargarFacturas();
  } catch (error) {
    console.error("❌ Error eliminando factura:", error);
  }
}

// ==========================
// Utilidades
// ==========================
function formatearFecha(fechaStr) {
  const fecha = new Date(fechaStr);
  return fecha.toLocaleDateString("es-CO", {
    year: "numeric",
    month: "long",
    day: "numeric"
  });
}
