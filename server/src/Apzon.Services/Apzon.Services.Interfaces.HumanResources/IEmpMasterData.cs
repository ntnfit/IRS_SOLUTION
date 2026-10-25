using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IEmpMasterData
{
	string Save(DataSet ds);

	DataSet GetDetail(int empId);

	string GetRecord(int empId, int type, string userCode);

	DataTable Search(DataTable dtDoc, string userCode);

	DataTable ShowBankAccByCountryCod(string countryCod);

	DataTable ExistFileInDoc(int empId, string fileName, string fileExt);

	string DeleteDoc(int empId);

	bool CheckDoc(int empId);

	HttpResult SaveDocDataSet(DataSet ds, string action);

	string SaveWithRollBack(DataTable dt);

	DataTable GetTeamMembers();

	string GetListUserOfEmp(string lstEmp);

	DataTable GetOrgChartData(int empID);

	DataTable GetDataEmployeeDMS(int pageIndex, string searchKey, int empID, int userId);

	DataTable UploadAvatarEmployeeDMS(DataTable param);

	DataTableResult GetEmployeeInfo(int empId);
}
