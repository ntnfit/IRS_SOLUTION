using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.SystemInitialization;

public class SeriesController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(string objCode, string docSubType, int userId)
	{
		return UnitOfWork.Series.LoadDoc(objCode, docSubType, userId);
	}

	[HttpPost]
	public string Save(DataTable dtDoc)
	{
		return UnitOfWork.Series.Save(dtDoc);
	}

	[HttpGet]
	public int GetNextNum()
	{
		return 0;
	}
}
