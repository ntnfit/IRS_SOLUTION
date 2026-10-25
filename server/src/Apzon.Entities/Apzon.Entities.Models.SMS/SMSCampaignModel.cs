namespace Apzon.Entities.Models.SMS;

public class SMSCampaignModel
{
	public string access_token { get; set; }

	public string session_id { get; set; }

	public string CampaignName { get; set; }

	public string BrandName { get; set; }

	public string Message { get; set; }

	public string ScheduleTime { get; set; }

	public int Quota { get; set; }
}
