using System.Collections.Generic;
using SAPbobsCOM;

namespace Apzon.Entities.Models.UserFields;

public class UserFieldsImpl
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

	public List<ValidValuesMDImpl> ValidValues { get; set; }

	public object Value { get; set; }
}
