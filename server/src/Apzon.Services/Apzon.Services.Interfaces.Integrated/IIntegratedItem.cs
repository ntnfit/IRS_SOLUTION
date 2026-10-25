using System.Data;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegratedItem
{
	DataTable Search(DataTable doc);

	DataSet SearchLine(DataTable dt);

	bool CheckExists(string itemCode);

	DataSet GetDocGridStruct();

	int GetManualSeries();
}
