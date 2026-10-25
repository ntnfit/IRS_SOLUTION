using System.Data;

namespace Apzon.HttpClientHelper;

public interface IIndustryClient
{
	DataTable LoadData();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int dt);

	bool ExistsIndCode(int indCode);
}
