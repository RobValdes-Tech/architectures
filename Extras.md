# Smoke Test de Validación de Flujo

1. **Crear Producto:** Realizar un HTTP POST a `http://localhost:8081/api/products` (Responde `201 Created`).
2. **Generar Compra:** Realizar un HTTP POST a `http://localhost:5291/api/orders` con la cantidad elegida.
3. **Validación Visual:** La ventana de Java imprimirá en milisegundos la reducción real del stock de 50 a 48 unidades en MongoDB, mientras que la ventana Serverless se activará bajo demanda imprimiendo los logs analíticos de la auditoría.

---

## 🔧 Bitácora de Ingeniería Histórica: Errores Comunes y Soluciones (Troubleshooting)

Esta bitácora recopila las incidencias reales detectadas durante la fase de desarrollo e integración de los componentes del sistema:

### 1. Error de compilación en .NET por archivo .slnx de SDK Experimental
* **Mensaje de Error:** `warning Unable to find a project to restore!`
* **Causa:** El SDK de .NET 9/10 asume automáticamente el formato experimental XML de soluciones `.slnx` al compilar desde la raíz mediante rutas relativas.
* **Solución:** Forzar el build saltando el analizador de soluciones apuntando directamente al archivo del proyecto:  
  ```bash
  dotnet build .\OrderService.Net.csproj
  ```

### 2. Error 13 de Autenticación Denegada en MongoDB
* **Mensaje de Error:** `UncategorizedMongoDbException: Command failed with error 13 (Unauthorized): 'Command update requires authentication'`
* **Causa:** El contenedor oficial de Mongo tiene habilitadas las directivas de seguridad locales y rechaza operaciones de escritura planas anónimas de Spring Boot.
* **Solución:** Unificar credenciales en `docker-compose.yml` (`MONGO_INITDB_ROOT_USERNAME: dbuser`) y actualizar la URI en Java:  
  `mongodb://dbuser:dbpassword123!@localhost:27017/ProductsDb?authSource=admin`

### 3. Error de Bloqueo de Archivo Ejecutable por Proceso Huérfano en Windows
* **Mensaje de Error:** `The process cannot access the file because it is being used by another process`
* **Causa:** Al detener la ejecución previa, el sistema operativo mantuvo el binario bloqueado e indexado bajo un ID de tarea activo.
* **Solución:** Rastrear el ID y liberar el archivo forzadamente por comandos de consola:  
  ```powershell
  Stop-Process -Name "OrderService.Net" -Force
  ```

### 4. Error 28P01 de Conexión en PostgreSQL por Volúmenes Docker Sucios
* **Mensaje de Error:** `28P01: password authentication failed for user "postgres"`
* **Causa:** Docker reutiliza los datos montados de contenedores anteriores e ignora la nueva clave declarada en las variables de entorno.
* **Solución:** Purgar los volúmenes montados locales para forzar la inicialización limpia del motor relacional:  
  ```bash
  docker-compose down -v
  ```

### 5. Error de Interoperabilidad por Evento Nulo (MassTransit a Java)
* **Mensaje de Error:** `Procesando decremento de stock para Producto ID: null`
* **Causa:** MassTransit de .NET envuelve por defecto las propiedades JSON en un nodo raíz llamado `message` usando `camelCase`, volviendo el payload incompatible con la lectura plana por defecto de Jackson en Java.
* **Solución:** Configurar en .NET la directiva de serialización limpia sin envolturas propietarias:  
  ```csharp
  cfg.UseRawJsonSerializer();
  ```

### 6. Error de Red de Docker: Bind for 0.0.0.0 failed: port is already allocated
* **Mensaje de Error:** `driver failed programming external connectivity... port is already allocated`
* **Causa:** Múltiples contenedores en diferentes carpetas de red compiten por adueñarse de las mismas interfaces físicas del host (como los puertos `27017` o `6379`).
* **Solución:** Identificar el PID intruso mediante `Get-NetTCPConnection -LocalPort 27017` y destruirlo con:  
  ```powershell
  Stop-Process -Id <PID> -Force
  ```

### 7. Error de Exposición de Puertos en Blanco por Aislamiento Estricto
* **Mensaje de Error:** `Connection refused` (Los contenedores están *Up* pero Postman no puede conectar).
* **Causa:** El uso de redes internas (`networks`) en Docker aísla de forma estricta los puertos si no se declaran explícitamente en formato de cadena hacia el host local.
* **Solución:** Eliminar el aislamiento estricto y envolver los mapeos entre comillas dobles (`"27017:27017"`) en el manifiesto global para forzar la exposición nativa hacia Windows.
