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

namespace Apzon.DataServices.Implements;

public class GroupCustomerVenderRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService) : MasterRepository(unitOfWork, dataService), IGroupCustomerVender, IDisposable
{
	private ApzObjectTypes _objectType = ApzObjectTypes.oBusinessPartnerGroups;

	private string _tableName = "OCRG";

	private string _keyTag = "Code";

	private string _rowTag = "BusinessPartnerGroups";

	private string _keyFieldName = "GroupCode";

	protected Dictionary<string, string> DicFieldTags = new Dictionary<string, string>
	{
		{ "Name", "GroupName" },
		{ "Type", "GroupType" }
	};

	public HttpResult Save(DataTable dt, string sessionId, string type)
	{
		try
		{
			return base.Save(dt, sessionId, _objectType, _keyFieldName, DicFieldTags, deleteOldRows: true, Enums.ImportActionEnum.AddUpdate, _tableName, _rowTag, _keyTag, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override DataTable GetDocs(string tableName, object parameter = null)
	{
		try
		{
			string query = "Select * from " + tableName + " where \"GroupType\"=@GroupType;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@GroupType", parameter));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Docs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual HttpResult Remove(string grCode, string sessionId)
	{
		try
		{
			return base.Remove(grCode, sessionId, ApzObjectTypes.oBusinessPartnerGroups);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
