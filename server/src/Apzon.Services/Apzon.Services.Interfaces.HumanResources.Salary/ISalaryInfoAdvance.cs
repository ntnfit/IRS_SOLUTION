using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryInfoAdvance
{
	DataTable SearchByPeriod(DataTable dt);

	DataTable GetAdvancedInfo(int empId);

	string SaveDoc(DataTable dt);

	string DeleteDoc(int lineNum, int empId);

	DataTable GetWorkPeriodsByBranch(int branch);
}
