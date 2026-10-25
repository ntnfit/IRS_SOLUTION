using System.Data;

namespace Apzon.Entities.Models.MobileApp;

public class FieldFormModel
{
	public string Label { get; set; }

	public string OrderNo { get; set; }

	public string Name { get; set; }

	public string DefaultValue { get; set; }

	public string DataType { get; set; }

	public object Value { get; set; }

	public string Nullable { get; set; }

	public DataTable ListOptions { get; set; }
}
