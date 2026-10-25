using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Service.MarketingCampaign;

public interface IMarketingCampaign
{
	DataTable LoadData(int id, string campaignNo, string startFrom, string startTo, string status);

	DataSet GetDataById(int Id);

	DataTable GetDataTargetBPById(int id);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string Id);

	int GetMaxId();

	DataTable FindNextLastFrist(int id, int type);

	DataTable LoadTargetGroup();

	DataTable LoadTargetGroupDetails(string code);
}
