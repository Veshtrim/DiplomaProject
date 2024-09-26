using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Controllers
{
    public class DayOffController : Controller
    {
        private readonly IDayOffService _dayOffService;

        public DayOffController(IDayOffService dayOffService)
        {
            _dayOffService = dayOffService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dayOffService.GetAllAsync();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DayOffViewModel dayOffViewModel)
        {
            if (ModelState.IsValid)
            {
                await _dayOffService.AddAsync(dayOffViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(dayOffViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dayOff = await _dayOffService.GetByIdAsync(id);
            if (dayOff == null)
            {
                return NotFound();
            }
            return View(dayOff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DayOffViewModel dayOffViewModel)
        {
            if (id != dayOffViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dayOffService.UpdateAsync(dayOffViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(dayOffViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dayOff = await _dayOffService.GetByIdAsync(id);
            if (dayOff == null)
            {
                return NotFound();
            }
            return View(dayOff);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dayOff = await _dayOffService.GetByIdAsync(id);
            if (dayOff == null)
            {
                return NotFound();
            }
            return View(dayOff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dayOffService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
