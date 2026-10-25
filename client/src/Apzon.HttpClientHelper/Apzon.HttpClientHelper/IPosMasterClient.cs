using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IPosMasterClient
{
	Task<DataTable> LoadAllDoc();

	Task<string> Save(DataTable dtDoc);

	Task<string> SaveAssignment(DataTable dt);

	Task<string> Remove(string code);

	Task<DataTable> GetUserPosAssignment(string userCode);
}
