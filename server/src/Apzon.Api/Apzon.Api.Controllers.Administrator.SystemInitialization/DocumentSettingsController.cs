using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Administrator.SystemInitialization;

public class DocumentSettingsController : BaseApiController
{
	[HttpGet]
	public DataSet LoadDoc()
	{
		return UnitOfWork.DocumentSettings.LoadDoc();
	}

	[HttpPost]
	public string Save(DataSet dt)
	{
		if (Global.LoginSap)
		{
			return "This function is not supported";
		}
		return UnitOfWork.DocumentSettings.Save(dt);
	}

	[HttpGet]
	public DataTable GetDataSettingByObjectType(ApzObjectTypes objectType)
	{
		return UnitOfWork.DocumentSettings.GetDataSettingByObjectType(objectType);
	}
}
