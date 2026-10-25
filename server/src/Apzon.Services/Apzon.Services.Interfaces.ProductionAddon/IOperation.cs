using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IOperation
{
	DataTable GetDocs();

	string DeleteDocs(string ids);

	string SaveDocs(DataTable dt);

	DataTable LoadListOperation();

	DataTable GetSuoOpCode();

	DataTable LoadListOperationByStatus();
}
