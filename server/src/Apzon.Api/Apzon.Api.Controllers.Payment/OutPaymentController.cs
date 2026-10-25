using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class OutPaymentController : BaseApiController
{
	[HttpGet]
	public DataTable GetLines(int docNo)
	{
		return UnitOfWork.OutPayment.GetLines(docNo);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.OutPayment.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.OutPayment.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.OutPayment.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.OutPayment.Save(ds);
	}
}
