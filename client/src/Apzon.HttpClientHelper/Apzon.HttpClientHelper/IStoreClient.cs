using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IStoreClient
{
	Task<DataTable> GetStore(string storeCode = "", string cardCode = "", string status = "");

	Task<DataTable> GetEvents(int clgCode);

	Task<DataTable> GetStoreEvents(string storeCode);

	Task<DataTable> GetGeography();

	Task<DataTable> GetRecord(string position, string storeCode);

	Task<DataTable> GetLastEvent(string storeCode);

	Task<HttpResult> Save(Tuple<DataSet, string> tuple);

	Task<HttpResult> CheckRemove(string storeCode);

	Task<HttpResult> Remove(Tuple<string> tuple);

	Task<DataTable> GetParentsGeography(string ggpCode);
}
