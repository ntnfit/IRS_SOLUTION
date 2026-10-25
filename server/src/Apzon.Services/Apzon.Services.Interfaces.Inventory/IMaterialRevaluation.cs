using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Services.Interfaces.Inventory;

public interface IMaterialRevaluation : IService<MaterialRevaluation>
{
	DataTable CostingTable(string period);

	string Add(MaterialRevaluation doc);

	string AddDt(MaterialRevaluation doc, DataTable dt);

	string AddPart(MaterialRevaluation doc, List<MaterialRevaluation_linesImpl> list);

	DataTable LoadCBPeriod();

	DataTable LastDate(string period);

	DataTable Costing(string period);

	DataTable Cogs(string period);

	DataTable KCSX(string period);

	DataTable CostingAllocate(string period);

	bool CheckRevaluation(string period);

	bool CheckCogs(string period);

	bool CheckProd(string period);

	DataTable GetAll();
}
