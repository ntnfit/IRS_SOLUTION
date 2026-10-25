using System.ComponentModel;
using Apzon.Controls.Editor.MyLookUpEdit;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class LookUpEditCombo : LookUpEditSimple
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemLookUpEditCombo Properties => base.Properties as RepositoryItemLookUpEditCombo;

	public override string EditorTypeName => "LookUpEditCombo";

	static LookUpEditCombo()
	{
		RepositoryItemLookUpEditCombo.RegisterLookUpEditCombo();
	}

	protected override PopupBaseForm CreatePopupForm()
	{
		return new LookUpEditComboPopupForm(this);
	}

	protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo)
	{
		if (!IsKeyDown)
		{
			IsKeyDown = true;
			OldValue = EditValue;
		}
		base.OnPressButton(buttonInfo);
	}
}
