using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegrated
{
	DataTable Search(DataTable doc);

	DataTable SearchLine(int objType, string id, string isIns, string isRecFromProd);

	DataTable SearchLineForSave(int objType, string id, string isIns, string isRecFromProd);

	string SaveADoc(DocumentsImpl doc);

	DataTable GetListConfig();

	string MapCodeSaveADoc(DataTable doc);

	DataTable CheckExists(DataTable dt);

	string CheckIRSAuthorizations();

	DataSet GetDocGridStruct();

	DataSet GetDocLineGridStruct();
}
