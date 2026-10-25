using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.KPIs;

public class SetIndicatorKPIsController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(string code, string name)
	{
		return UnitOfWork.SetIndicatorKPIs.LoadData(code, name);
	}

	[HttpGet]
	public DataTable GetDataById(string code)
	{
		return UnitOfWork.SetIndicatorKPIs.GetDataById(code);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.SetIndicatorKPIs.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.SetIndicatorKPIs.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string code)
	{
		return UnitOfWork.SetIndicatorKPIs.DeleteDocs(code);
	}
}
