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
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_SpecialforBP_LineSPP1 : XtraFormEx
{
	private string _formController = "SpecialBp_LineSPP1";

	public string _currency;

	public string _autoUpdt;

	public string _expand;

	public string _itemCode;

	public string _cardCode;

	public double priceItem;

	public int PriceList;

	public DataSet DsPriceConfig = new DataSet();

	private bool IgnoreEvent;

	private IContainer components;

	private Panel panel1;

	private GridControlEx grdDocLineSPP1;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcLINENUM;

	private GridColumnEx gcPriceAfterDiscount;

	private GridColumnEx gcCurrency;

	private GridColumnEx gcDiscount;

	private GridColumnEx gcListNum;

	private GridColumnEx gcFromDate;

	private GridColumnEx gcToDate;

	private GridColumnEx gcAutoUpdt;

	private GridColumnEx gcExpand;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rLU_PriceList;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2_Discount;

	private SimpleButtonEx bt_CopyDiscount;

	private GridViewEx gvDocLineSPP1;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

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

	public frm_SpecialforBP_LineSPP1()
	{
		InitializeComponent();
	}

	private async void frm_SpecialforBP_LineSPP1_Load(object sender, EventArgs e)
	{
		await LoadColumPriceList();
		grdDocLineSPP1.DataSource = DsPriceConfig.Tables[0];
		grdDocLineSPP1.RefreshDataSource();
		gvDocLineSPP1.HideLoadingPanel();
		AddNewRow();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if ((from t in grdDocLineSPP1.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["ListNum"]) == 0 && t["FromDate"] == DBNull.Value
				select t).ToList().Count != 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Invalid Date Range");
				return;
			}
			DialogResult = ((FormMode != Enums.FormViewModeType.View) ? DialogResult.OK : DialogResult.Cancel);
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocLineSPP1_DoubleClick(object sender, EventArgs e)
	{
		DateTime? dateTime = Function.ParseDateTime(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcFromDate));
		decimal num = Function.ParseDecimal(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcToDate));
		int lineNum = Function.ParseInt(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcLINENUM));
		double num2 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcDiscount));
		double num3 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, "SourcePrice"));
		if ((!dateTime.HasValue && !(num != 0m)) || num2 == 0.0)
		{
			return;
		}
		DataTable dtConfig = DsPriceConfig.Tables[1].Clone();
		List<DataRow> list = (from t in DsPriceConfig.Tables[1].AsEnumerable()
			where Function.ParseInt(t["SPP1LNum"]) == lineNum
			select t).ToList();
		if (list.Any())
		{
			dtConfig = list.CopyToDataTable();
		}
		frm_SpecialforBP_LineSPP2 frm_SpecialforBP_LineSPP3 = new frm_SpecialforBP_LineSPP2();
		frm_SpecialforBP_LineSPP3._ItemCode = _itemCode;
		frm_SpecialforBP_LineSPP3._CardCode = _cardCode;
		frm_SpecialforBP_LineSPP3._Spp1LNum = lineNum;
		frm_SpecialforBP_LineSPP3._Currency = _currency;
		frm_SpecialforBP_LineSPP3._Discount = num2;
		frm_SpecialforBP_LineSPP3.priceItem = num3;
		frm_SpecialforBP_LineSPP3.dtConfig = dtConfig;
		frm_SpecialforBP_LineSPP3.PriceList = Function.ParseInt(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcListNum));
		frm_SpecialforBP_LineSPP3.ShowDialog(this);
		if (frm_SpecialforBP_LineSPP3.DialogResult != DialogResult.OK)
		{
			return;
		}
		FormMode = Enums.FormViewModeType.Update;
		for (int num4 = 0; num4 < list.Count; num4++)
		{
			DsPriceConfig.Tables[1].Rows.Remove(list[num4]);
		}
		for (int num5 = 0; num5 < frm_SpecialforBP_LineSPP3.dtConfig.Rows.Count; num5++)
		{
			if (Function.ParseDouble(frm_SpecialforBP_LineSPP3.dtConfig.Rows[num5]["Discount"]) != 0.0)
			{
				DataRow row = frm_SpecialforBP_LineSPP3.dtConfig.Rows[num5];
				DsPriceConfig.Tables[1].ImportRow(row);
			}
		}
		DsPriceConfig.Tables[1].AcceptChanges();
	}

	private void gvDocLineSPP1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (IgnoreEvent)
		{
			return;
		}
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			if (e.Column == gcFromDate || e.Column == gcToDate)
			{
				if (Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(gvDocLineSPP1.FocusedRowHandle, gcDiscount)) != 0.0)
				{
					if (Function.ParseInt(gvDocLineSPP1.GetRowCellValue(e.RowHandle, gcDiscount)) == 0)
					{
						gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcDiscount, 0);
					}
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcPriceAfterDiscount, gvDocLineSPP1.GetRowCellValue(e.RowHandle, "SourcePrice"));
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcCurrency, gvDocLineSPP1.GetRowCellValue(e.RowHandle, "Currency"));
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcAutoUpdt, "Y");
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcCardCode, _cardCode);
					if (Function.ParseInt(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "ListNum")) == 0)
					{
						gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcListNum, PriceList);
					}
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcExpand, "N");
				}
				AddNewRow();
			}
			else if (e.Column == gcDiscount)
			{
				IgnoreEvent = true;
				if (Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "ListNum")) != 0.0)
				{
					double num = Function.ParseDouble(e.Value);
					double num2 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "SourcePrice")) * (1.0 - num / 100.0);
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcPriceAfterDiscount, num2);
				}
				IgnoreEvent = false;
			}
			else if (e.Column == gcPriceAfterDiscount)
			{
				IgnoreEvent = true;
				if (Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "ListNum")) != 0.0)
				{
					double num3 = Function.ParseDouble(e.Value);
					double num4 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "SourcePrice"));
					double num5 = ((num4 != 0.0) ? ((1.0 - num3 / num4) * 100.0) : 0.0);
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcDiscount, num5);
				}
				IgnoreEvent = false;
			}
			else if (e.Column == gcListNum)
			{
				string itemCode = Function.ToString(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "ItemCode"));
				int num6 = Function.ParseInt(e.Value);
				DataTable basePrice = HttpClient.PriceListSetup.GetBasePrice(num6, itemCode);
				IgnoreEvent = true;
				DataRow dataRow = gvDocLineSPP1.GetDataRow(e.RowHandle);
				if (basePrice.IsNotNull())
				{
					dataRow["SourcePrice"] = basePrice.Rows[0]["Price"];
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcCurrency, basePrice.Rows[0]["Currency"]);
				}
				else if (num6 != 0)
				{
					dataRow["SourcePrice"] = DBNull.Value;
				}
				double num7 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "SourcePrice"));
				if (num7 == 0.0)
				{
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcPriceAfterDiscount, 0);
				}
				else
				{
					double num8 = Function.ParseDouble(gvDocLineSPP1.GetRowCellValue(e.RowHandle, "Discount"));
					double num9 = num7 * (1.0 - num8 / 100.0);
					gvDocLineSPP1.SetRowCellValue(e.RowHandle, gcPriceAfterDiscount, num9);
				}
				IgnoreEvent = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocLineSPP1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDocLineSPP1, e.Point);
	}

	private void gvDocLineSPP1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (DsPriceConfig.Tables[1].IsNotNull())
			{
				List<DataRow> listDataSourceSelectedRow = gvDocLineSPP1.GetListDataSourceSelectedRow();
				IEnumerable<int> lstItem = listDataSourceSelectedRow.Select((DataRow t) => Function.ParseInt(t["LINENUM"]));
				List<DataRow> list = (from t in DsPriceConfig.Tables[1].AsEnumerable()
					where lstItem.Contains(Function.ParseInt(t["SPP1LNum"]))
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					DsPriceConfig.Tables[1].Rows.Remove(list[num]);
				}
			}
			gvDocLineSPP1.DeleteDataSourceRowSelected();
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDocLineSPP1_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseDouble(gvDocLineSPP1.GetFocusedRowCellValue("SourcePrice")) == 0.0 && gvDocLineSPP1.FocusedColumn == gcPriceAfterDiscount && Function.ParseInt(gvDocLineSPP1.GetFocusedRowCellValue("ListNum")) != 0)
			{
				WinCommons.Error(Enums.MessageCode.NoPriceDefined4PriceList, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_CopyDiscount_Click(object sender, EventArgs e)
	{
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private async Task LoadColumPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			DataTable dataTable = await _hp.GetAsync("GetGroupNumPriceList");
			DataRow dataRow = dataTable.NewRow();
			dataRow["ListNum"] = -1;
			dataRow["ListName"] = "Last Purchase Price";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["ListNum"] = -2;
			dataRow2["ListName"] = "Last Evaluated Price";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["ListNum"] = 0;
			dataRow3["ListName"] = "Without Price List";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			rLU_PriceList.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grdDocLineSPP1.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || dataSourceTable.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["FromDate"])) || !string.IsNullOrEmpty(Function.ToString(t["ToDate"]))))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				int num = (dataSourceTable.IsNotNull() ? (dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LINENUM"])) + 1) : 0);
				dataRow["LINENUM"] = num;
				dataRow["ItemCode"] = _itemCode;
				dataRow["CardCode"] = _cardCode;
				dataRow["Currency"] = _currency;
				dataRow["AutoUpdt"] = "Y";
				dataRow["Expand"] = "N";
				dataRow["ListNum"] = PriceList;
				dataRow["SourcePrice"] = priceItem;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				grdDocLineSPP1.DataSource = dataSourceTable;
				grdDocLineSPP1.RefreshDataSource();
			}
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_SpecialforBP_LineSPP1));
		this.panel1 = new System.Windows.Forms.Panel();
		this.bt_CopyDiscount = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDocLineSPP1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocLineSPP1 = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLINENUM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPriceAfterDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2_Discount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcListNum = new Apzon.Controls.Grid.GridColumnEx();
		this.rLU_PriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcAutoUpdt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpand = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.bt_CopyDiscount);
		this.panel1.Controls.Add(this.btOK);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.grdDocLineSPP1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(703, 299);
		this.panel1.TabIndex = 0;
		this.bt_CopyDiscount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bt_CopyDiscount.EditControl = null;
		this.bt_CopyDiscount.EditValue = "Copy Discounts";
		this.bt_CopyDiscount.FieldName = null;
		this.bt_CopyDiscount.IsSearchControl = false;
		this.bt_CopyDiscount.Location = new System.Drawing.Point(577, 274);
		this.bt_CopyDiscount.Name = "bt_CopyDiscount";
		this.bt_CopyDiscount.ReadOnly = false;
		this.bt_CopyDiscount.SettingInVisible = false;
		this.bt_CopyDiscount.SettingReadonly = false;
		this.bt_CopyDiscount.Size = new System.Drawing.Size(126, 23);
		this.bt_CopyDiscount.TabIndex = 40;
		this.bt_CopyDiscount.Text = "Copy Discounts";
		this.bt_CopyDiscount.Visible = false;
		this.bt_CopyDiscount.Click += new System.EventHandler(this.bt_CopyDiscount_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(0, 276);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 38;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(102, 276);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 39;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.grdDocLineSPP1.AllowPasteExcel = true;
		this.grdDocLineSPP1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDocLineSPP1.ClipboardData = null;
		this.grdDocLineSPP1.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDocLineSPP1.Location = new System.Drawing.Point(0, 0);
		this.grdDocLineSPP1.MainView = this.gvDocLineSPP1;
		this.grdDocLineSPP1.Name = "grdDocLineSPP1";
		this.grdDocLineSPP1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rLU_PriceList, this.repositoryItemDateEdit1, this.repositoryItemDateEdit2, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2_Discount });
		this.grdDocLineSPP1.Size = new System.Drawing.Size(703, 270);
		this.grdDocLineSPP1.TabIndex = 0;
		this.grdDocLineSPP1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocLineSPP1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDocLineSPP1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocLineSPP1.ColumnKey = null;
		this.gvDocLineSPP1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gcItemCode, this.gcCardCode, this.gcLINENUM, this.gcPriceAfterDiscount, this.gcCurrency, this.gcDiscount, this.gcListNum, this.gcFromDate, this.gcToDate, this.gcAutoUpdt,
			this.gcExpand
		});
		this.gvDocLineSPP1.DisableFormSetting = false;
		this.gvDocLineSPP1.DisplayException = false;
		this.gvDocLineSPP1.EditColumn = null;
		this.gvDocLineSPP1.FieldCurrency = null;
		this.gvDocLineSPP1.FieldRate = null;
		this.gvDocLineSPP1.FormId = "oPeriodDiscount";
		this.gvDocLineSPP1.GridControl = this.grdDocLineSPP1;
		this.gvDocLineSPP1.IgnoreFilter = false;
		this.gvDocLineSPP1.IndexEditRow = -1;
		this.gvDocLineSPP1.IndicatorWidth = 35;
		this.gvDocLineSPP1.IsApzTable = false;
		this.gvDocLineSPP1.IsInsertNewRow = false;
		this.gvDocLineSPP1.IsLoadingData = false;
		this.gvDocLineSPP1.IsMappingContextAction = false;
		this.gvDocLineSPP1.ItemId = "SPP1";
		this.gvDocLineSPP1.LstColumnAllowBatch = null;
		this.gvDocLineSPP1.LstColumnNotAllowBatch = null;
		this.gvDocLineSPP1.LstColumnNotEditable = null;
		this.gvDocLineSPP1.LstColumNotSetting = null;
		this.gvDocLineSPP1.Name = "gvDocLineSPP1";
		this.gvDocLineSPP1.OptionsView.ColumnAutoWidth = false;
		this.gvDocLineSPP1.OptionsView.ShowFooter = true;
		this.gvDocLineSPP1.OptionsView.ShowGroupPanel = false;
		this.gvDocLineSPP1.UseBatchMod = false;
		this.gvDocLineSPP1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocLineSPP1_CustomDrawRowIndicator);
		this.gvDocLineSPP1.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDocLineSPP1_PopupMenuShowing);
		this.gvDocLineSPP1.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDocLineSPP1_ShowingEditor);
		this.gvDocLineSPP1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocLineSPP1_CellValueChanged);
		this.gvDocLineSPP1.DoubleClick += new System.EventHandler(this.gvDocLineSPP1_DoubleClick);
		this.gcItemCode.Caption = "ItemCode";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Width = 70;
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.gcCardCode.Width = 69;
		this.gcLINENUM.Caption = "LINENUM";
		this.gcLINENUM.CurrencyDateField = null;
		this.gcLINENUM.CurrencyField = null;
		this.gcLINENUM.CurrencyFixed = null;
		this.gcLINENUM.CurrencyFixedRate = 0.0;
		this.gcLINENUM.CurrencyRateField = null;
		this.gcLINENUM.FieldName = "LINENUM";
		this.gcLINENUM.Name = "gcLINENUM";
		this.gcLINENUM.PrimaryKey = false;
		this.gcLINENUM.UnitField = null;
		this.gcLINENUM.Width = 54;
		this.gcPriceAfterDiscount.Caption = "Price after Discount";
		this.gcPriceAfterDiscount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcPriceAfterDiscount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPriceAfterDiscount.CurrencyDateField = null;
		this.gcPriceAfterDiscount.CurrencyField = "Currency";
		this.gcPriceAfterDiscount.CurrencyFixed = null;
		this.gcPriceAfterDiscount.CurrencyFixedRate = 0.0;
		this.gcPriceAfterDiscount.CurrencyRateField = null;
		this.gcPriceAfterDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPriceAfterDiscount.FieldName = "Price";
		this.gcPriceAfterDiscount.Name = "gcPriceAfterDiscount";
		this.gcPriceAfterDiscount.PrimaryKey = false;
		this.gcPriceAfterDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPriceAfterDiscount.UnitField = null;
		this.gcPriceAfterDiscount.Visible = true;
		this.gcPriceAfterDiscount.VisibleIndex = 4;
		this.gcPriceAfterDiscount.Width = 115;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.ReadOnly = true;
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		this.gcCurrency.Visible = true;
		this.gcCurrency.VisibleIndex = 5;
		this.gcCurrency.Width = 77;
		this.gcDiscount.Caption = "Discount";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEdit2_Discount;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 3;
		this.gcDiscount.Width = 141;
		this.repositoryItemCalcEdit2_Discount.AutoHeight = false;
		this.repositoryItemCalcEdit2_Discount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.Name = "repositoryItemCalcEdit2_Discount";
		this.gcListNum.Caption = "Price list";
		this.gcListNum.ColumnEdit = this.rLU_PriceList;
		this.gcListNum.CurrencyDateField = null;
		this.gcListNum.CurrencyField = null;
		this.gcListNum.CurrencyFixed = null;
		this.gcListNum.CurrencyFixedRate = 0.0;
		this.gcListNum.CurrencyRateField = null;
		this.gcListNum.FieldName = "ListNum";
		this.gcListNum.Name = "gcListNum";
		this.gcListNum.PrimaryKey = false;
		this.gcListNum.UnitField = null;
		this.gcListNum.Visible = true;
		this.gcListNum.VisibleIndex = 2;
		this.gcListNum.Width = 143;
		this.rLU_PriceList.AutoHeight = false;
		this.rLU_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLU_PriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListNum", "Code")
		});
		this.rLU_PriceList.DisplayMember = "ListName";
		this.rLU_PriceList.Name = "rLU_PriceList";
		this.rLU_PriceList.NullText = "";
		this.rLU_PriceList.ShowHeader = false;
		this.rLU_PriceList.ValueMember = "ListNum";
		this.gcFromDate.Caption = "Valid from";
		this.gcFromDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcFromDate.CurrencyDateField = null;
		this.gcFromDate.CurrencyField = null;
		this.gcFromDate.CurrencyFixed = null;
		this.gcFromDate.CurrencyFixedRate = 0.0;
		this.gcFromDate.CurrencyRateField = null;
		this.gcFromDate.FieldName = "FromDate";
		this.gcFromDate.Name = "gcFromDate";
		this.gcFromDate.PrimaryKey = false;
		this.gcFromDate.UnitField = null;
		this.gcFromDate.Visible = true;
		this.gcFromDate.VisibleIndex = 0;
		this.gcFromDate.Width = 93;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcToDate.Caption = "Valid until";
		this.gcToDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.gcToDate.CurrencyDateField = null;
		this.gcToDate.CurrencyField = null;
		this.gcToDate.CurrencyFixed = null;
		this.gcToDate.CurrencyFixedRate = 0.0;
		this.gcToDate.CurrencyRateField = null;
		this.gcToDate.FieldName = "ToDate";
		this.gcToDate.Name = "gcToDate";
		this.gcToDate.PrimaryKey = false;
		this.gcToDate.UnitField = null;
		this.gcToDate.Visible = true;
		this.gcToDate.VisibleIndex = 1;
		this.gcToDate.Width = 97;
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit2.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit2.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.gcAutoUpdt.Caption = "AutoUpdt";
		this.gcAutoUpdt.CurrencyDateField = null;
		this.gcAutoUpdt.CurrencyField = null;
		this.gcAutoUpdt.CurrencyFixed = null;
		this.gcAutoUpdt.CurrencyFixedRate = 0.0;
		this.gcAutoUpdt.CurrencyRateField = null;
		this.gcAutoUpdt.FieldName = "AutoUpdt";
		this.gcAutoUpdt.Name = "gcAutoUpdt";
		this.gcAutoUpdt.PrimaryKey = false;
		this.gcAutoUpdt.UnitField = null;
		this.gcAutoUpdt.Width = 58;
		this.gcExpand.Caption = "Expand";
		this.gcExpand.CurrencyDateField = null;
		this.gcExpand.CurrencyField = null;
		this.gcExpand.CurrencyFixed = null;
		this.gcExpand.CurrencyFixedRate = 0.0;
		this.gcExpand.CurrencyRateField = null;
		this.gcExpand.FieldName = "Expand";
		this.gcExpand.Name = "gcExpand";
		this.gcExpand.PrimaryKey = false;
		this.gcExpand.UnitField = null;
		this.gcExpand.Width = 48;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(703, 299);
		base.Controls.Add(this.panel1);
		base.FormId = "oPeriodDiscount";
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.Name = "frm_SpecialforBP_LineSPP1";
		this.Text = "Period discount";
		base.Load += new System.EventHandler(this.frm_SpecialforBP_LineSPP1_Load);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
