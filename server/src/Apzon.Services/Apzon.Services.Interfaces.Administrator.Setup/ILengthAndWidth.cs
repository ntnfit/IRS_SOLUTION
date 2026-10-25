using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ILengthAndWidth
{
	DataTable GetData();

	HttpResult Save(DataTable dt);

	bool CheckData(string key);

	int GetMax();

	DataTable CheckExistUnitCode(int unitCode);

	HttpResult Remove(DataTable dt);
}
