using Microsoft.AspNetCore.Identity.UI.Services;
using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class SalesAndPurchasesEmail : StepBody
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public SalesAndPurchasesEmail(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var todayDate = DateTime.Today;
            var totalSales = _context.Sales.Where(d => d.SaleDate == todayDate).Sum(s => s.TotalAmount);
            var totalPurchases = _context.Purchases.Where(d => d.PurchaseDate == todayDate).Sum(s => s.TotalAmount);

            // Create the "sexy" email template
            string emailBody = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 50px auto;
                        background-color: #ffffff;
                        padding: 20px;
                        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
                        border-radius: 8px;
                    }}
                    .header {{
                        text-align: center;
                        padding-bottom: 20px;
                    }}
                    .header h1 {{
                        color: #333;
                        font-size: 24px;
                    }}
                    .content {{
                        text-align: left;
                        font-size: 16px;
                        line-height: 1.6;
                    }}
                    .content p {{
                        margin-bottom: 20px;
                        color: #555;
                    }}
                    .summary {{
                        background-color: #f0f0f0;
                        padding: 10px;
                        border-radius: 5px;
                        margin-bottom: 20px;
                        text-align: center;
                    }}
                    .summary h2 {{
                        margin: 0;
                        color: #1e87f0;
                    }}
                    .footer {{
                        text-align: center;
                        font-size: 14px;
                        color: #888;
                        padding-top: 10px;
                    }}
                    .button {{
                        background-color: #1e87f0;
                        color: white;
                        padding: 10px 20px;
                        text-decoration: none;
                        border-radius: 5px;
                        display: inline-block;
                    }}
                    .button:hover {{
                        background-color: #166bb3;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Daily Sales and Purchases Summary</h1>
                    </div>
                    <div class='content'>
                        <p>Hello,</p>
                        <p>Here is the summary of today's sales and purchases:</p>

                        <div class='summary'>
                            <h2>Total Sales: ${totalSales:N2}</h2>
                            <h2>Total Purchases: ${totalPurchases:N2}</h2>
                        </div>

                        <p>Thank you for keeping track of your daily transactions.</p>

                        <a href='#' class='button'>View Detailed Report</a>
                    </div>
                    <div class='footer'>
                        <p>&copy; 2024 Your Company. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            _emailSender.SendEmailAsync("veshtrimm1@gmail.com", "Total Daily Sales and Purchases Summary", emailBody);

            return ExecutionResult.Next();
        }
    }
}
