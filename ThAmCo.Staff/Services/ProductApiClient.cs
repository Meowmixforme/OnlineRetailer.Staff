using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using ThAmCo.Staff.Models;

namespace ThAmCo.Staff.Services
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductApiClient> _logger;
        private TokenDto _token;
        private DateTime _tokenExpiration;

        private record TokenDto(string access_token, string token_type, int expires_in);

        public ProductApiClient(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<ProductApiClient> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<string> GetOrRefreshTokenAsync()
        {
            if (_token != null && DateTime.UtcNow < _tokenExpiration)
            {
                return _token.access_token;
            }

            _logger.LogInformation("Refreshing token");
            var tokenClient = _clientFactory.CreateClient("TokenClient");
            var domain = _configuration["Auth0:Domain"];
            var tokenEndpoint = $"https://{domain}/oauth/token";

            var tokenValues = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["Auth0:ClientId"] },
                { "client_secret", _configuration["Auth0:ClientSecret"] },
                { "audience", _configuration["Auth0:Audience"] },
            };

            var tokenForm = new FormUrlEncodedContent(tokenValues);
            var tokenResponse = await tokenClient.PostAsync(tokenEndpoint, tokenForm);
            var responseContent = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Token request failed: {StatusCode}, Error: {Error}", tokenResponse.StatusCode, responseContent);
                throw new HttpRequestException($"Token request failed: {tokenResponse.StatusCode}. Details: {responseContent}");
            }

            _token = JsonConvert.DeserializeObject<TokenDto>(responseContent);
            _tokenExpiration = DateTime.UtcNow.AddSeconds(_token.expires_in);
            _logger.LogInformation("Token refreshed successfully");
            return _token.access_token;
        }

        private async Task<HttpClient> GetAuthorizedClientAsync()
        {
            var client = _clientFactory.CreateClient("ProductsClient");
            var token = await GetOrRefreshTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogInformation("Authorized client created with base address: {BaseAddress}", client.BaseAddress);
            return client;
        }
        // Get all products
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Starting GetProductsAsync");
                var client = await GetAuthorizedClientAsync();
                _logger.LogInformation("Sending GET request to Products endpoint");
                var response = await client.GetAsync("Products");
                _logger.LogInformation("Response received: {StatusCode}", response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response content: {Content}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Get products request failed: {StatusCode}, Error: {Error}", response.StatusCode, content);
                    throw new HttpRequestException($"Get products request failed: {response.StatusCode}");
                }

                var products = JsonConvert.DeserializeObject<List<Product>>(content);
                _logger.LogInformation("Deserialized {Count} products", products?.Count ?? 0);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetProductsAsync");
                throw;
            }
        }
        // Get product by id
        public async Task<Product> GetProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Starting GetProductAsync for id: {Id}", id);
                var client = await GetAuthorizedClientAsync();
                var response = await client.GetAsync($"Products/{id}");
                _logger.LogInformation("Response received: {StatusCode}", response.StatusCode);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Product not found for id: {Id}", id);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response content: {Content}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Get product request failed: {StatusCode}, Error: {Error}", response.StatusCode, content);
                    throw new HttpRequestException($"Get product request failed: {response.StatusCode}");
                }

                var product = JsonConvert.DeserializeObject<Product>(content);
                _logger.LogInformation("Product retrieved successfully for id: {Id}", id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetProductAsync for id: {Id}", id);
                throw;
            }
        }
        // Create product
        public async Task CreateProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Starting CreateProductAsync");
                var client = await GetAuthorizedClientAsync();
                var response = await client.PostAsJsonAsync("Products", product);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response received: {StatusCode}, Content: {Content}", response.StatusCode, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Create product request failed: {StatusCode}, Error: {Error}", response.StatusCode, content);
                    throw new HttpRequestException($"Create product request failed: {response.StatusCode}");
                }

                _logger.LogInformation("Product created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in CreateProductAsync");
                throw;
            }
        }
        // Edit product
        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Starting UpdateProductAsync for id: {Id}", product.Id);
                var client = await GetAuthorizedClientAsync();
                var response = await client.PutAsJsonAsync($"Products/{product.Id}", product);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response received: {StatusCode}, Content: {Content}", response.StatusCode, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Update product request failed: {StatusCode}, Error: {Error}", response.StatusCode, content);
                    throw new HttpRequestException($"Update product request failed: {response.StatusCode}");
                }

                _logger.LogInformation("Product updated successfully for id: {Id}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateProductAsync for id: {Id}", product.Id);
                throw;
            }
        }
        // Delete product
        public async Task DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Starting DeleteProductAsync for id: {Id}", id);
                var client = await GetAuthorizedClientAsync();
                var response = await client.DeleteAsync($"Products/{id}");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response received: {StatusCode}, Content: {Content}", response.StatusCode, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Delete product request failed: {StatusCode}, Error: {Error}", response.StatusCode, content);
                    throw new HttpRequestException($"Delete product request failed: {response.StatusCode}");
                }

                _logger.LogInformation("Product deleted successfully for id: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in DeleteProductAsync for id: {Id}", id);
                throw;
            }
        }
    }
}