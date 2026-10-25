using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class AdvanceRoutingController : BaseApiController
{
	[HttpPost]
	public DataTable GetDataDoc(Tuple<DataTable, Enums.NavigateType> tuple)
	{
		return UnitOfWork.AdvanceRouting.GetDataDoc(tuple);
	}

	[HttpGet]
	public DataSet GetDataLine(string routCode)
	{
		return UnitOfWork.AdvanceRouting.GetDataLine(routCode);
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataSet, Enums.TransactionType> tuple)
	{
		return UnitOfWork.AdvanceRouting.SaveData(tuple);
	}

	[HttpPost]
	public HttpResult DeleteDataDoc(string routCode)
	{
		return UnitOfWork.AdvanceRouting.DeleteDataDoc(routCode);
	}

	[HttpPost]
	public HttpResult ImportFromFile(DataSet dtSet)
	{
		return UnitOfWork.AdvanceRouting.ImportFromFile(dtSet);
	}
}
