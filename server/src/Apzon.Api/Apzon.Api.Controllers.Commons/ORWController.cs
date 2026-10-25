using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class ORWController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableNameOUDO()
	{
		return UnitOfWork.ORW.GetTableNameOUDO();
	}

	[HttpGet]
	public DataTable GetTableChild(int objtype, string usedInObj)
	{
		return UnitOfWork.ORW.GetTableChild(objtype, usedInObj);
	}

	[HttpGet]
	public DataTable GetTableColumChild(string colchild)
	{
		return UnitOfWork.ORW.GetTableColumChild(colchild);
	}

	[HttpPost]
	public string SaveORW([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.UserObjectDefined.SaveData(ds, EnumCommandType.Add, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.ORW.SaveORWDataSet(ds, Enums.TransactionType.A);
	}

	[HttpPost]
	public string SaveORWUpdate([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.UserObjectDefined.SaveData(ds, EnumCommandType.Update, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.ORW.SaveORWDataSet(ds, Enums.TransactionType.U);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string RemoveObjectRegistration([FromBody] string code)
	{
		return UnitOfWork.ORW.RemoveObjectRegistration(code);
	}

	[HttpGet]
	public DataTable GetcolEditTableUDO1(int objtype, string colUDO1)
	{
		return UnitOfWork.ORW.GetcolEditTableUDO1(objtype, colUDO1);
	}

	[HttpPost]
	public string DeleteDT([FromBody] string tableName)
	{
		return UnitOfWork.ORW.DeleteDT(tableName);
	}

	[HttpGet]
	public DataSet GetFieldsForFind(string tableName, string type = "A")
	{
		return UnitOfWork.ORW.GetFieldsForFind(tableName, type);
	}

	[HttpGet]
	public DataSet GetFieldsForForm(string tableName, string type = "A")
	{
		return UnitOfWork.ORW.GetFieldsForForm(tableName, type);
	}

	[HttpGet]
	public DataSet GetFieldsChild(string tableName)
	{
		return UnitOfWork.ORW.GetFieldsChild(tableName);
	}

	[HttpPost]
	public HttpResult Unregister([FromBody] string code)
	{
		return UnitOfWork.ORW.Unregister(code);
	}

	[HttpPost]
	public HttpResult Delete([FromBody] string code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.UserObjectDefined.Delete(code, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.ORW.Delete(code);
	}
}
