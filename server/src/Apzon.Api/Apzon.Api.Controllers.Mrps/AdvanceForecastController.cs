using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;

namespace Apzon.Api.Controllers.Mrps;

public class AdvanceForecastController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataDoc(string forecastCode, string view, Enums.NavigateType type)
	{
		new NotificationHub().SendAllUser("All user");
		return UnitOfWork.AdvanceForecast.GetDataDoc(forecastCode, view, type);
	}

	[HttpGet]
	public DataSet GetDataLine(int absId)
	{
		return UnitOfWork.AdvanceForecast.GetDataLine(absId);
	}

	[HttpGet]
	public DataTable GetColumnDoc()
	{
		return UnitOfWork.AdvanceForecast.GetColumnDoc();
	}

	[HttpGet]
	public DataTable GetColumnLine()
	{
		return UnitOfWork.AdvanceForecast.GetColumnLine();
	}

	[HttpGet]
	public DataTable GetSettingSap()
	{
		return UnitOfWork.AdvanceForecast.GetSettingSap();
	}

	[HttpPost]
	public HttpResult DeleteForecast([FromBody] int code)
	{
		return UnitOfWork.AdvanceForecast.Save(null, Enums.TransactionType.D, Function.ToString(code));
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet dtSet)
	{
		return UnitOfWork.AdvanceForecast.Save(dtSet, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet dtSet)
	{
		return UnitOfWork.AdvanceForecast.Save(dtSet, Enums.TransactionType.U, "");
	}

	[HttpGet]
	public DataTable GetDataItemCode(string itemCode)
	{
		return UnitOfWork.AdvanceForecast.GetDataItemCode(itemCode);
	}

	[HttpGet]
	public DataTable GetDataPrice(string priceList)
	{
		return UnitOfWork.AdvanceForecast.GetDataPrice(priceList);
	}

	[HttpGet]
	public HttpResult GetNewIdForecast()
	{
		return UnitOfWork.AdvanceForecast.GetNewIdForecast();
	}

	[HttpPost]
	public DataSet GenerateForecast([FromBody] InputGenerateForecastImpl inputGen)
	{
		return UnitOfWork.AdvanceForecast.GenerateForecast(inputGen);
	}

	[HttpGet]
	public DataTable GetDataBaseOnInputForecast()
	{
		return UnitOfWork.AdvanceForecast.GetDataBaseOnInputForecast();
	}
}
