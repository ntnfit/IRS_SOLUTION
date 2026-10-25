using System.Data;

namespace Apzon.Services.Interfaces.Mrps;

public interface IItemQuotaArrangementDate
{
	DataTable DateGetDocs(int periodID);

	string DateSave(DataSet ds);

	string DeleteRows(int periodID);

	DataTable GetCardName(string CardCode);
}
