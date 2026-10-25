using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Tax;

public class frm_TaxDefinitionSetup : XtraFormEx
{
	public delegate void SaveDelegate(DataTable dt, string code);

	public string _CodeVat = string.Empty;

	private DataTable dtDoc = new DataTable();

	private string _privilege = "";

	private DataTable dt;

	private string code;

	private Dictionary<string, string> _lstTaxType = new Dictionary<string, string>
	{
		{ "V", "VAT" },
		{ "S", "Stamp" }
	};

	public SaveDelegate GetTable;

	private IContainer components;

	private GridControlEx grd_VTG1;

	private GridViewEx gv_VTG1;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private GridColumnEx gcEffecDate;

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx gcRate;

	private GridColumnEx gcCode;

	private GridColumnEx gcEquVatPr;

	private GridColumnEx gcMinAmount;

	private GridColumnEx gcFixedAmout;

	private GridColumnEx gcTaxType;

	private GridColumnEx gcLogInstanc;

	private RepositoryItemLookUpEdit rILUE_TaxType;

	private GridColumnEx gcDatevCode;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemCalcEdit rep_Rate;

	public new DataTable DataSource { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			bt_OK.Enabled = false;
			bt_Cancel.Enabled = true;
			return;
		}
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_TaxDefinitionSetup(DataTable dt, string code)
	{
		InitializeComponent();
		dtDoc = dt;
		this.code = code;
	}

	public frm_TaxDefinitionSetup()
	{
		InitializeComponent();
		dtDoc = dt;
	}

