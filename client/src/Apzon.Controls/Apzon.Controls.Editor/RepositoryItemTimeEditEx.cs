using DevExpress.Accessibility;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Controls.Editor;

public sealed class RepositoryItemTimeEditEx : RepositoryItemTextEdit
{
	public const string EditName = "TimeEditEx";

	public override string EditorTypeName => "TimeEditEx";

	static RepositoryItemTimeEditEx()
	{
		RegisterRepositoryItemTimeEditEx();
	}

	public static void RegisterRepositoryItemTimeEditEx()
	{
		EditorClassInfo itemInfo = new EditorClassInfo("TimeEditEx", typeof(TimeEditEx), typeof(RepositoryItemTimeEditEx), typeof(TextEditViewInfo), new TextEditPainter(), designTimeVisible: true, null, typeof(TextEditAccessible));
		EditorRegistrationInfo.Default.Editors.Add(itemInfo);
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemTimeEditEx;
		}
		finally
		{
			EndUpdate();
		}
	}
}
