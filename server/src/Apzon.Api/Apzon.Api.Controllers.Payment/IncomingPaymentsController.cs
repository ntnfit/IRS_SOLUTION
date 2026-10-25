using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class IncomingPaymentsController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(int docNo)
	{
		return UnitOfWork.IncomingPayments.GetLines(docNo);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.IncomingPayments.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.IncomingPayments.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.IncomingPayments.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.IncomingPayments.Save(ds);
	}
}
