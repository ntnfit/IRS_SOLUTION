using SAPbobsCOM;

namespace Apzon.Entities.Models.Udos;

public class UserObjectEnhancedFormColumnImpl
{
	public int ChildNumber { get; set; }

	public string Code { get; set; }

	public string ColumnAlias { get; set; }

	public string ColumnDescription { get; set; }

	public BoYesNoEnum ColumnIsUsed { get; set; }

	public int ColumnNumber { get; set; }

	public int Count { get; set; }

	public BoYesNoEnum Editable { get; set; }
}
