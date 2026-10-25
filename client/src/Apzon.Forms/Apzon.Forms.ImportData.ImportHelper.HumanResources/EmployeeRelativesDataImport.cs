using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Forms.Commons;
using Apzon.Global;

namespace Apzon.Forms.ImportData.ImportHelper.HumanResources;

internal class EmployeeRelativesDataImport : BaseDataImport
{
	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtEmpSource = null)
	{
		try
		{
			DataRow dataRow = dtEmpSource.Rows[0];
			Function.ParseInt(dataRow["empID"]);
			dataRow["UserSign"] = GlobalData.UserID;
			dtEmpSource.AcceptChanges();
			DataSet dataSet = new DataSet();
			dtEmpSource.TableName = "dtDoc";
			dataSet.Tables.Add(dtEmpSource);
			DataTable tableStructure = WinFuntions.GetTableStructure("ATC1");
			tableStructure.TableName = "AttFile";
			dataSet.Tables.Add(tableStructure);
			return HttpClient.EmpRelativesClient.SaveDocUpdate(dataSet).Result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObject(DataTable dtDoc, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataRow dataRow = dtDoc.Rows[0];
			Function.ParseInt(dataRow["empID"]);
			dataRow["UserSign"] = GlobalData.UserID;
			dtDoc.AcceptChanges();
			DataSet dataSet = new DataSet();
			dtDoc.TableName = "dtDoc";
			dataSet.Tables.Add(dtDoc);
			DataTable tableStructure = WinFuntions.GetTableStructure("ATC1");
			tableStructure.TableName = "AttFile";
			dataSet.Tables.Add(tableStructure);
			return HttpClient.EmpRelativesClient.SaveDoc(dataSet).Result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override string AddObjectFromChilds(object key, List<ImportDataTableObject> lstDtImportObject)
	{
		throw new Exception("Not support");
	}

	protected override bool CheckExist(object key)
	{
		try
		{
			if (key is DataRow dataRow)
			{
				return HttpClient.ImportDataClient.CheckExistEmployeesRelatives(Function.ParseInt(dataRow["empID"]), Function.ParseInt(dataRow["relaID"]));
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	protected override DataSet GetObjectAndSubObjectTables(object key)
	{
		try
		{
			if (!(key is DataRow dataRow))
			{
				return new DataSet();
			}
			int empId = Function.ParseInt(dataRow["empID"]);
			int relaId = Function.ParseInt(dataRow["RelaID"]);
			DataTable aEmployeesRelatives = HttpClient.ImportDataClient.GetAEmployeesRelatives(empId, relaId);
			int atcEntry = Function.ParseInt(aEmployeesRelatives.Rows[0]["AtcEntry"]);
			DataTable dataTable = HttpClient.ImportDataClient.GetAttachment(atcEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			aEmployeesRelatives.TableName = "dtDoc";
			aEmployeesRelatives.DataSet?.Tables.Remove(aEmployeesRelatives.TableName);
			dataSet.Tables.Add(aEmployeesRelatives);
			dataTable.TableName = "AttFile";
			dataTable.DataSet?.Tables.Remove(dataTable.TableName);
			dataSet.Tables.Add(dataTable);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}
}
