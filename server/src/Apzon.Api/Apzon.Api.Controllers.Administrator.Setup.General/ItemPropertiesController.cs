using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class ItemPropertiesController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(string tableName)
	{
		return UnitOfWork.Commons.LoadDoc(tableName);
	}

	[HttpPost]
	public HttpResult SaveOITG([FromBody] DataTable dtDoc)
	{
		return UnitOfWork.ItemProperties.SaveItemProperties(dtDoc);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}
}
