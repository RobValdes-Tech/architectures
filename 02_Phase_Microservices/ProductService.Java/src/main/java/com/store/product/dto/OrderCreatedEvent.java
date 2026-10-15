package com.store.product.dto;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import java.time.LocalDateTime;
import java.util.UUID;

// Ignoramos el resto de propiedades de la envoltura de MassTransit y nos enfocamos en el binding plano
@JsonIgnoreProperties(ignoreUnknown = true)
public record OrderCreatedEvent(
    UUID orderId,
    String userId,
    String productId,
    int quantity
) {}
