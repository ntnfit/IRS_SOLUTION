using System.Drawing;
using DevExpress.XtraBars.Alerter;

namespace Apzon.Controls.Controls;

public class AlertFormEx : AlertForm
{
	public AlertFormEx(Point location, IAlertControl control, AlertInfo info)
		: base(location, control, info)
	{
	}

	protected override AlertViewInfo CreateViewInfo()
	{
		return new AlertViewInfoEx(this);
	}
}
