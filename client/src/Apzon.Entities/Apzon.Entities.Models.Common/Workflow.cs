using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Documents;

namespace Apzon.Entities.Models.Common;

public class Workflow
{
	public Enums.ContextActionActionTypeEx ActionType { get; set; }

	public ApzObjectTypes ObjectType { get; set; }

	public DocumentsImpl Document { get; set; }

	public DataSet DataSet { get; set; }

	public Enums.ContextActionMethod Method { get; set; }
}
