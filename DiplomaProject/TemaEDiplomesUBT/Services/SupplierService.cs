using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierViewModel>> GetAllSupplierAsync()
        {
            return await _context.Suppliers
                .Select(s => new SupplierViewModel
                {
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    ContactNumber = s.ContactNumber,
                    Email = s.Email,
                    Address = s.Address
                }).ToListAsync();
        }

        public async Task<SupplierViewModel> GetSupplierByIdAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return null;

            return new SupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                ContactNumber = supplier.ContactNumber,
                Email = supplier.Email,
                Address = supplier.Address
            };
        }

        public async Task CreateSupplierAsync(SupplierViewModel supplierViewModel)
        {
            var supplier = new Supplier
            {
                Name = supplierViewModel.Name,
                ContactNumber = supplierViewModel.ContactNumber,
                Email = supplierViewModel.Email,
                Address = supplierViewModel.Address
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(SupplierViewModel supplierViewModel)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierViewModel.SupplierId);
            if (supplier == null) return;

            supplier.Name = supplierViewModel.Name;
            supplier.ContactNumber = supplierViewModel.ContactNumber;
            supplier.Email = supplierViewModel.Email;
            supplier.Address = supplierViewModel.Address;

            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}
