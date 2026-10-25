using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Commons;

internal class ScrollHelper
{
	private XtraScrollableControl scrollableControl;

	public ScrollHelper(XtraScrollableControl scrollableControl)
	{
		this.scrollableControl = scrollableControl;
	}

	public void EnableScrollOnMouseWheel()
	{
		scrollableControl.VisibleChanged += OnVisibleChanged;
	}

	private void OnVisibleChanged(object sender, EventArgs e)
	{
		scrollableControl.Select();
		UnsubscribeFromMouseWheel(scrollableControl.Controls);
		SubscribeToMouseWheel(scrollableControl.Controls);
	}

	private void SubscribeToMouseWheel(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			if (control is BaseControl || control is PanelControl)
			{
				control.MouseWheel += OnMouseWheel;
				control.MouseDown += ctrl_MouseDown;
			}
			SubscribeToMouseWheel(control.Controls);
		}
	}

	private void ctrl_MouseDown(object sender, MouseEventArgs e)
	{
		((Control)sender).Focus();
	}

	private void UnsubscribeFromMouseWheel(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			if (control is BaseControl || control is PanelControl)
			{
				control.MouseWheel -= OnMouseWheel;
				control.MouseDown -= ctrl_MouseDown;
			}
			UnsubscribeFromMouseWheel(control.Controls);
		}
	}

	private void OnMouseWheel(object sender, MouseEventArgs e)
	{
		DXMouseEventArgs.GetMouseArgs(e).Handled = true;
		int value = scrollableControl.VerticalScroll.Value;
		int largeChange = scrollableControl.VerticalScroll.LargeChange;
		if (e.Delta < 0)
		{
			scrollableControl.VerticalScroll.Value += scrollableControl.VerticalScroll.LargeChange;
		}
		else if (value < largeChange)
		{
			scrollableControl.VerticalScroll.Value = 0;
		}
		else
		{
			scrollableControl.VerticalScroll.Value -= largeChange;
		}
	}

	public void DisableScrollOnMouseWheel()
	{
		scrollableControl.VisibleChanged -= OnVisibleChanged;
		UnsubscribeFromMouseWheel(scrollableControl.Controls);
		scrollableControl = null;
	}
}
