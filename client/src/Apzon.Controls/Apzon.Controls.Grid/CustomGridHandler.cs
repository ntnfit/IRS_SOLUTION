using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Handler;

namespace Apzon.Controls.Grid;

public class CustomGridHandler : GridHandler
{
	public CustomGridHandler(GridView gridView)
		: base(gridView)
	{
	}

	protected override GridRowNavigator CreateRowNavigator()
	{
		return new CustomGridRegularRowNavigator(this);
	}
}
