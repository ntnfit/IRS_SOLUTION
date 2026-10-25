using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Apzon.Controls.Grid;

public class GridViewCustomFilterTextEx : XtraFormEx
{
	public bool isResizeInBottomLeft;

	private const int cGrip = 16;

	private const int cCaption = 32;

	private CriteriaOperator criteriaFirst;

	private CriteriaOperator criteriaSecond;

	private static readonly string[] VietNamChar = new string[15]
	{
		"aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
		"ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ"
	};

	private bool isLoadOldFilter;

	private IContainer components;

	private TabPane tabPane1;

	private TabNavigationPage tab_Value;

	private LookUpEdit txt_OperatorType;

	private CheckedListBoxControl checkedList_FilterItem;

	private SearchControl txt_search;

	private TabNavigationPage tab_Filter;

	private LayoutControl layoutControl1;

	private LabelControl labelControl2;

	private LabelControl labelControl1;

	private RadioGroup radioGroup1;

	private LookUpEdit txt_operatorSecond;

	private SearchLookUpEdit txt_selectSecond;

	private GridView searchLookUpEdit3View;

	private SearchLookUpEdit txt_selectFirst;

	private GridView searchLookUpEdit2View;

	private SearchControl txt_searchSecond;

	private SearchControl txt_searchFirst;

	private LookUpEdit txt_operatorFirst;

	private SearchLookUpEdit txt_filterTabSelect;

	private GridView searchLookUpEdit1View;

	private SearchControl txt_filterTabSearch;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlTop;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem7;

	private LayoutControlGroup layout_Parent;

	private LayoutControlGroup layout_ChildCustom;

	private LayoutControlItem layoutControlItem5;

	private LayoutControlItem layout_ChildCustomSearch;

	private LayoutControlItem layout_ChildCustomSelect;

	private LayoutControlItem layoutControlItem10;

	private LayoutControlItem layout_ChildCustomSearchSecond;

	private LayoutControlItem layout_ChildCustomSelectSecond;

	private LayoutControlItem layoutControlItem11;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlGroup layout_Child;

	private LayoutControlItem layout_ChildSelect;

	private LayoutControlItem layout_ChildSearch;

	private LookUpEdit txt_OperatorTypeFilterTab;

	private SimpleButton btn_Close;

	private SimpleButton btn_Clear;

	private TabNavigationPage tab_Editor;

	private FilterControl filterControl;

	private SimpleButtonEx btn_ApplyFilter;

	private GridColumn column { get; set; }

	private Rectangle bounds { get; set; }

	private GridControl gridControl { get; set; }

	private Control ownerControl { get; set; }

	private GridViewEx gridView { get; set; }

	private DataTable dtSource { get; set; }

	public List<Tuple<object, string>> lstTupleDataCheckedBox { get; set; }

	public List<Tuple<object, string>> lstTupleDataCheckedBoxNonFilter { get; set; }

	public List<Tuple<object, string>> lstTupleDataItem { get; set; }

	private TypeOperatorFilter operatorType { get; set; }

	private TypeOperatorFilter operatorTypeFilterTab { get; set; }

	private TypeOperatorFilter operatorTypeFilterTabFirst { get; set; }

	private TypeOperatorFilter operatorTypeFilterTabSecond { get; set; }

	public GridViewCustomFilterTextEx()
	{
		InitializeComponent();
	}

	public GridViewCustomFilterTextEx(GridColumn column, Rectangle bounds, Control ownerControl, object creator, GridViewEx gridView)
	{
		try
		{
			InitializeComponent();
			DoubleBuffered = true;
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			this.column = column;
			this.bounds = bounds;
			this.ownerControl = ownerControl;
			DataTable dataTable = (ownerControl as GridControl).DataSource as DataTable;
			gridControl = ownerControl as GridControl;
			this.gridView = gridView;
			(from r in dataTable.AsEnumerable()
				select r.Field<object>(column.FieldName)).Distinct().ToList();
			CriteriaOperator activeFilterCriteria = gridView.ActiveFilterCriteria;
			gridView.ClearColumnsFilter();
			lstTupleDataCheckedBoxNonFilter = new List<Tuple<object, string>>();
			lstTupleDataItem = new List<Tuple<object, string>>();
			for (int num = 0; num < gridView.RowCount; num++)
			{
				if (gridView.GetRowCellValue(num, column).GetType().Equals(typeof(DBNull)))
				{
					Tuple<object, string> item = new Tuple<object, string>(gridView.GetRowCellValue(num, column), "Null");
					lstTupleDataCheckedBoxNonFilter.Add(item);
					lstTupleDataItem.Add(item);
				}
				else if (string.IsNullOrEmpty(Function.ToString(gridView.GetRowCellValue(num, column))))
				{
					Tuple<object, string> item2 = new Tuple<object, string>(gridView.GetRowCellValue(num, column), "");
					lstTupleDataCheckedBoxNonFilter.Add(item2);
					lstTupleDataItem.Add(item2);
				}
				else
				{
					Tuple<object, string> item3 = new Tuple<object, string>(gridView.GetRowCellValue(num, column), Function.ToString(gridView.GetRowCellDisplayText(num, column)));
					lstTupleDataCheckedBoxNonFilter.Add(item3);
					lstTupleDataItem.Add(item3);
				}
			}
			lstTupleDataCheckedBoxNonFilter = lstTupleDataCheckedBoxNonFilter.Distinct().ToList();
			lstTupleDataCheckedBoxNonFilter.Insert(0, new Tuple<object, string>("Select All", "Select All"));
			gridView.ActiveFilterCriteria = activeFilterCriteria;
			lstTupleDataCheckedBox = new List<Tuple<object, string>>();
			for (int num2 = 0; num2 < gridView.RowCount; num2++)
			{
				if (gridView.GetRowCellValue(num2, column).GetType().Equals(typeof(DBNull)))
				{
					Tuple<object, string> item4 = new Tuple<object, string>(gridView.GetRowCellValue(num2, column), "Null");
					lstTupleDataCheckedBox.Add(item4);
				}
				else if (string.IsNullOrEmpty(Function.ToString(gridView.GetRowCellValue(num2, column))))
				{
					Tuple<object, string> item5 = new Tuple<object, string>(gridView.GetRowCellValue(num2, column), "");
					lstTupleDataCheckedBox.Add(item5);
				}
				else
				{
					Tuple<object, string> item6 = new Tuple<object, string>(gridView.GetRowCellValue(num2, column), Function.ToString(gridView.GetRowCellDisplayText(num2, column)));
					lstTupleDataCheckedBox.Add(item6);
				}
			}
			lstTupleDataCheckedBox.Insert(0, new Tuple<object, string>("Select All", "Select All"));
			lstTupleDataCheckedBox = lstTupleDataCheckedBox.Distinct().ToList();
			operatorType = TypeOperatorFilter.Contain;
			this.gridView.ColumnFilterChanged += gridView_ColumnFilterChanged;
			filterControl.SourceControl = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(gridView.ActiveFilterString))
			{
				gridView.ClearColumnsFilter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Rectangle rect = new Rectangle(ClientSize.Width - 16, ClientSize.Height - 16, 16, 16);
		Pen pen = new Pen(Color.Black);
		if (!isResizeInBottomLeft)
		{
			e.Graphics.DrawLine(pen, ClientSize.Width, ClientSize.Height - 16, ClientSize.Width - 16, ClientSize.Height);
			e.Graphics.DrawLine(pen, ClientSize.Width, ClientSize.Height - 16 + 6, ClientSize.Width - 16 + 6, ClientSize.Height);
			e.Graphics.DrawLine(pen, ClientSize.Width, ClientSize.Height - 16 + 12, ClientSize.Width - 16 + 12, ClientSize.Height);
		}
		else
		{
			e.Graphics.DrawLine(pen, 0, ClientSize.Height - 16, 16, ClientSize.Height);
			e.Graphics.DrawLine(pen, 0, ClientSize.Height - 16 + 6, 10, ClientSize.Height);
			e.Graphics.DrawLine(pen, 0, ClientSize.Height - 16 + 12, 4, ClientSize.Height);
		}
		rect = new Rectangle(0, 0, ClientSize.Width, 32);
		e.Graphics.FillRectangle(Brushes.WhiteSmoke, rect);
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 132)
		{
			Point p = new Point(m.LParam.ToInt32());
			p = PointToClient(p);
			if (p.Y < 32)
			{
				m.Result = (IntPtr)2;
				return;
			}
			if (p.X >= ClientSize.Width - 16 && p.Y >= ClientSize.Height - 16 && !isResizeInBottomLeft)
			{
				m.Result = (IntPtr)17;
				return;
			}
			if (p.X <= 16 && p.Y >= ClientSize.Height - 16 && isResizeInBottomLeft)
			{
				m.Result = (IntPtr)16;
				return;
			}
		}
		base.WndProc(ref m);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		Close();
	}

