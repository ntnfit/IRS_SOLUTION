using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IIrsConfigClient
{
	Task<DataTable> GetTableStructure(string tblName);
}
