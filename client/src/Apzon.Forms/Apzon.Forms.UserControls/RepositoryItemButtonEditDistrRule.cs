using System.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Forms.UserControls;

public class RepositoryItemButtonEditDistrRule : RepositoryItemButtonEdit
{
	public const string CustomEditName = "ButtonEditDistrRule";

	public override string EditorTypeName => "ButtonEditDistrRule";

	static RepositoryItemButtonEditDistrRule()
	{
		RegisterButtonEditDistrRule();
	}

	public static void RegisterButtonEditDistrRule()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("ButtonEditDistrRule", typeof(ButtonEditDistrRule), typeof(RepositoryItemButtonEditDistrRule), typeof(ButtonEditViewInfo), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemButtonEditDistrRule;
		}
		finally
		{
			EndUpdate();
		}
	}
}
