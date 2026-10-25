using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.Services.Repository.ImportData;

public class BaseImportData
{
	protected string _connection;

	protected int _userID;

	protected IUnitOfWork _unitOfWork;

	public BaseImportData()
	{
	}

	protected BaseImportData(string connection, int userID, IUnitOfWork unitOfWork)
	{
		_connection = connection;
		_userID = userID;
		_unitOfWork = unitOfWork;
	}

	protected string UpdateDataToRow(DataRow sourceRow, DataRow destRow)
	{
		try
		{
			for (int i = 0; i < sourceRow.Table.Columns.Count; i++)
			{
				string columnName = sourceRow.Table.Columns[i].ColumnName;
				if (destRow.Table.Columns.Contains(columnName))
				{
					destRow[columnName] = Function.GetValueByType(destRow.Table.Columns[columnName].DataType.Name.ToLower(), sourceRow[columnName]);
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
		}
	}
}
