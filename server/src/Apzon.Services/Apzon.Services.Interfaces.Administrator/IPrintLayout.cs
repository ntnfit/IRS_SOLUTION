using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Services.Interfaces.Administrator;

public interface IPrintLayout
{
	DataTable GetAll();

	string SaveData(DataTable dt);

	PrintLayout GetDefaultPrintLayout(string userName, string formLayoutCode, string cardCode);

	List<PrintLayout> GetLayouts(string userName, string formLayoutCode, string cardCode);

	string UpdatePrinted(string docentry, string table, string type, string userName);
}
