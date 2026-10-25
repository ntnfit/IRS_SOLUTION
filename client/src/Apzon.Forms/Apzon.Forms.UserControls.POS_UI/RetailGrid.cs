using System;
using System.Drawing;
using Apzon.Commons.Constants;
using Apzon.Controls.Grid;
using DevExpress.Utils;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class RetailGrid : GridViewEx
{
	public RetailGrid()
	{
		LookAndFeel.UseDefaultLookAndFeel = false;
		IndicatorWidth = 70;
		ColumnPanelRowHeight = 25;
		RowHeight = 60;
		OptionsView.ColumnAutoWidth = true;
		OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.False;
		OptionsView.ShowGroupPanel = false;
		OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
		OptionsView.EnableAppearanceEvenRow = true;
		OptionsView.EnableAppearanceOddRow = true;
		OptionsFilter.AllowFilterEditor = false;
		OptionsView.ShowGroupedColumns = false;
		OptionsView.ShowVerticalLines = DefaultBoolean.False;
		Appearance.Row.Options.UseFont = true;
		Appearance.HeaderPanel.Options.UseFont = true;
		FontFamily family = new FontFamily("Tahoma");
		Font font = new Font(family, 18f, FontStyle.Regular, GraphicsUnit.Pixel);
		Font font2 = new Font(family, 15f, FontStyle.Regular, GraphicsUnit.Pixel);
		Appearance.HeaderPanel.Font = font;
		Appearance.Row.Font = font2;
		Appearance.OddRow.Options.UseBackColor = true;
		Appearance.EvenRow.Options.UseBackColor = true;
		Appearance.OddRow.BackColor = Color.WhiteSmoke;
		Appearance.FocusedRow.BackColor = StyleColor.FocusedRow;
		Appearance.Preview.Font = font2;
		CustomDrawRowIndicator += RetailGrid_CustomDrawRowIndicator;
	}

	protected override ScrollInfo CreateScrollInfo()
	{
		return new ScrollInfoEx(this);
	}

	private static void RetailGrid_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
