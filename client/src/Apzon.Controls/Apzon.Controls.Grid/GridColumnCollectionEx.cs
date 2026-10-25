using DevExpress.XtraGrid.Columns;

namespace Apzon.Controls.Grid;

public class GridColumnCollectionEx : GridColumnCollection
{
	public GridColumnCollectionEx(GridViewEx view)
		: base(view)
	{
	}

	protected override GridColumn CreateColumn()
	{
		return new GridColumnEx();
	}

	public GridColumnEx AddEx()
	{
		GridColumnEx gridColumnEx = new GridColumnEx();
		Add(gridColumnEx);
		return gridColumnEx;
	}
}
