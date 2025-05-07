using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class ExcelExportHelper
{
    public static byte[] ExportToExcel<T>(List<T> data, string sheetName = "Sheet1")
    {
        if (data == null || !data.Any())
            throw new ArgumentException("No data available for export");

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Get all public instance properties of the object
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToArray();

            int row = 1;

            // Add Headers
            for (int col = 0; col < properties.Length; col++)
            {
                worksheet.Cell(row, col + 1).Value = properties[col].Name; // Set header to property name
                worksheet.Cell(row, col + 1).Style.Font.Bold = true; // Make header bold
                worksheet.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; // Center header text
            }

            // Add Data
            foreach (var item in data)
            {
                row++;
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item, null);
                    worksheet.Cell(row, col + 1).Value = value?.ToString() ?? ""; // Set cell value and handle null
                    worksheet.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; // Center data
                }
            }

            // Adjust columns to fit content
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray(); // Return file as byte array
            }
        }
    }
}
