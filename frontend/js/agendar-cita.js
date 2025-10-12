document.addEventListener("DOMContentLoaded", async () => {
  const tbody = document.querySelector("#citasTable tbody");

  try {
    const response = await fetch("https://localhost:5001/api/citas"); // Ajusta tu URL
    const citas = await response.json();

    tbody.innerHTML = ""; // limpiar
    citas.forEach(cita => {
      const fila = document.createElement("tr");

      // asignar clase de color según estado
      const estadoClase = cita.estado.toLowerCase();

      fila.innerHTML = `
        <td>${cita.id}</td>
        <td>${cita.cliente}</td>
        <td>${cita.vehiculo}</td>
        <td>${cita.tipoServicio}</td>
        <td>${cita.mecanico}</td>
        <td>${new Date(cita.fechaCita).toLocaleDateString()}</td>
        <td><span class="estado ${estadoClase}">${cita.estado}</span></td>
      `;

      tbody.appendChild(fila);
    });
  } catch (error) {
    console.error("Error al cargar citas:", error);
  }
});
