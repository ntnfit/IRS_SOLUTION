using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Financials;

public class frm_ListDitributeRule : XtraFormEx
{
	public DataTable dtFind = new DataTable();

	private string _Controller = "DistributeRule";

	public string keyFind = "";

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gcOcrCode;

	private GridColumnEx gcOcrName;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	public frm_ListDitributeRule()
	{
		InitializeComponent();
	}

	private void frm_ListDitributeRule_Load(object sender, EventArgs e)
	{
		LoadForm();
	}

	private async void LoadForm()
	{
		try
		{
			try
			{
				using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
				DataTable dataTable = await _hp.PostAsync("Search", dtFind);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					grd_Doc.DataSource = dataTable;
					return;
				}
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("OcrCode", typeof(string));
				dataTable2.Columns.Add("OcrName", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["OcrCode"] = string.Empty;
				dataRow["OcrName"] = string.Empty;
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		keyFind = Function.ToString(gv_Doc.GetFocusedRowCellValue("OcrCode"));
		Close();
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		keyFind = Function.ToString(gv_Doc.GetFocusedRowCellValue("OcrCode"));
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		keyFind = Function.ToString(gv_Doc.GetFocusedRowCellValue("OcrCode"));
		Close();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_ListDitributeRule));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcOcrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrName = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(1, 1);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(566, 282);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcOcrCode, this.gcOcrName });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsFind.AlwaysVisible = true;
		this.gv_Doc.OptionsFind.FindDelay = 100;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gcOcrCode.Caption = "OcrCode";
		this.gcOcrCode.CurrencyDateField = null;
		this.gcOcrCode.CurrencyField = null;
		this.gcOcrCode.CurrencyFixed = null;
		this.gcOcrCode.CurrencyFixedRate = 0.0;
		this.gcOcrCode.CurrencyRateField = null;
		this.gcOcrCode.FieldName = "OcrCode";
		this.gcOcrCode.Name = "gcOcrCode";
		this.gcOcrCode.PrimaryKey = true;
		this.gcOcrCode.UnitField = null;
		this.gcOcrCode.Visible = true;
		this.gcOcrCode.VisibleIndex = 0;
		this.gcOcrName.Caption = "OcrName";
		this.gcOcrName.CurrencyDateField = null;
		this.gcOcrName.CurrencyField = null;
		this.gcOcrName.CurrencyFixed = null;
		this.gcOcrName.CurrencyFixedRate = 0.0;
		this.gcOcrName.CurrencyRateField = null;
		this.gcOcrName.FieldName = "OcrName";
		this.gcOcrName.Name = "gcOcrName";
		this.gcOcrName.PrimaryKey = false;
		this.gcOcrName.UnitField = null;
		this.gcOcrName.Visible = true;
		this.gcOcrName.VisibleIndex = 1;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(82, 299);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 52;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(1, 299);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 51;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(567, 323);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListDitributeRule";
		this.Text = "List Ditribution Rules";
		base.Load += new System.EventHandler(this.frm_ListDitributeRule_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
