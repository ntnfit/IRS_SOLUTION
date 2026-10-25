using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class JobsManagementController : BaseApiController
{
	[HttpPost]
	public string SaveData([FromBody] DataSet ds)
	{
		return UnitOfWork.JobsManagement.SaveData(ds);
	}

	[HttpGet]
	public DataTable GetAllJobs()
	{
		return UnitOfWork.JobsManagement.GetAllJobs();
	}

	[HttpGet]
	public DataTable GetRecurInfo(int jobId)
	{
		return UnitOfWork.JobsManagement.GetRecurInfo(jobId);
	}

	[HttpGet]
	public string UpdateStatus(int jobId, string status)
	{
		return UnitOfWork.JobsManagement.UpdateStatus(jobId, status);
	}
}
