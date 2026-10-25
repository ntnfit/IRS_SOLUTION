using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IMailSetting
{
	DataTable GetDoc();

	string SaveDoc(DataSet ds);

	string DeleteDoc(string code);
}
