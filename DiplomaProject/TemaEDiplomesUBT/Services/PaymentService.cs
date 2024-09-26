using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemaEDiplomesUBT.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentViewModel>> GetUnpaidPaymentsByCustomerAsync(int customerId)
        {
   
            Console.WriteLine($"Fetching unpaid sales for customer ID: {customerId}");

       
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Where(s => s.CustomerId == customerId && !s.IsPaid)
                .ToListAsync();

           
            Console.WriteLine($"Number of unpaid sales found: {sales.Count}");

           
            var payments = new List<PaymentViewModel>();

           
            foreach (var sale in sales)
            {
                payments.Add(new PaymentViewModel
                {
                    SaleId = sale.SaleId,
                    DocumentNumber = sale.DocumentNumber,
                    Amount = sale.TotalAmount, 
                    PaymentDate = DateTime.Now,  
                    PaymentMethod = " ", 
                    CustomerName = sale.Customer.Name,
                    IsPaid = sale.IsPaid
                });
            }

            return payments;
        }



        public async Task MarkPaymentAsPaidAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);

            if (payment == null)
            {
                throw new ArgumentException("Payment not found.");
            }

            payment.IsPaid = true;

            // Update related Sale's IsPaid status
            var sale = await _context.Sales.FindAsync(payment.SaleId);
            if (sale != null)
            {
                sale.IsPaid = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddPaymentAsync(PaymentViewModel model)
        {
            var payment = new Payment
            {
                SaleId = model.SaleId,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentMethod = model.PaymentMethod,
                CustomerId = model.CustomerId,
                IsPaid = model.IsPaid
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
