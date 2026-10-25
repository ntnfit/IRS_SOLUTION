using System.Data;

namespace Apzon.Services.Interfaces.Administrator.SystemInitialization;

public interface ICompanyDetails
{
	DataTable GetOADM();

	DataTable GetADM1();

	DataTable GetHoliday();

	DataTable GetTableColumns(string tableName);

	string Save(DataSet ds);

	DataTable GetCountry();

	DataTable GetState(string codeCountry);
}
