using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Grid;

public class CustomGridRegularRowNavigator : GridRegularRowNavigator
{
	protected new CustomGridView View => base.View as CustomGridView;

	public CustomGridRegularRowNavigator(GridHandler handler)
		: base(handler)
	{
	}

	public override bool OnMouseDown(GridHitInfo hitInfo, DXMouseEventArgs e)
	{
		bool result = base.OnMouseDown(hitInfo, e);
		if (hitInfo.HitTest == GridHitTest.RowPreview)
		{
			View.FocusedColumn = null;
			View.ActivatePreviewEditor();
			View.IsRowPreviewSelected = true;
			return result;
		}
		View.IsRowPreviewSelected = false;
		return result;
	}
}
