using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class BranchesController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.Branches.LoadDoc();
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}

	[HttpPost]
	public HttpResult SaveOBPL([FromBody] DataTable dtDoc)
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
				return unitOfWork.Branches.SaveData(SessionUserInfo.SessionDiServer, dtDoc);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Branches.Save(dtDoc);
	}

	[HttpGet]
	public DataTable GetMaxCode()
	{
		return UnitOfWork.Branches.GetMaxCode();
	}

	[HttpPost]
	public DataTable CheckExistBPLInOWHS([FromBody] Tuple<string, string> tup)
	{
		return UnitOfWork.Branches.CheckExistBPLInOWHS(tup);
	}
}
