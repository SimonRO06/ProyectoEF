const API_CLIENTES = "http://localhost:5000/api/clientes/all";
const API_VEHICULOS = "http://localhost:5000/api/vehiculos/all";
const API_MODELOS = "http://localhost:5000/api/modelos/all";
const API_ORDENES = "http://localhost:5000/api/ordenes";

// Elementos del DOM
const elementos = {
    secciones: document.querySelectorAll('.seccion'),
    itemsMenu: document.querySelectorAll('.sidebar ul li'),
    tituloSeccion: document.getElementById('tituloSeccion'),
    btnAccionPrincipal: document.getElementById('btnAccionPrincipal'),
    textoBoton: document.getElementById('textoBoton'),
    contenidoDinamico: document.getElementById('contenidoDinamico'),
    logoutBtn: document.getElementById('logoutBtn')
};

// Estado de la aplicación
const estado = {
    seccionActual: 'inicio',
    datos: {
        clientes: [],
        vehiculos: [],
        ordenes: [],
        modelos: [],
        usuarios: []
    }
};

async function verDetallesOrden(id) {
    alert(`👁️ Ver detalles de orden ${id} - Funcionalidad en desarrollo`);
}

async function cancelarOrden(id) {
    if (!confirm("¿Seguro que deseas cancelar esta orden?")) return;
    
    try {
        // Aquí podrías hacer un PATCH para cambiar el estado a "Finalizado" o mantenerlo como está
        // Por ahora solo mostramos un mensaje
        alert(`✅ Orden ${id} marcada como finalizada (funcionalidad en desarrollo)`);
        
        // Recargar órdenes para reflejar el cambio
        await cargarOrdenes();
    } catch (error) {
        console.error("❌ Error cancelando orden:", error);
        alert("❌ Error al cancelar orden");
    }
}

// Inicialización
document.addEventListener('DOMContentLoaded', function() {
    inicializarNavegacion();
    inicializarEventos();
    cargarDatosIniciales();
});

function inicializarNavegacion() {
    elementos.itemsMenu.forEach(item => {
        item.addEventListener('click', function() {
            const seccion = this.getAttribute('data-section');
            cambiarSeccion(seccion);
        });
    });
}

function inicializarEventos() {
    // Logout
    elementos.logoutBtn.addEventListener('click', function() {
        localStorage.clear();
        window.location.href = 'login.html';
    });

    // Botón de acción principal
    elementos.btnAccionPrincipal.addEventListener('click', function() {
        ejecutarAccionPrincipal(estado.seccionActual);
    });

    // Modales - Botones para abrir
    const btnNuevoCliente = document.getElementById('btnNuevoCliente');
    const btnNuevoVehiculo = document.getElementById('btnNuevoVehiculo');
    
    if (btnNuevoCliente) {
        btnNuevoCliente.addEventListener('click', abrirModalCliente);
    } else {
        console.error("❌ Botón btnNuevoCliente no encontrado");
    }
    
    if (btnNuevoVehiculo) {
        btnNuevoVehiculo.addEventListener('click', abrirModalVehiculo);
    } else {
        console.error("❌ Botón btnNuevoVehiculo no encontrado");
    }

    // Botón cancelar en nueva orden
    const btnCancelarOrden = document.getElementById('btnCancelarOrden');
    if (btnCancelarOrden) {
        btnCancelarOrden.addEventListener('click', function() {
            console.log("↩️ Cancelando nueva orden, volviendo a órdenes");
            cambiarSeccion('ordenes');
        });
    } else {
        console.error("❌ Botón btnCancelarOrden no encontrado");
    }
    
    // Cerrar modales - Botones con clase cerrar-modal
    const botonesCerrar = document.querySelectorAll('.cerrar-modal');
    console.log(`🔍 Encontrados ${botonesCerrar.length} botones cerrar-modal`);
    
    botonesCerrar.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            console.log("❌ Cerrando modal desde botón cancelar");
            cerrarModales();
        });
    });

    // Formularios
    const formCliente = document.getElementById('formCliente');
    const formVehiculo = document.getElementById('formVehiculo');
    const formNuevaOrden = document.getElementById('formNuevaOrden');
    
    if (formCliente) {
        formCliente.addEventListener('submit', guardarCliente);
    } else {
        console.error("❌ Formulario formCliente no encontrado");
    }
    
    if (formVehiculo) {
        formVehiculo.addEventListener('submit', guardarVehiculo);
    } else {
        console.error("❌ Formulario formVehiculo no encontrado");
    }
    
    if (formNuevaOrden) {
        formNuevaOrden.addEventListener('submit', crearOrden);
    } else {
        console.error("❌ Formulario formNuevaOrden no encontrado");
    }
}

