using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Retail;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class frm_PosProcessBar : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly List<SimpleButton> _listButtons = new List<SimpleButton>();

	public ucCashIO UcCashIo;

	public DataTable UdoTable = new DataTable();

	private int _defPOS;

	private readonly string _pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\";

	private readonly ReportDocument _cryRpt = new ReportDocument();

	private IContainer components;

	public SimpleButton btn_Clockin;

	public SimpleButton btn_StartingCash;

	public SimpleButton btn_SalesTransactions;

	public SimpleButton btn_Clockout;

	public SimpleButton btn_ZReport;

	private NavigationFrame navigationFrame1;

	private NavigationPage navigationPage1;

	private NavigationPage navigationPage2;

	private GridLookUpEdit luk_POS;

	private RetailGrid retailGrid1;

	private GridColumn gridColumn4;

	private GridColumn gridColumn5;

	private GridColumn gridColumn6;

	public SimpleButton btn_Continue;

	private LabelControlEx labelControlEx1;

	private RetailGrid luk_POSView;

	public new string FormId
	{
		get
		{
			return luk_POSView.FormId;
		}
		set
		{
			luk_POSView.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return luk_POSView.ItemId;
		}
		set
		{
			luk_POSView.ItemId = value;
		}
	}

	public frm_PosProcessBar()
	{
		InitializeComponent();
		OnLoadAsync += frm_PosProcessBar_Load;
		UcCashIo = new ucCashIO();
		btn_Clockin.Click += BtnClockinOnClick;
		btn_StartingCash.Click += BtnStartingCashOnClick;
		btn_SalesTransactions.Click += BtnSalesTransactionsOnClick;
		btn_Clockout.Click += BtnClockoutOnClick;
		btn_ZReport.Click += BtnZReportOnClick;
	}

	private async void BtnClockinOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			Action dialogShown = () =>
			{
				UcCashIo.CashInType = true;
				UcCashIo.IgnoreEmptyAmt = true;
				UcCashIo.ResetValue();
			};
			if (RetailDialog.Show(UcCashIo, dialogShown, null, "Thu tiền") == DialogResult.OK)
			{
				WinCommons._OpenProcessing();
				await SaveClockIn(UcCashIo.DtCashIo);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> SaveUdoPayment(DataTable dtPayment)
	{
		int linkedPayment = 0;
		try
		{
			double num = Function.ParseDouble(dtPayment.Rows[0]["Amount"]);
			if (num > 0.0)
			{
				string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
				DataSet dataSet = new DataSet
				{
					Tables = { UdoTable.Copy() }
				};
				DataTable dataTable = new DataTable(tableName);
				dataTable.Columns.Add("DocEntry", typeof(int));
				dataTable.Columns.Add("Remark", typeof(string));
				dataTable.Columns.Add("U_Total", typeof(double));
				dataTable.Columns.Add("U_Type", typeof(string));
				dataTable.Columns.Add("U_StoreCode", typeof(string));
				dataTable.Columns.Add("U_PosId", typeof(int));
				string value = Function.ToString(dtPayment.Rows[0]["Remarks"]);
				Function.ToString(dtPayment.Rows[0]["Type"]);
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["Remark"] = value;
				dataRow["U_Total"] = ((num >= 0.0) ? num : 0.0);
				dataRow["U_Type"] = "C";
				dataRow["U_StoreCode"] = GlobalData.StoreCode;
				dataRow["U_PosId"] = GlobalData.POSID;
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable);
				HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					Logging.Write("TRACE", httpResult.Message);
					return linkedPayment;
				}
				linkedPayment = Function.ParseInt(httpResult.Content);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return linkedPayment;
	}

	private async Task SaveClockIn(DataTable dtPayment)
	{
		_ = 1;
		try
		{
			int savePaymentRs = await SaveUdoPayment(dtPayment);
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_CLOCK");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["LineId"] = 0;
			dataRow["StoreCode"] = GlobalData.StoreCode;
			dataRow["PosID"] = GlobalData.POSID;
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["ClockType"] = "I";
			dataRow["ClockDate"] = DateTime.Today.ToString("yyyy-MM-dd");
			dataRow["ClockTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			if (savePaymentRs > 0)
			{
				dataRow["CashID"] = savePaymentRs;
			}
			tableStructure.Rows.Add(dataRow);
			string text = await _httpClient.Retail.SaveClock(tableStructure);
			if (string.IsNullOrEmpty(text))
			{
				if (savePaymentRs > 0)
				{
					WinCommons.Success(Enums.MessageCode.None, "Bắt đầu ca thành công!");
					OpenCashDrawer();
				}
				EnableButtons(new List<SimpleButton> { btn_StartingCash, btn_SalesTransactions, btn_Clockout });
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				Logging.Write("TRACE", text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void BtnStartingCashOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			Action dialogShown = () =>
			{
				UcCashIo.CashInType = true;
				UcCashIo.IgnoreEmptyAmt = true;
				UcCashIo.ResetValue();
			};
			if (RetailDialog.Show(UcCashIo, dialogShown, null, "Thu tiền") == DialogResult.OK)
			{
				WinCommons._OpenProcessing();
				await SaveUdoPayment(UcCashIo.DtCashIo);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnSalesTransactionsOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "RetailStore"))
			{
				if (!httpClientHelper.Get("LoadDoc", new
				{
					code = Function.ToString(GlobalData.StoreCode)
				}).IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_GetStoreErr);
					Logging.Write("ERROR", "Lỗi lấy thông tin cửa hàng!");
					return;
				}
			}
			frm_PointOfSales frm_PointOfSales2 = new frm_PointOfSales();
			frm_PointOfSales2.FormBorderStyle = FormBorderStyle.None;
			frm_PointOfSales2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void BtnClockoutOnClick(object sender, EventArgs eventArgs)
	{
		_ = 2;
		try
		{
			if (RetailMessageBox.Show("Bạn có muốn đóng ca làm việc hiện tại?", "Xác nhận kết thúc ca", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_CLOCK");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["LineId"] = 0;
				dataRow["StoreCode"] = GlobalData.StoreCode;
				dataRow["PosID"] = GlobalData.POSID;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["ClockType"] = "O";
				dataRow["ClockDate"] = DateTime.Today.ToString("yyyy-MM-dd");
				dataRow["ClockTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				tableStructure.Rows.Add(dataRow);
				string text = await _httpClient.Retail.SaveClock(tableStructure);
				if (string.IsNullOrEmpty(text))
				{
					await PrintReceipt(GlobalData.StoreCode, GlobalData.POSID);
					EnableButtons(new List<SimpleButton> { btn_Clockin, btn_ZReport });
					await AddArInvoice();
					WinCommons.Success(Enums.MessageCode.None, "Đóng ca thành công!");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Lưu dữ liệu Đóng ca không thành công!");
					Logging.Write("TRACE", text);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddArInvoice()
	{
		_ = 1;
		try
		{
			DataTable entryRows = await _httpClient.Retail.GetSODocEntry(GlobalData.StoreCode, GlobalData.POSID);
			if (!entryRows.IsNotNull())
			{
				return;
			}
			Stopwatch watch = Stopwatch.StartNew();
			for (int i = 0; i < entryRows.Rows.Count; i++)
			{
				string text = Function.ToString(entryRows.Rows[i]["ListDocEntry"]);
				if (!string.IsNullOrEmpty(text))
				{
					await CopySOToInvoice(text);
				}
			}
			watch.Stop();
			long elapsedMilliseconds = watch.ElapsedMilliseconds;
			Logging.Write("TRACE", string.Format("Kết quả copy A/r cho Store: {0} - POS ID: {1} - Ngày: {2} - Thời gian chạy: ", GlobalData.StoreCode, GlobalData.POSID, DateTime.Now.ToString("yyyy-MM-dd HH:mm")) + elapsedMilliseconds / 1000 + "s");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task PrintReceipt(string storeCode, int posID)
	{
		try
		{
			string text = _pathFile + Path.GetFileName("Sales and Taxes Summary.rpt");
			if (!File.Exists(text))
			{
				WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_BillRptNotFound);
				Logging.Write("TRACE", "Rpt file path does not exist!");
				return;
			}
			PrintDialog pd = new PrintDialog
			{
				PrinterSettings = new PrinterSettings()
			};
			_cryRpt.Load(text);
			DataSet dataSet = await _httpClient.Retail.GetSummaryData(storeCode, posID);
			dataSet.Tables[0].TableName = "Command";
			_cryRpt.SetDataSource(dataSet.Tables[0]);
			_cryRpt.Refresh();
			_cryRpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
			_cryRpt.PrintToPrinter(1, collated: false, 0, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task CopySOToInvoice(string listEntry)
	{
		_ = 2;
		try
		{
			int objType = 13;
			List<Document_LinesImpl> lstLineDraft = new List<Document_LinesImpl>();
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient((ApzObjectTypes)objType);
			DocumentTable tableDocDraft = await httpClient.Documents.CopyHeader(ApzObjectTypes.oOrders, (ApzObjectTypes)objType, listEntry);
			DocumentLineTable documentLineTable = await httpClient.Documents.Copy(ApzObjectTypes.oOrders, (ApzObjectTypes)objType, listEntry);
			DocumentsImpl documentsImpl = ((tableDocDraft == null || tableDocDraft.Rows.Count <= 0) ? null : tableDocDraft[0].ToDocument());
			if (documentLineTable != null && documentLineTable.Rows.Count > 0)
			{
				lstLineDraft = documentLineTable.ToDocumentLine();
				lstLineDraft = lstLineDraft.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
			}
			if (documentsImpl != null)
			{
				documentsImpl.Lines = lstLineDraft;
				documentsImpl.DocObjectCode = (ApzObjectTypes)objType;
				documentsImpl.ObjType = objType;
				HttpResult httpResult = await httpClient.Documents.SaveDocument(documentsImpl);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					Logging.Write("ERROR", httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.None, Resources.Msg_Success + " Copy To AR Invoice " + httpResult.Content);
					Logging.Write("TRACE", Resources.Msg_Success + " Copy To AR Invoice " + httpResult.Content);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnZReportOnClick(object sender, EventArgs eventArgs)
	{
	}

	private async Task frm_PosProcessBar_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			UdoTable = await GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "R_Paid"
			});
			_listButtons.Add(btn_Clockin);
			_listButtons.Add(btn_StartingCash);
			_listButtons.Add(btn_SalesTransactions);
			_listButtons.Add(btn_Clockout);
			_listButtons.Add(btn_ZReport);
			await LoadInitForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.PosMasterClient.GetUserPosAssignment(GlobalData.UserCodeLogin);
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ToString(t["Checked"]) == "Y"
				select t;
			DataTable dataTable2 = dataTable.Clone();
			if (source.Any())
			{
				dataTable2 = source.CopyToDataTable();
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["IsDflt"]) == "Y");
				if (dataRow != null)
				{
					_defPOS = Function.ParseInt(dataRow["PosID"]);
				}
			}
			if (dataTable2.IsNotNull())
			{
				luk_POS.Properties.ValueMember = "PosID";
				luk_POS.Properties.DisplayMember = "PosCode";
				luk_POS.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				luk_POS.Properties.DataSource = dataTable2;
				luk_POS.Properties.PopulateViewColumns();
				luk_POS.Properties.View.Columns["Store"].GroupIndex = 0;
				luk_POS.Properties.View.Columns["Checked"].Visible = false;
				luk_POS.Properties.View.Columns["IsDflt"].Visible = false;
				luk_POS.Properties.View.Columns["Dscription"].Visible = false;
				luk_POS.EditValue = ((_defPOS != 0) ? _defPOS : Function.ParseInt(dataTable2.Rows[0]["PosID"]));
				if (dataTable2.Rows.Count == 1)
				{
					await NavigateToProcess();
				}
			}
			else
			{
				btn_Continue.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetUdoByCode(Common.Table_Code item)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			dt = await helper.PostAsync("GetUdoByCode", item);
			if (dt == null)
			{
				return new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	public void OpenCashDrawer()
	{
		try
		{
			byte[] source = new byte[5] { 27, 112, 0, 25, 250 };
			IntPtr intPtr = Marshal.AllocCoTaskMem(5);
			PrintDialog printDialog = new PrintDialog
			{
				PrinterSettings = new PrinterSettings()
			};
			Marshal.Copy(source, 0, intPtr, 5);
			if (!RawPrinterHelper.SendBytesToPrinter(printDialog.PrinterSettings.PrinterName, intPtr, 5))
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_OpenDrawerErr);
			}
			Marshal.FreeCoTaskMem(intPtr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableButtons(List<SimpleButton> listEnable = null)
	{
		try
		{
			foreach (SimpleButton listButton in _listButtons)
			{
				listButton.Enabled = listEnable?.Contains(listButton) ?? false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Continue_Click(object sender, EventArgs e)
	{
		try
		{
			await NavigateToProcess();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task NavigateToProcess()
	{
		try
		{
			if (luk_POS.GetSelectedDataRow() is DataRowView dataRowView)
			{
				GlobalData.StoreCode = Function.ToString(dataRowView.Row["Store"]);
				GlobalData.POSID = Function.ParseInt(dataRowView.Row["PosID"]);
			}
			navigationFrame1.SelectedPage = navigationPage2;
			string storeCode = GlobalData.StoreCode;
			int pOSID = GlobalData.POSID;
			DataTable dataTable = await _httpClient.Retail.CheckClock(storeCode, pOSID);
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Rows[0];
				string text = Function.ToString(dataRow["ClockIn"]);
				string text2 = Function.ToString(dataRow["ClockOut"]);
				if (text == "N")
				{
					EnableButtons(new List<SimpleButton> { btn_Clockin });
				}
				else if (!(text2 == "N"))
				{
					if (text2 == "Y")
					{
						EnableButtons(new List<SimpleButton> { btn_Clockin, btn_ZReport });
					}
				}
				else
				{
					EnableButtons(new List<SimpleButton> { btn_StartingCash, btn_SalesTransactions, btn_Clockout });
				}
			}
			else
			{
				EnableButtons(new List<SimpleButton>());
				WinCommons.Error(Enums.MessageCode.None, "Lỗi lấy dữ liệu! Vui lòng liên hệ kỹ thuật viên!");
				Logging.Write("Can not get Clock Data!");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_POSView_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int rowHandle = e.RowHandle;
			if (Function.ParseInt(Function.ParseInt(luk_POSView.GetRowCellValue(rowHandle, "PosID"))) == _defPOS && _defPOS != 0)
			{
				e.Appearance.FontStyleDelta = FontStyle.Bold;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.frm_PosProcessBar));
		this.btn_Clockin = new DevExpress.XtraEditors.SimpleButton();
		this.btn_StartingCash = new DevExpress.XtraEditors.SimpleButton();
		this.btn_SalesTransactions = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Clockout = new DevExpress.XtraEditors.SimpleButton();
		this.btn_ZReport = new DevExpress.XtraEditors.SimpleButton();
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_Continue = new DevExpress.XtraEditors.SimpleButton();
		this.luk_POS = new DevExpress.XtraEditors.GridLookUpEdit();
		this.luk_POSView = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.navigationPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_POS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_POSView).BeginInit();
		this.navigationPage2.SuspendLayout();
		base.SuspendLayout();
		this.btn_Clockin.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.btn_Clockin.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_Clockin.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Clockin.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Clockin.Appearance.Options.UseBackColor = true;
		this.btn_Clockin.Appearance.Options.UseFont = true;
		this.btn_Clockin.Appearance.Options.UseForeColor = true;
		this.btn_Clockin.AppearanceDisabled.BackColor = System.Drawing.Color.White;
		this.btn_Clockin.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Clockin.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clockin.ImageOptions.Image");
		this.btn_Clockin.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_Clockin.Location = new System.Drawing.Point(119, 237);
		this.btn_Clockin.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Clockin.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Clockin.Name = "btn_Clockin";
		this.btn_Clockin.Size = new System.Drawing.Size(197, 78);
		this.btn_Clockin.TabIndex = 0;
		this.btn_Clockin.Tag = "0";
		this.btn_Clockin.Text = "Mở ca";
		this.btn_StartingCash.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.btn_StartingCash.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_StartingCash.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_StartingCash.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_StartingCash.Appearance.Options.UseBackColor = true;
		this.btn_StartingCash.Appearance.Options.UseFont = true;
		this.btn_StartingCash.Appearance.Options.UseForeColor = true;
		this.btn_StartingCash.Appearance.Options.UseTextOptions = true;
		this.btn_StartingCash.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.btn_StartingCash.AppearanceDisabled.BackColor = System.Drawing.SystemColors.Control;
		this.btn_StartingCash.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_StartingCash.Enabled = false;
		this.btn_StartingCash.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_StartingCash.ImageOptions.Image");
		this.btn_StartingCash.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_StartingCash.Location = new System.Drawing.Point(324, 237);
		this.btn_StartingCash.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_StartingCash.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_StartingCash.Name = "btn_StartingCash";
		this.btn_StartingCash.Size = new System.Drawing.Size(197, 78);
		this.btn_StartingCash.TabIndex = 1;
		this.btn_StartingCash.Tag = "1";
		this.btn_StartingCash.Text = "Tiền đầu ca";
		this.btn_SalesTransactions.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.btn_SalesTransactions.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_SalesTransactions.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_SalesTransactions.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_SalesTransactions.Appearance.Options.UseBackColor = true;
		this.btn_SalesTransactions.Appearance.Options.UseFont = true;
		this.btn_SalesTransactions.Appearance.Options.UseForeColor = true;
		this.btn_SalesTransactions.AppearanceDisabled.BackColor = System.Drawing.SystemColors.Control;
		this.btn_SalesTransactions.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_SalesTransactions.Enabled = false;
		this.btn_SalesTransactions.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SalesTransactions.ImageOptions.Image");
		this.btn_SalesTransactions.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_SalesTransactions.Location = new System.Drawing.Point(529, 237);
		this.btn_SalesTransactions.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_SalesTransactions.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_SalesTransactions.Name = "btn_SalesTransactions";
		this.btn_SalesTransactions.Size = new System.Drawing.Size(197, 78);
		this.btn_SalesTransactions.TabIndex = 2;
		this.btn_SalesTransactions.Tag = "2";
		this.btn_SalesTransactions.Text = "Bán hàng";
		this.btn_Clockout.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.btn_Clockout.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_Clockout.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Clockout.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Clockout.Appearance.Options.UseBackColor = true;
		this.btn_Clockout.Appearance.Options.UseFont = true;
		this.btn_Clockout.Appearance.Options.UseForeColor = true;
		this.btn_Clockout.AppearanceDisabled.BackColor = System.Drawing.SystemColors.Control;
		this.btn_Clockout.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Clockout.Enabled = false;
		this.btn_Clockout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clockout.ImageOptions.Image");
		this.btn_Clockout.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_Clockout.Location = new System.Drawing.Point(734, 237);
		this.btn_Clockout.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Clockout.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Clockout.Name = "btn_Clockout";
		this.btn_Clockout.Size = new System.Drawing.Size(197, 78);
		this.btn_Clockout.TabIndex = 3;
		this.btn_Clockout.Tag = "3";
		this.btn_Clockout.Text = "Đóng ca";
		this.btn_ZReport.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.btn_ZReport.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_ZReport.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_ZReport.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_ZReport.Appearance.Options.UseBackColor = true;
		this.btn_ZReport.Appearance.Options.UseFont = true;
		this.btn_ZReport.Appearance.Options.UseForeColor = true;
		this.btn_ZReport.Appearance.Options.UseTextOptions = true;
		this.btn_ZReport.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.btn_ZReport.AppearanceDisabled.BackColor = System.Drawing.SystemColors.Control;
		this.btn_ZReport.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_ZReport.Enabled = false;
		this.btn_ZReport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ZReport.ImageOptions.Image");
		this.btn_ZReport.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_ZReport.Location = new System.Drawing.Point(939, 237);
		this.btn_ZReport.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_ZReport.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_ZReport.Name = "btn_ZReport";
		this.btn_ZReport.Size = new System.Drawing.Size(197, 78);
		this.btn_ZReport.TabIndex = 4;
		this.btn_ZReport.Tag = "5";
		this.btn_ZReport.Text = "Chốt ca (Z-Report)";
		this.btn_ZReport.Visible = false;
		this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.navigationFrame1.Controls.Add(this.navigationPage1);
		this.navigationFrame1.Controls.Add(this.navigationPage2);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.navigationPage1, this.navigationPage2 });
		this.navigationFrame1.SelectedPage = this.navigationPage1;
		this.navigationFrame1.Size = new System.Drawing.Size(1202, 512);
		this.navigationFrame1.TabIndex = 5;
		this.navigationFrame1.Text = "navigationFrame1";
		this.navigationPage1.Controls.Add(this.labelControlEx1);
		this.navigationPage1.Controls.Add(this.btn_Continue);
		this.navigationPage1.Controls.Add(this.luk_POS);
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(1202, 512);
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(390, 228);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(108, 18);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 59;
		this.labelControlEx1.Text = "Chọn máy POS";
		this.btn_Continue.Appearance.BackColor = System.Drawing.Color.FromArgb(239, 179, 105);
		this.btn_Continue.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Continue.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Continue.Appearance.Options.UseBackColor = true;
		this.btn_Continue.Appearance.Options.UseFont = true;
		this.btn_Continue.Appearance.Options.UseForeColor = true;
		this.btn_Continue.Appearance.Options.UseTextOptions = true;
		this.btn_Continue.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.btn_Continue.AppearanceDisabled.BackColor = System.Drawing.SystemColors.Control;
		this.btn_Continue.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Continue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Continue.ImageOptions.Image");
		this.btn_Continue.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.btn_Continue.Location = new System.Drawing.Point(581, 261);
		this.btn_Continue.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Continue.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Continue.Name = "btn_Continue";
		this.btn_Continue.Size = new System.Drawing.Size(130, 37);
		this.btn_Continue.TabIndex = 57;
		this.btn_Continue.Tag = "5";
		this.btn_Continue.Text = "Tiếp tục";
		this.btn_Continue.Click += new System.EventHandler(this.btn_Continue_Click);
		this.luk_POS.Location = new System.Drawing.Point(390, 261);
		this.luk_POS.Name = "luk_POS";
		this.luk_POS.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 17.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.luk_POS.Properties.Appearance.Options.UseFont = true;
		this.luk_POS.Properties.Appearance.Options.UseTextOptions = true;
		this.luk_POS.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.luk_POS.Properties.NullText = "";
		this.luk_POS.Properties.View = this.luk_POSView;
		this.luk_POS.Properties.ViewType = DevExpress.XtraEditors.Repository.GridLookUpViewType.GridView;
		this.luk_POS.Size = new System.Drawing.Size(171, 34);
		this.luk_POS.TabIndex = 55;
		this.luk_POSView.ColumnKey = null;
		this.luk_POSView.ColumnPanelRowHeight = 25;
		this.luk_POSView.DisableFormSetting = false;
		this.luk_POSView.DisplayException = false;
		this.luk_POSView.EditColumn = null;
		this.luk_POSView.FieldCurrency = null;
		this.luk_POSView.FieldRate = null;
		this.luk_POSView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.luk_POSView.FormId = "oPosProcessBar";
		this.luk_POSView.IndexEditRow = -1;
		this.luk_POSView.IndicatorWidth = 70;
		this.luk_POSView.IsInsertNewRow = false;
		this.luk_POSView.IsLoadingData = false;
		this.luk_POSView.ItemId = "ODATA";
		this.luk_POSView.LstColumNotSetting = null;
		this.luk_POSView.Name = "luk_POSView";
		this.luk_POSView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.luk_POSView.OptionsView.ShowGroupPanel = false;
		this.luk_POSView.OptionsView.ShowIndicator = false;
		this.luk_POSView.RowHeight = 60;
		this.luk_POSView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.luk_POSView_RowCellStyle);
		this.navigationPage2.Controls.Add(this.btn_ZReport);
		this.navigationPage2.Controls.Add(this.btn_Clockin);
		this.navigationPage2.Controls.Add(this.btn_Clockout);
		this.navigationPage2.Controls.Add(this.btn_StartingCash);
		this.navigationPage2.Controls.Add(this.btn_SalesTransactions);
		this.navigationPage2.Name = "navigationPage2";
		this.navigationPage2.Size = new System.Drawing.Size(1202, 512);
		this.gridColumn4.Caption = "Code";
		this.gridColumn4.FieldName = "Code";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 0;
		this.gridColumn5.Caption = "Name";
		this.gridColumn5.FieldName = "Name";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn6.Caption = "Rate";
		this.gridColumn6.FieldName = "Rate";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 1;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1202, 512);
		base.Controls.Add(this.navigationFrame1);
		base.Name = "frm_PosProcessBar";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Process";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.navigationPage1.ResumeLayout(false);
		this.navigationPage1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_POS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_POSView).EndInit();
		this.navigationPage2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
