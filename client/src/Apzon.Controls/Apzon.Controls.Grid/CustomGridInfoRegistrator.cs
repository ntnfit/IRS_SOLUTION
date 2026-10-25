using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Controls.Grid;

public class CustomGridInfoRegistrator : GridInfoRegistrator
{
	public override string ViewName => "CustomGridView";

	public override BaseViewPainter CreatePainter(BaseView view)
	{
		return new CustomGridPainter(view as GridView);
	}

	public override BaseViewInfo CreateViewInfo(BaseView view)
	{
		return new CustomGridViewInfo(view as GridView);
	}

	public override BaseViewHandler CreateHandler(BaseView view)
	{
		return new CustomGridHandler(view as GridView);
	}

	public override BaseView CreateView(GridControl grid)
	{
		CustomGridView customGridView = new CustomGridView();
		customGridView.SetGridControlAccessMetod(grid);
		return customGridView;
	}
}
