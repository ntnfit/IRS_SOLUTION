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
using Apzon.Entities.Models.Documents;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Purchase;

public class frm_PLandedCostsList : XtraFormEx
{
	public delegate void CloseEvent(string docEntry);

	private DocumentsImpl itemSearch;

	public CloseEvent reloadParentComboBox;

	private readonly HttpClientHelper<DocumentsImpl> _httpClientHelper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "LandedCost");

	private IContainer components;

	private SimpleButtonEx btClose;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	public frm_PLandedCostsList(DocumentsImpl item)
	{
		InitializeComponent();
		itemSearch = item;
	}

	private async void LoadData()
	{
		try
		{
			DataTable dt = await _httpClientHelper.GetDataTableAsJsonAsync("Search", itemSearch);
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = dt;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = dt;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			string rowCellDisplayText = gvDoc.GetRowCellDisplayText(gvDoc.FocusedRowHandle, "DocEntry");
			reloadParentComboBox(rowCellDisplayText);
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void frm_PLandedCostsList_Load(object sender, EventArgs e)
	{
		try
		{
			LoadData();
			for (int i = 0; i < gvDoc.Columns.Count; i++)
			{
				gvDoc.Columns[i].MinWidth = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Purchase.frm_PLandedCostsList));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.btClose = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(-4, 11);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(524, 280);
		this.grdDoc.TabIndex = 2;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.Editable = false;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.DoubleClick += new System.EventHandler(this.gvDoc_DoubleClick);
		this.btClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btClose.EditControl = null;
		this.btClose.EditValue = "Close";
		this.btClose.FieldName = null;
		this.btClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btClose.ImageOptions.Image");
		this.btClose.IsSearchControl = false;
		this.btClose.Location = new System.Drawing.Point(4, 297);
		this.btClose.Name = "btClose";
		this.btClose.ReadOnly = false;
		this.btClose.SettingInVisible = false;
		this.btClose.SettingReadonly = false;
		this.btClose.Size = new System.Drawing.Size(75, 23);
		this.btClose.TabIndex = 3;
		this.btClose.Text = "Close";
		this.btClose.Click += new System.EventHandler(this.btClose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(516, 324);
		base.Controls.Add(this.btClose);
		base.Controls.Add(this.grdDoc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PLandedCostsList";
		this.Text = "Landed Costs List";
		base.Load += new System.EventHandler(this.frm_PLandedCostsList_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		base.ResumeLayout(false);
	}
}
