using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using FastColoredTextBoxNS;

namespace Apzon.Forms.Commons;

public class frm_UserDefinedProcedureCreate : XtraFormEx
{
	protected FastColoredTextBox txt_Formula;

	private AutocompleteMenu popupMenu;

	private readonly string[] snippets = new string[5] { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(var i, 0, ^, 1)\n{\n;\n}", "while(^)\n{\n;\n}", "do\n{\n^;\n}while();" };

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public DataTable _dtFormula = new DataTable();

	public Action<string> SaveContextActionScript;

	protected int[] ListEventInGridId = new int[3] { 1, 4, 18 };

	private List<ViewEditModel> _listGridView = new List<ViewEditModel>();

	private readonly TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);

	private readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

	private readonly TextStyle DarkVioletStyle = new TextStyle(Brushes.DarkViolet, null, FontStyle.Regular);

	private readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);

	private readonly TextStyle MediumSeaGreenStyle = new TextStyle(Brushes.MediumSeaGreen, null, FontStyle.Italic);

	private readonly TextStyle DeepSkyBlueStyle = new TextStyle(Brushes.DeepSkyBlue, null, FontStyle.Bold);

	private readonly TextStyle CadetBlueStyle = new TextStyle(Brushes.CadetBlue, null, FontStyle.Regular);

	private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.Yellow));

	private IContainer components;

	public GridControlEx grd_DataSet;

	public GridViewEx gv_DataSet;

	public GridControlEx grd_Field;

	public GridViewEx gv_Field;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	public LookUpEdit look_Events;

	private LabelLine labelLine1;

	public SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Verify;

	public TextEditEx txt_Description;

	private LabelLine labelLine2;

	private DockManager dockManager1;

	private PanelControl panelControl1;

	private DockPanel dockPanel1;

	private ControlContainer dockPanel1_Container;

	public GridControlEx grd_Type;

	public GridViewEx gv_Type;

	private GridControlEx grd_Keywords;

	private GridViewEx gv_Keywords;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx1;

	public CheckEditEx chk_Active;

	public LookUpEdit look_Branch;

	public LookUpEdit look_dep;

	private AutoHideContainer hideContainerRight;

	public CheckEditEx chk_Dept;

	public CheckEditEx chk_Branch;

	private LookUpEditEx txtItemEvent;

	private LabelLine lbItemEvent;

	private AutoHideContainer autoHideContainer1;

	private PanelControl panelControl2;

	private SimpleButton btnDelete;

	public string Formula { get; set; }

	protected XtraFormEx BaseForm { get; set; }

	public string Api { get; set; }

	public string SessionId { get; set; }

	protected string FormCode { get; set; }

	public string FormName { get; set; }

	protected int Event { get; set; }

	protected int ActionId { get; set; }

	protected string ActionName { get; set; }

	protected bool ContextAction { get; set; }

	public frm_UserDefinedProcedureCreate(XtraFormEx baseform, string formCode, int events = -1)
	{
		InitializeComponent();
		BaseForm = baseform;
		FormCode = formCode;
		Event = events;
		InitCodeEditor();
	}

	public frm_UserDefinedProcedureCreate(XtraFormEx baseform, string formCode, int actionId, string actionName, bool isFromContextAction, string script)
	{
		InitializeComponent();
		try
		{
			BaseForm = baseform;
			FormCode = formCode;
			Event = -1;
			ActionId = actionId;
			ActionName = actionName;
			ContextAction = isFromContextAction;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		InitCodeEditor();
		txt_Formula.Text = script;
	}

	private void InitCodeEditor()
	{
		FormCode = ((BaseForm != null) ? BaseForm.FormId : "");
		txt_Formula = new FastColoredTextBox
		{
			Font = new Font("Consolas", 10f),
			Paddings = new Padding(10),
			LeftPadding = 17,
			AutoCompleteBrackets = true,
			AutoIndentChars = false,
			CaretBlinking = false,
			DelayedEventsInterval = 500,
			Dock = DockStyle.Fill
		};
		txt_Formula.AddStyle(SameWordsStyle);
		txt_Formula.TextChanged += Txt_Formula_TextChanged;
		txt_Formula.SelectionChangedDelayed += Txt_Formula_SelectionChangedDelayed;
		txt_Formula.AutoIndentNeeded += fctb_AutoIndentNeeded;
		splitContainerControl1.Panel2.AddControl(txt_Formula);
		popupMenu = new AutocompleteMenu(txt_Formula);
		BuildAutocompleteMenu();
	}

	private void BuildAutocompleteMenu()
	{
		popupMenu.MinFragmentLength = 2;
		List<AutocompleteItem> list = new List<AutocompleteItem>();
		string[] array = snippets;
		foreach (string snippet in array)
		{
			list.Add(new SnippetAutocompleteItem2(snippet));
		}
		array = ConstantsApzCtd.HighlightFunctions;
		foreach (string text in array)
		{
			list.Add(new AutocompleteItem(text));
		}
		array = ConstantsApzCtd.HighlightKeywords;
		foreach (string text2 in array)
		{
			list.Add(new AutocompleteItem(text2));
		}
		array = ConstantsApzCtd.HighlightFunctions;
		foreach (string text3 in array)
		{
			list.Add(new MethodAutocompleteItem2(text3));
		}
		array = ConstantsApzCtd.HighlightKeywords;
		foreach (string text4 in array)
		{
			list.Add(new MethodAutocompleteItem2(text4));
		}
		popupMenu.Items.SetAutocompleteItems(list);
		popupMenu.SearchPattern = "[\\w\\.]";
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	private void frm_CreateFormula_Load(object sender, EventArgs e)
	{
		try
		{
			InitValues();
			LoadEvents();
			LoadDataSet();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void fctb_AutoIndentNeeded(object sender, AutoIndentEventArgs args)
	{
		if (!Regex.IsMatch(args.LineText, "^[^\"']*\\{.*\\}[^\"']*$"))
		{
			if (Regex.IsMatch(args.LineText, "^[^\"']*\\{"))
			{
				args.ShiftNextLines = args.TabLength;
			}
			else if (Regex.IsMatch(args.LineText, "}[^\"']*$"))
			{
				args.Shift = -args.TabLength;
				args.ShiftNextLines = -args.TabLength;
			}
			else if (Regex.IsMatch(args.LineText, "^\\s*\\w+\\s*:\\s*($|//)") && !Regex.IsMatch(args.LineText, "^\\s*default\\s*:"))
			{
				args.Shift = -args.TabLength;
			}
			else if (Regex.IsMatch(args.LineText, "^\\s*(case|default)\\b.*:\\s*($|//)"))
			{
				args.Shift = -args.TabLength / 2;
			}
			else if (Regex.IsMatch(args.PrevLineText, "^\\s*(if|for|loop|while|[\\}\\s]*else)\\b[^{]*$") && !Regex.IsMatch(args.PrevLineText, "(;\\s*$)|(;\\s*//)"))
			{
				args.Shift = args.TabLength;
			}
		}
	}

	private void Txt_Formula_SelectionChangedDelayed(object sender, EventArgs e)
	{
		FastColoredTextBox fastColoredTextBox = sender as FastColoredTextBox;
		fastColoredTextBox.Range.ClearStyle(SameWordsStyle);
		if (!fastColoredTextBox.Selection.IsEmpty)
		{
			return;
		}
		string text = fastColoredTextBox.Selection.GetFragment("\\w").Text;
		if (text.Length == 0)
		{
			return;
		}
		Range[] array = fastColoredTextBox.Range.GetRanges("\\b" + text + "\\b").ToArray();
		if (array.Length > 1)
		{
			Range[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetStyle(SameWordsStyle);
			}
		}
	}

	private void Txt_Formula_TextChanged(object sender, TextChangedEventArgs e)
	{
		txt_Formula.LeftBracket = '(';
		txt_Formula.RightBracket = ')';
		txt_Formula.LeftBracket2 = '{';
		txt_Formula.RightBracket2 = '}';
		Range range = txt_Formula.Range;
		range.ClearStyle(MediumSeaGreenStyle, RedStyle, MagentaStyle, DeepSkyBlueStyle, DarkVioletStyle, BlueStyle, CadetBlueStyle);
		range.SetStyle(MediumSeaGreenStyle, "//.*$", RegexOptions.Multiline);
		range.SetStyle(MediumSeaGreenStyle, "(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline);
		range.SetStyle(MediumSeaGreenStyle, "(/\\*.*?\\*/)|(.*\\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
		range.SetStyle(RedStyle, "\r\n                            # Character definitions:\r\n                            '\r\n                            (?> # disable backtracking\r\n                              (?:\r\n                                \\\\[^\\r\\n]|    # escaped meta char\r\n                                [^'\\r\\n]      # any character except '\r\n                              )*\r\n                            )\r\n                            '?\r\n                            |\r\n                            # Normal string & verbatim strings definitions:\r\n                            (?<verbatimIdentifier>@)?         # this group matches if it is an verbatim string\r\n                            \"\r\n                            (?> # disable backtracking\r\n                              (?:\r\n                                # match and consume an escaped character including escaped double quote (\") char\r\n                                (?(verbatimIdentifier)        # if it is a verbatim string ...\r\n                                  \"\"|                         #   then: only match an escaped double quote (\") char\r\n                                  \\\\.                         #   else: match an escaped sequence\r\n                                )\r\n                                | # OR\r\n            \r\n                                # match any char except double quote char (\")\r\n                                [^\"]\r\n                              )*\r\n                            )\r\n                            \"\r\n                        ", RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		range.SetStyle(MagentaStyle, "\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b");
		range.SetStyle(DeepSkyBlueStyle, "(" + string.Join("|", ConstantsApzCtd.HighlightGlobal) + ")\\b", RegexOptions.IgnoreCase);
		range.SetStyle(DarkVioletStyle, "\\b(" + string.Join("|", ConstantsApzCtd.HighlightFunctions) + ")\\b", RegexOptions.IgnoreCase);
		range.SetStyle(BlueStyle, "\\b(" + string.Join("|", ConstantsApzCtd.HighlightKeywords) + ")\\b", RegexOptions.IgnoreCase);
		range.SetStyle(CadetBlueStyle, "\\b(" + string.Join("|", ConstantsApzCtd.ColorKeyWork) + ")\\b", RegexOptions.IgnoreCase);
		range.ClearFoldingMarkers();
		range.SetFoldingMarkers("{", "}");
		range.SetFoldingMarkers("/\\*", "\\*/");
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	protected virtual void LoadEvents()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			if (ContextAction)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = -1;
				dataRow["Name"] = "Context Action Event";
				dataTable.Rows.Add(dataRow);
				look_Events.Enabled = false;
			}
			else
			{
				foreach (object value in Enum.GetValues(typeof(Enums.EventId)))
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Code"] = (int)(Enums.EventId)value;
					dataRow2["Name"] = value;
					dataTable.Rows.Add(dataRow2);
				}
			}
			look_Events.Properties.DataSource = dataTable;
			look_Events.Properties.DisplayMember = "Name";
			look_Events.Properties.ValueMember = "Code";
			look_Events.Properties.ShowHeader = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void InitValues()
	{
	}

	private void GetControl(Control control, ref List<GridControlEx> lst)
	{
		try
		{
			if (control == null)
			{
				return;
			}
			if (control.GetType() == typeof(GridControlEx))
			{
				lst.Add((GridControlEx)control);
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				GetControl(control2, ref lst);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void gv_DataSet_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("FieldName", typeof(string));
			if (Function.ToString(gv_DataSet.GetRowCellValue(e.RowHandle, "DataSet")).Equals(FormCode))
			{
				foreach (IControlEx item in BaseForm.LstControlSource)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ColumnName"] = item.FieldName;
					dataRow["FieldName"] = item.FieldName;
					dataTable.Rows.Add(dataRow);
				}
			}
			else
			{
				ViewEditModel viewEditModel = _listGridView.FirstOrDefault((ViewEditModel t) => t.ItemId.Equals(Function.ToString(gv_DataSet.GetRowCellValue(e.RowHandle, "DataSet"))));
				if (viewEditModel != null)
				{
					for (int num = 0; num < viewEditModel.DataSource.Columns.Count; num++)
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["ColumnName"] = viewEditModel.GetCaptionColumInView(viewEditModel.DataSource.Columns[num].ColumnName);
						dataRow2["FieldName"] = viewEditModel.DataSource.Columns[num].ColumnName;
						dataTable.Rows.Add(dataRow2);
					}
				}
			}
			dataTable.AcceptChanges();
			grd_Field.DataSource = dataTable;
			grd_Field.RefreshDataSource();
			gv_Field.BestFitColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void LoadDataSet()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("DataSet", typeof(string));
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("ColumnName", typeof(string));
			dataTable2.Columns.Add("FieldName", typeof(string));
			if (BaseForm != null)
			{
				DataTable dataTable3 = new DataTable();
				dataTable3.Columns.Add("Code", typeof(string));
				dataTable3.Columns.Add("Name", typeof(string));
				DataRow dataRow = dataTable3.NewRow();
				dataRow[0] = BaseForm.FormId;
				dataRow[1] = BaseForm.Text;
				dataTable3.Rows.Add(dataRow);
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Name"] = (string.IsNullOrEmpty(BaseForm.Text) ? BaseForm.Name : BaseForm.Text);
				dataRow2["DataSet"] = BaseForm.FormId;
				dataTable.Rows.Add(dataRow2);
				foreach (IControlEx item in BaseForm.LstControlSource)
				{
					DataRow dataRow3 = dataTable2.NewRow();
					dataRow3["ColumnName"] = item.FieldName;
					dataRow3["FieldName"] = item.FieldName;
					dataTable2.Rows.Add(dataRow3);
				}
				_listGridView = ControlFunction.GetListEditGriview(BaseForm);
				if (_listGridView.Count > 0)
				{
					for (int i = 0; i < _listGridView.Count; i++)
					{
						if (!string.IsNullOrEmpty(_listGridView[i].ItemId))
						{
							DataRow dataRow4 = dataTable3.NewRow();
							dataRow4[0] = _listGridView[i].ItemId;
							dataRow4[1] = _listGridView[i].Name;
							dataTable3.Rows.Add(dataRow4);
							DataRow dataRow5 = dataTable.NewRow();
							dataRow5["Name"] = (string.IsNullOrEmpty(_listGridView[i].ViewCaption) ? _listGridView[i].Name : _listGridView[i].ViewCaption);
							dataRow5["DataSet"] = _listGridView[i].ItemId;
							dataTable.Rows.Add(dataRow5);
						}
					}
				}
				dataTable3.AcceptChanges();
				if (ContextAction)
				{
					txtItemEvent.SetDataSource(new Dictionary<string, string> { { "-", "Context Action Item" } });
					txtItemEvent.Enabled = false;
				}
				else
				{
					txtItemEvent.Properties.DataSource = dataTable3;
				}
				grd_DataSet.DataSource = dataTable;
				grd_DataSet.RefreshDataSource();
				gv_DataSet.BestFitColumns();
				grd_Field.DataSource = dataTable2;
				grd_Field.RefreshDataSource();
				gv_Field.BestFitColumns();
				_dtFormula = new DataTable();
				_dtFormula.Columns.Add("FormID", typeof(string));
				_dtFormula.Columns.Add("ItemId", typeof(string));
				_dtFormula.Columns.Add("MethodID", typeof(string));
				_dtFormula.Columns.Add("Description", typeof(string));
				_dtFormula.Columns.Add("Formula", typeof(string));
				_dtFormula.Columns.Add("Active", typeof(string));
				DataTable dataTable4 = new DataTable();
				dataTable4.Columns.Add("Name", typeof(string));
				DataRow dataRow6 = dataTable4.NewRow();
				dataRow6["Name"] = "Functions";
				dataTable4.Rows.Add(dataRow6);
				dataRow6 = dataTable4.NewRow();
				dataRow6["Name"] = "DataType";
				dataTable4.Rows.Add(dataRow6);
				dataRow6 = dataTable4.NewRow();
				dataRow6["Name"] = "Constants";
				dataTable4.Rows.Add(dataRow6);
				dataRow6 = dataTable4.NewRow();
				dataRow6["Name"] = "Operators";
				dataTable4.Rows.Add(dataRow6);
				grd_Type.DataSource = dataTable4;
				grd_Type.RefreshDataSource();
				if (Function.ParseInt(Event) > 0)
				{
					look_Events.EditValue = Event;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ThisFormIsNotAvaiable, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			if (FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			if (ContextAction)
			{
				string text = Function.ToString(txt_Formula.Text);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.PleaseWriteFomularBeforeSaving, "");
					return;
				}
				string text2 = Verify();
				if (!string.IsNullOrEmpty(text2))
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
					return;
				}
				SaveContextActionScript(text);
				Close();
			}
			else
			{
				SaveData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void SaveData()
	{
		try
		{
			if (string.IsNullOrEmpty(FormCode))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCategory, "");
				return;
			}
			string value = Function.ToString(txt_Formula.Text);
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.PleaseWriteFomularBeforeSaving, "");
				return;
			}
			string value2 = Function.ToString(look_Events.EditValue);
			if (string.IsNullOrEmpty(value2))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseEventType, "");
				return;
			}
			string text = Function.ToString(txtItemEvent.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseOneOfItemProcessEvent, "");
				return;
			}
			try
			{
				if (text.Equals("oUserDefinedReports") && text.Equals(FormCode))
				{
					text += ((dynamic)BaseForm).ItemId;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			string text2 = Verify();
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable = _dtFormula.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["FormID"] = FormCode;
				dataRow["ItemId"] = text;
				dataRow["MethodID"] = value2;
				dataRow["Description"] = Function.ToString(txt_Description.EditValue);
				dataRow["Formula"] = value;
				dataRow["Active"] = Function.ToString(chk_Active.EditValue);
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await _httpClient.Formula.Save(dataTable);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public string Verify()
	{
		try
		{
			string text = Function.ToString(txt_Formula.Text);
			if (!string.IsNullOrEmpty(text))
			{
				return new Compiler().ParseSyntax(BaseForm, text).Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	private void btn_Verify_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Verify();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void look_Events_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			await GetFormulaText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task GetFormulaText()
	{
		try
		{
			if (ListEventInGridId.Contains(Function.ParseInt(look_Events.EditValue)))
			{
				if (Function.ToString(txtItemEvent.EditValue).Equals(FormCode) && !string.IsNullOrEmpty(txtItemEvent.Text) && !string.IsNullOrEmpty(look_Events.Text))
				{
					WinCommons.Error(Enums.MessageCode.EventCannotProcessInForm, "", look_Events.Text);
					txtItemEvent.EditValue = null;
					return;
				}
			}
			else if (!Function.ToString(txtItemEvent.EditValue).Equals(FormCode) && !string.IsNullOrEmpty(txtItemEvent.Text) && !string.IsNullOrEmpty(look_Events.Text))
			{
				WinCommons.Error(Enums.MessageCode.EventCannotProcessInView, "", look_Events.Text);
				txtItemEvent.EditValue = null;
				return;
			}
			string text = Function.ToString(look_Events.EditValue);
			string text2 = Function.ToString(txtItemEvent.EditValue);
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				try
				{
					if (text2.Equals("oUserDefinedReports") && text2.Equals(FormCode))
					{
						text2 += ((dynamic)BaseForm).ItemId;
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
				Enums.EventId enumEvent = (Enums.EventId)Function.ParseInt(text);
				DataTable dataTable = await _httpClient.Formula.GetFormula(FormCode, enumEvent, text2);
				if (dataTable.IsNotNull())
				{
					txt_Formula.Text = Function.ToString(dataTable.Rows[0]["Formula"]);
					txt_Description.EditValue = Function.ToString(dataTable.Rows[0]["Description"]);
					chk_Active.EditValue = Function.ToString(dataTable.Rows[0]["Active"]);
				}
				else
				{
					txt_Formula.Text = "";
				}
			}
			else
			{
				txt_Formula.Text = "";
			}
			txt_Formula.ClearUndo();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
		finally
		{
			FormMode = Enums.FormViewModeType.View;
		}
	}

	private void txt_Formula_KeyPress(object sender, KeyPressEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Field_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Field.FocusedRowHandle;
			if (focusedRowHandle <= -1)
			{
				return;
			}
			DXMouseEventArgs e2 = e as DXMouseEventArgs;
			if (!gv_Field.CalcHitInfo(e2.Location).InRow)
			{
				return;
			}
			string text = Function.ToString(gv_DataSet.GetFocusedRowCellValue("DataSet"));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string text2 = Function.ToString(gv_Field.GetRowCellValue(focusedRowHandle, "FieldName"));
			if (!string.IsNullOrEmpty(text2))
			{
				string text3 = "";
				if (Function.ToString(gv_DataSet.GetFocusedRowCellValue("DataSet")).Equals(FormCode))
				{
					text3 = $"$[{FormCode}].[\"{text2}\"]";
				}
				else
				{
					int num = Function.ParseInt(look_Events.EditValue);
					string text4 = Function.ToString(txtItemEvent.EditValue);
					text3 = ((num != 1 || (!(text4 == text) && !(FormId == "oSalaryFormula"))) ? string.Format("$[{0}].[$[{0}].FocusRow].[\"{1}\"]", text, text2) : $"$[{text}].[@rowIndex].[\"{text2}\"]");
				}
				txt_Formula.InsertText(text3);
				txt_Formula.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Type_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			switch (Function.ToString(gv_Type.GetFocusedRowCellValue("Name")))
			{
			case "Functions":
				foreach (string item in ConstantsApzCtd.HighlightFunctions.OrderBy((string f) => f))
				{
					DataRow dataRow4 = dataTable.NewRow();
					dataRow4["Name"] = item;
					dataTable.Rows.Add(dataRow4);
				}
				break;
			case "DataType":
			{
				string[] highlightOperations = ConstantsApzCtd.HighlightDataType;
				foreach (string value2 in highlightOperations)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Name"] = value2;
					dataTable.Rows.Add(dataRow2);
				}
				break;
			}
			case "Constants":
			{
				string[] highlightOperations = ConstantsApzCtd.HighlightGlobal;
				foreach (string text in highlightOperations)
				{
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3["Name"] = text.ToUpper();
					dataTable.Rows.Add(dataRow3);
				}
				break;
			}
			case "Operators":
			{
				string[] highlightOperations = ConstantsApzCtd.HighlightOperations;
				foreach (string value in highlightOperations)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Name"] = value;
					dataTable.Rows.Add(dataRow);
				}
				break;
			}
			}
			grd_Keywords.DataSource = dataTable;
			grd_Keywords.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Keywords_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Type.GetFocusedRowCellValue("Name")) == "Functions")
			{
				string text = Function.ToString(gv_Keywords.GetFocusedRowCellValue("Name")).ToLower();
				string text2 = "";
				text2 = text switch
				{
					"showmessagebox" => $"var ret=ShowMessageBox(\"\",\"System\",yesno)", 
					"if" => "if(true)\n{ \n\n}", 
					"for" => "for(var i,0,n,1)\n{ \n\n}", 
					"while" => "while(true)\n{ \n\n}", 
					"else" => "else\n{\n\n}", 
					"getdata" => "var data= GetData(string,\"\")", 
					"execute" => "var data=Execute(\"SELECT 1\",parameters)", 
					"substring" => "SubString(\"\",begin,end)", 
					"trim" => "Trim(\"\")", 
					"tolower" => "ToLower(\"\")", 
					"toupper" => "ToUpper(\"\")", 
					"getrowcellvalue" => "GetRowCellValue(rowindex,column)", 
					"setfocusrowcellvalue" => "SetFocusRowCellValue(column,value)", 
					"setrowcellvalue" => "SetRowCellValue(rowindex,column,value)", 
					"getfocusrowcellvalue" => "GetFocusRowCellValue(rowindex,column)", 
					"max" => "Max(column)", 
					"min" => "Min(column)", 
					"avg" => "Avg(column)", 
					"sum" => "Sum(column)", 
					"sqrt" => "Sqrt(value)", 
					"abs" => "Abs(value)", 
					"showsuccess" => "ShowSuccess(\"\")", 
					"showwarning" => "ShowWarning\"\")", 
					"showerror" => "ShowError(\"\")", 
					_ => text, 
				};
				txt_Formula.InsertText(text2);
				txt_Formula.Focus();
			}
			else if (gv_Field.FocusedRowHandle > -1)
			{
				string text3 = Function.ToString(gv_Keywords.GetFocusedRowCellValue("Name"));
				if (!string.IsNullOrEmpty(text3))
				{
					string text4 = $"{text3}";
					txt_Formula.InsertText(text4);
					txt_Formula.Focus();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Description_EditValueChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void chk_Active_CheckedChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private async void txtItemEvent_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			await GetFormulaText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async void btnDelete_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(FormCode))
			{
				WinCommons.Error(Enums.MessageCode.FormIdCannotBeNull, "");
				return;
			}
			string text = Function.ToString(txtItemEvent.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.ItemIdCannotBeNull, "");
				return;
			}
			int num = Function.ParseInt(look_Events.EditValue);
			if (num == 0)
			{
				WinCommons.Error(Enums.MessageCode.EventIdCannotBeNull, "");
				return;
			}
			try
			{
				if (text.Equals("oUserDefinedReports") && text.Equals(FormCode))
				{
					text += ((dynamic)BaseForm).ItemId;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text2 = await _httpClient.Formula.DeleteFormula(FormCode, text, (Enums.EventId)num);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					txt_Formula.Text = "";
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_UserDefinedProcedureCreate));
		this.grd_DataSet = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DataSet = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Field = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Field = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.look_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.look_dep = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_Dept = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Branch = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_Description = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.look_Events = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Verify = new Apzon.Controls.Controls.SimpleButtonEx();
		this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
		this.autoHideContainer1 = new DevExpress.XtraBars.Docking.AutoHideContainer();
		this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.grd_Keywords = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Keywords = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Type = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Type = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
		this.hideContainerRight = new DevExpress.XtraBars.Docking.AutoHideContainer();
		this.lbItemEvent = new Apzon.Controls.Controls.LabelLine();
		this.txtItemEvent = new Apzon.Controls.Editor.LookUpEditEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_DataSet).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DataSet).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Field).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Field).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_dep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Dept.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Events.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).BeginInit();
		this.autoHideContainer1.SuspendLayout();
		this.dockPanel1.SuspendLayout();
		this.dockPanel1_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Keywords).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Keywords).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtItemEvent.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		base.SuspendLayout();
		this.grd_DataSet.AllowPasteExcel = false;
		this.grd_DataSet.ClipboardData = null;
		this.grd_DataSet.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DataSet.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_DataSet.Location = new System.Drawing.Point(0, 0);
		this.grd_DataSet.MainView = this.gv_DataSet;
		this.grd_DataSet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_DataSet.Name = "grd_DataSet";
		this.grd_DataSet.Size = new System.Drawing.Size(338, 255);
		this.grd_DataSet.TabIndex = 0;
		this.grd_DataSet.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DataSet });
		this.gv_DataSet.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DataSet.ColumnKey = null;
		this.gv_DataSet.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gv_DataSet.DetailHeight = 431;
		this.gv_DataSet.DisableFormSetting = true;
		this.gv_DataSet.DisplayException = false;
		this.gv_DataSet.FieldCurrency = null;
		this.gv_DataSet.FieldRate = null;
		this.gv_DataSet.FormId = "oUserDefinedProcedureCreate";
		this.gv_DataSet.GridControl = this.grd_DataSet;
		this.gv_DataSet.IndicatorWidth = 41;
		this.gv_DataSet.IsApzTable = false;
		this.gv_DataSet.IsInsertNewRow = false;
		this.gv_DataSet.IsLoadingData = false;
		this.gv_DataSet.ItemId = "OITEM";
		this.gv_DataSet.LstColumnAllowBatch = null;
		this.gv_DataSet.LstColumnNotAllowBatch = null;
		this.gv_DataSet.LstColumnNotEditable = null;
		this.gv_DataSet.LstColumNotSetting = null;
		this.gv_DataSet.Name = "gv_DataSet";
		this.gv_DataSet.OptionsBehavior.Editable = false;
		this.gv_DataSet.UseBatchMod = false;
		this.gv_DataSet.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_DataSet_FocusedRowObjectChanged);
		this.gridColumn1.Caption = "Name";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.MinWidth = 23;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 87;
		this.gridColumn2.Caption = "DataSet";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DataSet";
		this.gridColumn2.MinWidth = 23;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 87;
		this.grd_Field.AllowPasteExcel = false;
		this.grd_Field.ClipboardData = null;
		this.grd_Field.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Field.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Field.Location = new System.Drawing.Point(0, 0);
		this.grd_Field.MainView = this.gv_Field;
		this.grd_Field.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Field.Name = "grd_Field";
		this.grd_Field.Size = new System.Drawing.Size(338, 543);
		this.grd_Field.TabIndex = 1;
		this.grd_Field.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Field });
		this.gv_Field.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Field.ColumnKey = null;
		this.gv_Field.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn3, this.gridColumn4 });
		this.gv_Field.DetailHeight = 431;
		this.gv_Field.DisableFormSetting = true;
		this.gv_Field.DisplayException = false;
		this.gv_Field.FieldCurrency = null;
		this.gv_Field.FieldRate = null;
		this.gv_Field.FormId = "oUserDefinedProcedureCreate";
		this.gv_Field.GridControl = this.grd_Field;
		this.gv_Field.IndicatorWidth = 41;
		this.gv_Field.IsApzTable = false;
		this.gv_Field.IsInsertNewRow = false;
		this.gv_Field.IsLoadingData = false;
		this.gv_Field.ItemId = "OFIELD";
		this.gv_Field.LstColumnAllowBatch = null;
		this.gv_Field.LstColumnNotAllowBatch = null;
		this.gv_Field.LstColumnNotEditable = null;
		this.gv_Field.LstColumNotSetting = null;
		this.gv_Field.Name = "gv_Field";
		this.gv_Field.OptionsBehavior.Editable = false;
		this.gv_Field.UseBatchMod = false;
		this.gv_Field.DoubleClick += new System.EventHandler(this.gv_Field_DoubleClick);
		this.gridColumn3.Caption = "Column Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ColumnName";
		this.gridColumn3.MinWidth = 23;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn3.Width = 189;
		this.gridColumn4.Caption = "Field Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "FieldName";
		this.gridColumn4.MinWidth = 23;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn4.Width = 241;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1349, 810);
		this.splitContainerControl1.SplitterPosition = 338;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_DataSet);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Field);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(338, 810);
		this.splitContainerControl2.SplitterPosition = 255;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.look_Branch.Location = new System.Drawing.Point(960, 10);
		this.look_Branch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.look_Branch.Name = "look_Branch";
		this.look_Branch.Properties.AllowMouseWheel = false;
		this.look_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Branch.Properties.NullText = "";
		this.look_Branch.Size = new System.Drawing.Size(134, 22);
		this.look_Branch.TabIndex = 12;
		this.look_Branch.Visible = false;
		this.look_dep.Location = new System.Drawing.Point(1199, 10);
		this.look_dep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.look_dep.Name = "look_dep";
		this.look_dep.Properties.AllowMouseWheel = false;
		this.look_dep.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_dep.Properties.NullText = "";
		this.look_dep.Size = new System.Drawing.Size(112, 22);
		this.look_dep.TabIndex = 11;
		this.look_dep.Visible = false;
		this.chk_Dept.EditControl = null;
		this.chk_Dept.EditValue = "N";
		this.chk_Dept.FieldName = null;
		this.chk_Dept.IsSearchControl = false;
		this.chk_Dept.Location = new System.Drawing.Point(1099, 12);
		this.chk_Dept.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Dept.Name = "chk_Dept";
		this.chk_Dept.Properties.Caption = "Department";
		this.chk_Dept.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Dept.Properties.ValueChecked = "Y";
		this.chk_Dept.Properties.ValueGrayed = "N";
		this.chk_Dept.Properties.ValueUnchecked = "N";
		this.chk_Dept.SettingInVisible = false;
		this.chk_Dept.SettingReadonly = false;
		this.chk_Dept.Size = new System.Drawing.Size(94, 24);
		this.chk_Dept.TabIndex = 16;
		this.chk_Dept.Visible = false;
		this.chk_Branch.EditControl = null;
		this.chk_Branch.EditValue = "N";
		this.chk_Branch.FieldName = null;
		this.chk_Branch.IsSearchControl = false;
		this.chk_Branch.Location = new System.Drawing.Point(877, 12);
		this.chk_Branch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Branch.Name = "chk_Branch";
		this.chk_Branch.Properties.Caption = "Branch";
		this.chk_Branch.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Branch.Properties.ValueChecked = "Y";
		this.chk_Branch.Properties.ValueGrayed = "N";
		this.chk_Branch.Properties.ValueUnchecked = "N";
		this.chk_Branch.SettingInVisible = false;
		this.chk_Branch.SettingReadonly = false;
		this.chk_Branch.Size = new System.Drawing.Size(76, 24);
		this.chk_Branch.TabIndex = 15;
		this.chk_Branch.Visible = false;
		this.chk_Active.EditControl = null;
		this.chk_Active.EditValue = "Y";
		this.chk_Active.FieldName = null;
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(794, 12);
		this.chk_Active.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(77, 24);
		this.chk_Active.TabIndex = 10;
		this.chk_Active.CheckedChanged += new System.EventHandler(this.chk_Active_CheckedChanged);
		this.txt_Description.EditControl = null;
		this.txt_Description.FieldName = null;
		this.txt_Description.IsSearchControl = false;
		this.txt_Description.Location = new System.Drawing.Point(624, 9);
		this.txt_Description.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Description.Name = "txt_Description";
		this.txt_Description.SettingInVisible = false;
		this.txt_Description.SettingReadonly = false;
		this.txt_Description.Size = new System.Drawing.Size(163, 22);
		this.txt_Description.TabIndex = 8;
		this.txt_Description.EditValueChanged += new System.EventHandler(this.txt_Description_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(535, 11);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(77, 25);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 7;
		this.labelLine2.Utext = "Description";
		this.look_Events.Location = new System.Drawing.Point(80, 7);
		this.look_Events.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.look_Events.Name = "look_Events";
		this.look_Events.Properties.AllowMouseWheel = false;
		this.look_Events.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Events.Properties.NullText = "";
		this.look_Events.Size = new System.Drawing.Size(178, 22);
		this.look_Events.TabIndex = 6;
		this.look_Events.EditValueChanged += new System.EventHandler(this.look_Events_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "On Event";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(16, 11);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(57, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 5;
		this.labelLine1.Utext = "On Event";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 818);
		this.btn_OK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(87, 28);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Verify.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Verify.EditControl = null;
		this.btn_Verify.EditValue = "Verify Syntax";
		this.btn_Verify.FieldName = null;
		this.btn_Verify.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Verify.ImageOptions.Image");
		this.btn_Verify.IsSearchControl = false;
		this.btn_Verify.Location = new System.Drawing.Point(1222, 818);
		this.btn_Verify.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_Verify.Name = "btn_Verify";
		this.btn_Verify.ReadOnly = false;
		this.btn_Verify.SettingInVisible = false;
		this.btn_Verify.SettingReadonly = false;
		this.btn_Verify.Size = new System.Drawing.Size(122, 28);
		this.btn_Verify.TabIndex = 4;
		this.btn_Verify.Text = "Verify Syntax";
		this.btn_Verify.Click += new System.EventHandler(this.btn_Verify_Click);
		this.dockManager1.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[1] { this.autoHideContainer1 });
		this.dockManager1.Form = this;
		this.dockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.autoHideContainer1.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.autoHideContainer1.Controls.Add(this.dockPanel1);
		this.autoHideContainer1.Dock = System.Windows.Forms.DockStyle.Right;
		this.autoHideContainer1.Location = new System.Drawing.Point(1374, 0);
		this.autoHideContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.autoHideContainer1.Name = "autoHideContainer1";
		this.autoHideContainer1.Size = new System.Drawing.Size(26, 900);
		this.dockPanel1.Controls.Add(this.dockPanel1_Container);
		this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
		this.dockPanel1.FloatVertical = true;
		this.dockPanel1.ID = new System.Guid("8285b313-0c0b-4c32-88f5-df370ea0b60a");
		this.dockPanel1.Location = new System.Drawing.Point(0, 0);
		this.dockPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dockPanel1.Name = "dockPanel1";
		this.dockPanel1.Options.ShowCloseButton = false;
		this.dockPanel1.OriginalSize = new System.Drawing.Size(352, 200);
		this.dockPanel1.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
		this.dockPanel1.SavedIndex = 0;
		this.dockPanel1.Size = new System.Drawing.Size(352, 900);
		this.dockPanel1.Text = "Properties";
		this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
		this.dockPanel1_Container.Controls.Add(this.grd_Keywords);
		this.dockPanel1_Container.Controls.Add(this.grd_Type);
		this.dockPanel1_Container.Location = new System.Drawing.Point(6, 32);
		this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dockPanel1_Container.Name = "dockPanel1_Container";
		this.dockPanel1_Container.Size = new System.Drawing.Size(342, 864);
		this.dockPanel1_Container.TabIndex = 0;
		this.grd_Keywords.AllowPasteExcel = false;
		this.grd_Keywords.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Keywords.ClipboardData = null;
		this.grd_Keywords.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Keywords.Location = new System.Drawing.Point(1, 206);
		this.grd_Keywords.MainView = this.gv_Keywords;
		this.grd_Keywords.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Keywords.Name = "grd_Keywords";
		this.grd_Keywords.Size = new System.Drawing.Size(341, 659);
		this.grd_Keywords.TabIndex = 7;
		this.grd_Keywords.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Keywords });
		this.gv_Keywords.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Keywords.ColumnKey = null;
		this.gv_Keywords.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx2 });
		this.gv_Keywords.DetailHeight = 431;
		this.gv_Keywords.DisableFormSetting = true;
		this.gv_Keywords.DisplayException = false;
		this.gv_Keywords.FieldCurrency = null;
		this.gv_Keywords.FieldRate = null;
		this.gv_Keywords.FormId = "oUserDefinedProcedureCreate";
		this.gv_Keywords.GridControl = this.grd_Keywords;
		this.gv_Keywords.IndicatorWidth = 41;
		this.gv_Keywords.IsApzTable = false;
		this.gv_Keywords.IsInsertNewRow = false;
		this.gv_Keywords.IsLoadingData = false;
		this.gv_Keywords.ItemId = "OKEYWORD";
		this.gv_Keywords.LstColumnAllowBatch = null;
		this.gv_Keywords.LstColumnNotAllowBatch = null;
		this.gv_Keywords.LstColumnNotEditable = null;
		this.gv_Keywords.LstColumNotSetting = null;
		this.gv_Keywords.Name = "gv_Keywords";
		this.gv_Keywords.OptionsBehavior.Editable = false;
		this.gv_Keywords.OptionsView.ShowColumnHeaders = false;
		this.gv_Keywords.OptionsView.ShowGroupPanel = false;
		this.gv_Keywords.UseBatchMod = false;
		this.gv_Keywords.DoubleClick += new System.EventHandler(this.gv_Keywords_DoubleClick);
		this.gridColumnEx2.Caption = "Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.MinWidth = 23;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx2.Width = 175;
		this.grd_Type.AllowPasteExcel = false;
		this.grd_Type.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Type.ClipboardData = null;
		this.grd_Type.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Type.Location = new System.Drawing.Point(3, 0);
		this.grd_Type.MainView = this.gv_Type;
		this.grd_Type.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Type.Name = "grd_Type";
		this.grd_Type.Size = new System.Drawing.Size(338, 199);
		this.grd_Type.TabIndex = 0;
		this.grd_Type.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Type });
		this.gv_Type.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Type.ColumnKey = null;
		this.gv_Type.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx1 });
		this.gv_Type.DetailHeight = 431;
		this.gv_Type.DisableFormSetting = true;
		this.gv_Type.DisplayException = false;
		this.gv_Type.FieldCurrency = null;
		this.gv_Type.FieldRate = null;
		this.gv_Type.FormId = "oUserDefinedProcedureCreate";
		this.gv_Type.GridControl = this.grd_Type;
		this.gv_Type.IndicatorWidth = 41;
		this.gv_Type.IsApzTable = false;
		this.gv_Type.IsInsertNewRow = false;
		this.gv_Type.IsLoadingData = false;
		this.gv_Type.ItemId = "OTYPE";
		this.gv_Type.LstColumnAllowBatch = null;
		this.gv_Type.LstColumnNotAllowBatch = null;
		this.gv_Type.LstColumnNotEditable = null;
		this.gv_Type.LstColumNotSetting = null;
		this.gv_Type.Name = "gv_Type";
		this.gv_Type.OptionsBehavior.Editable = false;
		this.gv_Type.OptionsView.ShowColumnHeaders = false;
		this.gv_Type.OptionsView.ShowGroupPanel = false;
		this.gv_Type.UseBatchMod = false;
		this.gv_Type.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Type_FocusedRowObjectChanged);
		this.gridColumnEx1.Caption = "Name";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Name";
		this.gridColumnEx1.MinWidth = 23;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 87;
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.btnDelete);
		this.panelControl1.Controls.Add(this.splitContainerControl1);
		this.panelControl1.Controls.Add(this.btn_Verify);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Location = new System.Drawing.Point(3, 43);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1371, 852);
		this.panelControl1.TabIndex = 5;
		this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnDelete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnDelete.ImageOptions.Image");
		this.btnDelete.Location = new System.Drawing.Point(101, 818);
		this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnDelete.Name = "btnDelete";
		this.btnDelete.Size = new System.Drawing.Size(87, 28);
		this.btnDelete.TabIndex = 5;
		this.btnDelete.Text = "Delete";
		this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
		this.hideContainerRight.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.hideContainerRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.hideContainerRight.Location = new System.Drawing.Point(1218, 0);
		this.hideContainerRight.Name = "hideContainerRight";
		this.hideContainerRight.Size = new System.Drawing.Size(19, 518);
		this.lbItemEvent.EditControl = null;
		this.lbItemEvent.EditValue = "Item Event";
		this.lbItemEvent.FieldName = null;
		this.lbItemEvent.HideWhenEmpty = false;
		this.lbItemEvent.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbItemEvent.Location = new System.Drawing.Point(268, 11);
		this.lbItemEvent.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lbItemEvent.Name = "lbItemEvent";
		this.lbItemEvent.ReadOnly = false;
		this.lbItemEvent.SettingInVisible = false;
		this.lbItemEvent.SettingReadonly = false;
		this.lbItemEvent.ShowUnderLine = true;
		this.lbItemEvent.Size = new System.Drawing.Size(80, 25);
		this.lbItemEvent.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbItemEvent.TabIndex = 17;
		this.lbItemEvent.Utext = "Item Event";
		this.txtItemEvent.EditControl = null;
		this.txtItemEvent.FieldName = null;
		this.txtItemEvent.IsSearchControl = false;
		this.txtItemEvent.Location = new System.Drawing.Point(350, 7);
		this.txtItemEvent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtItemEvent.Name = "txtItemEvent";
		this.txtItemEvent.Properties.AllowMouseWheel = false;
		this.txtItemEvent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtItemEvent.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Item Id")
		});
		this.txtItemEvent.Properties.DisplayMember = "Name";
		this.txtItemEvent.Properties.NullText = "";
		this.txtItemEvent.Properties.ValueMember = "Code";
		this.txtItemEvent.SettingInVisible = false;
		this.txtItemEvent.SettingReadonly = false;
		this.txtItemEvent.Size = new System.Drawing.Size(174, 22);
		this.txtItemEvent.TabIndex = 18;
		this.txtItemEvent.EditValueChanged += new System.EventHandler(this.txtItemEvent_EditValueChanged);
		this.panelControl2.Controls.Add(this.txt_Description);
		this.panelControl2.Controls.Add(this.panelControl1);
		this.panelControl2.Controls.Add(this.txtItemEvent);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.lbItemEvent);
		this.panelControl2.Controls.Add(this.look_Events);
		this.panelControl2.Controls.Add(this.look_Branch);
		this.panelControl2.Controls.Add(this.chk_Active);
		this.panelControl2.Controls.Add(this.labelLine1);
		this.panelControl2.Controls.Add(this.look_dep);
		this.panelControl2.Controls.Add(this.chk_Branch);
		this.panelControl2.Controls.Add(this.chk_Dept);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(1374, 900);
		this.panelControl2.TabIndex = 20;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1400, 900);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.autoHideContainer1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUserDefinedProcedureCreate";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_UserDefinedProcedureCreate";
		this.Text = "DIY Script";
		base.Load += new System.EventHandler(this.frm_CreateFormula_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_DataSet).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DataSet).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Field).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Field).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_dep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Dept.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Events.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).EndInit();
		this.autoHideContainer1.ResumeLayout(false);
		this.dockPanel1.ResumeLayout(false);
		this.dockPanel1_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Keywords).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Keywords).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtItemEvent.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
