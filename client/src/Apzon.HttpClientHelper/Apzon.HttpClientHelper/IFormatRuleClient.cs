using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFormatRuleClient
{
	Task<DataTable> GetFormatRule(string formId, string itemId, string user);

	Task<HttpResult> Saves(DataTable dt);
}
