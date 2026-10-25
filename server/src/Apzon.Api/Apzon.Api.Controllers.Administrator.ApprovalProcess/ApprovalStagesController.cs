using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.ApprovalProcess;

public class ApprovalStagesController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.ApprovalStages.GetData();
	}

	[HttpGet]
	public DataTable GetDataLines(int stageCode)
	{
		return UnitOfWork.ApprovalStages.GetDataLines(stageCode);
	}

	[HttpGet]
	public DataTable SearchData(int stageCode, string stageName)
	{
		return UnitOfWork.ApprovalStages.SearchData(stageCode, stageName);
	}

	[HttpGet]
	public DataTable GetUserInfo(string userName)
	{
		return UnitOfWork.ApprovalStages.GetUserInfo(userName);
	}

	[HttpGet]
	public int GetNewCode()
	{
		return UnitOfWork.ApprovalStages.GetNewCode();
	}

	[HttpPost]
	public HttpResult Save(Tuple<DataTable, DataTable, string> tuple)
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
				return unitOfWork.ApprovalStages.Save(tuple, SessionUserInfo.SessionDiServer);
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
			return BrokerService.GetUnitOfWork(SessionId).ApprovalStages.Delete(tuple.Item1, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetRecord(string position, string stageCode)
	{
		return UnitOfWork.ApprovalStages.GetRecord(position, stageCode);
	}
}
