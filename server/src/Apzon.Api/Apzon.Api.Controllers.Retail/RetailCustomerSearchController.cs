using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Retail;

public class RetailCustomerSearchController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(string cardType, string userName, string branch)
	{
		return UnitOfWork.CustomerSearch.LoadDoc(cardType, userName, branch);
	}
}
