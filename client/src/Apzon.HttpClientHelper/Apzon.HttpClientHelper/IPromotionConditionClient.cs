using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPromotionConditionClient
{
	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);

	Task<string> GetNameByCode(string conditionCode);

	Task<DataTable> GetAttribute(string code, string objectType);

	Task<DataSet> SearchPromotionCondition(DataTable dt);

	Task<DataSet> GetLines(int docentry);
}
