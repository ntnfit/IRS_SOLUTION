using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface IPerIncomeTax
{
	DataTable FindDataNextLast_PerInTax(int id, int type);

	DataTable GetPerIncomeTax();

	DataTable GetDetail(int code);

	string RemovePerIncomeTax(string code);

	HttpResult SavePersonalIncomeTax(DataSet dS, string type);

	int GetMaxCode();
}
