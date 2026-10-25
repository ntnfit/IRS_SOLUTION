using System.Collections.Generic;
using Apzon.Api.Attributes;

namespace Apzon.Api;

public class RequestDatabase
{
	public List<QueueModifiedActionAttribute> QueueRequestModifiedData { get; set; }

	public int InProcessRequestCount { get; set; }

	public List<string> ListSessionInprocess { get; set; }

	public RequestDatabase()
	{
		QueueRequestModifiedData = new List<QueueModifiedActionAttribute>();
		ListSessionInprocess = new List<string>();
	}
}
