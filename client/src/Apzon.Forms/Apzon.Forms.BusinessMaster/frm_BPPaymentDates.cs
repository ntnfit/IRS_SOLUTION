using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Global;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.BusinessMaster;

public class frm_BPPaymentDates : XtraFormEx
{
	public DataTable _dtSource = new DataTable();

	public bool _isButtonOK;

	public bool _isOK = true;

	public string _cardCode = "";

	private IContainer components;

	private GridControlEx grd_Dates;

	private GridViewEx gv_Dates;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	public frm_BPPaymentDates()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			_dtSource = grd_Dates.DataSource as DataTable;
			_isButtonOK = true;
			_isOK = ValidateData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidateData()
	{
		if (grd_Dates.DataSource is DataTable dataTable && dataTable.Columns.Count == 3 && (from r in dataTable.AsEnumerable()
			group r by r["PmntDate"] into gr
			where gr.Count() > 1
			select gr).Any((IGrouping<object, DataRow> gr) => gr.Any((DataRow r) => !string.IsNullOrWhiteSpace(Function.ToString(r["PmntDate"])))))
		{
			WinCommons.Error(Enums.MessageCode.DuplicateDate, "");
			return false;
		}
		return true;
	}

	private void frm_BPPaymentDates_Load(object sender, EventArgs e)
	{
		try
		{
			grd_Dates.DataSource = _dtSource;
			grd_Dates.RefreshDataSource();
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		if (grd_Dates.DataSource is DataTable dataTable && dataTable.Columns.Count == 3 && !dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["PmntDate"]))))
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["CardCode"] = _cardCode;
			dataRow["PmntDate"] = "";
			dataRow["LogInstanc"] = GlobalData.UserID;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Dates.DataSource = dataTable;
			grd_Dates.RefreshDataSource();
		}
	}

	private void gv_Dates_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Dates_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Dates.DeleteSelectedRows();
			(grd_Dates.DataSource as DataTable).AcceptChanges();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_BPPaymentDates));
		this.grd_Dates = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Dates = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Dates).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Dates).BeginInit();
		base.SuspendLayout();
		this.grd_Dates.AllowPasteExcel = true;
		this.grd_Dates.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.grd_Dates.ClipboardData = null;
		this.grd_Dates.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Dates.Location = new System.Drawing.Point(7, 7);
		this.grd_Dates.MainView = this.gv_Dates;
		this.grd_Dates.Name = "grd_Dates";
		this.grd_Dates.Size = new System.Drawing.Size(277, 320);
		this.grd_Dates.TabIndex = 71;
		this.grd_Dates.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Dates });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.removeRowToolStripMenuItem.Text = "Remove row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Dates.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Dates.ColumnKey = null;
		this.gv_Dates.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gv_Dates.DisableFormSetting = false;
		this.gv_Dates.DisplayException = false;
		this.gv_Dates.EditColumn = null;
		this.gv_Dates.FieldCurrency = null;
		this.gv_Dates.FieldRate = null;
		this.gv_Dates.FormId = null;
		this.gv_Dates.GridControl = this.grd_Dates;
		this.gv_Dates.IgnoreFilter = false;
		this.gv_Dates.IndexEditRow = -1;
		this.gv_Dates.IndicatorWidth = 40;
		this.gv_Dates.IsApzTable = false;
		this.gv_Dates.IsInsertNewRow = false;
		this.gv_Dates.IsLoadingData = false;
		this.gv_Dates.IsMappingContextAction = false;
		this.gv_Dates.ItemId = null;
		this.gv_Dates.LstColumnAllowBatch = null;
		this.gv_Dates.LstColumnNotAllowBatch = null;
		this.gv_Dates.LstColumnNotEditable = null;
		this.gv_Dates.LstColumNotSetting = null;
		this.gv_Dates.Name = "gv_Dates";
		this.gv_Dates.OptionsCustomization.AllowSort = false;
		this.gv_Dates.OptionsView.ColumnAutoWidth = false;
		this.gv_Dates.OptionsView.ShowGroupPanel = false;
		this.gv_Dates.UseBatchMod = false;
		this.gv_Dates.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Dates_CustomDrawRowIndicator);
		this.gv_Dates.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Dates_CellValueChanged);
		this.gridColumn1.Caption = "CardCode";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CardCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "LogInstanc";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "LogInstanc";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Payment Dates";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "PmntDate";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn3.Width = 165;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 333);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(68, 20);
		this.btn_OK.TabIndex = 72;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(78, 333);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(68, 20);
		this.btn_Cancel.TabIndex = 73;
		this.btn_Cancel.Text = "Cancel";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(292, 357);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Dates);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.Name = "frm_BPPaymentDates";
		this.Text = "Payment Dates";
		base.Load += new System.EventHandler(this.frm_BPPaymentDates_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Dates).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Dates).EndInit();
		base.ResumeLayout(false);
	}
}
