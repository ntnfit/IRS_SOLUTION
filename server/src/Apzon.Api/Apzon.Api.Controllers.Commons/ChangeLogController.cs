using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class ChangeLogController : BaseApiController
{
	[HttpGet]
	public int GetDocEntryItemDetail(int id)
	{
		return UnitOfWork.ChangeLog.GetDocEntryItemDetail(id);
	}

	[HttpGet]
	public DataTable GetLog(int docEntry)
	{
		return UnitOfWork.ChangeLog.GetLog(docEntry);
	}

	[HttpGet]
	public DataTable GetChangeLogRow(int docEntry, int logId, int type)
	{
		return UnitOfWork.ChangeLog.GetChangeLogRow(docEntry, logId, type);
	}

	[HttpPost]
	public DataTable GetChangeLog([FromBody] GetChangeLogParams pr)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				if (pr.IsApzTable)
				{
					return UnitOfWork.ChangeLog.GetChangeLog(pr);
				}
				return unitOfWork.ChangeLog.GetChangeLog(pr, SessionUserInfo.SessionDiServer);
			}
			Logging.Write("ERROR", "INVALIDSESSION");
			return new DataTable();
		}
		return UnitOfWork.ChangeLog.GetChangeLog(pr);
	}

	[HttpPost]
	public DataTable GetChangeLogDifferences(GetChangeLogParams pr)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				if (pr.IsApzTable)
				{
					return UnitOfWork.ChangeLog.GetChangeLogDifferences(pr);
				}
				return unitOfWork.ChangeLog.GetChangeLogDifferences(pr, SessionUserInfo.SessionDiServer);
			}
			Logging.Write("ERROR", "INVALIDSESSION");
			return new DataTable();
		}
		return UnitOfWork.ChangeLog.GetChangeLogDifferences(pr);
	}
}
