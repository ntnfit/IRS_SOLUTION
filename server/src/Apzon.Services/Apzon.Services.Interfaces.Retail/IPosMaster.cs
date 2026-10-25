using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IPosMaster
{
	DataTable LoadDoc(string code);

	DataTable LoadAllDoc();

	string Save(DataTable dtDoc);

	string SaveAssignment(DataTable dt);

	string Remove(string code);

	DataTable GetUserPosAssignment(string userCode);
}
