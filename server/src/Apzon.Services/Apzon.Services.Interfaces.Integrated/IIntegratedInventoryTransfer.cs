using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegratedInventoryTransfer
{
	DataSet Search(DataTable doc);

	DataSet SearchLine(string id);

	DataTable SearchLineForSave(string id);

	string SaveADoc(DocumentsImpl doc);

	DataTable CheckExists(DataTable dt);
}
