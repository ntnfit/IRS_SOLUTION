using System.Data;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IProductionControl
{
	DataSet GetDocs(string cardcode, string cardname, string fromdate, string todate);

	DataTable GetDocsTabPlaned(ProductionOder_Impl_OWOR po);

	DataTable Search(ProductionControlImpl obj);

	DataTable GetDocsWhenProPlan(string ids);
}
