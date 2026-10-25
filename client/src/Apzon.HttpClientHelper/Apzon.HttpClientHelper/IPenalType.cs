using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IPenalType
{
	Task<DataTable> GetData();

	Task<string> Remove(int code);

	Task<string> Save(DataTable dt);
}
