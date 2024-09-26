using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<WarehouseViewModel> GetAllWarehouses()
        {
            return _context.Warehouses.Select(w => new WarehouseViewModel
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location
            }).ToList();
        }

        public WarehouseViewModel GetWarehouseById(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null) return null;

            return new WarehouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };
        }

        public void CreateWarehouse(WarehouseViewModel model)
        {
            var warehouse = new Warehouse
            {
                Name = model.Name,
                Location = model.Location
            };

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }

        public void UpdateWarehouse(WarehouseViewModel model)
        {
            var warehouse = _context.Warehouses.Find(model.Id);
            if (warehouse == null) return;

            warehouse.Name = model.Name;
            warehouse.Location = model.Location;

            _context.SaveChanges();
        }

        public void DeleteWarehouse(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null) return;

            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();
        }
    }

}
