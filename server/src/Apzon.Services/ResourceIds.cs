using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ResourceIds")]
public class ResourceIds
{
	[XmlElement("ResourceId")]
	public List<ResourceId> ListResourceId { get; set; }
}
