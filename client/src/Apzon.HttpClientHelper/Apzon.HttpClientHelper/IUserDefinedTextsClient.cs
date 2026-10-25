using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IUserDefinedTextsClient
{
	Task<DataTable> GetAll();

	Task<string> Save(DataTable dataTable);

	Task<string> Remove(int absEntry);
}
