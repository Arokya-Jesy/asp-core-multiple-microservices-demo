using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ProductMicroservice.Models;
using ProductMicroservice.Repositories;
using ProductMicroservice.Services;

namespace ProductMicroservice.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var mockRepo = new Mock<IProductRepository>();
        var logger = Mock.Of<ILogger<ProductService>>();
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 10.99m },
            new Product { Id = 2, Name = "Product2", Price = 20.99m }
        };
        
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        var service = new ProductService(mockRepo.Object, logger);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_ValidRequest_ReturnsProduct()
    {
        // Arrange
        var mockRepo = new Mock<IProductRepository>();
        var logger = Mock.Of<ILogger<ProductService>>();
        var request = new CreateProductRequest { Name = "New Product", Price = 15.99m };
        var expectedProduct = new Product { Id = 1, Name = "New Product", Price = 15.99m };
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(expectedProduct);
        var service = new ProductService(mockRepo.Object, logger);

        // Act
        var result = await service.CreateProductAsync(request);

        // Assert
        Assert.Equal("New Product", result.Name);
        Assert.Equal(15.99m, result.Price);
        mockRepo.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }
}
