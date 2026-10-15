# Runbook de Capacitación - Fase 2: Arquitectura de Microservicios Políglota y Eventos
**Ecosistema:** Java 17 (Spring Boot) & .NET 8 (C#) & Node.js (JavaScript)  
**Host de Desarrollo:** Windows 10 (Docker Desktop + WSL2)  
**Repositorio:** GitHub - RobValdes-Tech/architectures  

Este documento constituye una guía avanzada de ingeniería de sistemas distribuida para capacitar desarrolladores Junior. Describe el desacoplamiento de dominios, la persistencia políglota aislada y la coreografía de eventos asíncronos mediante un Message Broker.

---

## 📋 Índice del Runbook
1. [Vistas de Arquitectura y Diagramas de Integración](#1-vistas-de-arquitectura-y-diagramas-de-integración)
   * [Diagrama de Arquitectura de Bloques Distribuidos](#diagrama-de-arquitectura-de-bloques-distribuidos)
   * [Diagrama de Secuencia de la Coreografía de Eventos](#diagrama-de-secuencia-de-la-coreografía-de-eventos)
2. [Estructura y Modelado de Datos (Persistencia Políglota)](#2-estructura-y-modelado-de-datos-persistencia-políglota)
   * [Diagrama de Entidad-Relación (ER) y Modelos NoSQL](#diagrama-de-entidad-relación-er-y-modelos-nosql)
   * [Scripts DDL de Inicialización de Bases de Datos](#scripts-ddl-de-inicialización-de-bases-de-datos)
3. [Manifiesto de Infraestructura Global y Orquestación](#3-manifiesto-de-infraestructura-global-y-orquestación)
   * [Código del Archivo docker-compose.yml](#código-del-archivo-docker-composeyml)
   * [Comandos CLI para el Control de Contenedores](#comandos-cli-para-el-control-de-contenedores)
4. [Guía de Implementación por Componente de Software](#4-guía-de-implementación-por-componente-de-software)
   * [Componente 4.1: Microservicio de Productos (Java)](#componente-41-microservicio-de-productos-java)
   * [Componente 4.2: Microservicio de Órdenes (.NET 8)](#componente-42-microservicio-de-órdenes-net-8)
   * [Componente 4.3: Microservicio de Carrito (Node.js - Puente de Entrada)](#componente-43-microservicio-de-carrito-nodejs---puente-de-entrada)
5. [Runbook de Verificación y Solución de Errores (Troubleshooting)](#5-runbook-de-verificación-y-solución-de-errores-troubleshooting)

---

## 1. Vistas de Arquitectura y Diagramas de Integración

### Diagrama de Arquitectura de Bloques Distribuidos
Muestra cómo se rompe el monolito. Cada dominio funcional cuenta con su propio runtime, puerto independiente y base de datos aislada.

```mermaid
graph TD
    Client([Postman / Cliente Externo]) -->|POST :8081 /api/products| JavaService[ProductService Java]
    Client -->|POST :5291 /api/orders| NetService[OrderService .NET 8]
    Client -->|POST :8082 /api/cart| NodeService[CartService Node.js]

    subgraph Capa_Persistencia [Capa de Persistencia Aislada]
        JavaService -->|Conexión Nativa| MongoCont[MongoDB :27017]
        NetService -->|Entity Framework Core| PostgresCont[PostgreSQL :5432]
        NodeService -->|Cliente Redis| RedisCont[Redis :6379]
    end

    subgraph Bus_Eventos [Ecosistema de Mensajería Asíncrona]
        NetService -->|Publica con MassTransit| RabbitMQ[RabbitMQ :5672 / :15672]
        RabbitMQ -->|Suscripción AMQP| JavaService
        RabbitMQ -.->|Puente de Conexión Pendiente| NodeService
    end
```

### Diagrama de Secuencia de la Coreografía de Eventos
Ilustra el comportamiento del sistema cuando ocurre una compra, demostrando la reactividad y el desacoplamiento temporal de la plataforma.

```mermaid
sequenceDiagram
    autonumber
    actor Cliente as Desarrollador Junior / Postman
    participant Net as OrderService (.NET :5291)
    participant DB_Rel as PostgreSQL (:5432)
    participant Broker as RabbitMQ (:5672)
    participant Java as ProductService (Java :8081)
    participant DB_Doc as MongoDB (:27017)

    Cliente->>Net: POST /api/orders (Payload Compra)
    Note over Net,DB_Rel: Persistencia Relacional
    Net->>DB_Rel: INSERT INTO orders (Id, UserId, ProductId, Quantity)
    DB_Rel-->>Net: Transacción Confirmada
    
    Note over Net,Broker: Disparo Asíncrono (Raw JSON)
    Net->>Broker: Publish(OrderCreatedEvent) plano
    Net-->>Cliente: HTTP 201 Created (Respuesta Inmediata)

    Note over Broker,Java: Reactividad Guiada por Eventos
    Broker->>Java: Envío de Mensaje a 'order-created-product-service-queue'
    Java->>DB_Doc: findById(productId)
    DB_Doc-->>Java: Retorna Documento de Producto
    
    Note over Java,DB_Doc: Validación y Decremento de Stock
    Java->>DB_Doc: save(product con stock - quantity)
    DB_Doc-->>Java: Documento Actualizado
    Note over Java: Log en Consola: "Nuevo Stock Confirmado"
```

---

## 2. Estructura y Modelado de Datos (Persistencia Políglota)

### Diagrama de Entidad-Relación (ER) y Modelos NoSQL

```mermaid
erDiagram
    %% Base de Datos Relacional (PostgreSQL)
    ORDERS {
        uuid id PK
        string user_id
        string product_id
        int quantity
        timestamp created_at
    }

    %% Base de Datos NoSQL Documental (MongoDB)
    PRODUCTS {
        string _id PK
        string name
        double price
        int stock
    }

    %% Almacenamiento en Memoria Caché (Redis)
    CARTS {
        string key_userId PK
        string field_productId
        string value_quantity
    }

    ORDERS }o--|| PRODUCTS : "Referencia lógica de dominio"
```

### Scripts DDL de Inicialización de Bases de Datos

#### 1. Esquema Relacional de Transacciones (PostgreSQL)
Este script es ejecutado automáticamente por Entity Framework Core al aplicar la migración inicial.
```sql
-- Creación de la Tabla de Historial de Migraciones
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Creación de la Tabla de Órdenes
CREATE TABLE orders (
    id uuid NOT NULL,
    user_id character varying(255) NOT NULL,
    product_id character varying(255) NOT NULL,
    quantity integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT "PK_orders" PRIMARY KEY (id)
);
```

#### 2. Esquema NoSQL Documental (MongoDB)
Estructura JSON de la colección `products` dentro de la base de datos `ProductsDb`.
```json
{
  "_id": "PROD-100",
  "name": "Hoka Clifton 13 Wide",
  "price": 140.00,
  "stock": 50
}
```

---

## 3. Manifiesto de Infraestructura Global y Orquestación

### Código del Archivo `docker-compose.yml`
Ubicado en la raíz de la Fase 2 para aprovisionar los contenedores locales.

*Ruta exacta:* `02_Phase_Microservices/docker-compose.yml`
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

### Comandos CLI para el Control de Contenedores
```powershell
# Acceder a la carpeta de infraestructura
cd C:\Users\rob\proyectos\Tech-stack\architectures\02_Phase_Microservices

# Levantar los servicios y limpiar volumenes corruptos anteriores
docker-compose down -v
docker-compose up -d

# Validar que los puertos estén expuestos correctamente hacia Windows 10
docker ps
```

---

## 4. Guía de Implementación por Componente de Software

### Componente 4.1: Microservicio de Productos (Java)

#### Propiedades de Conexión (`ProductService.Java/src/main/resources/application.properties`)
```properties
server.port=8081
spring.data.mongodb.uri=mongodb://dbuser:dbpassword123!@localhost:27017/ProductsDb?authSource=admin
spring.rabbitmq.host=localhost
spring.rabbitmq.port=5672
spring.rabbitmq.username=guest
spring.rabbitmq.password=guest
```

#### Clase de Configuración de Mensajes (`config/RabbitMQConfig.java`)
```java
package com.store.product.config;

import org.springframework.amqp.core.*;
import org.springframework.amqp.support.converter.Jackson2JsonMessageConverter;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitMQConfig {

    @Bean
    public Queue productQueue() {
        return new Queue("order-created-product-service-queue", true);
    }

    @Bean
    public FanoutExchange orderExchange() {
        return new FanoutExchange("OrderService.Net.Shared.Events:OrderCreatedEvent", true, false);
    }

    @Bean
    public Binding binding(Queue productQueue, FanoutExchange orderExchange) {
