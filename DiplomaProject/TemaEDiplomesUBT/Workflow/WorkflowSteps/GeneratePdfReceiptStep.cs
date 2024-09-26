using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using TemaEDiplomesUBT.Models.ViewModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using System;
using System.Collections.Generic;
using System.IO;

public class GeneratePdfReceiptStep : StepBody
{
    public string DocumentNumber { get; set; }
    public List<SaleDetailViewModel> SaleDetails { get; set; }
    public bool IsUpdate { get; set; }
    public string CustomerName { get; set; } 

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        try
        {
            string baseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Receipts");

            if (!Directory.Exists(baseFolderPath))
            {
                Directory.CreateDirectory(baseFolderPath);
            }

            string folderPath = Path.Combine(baseFolderPath, DocumentNumber);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"Receipt_{DocumentNumber}.pdf");

            if (IsUpdate)
            {
                filePath = Path.Combine(folderPath, $"Receipt_{DocumentNumber}-updated.pdf");
            }

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
                var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
                var font = new XFont("Arial", 10, XFontStyle.Regular);
                var boldFont = new XFont("Arial", 10, XFontStyle.Bold);

                double yPoint = 40;
                double margin = 40;

                gfx.DrawString("Company Name", titleFont, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString($"Document Number: {DocumentNumber}", headerFont, XBrushes.Black, new XRect(margin, yPoint, page.Width - margin * 2, page.Height), XStringFormats.TopLeft);
                yPoint += 30;
                gfx.DrawString($"Customer: {CustomerName}", headerFont, XBrushes.Black, new XRect(margin, yPoint, page.Width - margin * 2, page.Height), XStringFormats.TopLeft);
                yPoint += 20;
                gfx.DrawString($"Date: {DateTime.Now:MMMM dd, yyyy}", font, XBrushes.Black, new XRect(margin, yPoint, page.Width - margin * 2, page.Height), XStringFormats.TopLeft);
                yPoint += 20;

                gfx.DrawLine(XPens.Black, margin, yPoint, page.Width - margin, yPoint);
                yPoint += 10;

                gfx.DrawString("Product Name", boldFont, XBrushes.Black, new XRect(margin, yPoint, 150, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("Warehouse Name", boldFont, XBrushes.Black, new XRect(margin + 150, yPoint, 150, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("Quantity", boldFont, XBrushes.Black, new XRect(margin + 300, yPoint, 100, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("Price", boldFont, XBrushes.Black, new XRect(margin + 400, yPoint, 100, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("Subtotal", boldFont, XBrushes.Black, new XRect(margin + 500, yPoint, 100, page.Height), XStringFormats.TopLeft);
                yPoint += 20;

                decimal totalAmount = 0;
                foreach (var detail in SaleDetails)
                {
                    string productName = detail.ProductName ?? "N/A";
                    string warehouseName = detail.WarehouseName ?? "N/A";
                    decimal subtotal = detail.Quantity * detail.Price;
                    totalAmount += subtotal;

                    gfx.DrawString(productName, font, XBrushes.Black, new XRect(margin, yPoint, 150, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString(warehouseName, font, XBrushes.Black, new XRect(margin + 150, yPoint, 150, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString(detail.Quantity.ToString(), font, XBrushes.Black, new XRect(margin + 300, yPoint, 100, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString(detail.Price.ToString("C2"), font, XBrushes.Black, new XRect(margin + 400, yPoint, 100, page.Height), XStringFormats.TopLeft);
                    gfx.DrawString(subtotal.ToString("C2"), font, XBrushes.Black, new XRect(margin + 500, yPoint, 100, page.Height), XStringFormats.TopLeft);

                    yPoint += 20;

                 
                    gfx.DrawLine(XPens.LightGray, margin, yPoint, page.Width - margin, yPoint);
                    yPoint += 10;
                }

                gfx.DrawLine(XPens.Black, margin, yPoint, page.Width - margin, yPoint);
                yPoint += 10;

                gfx.DrawString($"Total: {totalAmount.ToString("C2")}", boldFont, XBrushes.Black, new XRect(margin, yPoint, page.Width - margin * 2, page.Height), XStringFormats.TopLeft);

              
                document.Save(filePath);
            }

            return ExecutionResult.Next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating PDF receipt: {ex.Message}");
            return ExecutionResult.Persist(ex);
        }
    }
}
