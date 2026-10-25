using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IHREducation
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dt);

	DataTable GetManagerName();

	DataTable CheckEducationType(int edType);

	DataTable GetAutoKey(string ObjCode);

	int GetMaxID();
}
