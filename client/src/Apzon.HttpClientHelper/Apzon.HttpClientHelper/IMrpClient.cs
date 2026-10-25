using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IMrpClient
{
	Task<HttpResult> RunMrp(DataTable dt, int requestTimeout);

	Task<DataTable> GetWarehouse(int branch);

	Task<DataTable> Search(string mrpCode, DateTime? fdate, DateTime? tdate, string status);

	Task<string> GetEndateForecast(string forecastCode);

	Task<DataTable> GetDataLine(int id);

	Task<HttpResult> Delete(int id);

	Task<HttpResult> Update(DataTable dt);

	Task<HttpResult> ValidateScheduleForMrp(string lstPsCode);

	Task<DataTable> GetDataDoc(int id, Enums.NavigateType type);

	Task<HttpResult> GetNewIdMrp();

	Task<DataTable> GetSourceCellDataDocumentType();

	Task<DataTable> LoadDataSourceCell(string demandType, int demandEntry, int demandLine);

	Task<int> GetRequestTimeout();
}
