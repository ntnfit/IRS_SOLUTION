using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls.POS_UI;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using SAPbobsCOM;

namespace Apzon.Forms.Retail;

public class frm_PointOfSales : XtraFormEx
{
	private ucStatusBar _ucStatusBar;

	private ucGridCanvas _ucGridCanvas;

	private ucPaymentTotal _ucPaymentTotal;

	private ucBarcode _ucBarcode;

	private ucPowQuantity _ucPowQuantity;

	private ucDisplayScreen _ucDisplayScreen;

	private ucCommandPanel _ucCommandPanel;

	private ucNumPad _ucNumPad;

	private ButtonEditEx _btnBarCode;

	private GridControlEx _retailGrid;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oOrders);

	private const string FormController = "Retail";

	private const int MsgInterval = 100;

	public int WidgetEntry = -100;

	private DataTable _dtStructHeader;

	private DataTable _dtStructContents;

	private DataTable _dtStructPayments;

	private DataTable _dtStructCredit;

	private DataTable _dtStore;

	private DataTable _dtPayments;

	private string _cardCode = "";

	private string _cardName = "";

	private string _address = "";

	private string _tel = "";

	private string _email = "";

	private double _cusBalance;

	private string _storeCode = "";

	private double _invoiceTotal;

	private double _vatPrcnt;

	private double _subTotal;

	private double _invoiceDiscount;

	private double _discPrcnt;

	private double _vatSum;

	private string _vatAllGroup;

	private string _orderCmt;

	private bool _cellediting;

	private bool _isChanging;

	private bool _forceExit;

	private double _totalPaidAmount;

	private double _balanceDue;

	private string _paymentRemarks;

	private string _lastItemCode = "";

	public ucCashPay UcCash;

	public ucEditOrder UcEditOrder;

	public ucEditLine UcEditLine;

	public ucScanInvoice UcScanInvoice;

	public ucPaymentDialog UcPaymentDialog;

	public ucRecall UcRecall;

	public ucMemberBarcode UcMemberBarcode;

	public ucMemberDetail UcMemberDetail;

	public ucRefund UcRefund;

	public ucCashIO UcCashIo;

	public ucPromotions UcPromotions;

	public DataTable UdoTable = new DataTable();

	public DataRow[] PromotionItems;

	public static bool CloseCFL;

	private readonly string _pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\";

	private readonly ReportDocument _cryRpt = new ReportDocument();

	private CalcEdit _ceCatchResult;

	public Action<Tuple<double, double, double, double>> SetTotalText;

	private string _refundEntry;

	protected Enums.POSViewType _viewMode;

	protected Enums.POSActionType _actionType;

	private int _newDocEntry;

	private int _maxDocEntry;

	private int _draftKey = -1;

	private DataTable _dtSeries;

	private readonly SerialPort _sp = new SerialPort();

	private bool _isPortOk;

	private string _amountFormat;

	private string _greeting = "";

	private readonly List<Keys> _listKeys = new List<Keys>
	{
		Keys.NumPad0,
		Keys.NumPad1,
		Keys.NumPad2,
		Keys.NumPad3,
		Keys.NumPad4,
		Keys.NumPad5,
		Keys.NumPad6,
		Keys.NumPad7,
		Keys.NumPad8,
		Keys.NumPad9,
		Keys.Delete,
		Keys.Subtract,
		Keys.Decimal
	};

	private readonly Hashtable _tableRefund = new Hashtable();

	private readonly Hashtable _tablePromoItem = new Hashtable();

	private readonly Hashtable _tablePromoDisc = new Hashtable();

	private bool _preventInput;

	private IContainer components;

	private NavigationFrame nframe_Main;

	private NavigationPage npage_Main;

	private NavigationPage npage_Sub;

	private ucWidgetLayout ucWidgetLayout2;

	public Timer TimerStaticStatusBar;

	private ucStatusMessage ucStatusMessage1;

	public Enums.POSViewType ViewMode
	{
		get
		{
			return _viewMode;
		}
		set
		{
			_viewMode = value;
			if (value == Enums.POSViewType.MainView)
			{
				_ucCommandPanel?.NavigateToMainCmd();
				_ucDisplayScreen?.NavigateToItemScan();
				_ucGridCanvas?.NavigateToGrid();
				_btnBarCode?.Focus();
			}
		}
	}

	public Enums.POSActionType ActionType
	{
		get
		{
			return _actionType;
		}
		set
		{
			_actionType = value;
			_btnBarCode?.Focus();
		}
	}

	public frm_PointOfSales()
	{
		InitializeComponent();
		Closing += OnClosing;
		ucWidgetLayout2.widgetView1.QueryControl += widgetView1_QueryControl;
		ucWidgetLayout2.widgetView1.ControlShown += widgetView1_ControlShown;
		UcCash = new ucCashPay();
		UcEditOrder = new ucEditOrder();
		UcEditLine = new ucEditLine();
		UcScanInvoice = new ucScanInvoice();
		UcPaymentDialog = new ucPaymentDialog();
		UcRecall = new ucRecall();
		UcRecall.FormId = (UcPaymentDialog.FormId = "oPointOfSales");
		UcMemberBarcode = new ucMemberBarcode();
		UcMemberDetail = new ucMemberDetail();
		UcRefund = new ucRefund();
		UcCashIo = new ucCashIO();
		UcPromotions = new ucPromotions();
		UcPromotions.FormId = "oPointOfSales";
		OnLoadAsync += frm_PointOfSales_Load;
	}

	private async Task frm_PointOfSales_Load(object sender, EventArgs e)
	{
		try
		{
			ucWidgetLayout2.HideCaption();
			UdoTable = await GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "R_Paid"
			});
			EventStaticStatusBar();
			string currency = GlobalData.MainCurrency;
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			_amountFormat = new string('#', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
			_amountFormat = "#,0." + _amountFormat;
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
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "UDO");
			dt = await helper.PostAsync("GetUdoByCode", item);
			if (dt == null)
			{
				return new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private async void widgetView1_ControlShown(object sender, DevExpress.XtraBars.Docking2010.Views.DeferredControlLoadEventArgs e)
	{
		try
		{
			if (sender is WidgetView widgetView && e.Document == widgetView.Documents.Last())
			{
				await InitData();
				_btnBarCode?.Focus();
				SetPoleText(_greeting);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BackToMain()
	{
		try
		{
			nframe_Main.SelectedPage = npage_Main;
			ViewMode = Enums.POSViewType.MainView;
			CloseCFL = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool SetHotKey(TileItem item, Keys keyData)
	{
		try
		{
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(frm_Dialog))
				{
					return false;
				}
			}
			if (string.IsNullOrEmpty(Function.ToString(item.Tag)) || !item.Visible)
			{
				return false;
			}
			if (keyData == (Keys)Enum.Parse(typeof(Keys), Function.ToString(item.Tag).Replace("+", ",")) && (ActionType == Enums.POSActionType.AddItem || item.Name == "tileItem_RemoveItem"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return false;
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		try
		{
			ucMainCmd ucMainCmd2 = _ucCommandPanel.ucMainCmd1;
			if (ucMainCmd2 == null)
			{
				return false;
			}
			if (_ucCommandPanel.InMainScrn)
			{
				TileControl tile_Commands = ucMainCmd2.tile_Commands;
				if (tile_Commands != null)
				{
					for (int i = 0; i < tile_Commands.Groups.Count; i++)
					{
						foreach (TileItem item in tile_Commands.Groups[i].Items)
						{
							if (SetHotKey(item, keyData))
							{
								item.PerformItemClick();
								return true;
							}
						}
					}
				}
			}
			ucNumPad ucNumPad2 = _ucNumPad;
			if (ucNumPad2 == null)
			{
				return false;
			}
			if (_listKeys.Contains(keyData))
			{
				_retailGrid.Focus();
				ucNumPad2.PressButton(keyData);
				_btnBarCode?.Focus();
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return false;
	}

	private void widgetView1_QueryControl(object sender, DevExpress.XtraBars.Docking2010.Views.QueryControlEventArgs e)
	{
		RegisEvent(e.Control);
	}

	private void RegisEvent(Control control)
	{
		try
		{
			if (control is ucGridCanvas ucGridCanvas2)
			{
				_ucGridCanvas = ucGridCanvas2;
				_retailGrid = ucGridCanvas2.ucRetailGrid1.grd_Contents;
				GridViewEx retailView = _retailGrid.DefaultView as GridViewEx;
				if (retailView != null)
				{
					retailView.FocusedRowChanged += delegate
					{
						ResetNumPad();
					};
					retailView.CellValueChanged += GridviewOnCellValueChanged;
					ucGridCanvas2.ucRetailGrid1.rep_Qty.ButtonClick += (object sender, ButtonPressedEventArgs e) =>
					{
						try
						{
							double num = Function.ParseDouble(retailView.GetFocusedRowCellValue("Quantity"));
							if (e.Button.Index == 0)
							{
								retailView.SetFocusedRowCellValue("Quantity", num + 1.0);
							}
							else
							{
								retailView.SetFocusedRowCellValue("Quantity", num - 1.0);
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
						}
					};
					ucGridCanvas2.ucRetailGrid1.rep_Edit.ButtonClick += async (object sender, ButtonPressedEventArgs e) =>
					{
						try
						{
							if (e.Button.IsLeft)
							{
								EditLineProcess(retailView);
							}
							else
							{
								string itemCode = Function.ToString(retailView.GetFocusedRowCellValue("ItemCode"));
								await AddItem("", itemCode, Enums.POSActionType.RemoveItem);
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
						}
					};
				}
				TileView tileView = _retailGrid.MainView as TileView;
				if (tileView != null)
				{
					GridColumn gridColumn = new GridColumn();
					gridColumn.FieldName = "FromRefund";
					gridColumn.UnboundType = UnboundColumnType.Boolean;
					tileView.Columns.Add(gridColumn);
					GridColumn gridColumn2 = new GridColumn();
					gridColumn2.FieldName = "FromPromotionItem";
					gridColumn2.UnboundType = UnboundColumnType.Boolean;
					tileView.Columns.Add(gridColumn2);
					GridColumn gridColumn3 = new GridColumn();
					gridColumn3.FieldName = "FromPromotionDisc";
					gridColumn3.UnboundType = UnboundColumnType.Boolean;
					tileView.Columns.Add(gridColumn3);
					tileView.ItemClick += (object sender, TileViewItemClickEventArgs args) =>
					{
						try
						{
							ResetNumPad();
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
						}
						finally
						{
							_btnBarCode?.Focus();
						}
					};
					tileView.CellValueChanged += GridviewOnCellValueChanged;
					tileView.ItemDoubleClick += (object sender, TileViewItemClickEventArgs args) =>
					{
						try
						{
							EditLineProcess(tileView);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
						}
						finally
						{
							_btnBarCode?.Focus();
						}
					};
					tileView.FocusedRowChanged += delegate
					{
						ResetNumPad();
					};
					tileView.ContextButtonClick += (object sender, ContextItemClickEventArgs e) =>
					{
						try
						{
							if (e.Item.Name == "ctxButton_Remove")
							{
								tileView.OptionsBehavior.KeepFocusedRowOnUpdate = false;
								DataTable dataTable = _retailGrid.DataSource as DataTable;
								int num = Function.ParseInt(((TileViewItem)e.DataItem).RowHandle);
								tileView.DeleteRow(num);
								dataTable.AcceptChanges();
								if (!dataTable.IsNotNull())
								{
									_ucDisplayScreen.NavigateToItemScan("No Items", 1.0);
									_lastItemCode = "No Items";
									ResetNumPad();
								}
								if (tileView.FocusedRowHandle == num)
								{
									ResetNumPad();
								}
								ReCalcData(dataTable);
								tileView.OptionsBehavior.KeepFocusedRowOnUpdate = true;
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
						}
						finally
						{
							_btnBarCode?.Focus();
						}
					};
					tileView.ItemCustomize += (object sender, TileViewItemCustomizeEventArgs e) =>
					{
						if (Function.ParseBool(tileView.GetRowCellValue(e.RowHandle, "FromRefund")))
						{
							e.Item.AppearanceItem.Normal.BackColor = Color.DarkSalmon;
						}
						else if (Function.ParseBool(Function.ParseBool(tileView.GetRowCellValue(e.RowHandle, "FromPromotionItem")) || Function.ParseBool(tileView.GetRowCellValue(e.RowHandle, "FromPromotionDisc"))))
						{
							Font font = new Font(new FontFamily("Tahoma"), 14f, FontStyle.Italic, GraphicsUnit.Pixel);
							e.Item.AppearanceItem.Normal.Font = font;
							e.Item.AppearanceItem.Normal.ForeColor = Color.Purple;
							for (int i = 0; i < e.Item.Elements.Count; i++)
							{
								e.Item.Elements[i].Appearance.Normal.Font = font;
							}
						}
					};
					tileView.ContextButtonCustomize += (object sender, TileViewContextButtonCustomizeEventArgs e) =>
					{
						if (Function.ParseBool(Function.ParseBool(tileView.GetRowCellValue(e.RowHandle, "FromPromotionItem")) || Function.ParseBool(tileView.GetRowCellValue(e.RowHandle, "FromPromotionDisc"))))
						{
							if (e.Item.Name == "ctxButton_Remove")
							{
								e.Item.Visibility = ContextItemVisibility.Hidden;
							}
							else
							{
								e.Item.Visibility = ContextItemVisibility.Visible;
							}
						}
					};
					tileView.CustomUnboundColumnData += OnCustomUnbound;
				}
			}
			if (control is ucCommandPanel ucCommandPanel2)
			{
				_ucCommandPanel = ucCommandPanel2;
			}
			if (control is ucPaymentTotal ucPaymentTotal2)
			{
				_ucPaymentTotal = ucPaymentTotal2;
				_ucPaymentTotal.lblInvoiceTotal.DoubleClick += delegate
				{
					try
					{
						EditOrderProcess();
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", ex2);
						WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
					}
					finally
					{
						_btnBarCode?.Focus();
					}
				};
			}
			if (control is ucBarcode ucBarcode2)
			{
				_ucBarcode = ucBarcode2;
				_btnBarCode = ucBarcode2.btnBarCode;
				_btnBarCode.KeyDown += BtnBarCodeOnKeyDown;
				_btnBarCode.KeyPress += (object sender, KeyPressEventArgs e) =>
				{
					e.Handled = _preventInput;
				};
				_btnBarCode.ButtonClick += async (object sender, ButtonPressedEventArgs args) =>
				{
					CloseMessage();
					if (!string.IsNullOrEmpty(_btnBarCode.Text))
					{
						string barcode = Function.ToString(_btnBarCode.Text);
						await AddItem(barcode, "", ActionType);
						_btnBarCode.Text = string.Empty;
						_btnBarCode.Enabled = true;
						_btnBarCode.Focus();
						if (_retailGrid.DefaultView is GridViewEx gridViewEx)
						{
							int num = gridViewEx.LocateByValue("ItemCode", barcode);
							if (num != int.MinValue)
							{
								gridViewEx.FocusedRowHandle = num;
								gridViewEx.MakeRowVisible(num);
							}
						}
					}
				};
			}
			if (control is ucPowQuantity ucPowQuantity2)
			{
				_ucPowQuantity = ucPowQuantity2;
				ucPowQuantity2.SetQty = (Func<double, bool>)Delegate.Combine(ucPowQuantity2.SetQty, new Func<double, bool>(SetRowItemQty));
			}
			if (control is ucStatusBar ucStatusBar2)
			{
				_ucStatusBar = ucStatusBar2;
			}
			if (control is ucNumPad ucNumPad2)
			{
				_ucNumPad = ucNumPad2;
				_ceCatchResult = new CalcEditEx();
				_ceCatchResult.EditValueChanging += (object sender, ChangingEventArgs e) =>
				{
					if (!_isChanging)
					{
						e.Cancel = !SetRowItemQty(Function.ParseDouble(e.NewValue));
					}
				};
				_ceCatchResult.EditValueChanged += (object sender, EventArgs args) =>
				{
					_btnBarCode?.Focus();
				};
				_ucNumPad.ClientControl = _ceCatchResult;
			}
			if (control is ucDisplayScreen ucDisplayScreen2)
			{
				_ucDisplayScreen = ucDisplayScreen2;
			}
			_ = control is ucPromotions;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetNumPad()
	{
		try
		{
			_isChanging = true;
			_ucNumPad.btn_Cancel_Click(null, null);
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EditOrderProcess()
	{
		try
		{
			DataTable dataTable = _retailGrid.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				UcEditOrder.Total = _invoiceTotal;
				UcEditOrder.SubTotal = _subTotal;
				UcEditOrder.VatPrcnt = _vatPrcnt;
				UcEditOrder.Discount = _invoiceDiscount;
				UcEditOrder.DiscPrcnt = _discPrcnt;
				UcEditOrder.VatSum = _vatSum;
				UcEditOrder.VatGroup = _vatAllGroup;
				UcEditOrder.Comments = _orderCmt;
				UcEditOrder.DtLines = dataTable.Copy();
				Action dialogShown = () =>
				{
					UcEditOrder.SetValues();
				};
				if (RetailDialog.Show(UcEditOrder, dialogShown, null, "Sửa thông tin đơn hàng") != DialogResult.OK)
				{
					return;
				}
				_invoiceDiscount = UcEditOrder.Discount;
				_discPrcnt = UcEditOrder.DiscPrcnt;
				_vatPrcnt = UcEditOrder.VatPrcnt;
				_invoiceTotal = UcEditOrder.Total;
				_vatSum = UcEditOrder.VatSum;
				_vatAllGroup = UcEditOrder.VatGroup;
				_orderCmt = UcEditOrder.Comments;
				if (!string.IsNullOrEmpty(_vatAllGroup))
				{
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						DataRow dataRow = dataTable.Rows[num];
						dataRow["VatPrcnt"] = _vatPrcnt;
						dataRow["VatGroup"] = _vatAllGroup;
						double num2 = Function.ParseDouble(dataRow["Quantity"]);
						double num3 = Function.ParseDouble(dataRow["PriceBefDi"]);
						double num4 = Function.ParseDouble(dataRow["DiscPrcnt"]);
						double num5 = num2 * num3;
						double num6 = num5 - num5 * num4 / 100.0;
						dataRow["LineVat"] = num6 * _vatPrcnt / 100.0 * (1.0 - _discPrcnt / 100.0);
						double num7 = CalcLineTotal(num2, num3, num4);
						dataRow["LineTotal"] = num7;
					}
				}
				else if (_discPrcnt >= 0.0)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						double num8 = Function.ParseDouble(row["LineTotal"]);
						double num9 = Function.ParseDouble(row["VatPrcnt"]);
						row["LineVat"] = num8 * num9 / 100.0 * (1.0 - _discPrcnt / 100.0);
					}
					dataTable.AcceptChanges();
				}
				_ucPaymentTotal?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
				ResetNumPad();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EditLineProcess(dynamic view)
	{
		try
		{
			DataTable dataTable = _retailGrid.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
				return;
			}
			dynamic val = view.FocusedRowHandle;
			if (Function.ParseBool(view.GetRowCellValue(val, "FromPromotionDisc")))
			{
				return;
			}
			UcEditLine.DrLine = dataTable.NewRow();
			dynamic dataRow = view.GetDataRow(val);
			UcEditLine.DrLine.ItemArray = dataRow.ItemArray.Clone() as object[];
			UcEditLine.IsPromotionItem = Function.ParseBool(view.GetRowCellValue(val, "FromPromotionItem"));
			Action dialogShown = () =>
			{
				UcEditLine.SetValues();
			};
			if (RetailDialog.Show(UcEditLine, dialogShown, null, "Sửa thông tin mặt hàng") != DialogResult.OK)
			{
				return;
			}
			if (UcEditLine.DrLine != null)
			{
				view.SetRowCellValue(val, "PriceBefDi", UcEditLine.DrLine["PriceBefDi"]);
				view.SetRowCellValue(val, "VatPrcnt", UcEditLine.DrLine["VatPrcnt"]);
				view.SetRowCellValue(val, "DiscPrcnt", UcEditLine.DrLine["DiscPrcnt"]);
				view.SetRowCellValue(val, "VatGroup", UcEditLine.DrLine["VatGroup"]);
				view.SetRowCellValue(val, "FreeTxt", UcEditLine.DrLine["FreeTxt"]);
				dynamic val2 = Function.ParseBool(view.GetRowCellValue(val, "FromRefund"));
				double num = Function.ParseDouble(UcEditLine.DrLine["Quantity"]);
				if (num > 0.0 && val2)
				{
					num *= -1.0;
				}
				SetRowItemQty(num);
			}
			else
			{
				Logging.Write("TRACE", "Can not get values from UcEditLine!");
			}
			ResetNumPad();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OnCustomUnbound(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			dynamic mainView = _retailGrid.MainView;
			if (e.Column.FieldName == "FromRefund")
			{
				if (e.IsSetData)
				{
					if (_tableRefund.ContainsKey(e.Row))
					{
						_tableRefund[e.Row] = e.Value;
					}
					else
					{
						_tableRefund.Add(e.Row, e.Value);
					}
				}
				if (e.IsGetData && _tableRefund.ContainsKey(e.Row))
				{
					e.Value = (bool)_tableRefund[e.Row];
				}
			}
			if (e.Column.FieldName == "FromPromotionItem")
			{
				if (e.IsSetData)
				{
					if (_tablePromoItem.ContainsKey(e.Row))
					{
						_tablePromoItem[e.Row] = e.Value;
					}
					else
					{
						_tablePromoItem.Add(e.Row, e.Value);
					}
				}
				if (e.IsGetData && _tablePromoItem.ContainsKey(e.Row))
				{
					e.Value = (bool)_tablePromoItem[e.Row];
				}
			}
			if (e.Column.FieldName == "FromPromotionDisc")
			{
				if (e.IsSetData)
				{
					if (_tablePromoDisc.ContainsKey(e.Row))
					{
						_tablePromoDisc[e.Row] = e.Value;
					}
					else
					{
						_tablePromoDisc.Add(e.Row, e.Value);
					}
				}
				if (e.IsGetData && _tablePromoDisc.ContainsKey(e.Row))
				{
					e.Value = (bool)_tablePromoDisc[e.Row];
				}
			}
			if (e.Column.FieldName == "Total")
			{
				int listSourceRowIndex = e.ListSourceRowIndex;
				dynamic val = Function.ParseDouble(mainView.GetListSourceRowCellValue(listSourceRowIndex, "LineTotal"));
				dynamic val2 = Function.ParseDouble(mainView.GetListSourceRowCellValue(listSourceRowIndex, "LineVat"));
				e.Value = (object)(val + val2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private double CalcLineTotalAfterTax(double lineTotal, double vat)
	{
		try
		{
			double num = lineTotal * vat / 100.0 * (1.0 - _discPrcnt / 100.0);
			return lineTotal + num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0.0;
	}

	private void TileRemoveItemClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if (!(_retailGrid.DataSource as DataTable).IsNotNull() && ActionType == Enums.POSActionType.AddItem)
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
				return;
			}
			if (sender is TileItem tileItem)
			{
				tileItem.Checked = !tileItem.Checked;
				TileControl tile_Commands = _ucCommandPanel.ucMainCmd1.tile_Commands;
				if (tile_Commands != null)
				{
					for (int i = 0; i < tile_Commands.Groups.Count; i++)
					{
						TileGroup tileGroup = tile_Commands.Groups[i];
						for (int j = 0; j < tileGroup.Items.Count; j++)
						{
							TileItem tileItem2 = tileGroup.Items[j];
							if (tileItem2 != tileItem)
							{
								tileItem2.Enabled = !tileItem.Checked;
							}
						}
					}
				}
				ActionType = (tileItem.Checked ? Enums.POSActionType.RemoveItem : Enums.POSActionType.AddItem);
			}
			_btnBarCode?.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private void TileReturnClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		_btnBarCode?.Focus();
	}

	private async void TilePauseClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if ((_retailGrid.DataSource as DataTable).IsNotNull())
			{
				if (RetailMessageBox.Show(PoinOfSale.msg_Hold, "Xác nhận tạm dừng", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					await SaveData(isDraft: true);
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TilePaymentClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if ((_retailGrid.DataSource as DataTable).IsNotNull())
			{
				SetPoleText("Tong tien " + _invoiceTotal.ToString(_amountFormat).PadRight(10));
				Action dialogShown = () =>
				{
					UcPaymentDialog.SetValues(_invoiceTotal);
				};
				if (RetailDialog.Show(UcPaymentDialog, dialogShown, null, "Thanh toán") == DialogResult.OK)
				{
					_dtPayments = UcPaymentDialog.DtPayments.Copy();
					_dtPayments.AsEnumerable().ToDictionary((DataRow row) => row.Field<string>(0), (DataRow row) => row.Field<double>(1));
					_balanceDue = UcPaymentDialog.ChangeDue;
					_paymentRemarks = UcPaymentDialog.Remarks;
					SetPoleText("Tong tien " + _invoiceTotal.ToString(_amountFormat).PadLeft(10), "Tra khach " + _balanceDue.ToString(_amountFormat).PadLeft(10));
					await SaveData();
					ResetNumPad();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TilePromotionsClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if ((_retailGrid.DataSource as DataTable).IsNotNull())
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = new DataTable();
				DataTable dataTable2 = new DataTable();
				dataTable.Columns.Add("CardCode", typeof(string));
				dataTable.Columns.Add("CardName", typeof(string));
				dataTable.Columns.Add("DocTotal", typeof(double));
				dataTable.Columns.Add("VatTotal", typeof(double));
				dataTable.Columns.Add("DocDate", typeof(DateTime));
				dataTable.Columns.Add("DocDueDate", typeof(DateTime));
				dataTable.Columns.Add("NumAtCard", typeof(string));
				dataTable.Columns.Add("Comments", typeof(string));
				dataTable.Columns.Add("U_maCTKM", typeof(string));
				dataTable.Columns.Add("U_salesEmpCode", typeof(string));
				dataTable.Columns.Add("U_maNhom", typeof(string));
				dataTable.Columns.Add("U_tenNhom", typeof(string));
				dataTable.Columns.Add("U_DMSSONo", typeof(string));
				dataTable2.Columns.Add("ItemCode", typeof(string));
				dataTable2.Columns.Add("Dscription", typeof(string));
				dataTable2.Columns.Add("Quantity", typeof(double));
				dataTable2.Columns.Add("UnitMsr", typeof(string));
				dataTable2.Columns.Add("Price", typeof(double));
				dataTable2.Columns.Add("Rate", typeof(double));
				dataTable2.Columns.Add("WhsCode", typeof(string));
				dataTable2.Columns.Add("U_DMSSONo", typeof(string));
				DocumentsImpl documents = GetDocuments();
				if (documents != null)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["CardCode"] = documents.CardCode;
					dataRow["CardName"] = documents.CardName;
					dataRow["DocTotal"] = documents.DocTotal;
					dataRow["VatTotal"] = documents.VatSum;
					dataRow["DocDate"] = documents.DocDate;
					dataRow["DocDueDate"] = documents.DocDueDate;
					dataRow["NumAtCard"] = "test";
					dataRow["Comments"] = documents.Comments;
					dataRow["U_maCTKM"] = "";
					dataRow["U_salesEmpCode"] = "";
					dataRow["U_maNhom"] = "";
					dataRow["U_tenNhom"] = "";
					dataRow["U_DMSSONo"] = "test";
					dataTable.Rows.Add(dataRow);
					foreach (Document_LinesImpl line in documents.Lines)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2["ItemCode"] = line.ItemCode;
						dataRow2["Dscription"] = line.Dscription;
						dataRow2["Quantity"] = line.Quantity;
						dataRow2["UnitMsr"] = line.UomCode;
						dataRow2["Price"] = line.PriceBefDi;
						dataRow2["Rate"] = line.Rate;
						dataRow2["WhsCode"] = line.WhsCode;
						dataRow2["U_DMSSONo"] = "test";
						dataTable2.Rows.Add(dataRow2);
					}
				}
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
				WinCommons._OpenProcessing();
				DataTable dtPromo = await _httpClient.Retail.GetPromotionsData(dataSet);
				Action dialogShown = () =>
				{
					UcPromotions.SetDataSource(dtPromo);
				};
				WinCommons._CloseProcessing();
				if (RetailDialog.Show(UcPromotions, dialogShown, null, "Chương trình khuyến mãi") == DialogResult.OK)
				{
					ResetNumPad();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			_btnBarCode?.Focus();
		}
	}

	private void ClearPromotion()
	{
		try
		{
			if (!(_retailGrid.MainView is TileView tileView) || !(_retailGrid.DataSource is DataTable dataTable))
			{
				return;
			}
			List<DataRow> list = new List<DataRow>();
			List<DataRow> list2 = new List<DataRow>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow item = dataTable.Rows[i];
				int rowHandle = tileView.GetRowHandle(i);
				object rowCellValue = tileView.GetRowCellValue(rowHandle, "FromPromotionItem");
				object rowCellValue2 = tileView.GetRowCellValue(rowHandle, "FromPromotionDisc");
				if (Function.ParseBool(rowCellValue2))
				{
					list2.Add(item);
				}
				if (Function.ParseBool(rowCellValue) || Function.ParseBool(rowCellValue2))
				{
					list.Add(item);
				}
			}
			foreach (DataRow item2 in list)
			{
				dataTable.Rows.Remove(item2);
			}
			if (list2.Count > 0)
			{
				RecalcOder(dataTable, 0.0);
			}
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddItemsDiscount(DataRow[] obj)
	{
	}

	private void AddSoDiscount(DataRow[] rows)
	{
		try
		{
			if (rows != null && rows.Length != 0)
			{
				if (!(_retailGrid.MainView is TileView tileView) || !(_retailGrid.DataSource is DataTable dataTable))
				{
					return;
				}
				int num = 0;
				if (num < rows.Length)
				{
					DataRow dataRow = rows[num];
					DataRow dataRow2 = dataTable.NewRow();
					double num2 = Function.ParseDouble(dataRow["Price"]);
					Function.ParseDouble(dataRow["SL"]);
					dataRow2["LineNum"] = -1;
					dataRow2["ItemCode"] = Function.ToString(dataRow["MaHang"]);
					dataRow2["Dscription"] = "             " + Function.ToString(dataRow["TenHang"]);
					dataRow2["PriceBefDi"] = num2;
					dataTable.Rows.InsertAt(dataRow2, 0);
					dataTable.AcceptChanges();
					tileView.SetRowCellValue(tileView.GetRowHandle(0), "FromPromotionDisc", true);
					double num3 = Function.ParseDouble(dataRow["ChietKhau"]);
					if (num3 >= 0.0)
					{
						RecalcOder(dataTable, num3);
					}
					else
					{
						Logging.Write("TRACE", "Apply promotion (type SO Discount) Failed: invalid discount value " + _discPrcnt);
					}
				}
			}
			BackToMain();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RecalcOder(DataTable dt, double disc)
	{
		try
		{
			_discPrcnt = disc;
			foreach (DataRow row in dt.Rows)
			{
				double num = Function.ParseDouble(row["LineTotal"]);
				double num2 = Function.ParseDouble(row["VatPrcnt"]);
				row["LineVat"] = num * num2 / 100.0 * (1.0 - _discPrcnt / 100.0);
			}
			dt.AcceptChanges();
			double value = dt.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineVat"]));
			string currency = GlobalData.MainCurrency;
			int num3 = 0;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num3 = currencyCodesImpl.Decimals;
				}
			}
			_vatSum = Math.Round(value, (num3 >= 0) ? num3 : 0);
			_subTotal = dt.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
			_invoiceDiscount = Math.Round(_subTotal * _discPrcnt / 100.0, (num3 >= 0) ? num3 : 0);
			_invoiceTotal = _subTotal + _vatSum - _invoiceDiscount;
			_ucPaymentTotal?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void AddPromotionItems(DataRow[] rows)
	{
		try
		{
			if (rows != null && rows.Length != 0)
			{
				if (!(_retailGrid.MainView is TileView tileView) || !(_retailGrid.DataSource is DataTable dataTable))
				{
					return;
				}
				PromotionItems = rows;
				foreach (DataRow dataRow in rows)
				{
					DataRow dataRow2 = dataTable.NewRow();
					double num = Function.ParseDouble(dataRow["Price"]);
					int num2 = 0;
					int num3 = 0;
					double num4 = Function.ParseDouble(dataRow["SL"]);
					dataRow2["LineNum"] = -1;
					dataRow2["ItemCode"] = Function.ToString(dataRow["MaHang"]);
					dataRow2["Dscription"] = "             " + Function.ToString(dataRow["TenHang"]) + " (Hàng KM)";
					dataRow2["Quantity"] = num4;
					dataRow2["PriceBefDi"] = num;
					dataRow2["DiscPrcnt"] = num2;
					dataRow2["VatPrcnt"] = num3;
					dataRow2["VatGroup"] = "";
					double num5 = num4 * num;
					double num6 = num5 - num5 * (double)num2 / 100.0;
					dataRow2["LineVat"] = num6 * (double)num3 / 100.0 * (1.0 - _discPrcnt / 100.0);
					double num7 = CalcLineTotal(1.0, num, num2);
					dataRow2["LineTotal"] = num7;
					dataRow2["CodeBars"] = "";
					dataTable.Rows.InsertAt(dataRow2, 0);
					dataTable.AcceptChanges();
					tileView.SetRowCellValue(tileView.GetRowHandle(0), "FromPromotionItem", true);
				}
			}
			BackToMain();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void TileRefundClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		_ = 1;
		try
		{
			CloseMessage();
			if (VoidTransConfirm())
			{
				await ResetValue();
				Action dialogShown = () =>
				{
				};
				if (RetailDialog.Show(UcScanInvoice, dialogShown, null, "Nhập mã hóa đơn") == DialogResult.OK)
				{
					await LoadOrder(UcScanInvoice.InvNo);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async Task LoadOrder(string docEntry)
	{
		_ = 1;
		try
		{
			DataSet dataSet = await _httpClient.Retail.GetRefundLines(Function.ParseInt(docEntry));
			if (!dataSet.IsNotNull() || dataSet.Tables.Count < 2)
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_DocEntNotExist);
				return;
			}
			DataTable dtDoc = dataSet.Tables[0];
			DataTable dt = dataSet.Tables[1];
			if (!dtDoc.IsNotNull() || !dt.IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoRefundLines);
				return;
			}
			UcRefund.CardCode = Function.ToString(dtDoc.Rows[0]["CardCode"]);
			UcRefund.SetDataSource(dataSet);
			if (RetailDialog.Show(UcRefund, null, null, "Chọn hàng đổi trả") == DialogResult.OK)
			{
				if (UcRefund.DtRefund.IsNotNull())
				{
					_discPrcnt = Function.ParseDouble(dtDoc.Rows[0]["DiscPrcnt"]);
					await GetRefundRows(UcRefund.DtRefund);
					_refundEntry = docEntry;
					_cardCode = Function.ToString(dtDoc.Rows[0]["CardCode"]);
					_cardName = Function.ToString(dtDoc.Rows[0]["CardName"]);
					_address = Function.ToString(dtDoc.Rows[0]["Address"]);
					_email = Function.ToString(dtDoc.Rows[0]["Email"]);
					_ucStatusBar.lblCustomer.Text = "Khách hàng: " + _cardName;
				}
				ResetNumPad();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRefundRows(DataTable dtFund)
	{
		try
		{
			_retailGrid.DataSource = dtFund.Copy();
			DataTable dt = _retailGrid.DataSource as DataTable;
			if (_retailGrid.MainView is TileView tileView)
			{
				for (int i = 0; i < tileView.RowCount; i++)
				{
					tileView.SetRowCellValue(i, "FromRefund", true);
				}
			}
			ReCalcData(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> LoadDrafts()
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ObjType", typeof(string));
			dataTable.Columns.Add("UserSign", typeof(string));
			dataTable.Columns.Add("DocStatus", typeof(string));
			dataTable.Columns.Add("Date", typeof(string));
			dataTable.Columns.Add("TypeDate", typeof(string));
			dataTable.Columns.Add("From", typeof(string));
			dataTable.Columns.Add("To", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			List<string> list = new List<string>();
			list.Add("17");
			if (list.Count > 0)
			{
				dataRow["ObjType"] = string.Join(",", list.ToArray());
			}
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["DocStatus"] = "1";
			dataRow["TypeDate"] = 2;
			dataRow["From"] = DateTime.Today.ToString("yyyy-MM-dd");
			dataRow["To"] = DateTime.Today.ToString("yyyy-MM-dd");
			dataRow["Date"] = 1;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			return await Search(dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return new DataTable();
	}

	private async Task<DataTable> Search(DataTable dt)
	{
		DataTable dtObject = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "DocumentDraft");
			dtObject = await helper.PostAsync("SearchDoc", dt);
			if (dtObject != null && dtObject.Rows.Count > 0)
			{
				dtObject.PrimaryKey = new DataColumn[1] { dtObject.Columns["DocEntry"] };
				return dtObject;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dtObject;
	}

	private async void TileCancelClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if (VoidTransConfirm())
			{
				await ResetValue();
				WinCommons.Success(Enums.MessageCode.None, PoinOfSale.msg_VoidTransSuc);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private void TileScanCustomerClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			if (RetailDialog.Show(UcMemberBarcode, null, null, "Quét mã thành viên") == DialogResult.OK)
			{
				ShowMemberDetail(UcMemberBarcode.MemRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private bool VoidTransConfirm()
	{
		try
		{
			if ((_retailGrid.DataSource as DataTable).IsNotNull())
			{
				if (RetailMessageBox.Show(PoinOfSale.msg_VoidCurrentTrans, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2, MessageBoxIcon.Question) == DialogResult.No)
				{
					return false;
				}
				if (_totalPaidAmount > 0.0)
				{
					WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_VoidTransErr);
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private async void TileItemRecallOnItemClick(object sender1, TileItemEventArgs tileItemEventArgs)
	{
		_ = 2;
		try
		{
			CloseMessage();
			if (!VoidTransConfirm())
			{
				return;
			}
			await ResetValue();
			Action dialogShown = async () =>
			{
				DataTable dataTable4 = await LoadDrafts();
				EnumerableRowCollection<DataRow> source = from t in dataTable4.AsEnumerable()
					where Function.ToString(t["U_RetailStore"]) == GlobalData.StoreCode && Function.ParseInt(t["U_PosId"]) == GlobalData.POSID
					select t;
				UcRecall.grd_Doc.DataSource = (source.Any() ? source.CopyToDataTable() : dataTable4.Clone());
			};
			if (RetailDialog.Show(UcRecall, dialogShown, null, "Chọn đơn hàng") != DialogResult.OK)
			{
				return;
			}
			DataTable dataTable = new DataTable();
			int selectedDocEntry = UcRecall.SelectedDocEntry;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "DocumentDraft"))
			{
				dataTable = await helper.GetAsync("GetDraftByEntry", new
				{
					docEntry = selectedDocEntry
				});
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["DocEntry"] };
				}
			}
			if (dataTable.IsNotNull())
			{
				DataRow docRow = dataTable.Rows[0];
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("DocEntry", typeof(string));
				int selectedDocEntry2 = Function.ParseInt(docRow["DocEntry"]);
				dataTable2.Rows.Add(selectedDocEntry2);
				using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "DocumentDraft");
				DataTable dataTable3 = await helper.PostAsync("SearchLine", dataTable2);
				if (dataTable3.IsNotNull())
				{
					_retailGrid.DataSource = dataTable3;
					_draftKey = selectedDocEntry2;
					_cardCode = Function.ToString(docRow["CardCode"]);
					_cardName = Function.ToString(docRow["CardName"]);
					_vatSum = Function.ParseDouble(docRow["VatSum"]);
					_vatPrcnt = Function.ParseDouble(docRow["VatPercent"]);
					_discPrcnt = Function.ParseDouble(docRow["DiscPrcnt"]);
					_invoiceDiscount = Function.ParseDouble(docRow["DiscSum"]);
					_address = Function.ToString(docRow["Address"]);
					_invoiceTotal = Function.ParseDouble(docRow["DocTotal"]);
					_totalPaidAmount = Function.ParseDouble(docRow["PaidSum"]);
					_email = Function.ToString(docRow["Email"]);
					_orderCmt = Function.ToString(docRow["Comments"]);
					_ucStatusBar.lblCustomer.Text = "Khách hàng: " + _cardName;
					DataTable dt = _retailGrid.DataSource as DataTable;
					ReCalcData(dt);
				}
				else
				{
					_retailGrid.DataSource = _dtStructContents.Clone();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TileItemCashPayOnItemClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			if ((_retailGrid.DataSource as DataTable).IsNotNull())
			{
				Action dialogShown = () =>
				{
					UcCash.SetValues(_invoiceTotal);
				};
				if (RetailDialog.Show(UcCash, dialogShown, null, "Thanh toán nhanh bằng tiền mặt") == DialogResult.OK)
				{
					_dtPayments.Rows.Clear();
					_dtPayments.Rows.Add("1", UcCash.PaidAmount);
					_balanceDue = UcCash.ChangeDue;
					await SaveData();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_NoItemInList);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private void ShowMemberDetail(DataRow row)
	{
		try
		{
			UcMemberDetail.AddCustomerToTable(row);
			if (RetailDialog.Show(UcMemberDetail, null, null, "Thông tin thành viên") == DialogResult.OK)
			{
				if (row != null)
				{
					ChooseMember(row);
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_MemberNotChosen);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void TileItemEditLineOnItemClick(object o, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			DevExpress.XtraGrid.Views.Base.BaseView mainView = _retailGrid.MainView;
			EditLineProcess(mainView);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private void TileItemShutDownOnItemClick(object o, TileItemEventArgs tileItemEventArgs)
	{
		Close();
	}

	private void TileItemOpenDrawerOnItemClick(object o, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			OpenCashDrawer();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private void TileItemEditOrderOnItemClick(object o, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			EditOrderProcess();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TileItemCashOutOnItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			CloseMessage();
			Action dialogShown = () =>
			{
				UcCashIo.CashInType = false;
				UcCashIo.ResetValue();
			};
			if (RetailDialog.Show(UcCashIo, dialogShown, null, "Chi tiền khác") == DialogResult.OK)
			{
				await SaveUDO(UcCashIo.DtCashIo);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TileItemCashInOnItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			CloseMessage();
			Action dialogShown = () =>
			{
				UcCashIo.CashInType = true;
				UcCashIo.ResetValue();
			};
			if (RetailDialog.Show(UcCashIo, dialogShown, null, "Thu tiền khác") == DialogResult.OK)
			{
				await SaveUDO(UcCashIo.DtCashIo);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async void TileItemReprintOnItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			CloseMessage();
			Action dialogShown = () =>
			{
			};
			if (RetailDialog.Show(UcScanInvoice, dialogShown, null, "Nhập mã hóa đơn cần in lại") == DialogResult.OK)
			{
				await PrintReceipt(Function.ParseInt(UcScanInvoice.InvNo), 2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	private async Task SaveUDO(DataTable dt)
	{
		_ = 1;
		try
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
			string value = Function.ToString(dt.Rows[0]["Remarks"]);
			double num = Function.ParseDouble(dt.Rows[0]["Amount"]);
			string value2 = Function.ToString(dt.Rows[0]["Type"]);
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["Remark"] = value;
			dataRow["U_Total"] = ((num >= 0.0) ? num : 0.0);
			dataRow["U_Type"] = value2;
			dataRow["U_StoreCode"] = GlobalData.StoreCode;
			dataRow["U_PosId"] = GlobalData.POSID;
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				Logging.Write("TRACE", httpResult.Message);
			}
			else
			{
				OpenCashDrawer();
				await ResetValue();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void TileItemEndShiftOnItemClick(object o, TileItemEventArgs e)
	{
	}

	private void TileItemSignOutOnItemClick(object o, TileItemEventArgs e)
	{
		try
		{
			CloseMessage();
			if (RetailMessageBox.Show(PoinOfSale.msg_SignOutConfirm, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			_forceExit = true;
			Close();
			_forceExit = false;
			Main main = null;
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(Main))
				{
					main = openForm as Main;
				}
			}
			main?.navChangeUser_LinkClicked(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void TileItemExitOnItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			if (RetailMessageBox.Show(PoinOfSale.msg_ExitConfirm, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(Main))
				{
					Main obj = openForm as Main;
					_forceExit = true;
					Main.Exit = true;
					Main.IsRestart = true;
					obj?.Close();
					_forceExit = false;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GridviewOnCellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_cellediting && (!(e.Column.FieldName != "Quantity") || !(e.Column.FieldName != "VatPrcnt") || !(e.Column.FieldName != "DiscPrcnt") || !(e.Column.FieldName != "PriceBefDi")))
			{
				CellValueChanged(e.RowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CellValueChanged(int rowHandle)
	{
		try
		{
			dynamic val = _retailGrid.DefaultView as GridViewEx;
			if (val == null)
			{
				val = _retailGrid.MainView as TileView;
				if (val == null)
				{
					return;
				}
			}
			dynamic val2 = Function.ParseDouble(val.GetRowCellValue(rowHandle, "PriceBefDi"));
			dynamic val3 = Function.ParseDouble(val.GetRowCellValue(rowHandle, "VatPrcnt"));
			dynamic val4 = Function.ParseDouble(val.GetRowCellValue(rowHandle, "DiscPrcnt"));
			dynamic val5 = Function.ParseDouble(val.GetRowCellValue(rowHandle, "Quantity"));
			DataTable dataTable = _retailGrid.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				_cellediting = true;
				dynamic val6 = val5 * val2;
				dynamic val7 = val6 - val6 * val4 / 100;
				val.SetRowCellValue(rowHandle, "LineVat", val7 * val3 / 100 * (1.0 - _discPrcnt / 100.0));
				dynamic val8 = CalcLineTotal(val5, val2, val4);
				val.SetRowCellValue(rowHandle, "LineTotal", val8);
				val.RefreshData();
				dynamic val9 = Function.ToString(val.GetRowCellValue(rowHandle, "Dscription"));
				SetPoleText(val9.PadRight(20), " X " + val5.ToString().PadLeft(3) + " = " + val8.ToString(_amountFormat).PadLeft(11));
				dataTable.AcceptChanges();
				ReCalcData(dataTable);
				dynamic val10 = Function.ToString(val.GetRowCellValue(rowHandle, "ItemCode"));
				if (val10.Equals(_lastItemCode))
				{
					_ucDisplayScreen.NavigateToItemScan(val9, val5);
				}
				_cellediting = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool SetRowItemQty(double qty)
	{
		try
		{
			dynamic mainView = _retailGrid.MainView;
			if (mainView != null && mainView.RowCount > 0)
			{
				dynamic val = Function.ParseBool(mainView.GetFocusedRowCellValue("FromRefund"));
				dynamic val2 = Function.ParseBool(mainView.GetFocusedRowCellValue("FromPromotionDisc"));
				dynamic val3 = Function.ParseBool(mainView.GetFocusedRowCellValue("FromPromotionItem"));
				if (val2)
				{
					return false;
				}
				dynamic currentLineNum = Function.ParseInt(mainView.GetFocusedRowCellValue("LineNum"));
				if (val)
				{
					DataTable dtRefund = UcRefund.DtRefund;
					if (dtRefund.IsNotNull())
					{
						DataRow dataRow = dtRefund.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == currentLineNum);
						if (dataRow != null)
						{
							double num = Function.ParseDouble(dataRow["Quantity"]);
							if (qty >= 0.0)
							{
								if (qty > num * -1.0)
								{
									WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_InvalidRefundQty);
									return false;
								}
								mainView.SetFocusedRowCellValue("Quantity", qty * -1.0);
							}
							else
							{
								if (qty < num)
								{
									WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_InvalidRefundQty);
									return false;
								}
								mainView.SetFocusedRowCellValue("Quantity", qty);
							}
						}
					}
				}
				else if (!(val3 ? true : false))
				{
					mainView.SetFocusedRowCellValue("Quantity", qty);
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
		return false;
	}

	private void TileCustomerSearchClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			UcChooseFromList ucChooseFromList = new UcChooseFromList(Enums.TypeCFL.Customer)
			{
				Dock = DockStyle.Fill
			};
			ucChooseFromList.OnChooseFromListClose += OnChooseCustomer;
			NavigateToPage(ucChooseFromList);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OnChooseCustomer(params DataRow[] chooseItem)
	{
		try
		{
			if (chooseItem != null && chooseItem.Length != 0)
			{
				BackToMain();
				ShowMemberDetail(chooseItem[0]);
			}
			else
			{
				BackToMain();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseMember(DataRow drow)
	{
		try
		{
			if (drow != null)
			{
				_cardCode = Function.ToString(drow["CardCode"]);
				_cardName = Function.ToString(drow["CardName"]);
				_address = Function.ToString(drow["Address"]);
				_tel = Function.ToString(drow["Cellular"]);
				_email = Function.ToString(drow["MailAddres"]);
				_cusBalance = Function.ParseDouble(drow["Balance"]);
				_ucStatusBar.lblCustomer.Text = "Khách hàng: " + Regex.Replace(_cardName, "\\t|\\n|\\r", "");
				SetPoleText("KH: " + _cardName, "Diem t.luy: " + _cusBalance.ToString().PadLeft(8));
			}
			BackToMain();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void TileItemSearchClick(object sender, TileItemEventArgs tileItemEventArgs)
	{
		try
		{
			CloseMessage();
			UcChooseFromList ucChooseFromList = new UcChooseFromList(Enums.TypeCFL.ListItems)
			{
				Dock = DockStyle.Fill,
				CardCode = _cardCode
			};
			ucChooseFromList.OnChooseFromListClose += OnChooseFromListClose;
			NavigateToPage(ucChooseFromList);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CloseMessage()
	{
		RetailMessage.MsgCount = -1;
		RetailMessage.MsgMessage = "";
		RetailMessage.MsgEnable = false;
		ucStatusMessage1.Visible = false;
	}

	private async void OnChooseFromListClose(params DataRow[] chooseItem)
	{
		try
		{
			if (chooseItem != null && chooseItem.Length != 0)
			{
				for (int i = 0; i < chooseItem.Length; i++)
				{
					string itemCode = Function.ToString(chooseItem[i]["ItemCode"]);
					await AddItem("", itemCode, ActionType);
				}
			}
			BackToMain();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void BtnBarCodeOnKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			CloseMessage();
			if (_listKeys.Contains(e.KeyData))
			{
				_preventInput = true;
				return;
			}
			_preventInput = false;
			if (_btnBarCode == null || e.KeyCode != Keys.Return || string.IsNullOrEmpty(_btnBarCode.Text))
			{
				return;
			}
			string barCode = Function.ToString(_btnBarCode.Text);
			await AddItem(barCode, "", ActionType);
			_btnBarCode.Text = string.Empty;
			_btnBarCode.Enabled = true;
			_btnBarCode.Focus();
			if (_retailGrid.DefaultView is GridViewEx gridViewEx)
			{
				int num = gridViewEx.LocateByValue("ItemCode", barCode);
				if (num != int.MinValue)
				{
					gridViewEx.FocusedRowHandle = num;
					gridViewEx.MakeRowVisible(num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddItem(string barcode, string itemCode, Enums.POSActionType actionType = Enums.POSActionType.AddItem)
	{
		_ = 1;
		try
		{
			if (!(_retailGrid.MainView is TileView tileView))
			{
				return;
			}
			if (actionType == Enums.POSActionType.AddItem)
			{
				tileView.OptionsBehavior.KeepFocusedRowOnUpdate = false;
			}
			if (!(_retailGrid.DataSource is DataTable dt))
			{
				return;
			}
			DataTable dtDoc = new DataTable();
			DataSet dataSet = (string.IsNullOrEmpty(barcode) ? (await _httpClient.RetailChooseFromList.ListItems(GlobalData.UserName, GlobalData.StoreCode, itemCode, Function.ToString(_cardCode), "")) : (await _httpClient.RetailChooseFromList.ListItems(GlobalData.UserName, GlobalData.StoreCode, "", Function.ToString(_cardCode), barcode)));
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
			{
				dtDoc = dataSet.Tables[1];
			}
			int num = -1;
			if (dtDoc.IsNotNull())
			{
				itemCode = Function.ToString(dtDoc.Rows[0]["ItemCode"]);
				for (int i = 0; i < tileView.RowCount; i++)
				{
					object rowCellValue = tileView.GetRowCellValue(i, "FromRefund");
					string text = Function.ToString(tileView.GetRowCellValue(i, "ItemCode"));
					if (!Function.ParseBool(rowCellValue) && text.ToUpper() == itemCode.ToUpper())
					{
						num = i;
						break;
					}
				}
				foreach (DataRow row in dtDoc.Rows)
				{
					double num2 = 1.0;
					double num3 = 0.0;
					if (Function.ToString(row["ItemCode"]).ToUpper() == itemCode.ToUpper() && num != -1)
					{
						int num4 = 1;
						if (actionType == Enums.POSActionType.RemoveItem)
						{
							tileView.DeleteRow(num);
							if (tileView.FocusedRowHandle == num)
							{
								ResetNumPad();
							}
						}
						else if (Function.ParseBool(ConfigurationManager.AppSettings["GatherLines"]))
						{
							num2 = Function.ParseDouble(tileView.GetRowCellValue(num, "Quantity")) + (double)num4;
							double num5 = Function.ParseDouble(tileView.GetRowCellValue(num, "PriceBefDi"));
							double num6 = Function.ParseDouble(tileView.GetRowCellValue(num, "DiscPrcnt"));
							double num7 = Function.ParseDouble(tileView.GetRowCellValue(num, "VatPrcnt"));
							double num8 = num2 * num5;
							double num9 = num8 - num8 * num6 / 100.0;
							tileView.SetRowCellValue(num, "LineVat", num9 * num7 / 100.0 * (1.0 - _discPrcnt / 100.0));
							tileView.SetRowCellValue(num, "Quantity", num2);
							num3 = CalcLineTotal(num2, num5, num6);
							tileView.SetRowCellValue(num, "LineTotal", num3);
						}
						else
						{
							DataRow dataRow2 = dt.NewRow();
							double num10 = Function.ParseDouble(row["Price"]);
							double num11 = Function.ParseDouble(row["Discount"]);
							double num12 = Function.ParseDouble(row["RateSa"]);
							dataRow2["LineNum"] = -1;
							dataRow2["ItemCode"] = Function.ToString(row["ItemCode"]);
							dataRow2["Dscription"] = Function.ToString(row["ItemName"]);
							dataRow2["Quantity"] = 1;
							dataRow2["PriceBefDi"] = num10;
							dataRow2["DiscPrcnt"] = num11;
							dataRow2["VatPrcnt"] = num12;
							dataRow2["VatGroup"] = Function.ToString(row["VatGourpSa"]);
							double num13 = num2 * num10;
							double num14 = num13 - num13 * num11 / 100.0;
							dataRow2["LineVat"] = num14 * num12 / 100.0 * (1.0 - _discPrcnt / 100.0);
							num3 = CalcLineTotal(1.0, num10, num11);
							dataRow2["LineTotal"] = num3;
							dataRow2["CodeBars"] = barcode;
							dt.Rows.InsertAt(dataRow2, 0);
							dt.AcceptChanges();
						}
					}
					else
					{
						if (actionType != Enums.POSActionType.AddItem)
						{
							_retailGrid.DataSource = dt;
							_retailGrid.RefreshDataSource();
							ReCalcData(dt);
							WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_ItemCodeNotInList);
							return;
						}
						DataRow dataRow3 = dt.NewRow();
						double num15 = Function.ParseDouble(row["Price"]);
						double num16 = Function.ParseDouble(row["Discount"]);
						double num17 = Function.ParseDouble(row["RateSa"]);
						dataRow3["LineNum"] = -1;
						dataRow3["ItemCode"] = Function.ToString(row["ItemCode"]);
						dataRow3["Dscription"] = Function.ToString(row["ItemName"]);
						dataRow3["Quantity"] = 1;
						dataRow3["PriceBefDi"] = num15;
						dataRow3["DiscPrcnt"] = num16;
						dataRow3["VatPrcnt"] = num17;
						dataRow3["VatGroup"] = Function.ToString(row["VatGourpSa"]);
						double num18 = num2 * num15;
						double num19 = num18 - num18 * num16 / 100.0;
						dataRow3["LineVat"] = num19 * num17 / 100.0 * (1.0 - _discPrcnt / 100.0);
						num3 = CalcLineTotal(1.0, num15, num16);
						dataRow3["LineTotal"] = num3;
						dataRow3["CodeBars"] = barcode;
						dt.Rows.InsertAt(dataRow3, 0);
						dt.AcceptChanges();
					}
					if (!dt.IsNotNull())
					{
						_ucDisplayScreen.NavigateToItemScan("No Items", num2);
						_lastItemCode = "No Items";
						ResetNumPad();
					}
					else
					{
						_ucDisplayScreen.NavigateToItemScan(Function.ToString(row["ItemName"]), num2);
						_lastItemCode = Function.ToString(row["ItemCode"]);
						string text2 = Function.ToString(row["ItemName"]);
						SetPoleText(text2.PadRight(20), " X " + num2.ToString().PadLeft(3) + " = " + num3.ToString(_amountFormat).PadLeft(11));
					}
				}
				_retailGrid.DataSource = dt;
				_retailGrid.RefreshDataSource();
				ReCalcData(dt);
				if (tileView != null)
				{
					tileView.OptionsBehavior.KeepFocusedRowOnUpdate = true;
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, PoinOfSale.msg_ItemCodeNotExist);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_btnBarCode?.Focus();
		}
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
		finally
		{
			_btnBarCode?.Focus();
		}
	}

	public async Task PrintReceipt(int docEntry, int type = 1)
	{
		try
		{
			string text = _pathFile + Path.GetFileName("POS Invoice.rpt");
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
			DataSet dataSet = await _httpClient.Retail.GetReceiptData(docEntry, type);
			dataSet.Tables[0].TableName = "Command";
			_cryRpt.SetDataSource(dataSet.Tables[0]);
			_cryRpt.Refresh();
			_cryRpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
			PrinterSettings printerSettings = new PrinterSettings();
			printerSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("POS", 72, 210);
			PageSettings pageSettings = new PageSettings(printerSettings);
			_cryRpt.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
			_cryRpt.PrintOptions.PrinterName = printerSettings.PrinterName;
			_cryRpt.PrintToPrinter(printerSettings, pageSettings, reformatReportPageSettings: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NavigateToPage(XtraUserControl uc)
	{
		try
		{
			nframe_Main.SelectedPage = npage_Sub;
			npage_Sub.Controls.Clear();
			npage_Sub.Controls.Add(uc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReCalcData(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				_subTotal = 0.0;
				_invoiceDiscount = 0.0;
				_vatSum = 0.0;
				_invoiceTotal = 0.0;
				_ucPaymentTotal?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
				return;
			}
			TileView tileView = _retailGrid.MainView as TileView;
			double num = 0.0;
			if (tileView != null)
			{
				for (int i = 0; i < tileView.RowCount; i++)
				{
					num += Function.ParseDouble(tileView.GetRowCellValue(i, "Total"));
				}
			}
			double num2 = dt.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
			_vatSum = dt.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineVat"]));
			if (_discPrcnt > 0.0)
			{
				_invoiceDiscount = num2 * _discPrcnt / 100.0;
			}
			else
			{
				_invoiceDiscount = _discPrcnt;
			}
			_subTotal = num2;
			_invoiceTotal = _subTotal + _vatSum - _invoiceDiscount;
			_ucPaymentTotal?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetPoleText(string line1, string line2 = "")
	{
		try
		{
			if (string.IsNullOrEmpty(line1) || string.IsNullOrEmpty(Function.ToString(ConfigurationManager.AppSettings["PortName"])) || !_isPortOk)
			{
				return;
			}
			_sp.Open();
			_sp.Write(Convert.ToString('\f'));
			if (string.IsNullOrEmpty(line2))
			{
				if (line1.Length > 40)
				{
					line1 = line1.Substring(0, 40);
				}
				string text = Function.RemoveSign4VietnameseString(line1);
				_sp.Write(text.PadRight(40));
			}
			else
			{
				if (line1.Length > 20)
				{
					line1 = line1.Substring(0, 20);
				}
				if (line1.Length > 20)
				{
					line1 = line1.Substring(0, 20);
				}
				string text2 = Function.RemoveSign4VietnameseString(line1);
				string text3 = Function.RemoveSign4VietnameseString(line2);
				_sp.Write(text2.PadRight(20));
				_sp.Write(text3.PadRight(20));
			}
			_sp.Close();
			_sp.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public double CalcLineTotal(double qty, double price, double disc)
	{
		try
		{
			double num = qty * price;
			return num - num * disc / 100.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0.0;
	}

	private async Task InitData()
	{
		_ = 3;
		try
		{
			DataSet dataSet = await _httpClient.Retail.GetStruct();
			if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
			{
				_dtStructHeader = dataSet.Tables[0];
				_dtStructContents = dataSet.Tables[1];
				_dtStructContents.Clear();
				if (_retailGrid != null)
				{
					_retailGrid.DataSource = _dtStructContents.Copy();
					_retailGrid.RefreshDataSource();
				}
				else
				{
					Logging.Write("ERROR", "Can not assign structrure for grid!");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_GetTableStrucErr);
				Logging.Write("ERROR", "Lỗi lấy cấu trúc dữ liệu!");
			}
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Apzon.HttpClientHelper.Configuration.ApiServer, "RetailStore"))
			{
				_dtStore = httpClientHelper.Get("LoadDoc", new
				{
					code = Function.ToString(GlobalData.StoreCode)
				});
				if (!_dtStore.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_GetStoreErr);
					Logging.Write("ERROR", "Lỗi lấy thông tin cửa hàng!");
				}
			}
			await LoadDefaultCustomer();
			await LoadSeries();
			await GetNewDocEntry();
			_dtPayments = new DataTable();
			_dtPayments.Columns.Add("PayType", typeof(string));
			_dtPayments.Columns.Add("Amount", typeof(double));
			_dtPayments.Columns.Add("Note", typeof(string));
			TileControl tile_Commands = _ucCommandPanel.ucMainCmd1.tile_Commands;
			if (tile_Commands != null)
			{
				for (int i = 0; i < tile_Commands.Groups.Count; i++)
				{
					TileGroup tileGroup = tile_Commands.Groups[i];
					for (int j = 0; j < tileGroup.Items.Count; j++)
					{
						TileItem tileItem = tileGroup.Items[j];
						switch (tileItem.Name.Substring(9, tileItem.Name.Length - 9))
						{
						case "CustomerSearch":
							tileItem.ItemClick += TileCustomerSearchClick;
							break;
						case "ItemSearch":
							tileItem.ItemClick += TileItemSearchClick;
							break;
						case "ScanCustomer":
							tileItem.ItemClick += TileScanCustomerClick;
							break;
						case "Cancel":
							tileItem.ItemClick += TileCancelClick;
							break;
						case "Refund":
							tileItem.ItemClick += TileRefundClick;
							break;
						case "Payment":
							tileItem.ItemClick += TilePaymentClick;
							break;
						case "Promotions":
							tileItem.ItemClick += TilePromotionsClick;
							break;
						case "RemoveItem":
							tileItem.ItemClick += TileRemoveItemClick;
							break;
						case "Hold":
							tileItem.ItemClick += TilePauseClick;
							break;
						case "Recall":
							tileItem.ItemClick += TileItemRecallOnItemClick;
							break;
						case "EditLine":
							tileItem.ItemClick += TileItemEditLineOnItemClick;
							break;
						case "EditOrder":
							tileItem.ItemClick += TileItemEditOrderOnItemClick;
							break;
						case "OpenDrawer":
							tileItem.ItemClick += TileItemOpenDrawerOnItemClick;
							break;
						case "ShutDown":
							tileItem.ItemClick += TileItemShutDownOnItemClick;
							break;
						case "Exit":
							tileItem.ItemClick += TileItemExitOnItemClick;
							break;
						case "SignOut":
							tileItem.ItemClick += TileItemSignOutOnItemClick;
							break;
						case "EndShift":
							tileItem.ItemClick += TileItemEndShiftOnItemClick;
							break;
						case "CashIn":
							tileItem.ItemClick += TileItemCashInOnItemClick;
							break;
						case "CashOut":
							tileItem.ItemClick += TileItemCashOutOnItemClick;
							break;
						case "ReprintBill":
							tileItem.ItemClick += TileItemReprintOnItemClick;
							break;
						}
					}
				}
			}
			string text = Function.ToString(ConfigurationManager.AppSettings["PortName"]);
			if (!string.IsNullOrEmpty(text))
			{
				if (!SerialPort.GetPortNames().Contains(text))
				{
					_isPortOk = false;
				}
				else
				{
					_isPortOk = true;
					_sp.PortName = text;
					_sp.BaudRate = Function.ParseInt(ConfigurationManager.AppSettings["BaudRate"]);
					int value = Function.ParseInt(ConfigurationManager.AppSettings["Parity"]);
					_sp.Parity = (Parity)Enum.ToObject(typeof(Parity), value);
					_sp.DataBits = Function.ParseInt(ConfigurationManager.AppSettings["DataBits"]);
					int value2 = Function.ParseInt(ConfigurationManager.AppSettings["Stopbits"]);
					_sp.StopBits = (StopBits)Enum.ToObject(typeof(StopBits), value2);
					string text2 = Function.ToString(ConfigurationManager.AppSettings["Encoding"]);
					if (!string.IsNullOrEmpty(text2))
					{
						_sp.Encoding = Encoding.GetEncoding(text2);
					}
					_greeting = Function.ToString(ConfigurationManager.AppSettings["POSGreeting"]);
				}
			}
			else
			{
				_isPortOk = false;
				Logging.Write("ERROR", "Chưa thiết lập thông tin Customer Display!");
			}
			ucPromotions ucPromotions2 = UcPromotions;
			ucPromotions2.AddItems = (Action<DataRow[]>)Delegate.Combine(ucPromotions2.AddItems, new Action<DataRow[]>(AddPromotionItems));
			ucPromotions ucPromotions3 = UcPromotions;
			ucPromotions3.AddSoDiscount = (Action<DataRow[]>)Delegate.Combine(ucPromotions3.AddSoDiscount, new Action<DataRow[]>(AddSoDiscount));
			ucPromotions ucPromotions4 = UcPromotions;
			ucPromotions4.AddItemsDiscount = (Action<DataRow[]>)Delegate.Combine(ucPromotions4.AddItemsDiscount, new Action<DataRow[]>(AddItemsDiscount));
			ucPromotions ucPromotions5 = UcPromotions;
			ucPromotions5.ClearPromotion = (Action)Delegate.Combine(ucPromotions5.ClearPromotion, new Action(ClearPromotion));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ResetValue()
	{
		_ = 1;
		try
		{
			ViewMode = Enums.POSViewType.MainView;
			await LoadDefaultCustomer();
			_invoiceTotal = 0.0;
			_vatPrcnt = 0.0;
			_subTotal = 0.0;
			_invoiceDiscount = 0.0;
			_discPrcnt = 0.0;
			_vatAllGroup = "";
			_orderCmt = "";
			_totalPaidAmount = 0.0;
			_balanceDue = 0.0;
			_paymentRemarks = "";
			_ucPaymentTotal?.SetText(new Tuple<double, double, double, double>(0.0, 0.0, 0.0, 0.0));
			_ucDisplayScreen?.NavigateToItemScan("No Items");
			UcCash.ChangeDue = 0.0;
			UcPaymentDialog.ChangeDue = 0.0;
			ResetNumPad();
			await GetNewDocEntry();
			_draftKey = -1;
			_refundEntry = "";
			if (_retailGrid.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
			_btnBarCode?.Focus();
			SetPoleText(_greeting);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadDefaultCustomer()
	{
		try
		{
			if (_dtStore.IsNotNull())
			{
				_cardCode = Function.ToString(_dtStore.Rows[0]["CardCode"]);
				_storeCode = Function.ToString(_dtStore.Rows[0]["Code"]);
			}
			DataSet dataSet = await _httpClient.RetailChooseFromList.Customer(GlobalData.UserName, GlobalData.StoreCode, _cardCode);
			if (dataSet.IsNotNull() && dataSet.Tables.Count >= 1 && dataSet.Tables[1].IsNotNull() && !string.IsNullOrEmpty(_cardCode))
			{
				ChooseMember(dataSet.Tables[1].Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task GetNewDocEntry()
	{
	}

	private async Task LoadSeries()
	{
		try
		{
			_dtSeries = await _httpClient.Documents.LoadSeries(GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetDefautSeries(int bplId)
	{
		try
		{
			if (_dtSeries.IsNotNull())
			{
				DataRow dataRow = _dtSeries.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BPLId"]) == bplId && Function.ParseInt(t["Series"]) != -1);
				if (dataRow != null)
				{
					return Function.ParseInt(dataRow["Series"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	public async Task<string> SaveData(bool isDraft = false)
	{
		_ = 4;
		try
		{
			WinCommons._OpenProcessing();
			int retEntry = 0;
			if (!isDraft)
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
				dataTable.Columns.Add("U_ChangeDue", typeof(double));
				dataTable.Columns.Add("U_StoreCode", typeof(string));
				dataTable.Columns.Add("U_PosId", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["Remark"] = _paymentRemarks;
				_totalPaidAmount = _dtPayments.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Amount"]));
				dataRow["U_Total"] = ((_totalPaidAmount >= 0.0) ? _totalPaidAmount : 0.0);
				dataRow["U_ChangeDue"] = ((_balanceDue >= 0.0) ? _balanceDue : 0.0);
				dataRow["U_StoreCode"] = GlobalData.StoreCode;
				dataRow["U_PosId"] = GlobalData.POSID;
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable);
				DataTable dataTable2 = new DataTable("R_PAID_1");
				dataTable2.Columns.Add("U_PaidType", typeof(string));
				dataTable2.Columns.Add("U_Amt", typeof(double));
				dataTable2.Columns.Add("U_Comments", typeof(string));
				for (int num = 0; num < _dtPayments.Rows.Count; num++)
				{
					DataRow dataRow2 = _dtPayments.Rows[num];
					if (Function.ParseDouble(dataRow2["Amount"]) >= 0.0)
					{
						dataTable2.Rows.Add(dataRow2["PayType"], dataRow2["Amount"], dataRow2["Note"]);
					}
				}
				dataSet.Tables.Add(dataTable2.Copy());
				HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					Logging.Write("TRACE", httpResult.Message);
					return "Error";
				}
				retEntry = Function.ParseInt(httpResult.Content);
			}
			DataTable dtLine = _retailGrid.DataSourceTable();
			if (Function.ParseDecimal(_balanceDue) >= 0m)
			{
				DocumentsImpl documents = GetDocuments(dtLine, retEntry);
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oOrders);
				HttpResult saveDocRs = null;
				string message;
				if (isDraft)
				{
					message = (await httpClient.Documents.SaveAsDraft(documents)).Message;
				}
				else
				{
					saveDocRs = await httpClient.Documents.SaveDocument(documents);
					message = saveDocRs.Message;
				}
				if (string.IsNullOrEmpty(message))
				{
					WinCommons.Success(Enums.MessageCode.None, isDraft ? PoinOfSale.msg_HoldSuccess : PoinOfSale.msg_PaySuccess);
					if (!isDraft && RetailMessageBox.Show((_balanceDue > 0.0) ? (PoinOfSale.msg_ChangeDue + ": " + _balanceDue.ToString(_amountFormat) + "." + PoinOfSale.msg_Print) : PoinOfSale.msg_Print, "Xác nhận in", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						int docEntry = Function.ParseInt(saveDocRs.Content);
						await PrintReceipt(docEntry);
					}
					await ResetValue();
					return string.Empty;
				}
				WinCommons.Error(Enums.MessageCode.None, message);
				Logging.Write("TRACE", "Save SO to Save ERROR: " + message);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			_btnBarCode?.Focus();
		}
		return "Error!";
	}

	private DocumentsImpl GetDocuments(DataTable dtLine = null, int retEntry = 0)
	{
		if (!dtLine.IsNotNull())
		{
			dtLine = _retailGrid.DataSourceTable();
		}
		DocumentsImpl documentsImpl = new DocumentsImpl();
		documentsImpl.Lines = new List<Document_LinesImpl>();
		documentsImpl.DocEntry = 0;
		documentsImpl.DocType = "I";
		documentsImpl.DocStatus = "O";
		documentsImpl.DocStatusName = "Open";
		documentsImpl.GroupNum = -1;
		documentsImpl.ObjType = 17;
		documentsImpl.CardCode = _cardCode;
		documentsImpl.CardName = _cardName;
		documentsImpl.DocDate = DateTime.Today;
		documentsImpl.DocDueDate = DateTime.Today;
		documentsImpl.TaxDate = DateTime.Today;
		documentsImpl.DocCur = GlobalData.MainCurrency;
		documentsImpl.DocRate = 1.0;
		documentsImpl.VatSum = _vatSum;
		documentsImpl.VatPercent = _vatPrcnt;
		documentsImpl.DiscPrcnt = _discPrcnt;
		documentsImpl.TotalDiscount = _invoiceDiscount;
		documentsImpl.BPLId = GlobalData.DefaultBranch;
		documentsImpl.Address = _address;
		documentsImpl.Comments = _orderCmt;
		documentsImpl.DocTotal = _invoiceTotal;
		documentsImpl.PaidSum = ((_totalPaidAmount >= 0.0) ? _totalPaidAmount : 0.0);
		documentsImpl.draftKey = _draftKey;
		documentsImpl.Email = _email;
		if (_dtStore.IsNotNull())
		{
			documentsImpl.SlpCode = Function.ParseInt(_dtStore.Rows[0]["SlpCode"]);
		}
		List<UserFieldsImpl> list = new List<UserFieldsImpl>();
		if (Function.ParseInt(_refundEntry) > 0)
		{
			list.Add(new UserFieldsImpl
			{
				Name = "U_RelatedTyp",
				Value = 17,
				Type = BoFieldTypes.db_Alpha
			});
			list.Add(new UserFieldsImpl
			{
				Name = "U_RelatedEnt",
				Value = _refundEntry,
				Type = BoFieldTypes.db_Numeric
			});
		}
		if (retEntry > 0)
		{
			list.Add(new UserFieldsImpl
			{
				Name = "U_RetailPaidEnt",
				Value = retEntry,
				Type = BoFieldTypes.db_Numeric
			});
		}
		list.Add(new UserFieldsImpl
		{
			Name = "U_RetailStore",
			Value = _storeCode,
			Type = BoFieldTypes.db_Alpha
		});
		list.Add(new UserFieldsImpl
		{
			Name = "U_PosId",
			Value = GlobalData.POSID,
			Type = BoFieldTypes.db_Numeric
		});
		documentsImpl.UserFields = list;
		if (_draftKey != -1)
		{
			documentsImpl.IsDraft = true;
		}
		if (_dtStore.IsNotNull())
		{
			documentsImpl.SlpCode = Function.ParseInt(_dtStore.Rows[0]["SlpCode"]);
		}
		if (dtLine.IsNotNull())
		{
			for (int i = 0; i < dtLine.Rows.Count; i++)
			{
				DataRow dataRow = dtLine.Rows[i];
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
				if (Function.ParseInt(_refundEntry) > 0 && Function.ParseInt(dataRow["LineNum"]) != -1)
				{
					list2.Add(new UserFieldsImpl
					{
						Name = "U_OriginType",
						Value = 17,
						Type = BoFieldTypes.db_Alpha
					});
					list2.Add(new UserFieldsImpl
					{
						Name = "U_OriginCode",
						Value = _refundEntry,
						Type = BoFieldTypes.db_Numeric
					});
					list2.Add(new UserFieldsImpl
					{
						Name = "U_OriginLine",
						Value = Function.ParseInt(dataRow["LineNum"]),
						Type = BoFieldTypes.db_Numeric
					});
				}
				document_LinesImpl.UserFields = list2;
				document_LinesImpl.DocEntry = 0;
				document_LinesImpl.LineNum = i;
				document_LinesImpl.ItemCode = Function.ToString(dataRow["ItemCode"]);
				document_LinesImpl.Dscription = Function.ToString(dataRow["Dscription"]);
				document_LinesImpl.Quantity = Function.ParseDouble(dataRow["Quantity"]);
				document_LinesImpl.PriceBefDi = Function.ParseDouble(dataRow["PriceBefDi"]);
				document_LinesImpl.VatGroup = Function.ToString(dataRow["VatGroup"]);
				document_LinesImpl.TaxCode = Function.ToString(dataRow["TaxCode"]);
				document_LinesImpl.DiscPrcnt = Function.ParseDouble(dataRow["DiscPrcnt"]);
				document_LinesImpl.LineVat = Function.ParseDouble(dataRow["LineVat"]);
				if (dataRow["ShipDate"] is DateTime)
				{
					document_LinesImpl.ShipDate = Function.ParseDateTimes(dataRow["ShipDate"]);
				}
				document_LinesImpl.FreeTxt = Function.ToString(dataRow["FreeTxt"]);
				document_LinesImpl.LineTotal = Function.ParseDouble(dataRow["LineTotal"]);
				if (_dtStore.IsNotNull())
				{
					document_LinesImpl.WhsCode = Function.ToString(_dtStore.Rows[0]["WhsCode"]);
					document_LinesImpl.SlpCode = Function.ParseInt(_dtStore.Rows[0]["SlpCode"]);
				}
				document_LinesImpl.CodeBars = Function.ToString(dataRow["CodeBars"]);
				documentsImpl.Lines.Add(document_LinesImpl);
			}
		}
		return documentsImpl;
	}

	private void frm_PointOfSales_Shown(object sender, EventArgs e)
	{
	}

	private void OnClosing(object sender, CancelEventArgs e)
	{
		try
		{
			if (!_forceExit)
			{
				if (CloseCFL)
				{
					e.Cancel = true;
				}
				else if (RetailMessageBox.Show(PoinOfSale.msg_ShutDown, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EventStaticStatusBar()
	{
		TimerStaticStatusBar.Interval = 100;
		TimerStaticStatusBar.Enabled = true;
		TimerStaticStatusBar.Start();
	}

	private void TimerStaticStatusBar_Tick(object sender, EventArgs e)
	{
		if (!RetailMessage.MsgEnable)
		{
			return;
		}
		if (RetailMessage.MsgCount == 0)
		{
			MessageBoxIcon messageBoxIcon = MessageBoxIcon.None;
			if (RetailMessage.MsgColor == Enums.MessageType.Success)
			{
				messageBoxIcon = MessageBoxIcon.Asterisk;
			}
			else if (RetailMessage.MsgColor == Enums.MessageType.Warning)
			{
				messageBoxIcon = MessageBoxIcon.Exclamation;
			}
			else if (RetailMessage.MsgColor == Enums.MessageType.Error)
			{
				messageBoxIcon = MessageBoxIcon.Hand;
			}
			RetailMessage.MsgCount = 1;
			ucStatusMessage1.MessageBoxIcon = messageBoxIcon;
			ucStatusMessage1.Message = MainMessage.MsgMessage;
			ucStatusMessage1.Visible = true;
		}
		else if (RetailMessage.MsgCount == RetailMessage.MsgCountTimer)
		{
			RetailMessage.MsgCount = -1;
			RetailMessage.MsgMessage = "";
			RetailMessage.MsgEnable = false;
			ucStatusMessage1.Visible = false;
		}
		else
		{
			RetailMessage.MsgCount++;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.frm_PointOfSales));
		this.nframe_Main = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.npage_Main = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucWidgetLayout2 = new Apzon.Forms.UserControls.POS_UI.ucWidgetLayout();
		this.npage_Sub = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.TimerStaticStatusBar = new System.Windows.Forms.Timer(this.components);
		this.ucStatusMessage1 = new Apzon.Forms.UserControls.POS_UI.ucStatusMessage();
		((System.ComponentModel.ISupportInitialize)this.nframe_Main).BeginInit();
		this.nframe_Main.SuspendLayout();
		this.npage_Main.SuspendLayout();
		base.SuspendLayout();
		this.nframe_Main.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.nframe_Main.Controls.Add(this.npage_Main);
		this.nframe_Main.Controls.Add(this.npage_Sub);
		this.nframe_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nframe_Main.Location = new System.Drawing.Point(0, 0);
		this.nframe_Main.Name = "nframe_Main";
		this.nframe_Main.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.npage_Main, this.npage_Sub });
		this.nframe_Main.SelectedPage = this.npage_Main;
		this.nframe_Main.Size = new System.Drawing.Size(1028, 669);
		this.nframe_Main.TabIndex = 0;
		this.nframe_Main.Text = "navigationFrame1";
		this.npage_Main.Caption = "npage_Main";
		this.npage_Main.Controls.Add(this.ucWidgetLayout2);
		this.npage_Main.Name = "npage_Main";
		this.npage_Main.Size = new System.Drawing.Size(1028, 669);
		this.ucWidgetLayout2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucWidgetLayout2.Location = new System.Drawing.Point(0, 0);
		this.ucWidgetLayout2.Name = "ucWidgetLayout2";
		this.ucWidgetLayout2.Size = new System.Drawing.Size(1028, 669);
		this.ucWidgetLayout2.TabIndex = 0;
		this.npage_Sub.Caption = "npage_Sub";
		this.npage_Sub.Name = "npage_Sub";
		this.npage_Sub.Size = new System.Drawing.Size(1028, 669);
		this.TimerStaticStatusBar.Tick += new System.EventHandler(this.TimerStaticStatusBar_Tick);
		this.ucStatusMessage1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucStatusMessage1.Location = new System.Drawing.Point(0, 0);
		this.ucStatusMessage1.Message = "";
		this.ucStatusMessage1.MessageBoxIcon = System.Windows.Forms.MessageBoxIcon.Hand;
		this.ucStatusMessage1.Name = "ucStatusMessage1";
		this.ucStatusMessage1.Size = new System.Drawing.Size(1028, 35);
		this.ucStatusMessage1.TabIndex = 1;
		this.ucStatusMessage1.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1028, 669);
		base.Controls.Add(this.ucStatusMessage1);
		base.Controls.Add(this.nframe_Main);
		base.FormId = "oPointOfSales";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_PointOfSales";
		base.ShowIcon = false;
		this.Text = "Point Of Sales";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.Shown += new System.EventHandler(this.frm_PointOfSales_Shown);
		((System.ComponentModel.ISupportInitialize)this.nframe_Main).EndInit();
		this.nframe_Main.ResumeLayout(false);
		this.npage_Main.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
