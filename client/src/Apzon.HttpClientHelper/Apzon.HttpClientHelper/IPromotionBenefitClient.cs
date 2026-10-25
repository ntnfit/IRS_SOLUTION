using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPromotionBenefitClient
{
	Task<DataSet> Search(DataTable dt);

	Task<DataSet> GetLines(int docentry);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);

	Task<string> GetNameByCode(string code);

	Task<DataTable> GetAttribute(string code);
}
