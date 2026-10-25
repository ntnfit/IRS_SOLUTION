using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IWeight
{
	int GetMax();

	HttpResult Save(DataTable dtObj);

	bool Check(string key);

	DataTable GetData();

	string RemoveWeights(List<string> lstWeight);

	DataTable CheckUnitDisply(string key);
}
