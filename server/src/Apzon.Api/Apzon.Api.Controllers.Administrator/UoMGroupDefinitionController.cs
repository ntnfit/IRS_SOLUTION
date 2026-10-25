using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class UoMGroupDefinitionController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc(int ugpEntry)
	{
		return UnitOfWork.UoMGroupDefinition.LoadDoc(ugpEntry);
	}

	[HttpPost]
	public HttpResult Save(DataTable dt)
	{
		return UnitOfWork.UoMGroupDefinition.Save(dt);
	}
}
