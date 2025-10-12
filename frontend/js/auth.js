document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token");

  if (!token) {
    window.location.href = "login.html";
    return;
  }

  // 🔹 Decodificar token JWT
  const payload = JSON.parse(atob(token.split(".")[1]));
  const username = payload.username || payload.email || "Usuario";
  const role = (payload.role || "").toLowerCase();

  // 🔹 Mostrar información del usuario
  document.getElementById("userName").textContent = username;
  document.getElementById("userRole").textContent = `Rol: ${role.charAt(0).toUpperCase() + role.slice(1)}`;

  // 🔹 Construcción dinámica del menú por rol
  const menu = document.getElementById("menuOptions");

  const opcionesPorRol = {
    administrador: [
      { name: "Gestión de Usuarios", url: "admin/usuarios.html", icon: "fa-users" },
      { name: "Gestión de Repuestos", url: "admin/repuestos.html", icon: "fa-toolbox" },
      { name: "Gestión de Facturas", url: "admin/facturas.html", icon: "fa-file-invoice" },
    ],

    mecanico: [
      { name: "Órdenes Asignadas", url: "mecanico/ordenes-asignadas.html", icon: "fa-wrench" },
      { name: "Facturas", url: "mecanico/facturas.html", icon: "fa-file-invoice-dollar" },
    ],

    recepcionista: [
      { name: "Registrar Nueva Orden", url: "recepcionista/registrar-orden.html", icon: "fa-plus-circle" },
      { name: "Clientes", url: "recepcionista/clientes.html", icon: "fa-id-card" },
      { name: "Vehículos", url: "recepcionista/vehiculos.html", icon: "fa-car" },
    ],
  };

  const opciones = opcionesPorRol[role] || [];

  if (opciones.length === 0) {
    menu.innerHTML = `<p style="color:red;">⚠️ No hay opciones disponibles para este rol.</p>`;
  } else {
    menu.innerHTML = opciones
      .map(
        (opt) => `
        <button onclick="window.location.href='${opt.url}'">
          <i class="fa-solid ${opt.icon}"></i> ${opt.name}
        </button>
      `
      )
      .join("");
  }

  // 🔹 Cerrar sesión
  document.getElementById("logoutBtn").addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.href = "login.html";
  });
});
