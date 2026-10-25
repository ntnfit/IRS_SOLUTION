using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class HolidayController : BaseApiController
{
	[HttpGet]
	public DataTable ShowHoliday()
	{
		return UnitOfWork.Holiday.ShowHoliday();
	}

	[HttpGet]
	public DataTable GetMaxCode()
	{
		return UnitOfWork.Holiday.GetMaxCode();
	}

	[HttpGet]
	public string DeleteDoc(string hldCode)
	{
		return UnitOfWork.Holiday.DeleteDoc(hldCode);
	}

	[HttpGet]
	public DataTable Search()
	{
		return UnitOfWork.Holiday.Search();
	}

	[HttpGet]
	public DataTable GetHoliday(HolidayImpl _docSearch)
	{
		return UnitOfWork.Holiday.GetHoliday(_docSearch);
	}

	[HttpGet]
	public DataTable SearchLine(string hldCode)
	{
		return UnitOfWork.Holiday.SearchLine(hldCode);
	}

	[HttpPost]
	public string SaveDoc([FromBody] HolidayImpl doc)
	{
		return UnitOfWork.Holiday.SaveDoc(doc);
	}
}
