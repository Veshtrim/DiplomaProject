using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Services.IServices;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TemaEDiplomesUBT.Controllers
{
    [Authorize]
    public class DebtController : Controller
    {
        private readonly IDebtService _debtService;
        private readonly IPurchaseService _purchaseService;
        private readonly ISupplierService _supplierService;
        private readonly IWorkflowHost _workflowHost;

        public DebtController(IDebtService debtService, IPurchaseService purchaseService, ISupplierService supplierService, IWorkflowHost workflowHost)
        {
            _debtService = debtService;
            _purchaseService = purchaseService;
            _supplierService = supplierService;
            _workflowHost = workflowHost;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllSupplierAsync();

            var model = new DebtViewModel
            {
                Suppliers = new SelectList(suppliers, "SupplierId", "Name")
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnpaidDocuments(int selectedSupplierId)
        {
            var unpaidDebts = await _debtService.GetUnpaidDebtsBySupplierAsync(selectedSupplierId);

            if (unpaidDebts == null || !unpaidDebts.Any())
            {
                return PartialView("_UnpaidDebtsPartial", new List<DebtViewModel>());
            }

            foreach (var document in unpaidDebts)
            {
                document.SupplierId = selectedSupplierId;
                document.PurchaseId = document.PurchaseId;
            }

            return PartialView("_UnpaidDebtsPartial", unpaidDebts);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessDebt(List<DebtViewModel> model)
        {
            if (model == null || !model.Any())
            {
                TempData["Error"] = "No documents selected for payment.";
                return RedirectToAction(nameof(Index));
            }

            var selectedDebts = model.Where(m => m.IsSelectedForPayment).ToList();

            if (!selectedDebts.Any())
            {
                TempData["Error"] = "No documents selected for payment.";
                return RedirectToAction(nameof(Index));
            }

            var workflowData = new DebtPaymentWorkflowData
            {
                PurchaseIds = selectedDebts.Select(x => x.PurchaseId).ToList(),
                PurchaseId = selectedDebts.First().PurchaseId,
                SupplierId = selectedDebts.First().SupplierId,
                PaymentMethod = selectedDebts.First().PaymentMethod,
                TotalAmount = selectedDebts.Sum(x => x.Amount)
            };

            var workflowId = await _workflowHost.StartWorkflow("DebtWorkflow", workflowData);
            var result = await WaitForWorkflowToComplete(workflowId);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            TempData["Success"] = "Debts processed successfully.";
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

        private string GenerateDebtDocumentNumber()
        {
            return $"DBT-{DateTime.Now.Ticks}";
        }

        public class WorkflowResult
        {
            public bool IsSuccessful { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
