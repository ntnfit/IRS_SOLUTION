using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;

namespace Apzon.Forms.ImportData.ImportHelper;

public class ImportHelper
{
	public static DataTable GetDataTableForRollBackImport(ImportDataTableObject importDataTableObject)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure(importDataTableObject.ImportObject.TableName);
			tableStructure.TableName = importDataTableObject.ImportObject.TableName;
			for (int i = 0; i < importDataTableObject.ImportData.Rows.Count; i++)
			{
				DataTable dataTable = PrepareImportData(importDataTableObject.ImportData.Rows[i], importDataTableObject.MappingData, importDataTableObject.ImportObject);
				if (dataTable.IsNotNull())
				{
					tableStructure.ImportRow(dataTable.Rows[0]);
				}
			}
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public static DataTable PrepareImportData(DataRow sourceRow, DataTable mappingDt, ImportObjectImpls importObject)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure(importObject.TableName);
			DataRow dataRow = tableStructure.NewRow();
			for (int i = 0; i < tableStructure.Columns.Count; i++)
			{
				DataColumn targetCol = tableStructure.Columns[i];
				string text = "";
				DataRow dataRow2 = mappingDt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Target"]) == targetCol.ColumnName);
				if (dataRow2 != null)
				{
					text = Function.ToString(dataRow2["Source"]);
				}
				if (sourceRow.Table.Columns.Contains(text))
				{
					dataRow[targetCol] = Function.GetValueByType(targetCol.DataType.Name.ToLower(), sourceRow[text]);
				}
			}
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}
}
