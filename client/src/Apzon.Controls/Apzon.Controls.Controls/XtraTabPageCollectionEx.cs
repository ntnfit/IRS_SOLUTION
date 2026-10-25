using DevExpress.XtraTab;

namespace Apzon.Controls.Controls;

public class XtraTabPageCollectionEx : XtraTabPageCollection
{
	public XtraTabPageCollectionEx(XtraTabControlEx tabControl)
		: base(tabControl)
	{
	}

	protected override XtraTabPage CreatePage()
	{
		return new XtraTabPageEx();
	}
}
