using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryInfo
{
	string SaveDoc(DataTable dt);

	DataSet Search(DataTable dtDoc);

	DataTable SearchByDate(DataTable dtDoc);

	DataTable GetMaxLineNum(int empId);
}
