package com.store.product.consumer;

import com.store.product.dto.OrderCreatedEvent;
import com.store.product.model.Product;
import com.store.product.repository.ProductRepository;
import org.springframework.amqp.rabbit.annotation.Argument;
import org.springframework.amqp.rabbit.annotation.Exchange;
import org.springframework.amqp.rabbit.annotation.Queue;
import org.springframework.amqp.rabbit.annotation.QueueBinding;
import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.stereotype.Component;

import java.util.Optional;

@Component
public class OrderCreatedConsumer {

    private final ProductRepository productRepository;

    public OrderCreatedConsumer(ProductRepository productRepository) {
        this.productRepository = productRepository;
    }

    // Configuración políglota: Enlazamos la cola al exchange generado automáticamente por MassTransit en .NET
    @RabbitListener(bindings = @QueueBinding(
        value = @Queue(value = "order-created-product-service-queue", durable = "true"),
        exchange = @Exchange(value = "OrderService.Net.Shared.Events:OrderCreatedEvent", type = "fanout", durable = "true")
    ))
    public void consumeOrderCreatedEvent(OrderCreatedEvent event) {
        System.out.println("====== EVENTO RECIBIDO DESDE .NET CORREO ASÍNCRONO ======");
        System.out.println("Procesando decremento de stock para Producto ID: " + event.productId());

        // Buscar el producto en la base de datos documental (MongoDB)
        Optional<Product> productOptional = productRepository.findById(event.productId());

        if (productOptional.isPresent()) {
            Product product = productOptional.get();
            int currentStock = product.getStock();
            int quantityPurchased = event.quantity();

            if (currentStock >= quantityPurchased) {
                // Aplicar reducción de inventario
                product.setStock(currentStock - quantityPurchased);
                productRepository.save(product);
                System.out.println("Stock actualizado con éxito. Stock anterior: " + currentStock + " | Nuevo Stock: " + product.getStock());
            } else {
                System.out.println("ALERTA: Stock insuficiente para procesar la orden del producto: " + product.getName());
            }
        } else {
            System.out.println("ERROR: El producto con ID " + event.productId() + " no existe en MongoDB.");
        }
    }
}
