using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThAmCo.Staff.Models;
using ThAmCo.Staff.Services;
using Xunit;

namespace ThAmCo.Staff.Tests
{
    public class ProductApiClientTests
    {
        private readonly List<Product> _fakeProducts = new()
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

        private Mock<HttpMessageHandler> CreateTokenHttpMock()
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    access_token = "mocked_access_token",
                    token_type = "Bearer",
                    expires_in = 3600
                }), Encoding.UTF8, "application/json")
            };

            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("oauth/token")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse)
                .Verifiable();

            return mock;
        }

        private Mock<HttpMessageHandler> CreateHttpMock(HttpStatusCode expectedCode, string expectedJson)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = expectedCode
            };
            if (!string.IsNullOrEmpty(expectedJson))
            {
                response.Content = new StringContent(expectedJson,
                                                     Encoding.UTF8,
                                                     "application/json");
            }
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();
            return mock;
        }

        private IProductApiClient CreateProductApiClient(Mock<HttpMessageHandler> httpMock, Mock<HttpMessageHandler> tokenHttpMock, ILogger<ProductApiClient> logger = null)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var productClient = new HttpClient(httpMock.Object) { BaseAddress = new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/") };
            var tokenClient = new HttpClient(tokenHttpMock.Object);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["WebServices:Products:BaseAddress"]).Returns("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/");
            mockConfiguration.Setup(c => c["WebServices:Products:UseFake"]).Returns("false");
            mockConfiguration.Setup(c => c["Auth0:Domain"]).Returns("dev-jktwlj0wfv0smqmt.eu.auth0.com");
            mockConfiguration.Setup(c => c["Auth0:ClientId"]).Returns("w3r0J6TZYxFcj95xdgMtsIAUq6mMWIWL");
            mockConfiguration.Setup(c => c["Auth0:ClientSecret"]).Returns("VqlehITRO7XdaIsmxh-VA__0h3xOl9Rm51O_ttRhcVLUsbeWi-p5p7z1yURwWOUx");
            mockConfiguration.Setup(c => c["Auth0:Audience"]).Returns("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net");
            mockFactory.Setup(f => f.CreateClient("ProductsClient")).Returns(productClient);
            mockFactory.Setup(f => f.CreateClient("TokenClient")).Returns(tokenClient);
            return new ProductApiClient(mockFactory.Object, mockConfiguration.Object, logger ?? new Mock<ILogger<ProductApiClient>>().Object);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedJson = JsonConvert.SerializeObject(_fakeProducts);
            var httpMock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var tokenHttpMock = CreateTokenHttpMock();
            var client = CreateProductApiClient(httpMock, tokenHttpMock);

            // Act
            var result = await client.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_fakeProducts.Count, result.Count);
            Assert.Equal(_fakeProducts[0].Name, result[0].Name);
            Assert.Equal(_fakeProducts[9].Name, result[9].Name);

            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                    && req.RequestUri == new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products")),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetProductAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var expectedProduct = _fakeProducts[0];
            var expectedJson = JsonConvert.SerializeObject(expectedProduct);
            var httpMock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var tokenHttpMock = CreateTokenHttpMock();
            var client = CreateProductApiClient(httpMock, tokenHttpMock);

            // Act
            var result = await client.GetProductAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result.Id);
            Assert.Equal(expectedProduct.Name, result.Name);

            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                    && req.RequestUri == new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products/1")),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetProductAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var httpMock = CreateHttpMock(HttpStatusCode.NotFound, "");
            var tokenHttpMock = CreateTokenHttpMock();
            var client = CreateProductApiClient(httpMock, tokenHttpMock);

            // Act
            var result = await client.GetProductAsync(999);

            // Assert
            Assert.Null(result);

            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                    && req.RequestUri == new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products/999")),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddNewProduct()
        {
            // Arrange
            var newProduct = new Product { Name = "New Product", Description = "Test Description", Price = 99.99f };
            var httpMock = CreateHttpMock(HttpStatusCode.Created, "");
            var tokenHttpMock = CreateTokenHttpMock();
            var loggerMock = new Mock<ILogger<ProductApiClient>>();
            var client = CreateProductApiClient(httpMock, tokenHttpMock, loggerMock.Object);

            // Act
            await client.CreateProductAsync(newProduct);

            // Assert
            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products") &&
                    JsonConvert.DeserializeObject<Product>(req.Content.ReadAsStringAsync().Result).Name == newProduct.Name),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidProduct_ShouldUpdateProduct()
        {
            // Arrange
            var productToUpdate = _fakeProducts[0];
            var updatedProduct = new Product { Id = productToUpdate.Id, Name = "Updated " + productToUpdate.Name, Description = "Updated Description", Price = productToUpdate.Price + 10 };
            var httpMock = CreateHttpMock(HttpStatusCode.OK, "");
            var tokenHttpMock = CreateTokenHttpMock();
            var client = CreateProductApiClient(httpMock, tokenHttpMock);

            // Act
            await client.UpdateProductAsync(updatedProduct);

            // Assert
            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put &&
                    req.RequestUri == new Uri($"https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products/{productToUpdate.Id}") &&
                    JsonConvert.DeserializeObject<Product>(req.Content.ReadAsStringAsync().Result).Name == updatedProduct.Name),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteProductAsync_WithValidId_ShouldRemoveProduct()
        {
            // Arrange
            var httpMock = CreateHttpMock(HttpStatusCode.NoContent, "");
            var tokenHttpMock = CreateTokenHttpMock();
            var client = CreateProductApiClient(httpMock, tokenHttpMock);

            // Act
            await client.DeleteProductAsync(1);

            // Assert
            httpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri == new Uri("https://jsfproducts-g3egana5c7fsfcea.uksouth-01.azurewebsites.net/Products/1")),
                ItExpr.IsAny<CancellationToken>());

            tokenHttpMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("https://dev-jktwlj0wfv0smqmt.eu.auth0.com/oauth/token")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}