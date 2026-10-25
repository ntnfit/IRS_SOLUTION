using Newtonsoft.Json;

namespace Apzon.DataServices;

public class SessionObj
{
	public string SessionId { get; set; }

	[JsonProperty(PropertyName = "odata.metadata")]
	public string BaseUrl { get; set; }

	public string Version { get; set; }

	public int SessionTimeout { get; set; }
}
