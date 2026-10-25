using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISMSClient
{
	Task<DataTable> GetConfig();

	Task<string> SaveConfig(DataTable dt);

	Task<string> CreateCampagin(string campaignName, string brandName, string message, string scheduleTime, int quota);

	Task<string> SendOTPMessage(string brandName, string phoneNumber, string message);

	Task<string> SendCampaignAd(string campaignCode, string[] phoneList);

	Task<string> CancelCampaignAd(string campaignCode);

	Task<string> CreateAndSendCampaign(string campaignName, string brandName, string message, string scheduleTime, int quota, string[] phoneList);
}
