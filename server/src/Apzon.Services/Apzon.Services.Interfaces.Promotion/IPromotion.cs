using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Promotion;

public interface IPromotion
{
	DataTable GetListPromotions(int promotionType, string cardCode, string promotionCode, string groupCode, string userCode);

	DataTable GetHeader(string usercode, string code);

	DataSet GetDetailByCode(string code, string groupCode, string userCode);

	DataTable LoadCondition(string code);

	DataTable LoadBenefit(string code);

	HttpResult Save(DataSet model, string usercode);

	DataSet GetSyncData();

	DataTable CheckPromotion(DataTable input);
}
