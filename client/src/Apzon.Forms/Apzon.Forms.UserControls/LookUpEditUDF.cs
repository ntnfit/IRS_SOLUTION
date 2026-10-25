using System.ComponentModel;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors.Popup;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class LookUpEditUDF : LookUpEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemLookUpEditUDF Properties => base.Properties as RepositoryItemLookUpEditUDF;

	public override string EditorTypeName => "LookUpEditUDF";

	static LookUpEditUDF()
	{
		RepositoryItemLookUpEditUDF.RegisterLookUpEditUDF();
	}

	protected override PopupBaseForm CreatePopupForm()
	{
		return new LookUpEditUDFPopupForm(this);
	}
}
