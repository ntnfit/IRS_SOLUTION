using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Service;

public interface IContractTemplate
{
	DataTable GetData(string tmpltName);

	string SaveDoc(DataSet ds);

	DataTable CheckDoc(string tmpltName);

	DataTable GetRecord(string position, string tmpltName);

	DataSet Search(DataTable dt);
}