	protected override void OnDeactivate(EventArgs e)
	{
		base.OnDeactivate(e);
		Close();
	}

	private void LoadComboboxOperator()
	{
		List<Tuple<int, string>> list = new List<Tuple<int, string>>();
		list.Add(Tuple.Create(1, "Equal"));
		list.Add(Tuple.Create(2, "Does Not Equal"));
		list.Add(Tuple.Create(3, "Begin With"));
		list.Add(Tuple.Create(4, "End With"));
		list.Add(Tuple.Create(5, "Contain"));
		list.Add(Tuple.Create(6, "Does Not Contain"));
		txt_OperatorType.Properties.DataSource = list;
		txt_OperatorType.Properties.Columns.Clear();
		txt_OperatorType.Properties.Columns.Add(new LookUpColumnInfo("Item1", 100));
		txt_OperatorType.Properties.Columns.Add(new LookUpColumnInfo("Item2", 100, "Operator Type"));
		txt_OperatorType.Properties.DisplayMember = "Item2";
		txt_OperatorType.Properties.ValueMember = "Item1";
		txt_OperatorType.Properties.Columns[0].Visible = false;
		txt_OperatorType.EditValue = 5;
	}

	private void LoadComboboxOperatorFilterTab()
	{
		List<Tuple<int, string>> list = new List<Tuple<int, string>>();
		list.Add(Tuple.Create(1, "Equal"));
		list.Add(Tuple.Create(2, "Does Not Equal"));
		list.Add(Tuple.Create(3, "Begin With"));
		list.Add(Tuple.Create(4, "End With"));
		list.Add(Tuple.Create(5, "Contain"));
		list.Add(Tuple.Create(6, "Does Not Contain"));
		list.Add(Tuple.Create(7, "Is Blank"));
		list.Add(Tuple.Create(8, "Is Not Blank"));
		list.Add(Tuple.Create(9, "Custom"));
		txt_OperatorTypeFilterTab.Properties.DataSource = list;
		txt_OperatorTypeFilterTab.Properties.Columns.Clear();
		txt_OperatorTypeFilterTab.Properties.Columns.Add(new LookUpColumnInfo("Item1", 100));
		txt_OperatorTypeFilterTab.Properties.Columns.Add(new LookUpColumnInfo("Item2", 100, "Operator Type"));
		txt_OperatorTypeFilterTab.Properties.DisplayMember = "Item2";
		txt_OperatorTypeFilterTab.Properties.ValueMember = "Item1";
		txt_OperatorTypeFilterTab.Properties.Columns[0].Visible = false;
		txt_OperatorTypeFilterTab.EditValue = 5;
		List<Tuple<int, string>> list2 = new List<Tuple<int, string>>();
		list2.Add(Tuple.Create(1, "Equal"));
		list2.Add(Tuple.Create(2, "Does Not Equal"));
		list2.Add(Tuple.Create(3, "Begin With"));
		list2.Add(Tuple.Create(4, "End With"));
		list2.Add(Tuple.Create(5, "Contain"));
		list2.Add(Tuple.Create(6, "Does Not Contain"));
		list2.Add(Tuple.Create(7, "Is Blank"));
		list2.Add(Tuple.Create(8, "Is Not Blank"));
		txt_operatorFirst.Properties.DataSource = list2;
		txt_operatorFirst.Properties.Columns.Clear();
		txt_operatorFirst.Properties.Columns.Add(new LookUpColumnInfo("Item1", 100));
		txt_operatorFirst.Properties.Columns.Add(new LookUpColumnInfo("Item2", 100, "Operator Type"));
		txt_operatorFirst.Properties.DisplayMember = "Item2";
		txt_operatorFirst.Properties.ValueMember = "Item1";
		txt_operatorFirst.Properties.Columns[0].Visible = false;
		txt_operatorFirst.EditValue = 5;
		txt_operatorSecond.Properties.DataSource = list2;
		txt_operatorSecond.Properties.Columns.Clear();
		txt_operatorSecond.Properties.Columns.Add(new LookUpColumnInfo("Item1", 100));
		txt_operatorSecond.Properties.Columns.Add(new LookUpColumnInfo("Item2", 100, "Operator Type"));
		txt_operatorSecond.Properties.DisplayMember = "Item2";
		txt_operatorSecond.Properties.ValueMember = "Item1";
		txt_operatorSecond.Properties.Columns[0].Visible = false;
		txt_operatorSecond.EditValue = 5;
	}

