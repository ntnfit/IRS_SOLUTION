using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Administrator;

namespace Apzon.HttpClientHelper;

public interface IUserClient
{
	Task<UserDefaultGroupImpl> GetUserDefaultGroup(string username);

	string GetUserIdByEmpId(int empID);

	Task<DataTable> ShowUsers();

	Task<DataTable> ShowBranch();

	Task<DataTable> ShowDepartment();

	Task<string> DeleteDoc(int code);

	Task<string> SaveDoc(UsersImpl doc);

	Task<string> UpdateUsersAcount(UsersImpl doc);

	Task<DataTable> GetUserBranchAssignment(string userCode);

	Task<DataTable> GetEmployee();

	Task<DataTable> LoadData();

	Task<DataTable> GetAnUser(int userId);
}
