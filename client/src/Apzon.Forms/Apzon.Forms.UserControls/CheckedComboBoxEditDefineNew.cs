using System.ComponentModel;
using Apzon.Controls.Editor;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class CheckedComboBoxEditDefineNew : CheckedComboBoxEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemCheckedComboBoxEditDefineNew Properties => base.Properties as RepositoryItemCheckedComboBoxEditDefineNew;

	public override string EditorTypeName => "CheckedComboBoxEditDefineNew";

	static CheckedComboBoxEditDefineNew()
	{
		RepositoryItemCheckedComboBoxEditDefineNew.RegisterCheckedComboBoxEditDefineNew();
	}
}
