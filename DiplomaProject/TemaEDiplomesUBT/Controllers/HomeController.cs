using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalSales = _context.Sales.Count(),
                TotalPurchases = _context.Purchases.Count(),
                TotalDebts = _context.Purchases.Count(p => !p.IsPaid),
                TotalPayments = _context.Payments.Count(),

                TotalAmountPaidOnSales = _context.Sales
                    .Where(s => s.IsPaid)
                    .Sum(s => s.TotalAmount),

                TotalAmountPaidOnPurchases = _context.Purchases
                    .Where(p => p.IsPaid)
                    .Sum(p => p.TotalAmount),

                Warehouses = _context.Warehouses.Select(w => new WarehouseViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location
                }).ToList(),

                StockItems = _context.Stocks
                    .Include(s => s.Product)
                    .Include(s => s.Warehouse)
                    .Select(s => new StockViewModel
                    {
                        ProductId = s.ProductId,
                        ProductName = s.Product.Name,
                        WarehouseId = s.WarehouseId,
                        WarehouseName = s.Warehouse.Name,
                        Quantity = s.Quantity
                    }).ToList()
            };

            return View(dashboardViewModel);
        }
    }
}
