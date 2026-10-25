using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IUnitOfMeasureGroups
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dt);

	DataTable LoadDefinition(int ugpEntry);

	string CleanUoMGroup();

	DataTable CheckGroupInItems(int ugpEntry);

	DataTable CheckUoMInGroups(int uomEntry);

	DataTable GetDataUomInGroup(int ugpEntry, int uomEntry);

	bool CheckUoMGroupExists(int ugpEntry);

	DataTable GetUomItem(string itemCode);
}