	public static string LocDau(string str)
	{
		try
		{
			for (int i = 1; i < VietNamChar.Length; i++)
			{
				for (int j = 0; j < VietNamChar[i].Length; j++)
				{
					str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
				}
			}
			return str;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	public static string RemoveUnicode(string s)
	{
		try
		{
			string text = s.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(text[i]) != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(text[i]);
				}
				stringBuilder.Replace("Đ", "D");
				stringBuilder.Replace("đ", "d");
			}
			return Function.ToString(stringBuilder).Normalize(NormalizationForm.FormC);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	public List<Tuple<object, string>> LoadDataCheckedListBox(string searchString, TypeOperatorFilter operatorType)
	{
		try
		{
			List<Tuple<object, string>> list = new List<Tuple<object, string>>();
			return operatorType switch
			{
				TypeOperatorFilter.Contain => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).Contains(LocDau(searchString.ToLower()))).ToList(), 
				TypeOperatorFilter.EndWith => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).EndsWith(LocDau(searchString.ToLower()))).ToList(), 
				TypeOperatorFilter.Equal => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).Equals(LocDau(searchString.ToLower()))).ToList(), 
				TypeOperatorFilter.IsBlank => lstTupleDataItem.Where((Tuple<object, string> s) => string.IsNullOrEmpty(Function.ToString(s.Item2))).ToList(), 
				TypeOperatorFilter.IsNotBlank => lstTupleDataItem.Where((Tuple<object, string> s) => !string.IsNullOrEmpty(Function.ToString(s.Item2))).ToList(), 
				TypeOperatorFilter.DoesNotContain => lstTupleDataItem.Where((Tuple<object, string> s) => !LocDau(Function.ToString(s.Item2).ToLower()).Contains(LocDau(searchString.ToLower()))).ToList(), 
				TypeOperatorFilter.DoesNotEqual => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).Equals(LocDau(searchString.ToLower()))).ToList(), 
				TypeOperatorFilter.BeginWith => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).StartsWith(LocDau(searchString.ToLower()))).ToList(), 
				_ => lstTupleDataItem.Where((Tuple<object, string> s) => LocDau(Function.ToString(s.Item2).ToLower()).Contains(LocDau(searchString.ToLower()))).ToList(), 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private void LoadCheckedListBox(string searchText, TypeOperatorFilter operatorType)
	{
		try
		{
			if (!string.IsNullOrEmpty(searchText) || operatorType == TypeOperatorFilter.IsBlank)
			{
				List<Tuple<object, string>> list = LoadDataCheckedListBox(searchText, operatorType);
				list.Insert(0, new Tuple<object, string>("Select All", "Select All"));
				checkedList_FilterItem.DataSource = list;
				isLoadOldFilter = true;
				LoadOldFilter();
				isLoadOldFilter = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private TypeOperatorFilter GetEnumOperatorType(int value)
	{
		try
		{
			return value switch
			{
				1 => TypeOperatorFilter.Equal, 
				2 => TypeOperatorFilter.DoesNotEqual, 
				3 => TypeOperatorFilter.BeginWith, 
				4 => TypeOperatorFilter.EndWith, 
				5 => TypeOperatorFilter.Contain, 
				6 => TypeOperatorFilter.DoesNotContain, 
				7 => TypeOperatorFilter.IsBlank, 
				8 => TypeOperatorFilter.IsNotBlank, 
				9 => TypeOperatorFilter.Custom, 
				_ => TypeOperatorFilter.None, 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return TypeOperatorFilter.None;
		}
	}

	private void SetVisibilityLayoutControlItem(LayoutVisibility value, params LayoutControlItem[] layoutItem)
	{
		try
		{
			for (int i = 0; i < layoutItem.Length; i++)
			{
				layoutItem[i].Visibility = value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetVisibilityLayoutControlGroup(LayoutVisibility value, params LayoutControlGroup[] layoutGroup)
	{
		try
		{
			for (int i = 0; i < layoutGroup.Length; i++)
			{
				layoutGroup[i].Visibility = value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private CriteriaOperator GetApplyFilter(TypeOperatorFilter operatorType, string value)
	{
		try
		{
			CriteriaOperator criteriaOperator = null;
			return operatorType switch
			{
				TypeOperatorFilter.BeginWith => new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(column.FieldName), new OperandValue(value)), 
				TypeOperatorFilter.EndWith => new FunctionOperator(FunctionOperatorType.EndsWith, new OperandProperty(column.FieldName), new OperandValue(value)), 
				TypeOperatorFilter.Contain => new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column.FieldName), new OperandValue(value)), 
				TypeOperatorFilter.DoesNotContain => new UnaryOperator(UnaryOperatorType.Not, new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column.FieldName), new OperandValue(value))), 
				TypeOperatorFilter.DoesNotEqual => (!string.IsNullOrEmpty(value)) ? ((CriteriaOperator)new BinaryOperator(new OperandProperty(column.FieldName), new OperandValue(value), BinaryOperatorType.NotEqual)) : ((CriteriaOperator)new UnaryOperator(UnaryOperatorType.Not, new FunctionOperator(FunctionOperatorType.IsNull, new OperandProperty(column.FieldName)))), 
				TypeOperatorFilter.Equal => (!string.IsNullOrEmpty(value)) ? ((CriteriaOperator)new BinaryOperator(new OperandProperty(column.FieldName), new OperandValue(value), BinaryOperatorType.Equal)) : ((CriteriaOperator)new FunctionOperator(FunctionOperatorType.IsNull, new OperandProperty(column.FieldName))), 
				TypeOperatorFilter.IsBlank => new FunctionOperator(FunctionOperatorType.IsNull, new OperandProperty(column.FieldName)), 
				TypeOperatorFilter.IsNotBlank => new UnaryOperator(UnaryOperatorType.Not, new FunctionOperator(FunctionOperatorType.IsNull, new OperandProperty(column.FieldName))), 
				_ => new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column.FieldName), new OperandValue("")), 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private void ProcessCretiaFirstAndCretiaSecond()
	{
		try
		{
			if ((object)criteriaFirst == null && (object)criteriaSecond == null)
			{
				return;
			}
			if ((object)criteriaSecond != null && (object)criteriaFirst == null)
			{
				gridView.ActiveFilter.Add(column, new ColumnFilterInfo(criteriaSecond));
			}
			if ((object)criteriaFirst != null && (object)criteriaSecond == null)
			{
				gridView.ActiveFilter.Add(column, new ColumnFilterInfo(criteriaFirst));
			}
			else if ((object)criteriaSecond != null && (object)criteriaFirst != null)
			{
				if (radioGroup1.SelectedIndex == 0)
				{
					gridView.ActiveFilterCriteria = CriteriaOperator.And(new CriteriaOperator[2] { criteriaFirst, criteriaSecond });
				}
				else
				{
					gridView.ActiveFilterCriteria = CriteriaOperator.Or(new CriteriaOperator[2] { criteriaFirst, criteriaSecond });
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_OperatorType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txt_OperatorType.EditValue != null)
			{
				operatorType = GetEnumOperatorType(int.Parse(Function.ToString(txt_OperatorType.EditValue)));
				LoadCheckedListBox(txt_search.Text, operatorType);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_search_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue != null)
			{
				if (string.IsNullOrEmpty(Function.ToString(e.NewValue)))
				{
					FormLoadLoadOldFilter();
				}
				else
				{
					LoadCheckedListBox(Function.ToString(e.NewValue), operatorType);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void checkedList_FilterItem_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		try
		{
			if (isLoadOldFilter)
			{
				return;
			}
			column.ClearFilter();
			CriteriaOperator filter = null;
			if (e.Index == 0 && e.State == CheckState.Checked && checkedList_FilterItem.GetItemCheckState(checkedList_FilterItem.FindString("Select All")) == CheckState.Checked)
			{
				checkedList_FilterItem.CheckAll();
				if (checkedList_FilterItem.CheckedItemsCount == lstTupleDataCheckedBoxNonFilter.Count)
				{
					column.ClearFilter();
				}
				return;
			}
			if (e.Index == 0 && e.State == CheckState.Unchecked && Function.ToString((checkedList_FilterItem.DataSource as List<Tuple<object, string>>)[0].Item1) == "Select All")
			{
				checkedList_FilterItem.UnCheckAll();
				column.ClearFilter();
				return;
			}
			int num = 0;
			bool flag = false;
			List<object> list = new List<object>();
			foreach (Tuple<object, string> item in (IEnumerable)checkedList_FilterItem.CheckedItems)
			{
				if (string.IsNullOrEmpty(gridView.ActiveFilterString))
				{
					gridView.ColumnFirstFilter = column;
				}
				if (num != 0 || !(Function.ToString(item.Item1) == "Select All"))
				{
					if (string.IsNullOrEmpty(Function.ToString(item.Item1)) && Function.ToString(item.Item2) == "Null")
					{
						flag = true;
						filter |= CriteriaOperator.Parse("[" + column.FieldName + "] Is Null");
					}
					else
					{
						list.Add(item.Item1);
					}
					num++;
				}
			}
			if (checkedList_FilterItem.CheckedItemsCount == 0)
			{
				column.ClearFilter();
			}
			else if ((list.Count <= 0) & flag)
			{
				column.ClearFilter();
				column.FilterInfo = new ColumnFilterInfo("[" + column.FieldName + "] Is Null");
			}
			else
			{
				InOperator inOperator = new InOperator(column.FieldName, list.ToArray());
				filter |= (CriteriaOperator)inOperator;
				column.FilterInfo = new ColumnFilterInfo(filter);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_OperatorTypeFilterTab_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txt_OperatorTypeFilterTab.EditValue == null)
			{
				return;
			}
			operatorTypeFilterTab = GetEnumOperatorType(int.Parse(Function.ToString(txt_OperatorTypeFilterTab.EditValue)));
			if (operatorTypeFilterTab == TypeOperatorFilter.BeginWith || operatorTypeFilterTab == TypeOperatorFilter.EndWith || operatorTypeFilterTab == TypeOperatorFilter.Contain || operatorTypeFilterTab == TypeOperatorFilter.DoesNotContain)
			{
				SetVisibilityLayoutControlGroup(LayoutVisibility.Always, layout_Child);
				SetVisibilityLayoutControlGroup(LayoutVisibility.Never, layout_ChildCustom);
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildSearch);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildSelect);
				if (txt_filterTabSearch.EditValue != null)
				{
					CriteriaOperator applyFilter = GetApplyFilter(operatorTypeFilterTab, Function.ToString(txt_filterTabSearch.EditValue));
					gridView.ActiveFilter.Add(column, new ColumnFilterInfo(applyFilter));
				}
			}
			else if (operatorTypeFilterTab == TypeOperatorFilter.IsBlank || operatorTypeFilterTab == TypeOperatorFilter.IsNotBlank)
			{
				SetVisibilityLayoutControlGroup(LayoutVisibility.Never, layout_Child, layout_ChildCustom);
				CriteriaOperator applyFilter2 = GetApplyFilter(operatorTypeFilterTab, "");
				gridView.ActiveFilter.Add(column, new ColumnFilterInfo(applyFilter2));
			}
			else if (operatorTypeFilterTab == TypeOperatorFilter.Custom)
			{
				SetVisibilityLayoutControlGroup(LayoutVisibility.Never, layout_Child);
				SetVisibilityLayoutControlGroup(LayoutVisibility.Always, layout_ChildCustom);
			}
			else if (operatorTypeFilterTab == TypeOperatorFilter.Equal || operatorTypeFilterTab == TypeOperatorFilter.DoesNotEqual)
			{
				SetVisibilityLayoutControlGroup(LayoutVisibility.Always, layout_Child);
				SetVisibilityLayoutControlGroup(LayoutVisibility.Never, layout_ChildCustom);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildSearch);
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildSelect);
				CriteriaOperator applyFilter3 = GetApplyFilter(operatorTypeFilterTab, Function.ToString(txt_filterTabSelect.EditValue));
				gridView.ActiveFilter.Add(column, new ColumnFilterInfo(applyFilter3));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_filterTabSelect_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CriteriaOperator applyFilter = GetApplyFilter(operatorTypeFilterTab, Function.ToString(txt_filterTabSelect.EditValue));
			gridView.ActiveFilter.Add(column, new ColumnFilterInfo(applyFilter));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_filterTabSearch_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue != null)
			{
				CriteriaOperator applyFilter = GetApplyFilter(operatorTypeFilterTab, Function.ToString(e.NewValue));
				gridView.ActiveFilter.Add(column, new ColumnFilterInfo(applyFilter));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_operatorFirst_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txt_operatorFirst.EditValue == null)
			{
				return;
			}
			operatorTypeFilterTabFirst = GetEnumOperatorType(int.Parse(Function.ToString(txt_operatorFirst.EditValue)));
			if (operatorTypeFilterTabFirst == TypeOperatorFilter.BeginWith || operatorTypeFilterTabFirst == TypeOperatorFilter.EndWith || operatorTypeFilterTabFirst == TypeOperatorFilter.Contain || operatorTypeFilterTabFirst == TypeOperatorFilter.DoesNotContain)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildCustomSearch);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSelect);
				if (txt_searchFirst.EditValue == null)
				{
					return;
				}
				criteriaFirst = GetApplyFilter(operatorTypeFilterTabFirst, Function.ToString(txt_searchFirst.EditValue));
			}
			else if (operatorTypeFilterTabFirst == TypeOperatorFilter.Equal || operatorTypeFilterTabFirst == TypeOperatorFilter.DoesNotEqual)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildCustomSelect);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSearch);
				criteriaFirst = GetApplyFilter(operatorTypeFilterTabFirst, Function.ToString(txt_selectFirst.EditValue));
			}
			else if (operatorTypeFilterTabFirst == TypeOperatorFilter.IsBlank || operatorTypeFilterTabFirst == TypeOperatorFilter.IsNotBlank)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSearch);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSelect);
				criteriaFirst = GetApplyFilter(operatorTypeFilterTabFirst, "");
			}
			ProcessCretiaFirstAndCretiaSecond();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_searchFirst_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue != null)
			{
				criteriaFirst = GetApplyFilter(operatorTypeFilterTabFirst, Function.ToString(e.NewValue));
				ProcessCretiaFirstAndCretiaSecond();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_operatorSecond_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txt_operatorSecond.EditValue == null)
			{
				return;
			}
			operatorTypeFilterTabSecond = GetEnumOperatorType(int.Parse(Function.ToString(txt_operatorSecond.EditValue)));
			if (operatorTypeFilterTabSecond == TypeOperatorFilter.BeginWith || operatorTypeFilterTabSecond == TypeOperatorFilter.EndWith || operatorTypeFilterTabSecond == TypeOperatorFilter.Contain || operatorTypeFilterTabSecond == TypeOperatorFilter.DoesNotContain)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildCustomSearchSecond);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSelectSecond);
				if (txt_searchSecond.EditValue == null)
				{
					return;
				}
				criteriaSecond = GetApplyFilter(operatorTypeFilterTabSecond, Function.ToString(txt_searchSecond.EditValue));
			}
			else if (operatorTypeFilterTabSecond == TypeOperatorFilter.Equal || operatorTypeFilterTabSecond == TypeOperatorFilter.DoesNotEqual)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Always, layout_ChildCustomSelectSecond);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSearchSecond);
				criteriaSecond = GetApplyFilter(operatorTypeFilterTabSecond, Function.ToString(txt_selectSecond.EditValue));
			}
			else if (operatorTypeFilterTabSecond == TypeOperatorFilter.IsBlank || operatorTypeFilterTabSecond == TypeOperatorFilter.IsNotBlank)
			{
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSearchSecond);
				SetVisibilityLayoutControlItem(LayoutVisibility.Never, layout_ChildCustomSelectSecond);
				criteriaSecond = GetApplyFilter(operatorTypeFilterTabSecond, "");
			}
			ProcessCretiaFirstAndCretiaSecond();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_searchSecond_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue != null)
			{
				criteriaSecond = GetApplyFilter(operatorTypeFilterTabSecond, Function.ToString(e.NewValue));
				ProcessCretiaFirstAndCretiaSecond();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void LoadOldFilter()
	{
		try
		{
			ColumnFilterInfo filterInfo = column.FilterInfo;
			if (string.IsNullOrEmpty(filterInfo.FilterString))
			{
				return;
			}
			if (filterInfo.FilterCriteria.GetType().Equals(typeof(InOperator)))
			{
				InOperator inOperator = filterInfo.FilterCriteria as InOperator;
				for (int i = 0; i < inOperator.Operands.Count; i++)
				{
					object value = (inOperator.Operands[i] as OperandValue).Value;
					int num = checkedList_FilterItem.FindString(Function.ToString(value));
					if (num > 0)
					{
						checkedList_FilterItem.SetItemChecked(num, value: true);
					}
				}
			}
			else if (filterInfo.FilterCriteria.GetType().Equals(typeof(UnaryOperator)) && (filterInfo.FilterCriteria as UnaryOperator).OperatorType == UnaryOperatorType.IsNull)
			{
				checkedList_FilterItem.SetItemChecked(checkedList_FilterItem.FindString("Null"), value: true);
			}
			else
			{
				if (!filterInfo.FilterCriteria.GetType().Equals(typeof(GroupOperator)))
				{
					return;
				}
				GroupOperator groupOperator = filterInfo.FilterCriteria as GroupOperator;
				if (groupOperator.Operands.Count != 2 || ((!groupOperator.Operands[0].GetType().Equals(typeof(UnaryOperator)) || !groupOperator.Operands[1].GetType().Equals(typeof(InOperator))) && (!groupOperator.Operands[1].GetType().Equals(typeof(UnaryOperator)) || !groupOperator.Operands[0].GetType().Equals(typeof(InOperator)))))
				{
					return;
				}
				InOperator inOperator2;
				if (groupOperator.Operands[0].GetType().Equals(typeof(UnaryOperator)))
				{
					_ = groupOperator.Operands[0];
					inOperator2 = groupOperator.Operands[1] as InOperator;
				}
				else
				{
					_ = groupOperator.Operands[1];
					inOperator2 = groupOperator.Operands[0] as InOperator;
				}
				for (int j = 0; j < inOperator2.Operands.Count; j++)
				{
					object value2 = (inOperator2.Operands[j] as OperandValue).Value;
					int num2 = checkedList_FilterItem.FindString(Function.ToString(value2));
					if (num2 > 0)
					{
						checkedList_FilterItem.SetItemChecked(num2, value: true);
					}
				}
				checkedList_FilterItem.SetItemChecked(checkedList_FilterItem.FindString("Null"), value: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FormLoadLoadOldFilter()
	{
		if (gridView.ActiveFilter.Count != 0)
		{
			if (gridView.ColumnFirstFilter.Name.Equals(column.Name))
			{
				checkedList_FilterItem.DataSource = lstTupleDataCheckedBoxNonFilter;
			}
			else
			{
				checkedList_FilterItem.DataSource = lstTupleDataCheckedBox;
			}
		}
		else
		{
			checkedList_FilterItem.DataSource = lstTupleDataCheckedBoxNonFilter;
		}
		isLoadOldFilter = true;
		LoadOldFilter();
		isLoadOldFilter = false;
	}

	private void GridViewCustomFilterTextEx_Load(object sender, EventArgs e)
	{
		try
		{
			FormLoadLoadOldFilter();
			LoadComboboxOperator();
			LoadComboboxOperatorFilterTab();
			txt_filterTabSelect.Properties.DataSource = lstTupleDataItem;
			txt_selectFirst.Properties.DataSource = lstTupleDataItem;
			txt_selectSecond.Properties.DataSource = lstTupleDataItem;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_selectSecond_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			criteriaSecond = GetApplyFilter(operatorTypeFilterTabSecond, Function.ToString(txt_selectSecond.EditValue));
			ProcessCretiaFirstAndCretiaSecond();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_selectFirst_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			criteriaFirst = GetApplyFilter(operatorTypeFilterTabFirst, Function.ToString(txt_selectFirst.EditValue));
			ProcessCretiaFirstAndCretiaSecond();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			ProcessCretiaFirstAndCretiaSecond();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		column.ClearFilter();
		gridView.ActiveFilterString = "";
		Refresh();
	}

	private void txt_search_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(txt_search.Text)))
			{
				checkedList_FilterItem.DataSource = lstTupleDataCheckedBox;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tabPane1_SelectedPageIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (tabPane1.SelectedPage == tab_Value || tabPane1.SelectedPage == tab_Filter)
			{
				btn_ApplyFilter.Visible = false;
			}
			else
			{
				btn_ApplyFilter.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ApplyFilter_Click(object sender, EventArgs e)
	{
		try
		{
			CriteriaOperator criteria = gridView.ActiveFilter.Criteria;
			criteria |= filterControl.FilterCriteria;
			gridView.ActiveFilter.Add(column, new ColumnFilterInfo(criteria));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Grid.GridViewCustomFilterTextEx));
		this.tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
		this.btn_ApplyFilter = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tab_Value = new DevExpress.XtraBars.Navigation.TabNavigationPage();
		this.txt_OperatorType = new DevExpress.XtraEditors.LookUpEdit();
		this.checkedList_FilterItem = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.txt_search = new DevExpress.XtraEditors.SearchControl();
		this.tab_Filter = new DevExpress.XtraBars.Navigation.TabNavigationPage();
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
		this.txt_operatorSecond = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_selectSecond = new DevExpress.XtraEditors.SearchLookUpEdit();
		this.searchLookUpEdit3View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.txt_selectFirst = new DevExpress.XtraEditors.SearchLookUpEdit();
		this.searchLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.txt_searchSecond = new DevExpress.XtraEditors.SearchControl();
		this.txt_searchFirst = new DevExpress.XtraEditors.SearchControl();
		this.txt_operatorFirst = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_filterTabSelect = new DevExpress.XtraEditors.SearchLookUpEdit();
		this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.txt_filterTabSearch = new DevExpress.XtraEditors.SearchControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_Parent = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layout_ChildCustom = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_ChildCustomSearch = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_ChildCustomSelect = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_ChildCustomSearchSecond = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_ChildCustomSelectSecond = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_Child = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layout_ChildSelect = new DevExpress.XtraLayout.LayoutControlItem();
		this.layout_ChildSearch = new DevExpress.XtraLayout.LayoutControlItem();
		this.txt_OperatorTypeFilterTab = new DevExpress.XtraEditors.LookUpEdit();
		this.tab_Editor = new DevExpress.XtraBars.Navigation.TabNavigationPage();
		this.filterControl = new DevExpress.XtraEditors.FilterControl();
		this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Clear = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.tabPane1).BeginInit();
		this.tabPane1.SuspendLayout();
		this.tab_Value.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_OperatorType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkedList_FilterItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_search.Properties).BeginInit();
		this.tab_Filter.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_operatorSecond.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_selectSecond.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit3View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_selectFirst.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit2View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_searchSecond.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_searchFirst.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_operatorFirst.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_filterTabSelect.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_filterTabSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Parent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSearch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSelect).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSearchSecond).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSelectSecond).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Child).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildSelect).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildSearch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OperatorTypeFilterTab.Properties).BeginInit();
		this.tab_Editor.SuspendLayout();
		base.SuspendLayout();
		this.tabPane1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.True;
		this.tabPane1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabPane1.Controls.Add(this.btn_ApplyFilter);
		this.tabPane1.Controls.Add(this.tab_Value);
		this.tabPane1.Controls.Add(this.tab_Filter);
		this.tabPane1.Controls.Add(this.tab_Editor);
		this.tabPane1.Location = new System.Drawing.Point(0, 0);
		this.tabPane1.Name = "tabPane1";
		this.tabPane1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[3] { this.tab_Value, this.tab_Filter, this.tab_Editor });
		this.tabPane1.RegularSize = new System.Drawing.Size(243, 323);
		this.tabPane1.SelectedPage = this.tab_Value;
		this.tabPane1.Size = new System.Drawing.Size(243, 323);
		this.tabPane1.TabIndex = 6;
		this.tabPane1.Text = "tabPane1";
		this.tabPane1.TransitionAnimationProperties.FrameCount = 800;
		this.tabPane1.TransitionAnimationProperties.FrameInterval = 6000;
		this.tabPane1.TransitionType = DevExpress.Utils.Animation.Transitions.PushFade;
		this.tabPane1.SelectedPageIndexChanged += new System.EventHandler(this.tabPane1_SelectedPageIndexChanged);
		this.btn_ApplyFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ApplyFilter.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_ApplyFilter.Appearance.Options.UseBackColor = true;
		this.btn_ApplyFilter.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_ApplyFilter.FieldName = null;
		this.btn_ApplyFilter.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ApplyFilter.ImageOptions.Image");
		this.btn_ApplyFilter.IsSearchControl = false;
		this.btn_ApplyFilter.Location = new System.Drawing.Point(209, 3);
		this.btn_ApplyFilter.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_ApplyFilter.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_ApplyFilter.Name = "btn_ApplyFilter";
		this.btn_ApplyFilter.Size = new System.Drawing.Size(22, 22);
		this.btn_ApplyFilter.TabIndex = 3;
		this.btn_ApplyFilter.Visible = false;
		this.btn_ApplyFilter.Click += new System.EventHandler(this.btn_ApplyFilter_Click);
		this.tab_Value.Caption = "Values";
		this.tab_Value.Controls.Add(this.txt_OperatorType);
		this.tab_Value.Controls.Add(this.checkedList_FilterItem);
		this.tab_Value.Controls.Add(this.txt_search);
		this.tab_Value.Name = "tab_Value";
		this.tab_Value.Size = new System.Drawing.Size(225, 278);
		this.txt_OperatorType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_OperatorType.Location = new System.Drawing.Point(3, 5);
		this.txt_OperatorType.Name = "txt_OperatorType";
		this.txt_OperatorType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_OperatorType.Properties.NullText = "";
		this.txt_OperatorType.Size = new System.Drawing.Size(219, 20);
		this.txt_OperatorType.TabIndex = 9;
		this.txt_OperatorType.EditValueChanged += new System.EventHandler(this.txt_OperatorType_EditValueChanged);
		this.checkedList_FilterItem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkedList_FilterItem.CheckOnClick = true;
		this.checkedList_FilterItem.Cursor = System.Windows.Forms.Cursors.Default;
		this.checkedList_FilterItem.DisplayMember = "Item2";
		this.checkedList_FilterItem.Location = new System.Drawing.Point(3, 57);
		this.checkedList_FilterItem.Name = "checkedList_FilterItem";
		this.checkedList_FilterItem.Size = new System.Drawing.Size(219, 218);
		this.checkedList_FilterItem.TabIndex = 6;
		this.checkedList_FilterItem.ValueMember = "Item1";
		this.checkedList_FilterItem.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedList_FilterItem_ItemCheck);
		this.txt_search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_search.Location = new System.Drawing.Point(3, 31);
		this.txt_search.Name = "txt_search";
		this.txt_search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.txt_search.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_search_Properties_ButtonClick);
		this.txt_search.Size = new System.Drawing.Size(219, 20);
		this.txt_search.TabIndex = 5;
		this.txt_search.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_search_EditValueChanging);
		this.tab_Filter.Caption = "Filter";
		this.tab_Filter.Controls.Add(this.layoutControl1);
		this.tab_Filter.Controls.Add(this.txt_OperatorTypeFilterTab);
		this.tab_Filter.Name = "tab_Filter";
		this.tab_Filter.Size = new System.Drawing.Size(225, 278);
		this.layoutControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.layoutControl1.Controls.Add(this.labelControl2);
		this.layoutControl1.Controls.Add(this.labelControl1);
		this.layoutControl1.Controls.Add(this.radioGroup1);
		this.layoutControl1.Controls.Add(this.txt_operatorSecond);
		this.layoutControl1.Controls.Add(this.txt_selectSecond);
		this.layoutControl1.Controls.Add(this.txt_selectFirst);
		this.layoutControl1.Controls.Add(this.txt_searchSecond);
		this.layoutControl1.Controls.Add(this.txt_searchFirst);
		this.layoutControl1.Controls.Add(this.txt_operatorFirst);
		this.layoutControl1.Controls.Add(this.txt_filterTabSelect);
		this.layoutControl1.Controls.Add(this.txt_filterTabSearch);
		this.layoutControl1.Controls.Add(this.barDockControlLeft);
		this.layoutControl1.Controls.Add(this.barDockControlRight);
		this.layoutControl1.Controls.Add(this.barDockControlBottom);
		this.layoutControl1.Controls.Add(this.barDockControlTop);
		this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem3, this.layoutControlItem4, this.layoutControlItem6, this.layoutControlItem7 });
		this.layoutControl1.Location = new System.Drawing.Point(3, 31);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(479, 98, 450, 400);
		this.layoutControl1.Root = this.layout_Parent;
		this.layoutControl1.Size = new System.Drawing.Size(218, 216);
		this.layoutControl1.TabIndex = 1;
		this.layoutControl1.Text = "layoutControl1";
		this.labelControl2.Location = new System.Drawing.Point(2, 149);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(41, 13);
		this.labelControl2.StyleController = this.layoutControl1;
		this.labelControl2.TabIndex = 16;
		this.labelControl2.Text = "Second  ";
		this.labelControl1.Location = new System.Drawing.Point(2, 50);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(21, 13);
		this.labelControl1.StyleController = this.layoutControl1;
		this.labelControl1.TabIndex = 15;
		this.labelControl1.Text = "First";
		this.radioGroup1.EditValue = "And";
		this.radioGroup1.Location = new System.Drawing.Point(47, 122);
		this.radioGroup1.Name = "radioGroup1";
		this.radioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroup1.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("And", "And"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Or", "Or")
		});
		this.radioGroup1.Properties.ItemsLayout = DevExpress.XtraEditors.RadioGroupItemsLayout.Column;
		this.radioGroup1.Size = new System.Drawing.Size(147, 23);
		this.radioGroup1.StyleController = this.layoutControl1;
		this.radioGroup1.TabIndex = 14;
		this.radioGroup1.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
		this.txt_operatorSecond.Location = new System.Drawing.Point(47, 149);
		this.txt_operatorSecond.Name = "txt_operatorSecond";
		this.txt_operatorSecond.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_operatorSecond.Properties.NullText = "";
		this.txt_operatorSecond.Size = new System.Drawing.Size(169, 20);
		this.txt_operatorSecond.StyleController = this.layoutControl1;
		this.txt_operatorSecond.TabIndex = 13;
		this.txt_operatorSecond.EditValueChanged += new System.EventHandler(this.txt_operatorSecond_EditValueChanged);
		this.txt_selectSecond.Location = new System.Drawing.Point(47, 197);
		this.txt_selectSecond.Name = "txt_selectSecond";
		this.txt_selectSecond.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_selectSecond.Properties.DisplayMember = "Item2";
		this.txt_selectSecond.Properties.NullText = "";
		this.txt_selectSecond.Properties.ValueMember = "Item1";
		this.txt_selectSecond.Properties.View = this.searchLookUpEdit3View;
		this.txt_selectSecond.Size = new System.Drawing.Size(169, 20);
		this.txt_selectSecond.StyleController = this.layoutControl1;
		this.txt_selectSecond.TabIndex = 12;
		this.txt_selectSecond.EditValueChanged += new System.EventHandler(this.txt_selectSecond_EditValueChanged);
		this.searchLookUpEdit3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.searchLookUpEdit3View.Name = "searchLookUpEdit3View";
		this.searchLookUpEdit3View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.searchLookUpEdit3View.OptionsView.ShowGroupPanel = false;
		this.txt_selectFirst.Location = new System.Drawing.Point(47, 98);
		this.txt_selectFirst.Name = "txt_selectFirst";
		this.txt_selectFirst.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_selectFirst.Properties.DisplayMember = "Item2";
		this.txt_selectFirst.Properties.NullText = "";
		this.txt_selectFirst.Properties.ValueMember = "Item1";
		this.txt_selectFirst.Properties.View = this.searchLookUpEdit2View;
		this.txt_selectFirst.Size = new System.Drawing.Size(169, 20);
		this.txt_selectFirst.StyleController = this.layoutControl1;
		this.txt_selectFirst.TabIndex = 11;
		this.txt_selectFirst.EditValueChanged += new System.EventHandler(this.txt_selectFirst_EditValueChanged);
		this.searchLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.searchLookUpEdit2View.Name = "searchLookUpEdit2View";
		this.searchLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.searchLookUpEdit2View.OptionsView.ShowGroupPanel = false;
		this.txt_searchSecond.Location = new System.Drawing.Point(47, 173);
		this.txt_searchSecond.Name = "txt_searchSecond";
		this.txt_searchSecond.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.txt_searchSecond.Size = new System.Drawing.Size(169, 20);
		this.txt_searchSecond.StyleController = this.layoutControl1;
		this.txt_searchSecond.TabIndex = 10;
		this.txt_searchSecond.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_searchSecond_EditValueChanging);
		this.txt_searchFirst.Location = new System.Drawing.Point(47, 74);
		this.txt_searchFirst.Name = "txt_searchFirst";
		this.txt_searchFirst.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.txt_searchFirst.Size = new System.Drawing.Size(169, 20);
		this.txt_searchFirst.StyleController = this.layoutControl1;
		this.txt_searchFirst.TabIndex = 9;
		this.txt_searchFirst.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_searchFirst_EditValueChanging);
		this.txt_operatorFirst.Location = new System.Drawing.Point(47, 50);
		this.txt_operatorFirst.Name = "txt_operatorFirst";
		this.txt_operatorFirst.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_operatorFirst.Properties.NullText = "";
		this.txt_operatorFirst.Size = new System.Drawing.Size(169, 20);
		this.txt_operatorFirst.StyleController = this.layoutControl1;
		this.txt_operatorFirst.TabIndex = 8;
		this.txt_operatorFirst.EditValueChanged += new System.EventHandler(this.txt_operatorFirst_EditValueChanged);
		this.txt_filterTabSelect.Location = new System.Drawing.Point(2, 2);
		this.txt_filterTabSelect.Name = "txt_filterTabSelect";
		this.txt_filterTabSelect.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_filterTabSelect.Properties.DisplayMember = "Item2";
		this.txt_filterTabSelect.Properties.NullText = "";
		this.txt_filterTabSelect.Properties.ValueMember = "Item1";
		this.txt_filterTabSelect.Properties.View = this.searchLookUpEdit1View;
		this.txt_filterTabSelect.Size = new System.Drawing.Size(214, 20);
		this.txt_filterTabSelect.StyleController = this.layoutControl1;
		this.txt_filterTabSelect.TabIndex = 5;
		this.txt_filterTabSelect.EditValueChanged += new System.EventHandler(this.txt_filterTabSelect_EditValueChanged);
		this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
		this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.txt_filterTabSearch.Location = new System.Drawing.Point(2, 26);
		this.txt_filterTabSearch.Name = "txt_filterTabSearch";
		this.txt_filterTabSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.txt_filterTabSearch.Size = new System.Drawing.Size(214, 20);
		this.txt_filterTabSearch.StyleController = this.layoutControl1;
		this.txt_filterTabSearch.TabIndex = 4;
		this.txt_filterTabSearch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_filterTabSearch_EditValueChanging);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Location = new System.Drawing.Point(98, 170);
		this.barDockControlLeft.Manager = null;
		this.barDockControlLeft.Size = new System.Drawing.Size(101, 20);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Location = new System.Drawing.Point(98, 194);
		this.barDockControlRight.Manager = null;
		this.barDockControlRight.Size = new System.Drawing.Size(101, 20);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Location = new System.Drawing.Point(2, 146);
		this.barDockControlBottom.Manager = null;
		this.barDockControlBottom.Size = new System.Drawing.Size(197, 23);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Location = new System.Drawing.Point(98, 122);
		this.barDockControlTop.Manager = null;
		this.barDockControlTop.Size = new System.Drawing.Size(101, 49);
		this.layoutControlItem3.Control = this.barDockControlTop;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(201, 24);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(93, 13);
		this.layoutControlItem4.Control = this.barDockControlBottom;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(201, 24);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem6.Control = this.barDockControlLeft;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(201, 24);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(93, 13);
		this.layoutControlItem7.Control = this.barDockControlRight;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 219);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(201, 24);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(93, 13);
		this.layout_Parent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
		this.layout_Parent.GroupBordersVisible = false;
		this.layout_Parent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layout_ChildCustom, this.layout_Child });
		this.layout_Parent.Location = new System.Drawing.Point(0, 0);
		this.layout_Parent.Name = "Root";
		this.layout_Parent.Size = new System.Drawing.Size(218, 219);
		this.layout_Parent.TextVisible = false;
		this.layout_ChildCustom.GroupBordersVisible = false;
		this.layout_ChildCustom.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.layoutControlItem5, this.layout_ChildCustomSearch, this.layout_ChildCustomSelect, this.layoutControlItem10, this.layout_ChildCustomSearchSecond, this.layout_ChildCustomSelectSecond, this.layoutControlItem11, this.emptySpaceItem3, this.emptySpaceItem1, this.emptySpaceItem2,
			this.layoutControlItem1, this.layoutControlItem2
		});
		this.layout_ChildCustom.Location = new System.Drawing.Point(0, 48);
		this.layout_ChildCustom.Name = "layout_ChildCustom";
		this.layout_ChildCustom.Size = new System.Drawing.Size(218, 171);
		this.layout_ChildCustom.TextVisible = false;
		this.layoutControlItem5.Control = this.txt_operatorFirst;
		this.layoutControlItem5.Location = new System.Drawing.Point(45, 0);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layoutControlItem5.Size = new System.Drawing.Size(173, 24);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layout_ChildCustomSearch.Control = this.txt_searchFirst;
		this.layout_ChildCustomSearch.Location = new System.Drawing.Point(45, 24);
		this.layout_ChildCustomSearch.Name = "layout_ChildCustomSearch";
		this.layout_ChildCustomSearch.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layout_ChildCustomSearch.OptionsTableLayoutItem.RowIndex = 1;
		this.layout_ChildCustomSearch.Size = new System.Drawing.Size(173, 24);
		this.layout_ChildCustomSearch.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildCustomSearch.TextVisible = false;
		this.layout_ChildCustomSelect.Control = this.txt_selectFirst;
		this.layout_ChildCustomSelect.Location = new System.Drawing.Point(45, 48);
		this.layout_ChildCustomSelect.Name = "layout_ChildCustomSelect";
		this.layout_ChildCustomSelect.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layout_ChildCustomSelect.OptionsTableLayoutItem.RowIndex = 2;
		this.layout_ChildCustomSelect.Size = new System.Drawing.Size(173, 24);
		this.layout_ChildCustomSelect.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildCustomSelect.TextVisible = false;
		this.layoutControlItem10.Control = this.txt_operatorSecond;
		this.layoutControlItem10.Location = new System.Drawing.Point(45, 99);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 4;
		this.layoutControlItem10.Size = new System.Drawing.Size(173, 24);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.layout_ChildCustomSearchSecond.Control = this.txt_searchSecond;
		this.layout_ChildCustomSearchSecond.Location = new System.Drawing.Point(45, 123);
		this.layout_ChildCustomSearchSecond.Name = "layout_ChildCustomSearchSecond";
		this.layout_ChildCustomSearchSecond.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layout_ChildCustomSearchSecond.OptionsTableLayoutItem.RowIndex = 5;
		this.layout_ChildCustomSearchSecond.Size = new System.Drawing.Size(173, 24);
		this.layout_ChildCustomSearchSecond.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildCustomSearchSecond.TextVisible = false;
		this.layout_ChildCustomSelectSecond.Control = this.txt_selectSecond;
		this.layout_ChildCustomSelectSecond.Location = new System.Drawing.Point(45, 147);
		this.layout_ChildCustomSelectSecond.Name = "layout_ChildCustomSelectSecond";
		this.layout_ChildCustomSelectSecond.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layout_ChildCustomSelectSecond.OptionsTableLayoutItem.RowIndex = 6;
		this.layout_ChildCustomSelectSecond.Size = new System.Drawing.Size(173, 24);
		this.layout_ChildCustomSelectSecond.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildCustomSelectSecond.TextVisible = false;
		this.layoutControlItem11.Control = this.radioGroup1;
		this.layoutControlItem11.Location = new System.Drawing.Point(45, 72);
		this.layoutControlItem11.MaxSize = new System.Drawing.Size(151, 27);
		this.layoutControlItem11.MinSize = new System.Drawing.Size(151, 27);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 3;
		this.layoutControlItem11.Size = new System.Drawing.Size(173, 27);
		this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 17);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(45, 55);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 116);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(45, 55);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 72);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(45, 27);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.labelControl1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(45, 17);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.labelControl2;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 99);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(45, 17);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layout_Child.GroupBordersVisible = false;
		this.layout_Child.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layout_ChildSelect, this.layout_ChildSearch });
		this.layout_Child.Location = new System.Drawing.Point(0, 0);
		this.layout_Child.Name = "layout_Child";
		this.layout_Child.Size = new System.Drawing.Size(218, 48);
		this.layout_Child.TextVisible = false;
		this.layout_ChildSelect.Control = this.txt_filterTabSelect;
		this.layout_ChildSelect.Location = new System.Drawing.Point(0, 0);
		this.layout_ChildSelect.Name = "layout_ChildSelect";
		this.layout_ChildSelect.Size = new System.Drawing.Size(218, 24);
		this.layout_ChildSelect.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildSelect.TextVisible = false;
		this.layout_ChildSearch.Control = this.txt_filterTabSearch;
		this.layout_ChildSearch.Location = new System.Drawing.Point(0, 24);
		this.layout_ChildSearch.Name = "layout_ChildSearch";
		this.layout_ChildSearch.Size = new System.Drawing.Size(218, 24);
		this.layout_ChildSearch.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ChildSearch.TextVisible = false;
		this.txt_OperatorTypeFilterTab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_OperatorTypeFilterTab.Location = new System.Drawing.Point(3, 5);
		this.txt_OperatorTypeFilterTab.Name = "txt_OperatorTypeFilterTab";
		this.txt_OperatorTypeFilterTab.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_OperatorTypeFilterTab.Properties.NullText = "";
		this.txt_OperatorTypeFilterTab.Size = new System.Drawing.Size(219, 20);
		this.txt_OperatorTypeFilterTab.TabIndex = 0;
		this.txt_OperatorTypeFilterTab.EditValueChanged += new System.EventHandler(this.txt_OperatorTypeFilterTab_EditValueChanged);
		this.tab_Editor.Caption = "Editor";
		this.tab_Editor.Controls.Add(this.filterControl);
		this.tab_Editor.Name = "tab_Editor";
		this.tab_Editor.Size = new System.Drawing.Size(225, 278);
		this.filterControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.filterControl.Cursor = System.Windows.Forms.Cursors.Arrow;
		this.filterControl.Location = new System.Drawing.Point(3, 3);
		this.filterControl.Name = "filterControl";
		this.filterControl.Size = new System.Drawing.Size(219, 272);
		this.filterControl.TabIndex = 0;
		this.filterControl.Text = "filterControl1";
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btn_Close.Location = new System.Drawing.Point(154, 342);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(65, 23);
		this.btn_Close.TabIndex = 10;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.btn_Clear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btn_Clear.Location = new System.Drawing.Point(23, 342);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.Size = new System.Drawing.Size(63, 23);
		this.btn_Clear.TabIndex = 9;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		base.Appearance.BorderColor = System.Drawing.Color.Tomato;
		base.Appearance.Options.UseBorderColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(244, 374);
		base.ControlBox = false;
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.btn_Clear);
		base.Controls.Add(this.tabPane1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Name = "GridViewCustomFilterTextEx";
		base.ShowInTaskbar = false;
		base.Load += new System.EventHandler(this.GridViewCustomFilterTextEx_Load);
		((System.ComponentModel.ISupportInitialize)this.tabPane1).EndInit();
		this.tabPane1.ResumeLayout(false);
		this.tab_Value.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_OperatorType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkedList_FilterItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_search.Properties).EndInit();
		this.tab_Filter.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_operatorSecond.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_selectSecond.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit3View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_selectFirst.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit2View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_searchSecond.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_searchFirst.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_operatorFirst.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_filterTabSelect.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_filterTabSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Parent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSearch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSelect).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSearchSecond).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildCustomSelectSecond).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Child).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildSelect).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ChildSearch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OperatorTypeFilterTab.Properties).EndInit();
		this.tab_Editor.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
