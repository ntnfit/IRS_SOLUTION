using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.ApprovalProcess;

public class ApprovalTemplatesController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.ApprovalTemplates.GetData();
	}

	[HttpGet]
	public DataSet GetDataLines(int templateCode)
	{
		return UnitOfWork.ApprovalTemplates.GetDataLines(templateCode);
	}

	[HttpGet]
	public DataTable SearchData(string templateName, string templateRemarks)
	{
		return UnitOfWork.ApprovalTemplates.SearchData(templateName, templateRemarks);
	}

	[HttpGet]
	public DataTable GetUserInfo(string userName)
	{
		return UnitOfWork.ApprovalTemplates.GetUserInfo(userName);
	}

	[HttpGet]
	public int GetNewCode()
	{
		return UnitOfWork.ApprovalTemplates.GetNewCode();
	}

	[HttpPost]
	public HttpResult Save(Tuple<DataSet, string> tuple)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.ApprovalTemplates.Save(tuple, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult Delete(Tuple<int> tuple)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.ApprovalTemplates.Delete(tuple.Item1, SessionUserInfo.SessionDiServer);
			}
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetRecord(string position, string templateCode)
	{
		return UnitOfWork.ApprovalTemplates.GetRecord(position, templateCode);
	}

	[HttpGet]
	public DataTable GetApprovalStageInfo(string templateName)
	{
		return UnitOfWork.ApprovalTemplates.GetApprovalStageInfo(templateName);
	}

	[HttpGet]
	public int GetQueryId(string queryName)
	{
		return UnitOfWork.ApprovalTemplates.GetQueryId(queryName);
	}
}
