using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PackingManagement;

[ValidateLoginSapExceptionHandling]
public class PackingOrderController : BaseApiController
{
	[HttpGet]
	public new DataTable GetType()
	{
		return UnitOfWork.PackingOrder.GetType();
	}

	[HttpGet]
	public DataTable GetPOCode()
	{
		return UnitOfWork.PackingOrder.GetPOCode();
	}

	[HttpGet]
	public DataTable GetWhs()
	{
		return UnitOfWork.PackingOrder.GetWhs();
	}

	[HttpGet]
	public string DelPO(int POCode)
	{
		return UnitOfWork.PackingOrder.DelPO(POCode);
	}

	[HttpGet]
	public DataTable POGetDataDoc(int POCode, Enums.NavigateType type)
	{
		return UnitOfWork.PackingOrder.POGetDataDoc(POCode, type);
	}

	[HttpPost]
	public HttpResult SaveDocDataSet([FromBody] DataSet ds)
	{
		return UnitOfWork.PackingOrder.SaveDocDataSet(ds, "A");
	}

	[HttpPost]
	public HttpResult SaveDocDataSetUpdate([FromBody] DataSet ds)
	{
		return UnitOfWork.PackingOrder.SaveDocDataSet(ds, "U");
	}
}
