using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class InPaymentController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(int docNo)
	{
		return UnitOfWork.InPayment.GetLines(docNo);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.InPayment.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.InPayment.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.InPayment.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.InPayment.Save(ds);
	}
}
