using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Retail;

public class RetailStoreController : BaseApiController
{
	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.RetailStore.Save(dt);
	}

	[HttpPost]
	public string RemoveStore([FromBody] string code)
	{
		return UnitOfWork.RetailStore.RemoveStore(code);
	}

	[HttpGet]
	public DataTable LoadDoc(string code)
	{
		return UnitOfWork.RetailStore.LoadDoc(code);
	}

	[HttpGet]
	public DataTable LoadAllDoc()
	{
		return UnitOfWork.RetailStore.LoadAllDoc();
	}
}
