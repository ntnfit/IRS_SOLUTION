using System.Windows.Forms;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.UserControls.POS_UI;

public class ScrollInfoEx : ScrollInfo
{
	public override int VScrollSize => SystemInformation.VerticalScrollBarWidth * 2;

	protected override int HScrollHeight => base.HScrollHeight * 2;

	public ScrollInfoEx(BaseView view)
		: base(view)
	{
	}

	protected override VCrkScrollBar CreateVScroll()
	{
		return new MyVCrkScrollBar(this);
	}

	protected override HCrkScrollBar CreateHScroll()
	{
		return new MyHCrkScrollBar(this);
	}
}
