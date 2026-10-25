using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Intergrated;

[ValidateLoginSapExceptionHandling]
public class IntegratedWarehouseController : BaseApiController
{
	[HttpPost]
	public DataSet Search([FromBody] DataTable doc)
	{
		return UnitOfWork.IntegratedWarehouse.Search(doc);
	}

	[HttpPost]
	public DataTable CheckExists([FromBody] DataTable dt)
	{
		return UnitOfWork.IntegratedWarehouse.CheckExists(dt);
	}

	[HttpPost]
	public string SaveADoc([FromBody] WarehouseImpl doc)
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
				bool flag = CheckExists(doc.WhsCode);
				return unitOfWork.Warehouses.Save(doc, SessionUserInfo.SessionDiServer, flag ? Enums.TransactionType.U : Enums.TransactionType.A);
			}
			return "INVALIDSESSION";
		}
		return "This function is not supported";
	}

	private bool CheckExists(string whsCode)
	{
		try
		{
			return Function.ParseInt(UnitOfWork.Commons.ExecuteScalar("select 1 from OWHS where \"WhsCode\" = @whsCode;", CommandType.Text, new SqlParameter("@whsCode", whsCode))) == 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
