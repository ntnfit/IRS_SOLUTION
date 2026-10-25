using System;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Grid;

public class CustomGridViewInfo : GridViewInfo
{
	private BaseEditViewInfo fRowPreviewViewInfo;

	public new virtual CustomGridView View => base.View as CustomGridView;

	public CustomGridViewInfo(GridView gridView)
		: base(gridView)
	{
		UpdateRowPreviewEdit(View.PreviewRowEdit);
	}

	public virtual void UpdateRowPreviewEdit(RepositoryItem item)
	{
		if (item != null)
		{
			fRowPreviewViewInfo = CreateRowPreviewViewInfo(item);
		}
		else
		{
			fRowPreviewViewInfo = null;
		}
	}

	protected virtual BaseEditViewInfo CreateRowPreviewViewInfo(RepositoryItem item)
	{
		BaseEditViewInfo baseEditViewInfo = item.CreateViewInfo();
		UpdateEditViewInfo(baseEditViewInfo);
		Graphics g = GInfo.AddGraphics(null);
		try
		{
			baseEditViewInfo.CalcViewInfo(g);
			return baseEditViewInfo;
		}
		finally
		{
			GInfo.ReleaseGraphics();
		}
	}

	public virtual BaseEditViewInfo GetRowPreviewViewInfo(GridViewDrawArgs e, GridDataRowInfo ri)
	{
		fRowPreviewViewInfo.Bounds = GetRowPreviewEditBounds(ri);
		fRowPreviewViewInfo.EditValue = View.GetRowPreviewValue(ri.RowHandle);
		fRowPreviewViewInfo.Focused = true;
		fRowPreviewViewInfo.CalcViewInfo(e.Graphics);
		return fRowPreviewViewInfo;
	}

	public virtual Rectangle GetRowPreviewEditBounds(GridDataRowInfo ri)
	{
		Rectangle result = new Rectangle(new Point(0, 0), ri.PreviewBounds.Size);
		result.Inflate(-2, -1);
		result.X += ri.PreviewBounds.X;
		result.Width -= ri.PreviewBounds.Width;
		return result;
	}

	public override int CalcRowPreviewHeight(int rowHandle)
	{
		RepositoryItem previewRowEdit = View.PreviewRowEdit;
		if (previewRowEdit == null)
		{
			return base.CalcRowPreviewHeight(rowHandle);
		}
		return CalcRowPreviewEditorHeight(rowHandle, previewRowEdit);
	}

	protected virtual int CalcRowPreviewEditorHeight(int rowHandle, RepositoryItem item)
	{
		if (!View.OptionsView.ShowPreview || View.IsGroupRow(rowHandle) || View.IsFilterRow(rowHandle))
		{
			return 0;
		}
		int num = ((View.OptionsView.ShowPreviewRowLines != DefaultBoolean.False) ? 1 : 0);
		int num2 = View.RaiseMeasurePreviewHeightAccessMetod(rowHandle);
		switch (num2)
		{
		default:
			return num + num2;
		case 0:
			return 0;
		case -1:
		{
			Graphics g = GInfo.AddGraphics(null);
			try
			{
				if (fRowPreviewViewInfo is IHeightAdaptable heightAdaptable)
				{
					fRowPreviewViewInfo.EditValue = View.GetRowPreviewValue(rowHandle);
					num = heightAdaptable.CalcHeight(GInfo.Cache, CalcRowPreviewWidth(rowHandle) - GetPreviewIndent() - 4);
				}
				num = Math.Max(fRowPreviewViewInfo.CalcMinHeight(g), num);
			}
			finally
			{
				GInfo.ReleaseGraphics();
			}
			return num + 2;
		}
		}
	}

	protected override void CalcRowHitInfo(Point pt, GridRowInfo ri, GridHitInfo hi)
	{
		base.CalcRowHitInfo(pt, ri, hi);
	}
}
