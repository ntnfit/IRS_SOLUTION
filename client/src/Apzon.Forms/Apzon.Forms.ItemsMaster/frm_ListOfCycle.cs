using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_ListOfCycle : Form
{
	private IContainer components;

	private LabelLine lb_Find;

	private TextEditEx txt_Find;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx bt_Choose;

	private SimpleButtonEx bt_Cancel;

	private GridColumnEx gcName;

	private GridColumnEx gcFrequency;

	private GridColumnEx gcCode;

	private GridColumnEx gcDay;

	public frm_ListOfCycle()
	{
		InitializeComponent();
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_ListOfCycle_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			DataTable dataTable = await _help.GetDataTableAsync("GetShowOCYC");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_Doc.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_Choose_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_ListOfCycle));
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.txt_Find = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFrequency = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDay = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_Choose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.lb_Find.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Find";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(12, 12);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(116, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 0;
		this.lb_Find.Utext = "Find";
		this.txt_Find.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Find.EditControl = null;
		this.txt_Find.FieldName = null;
		this.txt_Find.IsSearchControl = false;
		this.txt_Find.Location = new System.Drawing.Point(127, 8);
		this.txt_Find.Name = "txt_Find";
		this.txt_Find.SettingInVisible = false;
		this.txt_Find.SettingReadonly = false;
		this.txt_Find.Size = new System.Drawing.Size(267, 20);
		this.txt_Find.TabIndex = 1;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(12, 34);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(506, 244);
		this.grd_Doc.TabIndex = 2;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcName, this.gcFrequency, this.gcCode, this.gcDay });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 25;
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
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = true;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 0;
		this.gcFrequency.Caption = "Frequency";
		this.gcFrequency.CurrencyDateField = null;
		this.gcFrequency.CurrencyField = null;
		this.gcFrequency.CurrencyFixed = null;
		this.gcFrequency.CurrencyFixedRate = 0.0;
		this.gcFrequency.CurrencyRateField = null;
		this.gcFrequency.FieldName = "Frequency";
		this.gcFrequency.Name = "gcFrequency";
		this.gcFrequency.PrimaryKey = false;
		this.gcFrequency.UnitField = null;
		this.gcFrequency.Visible = true;
		this.gcFrequency.VisibleIndex = 1;
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 2;
		this.gcDay.Caption = "Day";
		this.gcDay.CurrencyDateField = null;
		this.gcDay.CurrencyField = null;
		this.gcDay.CurrencyFixed = null;
		this.gcDay.CurrencyFixedRate = 0.0;
		this.gcDay.CurrencyRateField = null;
		this.gcDay.FieldName = "Day";
		this.gcDay.Name = "gcDay";
		this.gcDay.PrimaryKey = false;
		this.gcDay.UnitField = null;
		this.gcDay.Visible = true;
		this.gcDay.VisibleIndex = 3;
		this.bt_Choose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Choose.EditControl = null;
		this.bt_Choose.EditValue = "Choose";
		this.bt_Choose.FieldName = null;
		this.bt_Choose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Choose.ImageOptions.Image");
		this.bt_Choose.IsSearchControl = false;
		this.bt_Choose.Location = new System.Drawing.Point(12, 284);
		this.bt_Choose.Name = "bt_Choose";
		this.bt_Choose.ReadOnly = false;
		this.bt_Choose.SettingInVisible = false;
		this.bt_Choose.SettingReadonly = false;
		this.bt_Choose.Size = new System.Drawing.Size(75, 23);
		this.bt_Choose.TabIndex = 3;
		this.bt_Choose.Text = "Choose";
		this.bt_Choose.Click += new System.EventHandler(this.bt_Choose_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(91, 284);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 3;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(530, 313);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_Choose);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.txt_Find);
		base.Controls.Add(this.lb_Find);
		base.Name = "frm_ListOfCycle";
		base.ShowIcon = false;
		this.Text = "List of Inventory Cycle";
		base.Load += new System.EventHandler(this.frm_ListOfCycle_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
