namespace Apzon.Entities.Models.Common;

public class UDFsImpl
{
	public int ID_FieldTable { get; set; }

	public string TableName { get; set; }

	public string FieldName { get; set; }

	public string DataType { get; set; }

	public string Lenght { get; set; }

	public string DefaultValue { get; set; }

	public string Description { get; set; }

	public bool StatusDisplay { get; set; }

	public string TypeID { get; set; }

	public string TypeControl { get; set; }

	public int? ID_Structure { get; set; }

	public string LinkTable { get; set; }
}
