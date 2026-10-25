using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IEmployeeMasterClient
{
	DataTable GetBankCode(string countryCod);

	Task<DataTable> LoadComboPosition();

	Task<DataTable> LoadComboDept();

	Task<DataTable> LoadComboBranch();

	Task<DataTable> LoadEmployees();

	Task<DataTable> LoadComboUserCode();

	Task<DataTable> LoadComboSalesEmp();

	Task<DataTable> LoadComboCountry();

	DataTable LoadComboState(string code, int type);

	Task<DataTable> LoadLookkUpRole();

	Task<DataTable> LoadLookUpQua();

	Task<DataTable> LoadLookUpTeam();

	Task<DataTable> GetEmployeeStatus();

	Task<DataTable> GetTermReason();

	Task<int> GetMaxId();

	Task<HttpResult> SaveDoc(DataSet ds);

	Task<HttpResult> SaveDocUpdate(DataSet ds);

	Task<string> SaveWithRollBack(DataTable dt);

	Task<string> Remove(int empID);

	int GetAutoKey();

	Task<DataTable> Search(DataTable dt);

	Task<DataSet> GetDetail(int empId);

	Task<string> LoadDataNavigation(int type, int empId);

	bool ExistFileInDoc(int empId, string fileName, string fileExt);

	Task<DataTable> GetStates();

	Task<DataTable> GetOrgChartData(int empID);

	DataTableResult GetEmployeeInfo(int empId);
}
