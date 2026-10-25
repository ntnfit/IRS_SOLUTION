using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPromotionClient
{
	Task<DataTable> GetHeader(string code);

	Task<DataTable> GetListPromotions(int promotionType, string cardCode, string promotionCode, string groupCode);

	Task<DataTable> LoadCondition(string code);

	Task<DataTable> LoadBenefit(string code);

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> CheckPromotion(DataTable input);

	Task<HttpResult> SaveAppliedBenefit(DataTable model);

	Task<DataTable> GetExistDocPromotion(string guid);
}
