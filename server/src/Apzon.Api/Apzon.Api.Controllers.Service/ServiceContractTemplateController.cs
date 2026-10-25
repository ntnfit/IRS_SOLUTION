using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Service;

public class ServiceContractTemplateController : BaseApiController
{
	[HttpPost]
	public DataTable SearchData(DataTable dt)
	{
		return UnitOfWork.ServiceContractTemplate.SearchData(dt);
	}

	[HttpGet]
	public DataTable GetDataDoc(int id, Enums.NavigateType type)
	{
		return UnitOfWork.ServiceContractTemplate.GetDataDoc(id, type);
	}

	[HttpPost]
	public HttpResult SaveData(DataTable dt)
	{
		return UnitOfWork.ServiceContractTemplate.SaveData(dt);
	}

	[HttpPost]
	public HttpResult DeleteData(DataTable dt)
	{
		return UnitOfWork.ServiceContractTemplate.DeleteData(dt);
	}
}
