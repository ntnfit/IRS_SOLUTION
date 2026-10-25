using System.Data;

namespace Apzon.Services.Interfaces.Commons;

public interface ISearchSetting
{
	DataTable GetSearchSetting(string formId, string itemId, string user);

	string SaveData(DataTable dtDoc);
}
