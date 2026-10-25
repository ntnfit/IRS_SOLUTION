using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Purchasing;

public class LandedCostsSetupController : BaseApiController
{
	[HttpGet]
	public DataTable GetLandedCostsSetup()
	{
		return UnitOfWork.LandedCostsSetup.GetLandedCostsSetup();
	}

	[HttpPost]
	public string RemoveLandedCostsSetup([FromBody] string code)
	{
		return UnitOfWork.LandedCostsSetup.RemoveLandedCostsSetup(code);
	}

	[HttpPost]
	public HttpResult SaveLandedCostsSetup([FromBody] DataTable dt)
	{
		return UnitOfWork.LandedCostsSetup.SaveLandedCostsSetup(dt);
	}
}
