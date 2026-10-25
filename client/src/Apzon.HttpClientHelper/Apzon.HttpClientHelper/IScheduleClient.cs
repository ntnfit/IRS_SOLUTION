using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IScheduleClient
{
	Task<DataSet> GetDataLine(int id);

	Task<HttpResult> Save(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetDataDoc(int id, Enums.NavigateType type);

	Task<DataTable> GetDataMrp(int mrpCode);

	Task<DataTable> GetDataProductionOrder(string itemCode, string version, string sourceType, int sourceId, int sourceLine);

	Task<DataTable> GetDataWhs(string inputType, int inputId);

	HttpResult GetNewIdSchedule();

	DataTable GetdataBpItem(string bpCode, string itemCode, string shipDate);

	Task<DataTable> GetDocStructure();

	Task<string> CheckExistsDocInType(string code, string genType, string inputType);
}
