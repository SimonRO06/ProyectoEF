# 🧰 AutoTallerManager – Sistema de Gestión de Taller Automotriz (Backend)

**AutoTallerManager** es un backend **RESTful** desarrollado en **ASP.NET Core** diseñado para centralizar y automatizar las operaciones clave de un taller automotriz moderno.  
Su objetivo es **optimizar los flujos de trabajo** de mecánicos, recepcionistas y administradores mediante la gestión eficiente de **clientes, vehículos, órdenes de servicio, repuestos y facturación**.

---

## 🚀 Tecnologías Principales

- **ASP.NET Core** – Framework principal para la API REST.
- **Entity Framework Core (Fluent API)** – Mapeo ORM y persistencia en **MySQL**.
- **AutoMapper** – Mapeo automático entre entidades y DTOs.
- **JWT (JSON Web Token)** – Autenticación y control de acceso basado en roles.
- **AspNetCoreRateLimit** – Limitación de velocidad (Rate Limiting) por endpoint.
- **Swagger / OpenAPI** – Documentación y pruebas interactivas de la API.

---

## 🧱 Arquitectura

El sistema sigue una arquitectura **hexagonal (Ports & Adapters)** compuesta por cuatro capas principales:

### 1️⃣ Capa de Dominio
Define las entidades esenciales del negocio y su lógica:

- **Cliente:** Propietario de vehículos.  
- **Vehículo:** Asociado a un cliente, con datos técnicos y de identificación.  
- **OrdenServicio:** Solicitud de trabajo con tipo de servicio, mecánico y fechas.  
- **Repuesto:** Piezas con código, descripción, stock y precio.  
- **DetalleOrden:** Relación entre orden y repuestos utilizados.  
- **Usuario:** Personas con roles (“Admin”, “Mecánico”, “Recepcionista”).  
- **Factura:** Documento de cobro al cierre de la orden.  

Incluye reglas de negocio, por ejemplo:
- Validar que un vehículo no esté agendado en dos órdenes simultáneas.  
- Calcular fechas de entrega según tipo de servicio.  
- Evitar uso de repuestos fuera de stock.

---

### 2️⃣ Capa de Aplicación
Orquesta los **casos de uso** mediante **DTOs** y servicios de aplicación.

Ejemplos de casos de uso:
- `RegistrarClienteConVehiculo`
- `CrearOrdenServicio`
- `ActualizarOrdenConTrabajoRealizado`
- `GenerarFactura`

Utiliza **AutoMapper** para reducir código repetitivo y proteger datos sensibles (como contraseñas).

---

### 3️⃣ Capa de Infraestructura
Gestiona la persistencia y comunicación con la base de datos:

- **Entity Framework Core** con **Fluent API**.  
- **DbContext:** `AutoTallerDbContext`.  
- **Repository Pattern genérico** (`GenericRepository<T>`).  
- **Unit of Work (`IUnitOfWork`)** para transacciones atómicas.  
- Soporte para **migraciones** (`InitialCreate`, `AddRepuestosTable`, etc.).  

Configuraciones clave:
- Relaciones uno a muchos entre entidades.
- Claves únicas (VIN, código de repuesto).
- Borrado controlado de registros dependientes.

---

### 4️⃣ Capa de API
Proporciona endpoints RESTful organizados por controlador:

| Controlador | Funcionalidad Principal |
|--------------|--------------------------|
| **ClientesController** | CRUD de clientes y paginación |
| **VehiculosController** | CRUD y filtrado por cliente o VIN |
| **OrdenesServicioController** | Creación, actualización, cierre y cancelación de órdenes |
| **RepuestosController** | Gestión de inventario |
| **FacturasController** | Generación y consulta de facturas |
| **UsuariosController** | Gestión de usuarios y roles |

---

## 🔐 Autenticación y Autorización

- **JWT (JSON Web Token)** para autenticación.
- Token incluye `UserId`, `Email` y `Role`.
- **Roles y permisos**:

