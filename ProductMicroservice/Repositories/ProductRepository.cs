using Microsoft.EntityFrameworkCore;
using ProductMicroservice.Data;
using ProductMicroservice.Models;

namespace ProductMicroservice.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ProductDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
            return null;

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product updated with ID: {ProductId}", id);
        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product deleted with ID: {ProductId}", id);
        return true;
    }
}