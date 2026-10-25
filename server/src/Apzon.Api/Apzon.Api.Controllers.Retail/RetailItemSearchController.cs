using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Retail;

public class RetailItemSearchController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(string itemCode, string itemType, string userName, string branch, string pr)
	{
		return UnitOfWork.ItemSearch.LoadDoc(itemCode, itemType, userName, branch, pr);
	}
}
