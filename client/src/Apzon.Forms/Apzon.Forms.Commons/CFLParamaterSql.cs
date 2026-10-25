using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Commons;

public class CFLParamaterSql : XtraFormEx
{
	public DataRow Value;

	public List<DataRow> ChooseItems;

	public DataTable Dt;

	public bool MutilSelect;

	private IContainer components;

	private PanelControl panelControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx Name_;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private LabelLine lbSearch;

	private TextEditEx txtSearch;

	public CFLParamaterSql()
	{
		InitializeComponent();
	}

	private void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (MutilSelect)
			{
				ChooseItems = new List<DataRow>();
				int[] selectedRows = gv_Doc.GetSelectedRows();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					ChooseItems.Add(gv_Doc.GetDataRow(selectedRows[i]));
				}
			}
			else
			{
				Value = gv_Doc.GetFocusedDataRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		Close();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void grd_Doc_Load(object sender, EventArgs e)
	{
		if (MutilSelect)
		{
			gv_Doc.OptionsSelection.MultiSelect = true;
			gv_Doc.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
		}
		else
		{
			gv_Doc.OptionsSelection.MultiSelect = false;
		}
		grd_Doc.DataSource = Dt;
		grd_Doc.RefreshDataSource();
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			Value = gv_Doc.GetFocusedDataRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		Close();
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void txtSearch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			foreach (GridColumn column in gv_Doc.Columns)
			{
				if (column.Visible)
				{
					text += $"OR {column.FieldName} LIKE '%{txtSearch.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_Doc.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.CFLParamaterSql));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txtSearch = new Apzon.Controls.Editor.TextEditEx();
		this.lbSearch = new Apzon.Controls.Controls.LabelLine();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.Name_ = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.txtSearch);
		this.panelControl1.Controls.Add(this.lbSearch);
		this.panelControl1.Controls.Add(this.grd_Doc);
		this.panelControl1.Location = new System.Drawing.Point(5, 3);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(362, 218);
		this.panelControl1.TabIndex = 0;
		this.txtSearch.FieldName = null;
		this.txtSearch.IsSearchControl = false;
		this.txtSearch.Location = new System.Drawing.Point(91, 6);
		this.txtSearch.Name = "txtSearch";
		this.txtSearch.Size = new System.Drawing.Size(176, 20);
		this.txtSearch.TabIndex = 2;
		this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
		this.lbSearch.FieldName = null;
		this.lbSearch.HideWhenEmpty = false;
		this.lbSearch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSearch.Location = new System.Drawing.Point(8, 9);
		this.lbSearch.Name = "lbSearch";
		this.lbSearch.ShowUnderLine = true;
		this.lbSearch.Size = new System.Drawing.Size(80, 20);
		this.lbSearch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSearch.TabIndex = 1;
		this.lbSearch.Utext = "Search";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 35);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(362, 183);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.Load += new System.EventHandler(this.grd_Doc_Load);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.Name_ });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oParamaterQuery";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "ODATA";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.Name_.Caption = "Value";
		this.Name_.CurrencyDateField = null;
		this.Name_.CurrencyField = null;
		this.Name_.CurrencyFixed = null;
		this.Name_.CurrencyFixedRate = 0.0;
		this.Name_.CurrencyRateField = null;
		this.Name_.FieldName = "Value";
		this.Name_.Name = "Name_";
		this.Name_.PrimaryKey = false;
		this.Name_.UnitField = null;
		this.Name_.Visible = true;
		this.Name_.VisibleIndex = 0;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(206, 231);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(284, 231);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(367, 262);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oParamaterQuery";
		base.IconOptions.ShowIcon = false;
		base.Name = "CFLParamaterSql";
		this.Text = "Choose item";
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
