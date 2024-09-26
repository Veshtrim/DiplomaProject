using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using Microsoft.AspNetCore.Authorization;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly ApplicationDbContext _context;

        public SaleController(ISaleService saleService, ApplicationDbContext context)
        {
            _saleService = saleService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return View(sales);
        }

        public IActionResult Create()
        {
           
            var products = _context.Products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();

            var warehouses = _context.Warehouses.ToList();
            var customers = _context.Customers.ToList();

            
            if (products == null || !products.Any())
            {
                ModelState.AddModelError("", "No products available.");
                return View(new SaleViewModel());
            }

            if (warehouses == null || !warehouses.Any())
            {
                ModelState.AddModelError("", "No warehouses available.");
                return View(new SaleViewModel());
            }

            
            ViewBag.Products = products;
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");

          
            return View(new SaleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                var result = await _saleService.CreateSaleAsync(model);

              
                if (!result.IsSuccessful)
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Create));
                }

                
                return RedirectToAction(nameof(Index));
            }

           
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _context.Warehouses.ToListAsync(), "Id", "Name");
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "CustomerId", "Name");

          
            return View(model);
        }




        public async Task<IActionResult> Edit(int id)
        {
          
            var sale = await _saleService.GetSaleByIdAsync(id);

            if (sale == null)
            {
                return NotFound();
            }

            
            var saleViewModel = new SaleViewModel
            {
                SaleId = sale.SaleId,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                CustomerId = sale.CustomerId, 
                CustomerName = sale.CustomerName ?? "Unknown",
                IsPaid = sale.IsPaid,
                SaleDetails = sale.SaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    WarehouseId = sd.WarehouseId,
                    Quantity = sd.Quantity,
                    Price = sd.Price,
                    ProductName = sd.ProductName,
                    WarehouseName = sd.WarehouseName
                }).ToList(),
               
                OldSaleDetails = sale.SaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    WarehouseId = sd.WarehouseId,
                    Quantity = sd.Quantity,
                    Price = sd.Price
                }).ToList()
            };

         
            var products = _context.Products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();

            var warehouses = _context.Warehouses.ToList();
            var customers = _context.Customers.ToList();

            ViewBag.Products = products;
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");


            return View(saleViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _saleService.UpdateSaleAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _context.Warehouses.ToListAsync(), "Id", "Name");
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "CustomerId", "Name");


            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var saleViewModel = await _saleService.GetSaleByIdAsync(id);
            if (saleViewModel == null)
            {
                return NotFound();
            }

            return View(saleViewModel);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null) return NotFound();

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _saleService.DeleteSaleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
