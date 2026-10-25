using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAdvanceRoutingClient
{
	Task<DataTable> GetDataDoc(Tuple<DataTable, Enums.NavigateType> tuple);

	Task<DataSet> GetDataLine(string routCode);

	Task<HttpResult> SaveData(Tuple<DataSet, Enums.TransactionType> tuple);

	Task<HttpResult> DeleteDataDoc(string routCode);

	Task<HttpResult> ImportFromFile(DataSet dtSet);
}
