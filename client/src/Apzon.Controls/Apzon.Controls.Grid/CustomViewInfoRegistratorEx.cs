using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;

namespace Apzon.Controls.Grid;

public class CustomViewInfoRegistratorEx : GridInfoRegistrator
{
	public override string ViewName => "CustomGridView";

	public override BaseView CreateView(GridControl grid)
	{
		return new CustomGridView(grid as GridControlEx);
	}

	public override BaseViewInfo CreateViewInfo(BaseView view)
	{
		return new CustomViewInfoEx(view as CustomGridView);
	}

	public override BaseViewHandler CreateHandler(BaseView view)
	{
		return new CustomHandlerEx(view as CustomGridView);
	}
}
