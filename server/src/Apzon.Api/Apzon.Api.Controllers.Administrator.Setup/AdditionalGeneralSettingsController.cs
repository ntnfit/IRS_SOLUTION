using System.Data;
using System.Web.Mvc;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class AdditionalGeneralSettingsController : BaseApiController
{
	[HttpGet]
	public DataSet GetData()
	{
		return UnitOfWork.AdditionalGeneralSettings.GetData();
	}

	[HttpPost]
	public HttpResult Save(DataSet ds)
	{
		return UnitOfWork.AdditionalGeneralSettings.Save(ds);
	}

	[HttpGet]
	public DataSet GetData_Hana()
	{
		return new DataSet();
	}

	[HttpGet]
	public string GetDefaultLng()
	{
		return UnitOfWork.AdditionalGeneralSettings.GetDefaultLng();
	}
}
