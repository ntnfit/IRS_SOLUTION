using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Retail;

public class PosMasterController : BaseApiController
{
	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.PosMaster.Save(dt);
	}

	[HttpPost]
	public string SaveAssignment([FromBody] DataTable dt)
	{
		return UnitOfWork.PosMaster.SaveAssignment(dt);
	}

	[HttpPost]
	public string Remove([FromBody] string code)
	{
		return UnitOfWork.PosMaster.Remove(code);
	}

	[HttpGet]
	public DataTable LoadDoc(string code)
	{
		return UnitOfWork.PosMaster.LoadDoc(code);
	}

	[HttpGet]
	public DataTable LoadAllDoc()
	{
		return UnitOfWork.PosMaster.LoadAllDoc();
	}

	[HttpGet]
	public DataTable GetUserPosAssignment(string userCode)
	{
		return UnitOfWork.PosMaster.GetUserPosAssignment(userCode);
	}
}
