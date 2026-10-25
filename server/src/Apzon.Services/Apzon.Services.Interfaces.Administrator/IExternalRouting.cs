using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IExternalRouting
{
	DataTable GetAllRoutings();

	DataSet GetLines(int absEntry);

	string SaveDoc(DataSet ds);

	string DeleteDoc(int absEntry);
}
