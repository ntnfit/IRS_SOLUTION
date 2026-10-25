using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ILeaveParameter
{
	DataTable GetDoc();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int code);
}
