using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.BusinessPartner;

public class InternalReconciliationsController : BaseApiController
{
	[HttpGet]
	public DataSet GetStructure()
	{
		try
		{
			return UnitOfWork.Commons.ExecuteData("select top 0 * from OITR;\r\n                                                        select top 0 * from ITR1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpPost]
	public HttpResult Add(InternalReconciliationImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).InternalReconciliations.Add(doc, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "Function not support");
	}
}
