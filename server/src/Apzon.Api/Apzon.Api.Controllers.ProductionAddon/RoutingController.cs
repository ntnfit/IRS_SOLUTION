using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class RoutingController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.Routing.GetDocs();
	}

	[HttpGet]
	public DataTable GetListRouting()
	{
		return UnitOfWork.Routing.GetListRouting();
	}

	[HttpGet]
	public DataTable GetDoc(string roucode)
	{
		return UnitOfWork.Routing.GetDoc(roucode);
	}

	[HttpPost]
	public string SaveDoc(DataTable dt)
	{
		return UnitOfWork.Routing.SaveDoc(dt);
	}

	[HttpPost]
	public string SaveDocs(DataSet ds)
	{
		return UnitOfWork.Routing.SaveDocs(ds);
	}

	[HttpGet]
	public int GetMaxLineNum(string roucode)
	{
		return UnitOfWork.Routing.GetMaxLineNum(roucode);
	}

	[HttpPost]
	public DataTable SearchField(DataTable dts)
	{
		return UnitOfWork.Routing.SearchField(dts);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.Routing.DeleteDocs(ids);
	}

	[HttpPost]
	public string DeleteAllDocByKeyCode([FromBody] string ids)
	{
		return UnitOfWork.Routing.DeleteAllDocByKeyCode(ids);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] DataTable dts)
	{
		return UnitOfWork.Routing.DeleteDoc(dts);
	}

	[HttpPost]
	public string DeleteDocsAlone([FromBody] string ids)
	{
		return UnitOfWork.Routing.DeleteDocsAlone(ids);
	}
}
