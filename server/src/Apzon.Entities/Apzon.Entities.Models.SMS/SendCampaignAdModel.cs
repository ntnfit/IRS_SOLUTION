namespace Apzon.Entities.Models.SMS;

public class SendCampaignAdModel
{
	public string access_token { get; set; }

	public string session_id { get; set; }

	public string CampaignCode { get; set; }

	public string PhoneList { get; set; }
}
