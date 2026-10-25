using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetailStore
{
	DataTable LoadDoc(string code);

	DataTable LoadAllDoc();

	string Save(DataTable dtDoc);

	string RemoveStore(string code);
}
