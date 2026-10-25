using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDefineNewClient
{
	DataTable Load(string tableName);

	Task<DataTable> LoadAsync(string tableName);

	DataTable Check(int Id, string tableName, string fieldName);

	DataTable CheckStatusTrackingIssue(int Id, string tableName);

	HttpResult Save(Tuple<DataTable, string, string> dt);
}
