using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class DMSSettingUDFController : BaseApiController
{
	[HttpGet]
	public HttpResult DMSGetDataUDF(int menuId, string tableName, string codeField, string code)
	{
		return UnitOfWork.DMSSettingUDF.DMSGetDataUDF(menuId, tableName, codeField, code);
	}

	[HttpGet]
	public HttpResult GetData(string tableName, int menuId)
	{
		return UnitOfWork.DMSSettingUDF.GetData(tableName, menuId);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataTable dt)
	{
		return UnitOfWork.DMSSettingUDF.Save(dt, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataTable dt)
	{
		return UnitOfWork.DMSSettingUDF.Save(dt, "U");
	}
}
