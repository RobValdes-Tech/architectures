const amqp = require('amqplib');
const axios = require('axios');

const RABBITMQ_URL = 'amqp://guest:guest@localhost:5672';
const FUNCTION_URL = 'http://localhost:8080';
const EXCHANGE_NAME = 'OrderService.Net.Shared.Events:OrderCreatedEvent';
const QUEUE_NAME = 'order-created-serverless-queue';

async function startBridge() {
    try {
        console.log('[BRIDGE] Conectando al Broker de RabbitMQ...');
        const connection = await amqp.connect(RABBITMQ_URL);
        const channel = await connection.createChannel();

        // Asegurar topología políglota idéntica a la Fase 2
        await channel.assertExchange(EXCHANGE_NAME, 'fanout', { durable: true });
        await channel.assertQueue(QUEUE_NAME, { durable: true });
        await channel.bindQueue(QUEUE_NAME, EXCHANGE_NAME, '');

        console.log(`[BRIDGE] Escuchando cola '${QUEUE_NAME}'. Esperando checkouts de .NET...`);

        channel.consume(QUEUE_NAME, async (msg) => {
            if (msg !== null) {
                console.log('\n[BRIDGE] ¡Evento detectado en el Bus de Datos!');
                const rawPayload = JSON.parse(msg.content.toString());
                
                // Extraer el mensaje plano (Raw JSON)
                const orderData = rawPayload.message || rawPayload;

                try {
                    console.log(`[BRIDGE] Despertando Función Serverless en ${FUNCTION_URL}...`);
                    // Disparo HTTP asíncronamente a la FaaS local
                    const response = await axios.post(FUNCTION_URL, orderData);
                    console.log(`[SERVERLESS RESPONSE] Estatus: ${response.status} - ${response.data.status}`);
                    
                    channel.ack(msg); // Confirmar mensaje procesado con éxito
                } catch (error) {
                    console.error('[BRIDGE ERROR] La función Serverless no respondió:', error.message);
                    channel.nack(msg, false, true); // Reencolar el mensaje para reintento
                }
            }
        });

    } catch (error) {
        console.error('[BRIDGE CRITICAL] Error en el bus de datos:', error.message);
        setTimeout(startBridge, 5000); // Reintento de conexión automática
    }
}

startBridge();
