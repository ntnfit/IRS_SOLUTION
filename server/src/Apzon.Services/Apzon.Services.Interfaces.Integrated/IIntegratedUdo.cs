using System.Data;

namespace Apzon.Services.Interfaces.Integrated;

public interface IIntegratedUdo
{
	DataTable Search(DataTable doc);

	DataSet SearchLine(DataTable dt);

	bool CheckExists(string id, string code, string type);

	DataSet GetDocGridStruct(string udoCode);

	DataTable LoadUdoCombo();

	DataSet GetDocLineGridStruct(string udoCode);

	DataTable GetAllChildCode(string udoCode);
}
