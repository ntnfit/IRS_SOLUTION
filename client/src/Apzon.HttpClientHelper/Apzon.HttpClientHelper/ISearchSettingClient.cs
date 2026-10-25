using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISearchSettingClient
{
	Task<DataTable> GetSearchSetting(string formId, string itemId, string user);

	Task<string> SaveData(DataTable dt);
}
