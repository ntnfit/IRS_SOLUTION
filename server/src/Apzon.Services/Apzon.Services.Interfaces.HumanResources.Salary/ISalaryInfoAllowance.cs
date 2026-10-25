using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryInfoAllowance
{
	DataTable SearchByDate(DataTable dtDoc);

	string SaveDoc(DataTable dt);

	DataTable GetMaxLineNum(int empId);
}
