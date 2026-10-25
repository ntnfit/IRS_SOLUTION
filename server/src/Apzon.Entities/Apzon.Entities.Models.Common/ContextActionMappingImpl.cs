using System.Data;
using Apzon.Commons;

namespace Apzon.Entities.Models.Common;

public class ContextActionMappingImpl
{
	public int AbsEntry { get; set; }

	public string FormId { get; set; }

	public string ItemId { get; set; }

	public string MappingType { get; set; }

	public string TargetItem { get; set; }

	public string TargetField { get; set; }

	public string SourceField { get; set; }

	public object DfltValue { get; set; }

	public string SumType { get; set; }

	public ContextActionMappingImpl()
	{
	}

	public ContextActionMappingImpl(DataRow t)
	{
		AbsEntry = Function.ParseInt(t["AbsEntry"]);
		FormId = Function.ToString(t["FormId"]);
		ItemId = Function.ToString(t["ItemId"]);
		DfltValue = t["DfltValue"];
		MappingType = Function.ToString(t["MappingType"]);
		SourceField = Function.ToString(t["SourceField"]);
		TargetField = Function.ToString(t["TargetField"]);
		TargetItem = Function.ToString(t["TargetItem"]);
		SumType = Function.ToString(t["SumType"]);
	}
}
