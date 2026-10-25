using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IStates
{
	DataTable GetEmployeeStatus();

	DataTable GetTermReason();

	DataTable GetBranchs();

	DataTable GetCountryCode();

	DataTable GetNewRoles();

	HttpResult SaveNewRoles(DataTable dtset);

	string RemoveRoles(int objId);

	DataTable GetTeamSetup();

	HttpResult SaveTeamSetup(DataTable dt);

	string RemoveTeamSetup(int objId);

	DataTable GetEmpPos();

	HttpResult SaveEmpPos(DataTable dt);

	string RemoveEmpPos(int id);

	DataTable GetEmpStatus();

	HttpResult SaveEmpStatus(DataTable dt);

	string RemoveEmpStatus(int id);

	DataTable GetTerminationReasons();

	HttpResult SaveTerminationReasons(DataTable dt);

	string RemoveTerminationReasons(int id);

	DataTable GetDepartment();

	string RemoveDepartment(int id);

	HttpResult SaveDepartment(DataTable dt);

	DataTable GetBranches();

	string RemoveBranches(int id);

	HttpResult SaveBranches(DataTable dt);

	DataTable GetStates();

	HttpResult SaveStates(DataTable dt);

	string RemoveStates(string code);

	DataTable LoadStates(string countryCode);
}
