using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IEmpRelativesClient
{
	Task<DataSet> GetDetail(int relaId, int empId);

	Task<DataTable> GetAllRecords(int empId);

	Task<string> SaveDoc(DataSet data);

	Task<string> SaveDocUpdate(DataSet data);

	Task<string> DeleteDoc(int relaId, int empId);

	int GetNextId(int empId);

	bool ExistFileInDoc(int relaId, int empId, string fileName, string fileExt);

	Task<string> LoadDataNavigation(int type, int relaId, int empId);

	Task<DataTable> Search(DataTable dt);
}
