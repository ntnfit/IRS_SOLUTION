using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IResourceWorkScheduleClient
{
	Task<DataTable> SearchData(string code, string status, Enums.NavigateType type);

	Task<DataTable> GetDataLine(string code, int year);

	Task<HttpResult> Delete(string code);

	Task<HttpResult> AddNew(DataSet dtSave);

	Task<HttpResult> Update(DataSet dtSave);
}
