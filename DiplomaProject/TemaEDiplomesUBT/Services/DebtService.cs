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
    public class DebtService : IDebtService
    {
        private readonly ApplicationDbContext _context;

        public DebtService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DebtViewModel>> GetUnpaidDebtsBySupplierAsync(int supplierId)
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Where(p => p.SupplierId == supplierId && !p.IsPaid)
                .ToListAsync();

            var debts = new List<DebtViewModel>();

            foreach (var purchase in purchases)
            {
                debts.Add(new DebtViewModel
                {
                    PurchaseId = purchase.PurchaseId,
                    DocumentNumber = purchase.PurchaseDocumentNumber,
                    Amount = purchase.TotalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = " ",
                    SupplierName = purchase.Supplier.Name,
                    IsPaid = purchase.IsPaid
                });
            }

            return debts;
        }

        public async Task MarkDebtAsPaidAsync(int debtId)
        {
            var debt = await _context.Debts.FindAsync(debtId);

            if (debt == null)
            {
                throw new ArgumentException("Debt not found.");
            }

            debt.IsPaid = true;

            var purchase = await _context.Purchases.FindAsync(debt.PurchaseId);
            if (purchase != null)
            {
                purchase.IsPaid = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddDebtAsync(DebtViewModel model)
        {
            var debt = new Debt
            {
                PurchaseId = model.PurchaseId,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentMethod = model.PaymentMethod,
                SupplierId = model.SupplierId,
                IsPaid = model.IsPaid
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDebtAsync(DebtViewModel model)
        {
            var debt = await _context.Debts.FindAsync(model.DebtId);

            if (debt == null)
            {
                throw new ArgumentException("Debt not found.");
            }

            debt.Amount = model.Amount;
            debt.PaymentDate = model.PaymentDate;
            debt.PaymentMethod = model.PaymentMethod;
            debt.IsPaid = model.IsPaid;

            _context.Debts.Update(debt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDebtAsync(int debtId)
        {
            var debt = await _context.Debts.FindAsync(debtId);

            if (debt == null)
            {
                throw new ArgumentException("Debt not found.");
            }

            _context.Debts.Remove(debt);
            await _context.SaveChangesAsync();
        }
    }
}
