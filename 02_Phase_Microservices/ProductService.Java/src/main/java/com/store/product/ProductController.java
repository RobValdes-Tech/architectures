package com.store.product.controller;

import com.store.product.model.Product;
import com.store.product.repository.ProductRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/products")
public class ProductController {

    private final ProductRepository productRepository;

    // Inyección de dependencias nativa por medio del constructor
    public ProductController(ProductRepository productRepository) {
        // Vinculamos la capa de persistencia autogenerada por Spring Data
        this.productRepository = productRepository;
    }

    // Endpoint para obtener todos los productos del catálogo
    @GetMapping
    public List<Product> getAllProducts() {
        return productRepository.findAll();
    }

    // Endpoint para insertar un nuevo artículo en la base de datos documental
    @PostMapping
    public ResponseEntity<Product> createProduct(@RequestBody Product product) {
        Product savedProduct = productRepository.save(product);
        return new ResponseEntity<>(savedProduct, HttpStatus.CREATED);
    }
}
