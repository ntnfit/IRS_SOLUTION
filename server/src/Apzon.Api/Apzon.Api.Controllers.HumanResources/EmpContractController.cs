using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class EmpContractController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(int empID)
	{
		return UnitOfWork.EmpContract.LoadDoc(empID);
	}

	[HttpGet]
	public DataTable LoadContractTypes()
	{
		return UnitOfWork.EmpContract.LoadContractTypes();
	}

	[HttpGet]
	public DataTable CheckContractType(int typeID)
	{
		return UnitOfWork.EmpContract.CheckContractType(typeID);
	}

	[HttpPost]
	public string SaveContract([FromBody] DataTable dt)
	{
		return UnitOfWork.EmpContract.SaveContract(dt);
	}

	[HttpPost]
	public string DeleteContract([FromBody] DataTable dt)
	{
		return UnitOfWork.EmpContract.DeleteContract(dt);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
	{
		return UnitOfWork.EmpContract.SaveData(dt);
	}
}
