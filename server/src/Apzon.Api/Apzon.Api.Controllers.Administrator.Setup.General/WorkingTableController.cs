using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class WorkingTableController : BaseApiController
{
	[HttpGet]
	public DataSet GetWorkingTableDoc(string type)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataSet();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.WorkingTable.GetWorkingTableDoc(type, userName);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetWorkingTableLine(string id)
	{
		return UnitOfWork.WorkingTable.GetWorkingTableLine(id);
	}

	[HttpGet]
	public DataSet GetDataStepStatus(string id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataSet();
			}
			return UnitOfWork.WorkingTable.GetDataStepStatus(id);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		return UnitOfWork.Documents.CopyFrom("112", entry, type, bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine("112", entry, type);
	}

	[HttpGet]
	public DataTable GetUserSettings()
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return new DataTable();
		}
		string userName = SessionUserInfo.UserName;
		return UnitOfWork.WorkingTable.GetUserSettings(userName);
	}

	[HttpGet]
	public HttpResult GetTopDraft()
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new HttpResult(Enums.MessageCode.Error, "ERR: Invalid session id");
			}
			string userName = SessionUserInfo.UserName;
			if (string.IsNullOrEmpty(userName))
			{
				return new HttpResult(Enums.MessageCode.Error, "ERR: Invalid user name");
			}
			return UnitOfWork.WorkingTable.GetTopDraft(userName);
		}
		return new HttpResult(Enums.MessageCode.Error, "ERR: Function Not support");
	}

	[HttpPost]
	public string SetStatus([FromBody] DataTable dt)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "INVALIDSESSION";
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					string userName = SessionUserInfo.UserName;
					return unitOfWork.Documents.ApproveDraft(SessionUserInfo.SessionDiServer, userName, dt, string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
				}
				return "INVALIDSESSION";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return Resources.Msg_UnknownError;
	}

	[HttpGet]
	public string MarkReadMessage(int docEntry)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "INVALIDSESSION";
				}
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					string userName = SessionUserInfo.UserName;
					return UnitOfWork.WorkingTable.MarkReadMessage(docEntry, userName);
				}
				return "INVALIDSESSION";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return Resources.Msg_UnknownError;
	}

	[HttpPost]
	public async Task<string> CreateDoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return (await unitOfWork.Documents.Save(doc, SessionUserInfo.SessionDiServer, (ApzObjectTypes)doc.ObjType)).Message;
			}
			return "INVALIDSESSION";
		}
		return "";
	}

	[HttpPost]
	public HttpResult SaveDraftToDocument([FromBody] DocumentsImpl doc)
	{
		try
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
					return unitOfWork.Documents.SaveDraftToDocument(doc, SessionUserInfo.SessionDiServer);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpGet]
	public HttpResult GetDraftApprovalStatus(int draftKey)
	{
		return UnitOfWork.WorkingTable.GetDraftApprovalStatus(draftKey);
	}

	[HttpPost]
	public string CancelApprovalRequest([FromBody] int wddCode)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "INVALIDSESSION";
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.ApprovalRequests.CancelApprovalRequest(wddCode, SessionUserInfo.SessionDiServer);
				}
				return "INVALIDSESSION";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return Resources.Msg_UnknownError;
	}
}
