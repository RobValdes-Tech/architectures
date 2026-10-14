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
