using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraTab;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public class XtraTabControlEx : XtraTabControl
{
	private bool _isAddControl = true;

	public bool IsSearchControl { get; set; }

	protected override XtraTabPageCollection CreateTabCollection()
	{
		return new XtraTabPageCollectionEx(this);
	}

	protected override void InitLayout()
	{
		ReloadControlTitleSource();
		base.InitLayout();
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		ReloadControlTitleSource();
		base.OnLayout(levent);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		ReloadControlTitleSource();
		base.OnHandleCreated(e);
	}

	private void ReloadControlTitleSource()
	{
		if (!_isAddControl)
		{
			return;
		}
		if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
		{
			XtraTabPageCollection xtraTabPageCollection = TabPages;
			if (xtraTabPageCollection != null)
			{
				for (int i = 0; i < xtraTabPageCollection.Count; i++)
				{
					if (xtraTabPageCollection[i] is XtraTabPageEx xtraTabPageEx)
					{
						xtraTabPageEx.FieldName = (string.IsNullOrEmpty(xtraTabPageEx.FieldName) ? xtraTabPageEx.Name : xtraTabPageEx.FieldName);
					}
				}
			}
		}
		_isAddControl = FindForm() == null;
	}
}
