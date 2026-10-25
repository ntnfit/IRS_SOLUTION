using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IDepartment : IService<DepartmentImpl>
{
	DataTable ShowDepartment();

	DataTable Search(DepartmentImpl doc);

	int GetMaxCode();

	bool Check(int code);

	string SaveDoc(DepartmentImpl doc);

	string DeleteDoc(int code);
}
