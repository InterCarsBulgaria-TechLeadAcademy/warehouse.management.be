using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using ZXing;
using ZXing.Common;

namespace WarehouseManagement.Core.Services
{
    public class PDFService : IPDFService
    {
        public byte[] GenerateBarcodePdfReport(DeliveryPDFModelDto model)
        {
            using (var pdfStream = new MemoryStream())
            {
                // Create the PDF document
                var writer = new PdfWriter(pdfStream);
                var pdf = new PdfDocument(writer);
                var doc = new Document(pdf);

                // Load fonts
                PdfFont headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Header Table
                var headerTable = new Table(
                    UnitValue.CreatePercentArray(new float[] { 1, 1, 1 })
                ).UseAllAvailableWidth();

                headerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.VendorName)
                                .SetFont(headerFont)
                                .SetFontSize(20)
                                .SetFontColor(ColorConstants.WHITE)
                        )
                        .SetBackgroundColor(ColorConstants.BLACK)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                headerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("ID: " + model.Id).SetFont(headerFont).SetFontSize(20))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                headerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.DeliveredOn.ToString("dd/MM/yyyy"))
                                .SetFont(headerFont)
                                .SetFontSize(20)
                        )
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                headerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph(" ").SetFont(headerFont).SetFontSize(20))
                        .SetBackgroundColor(ColorConstants.BLACK)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                headerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.VendorSystemNumber)
                                .SetFont(headerFont)
                                .SetFontSize(20)
                        )
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                headerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph($"Zone: {string.Join(", ", model.Zones)}")
                                .SetFont(normalFont)
                                .SetFontSize(12)
                        )
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                headerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Delivery No:").SetFont(headerFont).SetFontSize(20))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                var systemNumbers = model.SystemNumber.Split(
                    " | ",
                    StringSplitOptions.RemoveEmptyEntries
                );
                headerTable.AddCell(
                    new Cell(1, 2)
                        .Add(
                            new Paragraph(string.Join(",", systemNumbers))
                                .SetFont(normalFont)
                                .SetFont(normalFont)
                                .SetFontSize(12)
                        )
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                doc.Add(headerTable);

                // Main Table
                var mainTable = new Table(
                    UnitValue.CreatePercentArray(new float[] { 1, 2.5f, 3.5f })
                ).UseAllAvailableWidth();

                mainTable.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("No:").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                mainTable.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("Reception No:").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );
                mainTable.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("Barcode").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                int num = 1;

                var receptionNumbers = model
                    .ReceptionNumber.Split(" | ", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                foreach (var number in receptionNumbers)
                {
                    // Generate Barcode
                    var barcodeWriter = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new EncodingOptions { Height = 50, Width = 350 }
                    };

                    var pixelData = barcodeWriter.Write(number);

                    using (
                        var bitmap = new Bitmap(
                            pixelData.Width,
                            pixelData.Height,
                            PixelFormat.Format32bppRgb
                        )
                    )
                    {
                        var bitmapData = bitmap.LockBits(
                            new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format32bppRgb
                        );
                        System.Runtime.InteropServices.Marshal.Copy(
                            pixelData.Pixels,
                            0,
                            bitmapData.Scan0,
                            pixelData.Pixels.Length
                        );
                        bitmap.UnlockBits(bitmapData);

                        using (var ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            var barcodeImageData = ImageDataFactory.Create(ms.ToArray());
                            var barcodeImage = new iText.Layout.Element.Image(barcodeImageData);

                            // Add numeration
                            mainTable.AddCell(
                                new Cell()
                                    .Add(
                                        new Paragraph(num.ToString())
                                            .SetFont(headerFont)
                                            .SetFontSize(16)
                                    )
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            );
                            num++;

                            // Add reception number
                            mainTable.AddCell(
                                new Cell()
                                    .Add(new Paragraph(number).SetFont(normalFont).SetFontSize(12))
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            );

                            // Add barcode image
                            mainTable.AddCell(
                                new Cell()
                                    .Add(barcodeImage)
                                    .SetPadding(10)
                                    .SetBackgroundColor(ColorConstants.YELLOW)
                            );
                        }
                    }
                }

                doc.Add(mainTable);

                // Footer Table
                var footerTable = new Table(
                    UnitValue.CreatePercentArray(new float[] { 1, 1, 1 })
                ).UseAllAvailableWidth();

                footerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Pallets").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                );
                footerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Packages").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                );
                footerTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Pieces").SetFont(headerFont).SetFontSize(16))
                        .SetTextAlignment(TextAlignment.CENTER)
                );

                footerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.Pallets.ToString())
                                .SetFont(headerFont)
                                .SetFontSize(16)
                        )
                        .SetTextAlignment(TextAlignment.CENTER)
                );
                footerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.Packages.ToString())
                                .SetFont(headerFont)
                                .SetFontSize(16)
                        )
                        .SetTextAlignment(TextAlignment.CENTER)
                );
                footerTable.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph(model.Pieces.ToString())
                                .SetFont(headerFont)
                                .SetFontSize(16)
                        )
                        .SetTextAlignment(TextAlignment.CENTER)
                );

                doc.Add(footerTable);

                // Close the document
                doc.Close();

                // Return the generated PDF as byte array
                return pdfStream.ToArray();
            }
        }
    }
}
