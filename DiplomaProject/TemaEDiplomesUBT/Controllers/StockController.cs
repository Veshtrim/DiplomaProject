using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using TemaEDiplomesUBT.Services.IServices;
using TemaEDiplomesUBT.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        public StockController(IStockService stockService, IProductService productService, IWarehouseService warehouseService)
        {
            _stockService = stockService;
            _productService = productService;
            _warehouseService = warehouseService;
        }

        public IActionResult Index()
        {
            var stocks = _stockService.GetStocks();
            return View(stocks);
        }

        public IActionResult Adjust()
        {
            ViewBag.Products = new SelectList(_productService.GetAllProducts(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(_warehouseService.GetAllWarehouses(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Adjust(int productId, int warehouseId, int quantity)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than 0.");
                ViewBag.Products = new SelectList(_productService.GetAllProducts(), "Id", "Name");
                ViewBag.Warehouses = new SelectList(_warehouseService.GetAllWarehouses(), "Id", "Name");
                return View();
            }

            
            await _stockService.AdjustStock(productId, warehouseId, quantity);

     
            return RedirectToAction("Index", "Stock");
        }
        public IActionResult Transfer()
        {
            ViewBag.Products = new SelectList(_productService.GetAllProducts(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(_warehouseService.GetAllWarehouses(), "Id", "Name");
            return View();
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(StockTransferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = new SelectList(_productService.GetAllProducts(), "Id", "Name");
                ViewBag.Warehouses = new SelectList(_warehouseService.GetAllWarehouses(), "Id", "Name");
                return View(model);
            }

            // Call the stock service to initiate the transfer workflow
            await _stockService.TransferStock(model.SourceWarehouseId, model.DestinationWarehouseId, model.ProductId, model.Quantity);

            return RedirectToAction("Index", "Stock");
        }
    }

}
