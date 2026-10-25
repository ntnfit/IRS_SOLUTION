using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IJobsManagement
{
	DataTable GetAllJobs();

	DataTable GetRecurInfo(int jobId);

	string SaveData(DataSet ds);

	string UpdateStatus(int jobId, string status);
}
