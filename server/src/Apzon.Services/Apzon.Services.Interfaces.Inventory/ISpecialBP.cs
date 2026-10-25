using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Items;

namespace Apzon.Services.Interfaces.Inventory;

public interface ISpecialBP
{
	bool CheckDoc(string cardCode, string itemCode);

	DataTable ShowCardCode(string cardCode);

	DataSet Search(string cardCode);

	string SaveDoc(List<SpecialBP_Impl> doc);

	string Delete(List<Tuple<string, string>> obj);

	DataTable GetItemByItemCode(string itemCode, int listNum);
}