| Rol | Permisos |
|-----|-----------|
| **Admin** | Control total (usuarios, repuestos, informes) |
| **Mecánico** | Actualizar órdenes y generar facturas |
| **Recepcionista** | Crear órdenes, registrar clientes y vehículos |

---

## ⚙️ Funcionalidades Clave

### 👥 Gestión de Clientes y Vehículos
- Registro de clientes con datos completos.  
- Asociación de múltiples vehículos por cliente.  
- Edición y eliminación controlada (sin órdenes activas).

### 🧾 Órdenes de Servicio
- Creación con validación de disponibilidad y stock.  
- Actualización de estado y registro de avances.  
- Listados paginados y filtrables por fecha, cliente, mecánico o estado.

### ⚙️ Control de Inventario
- Alta, edición y baja de repuestos.  
- Validación de stock antes de asignación.  
- Filtrado por categoría o nivel de stock.

### 💰 Facturación
- Cálculo automático de totales (mano de obra + repuestos).  
- Generación de facturas vinculadas a órdenes.  
- Consulta histórica por cliente o fecha.

### 📑 Paginación y Filtrado
- Parámetros: `pageNumber`, `pageSize`.  
- Encabezado de respuesta: `X-Total-Count`.  
- Filtros dinámicos por nombre, VIN, estado, etc.

### ⏱️ Limitación de Solicitudes (Rate Limiting)
- Límite: 60 req/min para `/api/ordenesservicio`.  
- Límite: 30 req/min para `/api/repuestos`.  
- Respuesta: **HTTP 429 (Too Many Requests)** al exceder.

### 🕵️ Auditoría
- Registro de operaciones en tabla `Auditorias`.  
- Campos: entidad, acción, usuario y fecha.  
- Consulta histórica de modificaciones y eliminaciones.

### 📄 Documentación Swagger
- Documentación generada automáticamente.  
- Endpoints probables mediante interfaz interactiva.  
- Ejemplo de autenticación con **Bearer Token**.

---

## 🧩 Migraciones y Configuración

    dotnet add Infrastructure package Microsoft.EntityFrameworkCore
    dotnet add Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL

    📦 Application (CQRS/Validación/Mapping)
    bash
    Copy code
    dotnet add Application package MediatR
    dotnet add Application package FluentValidation
    dotnet add Application package AutoMapper
    
    📦 Api (DI + Swagger + Validación)
    bash
    Copy code
    dotnet add Api package AutoMapper
    dotnet add Api package AutoMapper.Extensions.Microsoft.DependencyInjection
    dotnet add Api package FluentValidation.DependencyInjectionExtensions
    dotnet add Api package Microsoft.EntityFrameworkCore.Design

1. Levantar el contenedor: 

    docker compose up -d 

    - Crear la migración: 
    
    dotnet ef migrations add IniMig -p Infrastructure/ -s Api/ -o Data/Migrations


2. Configurar conexión a MySQL en appsettings.json:

    "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=proyectoef;Username=postgres;Password=postgres"
    }    FALTA ARREGLAR 

3.  Iniciar el proyecto:

    dotnet run

4.  Acceder a Swagger UI:

    http://localhost:5000/swagger/index.html


    👨🏻‍💻 Roles del Sistema 👩🏻‍💻

| Integrante | Rol / Responsabilidades |
|------------|---------------------------|
| 👨🏻‍💻​ **Simón Rubiano Ortiz** | Encargado del **backend**, gestión de la lógica del servidor y conexión con la base de datos. |
| 👩🏻‍💻​ **Juliana Andrea Pallares** | Participó en el desarrollo de la parte de **configurations**, asegurando la correcta integración técnica del proyecto. |
| 👩🏻‍💻​ **Ivanaa Patermina Mercado** | Desarrolló parte del **frontend**, contribuyendo al diseño visual y la estructura de la interfaz. |
| 👩🏻‍💻​ **Jhinet Daniela Pérez Tami** | Apoyé en el desarrollo de **interfaces**, manejo de **repositorios**, corrección de **errores** y realización de **pruebas en Insomnia** para validar las rutas del backend. |

---

