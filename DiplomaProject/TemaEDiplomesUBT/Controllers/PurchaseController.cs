using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ApplicationDbContext _context;

        public PurchaseController(IPurchaseService purchaseService, ApplicationDbContext context)
        {
            _purchaseService = purchaseService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return View(purchases);
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
            var suppliers = _context.Suppliers.ToList();

            if (products == null || !products.Any())
            {
                ModelState.AddModelError("", "No products available.");
                return View(new PurchaseViewModel());
            }

            if (warehouses == null || !warehouses.Any())
            {
                ModelState.AddModelError("", "No warehouses available.");
                return View(new PurchaseViewModel());
            }

            if (suppliers == null || !suppliers.Any())
            {
                ModelState.AddModelError("", "No suppliers available.");
                return View(new PurchaseViewModel());
            }

            ViewBag.Products = products;
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");

            return View(new PurchaseViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _purchaseService.CreatePurchaseAsync(model);

                if (!result.IsSuccessful)
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Create));
                }

                return RedirectToAction(nameof(Index));
            }
           
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _context.Warehouses.ToListAsync(), "Id", "Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Name");

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var purchaseViewModel = await _purchaseService.GetPurchaseByIdAsync(id);

            if (purchaseViewModel == null)
            {
                return NotFound();
            }

            var products = _context.Products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();
            var suppliers = _context.Suppliers.ToList();
            if (suppliers == null || !suppliers.Any())
            {
                ModelState.AddModelError("", "No suppliers available.");
                return View(new PurchaseViewModel());
            }
            var warehouses = _context.Warehouses.ToList();
            ViewBag.Products = products;
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
            return View(purchaseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _purchaseService.UpdatePurchaseAsync(model);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _context.Warehouses.ToListAsync(), "Id", "Name");
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchase == null) return NotFound();

            return View(purchase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _purchaseService.DeletePurchaseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var purchaseViewModel = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchaseViewModel == null)
            {
                return NotFound();
            }

            return View(purchaseViewModel);
        }
    }
}