function cambiarSeccion(seccion) {
    // Actualizar menú
    elementos.itemsMenu.forEach(item => {
        item.classList.remove('active');
        if (item.getAttribute('data-section') === seccion) {
            item.classList.add('active');
        }
    });

    // Ocultar todas las secciones
    elementos.secciones.forEach(sec => sec.classList.remove('activa'));

    // Mostrar sección seleccionada
    document.getElementById(seccion).classList.add('activa');

    // Actualizar título y botón principal
    actualizarInterfazSeccion(seccion);

    // Cargar datos específicos de la sección
    cargarDatosSeccion(seccion);

    estado.seccionActual = seccion;
}

function actualizarInterfazSeccion(seccion) {
    const titulos = {
        'inicio': 'Panel del Recepcionista',
        'clientes': 'Gestión de Clientes',
        'vehiculos': 'Gestión de Vehículos',
        'ordenes': 'Gestión de Órdenes',
        'nueva-orden': 'Nueva Orden de Servicio'
    };

    const botones = {
        'clientes': { texto: 'Nuevo Cliente', visible: true },
        'vehiculos': { texto: 'Nuevo Vehículo', visible: true },
        'ordenes': { texto: 'Nueva Orden', visible: true },
        'nueva-orden': { texto: '', visible: false },
        'inicio': { texto: '', visible: false }
    };

    elementos.tituloSeccion.textContent = titulos[seccion];
    
    if (botones[seccion].visible) {
        elementos.textoBoton.textContent = botones[seccion].texto;
        elementos.btnAccionPrincipal.classList.remove('hidden');
    } else {
        elementos.btnAccionPrincipal.classList.add('hidden');
    }
}

function ejecutarAccionPrincipal(seccion) {
    switch(seccion) {
        case 'clientes':
            abrirModalCliente();
            break;
        case 'vehiculos':
            abrirModalVehiculo();
            break;
        case 'ordenes':
            cambiarSeccion('nueva-orden');
            break;
    }
}

// Funciones para cargar datos
async function cargarDatosIniciales() {
    try {
        await Promise.all([
            cargarClientes(),
            cargarVehiculos(),
            cargarOrdenes(),
            cargarModelos(),
            cargarUsuarios() // Agregar carga de usuarios
        ]);
        actualizarEstadisticas();
    } catch (error) {
        console.error('Error cargando datos iniciales:', error);
    }
}

async function cargarDatosSeccion(seccion) {
    switch(seccion) {
        case 'clientes':
            await cargarClientes();
            break;
        case 'vehiculos':
            await cargarVehiculos();
            break;
        case 'ordenes':
            await cargarOrdenes();
            break;
        case 'nueva-orden':
            await cargarSelectUsuarios(); // Cambiar por usuarios
            await cargarSelectVehiculos();
            // Establecer fecha mínima como hoy
            const fechaInput = document.getElementById('fechaEstimadaEntrega');
            const hoy = new Date().toISOString().split('T')[0];
            fechaInput.min = hoy;
            break;
    }
}

// ==================== FUNCIONES DE VEHÍCULOS (MEJORADAS) ====================

async function cargarVehiculos() {
    try {
        console.log("🔄 Cargando vehículos...");
        const res = await fetch(API_VEHICULOS);
        
        if (!res.ok) {
            throw new Error(`Error ${res.status}: ${res.statusText}`);
        }
        
        const data = await res.json();
        console.log("🚗 Vehículos cargados:", data);

        // Enriquecer los datos de vehículos con información de clientes y modelos
        const vehiculosEnriquecidos = await enriquecerDatosVehiculos(data);
        
        estado.datos.vehiculos = vehiculosEnriquecidos;
        renderizarVehiculos(vehiculosEnriquecidos);
    } catch (error) {
        console.error("❌ Error cargando vehículos:", error);
        const tbody = document.querySelector('#tablaVehiculos tbody');
        tbody.innerHTML = `
            <tr>
                <td colspan="7" style="text-align: center; color: red;">
                    Error al cargar vehículos: ${error.message}
                </td>
            </tr>
        `;
    }
}

