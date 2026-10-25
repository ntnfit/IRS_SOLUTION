using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_QueryGenerator : XtraFormEx
{
	private readonly DataTable _dt = new DataTable();

	private int _focus;

	private IContainer components;

	private TextEditEx txt_table;

	private GridControlEx grd_Doc;

	private GridControlEx grd_Line;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private Label label1;

	private MemoEditEx memo_Select;

	private Label label2;

	private MemoEditEx memo_From;

	private Label label3;

	private MemoEditEx memo_Where;

	private Label label4;

	private MemoEditEx memo_SortBy;

	private Label label5;

	private MemoEditEx memo_GroupBy;

	private SimpleButtonEx bt_X;

	private SimpleButtonEx bt_Move;

	private SimpleButtonEx btn_Execute;

	private SimpleButtonEx btn_Close;

	private SimpleButtonEx btn_Conditions;

	private GridViewEx gv_Doc;

	private GridViewEx gv_Line;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	public frm_QueryGenerator()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_QueryGenerator"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_QueryGenerator" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Execute.Enabled = false;
			}
			else
			{
				btn_Execute.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_QueryGenerator_Load(object sender, EventArgs e)
	{
	}

	private void bt_Execute_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableControl(isEnable: false);
		try
		{
			string text = "";
			text = (string.IsNullOrEmpty(memo_Select.Text) ? (text + "SELECT  *  ") : (text + "SELECT  " + memo_Select.Text));
			if (!string.IsNullOrEmpty(memo_From.Text))
			{
				text = text + " FROM  " + memo_From.Text;
			}
			if (!string.IsNullOrEmpty(memo_Where.Text))
			{
				text = text + "where " + memo_Where.Text;
			}
			if (!string.IsNullOrEmpty(memo_SortBy.Text) && string.IsNullOrEmpty(memo_GroupBy.Text))
			{
				text = text + " ORDER BY  " + memo_SortBy.Text;
			}
			if (!string.IsNullOrEmpty(memo_GroupBy.Text) && string.IsNullOrEmpty(memo_SortBy.Text))
			{
				text = text + " GROUP BY " + memo_GroupBy.Text;
			}
			frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
			frm_QueryPreview2.Sql = text;
			frm_QueryPreview2.MdiParent = MdiParent;
			frm_QueryPreview2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private async void txt_table_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void btn_Conditions_Click(object sender, EventArgs e)
	{
		try
		{
			XtraMessageBoxEx.Show(Enums.MessageCode.InfoQueryCondition, Enums.MessageCode.Conditions, MessageBoxButtons.OK);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.CannotAccess, "");
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Line_RowClick(object sender, RowClickEventArgs e)
	{
		try
		{
			switch (_focus)
			{
			case 1:
				if (!string.IsNullOrWhiteSpace(memo_Select.Text))
				{
					if (_dt.Rows.Count > 1)
					{
						memo_Select.Text += string.Format(",T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
					else
					{
						memo_Select.Text += string.Format(",\"{0}\"", Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
				}
				else if (_dt.Rows.Count > 1)
				{
					memo_Select.Text = string.Format("T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
				}
				else
				{
					memo_Select.Text = "\"" + Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")) + "\"";
				}
				break;
			case 2:
				if (!string.IsNullOrWhiteSpace(memo_Where.Text))
				{
					if (_dt.Rows.Count > 1)
					{
						memo_Where.Text += string.Format(",T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
					else
					{
						memo_Where.Text += string.Format(",\"{0}\"", Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
				}
				else if (_dt.Rows.Count > 1)
				{
					memo_Where.Text += string.Format("T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
				}
				else
				{
					memo_Where.Text = "\"" + Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")) + "\"";
				}
				break;
			case 3:
				if (!string.IsNullOrWhiteSpace(memo_SortBy.Text))
				{
					if (_dt.Rows.Count > 1)
					{
						memo_SortBy.Text += string.Format(",T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
					else
					{
						memo_SortBy.Text += string.Format(",\"{0}\"", Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
				}
				else if (_dt.Rows.Count > 1)
				{
					memo_SortBy.Text += string.Format("T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
				}
				else
				{
					memo_SortBy.Text = "\"" + Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")) + "\"";
				}
				break;
			case 4:
				if (!string.IsNullOrWhiteSpace(memo_GroupBy.Text))
				{
					if (_dt.Rows.Count > 1)
					{
						memo_GroupBy.Text += string.Format(",T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
					else
					{
						memo_GroupBy.Text += string.Format(",\"{0}\"", Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
					}
				}
				else if (_dt.Rows.Count > 1)
				{
					memo_GroupBy.Text += string.Format("T{0}.\"{1}\"", gv_Doc.FocusedRowHandle, Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")));
				}
				else
				{
					memo_GroupBy.Text = "\"" + Function.ToString(gv_Line.GetFocusedRowCellValue("COLUMN_NAME")) + "\"";
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		await GetColumTable(Function.ToString(gv_Doc.GetFocusedRowCellValue("TABLE")));
	}

	private void gv_Line_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void memo_Select_Enter(object sender, EventArgs e)
	{
		_focus = 1;
	}

	private void memo_Where_Enter(object sender, EventArgs e)
	{
		_focus = 2;
	}

	private void memo_SortBy_Enter(object sender, EventArgs e)
	{
		_focus = 3;
	}

	private void memo_GroupBy_Enter(object sender, EventArgs e)
	{
		_focus = 4;
	}

	private void memo_From_Enter(object sender, EventArgs e)
	{
		_focus = 0;
	}

	private void bt_X_Click(object sender, EventArgs e)
	{
		try
		{
			grd_Doc.DataSourceTable.Rows.RemoveAt(gv_Doc.GetFocusedDataSourceRowIndex());
			grd_Doc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> GetColumTable(string tablename)
	{
		try
		{
			if (string.IsNullOrEmpty(tablename))
			{
				grd_Line.DataSourceTable.Rows.Clear();
				grd_Line.DataSourceTable.AcceptChanges();
				return false;
			}
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await helper.PostDataTableAsync("GetAllFieldTable", tablename);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_Line.DataSource = dataTable;
				grd_Line.RefreshDataSource();
				return true;
			}
			WinCommons.Error(Enums.MessageCode.CannotFindTable, "");
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Close.Enabled = isEnable;
			btn_Conditions.Enabled = isEnable;
			btn_Execute.Enabled = isEnable;
			txt_table.ReadOnly = !isEnable;
			memo_From.ReadOnly = !isEnable;
			memo_GroupBy.ReadOnly = !isEnable;
			memo_Select.ReadOnly = !isEnable;
			memo_SortBy.ReadOnly = !isEnable;
			memo_Where.ReadOnly = !isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void txt_table_Validated(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string table = txt_table.Text.ToUpper();
			if (string.IsNullOrEmpty(table))
			{
				return;
			}
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			if (!_dt.Columns.Contains("TABLE"))
			{
				_dt.Columns.Add("TABLE", typeof(string));
				_dt.Columns["TABLE"].ReadOnly = true;
			}
			if (_dt.Rows.Count == 0)
			{
				DataRow dr = _dt.NewRow();
				dr["TABLE"] = table;
				if (await GetColumTable(table))
				{
					_dt.Rows.Add(dr);
					grd_Doc.DataSource = _dt;
					gv_Doc.OptionsView.ShowColumnHeaders = false;
					gv_Doc.FocusedRowHandle = _dt.Rows.Count - 1;
					if (!string.IsNullOrWhiteSpace(memo_From.Text))
					{
						memo_From.Text += $",\"{table}\"";
					}
					else
					{
						memo_From.Text = "\"" + table + "\"";
					}
				}
			}
			for (int i = 0; i < _dt.Rows.Count && !txt_table.Text.ToLower().Equals(Function.ToString(_dt.Rows[i]["TABLE"]).ToLower()); i++)
			{
				if (i != _dt.Rows.Count - 1)
				{
					continue;
				}
				DataRow dr = _dt.NewRow();
				dr["TABLE"] = table;
				if (!(await GetColumTable(table)))
				{
					continue;
				}
				_dt.Rows.Add(dr);
				grd_Doc.DataSource = _dt;
				gv_Doc.OptionsView.ShowColumnHeaders = false;
				gv_Doc.FocusedRowHandle = _dt.Rows.Count - 1;
				if (!string.IsNullOrWhiteSpace(memo_From.Text))
				{
					if (_dt.Rows.Count == 2)
					{
						memo_From.Text += $" T0,\"{table}\" T1";
					}
					else if (_dt.Rows.Count > 2)
					{
						memo_From.Text += $" ,\"{table}\" T{i + 1}";
					}
				}
				else
				{
					memo_From.Text = table;
				}
			}
			if (_dt.Rows.Count == 2)
			{
				string[] array = memo_Select.Text.Split(',');
				memo_Select.Text = "";
				for (int j = 0; j < array.Length; j++)
				{
					if (j == array.Length - 1 && array[j] != "")
					{
						memo_Select.Text += $" T0.{array[j]}";
						break;
					}
					if (array[j] != "")
					{
						memo_Select.Text += $" T0.{array[j]} ,";
					}
				}
			}
			_focus = 1;
			txt_table.ResetText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void txt_table_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			txt_table_Validated(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_QueryGenerator));
		this.txt_table = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.label1 = new System.Windows.Forms.Label();
		this.memo_Select = new Apzon.Controls.Editor.MemoEditEx();
		this.label2 = new System.Windows.Forms.Label();
		this.memo_From = new Apzon.Controls.Editor.MemoEditEx();
		this.label3 = new System.Windows.Forms.Label();
		this.memo_Where = new Apzon.Controls.Editor.MemoEditEx();
		this.label4 = new System.Windows.Forms.Label();
		this.memo_SortBy = new Apzon.Controls.Editor.MemoEditEx();
		this.label5 = new System.Windows.Forms.Label();
		this.memo_GroupBy = new Apzon.Controls.Editor.MemoEditEx();
		this.bt_X = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Move = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Execute = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Conditions = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.txt_table.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Select.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Where.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_SortBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_GroupBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		base.SuspendLayout();
		this.txt_table.EditControl = null;
		this.txt_table.FieldName = null;
		this.txt_table.IsSearchControl = false;
		this.txt_table.Location = new System.Drawing.Point(0, 3);
		this.txt_table.Name = "txt_table";
		this.txt_table.SettingInVisible = false;
		this.txt_table.SettingReadonly = false;
		this.txt_table.Size = new System.Drawing.Size(99, 20);
		this.txt_table.TabIndex = 0;
		this.txt_table.EditValueChanged += new System.EventHandler(this.txt_table_EditValueChanged);
		this.txt_table.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_table_KeyDown);
		this.txt_table.Validated += new System.EventHandler(this.txt_table_Validated);
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(-1, 24);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(100, 415);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oQueryGenerator";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OQRT";
		this.gv_Doc.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(231, 471);
		this.grd_Line.TabIndex = 2;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oQueryGenerator";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "oColumns";
		this.gv_Line.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.ReadOnly = true;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_Line_RowClick);
		this.gv_Line.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Line_FocusedRowChanged);
		this.gridColumn1.Caption = "Name";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "COLUMN_NAME";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Description";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(10, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(36, 13);
		this.label1.TabIndex = 3;
		this.label1.Text = "Select";
		this.memo_Select.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Select.EditControl = null;
		this.memo_Select.FieldName = null;
		this.memo_Select.IsSearchControl = false;
		this.memo_Select.Location = new System.Drawing.Point(10, 16);
		this.memo_Select.Name = "memo_Select";
		this.memo_Select.SettingInVisible = false;
		this.memo_Select.SettingReadonly = false;
		this.memo_Select.Size = new System.Drawing.Size(595, 46);
		this.memo_Select.TabIndex = 4;
		this.memo_Select.Enter += new System.EventHandler(this.memo_Select_Enter);
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(8, 65);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(31, 13);
		this.label2.TabIndex = 3;
		this.label2.Text = "From";
		this.memo_From.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_From.EditControl = null;
		this.memo_From.FieldName = null;
		this.memo_From.IsSearchControl = false;
		this.memo_From.Location = new System.Drawing.Point(10, 81);
		this.memo_From.Name = "memo_From";
		this.memo_From.SettingInVisible = false;
		this.memo_From.SettingReadonly = false;
		this.memo_From.Size = new System.Drawing.Size(595, 46);
		this.memo_From.TabIndex = 5;
		this.memo_From.Enter += new System.EventHandler(this.memo_From_Enter);
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(11, 134);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(39, 13);
		this.label3.TabIndex = 3;
		this.label3.Text = "Where";
		this.memo_Where.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Where.EditControl = null;
		this.memo_Where.FieldName = null;
		this.memo_Where.IsSearchControl = false;
		this.memo_Where.Location = new System.Drawing.Point(10, 150);
		this.memo_Where.Name = "memo_Where";
		this.memo_Where.SettingInVisible = false;
		this.memo_Where.SettingReadonly = false;
		this.memo_Where.Size = new System.Drawing.Size(595, 46);
		this.memo_Where.TabIndex = 6;
		this.memo_Where.Enter += new System.EventHandler(this.memo_Where_Enter);
		this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(10, 202);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(42, 13);
		this.label4.TabIndex = 3;
		this.label4.Text = "Sort By";
		this.memo_SortBy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_SortBy.EditControl = null;
		this.memo_SortBy.FieldName = null;
		this.memo_SortBy.IsSearchControl = false;
		this.memo_SortBy.Location = new System.Drawing.Point(10, 218);
		this.memo_SortBy.Name = "memo_SortBy";
		this.memo_SortBy.SettingInVisible = false;
		this.memo_SortBy.SettingReadonly = false;
		this.memo_SortBy.Size = new System.Drawing.Size(595, 46);
		this.memo_SortBy.TabIndex = 7;
		this.memo_SortBy.Enter += new System.EventHandler(this.memo_SortBy_Enter);
		this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(10, 271);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(51, 13);
		this.label5.TabIndex = 3;
		this.label5.Text = "Group By";
		this.memo_GroupBy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_GroupBy.EditControl = null;
		this.memo_GroupBy.FieldName = null;
		this.memo_GroupBy.IsSearchControl = false;
		this.memo_GroupBy.Location = new System.Drawing.Point(10, 287);
		this.memo_GroupBy.Name = "memo_GroupBy";
		this.memo_GroupBy.SettingInVisible = false;
		this.memo_GroupBy.SettingReadonly = false;
		this.memo_GroupBy.Size = new System.Drawing.Size(595, 46);
		this.memo_GroupBy.TabIndex = 8;
		this.memo_GroupBy.Enter += new System.EventHandler(this.memo_GroupBy_Enter);
		this.bt_X.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_X.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.bt_X.Appearance.Options.UseFont = true;
		this.bt_X.EditControl = null;
		this.bt_X.EditValue = "X";
		this.bt_X.FieldName = null;
		this.bt_X.IsSearchControl = false;
		this.bt_X.Location = new System.Drawing.Point(0, 445);
		this.bt_X.Name = "bt_X";
		this.bt_X.ReadOnly = false;
		this.bt_X.SettingInVisible = false;
		this.bt_X.SettingReadonly = false;
		this.bt_X.Size = new System.Drawing.Size(38, 23);
		this.bt_X.TabIndex = 0;
		this.bt_X.Text = "X";
		this.bt_X.Click += new System.EventHandler(this.bt_X_Click);
		this.bt_Move.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Move.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.bt_Move.Appearance.Options.UseFont = true;
		this.bt_Move.EditControl = null;
		this.bt_Move.EditValue = "> >";
		this.bt_Move.FieldName = null;
		this.bt_Move.IsSearchControl = false;
		this.bt_Move.Location = new System.Drawing.Point(44, 445);
		this.bt_Move.Name = "bt_Move";
		this.bt_Move.ReadOnly = false;
		this.bt_Move.SettingInVisible = false;
		this.bt_Move.SettingReadonly = false;
		this.bt_Move.Size = new System.Drawing.Size(56, 23);
		this.bt_Move.TabIndex = 0;
		this.bt_Move.Text = "> >";
		this.btn_Execute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Execute.EditControl = null;
		this.btn_Execute.EditValue = "Execute";
		this.btn_Execute.FieldName = null;
		this.btn_Execute.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Execute.ImageOptions.Image");
		this.btn_Execute.IsSearchControl = false;
		this.btn_Execute.Location = new System.Drawing.Point(2, 482);
		this.btn_Execute.Name = "btn_Execute";
		this.btn_Execute.ReadOnly = false;
		this.btn_Execute.SettingInVisible = false;
		this.btn_Execute.SettingReadonly = false;
		this.btn_Execute.Size = new System.Drawing.Size(75, 20);
		this.btn_Execute.TabIndex = 9;
		this.btn_Execute.Text = "Execute";
		this.btn_Execute.Click += new System.EventHandler(this.bt_Execute_Click);
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(80, 482);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(75, 20);
		this.btn_Close.TabIndex = 10;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.btn_Conditions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Conditions.EditControl = null;
		this.btn_Conditions.EditValue = "Conditions";
		this.btn_Conditions.FieldName = null;
		this.btn_Conditions.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Conditions.ImageOptions.Image");
		this.btn_Conditions.IsSearchControl = false;
		this.btn_Conditions.Location = new System.Drawing.Point(879, 479);
		this.btn_Conditions.Name = "btn_Conditions";
		this.btn_Conditions.ReadOnly = false;
		this.btn_Conditions.SettingInVisible = false;
		this.btn_Conditions.SettingReadonly = false;
		this.btn_Conditions.Size = new System.Drawing.Size(79, 20);
		this.btn_Conditions.TabIndex = 11;
		this.btn_Conditions.Text = "Conditions";
		this.btn_Conditions.Click += new System.EventHandler(this.btn_Conditions_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_table);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Controls.Add(this.bt_X);
		this.splitContainerControl1.Panel1.Controls.Add(this.bt_Move);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(954, 471);
		this.splitContainerControl1.TabIndex = 12;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Line);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.label1);
		this.splitContainerControl2.Panel2.Controls.Add(this.label2);
		this.splitContainerControl2.Panel2.Controls.Add(this.label3);
		this.splitContainerControl2.Panel2.Controls.Add(this.label4);
		this.splitContainerControl2.Panel2.Controls.Add(this.memo_GroupBy);
		this.splitContainerControl2.Panel2.Controls.Add(this.label5);
		this.splitContainerControl2.Panel2.Controls.Add(this.memo_SortBy);
		this.splitContainerControl2.Panel2.Controls.Add(this.memo_Select);
		this.splitContainerControl2.Panel2.Controls.Add(this.memo_Where);
		this.splitContainerControl2.Panel2.Controls.Add(this.memo_From);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(844, 471);
		this.splitContainerControl2.SplitterPosition = 231;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(965, 506);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Conditions);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.btn_Execute);
		base.FormId = "oQueryGenerator";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_QueryGenerator";
		this.Text = "Query Generator";
		base.Load += new System.EventHandler(this.frm_QueryGenerator_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_table.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Select.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Where.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_SortBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_GroupBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
