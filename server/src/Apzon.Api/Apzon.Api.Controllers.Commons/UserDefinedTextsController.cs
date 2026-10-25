using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Commons;

[ValidateLoginSapExceptionHandling]
public class UserDefinedTextsController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		try
		{
			return UnitOfWork.UserDefinedTexts.GetAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public string Save([FromBody] DataTable dataTable)
	{
		if (Global.LoginSap && Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.UserDefinedTexts.Save(SessionUserInfo.SessionDiServer, dataTable);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.UserDefinedTexts.Save(dataTable);
	}

	[HttpPost]
	public string Remove([FromBody] int absEntry)
	{
		if (Global.LoginSap && Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.UserDefinedTexts.Remove(SessionUserInfo.SessionDiServer, absEntry);
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.UserDefinedTexts.Remove(absEntry);
	}
}
