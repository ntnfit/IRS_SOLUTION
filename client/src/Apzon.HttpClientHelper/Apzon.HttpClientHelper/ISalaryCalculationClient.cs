using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISalaryCalculationClient
{
	Task<DataSet> ViewSalary(DataTable dt);

	Task<string> SaveDoc(DataSet data);

	Task<DataSet> Calculate(DataTable dt);

	Task<DataTable> SearchEmployees(DataTable dt);

	Task<DataTable> GetSalarySlip(DataTable dt);

	Task<string> SaveSendMailStt(DataSet data);

	Task<string> SaveRptFileName(DataSet data);

	Task<DataTable> GetRptFileName();
}
