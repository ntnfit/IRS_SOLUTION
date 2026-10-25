using System.Data;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IEmpPenalization
{
	DataTable GetData(int docNo);

	DataTable GetPeriodRecords();

	string Save(DataSet ds);

	string Remove(int docNo);
}
