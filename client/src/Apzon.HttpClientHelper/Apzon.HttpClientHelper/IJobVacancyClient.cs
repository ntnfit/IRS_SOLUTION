using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IJobVacancyClient
{
	Task<DataTable> GetData(string status, int docNo, string fromdate, string todate);

	Task<DataTable> FindDataNextLast(int id, int type);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetNo();

	Task<DataSet> GetDetail(int docId);

	Task<DataTable> SetDataLookUp(string db);

	Task<DataTable> ProfileMacthUp(string qualification);

	Task<DataTable> GetListEmpId();

	Task<HttpResult> SaveEmplMasterData(DataTable dt);

	Task<DataTable> ListBenefits();

	Task<string> DeleteDocs(string ids);

	Task<int> GetMaxDocNum();

	Task<DataTable> GetQuaById(int id);

	Task<DataTable> GetEmpById(int id);

	Task<DataTable> GetAppById(string id);

	Task<int> GetDocMaxEntry();

	Task<int> GetNextNumber(int series);
}
