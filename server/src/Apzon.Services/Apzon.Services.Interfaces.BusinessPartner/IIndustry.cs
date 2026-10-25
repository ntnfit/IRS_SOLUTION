using System.Data;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IIndustry
{
	DataTable LoadData();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int indCode);

	bool ExistsIndCode(int indCode);
}
