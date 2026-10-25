using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAboutRangeTimeClient
{
	Task<DataTable> GetData();

	Task<HttpResult> SaveData(DataTable dtSave);

	Task<HttpResult> Delete(int docEntry);
}
