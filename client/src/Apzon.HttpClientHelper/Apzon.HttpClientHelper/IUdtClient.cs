using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IUdtClient
{
	Task<DataTable> DatDataUdt(string tableName);

	Task<string> SaveData(Tuple<DataTable, string> dataSave);
}
