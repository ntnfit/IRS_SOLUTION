using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.KPIs;

public class KPIsMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(string code, string name, string status)
	{
		return UnitOfWork.KPIsMaterData.LoadData(code, name, status);
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.KPIsMaterData.Saves(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.KPIsMaterData.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable LoadDataFormulaCode()
	{
		return UnitOfWork.KPIsMaterData.LoadDataFormulaCode();
	}
}
