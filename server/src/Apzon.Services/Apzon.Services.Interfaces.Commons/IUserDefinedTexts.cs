using System.Data;

namespace Apzon.Services.Interfaces.Commons;

public interface IUserDefinedTexts
{
	DataTable GetAll();

	string Save(DataTable dataTable);

	string Remove(int absEntry);
}
