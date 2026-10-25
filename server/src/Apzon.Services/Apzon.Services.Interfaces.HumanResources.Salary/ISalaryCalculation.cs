using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryCalculation
{
	string SaveDoc(DataSet ds);

	DataSet Calculalte(DataTable dtDoc);

	DataSet ViewSalaryCalculation(DataTable dtDoc);

	DataTable SearchEmployees(DataTable dtDoc);

	DataTable GetSalarySlip(DataTable dtDoc);

	string SaveSendMailStt(DataSet ds);

	string SaveRptFileName(DataSet ds);

	DataTable GetRptFileName();
}
