using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Grid;

public class GridViewExPainter : GridPainter
{
	private bool _IsCustomPainting;

	public bool IsCustomPainting
	{
		get
		{
			return _IsCustomPainting;
		}
		set
		{
			_IsCustomPainting = value;
		}
	}

	public GridViewExPainter(GridViewEx view)
		: base(view)
	{
	}

	public Rectangle DrawMergedCell(GridViewExMergeCell cell, PaintEventArgs e)
	{
		try
		{
			GridViewInfo gridViewInfo = View.GetViewInfo() as GridViewInfo;
			GridCellInfo gridCellInfo = gridViewInfo.GetGridCellInfo(cell.RowHandle, cell.Column1);
			if (gridCellInfo == null)
			{
				return default;
			}
			Rectangle bounds = gridCellInfo.Bounds;
			Rectangle rectangle = bounds;
			for (int i = cell.Column1.VisibleIndex + 1; i <= cell.Column2.VisibleIndex; i++)
			{
				GridCellInfo gridCellInfo2 = gridViewInfo.GetGridCellInfo(cell.RowHandle, View.Columns[i]);
				if (gridCellInfo2 != null)
				{
					rectangle = Rectangle.Union(gridCellInfo.Bounds, gridCellInfo2.Bounds);
				}
			}
			gridCellInfo.Bounds = rectangle;
			gridCellInfo.CellValueRect = rectangle;
			if (gridCellInfo.ViewInfo != null)
			{
				Rectangle bounds2 = gridCellInfo.ViewInfo.Bounds;
				bounds2.Width = rectangle.Width;
				bounds2.Height = rectangle.Height;
				gridCellInfo.ViewInfo.Bounds = bounds2;
				gridCellInfo.ViewInfo.CalcViewInfo(e.Graphics);
				IsCustomPainting = true;
				GraphicsCache graphicsCache = new GraphicsCache(e.Graphics);
				gridCellInfo.Appearance.FillRectangle(graphicsCache, gridCellInfo.Bounds);
				DrawRowCell(new GridViewDrawArgs(graphicsCache, gridViewInfo, gridViewInfo.ViewRects.Bounds), gridCellInfo);
				IsCustomPainting = false;
			}
			return bounds;
		}
		catch (Exception)
		{
		}
		return default;
	}
}
