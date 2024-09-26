using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using TemaEDiplomesUBT.Services;
using WorkflowCore.Models;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using Microsoft.AspNetCore.Authorization;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ISaleService _saleService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowHost _workflowHost;
        public readonly ApplicationDbContext _context;

        public PaymentController(IPaymentService paymentService, ISaleService saleService, ICustomerService customerService, IWorkflowHost workflowHost,ApplicationDbContext context)
        {
            _paymentService = paymentService;
            _saleService = saleService;
            _customerService = customerService;
            _workflowHost = workflowHost;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();

            var model = new PaymentViewModel
            {
                Customers = new SelectList(customers, "CustomerId", "DisplayName")
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnpaidDocuments(int selectedCustomerId)
        {
            var unpaidDocuments = await _paymentService.GetUnpaidPaymentsByCustomerAsync(selectedCustomerId);

            if (unpaidDocuments == null || !unpaidDocuments.Any())
            {
                return PartialView("_UnpaidDocumentsPartial", new List<PaymentViewModel>());
            }

           
            foreach (var document in unpaidDocuments)
            {
                document.CustomerId = selectedCustomerId; 
                document.SaleId = document.SaleId; 
            }

            return PartialView("_UnpaidDocumentsPartial", unpaidDocuments);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(List<PaymentViewModel> model)
        {
            if (model == null || !model.Any())
            {
                TempData["Error"] = "No documents selected for payment.";
                return RedirectToAction(nameof(Index));
            }

            var selectedSales = model.Where(m => m.IsSelectedForPayment).ToList();

            if (!selectedSales.Any())
            {
                TempData["Error"] = "No documents selected for payment.";
                return RedirectToAction(nameof(Index));
            }

            var workflowData = new PaymentWorkflowData
            {
                SaleIds = selectedSales.Select(x => x.SaleId).ToList(),
                SaleId=selectedSales.First().SaleId,
                CustomerId = selectedSales.First().CustomerId, 
                PaymentMethod = selectedSales.First().PaymentMethod, 
                TotalAmount = selectedSales.Sum(x => x.Amount)
            };

          
            var workflowId = await _workflowHost.StartWorkflow("PaymentWorkflow", workflowData);
            var result = await WaitForWorkflowToComplete(workflowId);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
            TempData["Success"] = "Payments processed successfully.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<WorkflowResult> WaitForWorkflowToComplete(string workflowId)
        {
            while (true)
            {
                var workflowInstance = await _workflowHost.PersistenceStore.GetWorkflowInstance(workflowId);

                if (workflowInstance.Status == WorkflowStatus.Complete)
                {
                    return new WorkflowResult { IsSuccessful = true };
                }

                if (workflowInstance.Status == WorkflowStatus.Terminated || workflowInstance.Status == WorkflowStatus.Suspended)
                {
                    var errorMessage = workflowInstance.Data as string;
                    return new WorkflowResult { IsSuccessful = false, ErrorMessage = errorMessage };
                }

                await Task.Delay(1000);
            }
        }
        private string GeneratePaymentDocumentNumber()
        {
           
            return $"PMT-{DateTime.Now.Ticks}";
        }
        public IActionResult ListPaymentDocuments() 
        {
            var paymentDocuments = _context.Payments
            .Select(p => new PaymentViewModel
            {
                PaymentId = p.PaymentId,
                SaleId = p.SaleId,
               
                DocumentNumber = p.DocumentNumber,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                CustomerName = p.Customer.Name,
                IsPaid = p.IsPaid
            })
        .ToList();

            return View(paymentDocuments);
        }
        public class WorkflowResult
        {
            public bool IsSuccessful { get; set; }
            public string ErrorMessage { get; set; }
        }

    }
}
