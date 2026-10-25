using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;

namespace Apzon.Api.Controllers.Financials;

public class JournalEntriesController : BaseApiController
{
	[HttpPost]
	public string Costing([FromBody] JournalEntriesImpl doc)
	{
		return UnitOfWork.JournalEntries.Costing(doc);
	}

	[HttpPost]
	public string Import([FromBody] JournalEntriesImpl doc, [FromBody] AccountingSalaryImpl _AccountingSalary)
	{
		return UnitOfWork.JournalEntries.Import(doc, _AccountingSalary);
	}

	[HttpGet]
	public string Cancel(int docEntry)
	{
		return UnitOfWork.JournalEntries.Cancel(docEntry);
	}

	[HttpGet]
	public int GetMax()
	{
		return UnitOfWork.JournalEntries.GetMax();
	}

	[HttpGet]
	public DataTable GetDoc(int transId)
	{
		return UnitOfWork.JournalEntries.GetDoc(transId);
	}

	[HttpGet]
	public DataTable GetLines(int transId)
	{
		return UnitOfWork.JournalEntries.GetLines(transId);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] JournalEntriesImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.JournalEntries.SaveData(doc, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult CreateJournalvoucher([FromBody] JournalEntriesImpl lstJE)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.JournalEntries.CreateJournalVoucher(lstJE, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult CancelJE([FromBody] int transId)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.JournalEntries.CancelJE(transId, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}
}
