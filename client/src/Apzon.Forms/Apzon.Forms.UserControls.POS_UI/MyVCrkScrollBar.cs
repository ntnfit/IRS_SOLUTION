using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Scrolling;

namespace Apzon.Forms.UserControls.POS_UI;

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
