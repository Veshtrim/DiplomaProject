using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerViewModel>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address
                }).ToListAsync();
        }
        public async Task<List<Sale>> GetUnpaidDocumentsByCustomerAsync(int customerId)
        {
            return await _context.Sales
                .Where(s => s.CustomerId == customerId && !s.IsPaid)
                .ToListAsync();
        }
        public async Task<CustomerViewModel> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return null;
            }

            return new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        public async Task AddCustomerAsync(CustomerViewModel model)
        {
            var customer = new Customer
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(CustomerViewModel model)
        {
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            customer.Name = model.Name;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

}
