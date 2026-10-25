using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAlertTemplate
{
	Task<DataTable> GetAllTemplate();

	Task<HttpResult> SaveData(Tuple<DataTable, string> tupData);

	Task<HttpResult> Remove(Tuple<string> tupData);
}
