using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;

namespace Apzon.Controls.Grid;

public class AdvBandViewInfoRegistratorEx : GridInfoRegistrator
{
	public override string ViewName => "AdvBandedGridViewEx";

	public override BaseView CreateView(GridControl grid)
	{
		return new AdvBandedGridViewEx(grid as GridControlEx);
	}

	public override BaseViewInfo CreateViewInfo(BaseView view)
	{
		return new AdvBandViewInfoEx(view as AdvBandedGridViewEx);
	}

	public override BaseViewHandler CreateHandler(BaseView view)
	{
		return new AdvBandHandlerEx(view as AdvBandedGridViewEx);
	}
}