	private void GetDefaultTable()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("EffecDate", typeof(DateTime));
			dataTable.Columns.Add("Rate", typeof(double));
			dataTable.Columns.Add("EquVatPr", typeof(double));
			dataTable.Columns.Add("MinAmount", typeof(double));
			dataTable.Columns.Add("FixedAmout", typeof(double));
			dataTable.Columns.Add("TaxType", typeof(string));
			dataTable.Columns.Add("LogInstanc", typeof(int));
			dataTable.Columns.Add("DatevCode", typeof(int));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = _CodeVat;
			dataRow["TaxType"] = "V";
			dataRow["LogInstanc"] = 0;
			dataTable.Rows.Add(dataRow);
			grd_VTG1.DataSource = dataTable;
			grd_VTG1.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			if (grd_VTG1.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow[] array = dataTable.Select("EffecDate is null");
				if ((array != null && array.Length < 1) || gv_VTG1.FocusedRowHandle == gv_VTG1.DataRowCount - 1)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = _CodeVat;
					dataRow["TaxType"] = "V";
					dataRow["LogInstanc"] = 0;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_VTG1.DataSource = dataTable;
					grd_VTG1.RefreshDataSource();
				}
			}
			else
			{
				GetDefaultTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<bool> CheckVTG1()
	{
		try
		{
			string text = Function.ToString(gv_VTG1.GetFocusedRowCellValue("Code"));
			string effecDate = Function.ToString(gv_VTG1.GetFocusedRowCellValue("EffecDate"));
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
			DataTable dataTable = await hp.GetAsync("CheckVTG1", new
			{
				code = text,
				effecDate = effecDate
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void DeleteRowEmpty()
	{
		try
		{
			DataTable dataTable = (grd_VTG1.DataSource as DataTable).Copy();
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["EffecDate"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				dataTable = array.CopyToDataTable();
			}
			DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetTaxType()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstTaxType.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rILUE_TaxType.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Save()
	{
		try
		{
			DataTable dataTable = grd_VTG1.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable source = dataTable.Copy();
			DataRow[] array = (from t in source.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["EffecDate"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				source = array.CopyToDataTable();
				List<IGrouping<string, DataRow>> list = (from i in source.AsEnumerable()
					group i by Function.ToString(i.Field<DateTime>("EffecDate")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list != null && list.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableVTG1, "");
					return;
				}
				DataTable dataTable2 = dataTable.Clone();
				foreach (DataRow row in source.Rows)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["Code"] = _CodeVat;
					dataRow2["EffecDate"] = row["EffecDate"];
					dataRow2["Rate"] = row["Rate"];
					dataRow2["EquVatPr"] = row["EquVatPr"];
					dataRow2["MinAmount"] = row["MinAmount"];
					dataRow2["FixedAmout"] = row["FixedAmout"];
					dataRow2["DatevCode"] = row["DatevCode"];
					if (!string.IsNullOrEmpty(Function.ToString(row["TaxType"])))
					{
						dataRow2["TaxType"] = row["TaxType"];
					}
					else
					{
						dataRow2["TaxType"] = "V";
					}
					if (Function.ParseInt(row["LogInstanc"]) > 0)
					{
						dataRow2["LogInstanc"] = row["LogInstanc"];
					}
					else
					{
						dataRow2["LogInstanc"] = 0;
					}
					dataTable2.Rows.Add(dataRow2);
				}
				dataTable2.AcceptChanges();
				GetTable(dataTable2, code);
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.DefineEffectiveDate, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		frm_TaxDefinitionSetup_Load(null, null);
	}

	private async void grd_VTG1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_VTG1, gv_VTG1, new List<string> { "EffecDate" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else
		{
			Save();
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_VTG1.BeginUpdate();
			gv_VTG1.DeleteSelectedRows();
			gv_VTG1.EndUpdate();
			DataTable dataTable = (DataTable)grd_VTG1.DataSource;
			dataTable.AcceptChanges();
			grd_VTG1.DataSource = dataTable;
			grd_VTG1.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_VTG1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_TaxDefinitionSetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			DeleteRowEmpty();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_VTG1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_VTG1.DataSource as DataTable;
			if (dataTable.IsNotNull() && (e.Column.FieldName == "Rate" || e.Column.FieldName == "EffecDate"))
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_VTG1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_VTG1.DataSource as DataTable;
			if (dataTable.IsNotNull() && (e.Column.FieldName == "Rate" || e.Column.FieldName == "EffecDate"))
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_TaxDefinitionSetup_Load(object sender, EventArgs e)
	{
		try
		{
			GetTaxType();
			if (dtDoc.IsNotNull())
			{
				grd_VTG1.DataSource = dtDoc;
				grd_VTG1.RefreshDataSource();
				AddNewRow();
			}
			else
			{
				GetDefaultTable();
			}
			rep_Rate.Mask.MaskType = MaskType.Numeric;
			rep_Rate.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			rep_Rate.Mask.UseMaskAsDisplayFormat = true;
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_Date.DisplayFormat.FormatType = FormatType.DateTime;
			rep_Date.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Tax.frm_TaxDefinitionSetup));
		this.grd_VTG1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_VTG1 = new Apzon.Controls.Grid.GridViewEx();
		this.gcEffecDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcRate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Rate = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEquVatPr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMinAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFixedAmout = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxType = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_TaxType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDatevCode = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_VTG1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_VTG1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Rate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_TaxType).BeginInit();
		base.SuspendLayout();
		this.grd_VTG1.AllowPasteExcel = true;
		this.grd_VTG1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_VTG1.ClipboardData = null;
		this.grd_VTG1.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_VTG1.Location = new System.Drawing.Point(1, 1);
		this.grd_VTG1.MainView = this.gv_VTG1;
		this.grd_VTG1.Name = "grd_VTG1";
		this.grd_VTG1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_Date, this.rILUE_TaxType, this.rep_Rate });
		this.grd_VTG1.Size = new System.Drawing.Size(443, 228);
		this.grd_VTG1.TabIndex = 0;
		this.grd_VTG1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_VTG1 });
		this.grd_VTG1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_VTG1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_VTG1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_VTG1.ColumnKey = null;
		this.gv_VTG1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcEffecDate, this.gcRate, this.gcCode, this.gcEquVatPr, this.gcMinAmount, this.gcFixedAmout, this.gcTaxType, this.gcLogInstanc, this.gcDatevCode });
		this.gv_VTG1.DisableFormSetting = false;
		this.gv_VTG1.DisplayException = false;
		this.gv_VTG1.EditColumn = null;
		this.gv_VTG1.FieldCurrency = null;
		this.gv_VTG1.FieldRate = null;
		this.gv_VTG1.FormId = "oTaxDefinitionSetup";
		this.gv_VTG1.GridControl = this.grd_VTG1;
		this.gv_VTG1.IgnoreFilter = false;
		this.gv_VTG1.IndexEditRow = -1;
		this.gv_VTG1.IndicatorWidth = 35;
		this.gv_VTG1.IsApzTable = false;
		this.gv_VTG1.IsInsertNewRow = false;
		this.gv_VTG1.IsLoadingData = false;
		this.gv_VTG1.IsMappingContextAction = false;
		this.gv_VTG1.ItemId = "VTG1";
		this.gv_VTG1.LstColumnAllowBatch = null;
		this.gv_VTG1.LstColumnNotAllowBatch = null;
		this.gv_VTG1.LstColumnNotEditable = null;
		this.gv_VTG1.LstColumNotSetting = null;
		this.gv_VTG1.Name = "gv_VTG1";
		this.gv_VTG1.OptionsView.ShowGroupPanel = false;
		this.gv_VTG1.UseBatchMod = false;
		this.gv_VTG1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_VTG1_CustomDrawRowIndicator);
		this.gv_VTG1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_VTG1_CellValueChanged);
		this.gv_VTG1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_VTG1_CellValueChanging);
		this.gcEffecDate.Caption = "Effective From";
		this.gcEffecDate.ColumnEdit = this.rep_Date;
		this.gcEffecDate.CurrencyDateField = null;
		this.gcEffecDate.CurrencyField = null;
		this.gcEffecDate.CurrencyFixed = null;
		this.gcEffecDate.CurrencyFixedRate = 0.0;
		this.gcEffecDate.CurrencyRateField = null;
		this.gcEffecDate.FieldName = "EffecDate";
		this.gcEffecDate.Name = "gcEffecDate";
		this.gcEffecDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcEffecDate.PrimaryKey = false;
		this.gcEffecDate.UnitField = null;
		this.gcEffecDate.Visible = true;
		this.gcEffecDate.VisibleIndex = 0;
		this.gcEffecDate.Width = 405;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_Date.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_Date.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_Date.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_Date.Mask.EditMask = "dd/MM/yyyy";
		this.rep_Date.Name = "rep_Date";
		this.gcRate.Caption = "Rate";
		this.gcRate.ColumnEdit = this.rep_Rate;
		this.gcRate.CurrencyDateField = null;
		this.gcRate.CurrencyField = null;
		this.gcRate.CurrencyFixed = null;
		this.gcRate.CurrencyFixedRate = 0.0;
		this.gcRate.CurrencyRateField = null;
		this.gcRate.FieldName = "Rate";
		this.gcRate.Name = "gcRate";
		this.gcRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRate.PrimaryKey = false;
		this.gcRate.UnitField = null;
		this.gcRate.Visible = true;
		this.gcRate.VisibleIndex = 1;
		this.gcRate.Width = 307;
		this.rep_Rate.AutoHeight = false;
		this.rep_Rate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Rate.Name = "rep_Rate";
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcEquVatPr.Caption = "EquVatPr";
		this.gcEquVatPr.CurrencyDateField = null;
		this.gcEquVatPr.CurrencyField = null;
		this.gcEquVatPr.CurrencyFixed = null;
		this.gcEquVatPr.CurrencyFixedRate = 0.0;
		this.gcEquVatPr.CurrencyRateField = null;
		this.gcEquVatPr.FieldName = "EquVatPr";
		this.gcEquVatPr.Name = "gcEquVatPr";
		this.gcEquVatPr.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcEquVatPr.PrimaryKey = false;
		this.gcEquVatPr.UnitField = null;
		this.gcMinAmount.Caption = "MinAmount";
		this.gcMinAmount.CurrencyDateField = null;
		this.gcMinAmount.CurrencyField = null;
		this.gcMinAmount.CurrencyFixed = null;
		this.gcMinAmount.CurrencyFixedRate = 0.0;
		this.gcMinAmount.CurrencyRateField = null;
		this.gcMinAmount.FieldName = "MinAmount";
		this.gcMinAmount.Name = "gcMinAmount";
		this.gcMinAmount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcMinAmount.PrimaryKey = false;
		this.gcMinAmount.UnitField = null;
		this.gcFixedAmout.Caption = "FixedAmout";
		this.gcFixedAmout.CurrencyDateField = null;
		this.gcFixedAmout.CurrencyField = null;
		this.gcFixedAmout.CurrencyFixed = null;
		this.gcFixedAmout.CurrencyFixedRate = 0.0;
		this.gcFixedAmout.CurrencyRateField = null;
		this.gcFixedAmout.FieldName = "FixedAmout";
		this.gcFixedAmout.Name = "gcFixedAmout";
		this.gcFixedAmout.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFixedAmout.PrimaryKey = false;
		this.gcFixedAmout.UnitField = null;
		this.gcTaxType.Caption = "TaxType";
		this.gcTaxType.ColumnEdit = this.rILUE_TaxType;
		this.gcTaxType.CurrencyDateField = null;
		this.gcTaxType.CurrencyField = null;
		this.gcTaxType.CurrencyFixed = null;
		this.gcTaxType.CurrencyFixedRate = 0.0;
		this.gcTaxType.CurrencyRateField = null;
		this.gcTaxType.FieldName = "TaxType";
		this.gcTaxType.Name = "gcTaxType";
		this.gcTaxType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcTaxType.PrimaryKey = false;
		this.gcTaxType.UnitField = null;
		this.rILUE_TaxType.AutoHeight = false;
		this.rILUE_TaxType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_TaxType.DisplayMember = "Name";
		this.rILUE_TaxType.Name = "rILUE_TaxType";
		this.rILUE_TaxType.NullText = "";
		this.rILUE_TaxType.ValueMember = "Code";
		this.gcLogInstanc.Caption = "LogInstanc";
		this.gcLogInstanc.CurrencyDateField = null;
		this.gcLogInstanc.CurrencyField = null;
		this.gcLogInstanc.CurrencyFixed = null;
		this.gcLogInstanc.CurrencyFixedRate = 0.0;
		this.gcLogInstanc.CurrencyRateField = null;
		this.gcLogInstanc.FieldName = "LogInstanc";
		this.gcLogInstanc.Name = "gcLogInstanc";
		this.gcLogInstanc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLogInstanc.PrimaryKey = false;
		this.gcLogInstanc.UnitField = null;
		this.gcDatevCode.Caption = "DatevCode";
		this.gcDatevCode.CurrencyDateField = null;
		this.gcDatevCode.CurrencyField = null;
		this.gcDatevCode.CurrencyFixed = null;
		this.gcDatevCode.CurrencyFixedRate = 0.0;
		this.gcDatevCode.CurrencyRateField = null;
		this.gcDatevCode.FieldName = "DatevCode";
		this.gcDatevCode.Name = "gcDatevCode";
		this.gcDatevCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDatevCode.PrimaryKey = false;
		this.gcDatevCode.UnitField = null;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(1, 232);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(78, 232);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(445, 258);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_VTG1);
		base.FormId = "oTaxDefinitionSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TaxDefinitionSetup";
		this.Text = "TaxDefinition -Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_TaxDefinitionSetup_FormClosing);
		base.Load += new System.EventHandler(this.frm_TaxDefinitionSetup_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_VTG1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_VTG1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Rate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_TaxType).EndInit();
		base.ResumeLayout(false);
	}
}
