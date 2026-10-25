using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ITerritoryClient
{
	Task<DataTable> Search(string id);

	Task<DataTable> GetData();

	Task<string> SaveData(DataTable dt);

	Task<bool> ExistsLinkedObject(int focusedID);
}
