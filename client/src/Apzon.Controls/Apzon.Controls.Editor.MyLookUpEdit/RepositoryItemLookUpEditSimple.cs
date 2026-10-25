using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using Apzon.Commons;
using Apzon.Commons.Enums;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Controls.Editor.MyLookUpEdit;

[UserRepositoryItem("RegisterLookUpEditSimple")]
public class RepositoryItemLookUpEditSimple : RepositoryItemLookUpEdit
{
	private const int MAX_SHOW_ITEM = 15;

	private int _NumOfItemShow = 10;

	public const string CustomEditName = "LookUpEditSimple";

	public bool IsSearchControl { get; set; }

	[DefaultValue(10)]
	[Description("Number of items show on popup")]
	public int NumOfItemShow
	{
		get
		{
			return _NumOfItemShow;
		}
		set
		{
			_NumOfItemShow = value;
		}
	}

	public override string EditorTypeName => "LookUpEditSimple";

	static RepositoryItemLookUpEditSimple()
	{
		RegisterLookUpEditSimple();
	}

	public RepositoryItemLookUpEditSimple()
	{
		Buttons.Clear();
		Buttons.Add(new EditorButton(ButtonPredefines.Combo));
		NullText = "";
		ShowHeader = false;
		ShowFooter = false;
		BestFitMode = BestFitMode.BestFit;
		Popup += luk_Popup;
		QueryPopUp += luk_QueryPopUp;
		AutoHeight = false;
	}

	private void luk_QueryPopUp(object sender, CancelEventArgs e)
	{
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			dynamic val = null;
			val = lookUpEdit.Properties.DataSource as DataTable;
			int num = 0;
			if (val != null)
			{
				num = val.Rows.Count;
			}
			else if ((dynamic)lookUpEdit.Properties.DataSource != null)
			{
				try
				{
					num = ((dynamic)lookUpEdit.Properties.DataSource).ToArray().Length;
				}
				catch (Exception)
				{
					num = 15;
				}
			}
			else
			{
				num = 15;
			}
			if ((num > 0 && num < NumOfItemShow + 1) || NumOfItemShow == 0)
			{
				if (num < 15)
				{
					lookUpEdit.Properties.DropDownRows = num;
				}
				else
				{
					lookUpEdit.Properties.DropDownRows = 15;
				}
			}
			else
			{
				lookUpEdit.Properties.DropDownRows = NumOfItemShow;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void luk_Popup(object sender, EventArgs e)
	{
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			PopupLookUpEditForm popupLookUpEditForm = ((IPopupControl)lookUpEdit).PopupWindow as PopupLookUpEditForm;
			_ = popupLookUpEditForm.Width;
			lookUpEdit.Properties.BestFit();
			if (lookUpEdit.Properties.Columns.Count > 0)
			{
				float num = 0f;
				if (popupLookUpEditForm.Filter.RowCount > 0)
				{
					for (int i = 0; i < lookUpEdit.Properties.Columns.Count; i++)
					{
						if (lookUpEdit.Properties.Columns[i].Visible)
						{
							num += (float)(lookUpEdit.Properties.Columns[i].Width + 18);
						}
					}
					int num2 = Function.ParseInt(Math.Round(num, 0).ToString());
					if (num2 > lookUpEdit.Width)
					{
						popupLookUpEditForm.Width = num2;
					}
					else
					{
						popupLookUpEditForm.Width = lookUpEdit.Width;
					}
					if (popupLookUpEditForm.Filter.RowCount > _NumOfItemShow)
					{
						popupLookUpEditForm.Width += 15;
					}
				}
			}
			else
			{
				popupLookUpEditForm.Width = lookUpEdit.Width;
			}
			if (popupLookUpEditForm.PointToScreen(Point.Empty).X < lookUpEdit.PointToScreen(Point.Empty).X)
			{
				popupLookUpEditForm.Location = new Point(lookUpEdit.PointToScreen(Point.Empty).X, popupLookUpEditForm.PointToScreen(Point.Empty).Y);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static void RegisterLookUpEditSimple()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("LookUpEditSimple", typeof(LookUpEditSimple), typeof(RepositoryItemLookUpEditSimple), typeof(LookUpEditSimpleViewInfo), new LookUpEditSimplePainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemLookUpEditSimple;
		}
		finally
		{
			EndUpdate();
		}
	}
}
