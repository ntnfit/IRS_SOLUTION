using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory;

public interface IPacking
{
	DataTable GetDoc(DataTable pr);

	DataTable GetLines(int docEntry);

	int GetNextId();

	DataTable GetLineByPickNo(int id);

	DataSet ChoosePickList(string cardcode);

	HttpResult Save(DataSet ds);

	DataTable GetLineByPackNo(int id);

	DataSet ChoosePacking();

	DataSet ChooseBatch();

	HttpResult Close(int docEntry);

	HttpResult Cancel(int docEntry);
}