// Función para enriquecer datos de vehículos con información de clientes y modelos
async function enriquecerDatosVehiculos(vehiculos) {
    try {
        // Obtener clientes y modelos
        const [clientesResponse, modelosResponse] = await Promise.all([
            fetch(API_CLIENTES),
            fetch(API_MODELOS)
        ]);

        if (!clientesResponse.ok || !modelosResponse.ok) {
            throw new Error('Error al cargar datos adicionales');
        }

        const clientes = await clientesResponse.json();
        const modelos = await modelosResponse.json();

        // Crear mapas para búsqueda rápida
        const mapaClientes = new Map(clientes.map(cliente => [cliente.id, cliente]));
        const mapaModelos = new Map(modelos.map(modelo => [modelo.id, modelo]));

        // Enriquecer cada vehículo
        return vehiculos.map(vehiculo => {
            const vehiculoEnriquecido = { ...vehiculo };
            
            // Buscar cliente por ID
            if (vehiculo.clienteId && mapaClientes.has(vehiculo.clienteId)) {
                vehiculoEnriquecido.cliente = mapaClientes.get(vehiculo.clienteId);
            } else if (vehiculo.cliente && vehiculo.cliente.id) {
                // Si ya viene el cliente pero solo con ID, buscar el nombre
                const clienteCompleto = mapaClientes.get(vehiculo.cliente.id);
                if (clienteCompleto) {
                    vehiculoEnriquecido.cliente = clienteCompleto;
                }
            }
            
            // Buscar modelo por ID
            if (vehiculo.modeloId && mapaModelos.has(vehiculo.modeloId)) {
                vehiculoEnriquecido.modelo = mapaModelos.get(vehiculo.modeloId);
            } else if (vehiculo.modelo && vehiculo.modelo.id) {
                // Si ya viene el modelo pero solo con ID, buscar el nombre
                const modeloCompleto = mapaModelos.get(vehiculo.modelo.id);
                if (modeloCompleto) {
                    vehiculoEnriquecido.modelo = modeloCompleto;
                }
            }
            
            return vehiculoEnriquecido;
        });

    } catch (error) {
        console.error("❌ Error enriqueciendo datos de vehículos:", error);
        return vehiculos; // Devolver datos originales si hay error
    }
}

async function cargarClientes() {
    try {
        console.log("🔄 Cargando clientes...");
        const res = await fetch(API_CLIENTES);
        
        if (!res.ok) {
            throw new Error(`Error ${res.status}: ${res.statusText}`);
        }
        
        const clientes = await res.json();
        console.log("👥 Clientes cargados:", clientes);

        estado.datos.clientes = clientes;
        renderizarClientes(clientes);
    } catch (error) {
        console.error("❌ Error cargando clientes:", error);
    }
}

async function cargarModelos() {
    try {
        console.log("🔄 Cargando modelos...");
        const res = await fetch(API_MODELOS);
        
        if (!res.ok) {
            throw new Error(`Error ${res.status}: ${res.statusText}`);
        }
        
        const modelos = await res.json();
        console.log("🚙 Modelos cargados:", modelos);

        estado.datos.modelos = modelos;
    } catch (error) {
        console.error("❌ Error cargando modelos:", error);
    }
}

async function cargarOrdenes() {
    try {
        const response = await fetch(API_ORDENES);
        const ordenes = await response.json();
        estado.datos.ordenes = ordenes;
        renderizarOrdenes(ordenes);
    } catch (error) {
        console.error('Error cargando órdenes:', error);
    }
}

