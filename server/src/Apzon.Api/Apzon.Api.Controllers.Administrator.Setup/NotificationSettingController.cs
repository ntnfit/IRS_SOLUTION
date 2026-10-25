using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class NotificationSettingController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(string user_code)
	{
		return UnitOfWork.NotificationSetting.GetData(user_code);
	}

	[HttpPost]
	public string SaveData(NotificationSettingImpl setting)
	{
		return UnitOfWork.NotificationSetting.SaveData(setting);
	}
}
