using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IJVApplicants
{
	DataSet LoadDoc(int id, string appNo);

	HttpResult Save(DataSet dt);

	DataTable CheckApplicant(string id);

	int GetMaxIdApplicant();
}
