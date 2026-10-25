using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Intergrated;

[ValidateLoginSapExceptionHandling]
public class IntegratedItemController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable doc)
	{
		return UnitOfWork.IntegratedItem.Search(doc);
	}

	[HttpPost]
	public DataSet SearchLine(DataTable dt)
	{
		return UnitOfWork.IntegratedItem.SearchLine(dt);
	}

	[HttpGet]
	public bool CheckExists(string itemCode)
	{
		return UnitOfWork.IntegratedItem.CheckExists(itemCode);
	}

	[HttpGet]
	public DataSet GetDocGridStruct()
	{
		return UnitOfWork.IntegratedItem.GetDocGridStruct();
	}

	[HttpGet]
	public int GetManualSeries()
	{
		return UnitOfWork.IntegratedItem.GetManualSeries();
	}
}
