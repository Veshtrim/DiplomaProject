using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public IActionResult Index()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            return View(warehouses);
        }

        public IActionResult Details(int id)
        {
            var warehouse = _warehouseService.GetWarehouseById(id);
            if (warehouse == null) return NotFound();

            return View(warehouse);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WarehouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _warehouseService.CreateWarehouse(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var warehouse = _warehouseService.GetWarehouseById(id);
            if (warehouse == null) return NotFound();

            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WarehouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _warehouseService.UpdateWarehouse(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var warehouse = _warehouseService.GetWarehouseById(id);
            if (warehouse == null) return NotFound();

            return View(warehouse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _warehouseService.DeleteWarehouse(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
