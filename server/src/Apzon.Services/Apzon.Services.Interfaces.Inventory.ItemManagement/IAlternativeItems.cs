using System;
using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Services.Interfaces.Inventory.ItemManagement;

public interface IAlternativeItems
{
	DataTable GetItemNameByItemCode(string itemCode);

	DataTable GetDataDoc(Tuple<string, Enums.NavigateType> tuple);

	DataTable GetData(string originItem);

	DataSet GetDataCfl(string itemCode, string userName, int branch, string cardCode, DateTime docDate);
}
