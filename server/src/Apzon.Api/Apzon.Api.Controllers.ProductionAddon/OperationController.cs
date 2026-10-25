using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class OperationController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.Operation.GetDocs();
	}

	[HttpGet]
	public DataTable LoadListOperation()
	{
		return UnitOfWork.Operation.LoadListOperation();
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.Operation.DeleteDocs(ids);
	}

	[HttpPost]
	public string SaveDocs(DataTable dt)
	{
		return UnitOfWork.Operation.SaveDocs(dt);
	}

	[HttpGet]
	public DataTable GetSuoOpCode()
	{
		return UnitOfWork.Operation.GetSuoOpCode();
	}

	[HttpGet]
	public DataTable LoadListOperationByStatus()
	{
		return UnitOfWork.Operation.LoadListOperationByStatus();
	}
}
