using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Scrolling;

namespace Apzon.Forms.Interfaces.Grid;

public class MyVCrkScrollBar : VCrkScrollBar
{
	public MyVCrkScrollBar(ScrollInfo scrollInfo)
		: base(scrollInfo)
	{
	}

	protected override ScrollBarViewInfo CreateScrollBarViewInfo()
	{
		return new MyScrollBarViewinfo(this);
	}
}
