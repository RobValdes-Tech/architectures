# Bitácora Técnica - Fase 2: Introducción a Microservicios
**Ecosistema:** Java 17 (Spring Boot) & MongoDB

Este documento detalla el diseño e implementación del primer bloque distribuido de la plataforma: el catálogo federado de productos.

## 🏛️ Decisiones de Diseño Arquitectónico
1. **Desacoplamiento de Dominios:** El catálogo de productos se separa por completo del monolito .NET, gestionando su propio ciclo de vida e infraestructura.
2. **Persistencia Políglota:** Se selecciona MongoDB como base de datos documental. Esto proporciona alta velocidad de lectura y un esquema flexible idóneo para catálogos de artículos.
3. **Paquetes Genéricos:** Se adopta la estructura limpia `com.store.product` para los componentes lógicos, manteniendo un estándar desacoplado e independiente de marcas organizacionales.

## 📁 Arquitectura Estructural del Módulo Java
El proyecto sigue el estándar de construcción de Maven:
* `pom.xml`: Manejo de dependencias (Web starter y Data MongoDB).
* `application.properties`: Configuración de puertos de red (`8081`) y credenciales de conexión al clúster de base de datos.
* `model/Product.java`: Mapeo documental de la entidad para el motor de persistencia.
* `repository/ProductRepository.java`: Capa de abstracción de datos utilizando Spring Data Mongo heredando de `MongoRepository`.
* `controller/ProductController.java`: Controlador REST que expone los endpoints GET y POST para interactuar con el catálogo de productos.
* `ProductServiceApplication.java`: Clase principal y punto de entrada para inicializar el contexto de Spring Boot.

## 🚀 Estado de Avance del Ecosistema
- [x] Árbol físico de directorios inicializado en disco duro.
- [x] Configuración de persistencia e inyección de dependencias (`ProductRepository`) completadas.
- [x] Habilitación de la clase de arranque nativa (`ProductServiceApplication`).
- [x] Implementación de interfaces REST (`ProductController`) para exponer las operaciones del catálogo.

## 📁 Arquitectura Estructural del Módulo .NET 8
El servicio de gestión de transacciones de compra se inicializó de forma aislada:
* `OrderArchitecture.sln`: Solución contenedora para el entorno de órdenes.
* `OrderService.NET/`: Proyecto Web API estructurado bajo controladores tradicionales para exponer las operaciones lógicas de compra de forma federada.

### Estado de Avance del Ecosistema:
- [x] Catálogo de Productos en Java Spring Boot estructurado y listo.
- [x] Orquestador `docker-compose.yml` global configurado con MongoDB.
- [x] Estructura base de `OrderService` en .NET 8 inicializada mediante .NET CLI.
- [ ] Configuración del bus de mensajes (RabbitMQ) y persistencia en PostgreSQL.


## 📁 Arquitectura Estructural del Módulo .NET 8
El servicio mapeará las transacciones hacia almacenamiento relacional utilizando la estructura de persistencia que incluye `Models/Order.cs`, `Data/OrderDbContext.cs` y el directorio `Migrations/`.

### Estado de Avance del Ecosistema:
- [x] Contenedor de PostgreSQL aprovisionado y paquetes EF Core configurados.
- [x] Generación del andamiaje de la migración inicial (`InitialCreate`).
- [ ] Aplicación en vivo de las tablas relacionales dentro del contenedor persistente.


## 🔧 Resolución de Errores Comunes (Troubleshooting)

### Error 28P01: password authentication failed for user "postgres"
* **Causa:** Desconexión entre la credencial del archivo de configuración del API (`appsettings.json`) y las variables de entorno del contenedor Docker.
* **Solución:** Sincronizar el campo `POSTGRES_PASSWORD` del archivo YAML con el parámetro `Password=` de la cadena de conexión de .NET. En caso de persistencia persistida inválida, reconstruir el contenedor usando `docker-compose down -v`.


## 📁 Arquitectura Estructural del Módulo .NET 8
El servicio mapea las transacciones hacia almacenamiento relacional utilizando la siguiente estructura:
* `Models/Order.cs`: Entidad que representa una orden de compra en el sistema.
* `Data/OrderDbContext.cs`: Contexto que gestiona las conexiones físicas con el motor transaccional.
* `Controllers/OrdersController.cs`: Controlador REST que expone los endpoints GET y POST para la administración de órdenes.

### Estado de Avance del Ecosistema:
- [x] Contenedor de PostgreSQL aprovisionado y base de datos inicializada de forma exitosa.
- [x] Aplicación en vivo de las tablas relacionales dentro del contenedor persistente.
- [x] Implementación de interfaces REST (`OrdersController`) para exponer las operaciones de compra.



## 🐳 Orquestación de Infraestructura Local
Actualiza tu `docker-compose.yml` para incluir **MongoDB** (catalogo, puerto 27017), **PostgreSQL** (`OrderService.NET`, puerto 5432) y **RabbitMQ** (mensajería asíncrona AMQP, puertos 5672/15672 con credenciales predeterminadas `guest / guest`) en la bitácora técnica de la fase.

## 🐳 Orquestación de Infraestructura Local
Se integró el broker de mensajería asíncrona al archivo `docker-compose.yml` de la Fase 2.

### Servicios Habilitados:
* **MongoDB (Puerto 27017):** Base de datos documental para el catálogo de productos (`product-service`).
* **PostgreSQL (Puerto 5432):** Base de datos relacional para la gestión de transacciones (`OrderService.NET`).
* **RabbitMQ (Puertos 5672 / 15672):** Gestor de colas y eventos asíncronos distribuidos (Consola de administración: `http://localhost:15672`).
