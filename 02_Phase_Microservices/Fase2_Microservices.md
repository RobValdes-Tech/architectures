# Bitácora Técnica - Fase 2: Arquitectura de Microservicios Políglota
**Ecosistema:** Java 17 (Spring Boot) & .NET 8 (C#) & Node.js (JavaScript)

Este documento detalla el runbook de diseño, orquestación de infraestructura local y la coreografía de eventos asíncronos para el sistema distribuido de la tienda.

## 🏛️ Decisiones de Diseño Arquitectónico
1. **Desacoplamiento de Dominios:** Se rompe el monolito federando las capacidades en tres servicios autónomos con ciclos de vida independientes (`product-service`, `OrderService.Net` y `CartService.Node`).
2. **Persistencia Políglota Dedicada:** Cada microservicio administra su propio motor de base de datos para garantizar el aislamiento absoluto del almacenamiento:
   * **MongoDB:** Almacenamiento NoSQL documental de alta lectura para el catálogo de productos.
   * **PostgreSQL:** Almacenamiento relacional transaccional para la gestión de órdenes de compra.
   * **Redis Cache:** Almacenamiento clave-valor en memoria de ultra baja latencia para los carritos temporales.
3. **Coreografía de Eventos Asíncronos:** La comunicación inter-servicio se realiza sin acoplamiento HTTP directo, delegando el transporte de mensajes al broker de **RabbitMQ** mediante el protocolo AMQP.

## 📁 Arquitectura Estructural del Ecosistema

### 1. Catálogo de Productos (Java 17 + Spring Boot + MongoDB)
* `pom.xml`: Gestión de dependencias integrando `spring-boot-starter-data-mongodb` y `spring-boot-starter-amqp`.
* `src/main/resources/application.properties`: Configuración del puerto de escucha `8081` y credenciales de acceso al clúster de Mongo.
* `product/model/Product.java`: Entidad para el mapeo documental en las colecciones de MongoDB.
* `product/repository/ProductRepository.java`: Abstracción CRUD que hereda de `MongoRepository`.
* `product/config/RabbitMQConfig.java`: Configuración del `Jackson2JsonMessageConverter` y declaración de los Beans para el autoprovisionamiento de colas.
* `product/dto/OrderCreatedEvent.java`: Record inmutable simplificado para la deserialización del payload asíncrono.
* `product/consumer/OrderCreatedConsumer.java`: Oyente `@RabbitListener` que intercepta las compras y decrementa las existencias en MongoDB.

### 2. Gestión de Órdenes (.NET 8 + PostgreSQL + MassTransit)
* `OrderService.Net.csproj`: Adición de paquetes NuGet de Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`) y MassTransit (`MassTransit.RabbitMQ`).
* `appsettings.json`: Registro de la cadena de conexión relacional apuntando al contenedor local.
* `Program.cs`: Inyección de dependencias del `OrderDbContext` y configuración de la topología AMQP con `cfg.UseRawJsonSerializer()`.
* `Models/Order.cs`: Clase que modela la tabla física de transacciones.
* `Shared/Events/OrderCreatedEvent.cs`: Record inmutable que define el contrato global del evento de negocio.
* `Controllers/OrdersController.cs`: Controlador REST que expone el puerto `5291`, guarda la orden en base de datos y publica el evento en el bus.

### 3. Carrito de Compras (Node.js + Redis Cache)
* `package.json`: Definición de dependencias ligeras Express, Redis y cliente AMQP (`amqplib`).
* `server.js`: Servidor web en el puerto `8082` que expone endpoints para gestionar el caché y levanta un consumidor asíncrono en segundo plano para limpiar las sesiones de los usuarios al concretarse un checkout.

## 🐳 Orquestación de Infraestructura Local (`docker-compose.yml`)
Se centralizó la administración del entorno distribuido en un único manifiesto YAML que aprovisiona los cuatro contenedores interconectados bajo un puente de red común expuesto hacia el sistema operativo:
* **products-mongodb (Puerto 27017):** Inicializado con credenciales explícitas en base de datos.
* **orders-postgres (Puerto 5432):** Motor relacional persistente con volumen local asignado.
* **cart-redis (Puerto 6379):** Memoria en caché de baja latencia.
* **global-rabbitmq (Puertos 5672 / 15672):** Broker AMQP y panel web de administración (`guest/guest`).

## 🔧 Resolución de Errores Comunes (Troubleshooting)

### Error: Mapeo de Payload Políglota (Propiedades null de MassTransit a Java)
* **Causa:** MassTransit envuelve de forma nativa los payloads de los eventos dentro de un nodo raíz llamado `message` en formato camelCase. Spring AMQP lee el JSON de forma plana, interpretando los atributos de dominio como nulos.
* **Solución:** Configurar en .NET la directiva `cfg.UseRawJsonSerializer()` para forzar el envío de JSON puro y simplificar el Record de Java con la anotación `@JsonIgnoreProperties(ignoreUnknown = true)` para lograr un desacoplamiento limpio de la mensajería.

### Error 13 (Unauthorized): Command update requires authentication on MongoDB
* **Causa:** El contenedor de MongoDB restringe las operaciones de escritura anónimas por políticas de seguridad locales.
* **Solución:** Definir variables de entorno de inicialización de usuarios raíz (`MONGO_INITDB_ROOT_USERNAME`) en el archivo docker-compose y actualizar la URI de conexión de Spring Boot incluyendo las credenciales con el parámetro `?authSource=admin`.

### Error: Bind for 0.0.0.0:<PORT> failed: port is already allocated
* **Causa:** Procesos o contenedores huérfanos se quedaron acaparando las interfaces de red del host local de Windows 10 (típico con puertos 27017 o 6379).
* **Solución:** Ejecutar consultas de rastreo `Get-NetTCPConnection -LocalPort <PORT>` en PowerShell y forzar la finalización de las tareas colgadas usando `Stop-Process -Id <PID> -Force` para liberar el puerto antes de lanzar el clúster.

## 🧪 Evidencia de Pruebas de Integración y Coreografía Políglota
Se validó con éxito el flujo de mensajería asíncrona interoperable en tiempo real:
1. El catálogo en Java Spring Boot persistió el producto inicial (`PROD-100`) con un stock base de 50 unidades en **MongoDB** bajo el esquema unificado de credenciales de forma exitosa (HTTP 201).
2. El API de .NET 8 (`OrdersController`) procesó una compra simulada en el puerto `5291`, registrando la transacción en **PostgreSQL** (HTTP 201).
3. El Bus de datos de **MassTransit** transmitió el evento de forma plana a **RabbitMQ**, gatillando de forma reactiva el decremento de inventario en Java a **48 unidades** en MongoDB de manera automática y transparente.

### Manifestación del Archivo de Infraestructura (`docker-compose.yml`)
```yaml
version: '3.8'

services:
  # 1. Base de Datos Documental (Java - Catálogo de Productos)
  products-db:
    image: mongo:6.0
    container_name: products-mongodb
    restart: always
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: dbuser
      MONGO_INITDB_ROOT_PASSWORD: dbpassword123!
      MONGO_INITDB_DATABASE: ProductsDb
    volumes:
      - mongodb_data:/data/db

  # 2. Base de Datos Relacional (.NET 8 - Gestión de Órdenes)
  orders-db:
    image: postgres:15-alpine
    container_name: orders-postgres
    restart: always
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: SecretPassword123!
      POSTGRES_DB: OrdersDb
    volumes:
      - postgres_data:/var/lib/postgresql/data

  # 3. Memoria en Caché Distribuida (Node.js - Carrito de Compras)
  cart-cache:
    image: redis:7-alpine
    container_name: cart-redis
    restart: always
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  # 4. Broker de Mensajería Asíncrona (Comunicación entre Plataformas)
  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: global-rabbitmq
    restart: always
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest

volumes:
  mongodb_data:
  postgres_data:
  redis_data:
```
