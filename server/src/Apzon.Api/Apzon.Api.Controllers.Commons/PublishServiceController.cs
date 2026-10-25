using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class PublishServiceController : BaseApiController
{
	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		return UnitOfWork.PublishService.Save(ds);
	}

	[HttpGet]
	public DataTable Search(string controllerName, string actionName, string description)
	{
		return UnitOfWork.PublishService.Search(controllerName, actionName, description);
	}

	[HttpGet]
	public DataSet GetById(int Id)
	{
		return UnitOfWork.PublishService.GetById(Id);
	}

	[HttpGet]
	public string Navigate(int id, int type)
	{
		return UnitOfWork.PublishService.Navigate(id, (Enums.NavigateType)type);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] int absEntry)
	{
		return UnitOfWork.PublishService.Remove(absEntry);
	}
}
