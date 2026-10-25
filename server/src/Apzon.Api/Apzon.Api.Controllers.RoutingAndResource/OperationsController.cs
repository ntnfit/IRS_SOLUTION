using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class OperationsController : BaseApiController
{
	[HttpPost]
	public DataTable SearchData(DataTable dt)
	{
		return UnitOfWork.Operations.SearchData(dt);
	}

	[HttpGet]
	public DataTable GetDataLine(string opeCode)
	{
		return UnitOfWork.Operations.GetDataLine(opeCode);
	}

	[HttpGet]
	public DataTable OperationsGetDataDoc(string OpeCode, Enums.NavigateType type)
	{
		return UnitOfWork.Operations.OperationsGetDataDoc(OpeCode, type);
	}

	[HttpPost]
	public HttpResult SaveData(DataTable dt)
	{
		return UnitOfWork.Operations.SaveData(dt);
	}

	[HttpPost]
	public HttpResult DeleteData(DataTable dt)
	{
		return UnitOfWork.Operations.DeleteData(dt);
	}
}
