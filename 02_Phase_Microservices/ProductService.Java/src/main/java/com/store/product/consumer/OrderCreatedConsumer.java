package com.store.product.consumer;

import com.store.product.dto.OrderCreatedEvent;
import com.store.product.repository.ProductRepository;
import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.stereotype.Component;

@Component
public class OrderCreatedConsumer {

    private final ProductRepository productRepository;

    public OrderCreatedConsumer(ProductRepository productRepository) {
        this.productRepository = productRepository;
    }

    // Spring AMQP mapea automáticamente los mensajes de la cola hacia nuestro DTO de Java
    @RabbitListener(queues = "order-created-queue")
    public void consumeOrderCreatedEvent(OrderCreatedEvent event) {
        System.out.println("====== EVENTO CONSUMIDO EN JAVA ======");
        System.out.println("Orden ID recibida: " + event.orderId());
        System.out.println("Producto ID a procesar: " + event.productId());
        System.out.println("Cantidad comprada: " + event.quantity());
        
        // En el siguiente paso implementaremos la actualización real del stock en MongoDB
    }
}
