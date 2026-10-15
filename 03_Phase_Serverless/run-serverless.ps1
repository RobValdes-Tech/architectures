# Script de Automatización de Entorno FaaS Local para Desarrolladores Junior
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Inicializando Runtime Serverless Local (Functions Framework)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# Verificar la existencia de dependencias locales (node_modules)
if (-not (Test-Path ".\node_modules")) {
    Write-Host "[INFRA] Directorio node_modules no detectado. Restaurando paquetes npm..." -ForegroundColor Yellow
    npm install
}

Write-Host "[RUNTIME] Levantando Funcion 'processAuditLog' en el puerto 8080..." -ForegroundColor Green
Write-Host "[RUNTIME] Escuchando peticiones HTTP POST bajo demanda..." -ForegroundColor Green

# Ejecutar el emulador local FaaS de forma nativa
npx @google-cloud/functions-framework --target=processAuditLog --port=8080
