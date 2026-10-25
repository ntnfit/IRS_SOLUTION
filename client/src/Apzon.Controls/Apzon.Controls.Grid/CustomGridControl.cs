using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Controls.Grid;

[ToolboxItem(true)]
public class CustomGridControl : GridControl
{
	protected override void RegisterAvailableViewsCore(InfoCollection collection)
	{
		base.RegisterAvailableViewsCore(collection);
		collection.Add(new CustomGridInfoRegistrator());
	}

	protected override BaseView CreateDefaultView()
	{
		return CreateView("CustomGridView");
	}
}
