using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IMailSettingClient
{
	Task<DataTable> GetDoc();

	Task<string> SaveDoc(DataSet ds);

	Task<string> DeleteDoc(string code);
}
