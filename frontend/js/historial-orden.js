document.addEventListener("DOMContentLoaded", async () => {
  const tabla = document.querySelector("#ordenesTable tbody");
  const searchInput = document.getElementById("searchInput");
  const modal = document.getElementById("detalleModal");
  const closeModal = document.getElementById("closeModal");
  const detalleContent = document.getElementById("detalleOrdenContent");

  // 🚀 Cargar órdenes desde el backend
  async function cargarOrdenes() {
    try {
      const res = await fetch("http://localhost:5000/api/ordenes/all"); // Ajusta el endpoint
      const data = await res.json();
      renderTabla(data);
      searchInput.addEventListener("input", () => filtrarOrdenes(data));
    } catch (error) {
      console.error("Error cargando órdenes:", error);
    }
  }

  function renderTabla(ordenes) {
    tabla.innerHTML = "";
    ordenes.forEach(o => {
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${o.id}</td>
        <td>${o.vehiculo?.placa ?? "N/A"}</td>
        <td>${o.cliente?.nombre ?? "N/A"}</td>
        <td>${o.tipoServicio}</td>
        <td>${o.mecanico?.nombre ?? "N/A"}</td>
        <td>${o.estado}</td>
        <td>${new Date(o.fechaIngreso).toLocaleDateString()}</td>
        <td><button class="btn-detalle"><i class="fas fa-eye"></i></button></td>
      `;
      tr.querySelector(".btn-detalle").addEventListener("click", () => abrirModal(o));
      tabla.appendChild(tr);
    });
  }

  function filtrarOrdenes(ordenes) {
    const term = searchInput.value.toLowerCase();
    const filtradas = ordenes.filter(o =>
      o.cliente?.nombre.toLowerCase().includes(term) ||
      o.vehiculo?.placa.toLowerCase().includes(term) ||
      o.estado.toLowerCase().includes(term)
    );
    renderTabla(filtradas);
  }

  function abrirModal(orden) {
    detalleContent.innerHTML = `
      <p><strong>Cliente:</strong> ${orden.cliente?.nombre}</p>
      <p><strong>Vehículo:</strong> ${orden.vehiculo?.placa}</p>
      <p><strong>Tipo de Servicio:</strong> ${orden.tipoServicio}</p>
      <p><strong>Estado:</strong> ${orden.estado}</p>
      <p><strong>Descripción:</strong> ${orden.descripcion}</p>
      <p><strong>Fecha de Ingreso:</strong> ${new Date(orden.fechaIngreso).toLocaleString()}</p>
    `;
    modal.classList.remove("hidden");
  }

  closeModal.addEventListener("click", () => modal.classList.add("hidden"));
  window.addEventListener("click", e => {
    if (e.target === modal) modal.classList.add("hidden");
  });

  cargarOrdenes();
});
