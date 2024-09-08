using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using ZXing;
using ZXing.Common;

using static WarehouseManagement.Common.GeneralApplicationConstants;

namespace WarehouseManagement.Core.Services
{
    public class PDFService : IPDFService
    {
        private readonly PdfFont headerFont;
        private readonly PdfFont normalFont;

        public PDFService()
        {
            headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        public byte[] GenerateBarcodePdfReport(DeliveryPDFModelDto model)
        {
            using var pdfStream = new MemoryStream();

            // Create the PDF document
            var writer = new PdfWriter(pdfStream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            Table headerTable = CreateHeaderTable(model, headerFont);
            Table mainTable = CreateMainTable();
            Table footerTable = CreateFooterTable(model);

            CreateMainTableContent(model, mainTable);

            doc.Add(headerTable);
            doc.Add(mainTable);
            doc.Add(footerTable);

            doc.Close();

            // Return the generated PDF as byte array
            return pdfStream.ToArray();
        }

        private Table CreateHeaderTable(DeliveryPDFModelDto model, PdfFont headerFont)
        {
            var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1 })).UseAllAvailableWidth();

            headerTable.AddCell(new Cell()
                .Add(new Paragraph(model.VendorName)
                    .SetFont(headerFont)
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(ColorConstants.BLACK)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph("ID: " + model.Id).SetFont(headerFont).SetFontSize(20))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph(model.DeliveredOn.ToString("dd/MM/yyyy"))
                    .SetFont(headerFont).SetFontSize(20))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph(" ").SetFont(headerFont).SetFontSize(20))
                .SetBackgroundColor(ColorConstants.BLACK)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph(model.VendorSystemNumber)
                    .SetFont(headerFont)
                    .SetFontSize(20))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph($"Zone: {string.Join(", ", model.Zones)}")
                    .SetFont(normalFont)
                    .SetFontSize(12))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell()
                .Add(new Paragraph("Delivery No:").SetFont(headerFont).SetFontSize(20))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            var systemNumbers = model.SystemNumber.Split(
                PipeSeparator,
                StringSplitOptions.RemoveEmptyEntries
            );

            headerTable.AddCell(new Cell(1, 2)
                .Add(new Paragraph(string.Join(",", systemNumbers))
                    .SetFont(normalFont)
                    .SetFont(normalFont)
                .SetFontSize(12))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            return headerTable;
        }

        private Table CreateMainTable()
        {
            var mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2.5f, 3.5f })).UseAllAvailableWidth();

            mainTable.AddHeaderCell(new Cell()
                .Add(new Paragraph("No:").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            mainTable.AddHeaderCell(new Cell()
                .Add(new Paragraph("Reception No:").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            mainTable.AddHeaderCell(new Cell()
                .Add(new Paragraph("Barcode").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            return mainTable;
        }

        private Table CreateFooterTable(DeliveryPDFModelDto model)
        {
            var footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1 })).UseAllAvailableWidth();

            footerTable.AddCell(new Cell()
                .Add(new Paragraph("Pallets").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            footerTable.AddCell(new Cell()
                .Add(new Paragraph("Packages").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            footerTable.AddCell(new Cell()
                .Add(new Paragraph("Pieces").SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            footerTable.AddCell(new Cell()
                .Add(new Paragraph(model.Pallets.ToString())
                    .SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            footerTable.AddCell(new Cell()
                .Add(new Paragraph(model.Packages.ToString())
                    .SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            footerTable.AddCell(new Cell()
                .Add(new Paragraph(model.Pieces.ToString())
                    .SetFont(headerFont).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER));

            return footerTable;
        }

        private void CreateMainTableContent(DeliveryPDFModelDto model, Table mainTable)
        {
            int rowNum = 1;
            var receptionNumbers = model.ReceptionNumber.Split(PipeSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var receptionNumber in receptionNumbers)
            {
                Image barcodeImageElement = GenerateBarcodeImage(receptionNumber);

                // Add numeration
                mainTable.AddCell(new Cell()
                    .Add(new Paragraph(rowNum.ToString())
                        .SetFont(headerFont)
                        .SetFontSize(16))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));
                rowNum++;

                // Add reception number
                mainTable.AddCell(new Cell()
                    .Add(new Paragraph(receptionNumber).SetFont(normalFont).SetFontSize(12))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));

                // Add barcode image
                mainTable.AddCell(new Cell()
                    .Add(barcodeImageElement)
                    .SetPadding(10));
            }
        }

        private Image GenerateBarcodeImage(string number)
        {
            var barcodeWriter = new ZXing.ImageSharp.BarcodeWriter<Rgba32>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Height = 50, Width = 350 }
            };

            using var barcodeImage = barcodeWriter.Write(number);
            using var ms = new MemoryStream();

            barcodeImage.Save(ms, new PngEncoder());
            var barcodeImageData = ImageDataFactory.Create(ms.ToArray());
            var barcodeImageElement = new Image(barcodeImageData);

            return barcodeImageElement;
        }
    }
}