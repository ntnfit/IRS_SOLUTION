using System.Collections.Generic;
using Apzon.Commons.Enums;
using SAPbobsCOM;

namespace Apzon.Entities.Models.FormTemplate;

public class ControlField
{
	public string DefaultValue { get; set; }

	public string Description { get; set; }

	public long EditSize { get; set; }

	public long FieldID { get; set; }

	public string LinkedTable { get; set; }

	public string LinkedUDO { get; set; }

	public string LinkedObject { get; set; }

	public BoYesNoEnum Mandatory { get; set; }

	public string Name { get; set; }

	public long Size { get; set; }

	public BoFldSubTypes SubType { get; set; }

	public string TableName { get; set; }

	public BoFieldTypes Type { get; set; }

	public object Value { get; set; }

	public bool IsLeft { get; set; }

	public Enums.FormTemplateEditorType Editor { get; set; }

	public Dictionary<string, string> ValidValues { get; set; }
}
