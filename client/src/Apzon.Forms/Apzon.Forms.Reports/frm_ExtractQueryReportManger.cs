using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Reports;

public class frm_ExtractQueryReportManger : XtraFormEx
{
	public string StrSearch;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private TextEditEx txtSearch;

	private LabelLine labelLine1;

	private GridControlEx grd_ExtractQRM;

	private GridViewEx gv_ExtractQRM;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btnNewExtracrQRM;

	private GridColumnEx cl_Id;

	private GridColumnEx cl_Category;

	private GridColumnEx cl_Description;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_QueryId;

	private GridColumnEx cl_Qname;

	private GridColumnEx cl_Qtype;

	private GridColumnEx cl_Qstring;

	private ContextMenuStrip conMenu_Remove;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private RepositoryItemLookUpEdit lk_Qtype;

	public frm_ExtractQueryReportManger()
	{
		InitializeComponent();
	}

	private async void btnNewExtracrQRM_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			new frm_ExtractQueryReport().Show();
			WinCommons._CloseProcessing();
			await refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			string id = Function.ToString(gv_ExtractQRM.GetFocusedRowCellValue("Id"));
			if (XtraMessageBox.Show("Are you sure want to remove?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveExQR(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task refresh()
	{
		try
		{
			GridControlEx gridControlEx = grd_ExtractQRM;
			gridControlEx.DataSource = await _httpClient.Queries.ListOfQueryReport();
			grd_ExtractQRM.DataSourceTable.AcceptChanges();
			grd_ExtractQRM.RefreshDataSource();
			btn_Save.Text = Resources.ButtonOK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_ExtractQueryReportManger_Load(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			SetDataType();
			await refresh();
			if (!string.IsNullOrEmpty(StrSearch))
			{
				txtSearch.Text = StrSearch;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtSearch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			foreach (GridColumn column in gv_ExtractQRM.Columns)
			{
				if (column.Visible)
				{
					text += $"OR {column.FieldName} LIKE '%{txtSearch.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_ExtractQRM.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveExQR(string Id)
	{
		_ = 1;
		try
		{
			string text = await _httpClient.Queries.RemoveExtractQR(Id);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_ExtractQRM.BeginUpdate();
				gv_ExtractQRM.DeleteSelectedRows();
				gv_ExtractQRM.EndUpdate();
				await refresh();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Text",
			Code = "T"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Store procedure",
			Code = "S"
		});
		lk_Qtype.DataSource = list;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ExtractQueryReportManger));
		this.txtSearch = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_ExtractQRM = new Apzon.Controls.Grid.GridControlEx();
		this.conMenu_Remove = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ExtractQRM = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Category = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_QueryId = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Qname = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Qtype = new Apzon.Controls.Grid.GridColumnEx();
		this.lk_Qtype = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Qstring = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnNewExtracrQRM = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ExtractQRM).BeginInit();
		this.conMenu_Remove.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ExtractQRM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Qtype).BeginInit();
		base.SuspendLayout();
		this.txtSearch.EditControl = null;
		this.txtSearch.FieldName = null;
		this.txtSearch.IsSearchControl = false;
		this.txtSearch.Location = new System.Drawing.Point(100, 13);
		this.txtSearch.Name = "txtSearch";
		this.txtSearch.SettingInVisible = false;
		this.txtSearch.SettingReadonly = false;
		this.txtSearch.Size = new System.Drawing.Size(430, 20);
		this.txtSearch.TabIndex = 0;
		this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Search";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(87, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Search";
		this.grd_ExtractQRM.AllowPasteExcel = true;
		this.grd_ExtractQRM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ExtractQRM.ClipboardData = null;
		this.grd_ExtractQRM.ContextMenuStrip = this.conMenu_Remove;
		this.grd_ExtractQRM.Location = new System.Drawing.Point(2, 42);
		this.grd_ExtractQRM.MainView = this.gv_ExtractQRM;
		this.grd_ExtractQRM.Name = "grd_ExtractQRM";
		this.grd_ExtractQRM.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.lk_Qtype });
		this.grd_ExtractQRM.Size = new System.Drawing.Size(800, 400);
		this.grd_ExtractQRM.TabIndex = 2;
		this.grd_ExtractQRM.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ExtractQRM });
		this.conMenu_Remove.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.conMenu_Remove.Name = "conMenu_Remove";
		this.conMenu_Remove.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_ExtractQRM.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ExtractQRM.ColumnKey = null;
		this.gv_ExtractQRM.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.cl_Id, this.cl_Category, this.cl_Description, this.cl_Name, this.cl_QueryId, this.cl_Qname, this.cl_Qtype, this.cl_Qstring });
		this.gv_ExtractQRM.DisableFormSetting = false;
		this.gv_ExtractQRM.DisplayException = false;
		this.gv_ExtractQRM.EditColumn = null;
		this.gv_ExtractQRM.FieldCurrency = null;
		this.gv_ExtractQRM.FieldRate = null;
		this.gv_ExtractQRM.FormId = null;
		this.gv_ExtractQRM.GridControl = this.grd_ExtractQRM;
		this.gv_ExtractQRM.GroupCount = 1;
		this.gv_ExtractQRM.IgnoreFilter = false;
		this.gv_ExtractQRM.IndexEditRow = -1;
		this.gv_ExtractQRM.IndicatorWidth = 35;
		this.gv_ExtractQRM.IsApzTable = false;
		this.gv_ExtractQRM.IsInsertNewRow = false;
		this.gv_ExtractQRM.IsLoadingData = false;
		this.gv_ExtractQRM.IsMappingContextAction = false;
		this.gv_ExtractQRM.ItemId = null;
		this.gv_ExtractQRM.LstColumnAllowBatch = null;
		this.gv_ExtractQRM.LstColumnNotAllowBatch = null;
		this.gv_ExtractQRM.LstColumnNotEditable = null;
		this.gv_ExtractQRM.LstColumNotSetting = null;
		this.gv_ExtractQRM.Name = "gv_ExtractQRM";
		this.gv_ExtractQRM.OptionsView.ColumnAutoWidth = false;
		this.gv_ExtractQRM.OptionsView.RowAutoHeight = true;
		this.gv_ExtractQRM.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_ExtractQRM.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.cl_Category, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_ExtractQRM.UseBatchMod = false;
		this.cl_Id.Caption = "Id";
		this.cl_Id.CurrencyDateField = null;
		this.cl_Id.CurrencyField = null;
		this.cl_Id.CurrencyFixed = null;
		this.cl_Id.CurrencyFixedRate = 0.0;
		this.cl_Id.CurrencyRateField = null;
		this.cl_Id.FieldName = "Id";
		this.cl_Id.Name = "cl_Id";
		this.cl_Id.OptionsColumn.ReadOnly = true;
		this.cl_Id.PrimaryKey = false;
		this.cl_Id.UnitField = null;
		this.cl_Id.Visible = true;
		this.cl_Id.VisibleIndex = 0;
		this.cl_Id.Width = 81;
		this.cl_Category.Caption = "Category";
		this.cl_Category.CurrencyDateField = null;
		this.cl_Category.CurrencyField = null;
		this.cl_Category.CurrencyFixed = null;
		this.cl_Category.CurrencyFixedRate = 0.0;
		this.cl_Category.CurrencyRateField = null;
		this.cl_Category.FieldName = "Category";
		this.cl_Category.Name = "cl_Category";
		this.cl_Category.OptionsColumn.ReadOnly = true;
		this.cl_Category.PrimaryKey = false;
		this.cl_Category.UnitField = null;
		this.cl_Category.Visible = true;
		this.cl_Category.VisibleIndex = 1;
		this.cl_Description.Caption = "Description";
		this.cl_Description.CurrencyDateField = null;
		this.cl_Description.CurrencyField = null;
		this.cl_Description.CurrencyFixed = null;
		this.cl_Description.CurrencyFixedRate = 0.0;
		this.cl_Description.CurrencyRateField = null;
		this.cl_Description.FieldName = "Dscription";
		this.cl_Description.Name = "cl_Description";
		this.cl_Description.OptionsColumn.ReadOnly = true;
		this.cl_Description.PrimaryKey = false;
		this.cl_Description.UnitField = null;
		this.cl_Description.Visible = true;
		this.cl_Description.VisibleIndex = 5;
		this.cl_Description.Width = 199;
		this.cl_Name.Caption = "Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.OptionsColumn.ReadOnly = true;
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 4;
		this.cl_Name.Width = 153;
		this.cl_QueryId.Caption = "QueryId";
		this.cl_QueryId.CurrencyDateField = null;
		this.cl_QueryId.CurrencyField = null;
		this.cl_QueryId.CurrencyFixed = null;
		this.cl_QueryId.CurrencyFixedRate = 0.0;
		this.cl_QueryId.CurrencyRateField = null;
		this.cl_QueryId.FieldName = "QueryId";
		this.cl_QueryId.Name = "cl_QueryId";
		this.cl_QueryId.PrimaryKey = false;
		this.cl_QueryId.UnitField = null;
		this.cl_Qname.Caption = "Query Name";
		this.cl_Qname.CurrencyDateField = null;
		this.cl_Qname.CurrencyField = null;
		this.cl_Qname.CurrencyFixed = null;
		this.cl_Qname.CurrencyFixedRate = 0.0;
		this.cl_Qname.CurrencyRateField = null;
		this.cl_Qname.FieldName = "Qname";
		this.cl_Qname.Name = "cl_Qname";
		this.cl_Qname.OptionsColumn.ReadOnly = true;
		this.cl_Qname.PrimaryKey = false;
		this.cl_Qname.UnitField = null;
		this.cl_Qname.Visible = true;
		this.cl_Qname.VisibleIndex = 1;
		this.cl_Qname.Width = 194;
		this.cl_Qtype.Caption = "Query Type";
		this.cl_Qtype.ColumnEdit = this.lk_Qtype;
		this.cl_Qtype.CurrencyDateField = null;
		this.cl_Qtype.CurrencyField = null;
		this.cl_Qtype.CurrencyFixed = null;
		this.cl_Qtype.CurrencyFixedRate = 0.0;
		this.cl_Qtype.CurrencyRateField = null;
		this.cl_Qtype.FieldName = "Qtype";
		this.cl_Qtype.Name = "cl_Qtype";
		this.cl_Qtype.OptionsColumn.ReadOnly = true;
		this.cl_Qtype.PrimaryKey = false;
		this.cl_Qtype.UnitField = null;
		this.cl_Qtype.Visible = true;
		this.cl_Qtype.VisibleIndex = 2;
		this.lk_Qtype.AutoHeight = false;
		this.lk_Qtype.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Qtype.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_Qtype.DisplayMember = "Name";
		this.lk_Qtype.Name = "lk_Qtype";
		this.lk_Qtype.ValueMember = "Code";
		this.cl_Qstring.Caption = "Query String";
		this.cl_Qstring.CurrencyDateField = null;
		this.cl_Qstring.CurrencyField = null;
		this.cl_Qstring.CurrencyFixed = null;
		this.cl_Qstring.CurrencyFixedRate = 0.0;
		this.cl_Qstring.CurrencyRateField = null;
		this.cl_Qstring.FieldName = "Qstring";
		this.cl_Qstring.Name = "cl_Qstring";
		this.cl_Qstring.OptionsColumn.ReadOnly = true;
		this.cl_Qstring.PrimaryKey = false;
		this.cl_Qstring.UnitField = null;
		this.cl_Qstring.Visible = true;
		this.cl_Qstring.VisibleIndex = 3;
		this.cl_Qstring.Width = 78;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 448);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 3;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnNewExtracrQRM.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnNewExtracrQRM.EditControl = null;
		this.btnNewExtracrQRM.EditValue = "New";
		this.btnNewExtracrQRM.FieldName = null;
		this.btnNewExtracrQRM.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnNewExtracrQRM.ImageOptions.Image");
		this.btnNewExtracrQRM.IsSearchControl = false;
		this.btnNewExtracrQRM.Location = new System.Drawing.Point(727, 448);
		this.btnNewExtracrQRM.Name = "btnNewExtracrQRM";
		this.btnNewExtracrQRM.ReadOnly = false;
		this.btnNewExtracrQRM.SettingInVisible = false;
		this.btnNewExtracrQRM.SettingReadonly = false;
		this.btnNewExtracrQRM.Size = new System.Drawing.Size(75, 23);
		this.btnNewExtracrQRM.TabIndex = 4;
		this.btnNewExtracrQRM.Text = "New";
		this.btnNewExtracrQRM.Click += new System.EventHandler(this.btnNewExtracrQRM_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(804, 477);
		base.Controls.Add(this.btnNewExtracrQRM);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_ExtractQRM);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txtSearch);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ExtractQueryReportManger.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ExtractQueryReportManger";
		base.ShowInTaskbar = false;
		this.Text = "Extract Query Report Manger";
		base.Load += new System.EventHandler(this.frm_ExtractQueryReportManger_Load);
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ExtractQRM).EndInit();
		this.conMenu_Remove.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ExtractQRM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Qtype).EndInit();
		base.ResumeLayout(false);
	}
}
