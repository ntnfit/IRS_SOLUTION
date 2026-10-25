using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory;

public interface IShipping
{
	DataTable GetDoc(DataTable pr);

	DataTable GetLines(int docEntry);

	int GetNextId();

	DataTable GetLineByPickNo(int id);

	DataSet ChoosePickList();

	HttpResult Save(DataSet ds);

	DataTable GetLineByPackNo(int id);

	DataSet ChoosePacking();

	DataSet ChooseBatch();

	HttpResult Close(int docEntry);

	HttpResult Cancel(int docEntry);

	DataSet ChooseList(ApzObjectTypes type, string cardcode);

	DataTable Copy(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lstEntry);

	DataTable CopyHeader(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lstEntry);

	int GetOrderEntry(ApzObjectTypes type, int entry);
}
