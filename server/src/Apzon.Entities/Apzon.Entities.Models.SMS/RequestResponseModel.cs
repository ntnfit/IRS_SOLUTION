namespace Apzon.Entities.Models.SMS;

public class RequestResponseModel
{
	public string error { get; set; }

	public string error_description { get; set; }

	public int MessageId { get; set; }

	public string BrandName { get; set; }

	public string Phone { get; set; }

	public string Message { get; set; }

	public string PartnerId { get; set; }

	public string Telco { get; set; }

	public string BatchId { get; set; }

	public int NumMessageSent { get; set; }

	public int NumRemainQuota { get; set; }

	public string[] success { get; set; }

	public string[] fail { get; set; }

	public string CampaignCode { get; set; }

	public string access_token { get; set; }

	public int expires_in { get; set; }

	public string token_type { get; set; }

	public string scope { get; set; }
}
