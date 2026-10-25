using System.Drawing;
using DevExpress.XtraBars.Alerter;

namespace Apzon.Controls.Controls;

public class AlertViewInfoEx : AlertViewInfo
{
	public AlertViewInfoEx(AlertFormCore form)
		: base(form)
	{
	}

	protected override Rectangle GetButtonElementRectangle(Size size, int indent)
	{
		if (size == Size.Empty)
		{
			return Rectangle.Empty;
		}
		return new Rectangle(Owner.ClientRectangle.Right - buttonsOffset.X - indent - size.Width, Owner.ClientRectangle.Bottom - buttonsOffset.Y - size.Height, size.Width, size.Height);
	}
}
