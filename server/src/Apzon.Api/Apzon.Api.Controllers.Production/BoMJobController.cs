using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Production;

public class BoMJobController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.BoMJob.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(string Code)
	{
		return UnitOfWork.BoMJob.GetDoc(Code);
	}

	[HttpGet]
	public DataTable GetRawMater(string father)
	{
		return UnitOfWork.BoMJob.GetRawMater(father);
	}

	[HttpGet]
	public DataTable GetJob(string father)
	{
		return UnitOfWork.BoMJob.GetJob(father);
	}

	[HttpGet]
	public DataTable GetJobItems(string father)
	{
		return UnitOfWork.BoMJob.GetJobItems(father);
	}

	[HttpGet]
	public int GetNewJobNum()
	{
		return UnitOfWork.BoMJob.GetNewJobNum();
	}

	[HttpGet]
	public DataTable GetResourceCFL()
	{
		return UnitOfWork.BoMJob.GetResourceCFL();
	}

	[HttpPost]
	public DataTable IsRawMaterExists([FromBody] List<string> lst)
	{
		return UnitOfWork.BoMJob.IsRawMaterExists(lst);
	}

	[HttpPost]
	public DataTable IsJobExists([FromBody] List<string> lst)
	{
		return UnitOfWork.BoMJob.IsJobExists(lst);
	}

	[HttpPost]
	public DataTable IsBoMExists([FromBody] string itemCode)
	{
		return UnitOfWork.BoMJob.IsBoMExists(itemCode);
	}

	[HttpPost]
	public DataTable IsJobItemExists([FromBody] List<string> lst)
	{
		return UnitOfWork.BoMJob.IsJobItemExists(lst);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc(DataSet ds)
	{
		return UnitOfWork.BoMJob.SaveDoc(ds);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.BoMJob.DeleteDocs(ids);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteRawMaters([FromBody] string fatherAndchildNums)
	{
		return UnitOfWork.BoMJob.DeleteRawMaters(fatherAndchildNums);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteJobs([FromBody] string fatherAndchildNums)
	{
		return UnitOfWork.BoMJob.DeleteJobs(fatherAndchildNums);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteJobItems([FromBody] string fatherFNumAndlineNums)
	{
		return UnitOfWork.BoMJob.DeleteJobItems(fatherFNumAndlineNums);
	}

	[HttpPost]
	public DataTable SearchDoc(DataTable dt)
	{
		return UnitOfWork.BoMJob.SearchDoc(dt);
	}
}
