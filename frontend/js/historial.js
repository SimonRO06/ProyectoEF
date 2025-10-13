class HistorialApp {
    constructor() {
        this.ordenes = [];
        this.ordenActual = null;
        this.token = localStorage.getItem('token');
        this.userId = localStorage.getItem('userId');
        this.userName = localStorage.getItem('userName');
        this.paginaActual = 1;
        this.totalPaginas = 1;
        this.resultadosPorPagina = 10;

        this.init();
    }

    init() {
        this.checkAuth();
        this.loadUserInfo();
        this.setupEventListeners();
        this.loadHistorial();
    }

    checkAuth() {
        if (!this.token) {
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
        document.getElementById('btnBuscar').addEventListener('click', () => this.filtrarHistorial());
        document.getElementById('filtroCliente').addEventListener('input', () => this.filtrarHistorial());
        document.getElementById('filtroEstado').addEventListener('change', () => this.filtrarHistorial());
        document.getElementById('filtroMes').addEventListener('change', () => this.filtrarHistorial());
        document.getElementById('filtroAnio').addEventListener('change', () => this.filtrarHistorial());

        // Exportar
        document.getElementById('btnExportar').addEventListener('click', () => this.exportarHistorial());

        // Paginación
        document.getElementById('btnPrev').addEventListener('click', () => this.paginaAnterior());
        document.getElementById('btnNext').addEventListener('click', () => this.paginaSiguiente());

        // Modal
        document.getElementById('btnCerrarDetalles').addEventListener('click', () => this.cerrarModal());
        document.getElementById('btnImprimir').addEventListener('click', () => this.imprimirDetalles());
    }

    async loadHistorial() {
        try {
            // Cargar órdenes finalizadas y canceladas
            const response = await fetch('/api/ordenesservicio/historial', {
                headers: {
                    'Authorization': `Bearer ${this.token}`
                }
            });

            if (!response.ok) throw new Error('Error al cargar historial');

            this.ordenes = await response.json();
            this.filtrarHistorial();
            this.calcularEstadisticas();

        } catch (error) {
            console.error('Error:', error);
            alert('Error al cargar el historial');
        }
    }

    filtrarHistorial() {
        const estadoFilter = document.getElementById('filtroEstado').value;
        const mesFilter = document.getElementById('filtroMes').value;
        const anioFilter = document.getElementById('filtroAnio').value;
        const clienteFilter = document.getElementById('filtroCliente').value.toLowerCase();

        let filtered = this.ordenes.filter(orden => {
            const fechaFinalizacion = orden.fechaFinalizacion ? new Date(orden.fechaFinalizacion) : new Date(orden.fechaActualizacion);
            
            const matchesEstado = !estadoFilter || orden.estado === estadoFilter;
            const matchesMes = !mesFilter || (fechaFinalizacion.getMonth() + 1) === parseInt(mesFilter);
            const matchesAnio = !anioFilter || fechaFinalizacion.getFullYear() === parseInt(anioFilter);
            const matchesCliente = !clienteFilter || 
                (orden.vehiculo?.cliente?.nombre?.toLowerCase().includes(clienteFilter));
            
            return matchesEstado && matchesMes && matchesAnio && matchesCliente;
        });

        // Ordenar por fecha de finalización descendente
        filtered.sort((a, b) => {
            const fechaA = a.fechaFinalizacion ? new Date(a.fechaFinalizacion) : new Date(a.fechaActualizacion);
            const fechaB = b.fechaFinalizacion ? new Date(b.fechaFinalizacion) : new Date(b.fechaActualizacion);
            return fechaB - fechaA;
        });

        this.mostrarHistorialPaginado(filtered);
    }

    mostrarHistorialPaginado(ordenesFiltradas) {
        this.totalPaginas = Math.ceil(ordenesFiltradas.length / this.resultadosPorPagina);
        const inicio = (this.paginaActual - 1) * this.resultadosPorPagina;
        const fin = inicio + this.resultadosPorPagina;
        const ordenesPagina = ordenesFiltradas.slice(inicio, fin);

        this.renderHistorial(ordenesPagina);
        this.actualizarPaginacion(ordenesFiltradas.length);
    }

    renderHistorial(ordenes) {
        const tbody = document.getElementById('historialBody');
        
        tbody.innerHTML = ordenes.map(orden => {
            const fechaIngreso = new Date(orden.fechaIngreso);
            const fechaFinalizacion = orden.fechaFinalizacion ? new Date(orden.fechaFinalizacion) : new Date(orden.fechaActualizacion);
            const duracion = this.calcularDuracion(fechaIngreso, fechaFinalizacion);
            const totalFactura = orden.facturas && orden.facturas.length > 0 ? orden.facturas[0].total : 0;

            return `
                <tr>
                    <td>${orden.id.substring(0, 8)}</td>
                    <td>${orden.vehiculo?.cliente?.nombre || 'N/A'}</td>
                    <td>${orden.vehiculo?.marca || ''} ${orden.vehiculo?.modelo || ''}</td>
                    <td>${this.getTipoServicioText(orden.tipoServicio)}</td>
                    <td>${fechaIngreso.toLocaleDateString()}</td>
                    <td>${fechaFinalizacion.toLocaleDateString()}</td>
                    <td>${duracion}</td>
                    <td>
                        <span class="estado-badge estado-${orden.estado.toLowerCase()}">
                            ${this.getEstadoText(orden.estado)}
                        </span>
                    </td>
                    <td>$${totalFactura.toFixed(2)}</td>
                    <td class="acciones-cell">
                        <button class="btn-accion btn-detalles" onclick="historialApp.verDetallesCompletos('${orden.id}')">
                            <i class="fas fa-eye"></i> Ver
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    }

    calcularDuracion(fechaInicio, fechaFin) {
        const diffTime = Math.abs(fechaFin - fechaInicio);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        
        if (diffDays === 1) return '1 día';
        if (diffDays < 7) return `${diffDays} días`;
        if (diffDays < 30) return `${Math.floor(diffDays / 7)} sem`;
        
        const meses = Math.floor(diffDays / 30);
        const diasRestantes = diffDays % 30;
        
        if (diasRestantes === 0) return `${meses} mes${meses > 1 ? 'es' : ''}`;
        return `${meses} mes${meses > 1 ? 'es' : ''} ${diasRestantes} días`;
    }

    calcularEstadisticas() {
        const ordenesFinalizadas = this.ordenes.filter(o => o.estado === 'Finalizado');
        const totalFacturado = ordenesFinalizadas.reduce((total, orden) => {
            const factura = orden.facturas && orden.facturas[0];
            return total + (factura ? factura.total : 0);
        }, 0);

        // Calcular promedio de tiempo
        let totalDias = 0;
        let countConDuracion = 0;

        ordenesFinalizadas.forEach(orden => {
            const fechaInicio = new Date(orden.fechaIngreso);
            const fechaFin = orden.fechaFinalizacion ? new Date(orden.fechaFinalizacion) : new Date(orden.fechaActualizacion);
            const dias = Math.ceil((fechaFin - fechaInicio) / (1000 * 60 * 60 * 24));
            
            if (dias > 0) {
                totalDias += dias;
                countConDuracion++;
            }
        });

        const promedioDias = countConDuracion > 0 ? Math.round(totalDias / countConDuracion) : 0;

        // Actualizar estadísticas
        document.getElementById('countTotal').textContent = this.ordenes.length;
        document.getElementById('countFinalizado').textContent = ordenesFinalizadas.length;
        document.getElementById('totalIngresos').textContent = `$${totalFacturado.toFixed(2)}`;
        document.getElementById('promedioTiempo').textContent = `${promedioDias}d`;
    }

    actualizarPaginacion(totalResultados) {
        document.getElementById('pageInfo').textContent = 
            `Página ${this.paginaActual} de ${this.totalPaginas} (${totalResultados} resultados)`;
        
        document.getElementById('btnPrev').disabled = this.paginaActual === 1;
        document.getElementById('btnNext').disabled = this.paginaActual === this.totalPaginas;
    }

    paginaAnterior() {
        if (this.paginaActual > 1) {
            this.paginaActual--;
            this.filtrarHistorial();
        }
    }

    paginaSiguiente() {
        if (this.paginaActual < this.totalPaginas) {
            this.paginaActual++;
            this.filtrarHistorial();
        }
    }

    async verDetallesCompletos(ordenId) {
        try {
            const response = await fetch(`/api/ordenesservicio/${ordenId}`, {
                headers: {
                    'Authorization': `Bearer ${this.token}`
                }
            });

            if (!response.ok) throw new Error('Error al cargar detalles');

            this.ordenActual = await response.json();
            this.mostrarDetallesCompletos();

        } catch (error) {
            console.error('Error:', error);
            alert('Error al cargar los detalles de la orden');
        }
    }

    mostrarDetallesCompletos() {
        if (!this.ordenActual) return;

        const orden = this.ordenActual;
        const fechaIngreso = new Date(orden.fechaIngreso);
        const fechaFinalizacion = orden.fechaFinalizacion ? new Date(orden.fechaFinalizacion) : new Date(orden.fechaActualizacion);
        const duracion = this.calcularDuracion(fechaIngreso, fechaFinalizacion);
        const factura = orden.facturas && orden.facturas[0];

        // Información básica
        document.getElementById('detalleNumero').textContent = orden.id.substring(0, 8);
        document.getElementById('detalleCliente').textContent = orden.vehiculo?.cliente?.nombre || 'N/A';
        document.getElementById('detalleTelefono').textContent = orden.vehiculo?.cliente?.telefono || 'N/A';
        document.getElementById('detalleVehiculo').textContent = 
            `${orden.vehiculo?.marca || ''} ${orden.vehiculo?.modelo || ''} - ${orden.vehiculo?.numeroSerie || ''}`;
        document.getElementById('detalleKilometraje').textContent = orden.vehiculo?.kilometraje ? `${orden.vehiculo.kilometraje} km` : 'N/A';

        // Información del servicio
        document.getElementById('detalleTipoServicio').textContent = this.getTipoServicioText(orden.tipoServicio);
        document.getElementById('detalleFechaIngreso').textContent = fechaIngreso.toLocaleDateString();
        document.getElementById('detalleFechaFinalizacion').textContent = fechaFinalizacion.toLocaleDateString();
        document.getElementById('detalleDuracion').textContent = duracion;
        document.getElementById('detalleEstado').textContent = this.getEstadoText(orden.estado);

        // Notas del trabajo
        document.getElementById('detalleNotas').innerHTML = orden.notasTrabajo ? 
            `<p>${orden.notasTrabajo.replace(/\n/g, '<br>')}</p>` : 
            '<p style="color: #6b7280; text-align: center;">No hay notas registradas</p>';

        // Repuestos
        this.mostrarRepuestosDetallados();

        // Factura
        if (factura) {
            document.getElementById('detalleFacturaNumero').textContent = factura.id.substring(0, 8);
            document.getElementById('detalleFacturaFecha').textContent = new Date(factura.fechaEmision).toLocaleDateString();
            document.getElementById('detalleFacturaTotal').textContent = `$${factura.total.toFixed(2)}`;
            document.getElementById('detalleEstadoPago').textContent = factura.pagos && factura.pagos.length > 0 ? 'Pagado' : 'Pendiente';
        } else {
            document.getElementById('detalleFacturaNumero').textContent = 'No generada';
            document.getElementById('detalleFacturaFecha').textContent = 'N/A';
            document.getElementById('detalleFacturaTotal').textContent = '$0.00';
            document.getElementById('detalleEstadoPago').textContent = 'N/A';
        }

        document.getElementById('modalDetallesCompletos').classList.remove('hidden');
    }

    mostrarRepuestosDetallados() {
        const container = document.getElementById('detalleRepuestos');
        
        if (this.ordenActual.detallesOrdenes && this.ordenActual.detallesOrdenes.length > 0) {
            let totalRepuestos = 0;
            
            const html = this.ordenActual.detallesOrdenes.map(detalle => {
                const subtotal = detalle.cantidad * detalle.costoUnitario;
                totalRepuestos += subtotal;

                return `
                    <div class="repuesto-item-detalle">
                        <div class="repuesto-info">
                            <div class="repuesto-codigo">${detalle.repuesto?.codigo || 'N/A'}</div>
                            <div class="repuesto-desc">${detalle.repuesto?.descripcion || 'Sin descripción'}</div>
                        </div>
                        <div class="repuesto-detalles">
                            <span class="repuesto-cantidad">${detalle.cantidad} x $${detalle.costoUnitario.toFixed(2)}</span>
                            <span class="repuesto-subtotal">$${subtotal.toFixed(2)}</span>
                        </div>
                    </div>
                `;
            }).join('');

            container.innerHTML = html + `
                <div class="repuesto-total">
                    <strong>Total en repuestos: $${totalRepuestos.toFixed(2)}</strong>
                </div>
            `;
        } else {
            container.innerHTML = '<p style="text-align: center; color: #6b7280;">No se utilizaron repuestos</p>';
        }
    }

    exportarHistorial() {
        // Simular exportación a CSV
        const filtros = this.obtenerFiltrosActuales();
        const fechaExportacion = new Date().toLocaleDateString();
        
        let csvContent = "data:text/csv;charset=utf-8,";
        csvContent += "Historial de Órdenes - AutoTallerManager\n";
        csvContent += `Exportado: ${fechaExportacion}\n`;
        csvContent += `Filtros: ${filtros}\n\n`;
        
        csvContent += "Orden,Cliente,Vehículo,Tipo Servicio,Fecha Ingreso,Fecha Finalización,Duración,Estado,Total\n";
        
        this.ordenes.forEach(orden => {
            const fechaIngreso = new Date(orden.fechaIngreso).toLocaleDateString();
            const fechaFinalizacion = orden.fechaFinalizacion ? 
                new Date(orden.fechaFinalizacion).toLocaleDateString() : 
                new Date(orden.fechaActualizacion).toLocaleDateString();
            const total = orden.facturas && orden.facturas[0] ? orden.facturas[0].total : 0;
            
            const row = [
                orden.id.substring(0, 8),
                `"${orden.vehiculo?.cliente?.nombre || 'N/A'}"`,
                `"${orden.vehiculo?.marca || ''} ${orden.vehiculo?.modelo || ''}"`,
                this.getTipoServicioText(orden.tipoServicio),
                fechaIngreso,
                fechaFinalizacion,
                this.calcularDuracion(new Date(orden.fechaIngreso), new Date(fechaFinalizacion)),
                this.getEstadoText(orden.estado),
                `$${total.toFixed(2)}`
            ].join(',');
            
            csvContent += row + "\n";
        });

        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", `historial_ordenes_${fechaExportacion.replace(/\//g, '-')}.csv`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    obtenerFiltrosActuales() {
        const estado = document.getElementById('filtroEstado').value || 'Todos';
        const mes = document.getElementById('filtroMes').value || 'Todos';
        const anio = document.getElementById('filtroAnio').value || 'Todos';
        const cliente = document.getElementById('filtroCliente').value || 'Todos';
        
        return `Estado: ${estado}, Mes: ${mes}, Año: ${anio}, Cliente: ${cliente}`;
    }

    imprimirDetalles() {
        window.print();
    }

    cerrarModal() {
        document.getElementById('modalDetallesCompletos').classList.add('hidden');
        this.ordenActual = null;
    }

    getEstadoText(estado) {
        const estados = {
            'Pendiente': 'Pendiente',
            'EnProgreso': 'En Progreso',
            'Finalizado': 'Finalizado',
            'Cancelado': 'Cancelado'
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

// Agregar estos estilos al archivo mecanico.css
const estilosAdicionales = `
    .btn-exportar {
        background-color: #8b5cf6;
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 6px;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 8px;
        transition: background-color 0.3s ease;
    }

    .btn-exportar:hover {
        background-color: #7c3aed;
    }

    .btn-detalles {
        background-color: #6b7280;
        color: white;
    }

    .btn-detalles:hover {
        background-color: #4b5563;
    }

    .btn-imprimir {
        background-color: #059669;
        color: white;
        border: none;
        padding: 10px 16px;
        border-radius: 6px;
        cursor: pointer;
        font-weight: 500;
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .btn-imprimir:hover {
        background-color: #047857;
    }

    .stat-icon.total {
        background: #e0e7ff;
        color: #3730a3;
    }

    .stat-icon.ingresos {
        background: #dcfce7;
        color: #166534;
    }

    .stat-icon.promedio {
        background: #fef7cd;
        color: #854d0e;
    }

    .pagination {
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 20px;
        margin-top: 20px;
        padding: 15px;
    }

    .btn-pagination {
        background-color: #3b82f6;
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 6px;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .btn-pagination:disabled {
        background-color: #9ca3af;
        cursor: not-allowed;
    }

    .btn-pagination:hover:not(:disabled) {
        background-color: #2563eb;
    }

    .notas-section {
        margin: 20px 0;
    }

    .notas-content {
        background: #f8fafc;
        padding: 15px;
        border-radius: 8px;
        border: 1px solid #e2e8f0;
        min-height: 60px;
    }

    .repuesto-item-detalle {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 10px 0;
        border-bottom: 1px solid #f1f5f9;
    }

    .repuesto-detalles {
        display: flex;
        gap: 15px;
        align-items: center;
    }

    .repuesto-subtotal {
        font-weight: 600;
        color: #059669;
        min-width: 80px;
        text-align: right;
    }

    .repuesto-total {
        text-align: right;
        padding: 15px 0;
        border-top: 2px solid #e2e8f0;
        margin-top: 10px;
        color: #111827;
    }

    .factura-section {
        background: #f0f9ff;
        padding: 15px;
        border-radius: 8px;
        margin-top: 20px;
    }
`;

// Agregar estilos al CSS existente
const styleSheet = document.createElement('style');
styleSheet.textContent = estilosAdicionales;
document.head.appendChild(styleSheet);

// Inicializar la aplicación de historial
const historialApp = new HistorialApp();