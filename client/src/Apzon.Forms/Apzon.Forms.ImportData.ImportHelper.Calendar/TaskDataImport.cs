using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;

namespace Apzon.Forms.ImportData.ImportHelper.Calendar;

internal class TaskDataImport : BaseDataImport
{
	protected override string AddObject(DataTable dtObject, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dtObject.TableName = "Table1";
			dataSet.Tables.Add(dtObject);
			return Function.ToString(HttpClient.SubActivities.AddTask(dataSet).Result.MessageCode);
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
				return HttpClient.SubActivities.CheckExists(Function.ParseInt(dataRow["ClgCode"]));
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
		throw new NotImplementedException();
	}

	protected override string UpdateObject(List<ImportDataTableObject> lstDtImportObject, object key = null, DataTable dtObjectSource = null)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dtObjectSource.TableName = "Table1";
			dataSet.Tables.Add(dtObjectSource);
			return Function.ToString(HttpClient.SubActivities.SaveTask(dataSet).Result.MessageCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}
}
