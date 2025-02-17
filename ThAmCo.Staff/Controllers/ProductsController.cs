using Microsoft.AspNetCore.Mvc;
using ThAmCo.Staff.Services;
using ThAmCo.Staff.Models;
using Microsoft.AspNetCore.Authorization;

//[Authorize]
public class ProductsController : Controller
{
    private readonly IProductApiClient _productApiClient;

    public ProductsController(IProductApiClient productApiClient)
    {
        _productApiClient = productApiClient;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productApiClient.GetProductsAsync();
        return View(new ProductViewModel { Products = products });
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }
    // Create product
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productApiClient.CreateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }
    // Edit product
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productApiClient.GetProductAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Edit product
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _productApiClient.UpdateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }
    // Get product by id
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productApiClient.GetProductAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Delete product
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productApiClient.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}