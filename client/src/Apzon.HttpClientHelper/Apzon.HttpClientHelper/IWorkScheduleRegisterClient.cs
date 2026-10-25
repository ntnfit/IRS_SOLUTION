using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IWorkScheduleRegisterClient
{
	Task<string> SaveDoc(DataTable data);

	Task<DataTable> Search(DataTable dt);

	Task<DataTable> GetDetail(int empID);

	Task<string> Remove(int lineNum, int empID);
}
