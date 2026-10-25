using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class OutgoingPaymentsController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(int docNo)
	{
		return UnitOfWork.OutgoingPayments.GetLines(docNo);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.OutgoingPayments.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.OutgoingPayments.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.OutgoingPayments.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.OutgoingPayments.Save(ds);
	}
}
