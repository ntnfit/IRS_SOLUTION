using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;

namespace Apzon.Controls.Grid;

public class BandViewInfoRegistratorEx : GridInfoRegistrator
{
	public override string ViewName => "BandedGridViewEx";

	public override BaseView CreateView(GridControl grid)
	{
		return new BandedGridViewEx(grid as GridControlEx);
	}

	public override BaseViewInfo CreateViewInfo(BaseView view)
	{
		return new BandViewInfoEx(view as BandedGridViewEx);
	}

	public override BaseViewHandler CreateHandler(BaseView view)
	{
		return new BandHandlerEx(view as BandedGridViewEx);
	}
}
