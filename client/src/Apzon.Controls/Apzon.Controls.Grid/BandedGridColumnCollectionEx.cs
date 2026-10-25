using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Apzon.Controls.Grid;

public class BandedGridColumnCollectionEx : BandedGridColumnCollection
{
	public BandedGridColumnCollectionEx(BandedGridViewEx view)
		: base(view)
	{
	}

	protected override GridColumn CreateColumn()
	{
		return new BandedGridColumnEx();
	}

	public BandedGridColumnEx AddEx()
	{
		BandedGridColumnEx bandedGridColumnEx = new BandedGridColumnEx();
		Add(bandedGridColumnEx);
		return bandedGridColumnEx;
	}
}
