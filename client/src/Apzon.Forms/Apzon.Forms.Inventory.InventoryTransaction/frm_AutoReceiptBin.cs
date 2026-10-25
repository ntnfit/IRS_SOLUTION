using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_AutoReceiptBin : Form
{
	private const string UdoCode = "OARB";

	public string UdoTableName = "@OARB";

	public DataTable UdoTable = new DataTable();

	public DataTable UdoTableCounting = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string BinLocation = "";

	private IContainer components;

	private BackgroundWorker backgroundWorker1;

	private SimpleButtonEx simpleButton2;

	private ButtonEditEx bte_BIN;

	private SimpleButtonEx btn_OK;

	private GridViewEx gv_Counting;

	private GridControlEx grd_Counting;

	private LabelLine labelLine4;

	private TextEditEx txt_Batch;

	private LabelLine BIN;

	private DateEditEx date_Doc;

	private LabelLine labelLine3;

	private TextEditEx txt_DocNo;

	private LabelLine labelLine1;

	public DateTime GetDate { get; set; }

	public frm_AutoReceiptBin()
	{
		InitializeComponent();
	}

	private async void frm_AutoReceiptBin_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OARB"
			});
			int num = await _httpClient.Udo.GetMaxId("OARB");
			txt_DocNo.Text = Function.ToString(num + 1);
			DateTime dateTime = await _httpClient.Commons.GetServerDate();
			date_Doc.EditValue = dateTime;
			GetDate = dateTime;
			ResetData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetData()
	{
		try
		{
			bte_BIN.Text = "";
			txt_Batch.Text = "";
			txt_Batch.Enabled = false;
			btn_OK.Text = Resources.ButtonOK;
			bte_BIN.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_AutoReceiptBin));
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_BIN = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gv_Counting = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Counting = new Apzon.Controls.Grid.GridControlEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Batch = new Apzon.Controls.Editor.TextEditEx();
		this.BIN = new Apzon.Controls.Controls.LabelLine();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		base.SuspendLayout();
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Clear";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(353, 35);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 20);
		this.simpleButton2.TabIndex = 41;
		this.simpleButton2.Text = "Clear";
		this.bte_BIN.EditControl = null;
		this.bte_BIN.FieldName = null;
		this.bte_BIN.IsSearchControl = false;
		this.bte_BIN.Location = new System.Drawing.Point(106, 10);
		this.bte_BIN.Name = "bte_BIN";
		this.bte_BIN.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BIN.SettingInVisible = false;
		this.bte_BIN.SettingReadonly = false;
		this.bte_BIN.Size = new System.Drawing.Size(241, 20);
		this.bte_BIN.TabIndex = 40;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 438);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 37;
		this.btn_OK.Text = "OK";
		this.gv_Counting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Counting.ColumnKey = null;
		this.gv_Counting.DisableFormSetting = false;
		this.gv_Counting.DisplayException = false;
		this.gv_Counting.EditColumn = null;
		this.gv_Counting.FieldCurrency = null;
		this.gv_Counting.FieldRate = null;
		this.gv_Counting.FormId = null;
		this.gv_Counting.GridControl = this.grd_Counting;
		this.gv_Counting.IgnoreFilter = false;
		this.gv_Counting.IndexEditRow = -1;
		this.gv_Counting.IndicatorWidth = 35;
		this.gv_Counting.IsApzTable = false;
		this.gv_Counting.IsInsertNewRow = false;
		this.gv_Counting.IsLoadingData = false;
		this.gv_Counting.IsMappingContextAction = false;
		this.gv_Counting.ItemId = null;
		this.gv_Counting.LstColumnAllowBatch = null;
		this.gv_Counting.LstColumnNotAllowBatch = null;
		this.gv_Counting.LstColumnNotEditable = null;
		this.gv_Counting.LstColumNotSetting = null;
		this.gv_Counting.Name = "gv_Counting";
		this.gv_Counting.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_Counting.OptionsSelection.MultiSelect = true;
		this.gv_Counting.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Counting.OptionsView.ShowGroupPanel = false;
		this.gv_Counting.UseBatchMod = false;
		this.grd_Counting.AllowPasteExcel = true;
		this.grd_Counting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Counting.ClipboardData = null;
		this.grd_Counting.Location = new System.Drawing.Point(0, 61);
		this.grd_Counting.MainView = this.gv_Counting;
		this.grd_Counting.Name = "grd_Counting";
		this.grd_Counting.Size = new System.Drawing.Size(876, 371);
		this.grd_Counting.TabIndex = 36;
		this.grd_Counting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Counting });
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Batch (Mã hàng)";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(1, 37);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(105, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 35;
		this.labelLine4.Utext = "Batch (Mã hàng)";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = false;
		this.txt_Batch.Location = new System.Drawing.Point(106, 35);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(241, 20);
		this.txt_Batch.TabIndex = 34;
		this.BIN.EditControl = null;
		this.BIN.EditValue = "BIN";
		this.BIN.FieldName = null;
		this.BIN.HideWhenEmpty = false;
		this.BIN.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.BIN.Location = new System.Drawing.Point(1, 11);
		this.BIN.Name = "BIN";
		this.BIN.ReadOnly = false;
		this.BIN.SettingInVisible = false;
		this.BIN.SettingReadonly = false;
		this.BIN.ShowUnderLine = true;
		this.BIN.Size = new System.Drawing.Size(105, 20);
		this.BIN.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.BIN.TabIndex = 33;
		this.BIN.Utext = "BIN";
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.Enabled = false;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(726, 36);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(142, 20);
		this.date_Doc.TabIndex = 32;
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(647, 39);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 31;
		this.labelLine3.Utext = "Doc Date";
		this.txt_DocNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(726, 11);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.Properties.ReadOnly = true;
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(142, 20);
		this.txt_DocNo.TabIndex = 28;
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(647, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 27;
		this.labelLine1.Utext = "Doc No.";
		base.AutoScaleDimensions = new System.Drawing.SizeF(5f, 9f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(877, 460);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.bte_BIN);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Counting);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.BIN);
		base.Controls.Add(this.date_Doc);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine1);
		this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "frm_AutoReceiptBin";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Auto Receipt Binlocation";
		base.Load += new System.EventHandler(this.frm_AutoReceiptBin_Load);
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
