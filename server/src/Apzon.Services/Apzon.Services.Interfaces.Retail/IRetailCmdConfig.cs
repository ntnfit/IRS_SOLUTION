using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetailCmdConfig
{
	DataTable LoadDoc();

	string Save(DataTable dtDoc);

	string Remove(int code);
}
