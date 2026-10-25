using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class OutletVisitingFrequencyController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(string userName, int branch)
	{
		return UnitOfWork.OutletVisitingFrequency.GetData(userName, branch);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
	{
		return UnitOfWork.OutletVisitingFrequency.Save(dt);
	}
}
