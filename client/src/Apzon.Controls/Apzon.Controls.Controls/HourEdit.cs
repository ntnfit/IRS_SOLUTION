using System;
using System.ComponentModel;
using Apzon.Commons;
using Apzon.Controls.Editor;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public class HourEdit : TextEditEx
{
	public TimeSpan TimeSpanValue => Function.IntSAPToTimeSpan(EditValue);

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemHourEdit Properties => base.Properties as RepositoryItemHourEdit;

	public override string EditorTypeName => "HourEdit";

	static HourEdit()
	{
		RepositoryItemHourEdit.RegisterHourEdit();
	}

	public HourEdit()
	{
		Width = 41;
		Validating += HourEdit_Validating;
	}

	private void HourEdit_Validating(object sender, CancelEventArgs e)
	{
		Properties.RepositoryItemHourEdit_Validating(sender, e);
	}
}
