using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class PaymentReconController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(int docNo)
	{
		return UnitOfWork.PaymentRecon.GetLines(docNo);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.PaymentRecon.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.PaymentRecon.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.PaymentRecon.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.PaymentRecon.Save(ds);
	}
}
