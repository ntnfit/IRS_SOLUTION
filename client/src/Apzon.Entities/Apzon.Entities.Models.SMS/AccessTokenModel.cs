namespace Apzon.Entities.Models.SMS;

public class AccessTokenModel
{
	public string grant_type { get; set; }

	public string client_id { get; set; }

	public string client_secret { get; set; }

	public string scope { get; set; }

	public string session_id { get; set; }
}
