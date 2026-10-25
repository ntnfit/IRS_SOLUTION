using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IUsers : IService<UsersImpl>
{
	DataTable ShowUsers();

	DataTable Search(UsersImpl doc);

	int GetMaxCode();

	bool Check(int code);

	string SaveDoc(UsersImpl doc);

	string DeleteDoc(int code);

	DataTable ShowBranch();

	string UpdateUsersAcount(UsersImpl doc);

	bool CheckUsersAcount(string userCode);

	DataTable GetUserLogin(string madk);

	DataTable GetUserSign(string nameLogin);

	DataTable GetUsersByCode(string userCode);

	DataSet GetAll();

	UserDefaultGroupImpl GetUserDefaultGroup(string username);

	bool CheckUser_Code(string user);

	string GetUserNameById(int id);

	string GetLstUserNameByCode(string lstCode);

	string GetUserCodeById(int id);

	string GetUserIdByEmpId(int empID);

	DataTable GetUserBranchAssignment(string userCode);

	DataTable GetEmployee();

	DataTable LoadData();

	DataTable GetAnUser(int userId);

	DataTable DataPassAdmin();
}
