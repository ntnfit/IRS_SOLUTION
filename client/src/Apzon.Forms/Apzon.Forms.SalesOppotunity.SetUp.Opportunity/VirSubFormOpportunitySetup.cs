using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class VirSubFormOpportunitySetup : XtraFormEx
{
	public RefreshLookupType RefeshParentLookUpSource;

	protected string FormController = "";

	protected string TableName = "";

	private int hotTrackRow = int.MinValue;

	private IContainer components;

	protected GridControlEx grdDoc;

	protected GridViewEx gvDoc;

	protected ContextMenuStrip contextMenuStrip1;

	protected ToolStripMenuItem removeToolStripMenuItem;

	protected SimpleButtonEx btCancel;

	protected SimpleButtonEx btOK;

	private int HotTrackRow
	{
		get
		{
			return hotTrackRow;
		}
		set
		{
			if (hotTrackRow != value)
			{
				int rowHandle = hotTrackRow;
				hotTrackRow = value;
				if (gvDoc.ActiveEditor != null)
				{
					gvDoc.PostEditor();
				}
				gvDoc.RefreshRow(rowHandle);
				gvDoc.RefreshRow(hotTrackRow);
				if (hotTrackRow >= 0)
				{
					grdDoc.Cursor = Cursors.Hand;
				}
				else
				{
					grdDoc.Cursor = Cursors.Default;
				}
			}
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	protected VirSubFormOpportunitySetup()
	{
		InitializeComponent();
		SettingLanguage();
		OnLoadAsync += VirSubFormActivitySetup_Load;
	}

	private async Task VirSubFormActivitySetup_Load(object sender, EventArgs e)
	{
		try
		{
			CreateGridViewColumn();
			await ShowData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btOK.Enabled = false;
			WinCommons._OpenProcessing();
			DataTable dtRet = new DataTable();
			if (CheckSave(out dtRet))
			{
				await Save(dtRet);
			}
			WinCommons._CloseProcessing();
			btOK.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			btOK.Enabled = true;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void grdDoc_ProcessGridKey(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "Name" });
			}
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
			FormMode = Enums.FormViewModeType.Update;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_RowClick(object sender, RowClickEventArgs e)
	{
		ApzonCommon.SetColorRowSelected(ref gvDoc);
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_MouseMove(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(new Point(e.X, e.Y));
		if (gridHitInfo.InRowCell)
		{
			HotTrackRow = gridHitInfo.RowHandle;
		}
		else
		{
			HotTrackRow = int.MinValue;
		}
	}

	private void gvDoc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (e.RowHandle == HotTrackRow)
		{
			e.Appearance.BackColor = ApzonCommon.colorFocusRow;
		}
	}

	public async Task REFRESH_DATA()
	{
		await ShowData();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (msg.Msg == 256 || msg.Msg == 260)
		{
			_ = 13;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	protected virtual void CreateGridViewColumn()
	{
	}

	protected virtual async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	protected virtual void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	protected virtual void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	protected virtual void SettingLanguage()
	{
	}

	protected virtual async Task Save(DataTable dt)
	{
		_ = 2;
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await _helper.PostAsync("SaveDoc", dt);
			if (string.IsNullOrEmpty(text))
			{
				if (RefeshParentLookUpSource != null)
				{
					await RefeshParentLookUpSource();
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				await ShowData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task ShowData()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			DataTable dataTable = await _helper.GetAsync("GetData");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual bool CheckSave(out DataTable dtRet)
	{
		dtRet = new DataTable();
		return false;
	}

	protected virtual void AddNewRow()
	{
	}

	protected virtual async Task<string> Delete(int id)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await hp.PostAsync("DeleteDoc", id);
			if (string.IsNullOrEmpty(text))
			{
				if (RefeshParentLookUpSource != null)
				{
					await RefeshParentLookUpSource();
				}
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				return "";
			}
			WinCommons.Error(Enums.MessageCode.None, text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private async void VirSubFormOpportunitySetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			btOK_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.SetUp.Opportunity.VirSubFormOpportunitySetup));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(3, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(489, 286);
		this.grdDoc.TabIndex = 8;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grdDoc_ProcessGridKey);
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oVirActivityStatus";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODATA";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvDoc_RowClick);
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 292);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 7;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(3, 292);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 6;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		base.AcceptButton = this.btOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(495, 318);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.FormId = "oVirActivityStatus";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirSubFormOpportunitySetup";
		this.Text = "Activity Statuses - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirSubFormOpportunitySetup_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		base.ResumeLayout(false);
	}
}
