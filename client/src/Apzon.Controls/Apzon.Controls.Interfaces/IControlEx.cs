using System.Drawing;
using System.Windows.Forms;

namespace Apzon.Controls.Interfaces;

public interface IControlEx
{
	string Text { get; set; }

	bool Visible { get; set; }

	bool ReadOnly { get; set; }

	string FieldName { get; set; }

	string Name { get; set; }

	bool SettingReadonly { get; set; }

	bool SettingInVisible { get; set; }

	object EditValue { get; set; }

	bool Enabled { get; set; }

	Color BackColor { get; set; }

	object Tag { get; set; }

	IEditControlEx EditControl { get; set; }

	bool Focus();

	System.Windows.Forms.Form FindForm();
}
