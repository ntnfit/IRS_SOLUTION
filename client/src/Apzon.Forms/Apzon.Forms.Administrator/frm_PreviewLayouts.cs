using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator;

public class frm_PreviewLayouts : XtraFormEx
{
	public List<PrintLayout> Source = new List<PrintLayout>();

	private IContainer components;

	private GridControlEx grd_Layout;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridViewEx gv_Layout;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx simpleButton2;

	private GridColumnEx cl_XtraText;

	public PrintLayout ChoosedLayout { get; set; }

	public frm_PreviewLayouts()
	{
		InitializeComponent();
	}

	private void frm_PreviewLayouts_Load(object sender, EventArgs e)
	{
		try
		{
			if (Source != null && Source.Count > 0)
			{
				grd_Layout.DataSource = Source;
				grd_Layout.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			ChoosedLayout = (PrintLayout)gv_Layout.GetFocusedRow();
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Layout_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			ChoosedLayout = (PrintLayout)gv_Layout.GetFocusedRow();
			Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_PreviewLayouts));
		this.grd_Layout = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Layout = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_XtraText = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Layout).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Layout).BeginInit();
		base.SuspendLayout();
		this.grd_Layout.AllowPasteExcel = true;
		this.grd_Layout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Layout.ClipboardData = null;
		this.grd_Layout.Location = new System.Drawing.Point(1, 2);
		this.grd_Layout.MainView = this.gv_Layout;
		this.grd_Layout.Name = "grd_Layout";
		this.grd_Layout.Size = new System.Drawing.Size(457, 304);
		this.grd_Layout.TabIndex = 0;
		this.grd_Layout.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Layout });
		this.gv_Layout.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Layout.ColumnKey = null;
		this.gv_Layout.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.cl_XtraText });
		this.gv_Layout.DisableFormSetting = false;
		this.gv_Layout.DisplayException = false;
		this.gv_Layout.EditColumn = null;
		this.gv_Layout.FieldCurrency = null;
		this.gv_Layout.FieldRate = null;
		this.gv_Layout.FormId = "oPreviewLayout";
		this.gv_Layout.GridControl = this.grd_Layout;
		this.gv_Layout.IgnoreFilter = false;
		this.gv_Layout.IndexEditRow = -1;
		this.gv_Layout.IndicatorWidth = 35;
		this.gv_Layout.IsApzTable = false;
		this.gv_Layout.IsInsertNewRow = false;
		this.gv_Layout.IsLoadingData = false;
		this.gv_Layout.IsMappingContextAction = false;
		this.gv_Layout.ItemId = "OPRV";
		this.gv_Layout.LstColumnAllowBatch = null;
		this.gv_Layout.LstColumnNotAllowBatch = null;
		this.gv_Layout.LstColumnNotEditable = null;
		this.gv_Layout.LstColumNotSetting = null;
		this.gv_Layout.Name = "gv_Layout";
		this.gv_Layout.OptionsView.AnimationType = DevExpress.XtraGrid.Views.Base.GridAnimationType.AnimateFocusedItem;
		this.gv_Layout.OptionsView.ShowGroupPanel = false;
		this.gv_Layout.UseBatchMod = false;
		this.gv_Layout.DoubleClick += new System.EventHandler(this.gv_Layout_DoubleClick);
		this.gridColumn1.Caption = "TypeCode";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "TypeCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "TypeName";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "TypeName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "DocCode";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "DocCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn4.Caption = "Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DocName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 0;
		this.gridColumn4.Width = 300;
		this.gridColumn5.Caption = "FileName";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "FileName";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.cl_XtraText.Caption = "XtraText";
		this.cl_XtraText.CurrencyDateField = null;
		this.cl_XtraText.CurrencyField = null;
		this.cl_XtraText.CurrencyFixed = null;
		this.cl_XtraText.CurrencyFixedRate = 0.0;
		this.cl_XtraText.CurrencyRateField = null;
		this.cl_XtraText.FieldName = "XtraText";
		this.cl_XtraText.Name = "cl_XtraText";
		this.cl_XtraText.PrimaryKey = false;
		this.cl_XtraText.UnitField = null;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 310);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.simpleButton2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Cancel";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(83, 310);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 20);
		this.simpleButton2.TabIndex = 2;
		this.simpleButton2.Text = "Cancel";
		this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(460, 335);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Layout);
		base.FormId = "oPreviewLayout";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PreviewLayouts";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Preview layouts";
		base.Load += new System.EventHandler(this.frm_PreviewLayouts_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Layout).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Layout).EndInit();
		base.ResumeLayout(false);
	}
}
