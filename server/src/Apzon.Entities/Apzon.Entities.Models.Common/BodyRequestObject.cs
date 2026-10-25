using Apzon.Commons;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class BodyRequestObject
{
	public ApzObjectTypes ObjectType { get; set; }

	public bool OutObjectKey { get; set; }

	public object Key { get; set; }

	public Enums.HttpAction Action { get; set; }

	public string BodyObject { get; set; }

	public string UdoCode { get; set; }

	public bool IsServiceAction { get; set; }
}
