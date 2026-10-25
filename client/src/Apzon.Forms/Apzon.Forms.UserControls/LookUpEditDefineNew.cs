using System.ComponentModel;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors.Popup;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class LookUpEditDefineNew : LookUpEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemLookUpEditDefineNew Properties => base.Properties as RepositoryItemLookUpEditDefineNew;

	public override string EditorTypeName => "LookUpEditDefineNew";

	static LookUpEditDefineNew()
	{
		RepositoryItemLookUpEditDefineNew.RegisterLookUpEditDefineNew();
	}

	protected override PopupBaseForm CreatePopupForm()
	{
		return new LookUpEditDefineNewPopupForm(this);
	}
}
