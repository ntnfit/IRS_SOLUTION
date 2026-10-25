using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaGroupCustomerVenderRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService) : HanaMasterRepository(serviceUnitOfWork, dataService), IGroupCustomerVender, IDisposable
{
	private ApzObjectTypes _objectType = ApzObjectTypes.oBusinessPartnerGroups;

	private string _tableName = "OCRG";

	private string _keyTag = "Code";

	private string _rowTag = "BusinessPartnerGroups";

	private string _keyFieldName = "GroupCode";

	private Dictionary<string, string> _dicFieldTags = new Dictionary<string, string>
	{
		{ "Name", "GroupName" },
		{ "Type", "GroupType" }
	};

	public bool CheckExists(string code, string sessionId)
	{
		string query = "select 1 from OCRG where \"GroupCode\" = @code;";
		if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code)).Rows.Count > 0)
		{
			return true;
		}
		return false;
	}

	public HttpResult Save(DataTable dt, string sessionId, string type)
	{
		HttpResult result = new HttpResult();
		try
		{
			if (string.IsNullOrEmpty(type))
			{
				foreach (DataRow row in dt.Rows)
				{
					result = base.Remove(Function.ParseInt(row[_keyFieldName]), sessionId, _objectType, _keyTag);
				}
				return result;
			}
			return base.Save(dt, sessionId, _objectType, _keyFieldName, _dicFieldTags, deleteOldRows: false, Enums.ImportActionEnum.AddUpdate, _tableName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(DataTable dt, string sessionId)
	{
		HttpResult result = new HttpResult();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				result = base.Remove(row["GroupCode"], sessionId, ApzObjectTypes.oBusinessPartnerGroups);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Dispose()
	{
	}
}
