using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Apzon.Commons.Helper;

public static class ExcelHelper
{
	public static DataTable GetDataTableFromExcelWorkSheet(ExcelWorksheet sheet)
	{
		DataTable dataTable = new DataTable();
		bool flag = true;
		int num = 0;
		foreach (ExcelRangeBase item in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
		{
			dataTable.Columns.Add(flag ? (item.Text + num) : $"Column {item.Start.Column}{num}");
			num++;
		}
		for (int i = ((!flag) ? 1 : 2); i <= sheet.Dimension.End.Row; i++)
		{
			ExcelRange excelRange = sheet.Cells[i, 1, i, sheet.Dimension.End.Column];
			DataRow dataRow = dataTable.NewRow();
			foreach (ExcelRangeBase item2 in excelRange)
			{
				dataRow[item2.Start.Column - 1] = item2.Value;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static DataTable GetAllDataFromExcel(string path, bool hasHeader = true, int colfieldName = 1)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];
		DataTable dataTable = new DataTable();
		foreach (ExcelRangeBase item in excelWorksheet.Cells[colfieldName, 1, colfieldName, excelWorksheet.Dimension.End.Column])
		{
			dataTable.Columns.Add(hasHeader ? item.Text : $"Column {item.Start.Column}");
		}
		for (int i = 1; i <= excelWorksheet.Dimension.End.Row; i++)
		{
			ExcelRange excelRange = excelWorksheet.Cells[i, 1, i, excelWorksheet.Dimension.End.Column];
			DataRow dataRow = dataTable.NewRow();
			foreach (ExcelRangeBase item2 in excelRange)
			{
				dataRow[item2.Start.Column - 1] = item2.Value;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static DataTable GetDataTableFromExcel(string path, bool hasHeader = true, int startRow = 0)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];
		DataTable dataTable = new DataTable();
		foreach (ExcelRangeBase item in excelWorksheet.Cells[1, 1, 1, excelWorksheet.Dimension.End.Column])
		{
			dataTable.Columns.Add(hasHeader ? item.Text : $"Column {item.Start.Column}");
		}
		startRow = (hasHeader ? (2 + startRow) : (1 + startRow));
		for (int i = startRow; i <= excelWorksheet.Dimension.End.Row; i++)
		{
			ExcelRange excelRange = excelWorksheet.Cells[i, 1, i, excelWorksheet.Dimension.End.Column];
			DataRow dataRow = dataTable.NewRow();
			foreach (ExcelRangeBase item2 in excelRange)
			{
				dataRow[item2.Start.Column - 1] = item2.Value;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static DataTable ReturnTableFromExcel(string path, bool hasHeader = true, int startRow = -1)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];
		DataTable dataTable = new DataTable();
		foreach (ExcelRangeBase item in excelWorksheet.Cells[1, 1, 1, excelWorksheet.Dimension.End.Column])
		{
			dataTable.Columns.Add(hasHeader ? item.Text : $"Column {item.Start.Column + 1}");
		}
		startRow = (hasHeader ? (2 + startRow) : (1 + startRow));
		for (int i = startRow; i <= excelWorksheet.Dimension.End.Row; i++)
		{
			ExcelRange excelRange = excelWorksheet.Cells[i, 1, i, excelWorksheet.Dimension.End.Column];
			DataRow dataRow = dataTable.NewRow();
			foreach (ExcelRangeBase item2 in excelRange)
			{
				dataRow[item2.Start.Column - 1] = item2.Value;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static ExcelWorkbook GetWorkBook(string path)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		return excelPackage.Workbook;
	}

	public static ExcelWorksheet GetWorksheet(string path, string sheetName)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		return string.IsNullOrEmpty(sheetName) ? excelPackage.Workbook.Worksheets.First() : excelPackage.Workbook.Worksheets[sheetName];
	}

	public static ExcelWorksheet GetWorksheet(string path, int sheetIndex)
	{
		using ExcelPackage excelPackage = new ExcelPackage();
		using (FileStream input = File.OpenRead(path))
		{
			excelPackage.Load(input);
		}
		return excelPackage.Workbook.Worksheets[sheetIndex];
	}
}
