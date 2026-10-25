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
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_AutoSelectBatchSerialConfig : XtraFormEx
{
	private IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lbOrderBy;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private SimpleButton btnCancel;

	private SimpleButton btnSave;

	private GridColumnEx gcColumnId;

	private RepositoryItemLookUpEdit rColumn;

	private GridColumnEx gcType;

	private RepositoryItemLookUpEdit rType;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			btnSave.Enabled = false;
			btnCancel.Enabled = true;
			return;
		}
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnSave.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnSave.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_AutoSelectBatchSerialConfig()
	{
		InitializeComponent();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_AutoSelectBatchSerialConfig_Load(object sender, EventArgs e)
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Code = "A",
				Name = "Ascending"
			});
			list.Add(new Common.Table_Code
			{
				Code = "D",
				Name = "decrease"
			});
			rType.DataSource = list;
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals(gcColumnId.FieldName))
			{
				if (grdDoc.DataSourceTable.AsEnumerable().Count((DataRow t) => Function.ToString(t["ColumnId"]).Equals(Function.ToString(e.Value))) > 1)
				{
					WinCommons.Warning(Enums.MessageCode.ColumnHaveBeenSelected, "");
					grdDoc.DataSourceTable.Rows.RemoveAt(gvDoc.GetFocusedDataSourceRowIndex());
					grdDoc.DataSourceTable.AcceptChanges();
					AddNewLine();
					return;
				}
				gvDoc.SetRowCellValue(e.RowHandle, gcType, "A");
			}
			AddNewLine();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.View:
				Close();
				break;
			case Enums.FormViewModeType.Update:
				await Save();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvDoc.DeleteDataSourceRowSelected();
			AddNewLine();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		try
		{
			WinCommons._OpenProcessing();
			btnSave.Enabled = false;
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DataIsEmpty, "");
				return;
			}
			EnumerableRowCollection<DataRow> source = from t in dataSourceTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ColumnId"]))
				select t;
			if (!source.Any())
			{
				WinCommons.Error(Enums.MessageCode.DataIsEmpty, "");
			}
			DataTable dt = source.CopyToDataTable();
			string text = await httpClient.BatchDetails.SaveConfig(dt);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btnSave.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async Task LoadData()
	{
		DataSet dataSet = await httpClient.BatchDetails.LoadDataAutoSelectConfig();
		if (dataSet == null || dataSet.Tables.Count != 2 || !dataSet.Tables[0].IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.CannotGetDataConfig, "");
			return;
		}
		rColumn.DataSource = dataSet.Tables[0];
		grdDoc.DataSource = dataSet.Tables[1];
		AddNewLine();
	}

	private void AddNewLine()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable();
			if (dataTable != null && dataTable.Columns.Count != 0 && (!dataTable.IsNotNull() || dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ColumnId"]))) == null))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["SortId"] = ((!dataTable.IsNotNull()) ? 1 : ((from t in dataTable.AsEnumerable()
					select Function.ParseInt(t["SortId"])).Max() + 1));
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_AutoSelectBatchSerialConfig_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_AutoSelectBatchSerialConfig));
		this.lbOrderBy = new Apzon.Controls.Controls.LabelLine();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcColumnId = new Apzon.Controls.Grid.GridColumnEx();
		this.rColumn = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnSave = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rColumn).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rType).BeginInit();
		base.SuspendLayout();
		this.lbOrderBy.FieldName = null;
		this.lbOrderBy.HideWhenEmpty = false;
		this.lbOrderBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOrderBy.Location = new System.Drawing.Point(11, 2);
		this.lbOrderBy.Name = "lbOrderBy";
		this.lbOrderBy.ShowUnderLine = true;
		this.lbOrderBy.Size = new System.Drawing.Size(80, 20);
		this.lbOrderBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOrderBy.TabIndex = 0;
		this.lbOrderBy.Utext = "Order By";
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(1, 28);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rType, this.rColumn });
		this.grdDoc.Size = new System.Drawing.Size(282, 239);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcColumnId, this.gcType });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oAutoSetectedBatchConfig";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "OCONF";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcColumnId.Caption = "Column id";
		this.gcColumnId.ColumnEdit = this.rColumn;
		this.gcColumnId.CurrencyDateField = null;
		this.gcColumnId.CurrencyField = null;
		this.gcColumnId.CurrencyFixed = null;
		this.gcColumnId.CurrencyFixedRate = 0.0;
		this.gcColumnId.CurrencyRateField = null;
		this.gcColumnId.FieldName = "ColumnId";
		this.gcColumnId.Name = "gcColumnId";
		this.gcColumnId.PrimaryKey = false;
		this.gcColumnId.UnitField = null;
		this.gcColumnId.Visible = true;
		this.gcColumnId.VisibleIndex = 0;
		this.gcColumnId.Width = 123;
		this.rColumn.AutoHeight = false;
		this.rColumn.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rColumn.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ColumnId", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ColumnName", "Name")
		});
		this.rColumn.DisplayMember = "ColumnName";
		this.rColumn.Name = "rColumn";
		this.rColumn.NullText = "";
		this.rColumn.ShowHeader = false;
		this.rColumn.ValueMember = "ColumnId";
		this.gcType.Caption = "Type";
		this.gcType.ColumnEdit = this.rType;
		this.gcType.CurrencyDateField = null;
		this.gcType.CurrencyField = null;
		this.gcType.CurrencyFixed = null;
		this.gcType.CurrencyFixedRate = 0.0;
		this.gcType.CurrencyRateField = null;
		this.gcType.FieldName = "Type";
		this.gcType.Name = "gcType";
		this.gcType.PrimaryKey = false;
		this.gcType.UnitField = null;
		this.gcType.Visible = true;
		this.gcType.VisibleIndex = 1;
		this.gcType.Width = 101;
		this.rType.AutoHeight = false;
		this.rType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rType.DisplayMember = "Name";
		this.rType.Name = "rType";
		this.rType.NullText = "";
		this.rType.ShowHeader = false;
		this.rType.ValueMember = "Code";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(208, 270);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(130, 270);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 3;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 295);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.lbOrderBy);
		base.FormId = "oAutoSetectedBatchConfig";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AutoSelectBatchSerialConfig";
		this.Text = "Automated selection setting for Batch and Serial Number";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AutoSelectBatchSerialConfig_FormClosing);
		base.Load += new System.EventHandler(this.frm_AutoSelectBatchSerialConfig_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rColumn).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rType).EndInit();
		base.ResumeLayout(false);
	}
}
