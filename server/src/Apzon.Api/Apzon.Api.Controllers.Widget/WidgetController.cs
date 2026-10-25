using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Widget;

public class WidgetController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocument(int widgetEntry)
	{
		return UnitOfWork.Widget.GetDocument(widgetEntry);
	}

	[HttpGet]
	public DataTable GetItems(int widgetEntry)
	{
		return UnitOfWork.Widget.GetItems(widgetEntry);
	}

	[HttpGet]
	public DataTable GetHeader(int widgetEntry)
	{
		return UnitOfWork.Widget.GetHeader(widgetEntry);
	}

	public DataTable GetPages(int widgetEntry)
	{
		return UnitOfWork.Widget.GetPages(widgetEntry);
	}

	[HttpPost]
	public HttpResult Save(DataSet ds)
	{
		return UnitOfWork.Widget.Save(ds);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.Widget.GetAll();
	}

	[HttpPost]
	public HttpResult Remove(int absEntry)
	{
		return UnitOfWork.Widget.Remove(absEntry);
	}
}
