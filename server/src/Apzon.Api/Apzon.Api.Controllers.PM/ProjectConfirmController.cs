using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class ProjectConfirmController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string projectCode, string startFrom, string startTo, string status)
	{
		return UnitOfWork.ProjectConfirm.LoadData(id, projectCode, startFrom, startTo, status);
	}

	[HttpGet]
	public DataTable GetDataById(int Id)
	{
		return UnitOfWork.ProjectConfirm.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.ProjectConfirm.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.ProjectConfirm.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.ProjectConfirm.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetNo()
	{
		return UnitOfWork.ProjectConfirm.GetNo();
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] Tuple<int, string> model)
	{
		return UnitOfWork.ProjectConfirm.UpdateStatus(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.ProjectConfirm.FindNextLastFrist(id, type);
	}
}
