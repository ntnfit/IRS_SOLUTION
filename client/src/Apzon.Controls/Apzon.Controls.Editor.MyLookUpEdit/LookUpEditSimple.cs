using System.ComponentModel;
using DevExpress.XtraEditors.Popup;

namespace Apzon.Controls.Editor.MyLookUpEdit;

[ToolboxItem(true)]
public class LookUpEditSimple : LookUpEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemLookUpEditSimple Properties => base.Properties as RepositoryItemLookUpEditSimple;

	public override string EditorTypeName => "LookUpEditSimple";

	static LookUpEditSimple()
	{
		RepositoryItemLookUpEditSimple.RegisterLookUpEditSimple();
	}

	protected override PopupBaseForm CreatePopupForm()
	{
		return new LookUpEditSimplePopupForm(this);
	}
}
