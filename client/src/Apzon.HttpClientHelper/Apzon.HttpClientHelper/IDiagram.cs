using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public interface IDiagram
{
	Task<DataTable> GetDiagram(ApzObjectTypes type, string objId);
}
