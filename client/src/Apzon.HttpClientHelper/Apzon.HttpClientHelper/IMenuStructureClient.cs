using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IMenuStructureClient
{
	Task<DataSet> GetListMenu(string lngCode = "");

	Task<string> AddNewFolder(DataTable dr);

	Task<string> RelocateFolder(DataTable dr);

	Task<string> Rename(DataTable dr);

	Task<string> DeleteFolder(int id);

	Task<DataTable> GetListCustomizeMenu();
}
