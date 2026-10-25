using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;

namespace Apzon.HttpClientHelper;

public interface IAlternativeItemsClient
{
	Task<DataTable> GetItemNameByItemCode(string itemCode);

	Task<DataTable> GetDataDoc(Tuple<string, Enums.NavigateType> tuple);

	Task<DataTable> GetData(string originItem);

	Task<string> Save(Tuple<string, DataTable> tuple);

	Task<string> DeleteData(string itemNo);

	Task<DataSet> GetDataCfl(string itemCode, string userName, int branch, string cardCode, DateTime docDate);
}
