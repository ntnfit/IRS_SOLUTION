using System.Xml.Serialization;

public class ResourceId
{
	[XmlAttribute("Type")]
	public string Type { get; set; }

	[XmlAttribute("Value")]
	public string Value { get; set; }
}
