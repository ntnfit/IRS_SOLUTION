using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class WBSTemplateController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(string code, string fromDate, string toDate)
	{
		return UnitOfWork.WBSTemplate.LoadData(code, fromDate, toDate);
	}

	[HttpGet]
	public DataSet GetDataById(string code)
	{
		return UnitOfWork.WBSTemplate.GetDataById(code);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.WBSTemplate.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.WBSTemplate.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string code)
	{
		return UnitOfWork.WBSTemplate.DeleteDocs(code);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.WBSTemplate.FindNextLastFrist(id, type);
	}
}
