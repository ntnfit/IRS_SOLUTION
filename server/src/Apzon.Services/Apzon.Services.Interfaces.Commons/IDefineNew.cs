using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IDefineNew
{
	DataTable Load(string tableName);

	HttpResult Save(Tuple<DataTable, string, string> dt);

	DataTable Check(int Id, string tableName, string fieldName);

	DataTable CheckStatusTrackingIssue(int Id, string tableName);
}
