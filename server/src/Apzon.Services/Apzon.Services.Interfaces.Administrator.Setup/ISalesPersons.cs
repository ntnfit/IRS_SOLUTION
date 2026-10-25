using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ISalesPersons : IService<SalesPersonsImpl>
{
	List<SalesPersonsImpl> Cfl();

	DataTable GetStructure();
}
