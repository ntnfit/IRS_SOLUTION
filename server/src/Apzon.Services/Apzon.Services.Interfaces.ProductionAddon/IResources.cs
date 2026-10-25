using System.Data;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IResources
{
	DataTable GetDocs();

	DataTable GetListResCode();

	DataTable GetCapacityChildren(string rescode);

	DataTable GetDocTabFix(string rescode);

	DataTable GetDocTabCosting(string rescode);

	string Save(DataSet ds);

	DataTable GetDate(string resCode);

	DataTable GetCapacityByRes(Obj_Date obj);

	DataTable GetMaxLineNumByResCode(string resCode);

	string DeleteDocs(string resCode);
}
