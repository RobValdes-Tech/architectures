package com.store.product.repository;

import com.store.product.model.Product;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ProductRepository extends MongoRepository<Product, String> {
    // La interfaz hereda automáticamente operaciones como findAll(), findById() y save()
}
