using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.Sales;

public class DocumentDraftController : BaseApiController
{
	public ApzObjectTypes FormObjectTypes = ApzObjectTypes.oDrafts;

	[HttpGet]
	public DataTable GetTableDefault()
	{
		return UnitOfWork.DocumentDraft.GetTableDefault();
	}

	[HttpGet]
	public DataTable GetDraftByEntry(int docEntry)
	{
		return UnitOfWork.DocumentDraft.GetDraftByEntry(docEntry);
	}

	[HttpPost]
	public ResponseResult DMSSearchDoc([FromBody] DataTable dt)
	{
		if (dt.IsNotNull() && !dt.Columns.Contains("UserLogin"))
		{
			dt.Columns.Add("UserLogin");
			dt.Rows[0]["UserLogin"] = Global.SessionUserInfo[SessionId].UserName;
		}
		return UnitOfWork.DocumentDraft.DMSSearchDoc(dt);
	}

	[HttpPost]
	public DataTable SearchDoc([FromBody] DataTable dt)
	{
		if (dt.IsNotNull() && !dt.Columns.Contains("UserLogin"))
		{
			dt.Columns.Add("UserLogin");
			dt.Rows[0]["UserLogin"] = Global.SessionUserInfo[SessionId].UserName;
		}
		return UnitOfWork.DocumentDraft.SearchDoc(dt);
	}

	[HttpPost]
	public DataTable SearchListDocEntry([FromBody] DataTable dt)
	{
		return UnitOfWork.DocumentDraft.SearchListDocEntry(dt);
	}

	[HttpPost]
	public DataTable SearchLine([FromBody] DataTable dt)
	{
		return UnitOfWork.DocumentDraft.SearchLine(dt);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Close([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.CloseDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return UnitOfWork.Documents.CloseDoc(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public string RemoveAndClose([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.RemoveAndCloseDocumentDraft(dt, SessionUserInfo.SessionDiServer);
		}
		return "Chức năng đang cập nhật";
	}

	[HttpGet]
	public DataTable GetPayment(int docEntry, int objType)
	{
		return UnitOfWork.Documents.GetPayment(docEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDraft(DataSet ds)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveDataSet(ds, FormObjectTypes);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Remove([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			string message = BrokerService.GetUnitOfWork(SessionId).Documents.RemoveDoc(docEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		return UnitOfWork.Documents.Remove(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public string CheckIsIns(int docEntry)
	{
		return UnitOfWork.DocumentDraft.CheckIsIns(docEntry);
	}

	[HttpPost]
	public string SaveLogForm([FromBody] DataTable dt)
	{
		return UnitOfWork.DocumentDraft.SaveLogForm(dt);
	}

	[HttpGet]
	public DataTable GetLogForm(string userCode)
	{
		return UnitOfWork.Commons.GetLogFormById("oDocumentDraftRP", userCode);
	}

	[HttpGet]
	public DataTable GetReferencedDocument(int docEntry)
	{
		return UnitOfWork.Documents.GetReferencedDocument(FormObjectTypes, docEntry);
	}
}
