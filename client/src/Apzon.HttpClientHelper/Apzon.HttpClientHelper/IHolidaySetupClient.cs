using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IHolidaySetupClient
{
	Task<DataTable> GetLine(string hldCode);

	Task<DataTable> GetAll();

	Task<string> Remove(string hldCode);

	Task<string> Save(DataSet ds);
}
