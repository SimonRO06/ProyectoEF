class MecanicoApp {
    constructor() {
        this.ordenes = [];
        this.ordenActual = null;
        this.token = localStorage.getItem('token');
        this.userId = localStorage.getItem('userId');
        this.userName = localStorage.getItem('userName');
        
        this.init();
    }

    init() {
        this.checkAuth();
        this.loadUserInfo();
        this.setupEventListeners();
        this.loadOrdenes();
    }

    checkAuth() {
        if (!this.token) {
            window.location.href = 'login.html';
            return;
        }

        // Verificar que el usuario sea mecánico
        const userRole = localStorage.getItem('userRole');
        if (userRole !== 'Mecánico') {
            alert('No tienes permisos para acceder a esta página');
            window.location.href = 'login.html';
            return;
        }
    }

    loadUserInfo() {
        if (this.userName) {
            document.getElementById('userName').textContent = this.userName;
        }
    }

    setupEventListeners() {
        // Logout
        document.getElementById('logoutBtn').addEventListener('click', () => {
            localStorage.clear();
            window.location.href = 'login.html';
        });

        // Filtros
        document.getElementById('filtroEstado').addEventListener('change', () => this.filterOrdenes());
        document.getElementById('filtroCliente').addEventListener('input', () => this.filterOrdenes());
        document.getElementById('btnActualizar').addEventListener('click', () => this.loadOrdenes());

        // Modal eventos
        document.getElementById('btnCerrarModal').addEventListener('click', () => this.closeModal());
        document.getElementById('selectEstado').addEventListener('change', (e) => this.onEstadoChange(e.target.value));
        document.getElementById('btnGuardarCambios').addEventListener('click', () => this.guardarCambios());
        document.getElementById('btnGenerarFactura').addEventListener('click', () => this.generarFactura());

        // Confirmación modal
        document.getElementById('btnCancelarConfirm').addEventListener('click', () => this.closeConfirmModal());
    }

    async loadOrdenes() {
        try {
            const response = await fetch('/api/ordenesservicio', {
                headers: {
                    'Authorization': `Bearer ${this.token}`
                }
            });

            if (!response.ok) throw new Error('Error al cargar órdenes');

            this.ordenes = await response.json();
            this.renderOrdenes();
            this.updateStats();
        } catch (error) {
            console.error('Error:', error);
            alert('Error al cargar las órdenes');
        }
    }

    renderOrdenes() {
        const tbody = document.getElementById('ordenesBody');
        const filteredOrdenes = this.filterOrdenes();

        tbody.innerHTML = filteredOrdenes.map(orden => `
            <tr>
                <td>${orden.id.substring(0, 8)}</td>
                <td>${orden.vehiculo?.cliente?.nombre || 'N/A'}</td>
                <td>${orden.vehiculo?.marca || ''} ${orden.vehiculo?.modelo || ''}</td>
                <td>${this.getTipoServicioText(orden.tipoServicio)}</td>
                <td>${new Date(orden.fechaIngreso).toLocaleDateString()}</td>
                <td>
                    <span class="estado-badge estado-${orden.estado.toLowerCase()}">
                        ${this.getEstadoText(orden.estado)}
                    </span>
                </td>
                <td class="acciones-cell">
                    <button class="btn-accion btn-gestionar" onclick="app.openOrden('${orden.id}')">
                        <i class="fas fa-edit"></i> Gestionar
                    </button>
                </td>
            </tr>
        `).join('');
    }

    filterOrdenes() {
        const estadoFilter = document.getElementById('filtroEstado').value;
        const clienteFilter = document.getElementById('filtroCliente').value.toLowerCase();

        const filtered = this.ordenes.filter(orden => {
            const matchesEstado = !estadoFilter || orden.estado === estadoFilter;
            const matchesCliente = !clienteFilter || 
                (orden.vehiculo?.cliente?.nombre?.toLowerCase().includes(clienteFilter));
            
            return matchesEstado && matchesCliente;
        });

        document.getElementById('ordenesBody').innerHTML = filtered.map(orden => `
            <tr>
                <td>${orden.id.substring(0, 8)}</td>
                <td>${orden.vehiculo?.cliente?.nombre || 'N/A'}</td>
                <td>${orden.vehiculo?.marca || ''} ${orden.vehiculo?.modelo || ''}</td>
                <td>${this.getTipoServicioText(orden.tipoServicio)}</td>
                <td>${new Date(orden.fechaIngreso).toLocaleDateString()}</td>
                <td>
                    <span class="estado-badge estado-${orden.estado.toLowerCase()}">
                        ${this.getEstadoText(orden.estado)}
                    </span>
                </td>
                <td class="acciones-cell">
                    <button class="btn-accion btn-gestionar" onclick="app.openOrden('${orden.id}')">
                        <i class="fas fa-edit"></i> Gestionar
                    </button>
                </td>
            </tr>
        `).join('');

        return filtered;
    }

    updateStats() {
        const countPendiente = this.ordenes.filter(o => o.estado === 'Pendiente').length;
        const countEnProgreso = this.ordenes.filter(o => o.estado === 'EnProgreso').length;
        const countFinalizado = this.ordenes.filter(o => o.estado === 'Finalizado').length;

        document.getElementById('countPendiente').textContent = countPendiente;
        document.getElementById('countEnProgreso').textContent = countEnProgreso;
        document.getElementById('countFinalizado').textContent = countFinalizado;
    }

    openOrden(ordenId) {
        this.ordenActual = this.ordenes.find(o => o.id === ordenId);
        if (!this.ordenActual) return;

        this.loadOrdenDetails();
        this.showModal();
    }

    loadOrdenDetails() {
        // Información básica
        document.getElementById('detalleNumero').textContent = this.ordenActual.id.substring(0, 8);
        document.getElementById('detalleCliente').textContent = this.ordenActual.vehiculo?.cliente?.nombre || 'N/A';
        document.getElementById('detalleVehiculo').textContent = 
            `${this.ordenActual.vehiculo?.marca || ''} ${this.ordenActual.vehiculo?.modelo || ''} - ${this.ordenActual.vehiculo?.numeroSerie || ''}`;
        document.getElementById('detalleTipoServicio').textContent = this.getTipoServicioText(this.ordenActual.tipoServicio);
        document.getElementById('detalleFechaIngreso').textContent = new Date(this.ordenActual.fechaIngreso).toLocaleDateString();

        // Estado actual
        document.getElementById('selectEstado').value = this.ordenActual.estado;
        document.getElementById('notasTrabajo').value = this.ordenActual.notasTrabajo || '';

        // Repuestos
        this.loadRepuestos();

        // Habilitar/deshabilitar botones según estado
        this.updateActionButtons();
    }

    loadRepuestos() {
        const repuestosContainer = document.getElementById('listaRepuestos');
        
        if (this.ordenActual.detallesOrdenes && this.ordenActual.detallesOrdenes.length > 0) {
            repuestosContainer.innerHTML = this.ordenActual.detallesOrdenes.map(detalle => `
                <div class="repuesto-item">
                    <div class="repuesto-info">
                        <div class="repuesto-codigo">${detalle.repuesto?.codigo || 'N/A'}</div>
                        <div class="repuesto-desc">${detalle.repuesto?.descripcion || 'Sin descripción'}</div>
                    </div>
                    <div class="repuesto-cantidad">
                        ${detalle.cantidad} unidades
                    </div>
                </div>
            `).join('');
        } else {
            repuestosContainer.innerHTML = '<p style="text-align: center; color: #6b7280;">No hay repuestos asignados</p>';
        }
    }

    onEstadoChange(nuevoEstado) {
        this.updateActionButtons();
    }

    updateActionButtons() {
        const estadoActual = document.getElementById('selectEstado').value;
        const btnFactura = document.getElementById('btnGenerarFactura');

        // Solo permitir generar factura si la orden está finalizada
        btnFactura.disabled = estadoActual !== 'Finalizado';
    }

    async guardarCambios() {
        if (!this.ordenActual) return;

        const nuevoEstado = document.getElementById('selectEstado').value;
        const notasTrabajo = document.getElementById('notasTrabajo').value;

        try {
            const updateData = {
                estado: nuevoEstado,
                notasTrabajo: notasTrabajo,
                userMemberId: parseInt(this.userId)
            };

            const response = await fetch(`/api/ordenesservicio/${this.ordenActual.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.token}`
                },
                body: JSON.stringify(updateData)
            });

            if (!response.ok) throw new Error('Error al actualizar la orden');

            alert('Orden actualizada correctamente');
            this.closeModal();
            this.loadOrdenes();

        } catch (error) {
            console.error('Error:', error);
            alert('Error al actualizar la orden');
        }
    }

    async generarFactura() {
        if (!this.ordenActual) return;

        try {
            const response = await fetch('/api/facturas/generar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.token}`
                },
                body: JSON.stringify({
                    ordenServicioId: this.ordenActual.id
                })
            });

            if (!response.ok) throw new Error('Error al generar factura');

            const factura = await response.json();
            alert(`Factura generada correctamente. Total: $${factura.total}`);
            this.closeModal();
            this.loadOrdenes();

        } catch (error) {
            console.error('Error:', error);
            alert('Error al generar la factura');
        }
    }

    showModal() {
        document.getElementById('modalOrden').classList.remove('hidden');
    }

    closeModal() {
        document.getElementById('modalOrden').classList.add('hidden');
        this.ordenActual = null;
    }

    showConfirmModal(mensaje, onConfirm) {
        document.getElementById('mensajeConfirmacion').textContent = mensaje;
        document.getElementById('modalConfirmacion').classList.remove('hidden');
        
        const btnConfirmar = document.getElementById('btnConfirmar');
        btnConfirmar.onclick = onConfirm;
    }

    closeConfirmModal() {
        document.getElementById('modalConfirmacion').classList.add('hidden');
    }

    getEstadoText(estado) {
        const estados = {
            'Pendiente': 'Pendiente',
            'EnProgreso': 'En Progreso',
            'Finalizado': 'Finalizado'
        };
        return estados[estado] || estado;
    }

    getTipoServicioText(tipo) {
        const tipos = {
            'Preventivo': 'Mantenimiento Preventivo',
            'Reparacion': 'Reparación',
            'Diagnostico': 'Diagnóstico'
        };
        return tipos[tipo] || tipo;
    }
}

// Inicializar la aplicación
const app = new MecanicoApp();