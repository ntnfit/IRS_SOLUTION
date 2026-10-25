using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.ImportData.ImportHelper;

internal abstract class BaseDataImport
{
	protected IHttpClient HttpClient = HttpClientHelperFactory.GetHttpClient();

	public string Import(DataTable dt, Enums.ImportActionEnum action, List<ImportDataTableObject> lstDtImportObject, bool isImportChild = false)
	{
		try
		{
			switch (action)
			{
			case Enums.ImportActionEnum.None:
				return "Import action 'ImportActionEnum.None' is invalid.";
			case Enums.ImportActionEnum.Add:
				if (isImportChild)
				{
					return AddObjectFromChilds(dt.Rows[0], lstDtImportObject);
				}
				return AddObject(dt, lstDtImportObject);
			case Enums.ImportActionEnum.Update:
				return UpdateObject(lstDtImportObject, dt.Rows[0], dt);
			case Enums.ImportActionEnum.AddUpdate:
				if (CheckExist(dt.Rows[0]))
				{
					return UpdateObject(lstDtImportObject, dt.Rows[0], dt);
				}
				if (isImportChild)
				{
					return AddObjectFromChilds(dt.Rows[0], lstDtImportObject);
				}
				return AddObject(dt, lstDtImportObject);
			default:
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected abstract string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtObjectSource = null);

	protected abstract string AddObject(DataTable dtObject, List<ImportDataTableObject> lstDtImportObject);

	protected abstract string AddObjectFromChilds(object key, List<ImportDataTableObject> lstDtImportObject);

	protected abstract DataSet GetObjectAndSubObjectTables(object key);

	protected abstract bool CheckExist(object key);

	protected string UpdateDataToRow(DataRow sourceRow, DataRow destRow, DataTable mappingDt = null)
	{
		try
		{
			if (mappingDt == null)
			{
				for (int i = 0; i < sourceRow.Table.Columns.Count; i++)
				{
					string columnName = sourceRow.Table.Columns[i].ColumnName;
					if (destRow.Table.Columns.Contains(columnName))
					{
						destRow[columnName] = Function.GetValueByType(destRow.Table.Columns[columnName].DataType.Name.ToLower(), sourceRow[columnName]);
					}
				}
			}
			else
			{
				for (int j = 0; j < mappingDt.Rows.Count; j++)
				{
					string text = Function.ToString(mappingDt.Rows[j]["Target"]);
					if (destRow.Table.Columns.Contains(text))
					{
						destRow[text] = Function.GetValueByType(destRow.Table.Columns[text].DataType.Name.ToLower(), sourceRow[text]);
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected void RemoveRowsFromDatatable(List<DataRow> lstRows, DataTable sourceTable)
	{
		try
		{
			for (int i = 0; i < lstRows.Count; i++)
			{
				sourceTable.Rows.Remove(lstRows[i]);
			}
			sourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
