using ThAmCo.Staff.Models;

namespace ThAmCo.Staff.Services
{
    public interface IProductApiClient
    {

        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(int id);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

    }
}