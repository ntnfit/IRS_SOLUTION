using DevExpress.XtraGrid.Columns;

namespace Apzon.Controls.Grid;

public class GridViewExMergeCell
{
	public GridColumn Column1 { get; set; }

	public GridColumn Column2 { get; set; }

	public int RowHandle { get; set; }

	public GridViewExMergeCell(int rowHandle, GridColumn col1, GridColumn col2)
	{
		RowHandle = rowHandle;
		Column1 = col1;
		Column2 = col2;
	}
}
