package com.store.product.dto;

import java.time.LocalDateTime;
import java.util.UUID;

// Registro inmutable (Record) equivalente al contrato emitido por .NET
public record OrderCreatedEvent(
    UUID orderId,
    String userId,
    String productId,
    int quantity,
    LocalDateTime createdAt
) {}
