using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Commons;

public class ContextActionController : BaseApiController
{
	[HttpGet]
	public DataTable GetListContextAction(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListContextAction(formId, itemId);
	}

	[HttpGet]
	public DataTable GetListParams(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListParams(formId, itemId);
	}

	[HttpGet]
	public DataTable GetListAttachment(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListAttachment(formId, itemId);
	}

	[HttpGet]
	public DataTable GetListAttachParams(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListAttachParams(formId, itemId);
	}

	[HttpGet]
	public DataTable GetListMapping(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListMapping(formId, itemId);
	}

	[HttpPost]
	public HttpResult Save([FromBody] Tuple<string, string, DataSet> data)
	{
		return UnitOfWork.ContextAction.Save(data.Item1, data.Item2, data.Item3);
	}

	[HttpPost]
	public HttpResult SavePoint(DataTable dt)
	{
		return UnitOfWork.ContextAction.SavePoint(dt);
	}

	[HttpGet]
	public DataSet GetData(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetData(formId, itemId);
	}

	[HttpGet]
	public DataSet GetDataByEntry(int absEntry, string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetDataByEntry(absEntry, formId, itemId);
	}

	[HttpGet]
	public DataTable GetMappingUdo(string udocode, string actionFormId, string actionItemId, int absEntry, string sonname = "")
	{
		return UnitOfWork.ContextAction.GetMappingUdo(udocode, actionFormId, actionItemId, absEntry, sonname);
	}

	[HttpGet]
	public DataTable GetTargetField(string udocode, string sonname = "")
	{
		return UnitOfWork.ContextAction.GetTargetField(udocode, sonname);
	}

	[HttpGet]
	public DataTable GetProcessGroup(int absEntry, string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetProcessGroup(absEntry, formId, itemId);
	}

	[HttpPost]
	public HttpResult HandleWorkflow([FromBody] List<Workflow> processGroup)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Common.HandleWorkflow(processGroup, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult HandleWorkflowProcess([FromBody] List<IWorkflowProcess> processGroup)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Common.HandleWorkflowProcess(processGroup, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetListHttpRequestConfig(string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetListHttpRequestConfig(formId, itemId);
	}

	[HttpGet]
	public DataTable GetHeaderByAlias(string alias, string formId, string itemId)
	{
		return UnitOfWork.ContextAction.GetHeaderByAlias(alias, formId, itemId);
	}

	[HttpGet]
	public DataTable GetColumnsFromPhysicalTableName(string physicalTableName)
	{
		try
		{
			string[] array = physicalTableName.Split('.');
			if (array.Length > 1)
			{
				string text = array[0];
				string physicalTableName2 = array[1];
				return Apzon.Services.UnitOfWorkFactory.GetUnitOfWork((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {Global.CommonDbServer};CS=\"{text}\"; UserID = {Global.CommonDbUserName}; Password = {Global.CommonDbPassword}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", Global.CommonDbServer, Global.CommonDbUserName, Global.CommonDbPassword, text), Global.DatabaseSystemType).ContextAction.GetColumnsFromPhysicalTableName(physicalTableName2);
			}
			return UnitOfWork.ContextAction.GetColumnsFromPhysicalTableName(physicalTableName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
