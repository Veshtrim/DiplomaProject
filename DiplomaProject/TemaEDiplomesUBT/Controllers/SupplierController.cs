using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Services.IServices;
using Microsoft.AspNetCore.Authorization;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var Supplier = await _supplierService.GetAllSupplierAsync();
            return View(Supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.CreateSupplierAsync(supplierViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(supplierViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplierViewModel = await _supplierService.GetSupplierByIdAsync(id);
            if (supplierViewModel == null)
            {
                return NotFound();
            }
            return View(supplierViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel supplierViewModel)
        {
            if (id != supplierViewModel.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _supplierService.UpdateSupplierAsync(supplierViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(supplierViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplierViewModel = await _supplierService.GetSupplierByIdAsync(id);
            if (supplierViewModel == null)
            {
                return NotFound();
            }
            return View(supplierViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplierViewModel = await _supplierService.GetSupplierByIdAsync(id);
            if (supplierViewModel == null)
            {
                return NotFound();
            }
            return View(supplierViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _supplierService.DeleteSupplierAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
