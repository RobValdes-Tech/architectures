package com.store.product.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import java.util.Map;

// DTO corregido con mapeo explícito para atrapar la estructura nativa de MassTransit
public record OrderCreatedEvent(
    String orderId,
    String userId,
    String productId,
    int quantity,
    String createdAt
) {
    // Constructor compacto para extraer los datos desde el objeto raíz 'message' de MassTransit
    @JsonProperty("message")
    public OrderCreatedEvent(Map<String, Object> message) {
        this(
            (String) message.get("orderId"),
            (String) message.get("userId"),
            (String) message.get("productId"),
            (Integer) message.get("quantity"),
            (String) message.get("createdAt")
        );
    }
}
