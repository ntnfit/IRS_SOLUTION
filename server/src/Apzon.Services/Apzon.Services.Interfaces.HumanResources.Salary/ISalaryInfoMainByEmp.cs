using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryInfoMainByEmp
{
	DataTable GetDetail(int empId);

	string SaveDoc(DataTable dt);

	string DeleteDoc(int lineNum, int empId);
}
