using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class WorkingProcessController : BaseApiController
{
	[HttpGet]
	public DataSet GetWorkingProcessSetting(int userId)
	{
		return UnitOfWork.WorkingProcess.GetWorkingProcessSettingData(userId);
	}

	[HttpGet]
	public DataSet GetSetting(int userId)
	{
		return UnitOfWork.WorkingProcess.GetSetting(userId);
	}

	[HttpPost]
	public HttpResult SaveWorkingProcessSetting(Tuple<DataSet> tupData)
	{
		return UnitOfWork.WorkingProcess.SaveWorkingProcessSetting(tupData);
	}

	[HttpGet]
	public DataTable GetTemplateData(int userId, string status, string requestType)
	{
		return UnitOfWork.WorkingProcess.GetTemplateData(userId, status, requestType);
	}

	[HttpGet]
	public DataTable GetTemplateDetails(int wddCode)
	{
		return UnitOfWork.WorkingProcess.GetTemplateDetails(wddCode);
	}

	[HttpGet]
	public DataTable GetTooltipData(int wddCode, int wtmCode, int docEntry, int userId)
	{
		return UnitOfWork.WorkingProcess.GetTooltipData(wddCode, wtmCode, docEntry, userId);
	}

	[HttpGet]
	public DataTable GetTileViewSetting(int userId)
	{
		return UnitOfWork.WorkingProcess.GetTileViewSetting(userId);
	}

	[HttpPost]
	public async Task<HttpResult> SaveDraftToDocument([FromBody] DocumentsImpl doc)
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
					return await unitOfWork.Documents.SaveDraftToDocumentApproval(doc, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
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
	public bool CheckDocumentCompleteApproval(int wddCode)
	{
		return UnitOfWork.WorkingProcess.CheckDocumentCompleteApproval(wddCode);
	}

	[HttpPost]
	public DataTable FilterMenu(Tuple<DataSet> tupFilter)
	{
		return UnitOfWork.WorkingProcess.FilterMenu(tupFilter);
	}

	[HttpPost]
	public HttpResult SaveMenu(Tuple<int, DataTable> tup)
	{
		return UnitOfWork.WorkingProcess.SaveMenu(tup);
	}

	[HttpGet]
	public bool AllowLastGroupUserCreatedDoc(int wddCode, int userId)
	{
		return UnitOfWork.WorkingProcess.AllowLastGroupUserCreatedDoc(wddCode, userId);
	}

	[HttpGet]
	public bool IsUserApproved(int wddCode, int userId)
	{
		return UnitOfWork.WorkingProcess.IsUserApproved(wddCode, userId);
	}
}
