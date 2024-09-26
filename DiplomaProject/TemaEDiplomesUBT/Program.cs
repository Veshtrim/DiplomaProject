using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Services.IServices;
using TemaEDiplomesUBT.Services;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow;
using WorkflowCore.Interface;
using TemaEDiplomesUBT.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using TemaEDiplomesUBT.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<SendBerrySettings>(builder.Configuration.GetSection("SendBerrySettings"));

builder.Services.AddWorkflow();
builder.Services.AddHttpClient();
builder.Services.AddScoped<CheckStockStep>();
builder.Services.AddScoped<UpdateStockStep>();
builder.Services.AddScoped<CustomLogStep>();
builder.Services.AddScoped<UpdateStockForSaleStep>();
builder.Services.AddScoped<AdjustStockForUpdatedSaleStep>();
builder.Services.AddScoped<RestoreStockForDeletedSaleStep>();
builder.Services.AddScoped<GeneratePdfReceiptStep>();
builder.Services.AddScoped<ProcessPaymentStep>();
builder.Services.AddScoped<UpdateSaleInPayment>();
builder.Services.AddScoped<UpdateStockForPurchaseStep>();
builder.Services.AddScoped<AdjustStockForUpdatedPurchaseStep>();
builder.Services.AddScoped<GeneratePdfReceiptPurchaseStep>();
builder.Services.AddScoped<ProcessPaymentStep>();
builder.Services.AddScoped<UpdatePurchaseInDebt>();
builder.Services.AddScoped<ProcessDebtStep>();
builder.Services.AddScoped<ContantSuppliers>();
builder.Services.AddScoped<SendSmsStep>();


builder.Services.AddTransient<SalesAndPurchasesEmail>();
builder.Services.AddTransient<CheckStockAvailabilityStep>();
builder.Services.AddTransient<DeductStockStep>();
builder.Services.AddTransient<AddStockToDestinationStep>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IDebtService, DebtService>();
builder.Services.AddScoped<IDayOffService, DayOffService>();

builder.Services.AddHostedService<ScheduledEmailService>();
builder.Services.AddHostedService<ScheduledDayOffCheckService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
var app = builder.Build();
var host = app.Services.GetRequiredService<IWorkflowHost>();
host.RegisterWorkflow<StockAdjustmentWorkflow, StockAdjustmentData>();
host.RegisterWorkflow<StockTransferWorkflow, StockTransferData>();
host.RegisterWorkflow<SaleWorkflow, SaleWorkflowData>();
host.RegisterWorkflow<UpdateSaleWorkflow, SaleWorkflowData>();
host.RegisterWorkflow<DeleteSaleWorkflow, SaleWorkflowData>();
host.RegisterWorkflow<PaymentWorkflow, PaymentWorkflowData>();
host.RegisterWorkflow<PurchaseWorkflow, PurchaseWorkflowData>();
host.RegisterWorkflow<UpdatePurchaseWorkflow, PurchaseWorkflowData>();
host.RegisterWorkflow<DebtWorkflow, DebtPaymentWorkflowData>();
host.RegisterWorkflow<NoStockSupplier, SaleWorkflowData>();
host.RegisterWorkflow<SalesAndPurchasesEmailWorkflow>();
host.RegisterWorkflow<DayOffWorkflow>();



host.Start();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
