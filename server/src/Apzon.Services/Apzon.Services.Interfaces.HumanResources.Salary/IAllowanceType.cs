using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface IAllowanceType
{
	DataTable GetAllowanceType();

	HttpResult SaveAllowanceType(DataTable dt);

	string RemoveAllowanceType(int code);
}
