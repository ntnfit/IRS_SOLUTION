using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;

namespace Apzon.Services.Interfaces.Mrps;

public interface IAdvanceForecast
{
	DataTable GetDataDoc(string forecastCode, string view, Enums.NavigateType type);

	DataSet GetDataLine(int absId);

	DataTable GetColumnDoc();

	DataTable GetColumnLine();

	DataTable GetSettingSap();

	HttpResult Save(DataSet dtset, Enums.TransactionType tranType, string objType);

	DataTable GetDataItemCode(string itemCode);

	DataTable GetDataPrice(string priceList);

	HttpResult GetNewIdForecast();

	DataSet GenerateForecast(InputGenerateForecastImpl inputGen);

	DataTable GetDataBaseOnInputForecast();
}
