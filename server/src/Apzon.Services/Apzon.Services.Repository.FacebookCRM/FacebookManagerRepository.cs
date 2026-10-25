using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.FacebookCRM;

namespace Apzon.Services.Repository.FacebookCRM;

public class FacebookManagerRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IFacebookManager
{
	private FacebookManagerSaveType _saveType;

	private string _pageId = "";

	public DataTable GetDataAccessUser(string userCode)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_FUSR where \"UserCode\" = @userCode and IsDefault = 'Y';", CommandType.Text, new SqlParameter("@userCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveUserAccessToken(DataTable dtUserAccessToken)
	{
		try
		{
			_saveType = FacebookManagerSaveType.UserToken;
			return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dtUserAccessToken);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SavePageUserInfo(DataTable dtPageInfo)
	{
		try
		{
			_saveType = FacebookManagerSaveType.PageAccessInfo;
			return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dtPageInfo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		return _saveType switch
		{
			FacebookManagerSaveType.UserToken => ProcessSaveUserToken(document), 
			FacebookManagerSaveType.PageAccessInfo => ProcessSavePageAccessInfo(document), 
			FacebookManagerSaveType.PageUser => ProcessSavePageUser(document), 
			_ => new HttpResult(Enums.MessageCode.Error, "Function not support"), 
		};
	}

	private HttpResult ProcessSaveUserToken(DataTable dtUserToken)
	{
		if (!dtUserToken.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Not enough data to save");
		}
		CreateTempTable(dtUserToken, "#APZ_FUSR");
		BulkCopy(dtUserToken, "#APZ_FUSR");
		DataTable dataTable = ExecuteDataTable("APZ_FacebookManager_SaveUserToken", CommandType.StoredProcedure);
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Cannot save data");
		}
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])} - {Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	private HttpResult ProcessSavePageAccessInfo(DataTable dtPageInfo)
	{
		if (!dtPageInfo.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Not enough data to save");
		}
		CreateTempTable(dtPageInfo, "#APZ_FUSP");
		BulkCopy(dtPageInfo, "#APZ_FUSP");
		DataTable dataTable = ExecuteDataTable("APZ_FacebookManager_SavePageAccessInfo", CommandType.StoredProcedure);
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Cannot save data");
		}
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])} - {Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult();
	}

	private HttpResult ProcessSavePageUser(DataTable dtConver)
	{
		if (!dtConver.IsNotNull() || string.IsNullOrEmpty(_pageId))
		{
			return new HttpResult(Enums.MessageCode.Error, "Not enough data to save");
		}
		CreateTempTable(dtConver, "#APZ_FPUS");
		BulkCopy(dtConver, "#APZ_FPUS");
		DataTable dataTable = ExecuteDataTable("APZ_FacebookManager_SavePageUser", CommandType.StoredProcedure, new SqlParameter("@pageId", _pageId));
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Cannot save data");
		}
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])} - {Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult();
	}

	public DataTable GetListUserAccessPage(string userCode)
	{
		try
		{
			return ExecuteDataTable("select \"F_PageId\", \"PageName\", \"AccessPageToken\", \"Actived\", \"Avatar\" from APZ_FUSP where \"UserCode\" = @userCode;", CommandType.Text, new SqlParameter("@userCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataConversationPage(string pageId)
	{
		try
		{
			return ExecuteData("select * from APZ_FPMS where \"F_PageId\" = @pageId and \"ConvrType\" = 'M';\r\n                                     select * from APZ_FPMS where \"F_PageId\" = @pageId and \"ConvrType\" = 'C';", CommandType.Text, new SqlParameter("@pageId", pageId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataPageUser(string pageId)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_FPUS where \"F_PageId\" = @pageId;", CommandType.Text, new SqlParameter("@pageId", Function.ToString(pageId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveDataPageUser(string pageId, DataTable dtConver)
	{
		try
		{
			_saveType = FacebookManagerSaveType.PageUser;
			_pageId = pageId;
			return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dtConver);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDataItem(string itemCode, string bpCode)
	{
		try
		{
			return ExecuteDataTable("APZ_FacebookManager_GetDataItem", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@bpCode", bpCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataFacebookCustomer(string facebookUserId)
	{
		try
		{
			return ExecuteDataTable("APZ_FacebookManager_GetDataFacebookCustomer", CommandType.StoredProcedure, new SqlParameter("@facebookUserId", facebookUserId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
