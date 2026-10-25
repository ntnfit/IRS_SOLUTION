using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources;

public class EmpRelativesController : BaseApiController
{
	[HttpPost]
	public DataTable Search(DataTable dt)
	{
		return UnitOfWork.EmpRelatives.Search(dt);
	}

	[HttpGet]
	public DataSet GetDetail(int relaId, int empId)
	{
		return UnitOfWork.EmpRelatives.GetDetail(relaId, empId);
	}

	[HttpGet]
	public string GetRecord(int relaId, int empId, int type)
	{
		return UnitOfWork.EmpRelatives.GetRecord(relaId, empId, type);
	}

	[HttpGet]
	public DataTable GetAllRecords(int empId)
	{
		return UnitOfWork.EmpRelatives.GetAllRecords(empId);
	}

	[HttpGet]
	public bool CheckDoc(int relaId, int empId)
	{
		return UnitOfWork.EmpRelatives.CheckDoc(relaId, empId);
	}

	[HttpGet]
	public int GetNextId(int empId)
	{
		return UnitOfWork.EmpRelatives.GetNextId(empId);
	}

	[HttpPost]
	public string SaveDocDataSet([FromBody] DataSet ds)
	{
		return UnitOfWork.EmpRelatives.SaveDocDataSet(ds, "A");
	}

	[HttpPost]
	public string SaveDocDataSetUpdate([FromBody] DataSet ds)
	{
		return UnitOfWork.EmpRelatives.SaveDocDataSet(ds, "U");
	}

	[HttpGet]
	public string DeleteDoc(int relaId, int empId)
	{
		return UnitOfWork.EmpRelatives.DeleteDoc(relaId, empId);
	}

	[HttpGet]
	public DataTable ExistFileInDoc(int relaId, int empId, string fileName, string fileExt)
	{
		return UnitOfWork.EmpRelatives.ExistFileInDoc(relaId, empId, fileName, fileExt);
	}
}
