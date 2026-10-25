using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IJobsManagementClient
{
	Task<DataTable> GetAllJobs();

	Task<DataTable> GetRecurInfo(int jobId);

	Task<string> SaveData(DataSet ds);

	Task<string> UpdateStatus(int jobId, string status);
}
