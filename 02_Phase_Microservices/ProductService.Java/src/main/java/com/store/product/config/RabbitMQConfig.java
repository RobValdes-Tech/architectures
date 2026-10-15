package com.store.product.config;

import org.springframework.amqp.core.Binding;
import org.springframework.amqp.core.BindingBuilder;
import org.springframework.amqp.core.FanoutExchange;
import org.springframework.amqp.core.Queue;
import org.springframework.amqp.support.converter.Jackson2JsonMessageConverter;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitMQConfig {

    // 1. Declarar la cola para el servicio de productos
    @Bean
    public Queue productQueue() {
        return new Queue("order-created-product-service-queue", true);
    }

    // 2. Declarar el Exchange homólogo al que emite .NET
    @Bean
    public FanoutExchange orderExchange() {
        return new FanoutExchange("OrderService.Net.Shared.Events:OrderCreatedEvent", true, false);
    }

    // 3. Crear el enlace (Binding) automático entre la cola y el exchange
    @Bean
    public Binding binding(Queue productQueue, FanoutExchange orderExchange) {
        return BindingBuilder.bind(productQueue).to(orderExchange);
    }

    // 4. Mantener el convertidor JSON para Jackson
    @Bean
    public Jackson2JsonMessageConverter messageConverter() {
        return new Jackson2JsonMessageConverter();
    }
}
