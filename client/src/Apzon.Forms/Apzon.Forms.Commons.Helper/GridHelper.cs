using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Apzon.Forms.Commons.Helper;

internal static class GridHelper
{
	public static void SetFindControlImages(GridControl grid)
	{
		SetFindControlImages(grid, forceAlignment: true);
	}

	public static void SetFindControlImages(GridControl grid, bool forceAlignment)
	{
		FindControl findControl = null;
		foreach (Control control in grid.Controls)
		{
			findControl = control as FindControl;
			if (findControl != null)
			{
				break;
			}
		}
		if (findControl == null)
		{
			return;
		}
		EditorButton editorButton = findControl.FindEdit.Properties.Buttons[0];
		editorButton.Kind = ButtonPredefines.Search;
		editorButton = new EditorButton(ButtonPredefines.Close);
		editorButton.Visible = false;
		findControl.FindEdit.Properties.Buttons.Add(editorButton);
		findControl.FindEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
		{
			if (!e.Button.IsDefaultButton)
			{
				(s as ButtonEdit).Text = string.Empty;
			}
		};
		findControl.FindEdit.EditValueChanged += (object s, EventArgs e) =>
		{
			MRUEdit mRUEdit = s as MRUEdit;
			mRUEdit.Properties.BeginUpdate();
			try
			{
				mRUEdit.Properties.Buttons[0].Visible = string.IsNullOrEmpty(mRUEdit.Text);
				mRUEdit.Properties.Buttons[1].Visible = !string.IsNullOrEmpty(mRUEdit.Text);
			}
			finally
			{
				mRUEdit.Properties.EndUpdate();
			}
		};
		if (!forceAlignment)
		{
			return;
		}
		LayoutControl layoutControl = findControl.FindEdit.Parent as LayoutControl;
		layoutControl.BeginUpdate();
		try
		{
			for (int num = layoutControl.Root.Items.Count - 1; num >= 0; num--)
			{
				if (layoutControl.Root.Items[num] is LayoutControlItem layoutControlItem)
				{
					if (layoutControlItem.Visibility == LayoutVisibility.Never)
					{
						layoutControl.Root.Remove(layoutControlItem);
					}
					else
					{
						layoutControlItem.ControlAlignment = ContentAlignment.MiddleRight;
					}
				}
			}
		}
		finally
		{
			layoutControl.EndUpdate();
		}
	}

	internal static Color GetRowBackColor(GridControl gridControl)
	{
		return GridSkins.GetSkin(gridControl.LookAndFeel)[GridSkins.SkinGridRow].Color.BackColor;
	}

	internal static void HideCustomization(Control control)
	{
		if (control == null)
		{
			return;
		}
		foreach (Control control2 in control.Controls)
		{
			if (control2 is GridControl gridControl)
			{
				if (gridControl.MainView is GridView gridView)
				{
					gridView.HideCustomization();
				}
			}
			else
			{
				HideCustomization(control2);
			}
		}
	}
}
