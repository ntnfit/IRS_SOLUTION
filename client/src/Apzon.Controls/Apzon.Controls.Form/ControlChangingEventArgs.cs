using Apzon.Controls.Interfaces;

namespace Apzon.Controls.Form;

public class ControlChangingEventArgs
{
	public IControlEx FormControl { get; set; }

	public object NewValue { get; set; }

	public object OldValue { get; set; }

	public string FieldName { get; set; }

	public bool Cancel { get; set; }

	public ControlChangingEventArgs(IControlEx ctrl, object newValue, object oldValue, string fieldName)
	{
		FormControl = ctrl;
		NewValue = newValue;
		OldValue = oldValue;
		FieldName = fieldName;
	}
}
