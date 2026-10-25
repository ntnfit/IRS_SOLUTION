using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class JobVacancyController : BaseApiController
{
	[HttpGet]
	public DataTable FindDataNextLast(int id, int type)
	{
		return UnitOfWork.JobVacancy.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetData(string status, int docNo, string fromdate, string todate)
	{
		return UnitOfWork.JobVacancy.GetData(status, docNo, fromdate, todate);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.JobVacancy.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.JobVacancy.Save(ds, "U");
	}

	[HttpGet]
	public DataTable GetNo()
	{
		return UnitOfWork.JobVacancy.GetNo();
	}

	[HttpGet]
	public DataSet GetDetail(int docId)
	{
		return UnitOfWork.JobVacancy.GetDetail(docId);
	}

	[HttpGet]
	public DataTable SetDataLookUp(string db)
	{
		return UnitOfWork.JobVacancy.SetDataLookUp(db);
	}

	[HttpGet]
	public DataTable ProfileMacthUp(string qualification)
	{
		return UnitOfWork.JobVacancy.ProfileMacthUp(qualification);
	}

	[HttpGet]
	public DataTable GetListEmpId()
	{
		return UnitOfWork.JobVacancy.GetListEmpId();
	}

	[HttpGet]
	public DataTable ListBenefits()
	{
		return UnitOfWork.JobVacancy.ListBenefits();
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.JobVacancy.DeleteDocs(ids);
	}

	[HttpGet]
	public int GetMaxDocNum()
	{
		return UnitOfWork.JobVacancy.GetMaxDocNum();
	}

	[HttpGet]
	public DataTable GetQuaById(int id)
	{
		return UnitOfWork.JobVacancy.GetQuaById(id);
	}

	[HttpGet]
	public DataTable GetEmpById(int id)
	{
		return UnitOfWork.JobVacancy.GetEmpById(id);
	}

	[HttpGet]
	public DataTable GetAppById(string id)
	{
		return UnitOfWork.JobVacancy.GetAppById(id);
	}

	[HttpGet]
	public int GetDocMaxEntry()
	{
		return UnitOfWork.JobVacancy.GetDocMaxEntry();
	}

	[HttpGet]
	public int GetNextNumber(int series)
	{
		return UnitOfWork.JobVacancy.GetNextNumber(series);
	}
}
