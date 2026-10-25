using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class ProjectReportCompletionController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string projectCode, string startFrom, string startTo, string status)
	{
		return UnitOfWork.ReportCompletion.LoadData(id, projectCode, startFrom, startTo, status);
	}

	[HttpGet]
	public DataTable GetDataById(int Id)
	{
		return UnitOfWork.ReportCompletion.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.ReportCompletion.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.ReportCompletion.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.ReportCompletion.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetNo()
	{
		return UnitOfWork.ReportCompletion.GetNo();
	}

	[HttpGet]
	public DataTable GetListDetail(string ids)
	{
		return UnitOfWork.ReportCompletion.GetListDetail(ids);
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] Tuple<int, string> model)
	{
		return UnitOfWork.ReportCompletion.UpdateStatus(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.ReportCompletion.FindNextLastFrist(id, type);
	}
}
