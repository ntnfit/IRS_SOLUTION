using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.UserControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.RoutingAndResource;

public class frm_AppointmentEditting : XtraFormEx
{
	public DataTable DataAppointment;

	private IContainer components;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcResCode;

	private GridColumnEx gcResName;

	private GridColumnEx gcOpeCode;

	private GridColumnEx gcOpeName;

	private GridColumnEx gcQty;

	private SimpleButton btnCancel;

	private SimpleButton btnOk;

	private RepositoryItemButtonEditCFL rResCode;

	private RepositoryItemButtonEditCFL rResName;

	private GridColumnEx gcWorkDay;

	private RepositoryItemCalcEdit rCal;

	private RepositoryItemDateEditEx rDate;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcDocItemCode;

	private GridColumnEx gcDocItemName;

	private GridColumnEx gcDocResCode;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL1;

	private GridColumnEx gcDocResName;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2;

	private GridColumnEx gcDocOpeCode;

	private GridColumnEx gcDocOpeName;

	private GridColumnEx gcDocQty;

	private RepositoryItemCalcEdit rDocCal;

	private GridColumnEx gcDocWorkDay;

	private RepositoryItemDateEditEx rDocDate;

	private ContextMenuStrip contextMenuStripLine;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private GridColumnEx gcStartTime;

	private GridColumnEx gcEndTime;

	private GridColumnEx gcEndDate;

	public frm_AppointmentEditting(DataTable dt)
	{
		if (!dt.IsNotNull())
		{
			throw new Exception("paramater dt can not be null");
		}
		DataAppointment = dt;
		InitializeComponent();
	}

