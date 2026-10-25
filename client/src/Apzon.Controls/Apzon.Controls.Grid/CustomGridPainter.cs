using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Grid;

public class CustomGridPainter : GridPainter
{
	public new virtual CustomGridView View => (CustomGridView)base.View;

	public CustomGridPainter(GridView view)
		: base(view)
	{
	}

	protected override void DrawRowPreview(GridViewDrawArgs e, GridDataRowInfo ri)
	{
		RepositoryItem previewRowEdit = ((CustomGridView)e.ViewInfo.View).PreviewRowEdit;
		if (previewRowEdit == null)
		{
			base.DrawRowPreview(e, ri);
		}
		else
		{
			DrawRowPreviewEditor(e, ri, previewRowEdit);
		}
	}

	private void DrawRowPreviewEditor(GridViewDrawArgs e, GridDataRowInfo ri, RepositoryItem item)
	{
		GridCellInfo gridCellInfo = new GridCellInfo(null, ri, ri.PreviewBounds);
		gridCellInfo.Editor = item;
		DrawCellEdit(e, ((CustomGridViewInfo)e.ViewInfo).GetRowPreviewViewInfo(e, ri), gridCellInfo, ri.AppearancePreview, isSelectedCell: false);
	}
}
