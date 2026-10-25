using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Scrolling;

namespace Apzon.Forms.UserControls.POS_UI;

public class MyHCrkScrollBar : HCrkScrollBar
{
	public MyHCrkScrollBar(ScrollInfo scrollInfo)
		: base(scrollInfo)
	{
	}

	protected override ScrollBarViewInfo CreateScrollBarViewInfo()
	{
		return new MyScrollBarViewinfo(this);
	}
}
