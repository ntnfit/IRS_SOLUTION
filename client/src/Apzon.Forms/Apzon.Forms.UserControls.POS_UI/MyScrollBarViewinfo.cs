using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Forms.UserControls.POS_UI;

public class MyScrollBarViewinfo : ScrollBarViewInfo
{
	public override int ButtonWidth => SystemInformation.VerticalScrollBarArrowHeight * 2;

	public MyScrollBarViewinfo(IScrollBar scrollBar)
		: base(scrollBar)
	{
	}
}
