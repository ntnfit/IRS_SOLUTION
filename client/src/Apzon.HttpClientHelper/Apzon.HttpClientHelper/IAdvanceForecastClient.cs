using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;

namespace Apzon.HttpClientHelper;

public interface IAdvanceForecastClient
{
	Task<HttpResult> Save(DataSet dsSave);

	Task<HttpResult> Update(DataSet dsSave);

	Task<HttpResult> Delete(int code);

	Task<DataTable> GetSettingSap();

	DataTable GetDataDoc(string forecastCode, string view, Enums.NavigateType type);

	DataSet GetDataLine(int absId);

	DataTable GetColumnLine();

	Task<DataTable> GetColumnDoc();

	DataTable GetDataItemCode(string itemCode);

	DataTable GetDataPrice(int priceList);

	Task<HttpResult> GetNewIdForecast();

	Task<DataSet> GenerateForecast(InputGenerateForecastImpl inputGen);
}