	private void frm_AppointmentEditting_Load(object sender, EventArgs e)
	{
		try
		{
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rResCode;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvLine.SetFocusedRowCellValue(gcResCode, chooseItem["ResCode"]);
					grdLine.DataSourceTable.AcceptChanges();
					grdLine.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rResName;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvLine.SetFocusedRowCellValue(gcResCode, chooseItem["ResCode"]);
					grdLine.DataSourceTable.AcceptChanges();
					grdLine.Refresh();
				}
			}));
			grdDoc.DataSource = DataAppointment.Copy();
			grdLine.DataSource = DataAppointment;
			grdLine.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				List<DataRow> listDataSourceSelectedRow = gvLine.GetListDataSourceSelectedRow();
				DataTable dataSourceTable = grdLine.DataSourceTable;
				for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
				{
					dataSourceTable.Rows.Remove(listDataSourceSelectedRow[i]);
				}
				grdLine.DataSource = dataSourceTable;
				grdLine.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdLine.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow.ItemArray = gvDoc.GetFocusedDataRow().ItemArray;
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grdLine.DataSource = dataSourceTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		DataAppointment = grdLine.DataSourceTable;
		DialogResult = DialogResult.OK;
		Close();
	}

	private void gvLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataRow dataRow = gvLine.GetDataRow(e.RowHandle);
			if (e.Column.FieldName.Equals(gcWorkDay.FieldName))
			{
				int days = (Function.ParseDateTimes(e.Value).Date - Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcDocWorkDay)).Date).Days;
				dataRow["StartDate"] = Function.ParseDateTimes(dataRow["StartDate"]).AddDays(days);
				dataRow["EndDate"] = Function.ParseDateTimes(dataRow["EndDate"]).AddDays(days);
				dataRow["EndWorkDay"] = Function.ParseDateTimes(dataRow["EndDate"]).Date;
			}
			else if (e.Column.FieldName.Equals(gcStartTime.FieldName))
			{
				int num = Function.ParseInt(e.Value);
				dataRow["StartDate"] = Function.ParseDateTimes(dataRow["WorkDay"]).AddHours(num / 100).AddMinutes(num % 100);
				dataRow["EndDate"] = Function.ParseDateTimes(dataRow["StartDate"]).AddMinutes(Function.ParseDouble(dataRow["TimeRun"]));
				dataRow["EndWorkDay"] = Function.ParseDateTimes(dataRow["EndDate"]).Date;
				dataRow["EndTime"] = Function.ParseDateTimes(dataRow["EndDate"]).Hour * 100 + Function.ParseDateTimes(dataRow["EndDate"]).Minute;
			}
			else if (e.Column.FieldName.Equals(gcQty.FieldName))
			{
				dataRow["EndDate"] = Function.ParseDateTimes(dataRow["StartDate"]).AddMinutes(Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TimeRun")) * Function.ParseDouble(e.Value) / Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Qty")));
				DateTime dateTime = Function.ParseDateTimes(dataRow["EndDate"]);
				dataRow["EndWorkDay"] = dateTime.Date;
				dataRow["EndTime"] = dateTime.Hour * 100 + dateTime.Minute;
				dataRow["TimeRun"] = Function.ParseDouble(e.Value) * Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TimeRun")) / Function.ParseDouble(gvDoc.GetFocusedRowCellValue(gcDocQty));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gvLine.RefreshData();
		grdLine.DataSourceTable.AcceptChanges();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_AppointmentEditting));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rResCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcResName = new Apzon.Controls.Grid.GridColumnEx();
		this.rResName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcOpeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWorkDay = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gcStartTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEndTime = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocOpeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocOpeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDocWorkDay = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.repositoryItemButtonEditCFL1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.repositoryItemButtonEditCFL2 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		this.contextMenuStripLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rResName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL2).BeginInit();
		base.SuspendLayout();
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.ClipboardData = null;
		this.grdLine.ContextMenuStrip = this.contextMenuStripLine;
		this.grdLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLine.Location = new System.Drawing.Point(0, 0);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rResCode, this.rResName, this.rCal, this.rDate });
		this.grdLine.Size = new System.Drawing.Size(825, 169);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.contextMenuStripLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.insertRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStripLine.Name = "contextMenuStripLine";
		this.contextMenuStripLine.Size = new System.Drawing.Size(134, 48);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.insertRowToolStripMenuItem.Text = "New Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvLine.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvLine.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gcItemCode, this.gcItemName, this.gcResCode, this.gcResName, this.gcOpeCode, this.gcOpeName, this.gcQty, this.gcWorkDay, this.gcStartTime, this.gcEndDate,
			this.gcEndTime
		});
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "AppointmentEditting";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 35;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = "DOC";
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvLine.OptionsPrint.AutoWidth = false;
		this.gvLine.OptionsView.BestFitMaxRowCount = 100;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanged);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.ReadOnly = true;
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 90;
		this.gcItemName.Caption = "Item Name";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 102;
		this.gcResCode.Caption = "Resource Code";
		this.gcResCode.ColumnEdit = this.rResCode;
		this.gcResCode.CurrencyDateField = null;
		this.gcResCode.CurrencyField = null;
		this.gcResCode.CurrencyFixed = null;
		this.gcResCode.CurrencyFixedRate = 0.0;
		this.gcResCode.CurrencyRateField = null;
		this.gcResCode.FieldName = "ResCode";
		this.gcResCode.Name = "gcResCode";
		this.gcResCode.PrimaryKey = false;
		this.gcResCode.UnitField = null;
		this.gcResCode.Visible = true;
		this.gcResCode.VisibleIndex = 2;
		this.gcResCode.Width = 106;
		this.rResCode.AutoHeight = false;
		this.rResCode.AutoLoadDataSource = true;
		this.rResCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rResCode.ContainerControl = this;
		this.rResCode.DisplayMember = "";
		this.rResCode.DisplayMember2 = "";
		this.rResCode.LoadDataSourceWhenEnter = false;
		this.rResCode.Name = "rResCode";
		this.rResCode.SeparateCharacterForMulti = ';';
		this.rResCode.SeparateString = ", ";
		this.rResCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.rResCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.rResCode.UsingAutoCompleteBox = false;
		this.rResCode.UsingDataSource = true;
		this.rResCode.ValidateWhenNoDataSource = false;
		this.rResCode.ValueMember = "ResCode";
		this.gcResName.Caption = "Resource Name";
		this.gcResName.ColumnEdit = this.rResName;
		this.gcResName.CurrencyDateField = null;
		this.gcResName.CurrencyField = null;
		this.gcResName.CurrencyFixed = null;
		this.gcResName.CurrencyFixedRate = 0.0;
		this.gcResName.CurrencyRateField = null;
		this.gcResName.FieldName = "ResName";
		this.gcResName.Name = "gcResName";
		this.gcResName.PrimaryKey = false;
		this.gcResName.UnitField = null;
		this.gcResName.Visible = true;
		this.gcResName.VisibleIndex = 3;
		this.gcResName.Width = 108;
		this.rResName.AutoHeight = false;
		this.rResName.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rResName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rResName.ContainerControl = this;
		this.rResName.DisplayMember = "";
		this.rResName.DisplayMember2 = "";
		this.rResName.LoadDataSourceWhenEnter = false;
		this.rResName.Name = "rResName";
		this.rResName.SeparateCharacterForMulti = ';';
		this.rResName.SeparateString = ", ";
		this.rResName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.rResName.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.rResName.UsingAutoCompleteBox = false;
		this.rResName.UsingDataSource = true;
		this.rResName.ValidateWhenNoDataSource = false;
		this.rResName.ValueMember = "ResName";
		this.gcOpeCode.Caption = "Operation Code";
		this.gcOpeCode.CurrencyDateField = null;
		this.gcOpeCode.CurrencyField = null;
		this.gcOpeCode.CurrencyFixed = null;
		this.gcOpeCode.CurrencyFixedRate = 0.0;
		this.gcOpeCode.CurrencyRateField = null;
		this.gcOpeCode.FieldName = "OpeCode";
		this.gcOpeCode.Name = "gcOpeCode";
		this.gcOpeCode.OptionsColumn.ReadOnly = true;
		this.gcOpeCode.PrimaryKey = false;
		this.gcOpeCode.UnitField = null;
		this.gcOpeCode.Visible = true;
		this.gcOpeCode.VisibleIndex = 4;
		this.gcOpeCode.Width = 119;
		this.gcOpeName.Caption = "Operation Name";
		this.gcOpeName.CurrencyDateField = null;
		this.gcOpeName.CurrencyField = null;
		this.gcOpeName.CurrencyFixed = null;
		this.gcOpeName.CurrencyFixedRate = 0.0;
		this.gcOpeName.CurrencyRateField = null;
		this.gcOpeName.FieldName = "OpeName";
		this.gcOpeName.Name = "gcOpeName";
		this.gcOpeName.OptionsColumn.ReadOnly = true;
		this.gcOpeName.PrimaryKey = false;
		this.gcOpeName.UnitField = null;
		this.gcOpeName.Visible = true;
		this.gcOpeName.VisibleIndex = 5;
		this.gcOpeName.Width = 110;
		this.gcQty.Caption = "Quantity";
		this.gcQty.ColumnEdit = this.rCal;
		this.gcQty.CurrencyDateField = null;
		this.gcQty.CurrencyField = null;
		this.gcQty.CurrencyFixed = null;
		this.gcQty.CurrencyFixedRate = 0.0;
		this.gcQty.CurrencyRateField = null;
		this.gcQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcQty.FieldName = "Qty";
		this.gcQty.Name = "gcQty";
		this.gcQty.PrimaryKey = false;
		this.gcQty.UnitField = null;
		this.gcQty.Visible = true;
		this.gcQty.VisibleIndex = 6;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.gcWorkDay.Caption = "Start Date";
		this.gcWorkDay.ColumnEdit = this.rDate;
		this.gcWorkDay.CurrencyDateField = null;
		this.gcWorkDay.CurrencyField = null;
		this.gcWorkDay.CurrencyFixed = null;
		this.gcWorkDay.CurrencyFixedRate = 0.0;
		this.gcWorkDay.CurrencyRateField = null;
		this.gcWorkDay.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcWorkDay.FieldName = "WorkDay";
		this.gcWorkDay.Name = "gcWorkDay";
		this.gcWorkDay.PrimaryKey = false;
		this.gcWorkDay.UnitField = null;
		this.gcWorkDay.Visible = true;
		this.gcWorkDay.VisibleIndex = 7;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.IsSearchControl = false;
		this.rDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rDate.Name = "rDate";
		this.gcStartTime.Caption = "Start Time";
		this.gcStartTime.CurrencyDateField = null;
		this.gcStartTime.CurrencyField = null;
		this.gcStartTime.CurrencyFixed = null;
		this.gcStartTime.CurrencyFixedRate = 0.0;
		this.gcStartTime.CurrencyRateField = null;
		this.gcStartTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gcStartTime.FieldName = "StartTime";
		this.gcStartTime.Name = "gcStartTime";
		this.gcStartTime.PrimaryKey = false;
		this.gcStartTime.UnitField = null;
		this.gcStartTime.Visible = true;
		this.gcStartTime.VisibleIndex = 8;
		this.gcEndDate.Caption = "EndDate";
		this.gcEndDate.ColumnEdit = this.rDate;
		this.gcEndDate.CurrencyDateField = null;
		this.gcEndDate.CurrencyField = null;
		this.gcEndDate.CurrencyFixed = null;
		this.gcEndDate.CurrencyFixedRate = 0.0;
		this.gcEndDate.CurrencyRateField = null;
		this.gcEndDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcEndDate.FieldName = "EndWorkDay";
		this.gcEndDate.Name = "gcEndDate";
		this.gcEndDate.OptionsColumn.ReadOnly = true;
		this.gcEndDate.PrimaryKey = false;
		this.gcEndDate.UnitField = null;
		this.gcEndDate.Visible = true;
		this.gcEndDate.VisibleIndex = 9;
		this.gcEndTime.Caption = "End Time";
		this.gcEndTime.CurrencyDateField = null;
		this.gcEndTime.CurrencyField = null;
		this.gcEndTime.CurrencyFixed = null;
		this.gcEndTime.CurrencyFixedRate = 0.0;
		this.gcEndTime.CurrencyRateField = null;
		this.gcEndTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gcEndTime.FieldName = "EndTime";
		this.gcEndTime.Name = "gcEndTime";
		this.gcEndTime.OptionsColumn.ReadOnly = true;
		this.gcEndTime.PrimaryKey = false;
		this.gcEndTime.UnitField = null;
		this.gcEndTime.Visible = true;
		this.gcEndTime.VisibleIndex = 10;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(753, 303);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 1;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.Location = new System.Drawing.Point(672, 303);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 2;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grdLine);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(825, 296);
		this.splitContainerControl1.SplitterPosition = 117;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemButtonEditCFL1, this.repositoryItemButtonEditCFL2, this.rDocCal, this.rDocDate });
		this.grdDoc.Size = new System.Drawing.Size(825, 117);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcDocItemCode, this.gcDocItemName, this.gcDocResCode, this.gcDocResName, this.gcDocOpeCode, this.gcDocOpeName, this.gcDocQty, this.gcDocWorkDay });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "AppointmentEditting";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "DOC";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.ReadOnly = true;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gcDocItemCode.Caption = "Item Code";
		this.gcDocItemCode.CurrencyDateField = null;
		this.gcDocItemCode.CurrencyField = null;
		this.gcDocItemCode.CurrencyFixed = null;
		this.gcDocItemCode.CurrencyFixedRate = 0.0;
		this.gcDocItemCode.CurrencyRateField = null;
		this.gcDocItemCode.FieldName = "ItemCode";
		this.gcDocItemCode.Name = "gcDocItemCode";
		this.gcDocItemCode.OptionsColumn.ReadOnly = true;
		this.gcDocItemCode.PrimaryKey = true;
		this.gcDocItemCode.UnitField = null;
		this.gcDocItemCode.Visible = true;
		this.gcDocItemCode.VisibleIndex = 0;
		this.gcDocItemCode.Width = 90;
		this.gcDocItemName.Caption = "Item Name";
		this.gcDocItemName.CurrencyDateField = null;
		this.gcDocItemName.CurrencyField = null;
		this.gcDocItemName.CurrencyFixed = null;
		this.gcDocItemName.CurrencyFixedRate = 0.0;
		this.gcDocItemName.CurrencyRateField = null;
		this.gcDocItemName.FieldName = "ItemName";
		this.gcDocItemName.Name = "gcDocItemName";
		this.gcDocItemName.OptionsColumn.ReadOnly = true;
		this.gcDocItemName.PrimaryKey = false;
		this.gcDocItemName.UnitField = null;
		this.gcDocItemName.Visible = true;
		this.gcDocItemName.VisibleIndex = 1;
		this.gcDocItemName.Width = 102;
		this.gcDocResCode.Caption = "Resource Code";
		this.gcDocResCode.CurrencyDateField = null;
		this.gcDocResCode.CurrencyField = null;
		this.gcDocResCode.CurrencyFixed = null;
		this.gcDocResCode.CurrencyFixedRate = 0.0;
		this.gcDocResCode.CurrencyRateField = null;
		this.gcDocResCode.FieldName = "ResCode";
		this.gcDocResCode.Name = "gcDocResCode";
		this.gcDocResCode.PrimaryKey = false;
		this.gcDocResCode.UnitField = null;
		this.gcDocResCode.Visible = true;
		this.gcDocResCode.VisibleIndex = 2;
		this.gcDocResCode.Width = 106;
		this.gcDocResName.Caption = "Resource Name";
		this.gcDocResName.CurrencyDateField = null;
		this.gcDocResName.CurrencyField = null;
		this.gcDocResName.CurrencyFixed = null;
		this.gcDocResName.CurrencyFixedRate = 0.0;
		this.gcDocResName.CurrencyRateField = null;
		this.gcDocResName.FieldName = "ResName";
		this.gcDocResName.Name = "gcDocResName";
		this.gcDocResName.PrimaryKey = false;
		this.gcDocResName.UnitField = null;
		this.gcDocResName.Visible = true;
		this.gcDocResName.VisibleIndex = 3;
		this.gcDocResName.Width = 108;
		this.gcDocOpeCode.Caption = "Operation Code";
		this.gcDocOpeCode.CurrencyDateField = null;
		this.gcDocOpeCode.CurrencyField = null;
		this.gcDocOpeCode.CurrencyFixed = null;
		this.gcDocOpeCode.CurrencyFixedRate = 0.0;
		this.gcDocOpeCode.CurrencyRateField = null;
		this.gcDocOpeCode.FieldName = "OpeCode";
		this.gcDocOpeCode.Name = "gcDocOpeCode";
		this.gcDocOpeCode.OptionsColumn.ReadOnly = true;
		this.gcDocOpeCode.PrimaryKey = false;
		this.gcDocOpeCode.UnitField = null;
		this.gcDocOpeCode.Visible = true;
		this.gcDocOpeCode.VisibleIndex = 4;
		this.gcDocOpeCode.Width = 119;
		this.gcDocOpeName.Caption = "Operation Name";
		this.gcDocOpeName.CurrencyDateField = null;
		this.gcDocOpeName.CurrencyField = null;
		this.gcDocOpeName.CurrencyFixed = null;
		this.gcDocOpeName.CurrencyFixedRate = 0.0;
		this.gcDocOpeName.CurrencyRateField = null;
		this.gcDocOpeName.FieldName = "OpeName";
		this.gcDocOpeName.Name = "gcDocOpeName";
		this.gcDocOpeName.OptionsColumn.ReadOnly = true;
		this.gcDocOpeName.PrimaryKey = false;
		this.gcDocOpeName.UnitField = null;
		this.gcDocOpeName.Visible = true;
		this.gcDocOpeName.VisibleIndex = 5;
		this.gcDocOpeName.Width = 110;
		this.gcDocQty.Caption = "Quantity";
		this.gcDocQty.ColumnEdit = this.rDocCal;
		this.gcDocQty.CurrencyDateField = null;
		this.gcDocQty.CurrencyField = null;
		this.gcDocQty.CurrencyFixed = null;
		this.gcDocQty.CurrencyFixedRate = 0.0;
		this.gcDocQty.CurrencyRateField = null;
		this.gcDocQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcDocQty.FieldName = "Qty";
		this.gcDocQty.Name = "gcDocQty";
		this.gcDocQty.PrimaryKey = false;
		this.gcDocQty.UnitField = null;
		this.gcDocQty.Visible = true;
		this.gcDocQty.VisibleIndex = 6;
		this.rDocCal.AutoHeight = false;
		this.rDocCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocCal.Name = "rDocCal";
		this.gcDocWorkDay.Caption = "WorkDay";
		this.gcDocWorkDay.ColumnEdit = this.rDocDate;
		this.gcDocWorkDay.CurrencyDateField = null;
		this.gcDocWorkDay.CurrencyField = null;
		this.gcDocWorkDay.CurrencyFixed = null;
		this.gcDocWorkDay.CurrencyFixedRate = 0.0;
		this.gcDocWorkDay.CurrencyRateField = null;
		this.gcDocWorkDay.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocWorkDay.FieldName = "WorkDay";
		this.gcDocWorkDay.Name = "gcDocWorkDay";
		this.gcDocWorkDay.PrimaryKey = false;
		this.gcDocWorkDay.UnitField = null;
		this.gcDocWorkDay.Visible = true;
		this.gcDocWorkDay.VisibleIndex = 7;
		this.rDocDate.AutoHeight = false;
		this.rDocDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDocDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDocDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rDocDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDocDate.IsSearchControl = false;
		this.rDocDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rDocDate.Name = "rDocDate";
		this.repositoryItemButtonEditCFL1.AutoHeight = false;
		this.repositoryItemButtonEditCFL1.AutoLoadDataSource = true;
		this.repositoryItemButtonEditCFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEditCFL1.ContainerControl = this;
		this.repositoryItemButtonEditCFL1.DisplayMember = "";
		this.repositoryItemButtonEditCFL1.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL1.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL1.Name = "repositoryItemButtonEditCFL1";
		this.repositoryItemButtonEditCFL1.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL1.SeparateString = ", ";
		this.repositoryItemButtonEditCFL1.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.repositoryItemButtonEditCFL1.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.repositoryItemButtonEditCFL1.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL1.UsingDataSource = true;
		this.repositoryItemButtonEditCFL1.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL1.ValueMember = "ResCode";
		this.repositoryItemButtonEditCFL2.AutoHeight = false;
		this.repositoryItemButtonEditCFL2.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.repositoryItemButtonEditCFL2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEditCFL2.ContainerControl = this;
		this.repositoryItemButtonEditCFL2.DisplayMember = "";
		this.repositoryItemButtonEditCFL2.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL2.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL2.Name = "repositoryItemButtonEditCFL2";
		this.repositoryItemButtonEditCFL2.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL2.SeparateString = ", ";
		this.repositoryItemButtonEditCFL2.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.repositoryItemButtonEditCFL2.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.repositoryItemButtonEditCFL2.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL2.UsingDataSource = true;
		this.repositoryItemButtonEditCFL2.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL2.ValueMember = "ResName";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(832, 329);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.btnCancel);
		base.FormId = "AppointmentEditting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AppointmentEditting";
		this.Text = "Appointment Editting";
		base.Load += new System.EventHandler(this.frm_AppointmentEditting_Load);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		this.contextMenuStripLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rResName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL2).EndInit();
		base.ResumeLayout(false);
	}
}
