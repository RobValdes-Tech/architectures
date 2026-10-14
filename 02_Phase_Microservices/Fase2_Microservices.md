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


## 📬 Integración de Mensajería Asíncrona (MassTransit)
Se añadieron abstracciones del bus de datos en `OrderService.Net` para iniciar la coreografía de eventos distributed:

* **MassTransit & MassTransit.RabbitMQ:** Biblioteca encargada de gestionar de forma automática los exchanges y colas dentro del broker.
* **Shared/Events/OrderCreatedEvent.cs:** Contrato inmutable que emite el microservicio de órdenes tras persistir una compra en PostgreSQL.

### Estado de Avance del Ecosistema:
- [x] Broker de RabbitMQ aprovisionado e integrado en la infraestructura global.
- [x] Dependencias de MassTransit añadidas al proyecto de órdenes.
- [x] Definición del contrato inmutable del evento `OrderCreatedEvent`.
- [ ] Inyección de MassTransit en `Program.cs` y publicación del evento desde el controlador.

### Estado de Avance del Ecosistema:
- [x] Broker de RabbitMQ aprovisionado e integrado en la infraestructura global.
- [x] Dependencias de MassTransit añadidas al proyecto de órdenes.
- [x] Definición del contrato inmutable del evento `OrderCreatedEvent`.
- [x] Configuración de topología de RabbitMQ e inyección del Bus en `Program.cs`.
- [x] Publicación del evento distribuido desde el endpoint de `OrdersController`.

## 📬 Coreografía de Eventos (Consumidor Java)
Se implementaron los componentes de escucha en el microservicio de Java para interceptar las publicaciones del bus de datos de .NET:

* `product/dto/OrderCreatedEvent.java`: Record homólogo encargado de acoplar el esquema JSON enviado por MassTransit.
* `product/consumer/OrderCreatedConsumer.java`: Componente `@RabbitListener` configurado para procesar los mensajes asíncronos y preparar el ajuste de inventario.

### Estado de Avance del Ecosistema:
- [x] Contrato inmutable del evento y publicador configurados en .NET 8.
- [x] Dependencia Spring AMQP integrada en el módulo de Java.
- [x] Creación del DTO de eventos y clase consumidora (`OrderCreatedConsumer`) en Java.
- [ ] Lógica de decremento de stock persistida en MongoDB al recibir el mensaje.

## 📬 Coreografía de Eventos (Consumidor Java)
* `product/config/RabbitMQConfig.java`: Configuración del convertidor de mensajes a formato JSON (Jackson) para permitir la interoperabilidad políglota con los eventos emitidos por .NET.

### Estado de Avance del Ecosistema:
- [x] Contrato inmutable del evento y publicador configurados en .NET 8.
- [x] Configuración de credenciales y Jackson2JsonMessageConverter en Java.
- [ ] Implementación de la lógica transaccional para decrementar stock en MongoDB.


## 📬 Coreografía de Eventos (Consumidor Java)
* `product/consumer/OrderCreatedConsumer.java`: Modificado para consumir el payload JSON interoperable. Implementa la lógica transaccional que extrae el documento desde MongoDB, valida la disponibilidad de existencias y guarda el decremento real de stock.

### Estado de Avance del Ecosistema:
- [x] Contrato inmutable del evento y publicador configurados en .NET 8.
- [x] Configuración de credenciales y Jackson2JsonMessageConverter en Java.
- [x] Implementación de la lógica transaccional para decrementar stock en MongoDB.
- [ ] Configuración del servicio de caché distribuido (Redis) para el Carrito de compras.

## 📁 Arquitectura Estructural del Módulo Node.js
Se incorporó un entorno asíncrono basado en eventos de alta velocidad para la gestión de datos efímeros en el servidor:
* `CartService.Node/package.json`: Manifiesto de dependencias y scripts de arranque para el microservicio de carrito.

## 🐳 Orquestación de Infraestructura Local
* **Redis (Puerto 6379):** Almacenamiento clave-valor en memoria de baja latencia utilizado para las sesiones del carrito de compras (`cart-cache`).

### Estado de Avance del Ecosistema:
- [x] Sincronización políglota RabbitMQ completada entre .NET 8 y Java.
- [x] Contenedor de Redis adicionado al docker-compose global.
- [x] Proyecto base CartService.Node inicializado mediante NPM CLI.
- [ ] Implementación de lógica de conexión a Redis e interfaces Express para el carrito.

### Error: Mapeo de Payload Políglota (Propiedades null de MassTransit a Java)
* **Causa:** MassTransit envuelve de forma nativa los payloads de los eventos dentro de un nodo raíz llamado `message` en formato camelCase. Spring AMQP lee el JSON de forma plana, interpretando los atributos de dominio como nulos.
* **Solución:** Implementar un constructor con la anotación `@JsonProperty("message")` en el Record de Java para extraer y tipar de forma manual las claves distribuidas.

