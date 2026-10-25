using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IBranches : IService<DataTable>
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dtDoc);

	DataTable GetMaxCode();

	DataTable CheckExistBPLInOWHS(Tuple<string, string> tup);
}
