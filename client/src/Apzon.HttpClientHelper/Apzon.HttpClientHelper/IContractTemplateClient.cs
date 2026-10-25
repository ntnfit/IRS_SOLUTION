using System.Data;

namespace Apzon.HttpClientHelper;

public interface IContractTemplateClient
{
	DataTable GetData(string tmpltName);

	string SaveDoc(DataSet ds);

	DataTable GetRecord(string position, string tmpltName);

	DataTable CheckDoc(string tmpltName);

	DataSet Search(DataTable dt);
}
