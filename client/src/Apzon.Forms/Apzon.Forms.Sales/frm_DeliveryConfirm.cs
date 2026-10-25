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
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Sales;

public class frm_DeliveryConfirm : XtraFormEx
{
	private const string UdoCode = "ODLO";

	public string UdoTableName = "@ODLO";

	public DataTable UdoTable = new DataTable();

	private const string FormController = "DeliveryConfirm";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private IContainer components;

	private SimpleButtonEx btn_Update;

	private TextEditEx txt_BarCode;

	private LabelLine lbl_BarCode;

	private LabelLine lbl_DocEntry;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Line;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl3;

	private GridViewEx gv_Line;

	private LabelLine labelLine3;

	private TextEditEx txt_User;

	private LabelLine labelLine2;

	private TextEditEx txt_DocEntry;

	private SimpleButtonEx btn_Refresh;

	private ToolTipController ttc_DeliveryConfirm;

	private DateEditEx date_Doc;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_DeliveryConfirm()
	{
		InitializeComponent();
	}

	private async void frm_DeliveryConfirm_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			txt_User.Text = GlobalData.UserName;
			date_Doc.EditValue = DateTime.Today;
			lstUdf = await _httpClient.Udf.GetListField("OBTN");
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "ODLO"
			});
			Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_DocEntry_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_DocEntry.Text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			int num = Function.ParseInt(txt_DocEntry.EditValue);
			if (num < 1)
			{
				WinCommons._CloseProcessing();
				XtraMessageBox.Show("Số phiếu không hợp lệ!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				txt_DocEntry.EditValue = null;
				txt_DocEntry.Focus();
				return;
			}
			DataSet ds = await GetDeliveryOrder(num);
			if (ds.IsHasStructure())
			{
				grd_Line.DataSource = null;
				GridViewHelper.dfAppearanceEvenRow = false;
				await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
				GridViewHelper.dfAppearanceEvenRow = true;
				if (!ds.Tables[1].IsNotNull())
				{
					WinCommons._CloseProcessing();
					XtraMessageBox.Show("Không có dữ liệu batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					txt_DocEntry.EditValue = null;
					txt_DocEntry.Focus();
					return;
				}
				txt_DocEntry.Enabled = false;
				btn_Refresh.Enabled = true;
				txt_BarCode.Enabled = true;
				txt_BarCode.Text = "";
				txt_BarCode.Focus();
			}
			ds.Tables.Clear();
			ds.Dispose();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_BarCode_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BarCode.Text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (grd_Line.DataSource != null)
			{
				string barcode = txt_BarCode.Text;
				int packingNo = 0;
				DataTable dt = grd_Line.DataSource as DataTable;
				if (dt.ExistsColumn("U_BatchNo") && dt.ExistsColumn("U_PackNo") && dt.ExistsColumn("U_Confirmed"))
				{
					DataRow[] dr = dt.Select($"U_BatchNo='{barcode}' AND DocEntry>0 AND U_ItemCode IS NOT NULL");
					if (dr.Length == 0)
					{
						string text = ((Function.ParseInt(barcode) > 0) ? barcode : "-1");
						dr = dt.Select($"U_PackNo='{text}'");
						if (dr.Length == 0)
						{
							DataTable dataTable = await GetBatchDetail(barcode);
							if (dataTable.IsNotNull())
							{
								DataRow dataRow = dt.NewRow();
								WinFuntions.DataRowToDataRow(dataTable.Rows[0], dataRow);
								dt.Rows.Add(dataRow);
								gv_Line.FocusedRowHandle = dt.Rows.IndexOf(dataRow);
							}
							else
							{
								DataRow dataRow2 = dt.NewRow();
								dataRow2["DocEntry"] = -2;
								dataRow2["U_BatchNo"] = barcode;
								dataRow2["BatchEncrypt"] = barcode;
								dataRow2["Checked"] = "False";
								dt.Rows.Add(dataRow2);
								gv_Line.FocusedRowHandle = dt.Rows.IndexOf(dataRow2);
							}
						}
						else
						{
							packingNo = Function.ParseInt(text);
						}
					}
					if (dr.Length != 0)
					{
						if (packingNo > 0)
						{
							DataSet dataSet = await GetRecordPickAndPack(packingNo, "S");
							if (dataSet.IsHasStructure() && dataSet.Tables[0].IsNotNull() && dataSet.Tables[1].IsNotNull() && dr.Length != dataSet.Tables[1].Rows.Count)
							{
								XtraMessageBox.Show("Số batch trong tem packing và trong phiếu không đúng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
								txt_BarCode.Text = string.Empty;
								txt_BarCode.Focus();
								WinCommons._CloseProcessing();
								return;
							}
						}
						DataRow[] array = dr;
						foreach (DataRow dataRow3 in array)
						{
							if (!Function.ParseBool(dataRow3["Checked"]))
							{
								dataRow3["Checked"] = "True";
								gv_Line.FocusedRowHandle = dt.Rows.IndexOf(dataRow3);
								continue;
							}
							DataRow dataRow4 = dt.NewRow();
							WinFuntions.DataRowToDataRow(dataRow3, dataRow4);
							dataRow4["DocEntry"] = -3;
							dataRow4["Checked"] = "False";
							dt.Rows.Add(dataRow4);
							gv_Line.FocusedRowHandle = dt.Rows.IndexOf(dataRow4);
							txt_BarCode.Text = string.Empty;
							txt_BarCode.Focus();
							WinCommons._CloseProcessing();
							return;
						}
					}
				}
			}
			FormMode = Enums.FormViewModeType.Update;
			txt_BarCode.Text = string.Empty;
			txt_BarCode.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				return;
			}
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				XtraMessageBox.Show("Không có danh sách batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			DataRow[] array = dataSourceTable.Select("Checked = 'False'");
			if (array != null && array.Length != 0)
			{
				XtraMessageBox.Show("Chưa confirm hết batch!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			dataSet.Tables.Add(UdoTable.Copy());
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("U_Confirmed", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = txt_DocEntry.EditValue;
			dataRow["U_Confirmed"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable);
			DataTable dataTable2 = dataSourceTable.Copy();
			dataTable2.TableName = "DLO1";
			dataSet.Tables.Add(dataTable2);
			string text = await ConfirmBatch(dataSet);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				WinCommons.Success(Enums.MessageCode.Success, "");
				REFRESH();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Line";
			dXMenuItem.Click += lineMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lineMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(sender as DXMenuItem).Caption.Equals("Remove Line"))
			{
				return;
			}
			GridView gridView = (GridView)grd_Line.MainView;
			if (gridView.FocusedRowHandle <= -1)
			{
				return;
			}
			if (Function.ParseInt(gridView.GetFocusedRowCellValue("DocEntry").ToString()) > 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					gridView.DeleteRow(gridView.FocusedRowHandle);
				}
			}
			else
			{
				gridView.DeleteRow(gridView.FocusedRowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle <= -1 || !gv_Line.IsExistColumn("DocEntry"))
			{
				return;
			}
			int num = Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "DocEntry"));
			bool flag = Function.ParseBool(gv_Line.GetRowCellValue(e.RowHandle, "Checked"));
			switch (num)
			{
			case -1:
				e.Appearance.BackColor = Color.Red;
				return;
			case -2:
				e.Appearance.BackColor = Color.DarkRed;
				return;
			case -3:
				e.Appearance.BackColor = Color.Orange;
				return;
			}
			if (flag)
			{
				e.Appearance.BackColor = Color.DodgerBlue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.View;
		grd_Line.DataSource = null;
		txt_DocEntry.Enabled = true;
		txt_BarCode.Enabled = false;
		btn_Refresh.Enabled = false;
		txt_BarCode.Text = "";
		txt_DocEntry.Text = "";
		txt_DocEntry.Focus();
	}

	private async Task<string> ConfirmBatch(DataSet ds)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryConfirm");
			return await _help.PostAsJsonAsync("ConfirmBatch", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private async Task<DataSet> GetDeliveryOrder(int docEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryConfirm");
			return await hp.GetAsync("GetDeliveryOrder", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataTable> GetBatchDetail(string batch)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryConfirm");
			return await hp.GetAsync("GetBatchDetail", new { batch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataSet> GetRecordPickAndPack(int docEntry, string type)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "PickAndPack");
			return await hp.GetAsync("GetRecord", new { docEntry, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocEntry.Text);
			await WinFuntions.PrintLayout("", num, "ODLO");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			int docEntry = Function.ParseInt(txt_DocEntry.Text);
			await WinFuntions.Print("", docEntry, "ODLO");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ttc_DeliveryConfirm_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (e.SelectedControl != grd_Line)
			{
				return;
			}
			ToolTipControlInfo toolTipControlInfo = null;
			if (!(grd_Line.GetViewAt(e.ControlMousePosition) is GridView gridView))
			{
				return;
			}
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.ControlMousePosition);
			if (gridHitInfo.HitTest == GridHitTest.RowCell && gridHitInfo.RowHandle >= 0)
			{
				string text = "";
				int num = Function.ParseInt(gv_Line.GetRowCellValue(gridHitInfo.RowHandle, "DocEntry"));
				bool flag = Function.ParseBool(gv_Line.GetRowCellValue(gridHitInfo.RowHandle, "Checked"));
				switch (num)
				{
				case -1:
					Text = "Hàng bắn sai tem!";
					break;
				case -2:
					text = "Hàng không có trong hệ thống!";
					break;
				case -3:
					text = "Hàng bắn trùng!";
					break;
				default:
					if (flag)
					{
						text = "Hàng đã bắn barcode ok!";
					}
					else if (!flag)
					{
						text = "Hàng chưa kiểm!";
					}
					break;
				}
				toolTipControlInfo = new ToolTipControlInfo(gridHitInfo.HitTest + gridHitInfo.RowHandle, text);
			}
			if (toolTipControlInfo != null)
			{
				e.Info = toolTipControlInfo;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_DeliveryConfirm));
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.ttc_DeliveryConfirm = new DevExpress.Utils.ToolTipController(this.components);
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DocEntry = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_User = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocEntry = new Apzon.Controls.Editor.TextEditEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		base.SuspendLayout();
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(12, 7);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(75, 23);
		this.btn_Update.TabIndex = 18;
		this.btn_Update.Text = "OK";
		this.btn_Update.ToolTip = "Xác nhận ra cổng";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(2, 2);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(749, 326);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ToolTipController = this.ttc_DeliveryConfirm;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
		this.gv_Line.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Line.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
		this.gv_Line.Appearance.FocusedRow.BorderColor = System.Drawing.Color.Transparent;
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Line.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Line.FormId = "oDeliveryConfirm";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OLINE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Line_RowStyle);
		this.gv_Line.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Line_PopupMenuShowing);
		this.ttc_DeliveryConfirm.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.ttc_DeliveryConfirm_GetActiveObjectInfo);
		this.txt_BarCode.EditControl = null;
		this.txt_BarCode.FieldName = null;
		this.txt_BarCode.IsSearchControl = false;
		this.txt_BarCode.Location = new System.Drawing.Point(92, 31);
		this.txt_BarCode.Name = "txt_BarCode";
		this.txt_BarCode.SettingInVisible = false;
		this.txt_BarCode.SettingReadonly = false;
		this.txt_BarCode.Size = new System.Drawing.Size(251, 20);
		this.txt_BarCode.TabIndex = 13;
		this.txt_BarCode.ToolTip = "Bắn barcode hàng";
		this.txt_BarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCode_KeyDown);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 7);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 19;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbl_BarCode.EditControl = null;
		this.lbl_BarCode.EditValue = "Batch (Barcode)";
		this.lbl_BarCode.FieldName = null;
		this.lbl_BarCode.HideWhenEmpty = false;
		this.lbl_BarCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BarCode.Location = new System.Drawing.Point(5, 33);
		this.lbl_BarCode.Name = "lbl_BarCode";
		this.lbl_BarCode.ReadOnly = false;
		this.lbl_BarCode.SettingInVisible = false;
		this.lbl_BarCode.SettingReadonly = false;
		this.lbl_BarCode.ShowUnderLine = true;
		this.lbl_BarCode.Size = new System.Drawing.Size(89, 20);
		this.lbl_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BarCode.TabIndex = 12;
		this.lbl_BarCode.Utext = "Batch (Barcode)";
		this.lbl_DocEntry.EditControl = null;
		this.lbl_DocEntry.EditValue = "Số phiếu";
		this.lbl_DocEntry.FieldName = null;
		this.lbl_DocEntry.HideWhenEmpty = false;
		this.lbl_DocEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocEntry.Location = new System.Drawing.Point(5, 8);
		this.lbl_DocEntry.Name = "lbl_DocEntry";
		this.lbl_DocEntry.ReadOnly = false;
		this.lbl_DocEntry.SettingInVisible = false;
		this.lbl_DocEntry.SettingReadonly = false;
		this.lbl_DocEntry.ShowUnderLine = true;
		this.lbl_DocEntry.Size = new System.Drawing.Size(88, 20);
		this.lbl_DocEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocEntry.TabIndex = 10;
		this.lbl_DocEntry.Utext = "Số phiếu";
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Update);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 390);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(753, 35);
		this.panelControl1.TabIndex = 22;
		this.panelControl2.Controls.Add(this.date_Doc);
		this.panelControl2.Controls.Add(this.btn_Refresh);
		this.panelControl2.Controls.Add(this.labelLine3);
		this.panelControl2.Controls.Add(this.txt_User);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.txt_DocEntry);
		this.panelControl2.Controls.Add(this.lbl_DocEntry);
		this.panelControl2.Controls.Add(this.txt_BarCode);
		this.panelControl2.Controls.Add(this.lbl_BarCode);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(753, 60);
		this.panelControl2.TabIndex = 23;
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(613, 6);
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
		this.date_Doc.Size = new System.Drawing.Size(135, 20);
		this.date_Doc.TabIndex = 20;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Clear";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(268, 3);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 19;
		this.btn_Refresh.Text = "Clear";
		this.btn_Refresh.ToolTip = "Làm mới";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(534, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 16;
		this.labelLine3.Utext = "Doc Date";
		this.txt_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_User.EditControl = null;
		this.txt_User.FieldName = null;
		this.txt_User.IsSearchControl = false;
		this.txt_User.Location = new System.Drawing.Point(613, 31);
		this.txt_User.Name = "txt_User";
		this.txt_User.Properties.ReadOnly = true;
		this.txt_User.SettingInVisible = false;
		this.txt_User.SettingReadonly = false;
		this.txt_User.Size = new System.Drawing.Size(135, 20);
		this.txt_User.TabIndex = 15;
		this.txt_User.ToolTip = "Tài khoản hiện tại";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "User";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(534, 33);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 14;
		this.labelLine2.Utext = "User";
		this.txt_DocEntry.EditControl = null;
		this.txt_DocEntry.FieldName = null;
		this.txt_DocEntry.IsSearchControl = false;
		this.txt_DocEntry.Location = new System.Drawing.Point(92, 5);
		this.txt_DocEntry.Name = "txt_DocEntry";
		this.txt_DocEntry.Properties.DisplayFormat.FormatString = "n0";
		this.txt_DocEntry.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_DocEntry.Properties.EditFormat.FormatString = "n0";
		this.txt_DocEntry.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_DocEntry.Properties.Mask.EditMask = "n0";
		this.txt_DocEntry.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_DocEntry.SettingInVisible = false;
		this.txt_DocEntry.SettingReadonly = false;
		this.txt_DocEntry.Size = new System.Drawing.Size(170, 20);
		this.txt_DocEntry.TabIndex = 11;
		this.txt_DocEntry.ToolTip = "Bắn barcode phiếu giao việc";
		this.txt_DocEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocEntry_KeyDown);
		this.panelControl3.Controls.Add(this.grd_Line);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 60);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(753, 330);
		this.panelControl3.TabIndex = 24;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(753, 425);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oDeliveryConfirm";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DeliveryConfirm";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Delivery Confirm";
		base.Load += new System.EventHandler(this.frm_DeliveryConfirm_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
