using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IRetailStoreClient
{
	Task<DataTable> LoadDoc(string code);

	Task<DataTable> LoadAllDoc();

	Task<string> Save(DataTable dtDoc);

	Task<string> RemoveStore(string code);
}
