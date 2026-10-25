using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;

namespace Apzon.Controls.Grid;

public class GridViewInfoRegistratorEx : GridInfoRegistrator
{
	public override string ViewName => "GridViewEx";

	public override BaseView CreateView(GridControl grid)
	{
		return new GridViewEx(grid as GridControlEx);
	}

	public override BaseViewInfo CreateViewInfo(BaseView view)
	{
		return new GridViewInfoEx(view as GridViewEx);
	}

	public override BaseViewHandler CreateHandler(BaseView view)
	{
		return new GridHandlerEx(view as GridViewEx);
	}
}
