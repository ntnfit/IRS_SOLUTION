using System;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Interfaces.Grid;

public class RetailGrid : GridView
{
	public RetailGrid()
	{
		LookAndFeel.UseDefaultLookAndFeel = false;
		IndicatorWidth = 50;
		ColumnPanelRowHeight = 60;
		RowHeight = 70;
		OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.False;
		OptionsView.ShowGroupPanel = false;
		OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
		OptionsView.EnableAppearanceEvenRow = true;
		OptionsView.EnableAppearanceOddRow = true;
		OptionsFilter.AllowFilterEditor = false;
		Appearance.Row.Options.UseFont = true;
		Appearance.HeaderPanel.Options.UseFont = true;
		Font font = new Font(new FontFamily("Tahoma"), 15f, FontStyle.Regular, GraphicsUnit.Pixel);
		Appearance.HeaderPanel.Font = font;
		Appearance.Row.Font = font;
		Appearance.OddRow.Options.UseBackColor = true;
		Appearance.EvenRow.Options.UseBackColor = true;
		Appearance.OddRow.BackColor = Color.WhiteSmoke;
		CustomDrawRowIndicator += RetailGrid_CustomDrawRowIndicator;
	}

	protected override ScrollInfo CreateScrollInfo()
	{
		return new ScrollInfoEx(this);
	}

	private void RetailGrid_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}
}
