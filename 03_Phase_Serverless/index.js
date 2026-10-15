const functions = require('@google-cloud/functions-framework');

// Registrar una función HTTP Serverless llamada 'processAuditLog'
functions.http('processAuditLog', (req, res) => {
    console.log('====== FUNCIÓN SERVERLESS GATILLADA BAJO DEMANDA ======');
    
    const orderData = req.body;
    
    // Validar el payload de entrada estructurado
    if (!orderData || !orderData.productId) {
        console.error('Error: Datos de orden incompletos.');
        return res.status(400).send({ status: 'Rechazado', error: 'Payload Invalido' });
    }

    console.log(`[AUDIT LOG] Procesando transacción para el Producto ID: ${orderData.productId}`);
    console.log(`[AUDIT LOG] Unidades adquiridas: ${orderData.quantity}`);
    console.log(`[ANALÍTICA] Evento analizado con éxito de forma asíncrona.`);

    // Respuesta típica de una función FaaS
    res.status(200).send({
        status: 'Procesado',
        message: 'Log transaccional de auditoría registrado en el entorno Serverless.',
        timestamp: new Date().toISOString()
    });
});
