using System.Collections.Generic;

namespace Apzon.Api.Models;

public class SessionUserInfoImpl
{
	public string UserName { get; set; }

	public string DatabaseName { get; set; }

	public string SessionDiServer { get; set; }

	public List<string> ListLicense { get; set; }

	public List<KeySocketModel> ListSocketClientConect { get; set; }
}
