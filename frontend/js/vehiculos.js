const API_BASE = "http://localhost:5070/api"; // Ajusta al puerto real de tu backend

// Referencias del DOM
const modal = document.getElementById("modalVehiculo");
const nuevoVehiculoBtn = document.getElementById("nuevoVehiculoBtn");
const cancelarBtn = document.getElementById("cancelarBtn");
const vehiculoForm = document.getElementById("vehiculoForm");
const vehiculosTableBody = document.getElementById("vehiculosTableBody");

// Mostrar modal
nuevoVehiculoBtn.addEventListener("click", () => modal.classList.remove("hidden"));
cancelarBtn.addEventListener("click", () => modal.classList.add("hidden"));

// Cargar vehículos
async function cargarVehiculos() {
  const res = await fetch(`${API_BASE}/vehiculo`);
  const data = await res.json();

  vehiculosTableBody.innerHTML = "";
  data.forEach(v => {
    const fila = `
      <tr>
        <td>${v.id}</td>
        <td>${v.año}</td>
        <td>${v.numeroSerie}</td>
        <td>${v.kilometraje} km</td>
        <td>${v.cliente?.nombre ?? "—"}</td>
        <td>${v.modelo?.nombre ?? "—"}</td>
        <td>
          <button class="btn-editar" onclick="editarVehiculo('${v.id}')"><i class="fas fa-edit"></i></button>
          <button class="btn-eliminar" onclick="eliminarVehiculo('${v.id}')"><i class="fas fa-trash"></i></button>
        </td>
      </tr>`;
    vehiculosTableBody.insertAdjacentHTML("beforeend", fila);
  });
}

// Cargar clientes
async function cargarClientes() {
  const res = await fetch(`${API_BASE}/cliente`);
  const clientes = await res.json();
  const select = document.getElementById("cliente");

  clientes.forEach(c => {
    const option = document.createElement("option");
    option.value = c.id;
    option.textContent = c.nombre;
    select.appendChild(option);
  });
}

// Cargar modelos
async function cargarModelos() {
  const res = await fetch(`${API_BASE}/modelo`);
  const modelos = await res.json();
  const select = document.getElementById("modelo");

  modelos.forEach(m => {
    const option = document.createElement("option");
    option.value = m.id;
    option.textContent = m.nombre;
    select.appendChild(option);
  });
}

// Guardar nuevo vehículo
vehiculoForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const nuevoVehiculo = {
    año: parseInt(document.getElementById("año").value),
    numeroSerie: document.getElementById("numeroSerie").value,
    kilometraje: parseInt(document.getElementById("kilometraje").value),
    clienteId: document.getElementById("cliente").value,
    modeloId: document.getElementById("modelo").value
  };

  await fetch(`${API_BASE}/vehiculo`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(nuevoVehiculo)
  });

  modal.classList.add("hidden");
  vehiculoForm.reset();
  cargarVehiculos();
});

// Eliminar vehículo
async function eliminarVehiculo(id) {
  if (!confirm("¿Seguro que deseas eliminar este vehículo?")) return;
  await fetch(`${API_BASE}/vehiculo/${id}`, { method: "DELETE" });
  cargarVehiculos();
}

// Inicialización
cargarVehiculos();
cargarClientes();
cargarModelos();
