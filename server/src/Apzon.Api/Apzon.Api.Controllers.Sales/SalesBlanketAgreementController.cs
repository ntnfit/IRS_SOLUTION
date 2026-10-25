using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Sales;

[ValidateLoginSapExceptionHandling]
public class SalesBlanketAgreementController : BaseApiController
{
	public ApzObjectTypes FormObjectTypes = ApzObjectTypes.oBlanketAgreement;

	[HttpPost]
	public DataTable GetDoc([FromBody] DocumentSBAImpl condiDoc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.SalesBlanketAgreement.GetDoc(condiDoc, userName);
		}
		return UnitOfWork.SalesBlanketAgreement.GetDoc(condiDoc, "");
	}

	[HttpGet]
	public DataTable GetOwner()
	{
		return UnitOfWork.SalesBlanketAgreement.GetOwner();
	}

	[HttpGet]
	public DataTable GetNumerMax(int series)
	{
		return UnitOfWork.SalesBlanketAgreement.GetNumerMax(series);
	}

	[HttpGet]
	public DataTable GetNameCntctCode()
	{
		return UnitOfWork.SalesBlanketAgreement.GetNameCntctCode();
	}

	[HttpPost]
	public DataTable GetLine([FromBody] int AbsId)
	{
		return UnitOfWork.SalesBlanketAgreement.GetLine(AbsId);
	}

	[HttpGet]
	public DataTable GetContactPerson(string code)
	{
		return UnitOfWork.SalesBlanketAgreement.GetContactPerson(code);
	}

	[HttpGet]
	public DataTable GetItemGroup()
	{
		return UnitOfWork.SalesBlanketAgreement.GetItemGroup();
	}

	[HttpPost]
	[QueueModifiedAction]
	public string Save([FromBody] DataSet tpl)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			return BrokerService.GetUnitOfWork(SessionId).SalesBlanketAgreement.Save(SessionUserInfo.SessionDiServer, tpl);
		}
		return UnitOfWork.SalesBlanketAgreement.Save(tpl);
	}

	[HttpGet]
	public DataTable GetPriceList()
	{
		return UnitOfWork.SalesBlanketAgreement.GetPriceList();
	}

	[HttpPost]
	[QueueModifiedAction]
	public string DeleteRow([FromBody] int AgrNo)
	{
		return UnitOfWork.SalesBlanketAgreement.DeleteRow(AgrNo);
	}

	[HttpGet]
	public int GetOwnerByBPCode(string cardCode)
	{
		return UnitOfWork.SalesBlanketAgreement.GetOwnerByBPCode(cardCode);
	}

	[HttpPost]
	public DataTable GetBlanketAgreementDetails([FromBody] Tuple<int, int> tupe)
	{
		return UnitOfWork.SalesBlanketAgreement.GetBlanketAgreementDetails(tupe.Item1, tupe.Item2);
	}

	[HttpPost]
	public DataTable CheckExistActivity([FromBody] Tuple<int, int, int> tpl)
	{
		return UnitOfWork.SalesBlanketAgreement.CheckExistActivity(tpl.Item1, tpl.Item2, tpl.Item3);
	}

	[HttpGet]
	public DataTable GetActivitiesByActivityID(string lstClgCodes)
	{
		return UnitOfWork.SalesBlanketAgreement.GetActivitiesByActivityID(lstClgCodes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string Cancel([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			return BrokerService.GetUnitOfWork(SessionId).SalesBlanketAgreement.Cancel(DocEntry, SessionUserInfo.SessionDiServer);
		}
		return "Function not support!";
	}
}
