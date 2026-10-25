using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class PromotionSetupController : BaseApiController
{
	[HttpGet]
	public DataSet GetData()
	{
		return UnitOfWork.PromotionSetup.GetData();
	}

	[HttpPost]
	public HttpResult SaveData(DataSet ds)
	{
		return UnitOfWork.PromotionSetup.SaveData(ds);
	}

	[HttpGet]
	public string CheckUsed(string type, int id)
	{
		return UnitOfWork.PromotionSetup.CheckUsed(type, id);
	}
}
