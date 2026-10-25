using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ICalendarSettingClient
{
	Task<DataSet> GetDoc(string user);

	Task<DataTable> GetGeneSettings();

	Task<string> SaveDoc(DataSet ds);

	Task<string> DeleteDoc(string code);
}
