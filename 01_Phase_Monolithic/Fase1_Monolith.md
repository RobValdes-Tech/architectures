# Bitácora Técnica - Fase 1: Arquitectura Monolítica
**Ecosistema:** .NET 8 (ASP.NET Core Web API)

Este documento detalla los pasos de diseño e implementación del monolito inicial para el sistema de tienda en Appxolot Training.

## 🏛️ Decisiones de Diseño Arquitectónico
1. **Unidad de Despliegue Única:** Todo el código fuente (Módulos de Productos, Órdenes y Controladores) se compila y empaqueta en un solo binario ejecutable.
2. **Comunicación en Memoria:** Los componentes no sufren de latencia de red. La comunicación se realiza mediante interfaces (`IProductService`, `IOrderService`) registradas en el contenedor nativo de Inyección de Dependencias.
3. **Contrato Centralizado:** Se utiliza el paquete nativo de OpenAPI de .NET para autogenerar la documentación del API en tiempo de ejecución.

## 🛠️ Pasos de Implementación Realizados

### 1. Inicialización del Entorno
Se estructuró la solución y el proyecto web mediante la interfaz de comandos (.NET CLI):
```powershell
dotnet new sln -n MonolithicArchitecture
dotnet new webapi -controllers -n MonolithStore.NET
dotnet sln MonolithicArchitecture.sln add MonolithStore.NET/MonolithStore.NET.csproj
```

### 2. Definición del Core de Negocio
Se modularizaron las capacidades esenciales simulando capas aisladas en memoria para evitar el acoplamiento directo desde el inicio.

### 3. Exposición del Punto de Acceso (Endpoint)
El controlador expone la ruta HTTP POST `api/store/checkout`, encargándose de coordinar de forma secuencial la validación del inventario y el registro transaccional de la orden de compra.

## 🚀 Cómo Probar el Endpoint Localmente
Envía una petición HTTP POST utilizando un cliente como Postman o cURL con la siguiente estructura:

* **URL:** `https://localhost:<PORT>/api/store/checkout`
* **Headers:** `Content-Type: application/json`
* **Payload (JSON):**
```json
{
  "userId": "USR-737",
  "productId": "PROD-A10",
  "quantity": 3
}
```
