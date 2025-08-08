using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ThAmCo.Staff.Models;
using ThAmCo.Staff.Services;

    public class ProductApiClientFake : IProductApiClient
    {
    private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Microwave Oven", Description = "1000W countertop microwave with multiple cooking presets", Price = 89.99f },
            new Product { Id = 2, Name = "Coffee Maker", Description = "12-cup programmable coffee maker with auto shut-off", Price = 49.99f },
            new Product { Id = 3, Name = "Vacuum Cleaner", Description = "Bagless upright vacuum with HEPA filter", Price = 129.99f },
            new Product { Id = 4, Name = "Toaster", Description = "4-slice toaster with wide slots and bagel function", Price = 34.99f },
            new Product { Id = 5, Name = "Blender", Description = "High-speed blender for smoothies and food processing", Price = 79.99f },
            new Product { Id = 6, Name = "Electric Kettle", Description = "1.7L cordless electric kettle with auto shut-off", Price = 29.99f },
            new Product { Id = 7, Name = "Air Fryer", Description = "Digital air fryer with multiple cooking functions", Price = 99.99f },
            new Product { Id = 8, Name = "Food Storage Containers", Description = "Set of 10 BPA-free food storage containers with lids", Price = 24.99f },
            new Product { Id = 9, Name = "Dish Drying Rack", Description = "Stainless steel 2-tier dish drying rack", Price = 39.99f },
            new Product { Id = 10, Name = "Slow Cooker", Description = "6-litre programmable slow cooker with removable stoneware", Price = 59.99f }
        };
        // Get all products
        public async Task<List<Product>> GetProductsAsync()
        {
            return await Task.FromResult(_products);
        }
        // Get product by id
        public async Task<Product?> GetProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return await Task.FromResult(product);
        }
        // Create product
        public async Task CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
            await Task.CompletedTask;
        }
        // Edit product
        public async Task UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {product.Id} not found.");

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;

            await Task.CompletedTask;
        }
        // Delete product
        public async Task DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            _products.Remove(product);
            await Task.CompletedTask;
        }
    
}

