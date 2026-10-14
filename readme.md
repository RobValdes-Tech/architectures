# Runbook de Arquitecturas Empresariales: Monolito a Microservices & Serverless
**Organización:** Appxolot Training
**Arquitecto Líder:** Roberto Azael Valdes Rodriguez

Este repositorio es una guía práctica paso a paso diseñada para desarrolladores Junior, mostrando la transición y coexistencia de sistemas utilizando entornos reales multitecnología.

## 🚀 Hoja de Ruta del Proyecto
- [ ] **Fase 1: Arquitectura Monolítica** -> Desarrollo unificado e inyección de dependencias local con ASP.NET Core Web API.
- [ ] **Fase 2: Arquitectura de Microservicios** -> Separación de dominios federados (.NET 8 + Java Spring Boot), bases de datos dedicadas por servicio y mensajería asíncrona con RabbitMQ.
- [ ] **Fase 3: Arquitectura Serverless** -> Procesamiento asíncrono optimizado y cómputo guiado por eventos bajo demanda.

## 🛠️ Tecnologías del Ecosistema Integrado
* **Backend:** .NET 8 (C#) & Spring Boot (Java)
* **Bases de Datos:** PostgreSQL, MongoDB & Redis Cache
* **Mensajería:** RabbitMQ (MassTransit / Spring AMQP)
* **Contenedores:** Docker & Docker Compose

## 🚀 Hoja de Ruta del Proyecto
- [x] **Fase 1: Arquitectura Monolítica** -> Estructura base inicializada mediante .NET CLI.
- [ ] **Fase 2: Arquitectura de Microservicios** -> Separación de dominios federados (.NET 8 + Java Spring Boot), bases de datos dedicadas por servicio y mensajería asíncrona con RabbitMQ.
- [ ] **Fase 3: Arquitectura Serverless** -> Procesamiento asíncrono optimizado y cómputo guiado por eventos bajo demanda.

## 🚀 Hoja de Ruta del Proyecto
- [x] **Fase 1: Arquitectura Monolítica** -> Desarrollo unificado e inyección de dependencias local con ASP.NET Core Web API.
- [/] **Fase 2: Arquitectura de Microservicios** -> Inicialización del ecosistema multitecnología. Microservicio de Productos en Java (Spring Boot + MongoDB) estructurado e infraestructura base lista.
- [ ] **Fase 3: Arquitectura Serverless** -> Procesamiento asíncrono optimizado y cómputo guiado por eventos bajo demanda.


## 🐳 Orquestación de Infraestructura Local
Se actualizó el archivo `docker-compose.yml` para centralizar la administración de contenedores del entorno distribuido.

### Servicios Habilitados:
* **MongoDB (Puerto 27017):** Motor documental persistente utilizado por el microservicio de catálogo (`product-service`).
* **PostgreSQL (Puerto 5432):** Motor relacional persistente utilizado por el microservicio de transacciones (`OrderService.NET`).

### Credenciales de Conexión Base de Datos (.NET):
* **Host:** localhost:5432
* **Database:** OrdersDb
* **User:** postgres
* **Password:** SecretPassword123!


# Bitácora Técnica - Fase 2: Introducción a Microservicios
**Ecosistema:** Java 17 (Spring Boot) & .NET 8 (C#)

## 🔧 Gestión de Dependencias en .NET 8 (NuGet)
Se agregaron las herramientas de persistencia ORM para la comunicación de `OrderService.NET` con la base de datos relacional:
* **Microsoft.EntityFrameworkCore:** Núcleo del ORM para el mapeo objeto-relacional.
* **Npgsql.EntityFrameworkCore.PostgreSQL:** Proveedor de datos para traducir las consultas de LINQ a PostgreSQL.
* **Microsoft.EntityFrameworkCore.Design:** Soporte en tiempo de diseño requerido para la generación y ejecución de migraciones desde la CLI.

## 📁 Arquitectura Estructural del Módulo .NET 8
El servicio mapeará las transacciones hacia almacenamiento relacional utilizando la siguiente estructura de persistencia:
* `Models/Order.java`: Entidad de dominio que representa una transacción de compra.
* `Data/OrderDbContext.java`: Contexto de Entity Framework Core encargado de gestionar las conexiones y el mapeo físico de tablas.

### Estado de Avance del Ecosistema:
- [x] Contenedor de PostgreSQL aprovisionado en docker-compose global.
- [x] Paquetes de Entity Framework Core configurados en `OrderService.NET`.
- [x] Creación de la entidad de dominio `Order` y el contexto de datos `OrderDbContext`.

