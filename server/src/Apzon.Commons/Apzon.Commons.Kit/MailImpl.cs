using System.Collections.Generic;
using System.IO;

namespace Apzon.Commons.Kit;

public class MailImpl
{
	public List<string> AddressTo { get; set; }

	public List<string> AddressCc { get; set; }

	public List<string> AddressBcc { get; set; }

	public string Subject { get; set; }

	public string BodyText { get; set; }

	public string AddressFrom { get; set; }

	public Dictionary<string, Stream> AttachStream { get; set; }

	public List<string> AttachPath { get; set; }
}
