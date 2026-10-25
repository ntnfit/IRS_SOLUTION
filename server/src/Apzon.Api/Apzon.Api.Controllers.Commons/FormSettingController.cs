using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class FormSettingController : BaseApiController
{
	[HttpGet]
	public DataTable GetFormSetting(string formId, string itemId, string user, string lngCode)
	{
		return UnitOfWork.FormSetting.GetFormSetting(formId, itemId, user, lngCode);
	}

	[HttpPost]
	public string SaveData([FromBody] DataTable lst)
	{
		try
		{
			return UnitOfWork.FormSetting.SaveData(lst);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string CopyFormSetting([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.FormSetting.CopyFormSetting(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string CopyFormSettingToItem([FromBody] int settingId)
	{
		try
		{
			return UnitOfWork.FormSetting.CopyFormSettingToItem(settingId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataSet GetStructure(int id, string formId, string itemId, string userName, int branch, string type)
	{
		return UnitOfWork.FormSetting.GetStructure(id, formId, itemId, userName, branch, type);
	}

	[HttpGet]
	public DataTable GetListFormSetting(string formId, string itemId, string user, int id)
	{
		return UnitOfWork.FormSetting.GetListFormSetting(formId, itemId, user, id);
	}

	[HttpGet]
	public DataTable GetValidData(string formId, string itemId, string user, int id)
	{
		return UnitOfWork.FormSetting.GetValidData(formId, itemId, user, id);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		return UnitOfWork.FormSetting.Save(ds, "", Enums.TransactionType.A, Global.DatabaseSystemType);
	}

	[HttpPost]
	public HttpResult Delete([FromBody] int id)
	{
		Logging.Write("TRACE", SessionUserInfo?.UserName, id.ToString());
		return UnitOfWork.FormSetting.Save(null, id.ToString(), Enums.TransactionType.D);
	}

	[HttpGet]
	public DataSet GetUserForCopy(string userName, int idSetting)
	{
		return UnitOfWork.FormSetting.GetUserForCopy(userName, idSetting);
	}

	[HttpGet]
	public HttpResult CopyReportFormSetting(string listUser, int idSetting)
	{
		return UnitOfWork.FormSetting.CopyReportFormSetting(listUser, idSetting);
	}

	[HttpGet]
	public DataTable GetDataLanguage(string lngCode)
	{
		return UnitOfWork.FormSetting.GetDataLanguage(lngCode);
	}

	[HttpGet]
	public DataTable GetDataSettingLanguage(string currLng)
	{
		return UnitOfWork.FormSetting.GetDataSettingLanguage(currLng);
	}

	[HttpPost]
	public HttpResult RestoreDefault([FromBody] Tuple<string, string, string> restoreData)
	{
		Logging.Write("TRACE", "FormSetting_RestoreDefault", SessionUserInfo?.UserName, restoreData.Item1, restoreData.Item2, restoreData.Item3);
		return UnitOfWork.FormSetting.RestoreDefault(restoreData.Item1, restoreData.Item2, restoreData.Item3);
	}

	[HttpPost]
	public HttpResult RestoreDefaultAllUser([FromBody] Tuple<string, string> restoreData)
	{
		Logging.Write("TRACE", "FormSetting_RestoreDefaultAllUser", SessionUserInfo?.UserName, restoreData.Item1, restoreData.Item2);
		return UnitOfWork.FormSetting.RestoreDefaultAllUser(restoreData.Item1, restoreData.Item2);
	}

	[HttpGet]
	public DataTable GetMessageDataCenter(string lngCode)
	{
		return UnitOfWork.FormSetting.GetMessageDataCenter(lngCode);
	}
}
