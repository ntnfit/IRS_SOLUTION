using Apzon.Controls.Interfaces;

namespace Apzon.Controls.Form;

public class ControlChangedEventArgs
{
	public IControlEx FormControl { get; set; }

	public object Value { get; set; }

	public string FieldName { get; set; }

	public ControlChangedEventArgs(IControlEx ctrl, object value, string fieldName)
	{
		FormControl = ctrl;
		Value = value;
		FieldName = fieldName;
	}
}
