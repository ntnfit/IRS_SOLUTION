using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class CapacitySettingController : BaseApiController
{
	[HttpGet]
	public DataTable GetColumnSetting()
	{
		return UnitOfWork.CapacitySetting.GetColumnSetting();
	}

	[HttpGet]
	public DataSet GetDataSetting()
	{
		return UnitOfWork.CapacitySetting.GetDataSetting();
	}

	[HttpPost]
	public HttpResult SaveData(DataSet dsSave)
	{
		return UnitOfWork.CapacitySetting.SaveData(dsSave);
	}
}