// Funciones de renderizado MEJORADAS
function renderizarClientes(clientes) {
    const tbody = document.querySelector('#tablaClientes tbody');
    tbody.innerHTML = '';

    if (clientes.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center; color: #666;">
                    No hay clientes registrados.
                </td>
            </tr>
        `;
        return;
    }

    clientes.forEach(cliente => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${cliente.id || 'N/A'}</td>
            <td>${cliente.nombre || '—'}</td>
            <td>${cliente.telefono || '—'}</td>
            <td>${cliente.correo || '—'}</td>
            <td>${cliente.vehiculosCount || 0}</td>
            <td>
                <div class="acciones-td">
                    <button class="btn-editar" onclick="editarCliente('${cliente.id}')" title="Editar cliente">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn-eliminar" onclick="eliminarCliente('${cliente.id}')" title="Eliminar cliente">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function renderizarVehiculos(vehiculos) {
    const tbody = document.querySelector('#tablaVehiculos tbody');
    tbody.innerHTML = '';

    if (vehiculos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="7" style="text-align: center; color: #666;">
                    No hay vehículos registrados.
                </td>
            </tr>
        `;
        return;
    }

    vehiculos.forEach(vehiculo => {
        // Obtener nombre del cliente - manejar diferentes casos
        let nombreCliente = '—';
        if (vehiculo.cliente) {
            nombreCliente = vehiculo.cliente.nombre || 
                           vehiculo.cliente.nombreCompleto || 
                           `Cliente ${vehiculo.cliente.id}`;
        } else if (vehiculo.clienteId) {
            nombreCliente = `Cliente ${vehiculo.clienteId}`;
        }

        // Obtener nombre del modelo - manejar diferentes casos
        let nombreModelo = '—';
        if (vehiculo.modelo) {
            nombreModelo = vehiculo.modelo.nombre || 
                          vehiculo.modelo.descripcion || 
                          `Modelo ${vehiculo.modelo.id}`;
        } else if (vehiculo.modeloId) {
            nombreModelo = `Modelo ${vehiculo.modeloId}`;
        } else if (vehiculo.modelo) {
            // Si modelo es un string directamente
            nombreModelo = vehiculo.modelo;
        }

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${vehiculo.id || 'N/A'}</td>
            <td>${nombreCliente}</td>
            <td>${nombreModelo}</td>
            <td>${vehiculo.año || 'N/A'}</td>
            <td>${vehiculo.numeroSerie || 'N/A'}</td>
            <td>${vehiculo.kilometraje || 0} km</td>
            <td>
                <div class="acciones-td">
                    <button class="btn-editar" onclick="editarVehiculo('${vehiculo.id}')" title="Editar vehículo">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn-eliminar" onclick="eliminarVehiculo('${vehiculo.id}')" title="Eliminar vehículo">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// En renderizarOrdenes, puedes mostrar el tipo de servicio si quieres:
function renderizarOrdenes(ordenes) {
    const tbody = document.querySelector('#tablaOrdenes tbody');
    tbody.innerHTML = '';

    if (ordenes.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center; color: #666;">
                    No hay órdenes registradas.
                </td>
            </tr>
        `;
        return;
    }

    ordenes.forEach(orden => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>#${orden.id || 'N/A'}</td>
            <td>${orden.usuario?.nombre || orden.usuario?.userName || '—'}</td> <!-- Cambiar a usuario -->
            <td>${orden.vehiculo?.modelo?.nombre || orden.vehiculo?.modelo || '—'}</td>
            <td>${new Date(orden.fechaIngreso).toLocaleDateString()}</td>
            <td><span class="estado-badge estado-${getEstadoClase(orden.estado)}">${getEstadoTexto(orden.estado)}</span></td>
            <td>
                <div class="acciones-td">
                    <button class="btn-editar" onclick="verDetallesOrden('${orden.id}')" title="Ver detalles">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn-eliminar" onclick="cancelarOrden('${orden.id}')" title="Cancelar orden">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// Funciones auxiliares para estados
function getEstadoClase(estado) {
    const estados = {
        0: 'pendiente',
        1: 'en-progreso', 
        2: 'finalizado'
    };
    return estados[estado] || 'pendiente';
}

function getEstadoTexto(estado) {
    const estados = {
        0: 'Pendiente',
        1: 'En Progreso',
        2: 'Finalizado'
    };
    return estados[estado] || 'Pendiente';
}



// Funciones para selects
async function cargarSelectClientes() {
    const selectOrden = document.getElementById('clienteOrden');
    const selectVehiculo = document.getElementById('clienteVehiculo');
    
    selectOrden.innerHTML = '<option value="">Seleccione un cliente</option>';
    selectVehiculo.innerHTML = '<option value="">Seleccione un cliente</option>';

    estado.datos.clientes.forEach(cliente => {
        const option = document.createElement('option');
        option.value = cliente.id;
        option.textContent = `${cliente.nombre} - ${cliente.telefono}`;
        
        selectOrden.appendChild(option.cloneNode(true));
        selectVehiculo.appendChild(option);
    });
}

async function cargarSelectUsuarios() {
    const select = document.getElementById('usuarioOrden');
    
    if (!select) {
        console.error("❌ Select de usuarios no encontrado");
        return;
    }
    
    // Mostrar estado de carga
    select.innerHTML = '<option value="">Cargando usuarios...</option>';
    select.disabled = true;

    try {
        const response = await fetch("http://localhost:5000/api/users/all");
        if (!response.ok) throw new Error('Error al cargar usuarios');
        
        const usuarios = await response.json();
        
        // Limpiar y llenar el select
        select.innerHTML = '<option value="">Seleccione un usuario</option>';
        select.disabled = false;

        usuarios.forEach(usuario => {
            const option = document.createElement('option');
            option.value = usuario.id;
            // Mostrar información del usuario (ajusta según la estructura de tu objeto Usuario)
            option.textContent = `${usuario.nombre || usuario.userName} - ${usuario.email || ''}`;
            select.appendChild(option);
        });
        
        console.log(`✅ ${usuarios.length} usuarios cargados en el select`);
        
    } catch (error) {
        console.error('❌ Error cargando usuarios para orden:', error);
        select.innerHTML = '<option value="">Error al cargar usuarios</option>';
    }
}

async function cargarSelectVehiculos() {
    const select = document.getElementById('vehiculoOrden');
    
    // Mostrar estado de carga
    select.innerHTML = '<option value="">Cargando vehículos...</option>';
    select.disabled = true;

    try {
        const response = await fetch(API_VEHICULOS);
        if (!response.ok) throw new Error('Error al cargar vehículos');
        
        const vehiculos = await response.json();
        
        // Limpiar y llenar el select
        select.innerHTML = '<option value="">Seleccione un vehículo</option>';
        select.disabled = false;

        vehiculos.forEach(vehiculo => {
            const option = document.createElement('option');
            option.value = vehiculo.id;
            
            // Mostrar información útil: Modelo - Placa - Cliente
            const modelo = vehiculo.modelo?.nombre || vehiculo.modelo || 'Modelo no disponible';
            const cliente = vehiculo.cliente?.nombre || 'Cliente no asignado';
            option.textContent = `${modelo} - ${vehiculo.numeroSerie} (${cliente})`;
            
            select.appendChild(option);
        });
        
        console.log(`✅ ${vehiculos.length} vehículos cargados en el select`);
        
    } catch (error) {
        console.error('❌ Error cargando vehículos para orden:', error);
        select.innerHTML = '<option value="">Error al cargar vehículos</option>';
    }
}

async function cargarSelectModelos() {
    const select = document.getElementById('modeloVehiculo');
    select.innerHTML = '<option value="">Seleccione un modelo</option>';

    estado.datos.modelos.forEach(modelo => {
        const option = document.createElement('option');
        option.value = modelo.id;
        option.textContent = modelo.nombre;
        select.appendChild(option);
    });
}

// Funciones de modales
function abrirModalCliente() {
    document.getElementById('modalCliente').classList.remove('hidden');
}

function abrirModalVehiculo() {
    // Cargar clientes y modelos en los selects
    cargarSelectClientesModal();
    cargarSelectModelosModal();
    
    // Resetear el formulario
    document.getElementById('formVehiculo').reset();
    
    // Mostrar el modal
    document.getElementById('modalVehiculo').classList.remove('hidden');
    
    console.log("📝 Abriendo modal para nuevo vehículo");
}

// Función específica para cargar clientes en el modal de vehículos
async function cargarSelectClientesModal() {
    const select = document.getElementById('clienteVehiculo');
    
    // Mostrar estado de carga
    select.innerHTML = '<option value="">Cargando clientes...</option>';
    select.disabled = true;

    try {
        const response = await fetch("http://localhost:5000/api/clientes/all");
        if (!response.ok) throw new Error('Error al cargar clientes');
        
        const clientes = await response.json();
        
        // Limpiar y llenar el select
        select.innerHTML = '<option value="">Seleccione un cliente</option>';
        select.disabled = false;

        clientes.forEach(cliente => {
            const option = document.createElement('option');
            option.value = cliente.id;
            option.textContent = `${cliente.nombre} - ${cliente.telefono}`;
            select.appendChild(option);
        });
        
        console.log(`✅ ${clientes.length} clientes cargados en el select`);
        
    } catch (error) {
        console.error('❌ Error cargando clientes para el modal:', error);
        select.innerHTML = '<option value="">Error al cargar clientes</option>';
    }
}

// Función específica para cargar modelos en el modal de vehículos
async function cargarSelectModelosModal() {
    const select = document.getElementById('modeloVehiculo');
    
    // Mostrar estado de carga
    select.innerHTML = '<option value="">Cargando modelos...</option>';
    select.disabled = true;

    try {
        const response = await fetch("http://localhost:5000/api/modelos/all");
        if (!response.ok) throw new Error('Error al cargar modelos');
        
        const modelos = await response.json();
        
        // Limpiar y llenar el select
        select.innerHTML = '<option value="">Seleccione un modelo</option>';
        select.disabled = false;

        modelos.forEach(modelo => {
            const option = document.createElement('option');
            option.value = modelo.id;
            option.textContent = modelo.nombre;
            select.appendChild(option);
        });
        
        console.log(`✅ ${modelos.length} modelos cargados en el select`);
        
    } catch (error) {
        console.error('❌ Error cargando modelos para el modal:', error);
        select.innerHTML = '<option value="">Error al cargar modelos</option>';
    }
}

function cerrarModales() {
    console.log("🔒 Cerrando modales...");
    
    const modales = document.querySelectorAll('.modal');
    console.log(`📦 Encontrados ${modales.length} modales`);
    
    modales.forEach(modal => {
        console.log(`👁️ Ocultando modal: ${modal.id}`);
        modal.classList.add('hidden');
    });
    
    // Resetear formularios
    const forms = ['formCliente', 'formVehiculo', 'formNuevaOrden'];
    forms.forEach(formId => {
        const form = document.getElementById(formId);
        if (form) {
            form.reset();
            console.log(`🔄 Formulario ${formId} reseteado`);
        }
    });
}

// ==================== FUNCIONES PARA GUARDAR VEHÍCULOS ====================

async function guardarVehiculo(e) {
    e.preventDefault();
    
    console.log("💾 Guardando nuevo vehículo...");

    // Obtener el formulario
    const form = e.target;
    const formData = new FormData(form);

    // Obtener valores por name en lugar de ID
    const año = formData.get('año');
    const numeroSerie = formData.get('numeroSerie');
    const kilometraje = formData.get('kilometraje');
    const clienteId = formData.get('clienteId');
    const modeloId = formData.get('modeloId');

    const nuevoVehiculo = {
        año: parseInt(año),
        numeroSerie: numeroSerie.trim(),
        kilometraje: parseInt(kilometraje),
        clienteId: clienteId,
        modeloId: modeloId
    };

    console.log("📦 Datos del vehículo:", nuevoVehiculo);

    // Validaciones (las mismas de arriba)
    if (!nuevoVehiculo.clienteId) {
        alert("❌ Por favor selecciona un cliente");
        return;
    }

    if (!nuevoVehiculo.modeloId) {
        alert("❌ Por favor selecciona un modelo");
        return;
    }

    if (nuevoVehiculo.año < 1900 || nuevoVehiculo.año > new Date().getFullYear() + 1) {
        alert("❌ El año debe ser válido");
        return;
    }

    if (nuevoVehiculo.kilometraje < 0) {
        alert("❌ El kilometraje no puede ser negativo");
        return;
    }

    try {
        const res = await fetch("http://localhost:5000/api/vehiculos", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(nuevoVehiculo)
        });

        console.log("📡 Respuesta del servidor:", res.status, res.statusText);

        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(errorText || "Error al guardar vehículo");
        }

        const resultado = await res.json();
        console.log("✅ Vehículo guardado:", resultado);

        cerrarModales();
        
        // Recargar la lista de vehículos
        await cargarVehiculos();
        await cargarSelectVehiculos();
        actualizarEstadisticas();
        
        alert("✅ Vehículo registrado correctamente");
        
    } catch (error) {
        console.error("❌ Error guardando vehículo:", error);
        alert("❌ Error: " + error.message);
    }
}

async function guardarCliente(e) {
    e.preventDefault();
    
    console.log("💾 Guardando nuevo cliente...");

    // Obtener elementos de forma segura
    const nombreInput = document.getElementById('nombreCliente');
    const telefonoInput = document.getElementById('telefonoCliente');
    const emailInput = document.getElementById('emailCliente');

    // Verificar que los elementos existan
    if (!nombreInput || !telefonoInput || !emailInput) {
        console.error("❌ Elementos del formulario de cliente no encontrados");
        alert("❌ Error en el formulario. Por favor recarga la página.");
        return;
    }

    const cliente = {
        nombre: nombreInput.value.trim(),
        telefono: telefonoInput.value.trim(),
        correo: emailInput.value.trim()
    };

    console.log("📦 Datos del cliente:", cliente);

    // Validaciones
    if (!cliente.nombre) {
        alert("❌ El nombre es requerido");
        return;
    }

    if (!cliente.telefono) {
        alert("❌ El teléfono es requerido");
        return;
    }

    try {
        const response = await fetch("http://localhost:5000/api/clientes", {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(cliente)
        });

        if (response.ok) {
            const resultado = await response.json();
            console.log("✅ Cliente guardado:", resultado);
            
            cerrarModales();
            await cargarClientes();
            await cargarSelectClientes();
            actualizarEstadisticas();
            alert('✅ Cliente guardado correctamente');
        } else {
            const errorText = await response.text();
            throw new Error(errorText || 'Error al guardar cliente');
        }
    } catch (error) {
        console.error('❌ Error guardando cliente:', error);
        alert('❌ Error al guardar cliente: ' + error.message);
    }
}

function verificarElementosDOM() {
    console.log("🔍 VERIFICACIÓN DE ELEMENTOS DEL DOM:");
    
    const elementosClave = [
        'nombreCliente', 'telefonoCliente', 'emailCliente',
        'btnNuevoCliente', 'btnNuevoVehiculo', 'btnCancelarOrden',
        'formCliente', 'formVehiculo', 'formNuevaOrden',
        'modalCliente', 'modalVehiculo'
    ];
    
    elementosClave.forEach(id => {
        const elemento = document.getElementById(id);
        console.log(`- ${id}:`, elemento ? '✅ Encontrado' : '❌ No encontrado');
    });
    
    // Verificar botones cerrar-modal
    const botonesCerrar = document.querySelectorAll('.cerrar-modal');
    console.log(`- Botones .cerrar-modal: ${botonesCerrar.length} encontrados`);
}

async function crearOrden(e) {
    e.preventDefault();
    
    console.log("📋 Creando nueva orden...");

    // Obtener elementos de forma segura
    const tipoServicioSelect = document.getElementById('tipoServicio');
    const fechaEstimadaInput = document.getElementById('fechaEstimadaEntrega');
    const vehiculoSelect = document.getElementById('vehiculoOrden');
    const usuarioSelect = document.getElementById('usuarioOrden'); // Cambiar cliente por usuario
    const descripcionTextarea = document.getElementById('descripcionProblema');

    // Verificar que los elementos existan
    if (!tipoServicioSelect || !fechaEstimadaInput || !vehiculoSelect || !usuarioSelect) {
        console.error("❌ Elementos del formulario no encontrados");
        alert("❌ Error en el formulario. Por favor recarga la página.");
        return;
    }

    // Obtener valores
    const vehiculoId = vehiculoSelect.value;
    const usuarioId = usuarioSelect.value; // Obtener ID del usuario
    const tipoServicio = parseInt(tipoServicioSelect.value);
    const fechaEstimadaEntrega = fechaEstimadaInput.value;
    const descripcionProblema = descripcionTextarea ? descripcionTextarea.value : "";

    // Validar que el tipoServicio sea válido según el enum (1, 2, 3)
    if (tipoServicio < 1 || tipoServicio > 3) {
        alert("❌ Por favor selecciona un tipo de servicio válido");
        return;
    }

    // Validaciones básicas
    if (!vehiculoId) {
        alert("❌ Por favor selecciona un vehículo");
        return;
    }

    if (!usuarioId) {
        alert("❌ Por favor selecciona un usuario");
        return;
    }

    if (!fechaEstimadaEntrega) {
        alert("❌ Por favor selecciona una fecha estimada de entrega");
        return;
    }

    // Formatear correctamente la fecha a YYYY-MM-DD
    function formatearFechaISO(fechaString) {
        const fecha = new Date(fechaString);
        const año = fecha.getFullYear();
        const mes = String(fecha.getMonth() + 1).padStart(2, '0');
        const dia = String(fecha.getDate()).padStart(2, '0');
        return `${año}-${mes}-${dia}`;
    }

    // Crear objeto de orden según tu entidad - Estado 0 = Pendiente
    const nuevaOrden = {
        tipoServicio: tipoServicio,
        fechaIngreso: new Date().toISOString().split('T')[0], // Solo la fecha sin hora
        fechaEstimadaEntrega: formatearFechaISO(fechaEstimadaEntrega),
        estado: 0, // Estado.Pendiente (según tu enum)
        vehiculoId: vehiculoId,
        usuarioId: usuarioId, // Cambiar de clienteId a usuarioId
        descripcionProblema: descripcionProblema || "Sin descripción adicional"
    };

    console.log("📦 Datos de la orden a enviar:", nuevaOrden);

    try {
        const response = await fetch("http://localhost:5000/api/ordenes", {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(nuevaOrden)
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error("❌ Error del servidor:", errorText);
            
            if (errorText.includes("fecha") || errorText.includes("date")) {
                alert("❌ Error en el formato de fecha. Por favor verifica la fecha ingresada.");
            } else {
                throw new Error(errorText || 'Error al crear orden');
            }
            return;
        }

        const resultado = await response.json();
        console.log("✅ Orden creada:", resultado);

        alert('✅ Orden creada correctamente');
        
        // Resetear formulario
        document.getElementById('formNuevaOrden').reset();
        
        // Recargar la lista de órdenes
        await cargarOrdenes();
        actualizarEstadisticas();
        
        // Regresar a la sección de órdenes
        cambiarSeccion('ordenes');
        
    } catch (error) {
        console.error('❌ Error creando orden:', error);
        alert('❌ Error al crear orden: ' + error.message);
    }
}

async function cargarUsuarios() {
    try {
        console.log("🔄 Cargando usuarios...");
        const res = await fetch("http://localhost:5000/api/users/all");
        
        if (!res.ok) {
            throw new Error(`Error ${res.status}: ${res.statusText}`);
        }
        
        const usuarios = await res.json();
        console.log("👤 Usuarios cargados:", usuarios);

        estado.datos.usuarios = usuarios;
    } catch (error) {
        console.error("❌ Error cargando usuarios:", error);
    }
}

// Funciones de edición y eliminación para vehículos
async function editarVehiculo(id) {
    try {
        console.log("✏️ Editando vehículo:", id);
        
        const res = await fetch(`http://localhost:5000/api/vehiculos/${id}`);
        if (!res.ok) throw new Error("No se encontró el vehículo");
        
        const vehiculo = await res.json();
        console.log("📋 Vehículo a editar:", vehiculo);

        alert(`✏️ Funcionalidad de edición en desarrollo.\nVehículo ID: ${vehiculo.id}\nModelo: ${vehiculo.modelo?.nombre || vehiculo.modelo}\nCliente: ${vehiculo.cliente?.nombre}`);
        
    } catch (error) {
        console.error("❌ Error editando vehículo:", error);
        alert("❌ Error al cargar vehículo para editar: " + error.message);
    }
}

async function eliminarVehiculo(id) {
    if (!confirm("¿Seguro que deseas eliminar este vehículo?")) return;
  
    try {
        console.log("🗑️ Eliminando vehículo:", id);
        const res = await fetch(`http://localhost:5000/api/vehiculos/${id}`, { method: "DELETE" });
        
        if (!res.ok) {
            throw new Error("Error al eliminar vehículo");
        }
        
        await cargarVehiculos();
        await cargarSelectVehiculos();
        actualizarEstadisticas();
        alert("✅ Vehículo eliminado correctamente");
    } catch (error) {
        console.error("❌ Error eliminando vehículo:", error);
        alert("❌ Error al eliminar vehículo");
    }
}

// Funciones para clientes (placeholder)
async function editarCliente(id) {
    alert(`✏️ Editar cliente ${id} - Funcionalidad en desarrollo`);
}

async function eliminarCliente(id) {
    if (!confirm("¿Seguro que deseas eliminar este cliente?")) return;
    alert(`🗑️ Eliminar cliente ${id} - Funcionalidad en desarrollo`);
}

// Estadísticas
function actualizarEstadisticas() {
    document.getElementById('totalClientes').textContent = estado.datos.clientes.length;
    document.getElementById('totalVehiculos').textContent = estado.datos.vehiculos.length;
    document.getElementById('totalOrdenes').textContent = estado.datos.ordenes.length;
}

// Función auxiliar para convertir el número del enum a texto
function getTipoServicioTexto(tipoServicio) {
    const tipos = {
        1: 'Preventivo',
        2: 'Reparación', 
        3: 'Diagnóstico'
    };
    return tipos[tipoServicio] || 'Desconocido';
}

setTimeout(verificarElementosDOM, 1000);

// CSS adicional para estados y acciones
const estiloEstados = document.createElement('style');
estiloEstados.textContent = `
    .estado-badge {
        padding: 4px 8px;
        border-radius: 12px;
        font-size: 0.8rem;
        font-weight: 500;
    }
    .estado-pendiente { background: #fef3c7; color: #92400e; }
    .estado-en-proceso { background: #dbeafe; color: #1e40af; }
    .estado-completado { background: #d1fae5; color: #065f46; }
    .estado-cancelado { background: #fee2e2; color: #991b1b; }
    
    .acciones-td {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 5px;
        padding: 8px 0;
    }
    
    .btn-editar, .btn-eliminar {
        width: 32px;
        height: 32px;
        display: flex;
        align-items: center;
        justify-content: center;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 0.9rem;
        transition: all 0.2s ease;
    }
    
    .btn-editar {
        background-color: #e3f2fd;
        color: #1976d2;
    }
    
    .btn-editar:hover {
        background-color: #bbdefb;
    }
    
    .btn-eliminar {
        background-color: #ffebee;
        color: #d32f2f;
    }
    
    .btn-eliminar:hover {
        background-color: #ffcdd2;
    }
`;
document.head.appendChild(estiloEstados);