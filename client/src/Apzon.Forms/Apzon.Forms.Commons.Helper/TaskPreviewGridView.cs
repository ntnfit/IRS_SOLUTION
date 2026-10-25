using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Commons.Helper;

internal class TaskPreviewGridView : GridView
{
	private float rowFontSize = AppearanceObject.DefaultFont.Size;

	protected override bool IsAllowPixelScrollingPreview => true;

	public TaskPreviewGridView()
	{
		Appearance.Row.Font = new Font("Segoe UI", rowFontSize + 4f);
		OptionsSelection.EnableAppearanceHideSelection = false;
		OptionsView.AutoCalcPreviewLineCount = true;
		OptionsView.EnableAppearanceEvenRow = true;
		OptionsView.ShowGroupPanel = false;
		OptionsView.ShowHorizontalLines = DefaultBoolean.False;
		OptionsView.ShowIndicator = false;
		OptionsView.ShowPreview = true;
		OptionsView.ShowVerticalLines = DefaultBoolean.False;
		PreviewIndent = 0;
		RowCellStyle += (object s, RowCellStyleEventArgs e) =>
		{
			e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, rowFontSize, e.Appearance.Font.Style);
			if (e.RowHandle == FocusedRowHandle && GridControl.Focused)
			{
				e.Appearance.BackColor = PaintAppearance.FocusedRow.BackColor;
			}
			else
			{
				SetEvenRowAppearance(e.Appearance, e.RowHandle);
			}
		};
		CustomDrawRowPreview += (object s, RowObjectCustomDrawEventArgs e) =>
		{
			if (e.RowHandle == FocusedRowHandle && GridControl.Focused)
			{
				e.Appearance.BackColor = PaintAppearance.FocusedRow.BackColor;
				e.Appearance.ForeColor = PaintAppearance.FocusedRow.ForeColor;
			}
			else
			{
				SetEvenRowAppearance(e.Appearance, e.RowHandle);
			}
		};
	}

	private void SetEvenRowAppearance(AppearanceObject appearance, int rowHandle)
	{
		if (rowHandle % 2 == 0)
		{
			appearance.BackColor = PaintAppearance.EvenRow.BackColor;
		}
		else
		{
			appearance.BackColor = PaintAppearance.Row.BackColor;
		}
	}

	public void SetViewFontSize(float rowFontSize, float previewFontSize)
	{
		if (previewFontSize > 0f)
		{
			Appearance.Preview.Font = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + previewFontSize);
		}
		if (rowFontSize > 0f)
		{
			this.rowFontSize += rowFontSize;
			Appearance.Row.Font = new Font("Segoe UI", this.rowFontSize + 4f);
		}
	}
}
