using System.Collections.Generic;
using System.Data;

namespace Apzon.Services.Interfaces.Production;

public interface IBoMJob
{
	string SaveDoc(DataSet ds);

	DataTable GetDocs();

	DataTable GetDoc(string Code);

	DataTable GetRawMater(string father);

	DataTable GetJob(string father);

	DataTable GetJobItems(string jobNums);

	int GetNewJobNum();

	DataTable SearchDoc(DataTable dt);

	DataTable GetResourceCFL();

	string DeleteDocs(string ids);

	string DeleteJobs(string fatherAndchildNums);

	string DeleteJobItems(string fatherFNumAndlineNums);

	string DeleteRawMaters(string fatherAndchildNums);

	DataTable IsRawMaterExists(List<string> lst);

	DataTable IsJobExists(List<string> lst);

	DataTable IsBoMExists(string itemCode);

	DataTable IsJobItemExists(List<string> lst);
}
