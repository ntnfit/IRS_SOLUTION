using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Controls.Controls;

public class RepositoryItemGridLookUpEditEx : RepositoryItemGridLookUpEdit
{
	private Color _bColor;

	public bool IsSearchControl { get; set; }

	public RepositoryItemGridLookUpEditEx(int width, int height, int minWidth, int minHeight)
	{
		LookAndFeel.Style = LookAndFeelStyle.Flat;
		LookAndFeel.UseDefaultLookAndFeel = false;
		_bColor = Color.FromArgb(254, 234, 163);
		View.Appearance.FocusedRow.BackColor = _bColor;
		View.Appearance.FocusedRow.ForeColor = Color.Black;
		PopupFormSize = new Size(width, height);
		PopupSizeable = false;
		PopupFormMinSize = new Size(minWidth, minHeight);
		View.OptionsView.ShowIndicator = false;
		View.OptionsView.ShowHorizontalLines = DefaultBoolean.False;
		View.OptionsView.ShowVerticalLines = DefaultBoolean.False;
		View.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
		View.OptionsView.ShowFooter = false;
	}
}
