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
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_SpecialforBP : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public ShowForms frmShowChildrenForm;

	private string _formController = "SpecialBp";

	private DataSet _dsConfig = new DataSet();

	private bool _CancelCellValueChangeEvent;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private TextEditEx txtCardName;

	private Label lb_BPName;

	private LabelLine lb_DiscountPersentage;

	private LabelLine lb_PriceList;

	private Label lb_BPCode;

	private PanelControl panelControl4;

	private PanelControl panelControl3;

	private Label lb_BPType;

	private GridControlEx grdDoc;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcPrice;

	private GridColumnEx gcDiscount;

	private RepositoryItemLookUpEdit rLU_VatGroup;

	private GridColumnEx gcListNum;

	private GridColumnEx gcAutoUpdt;

	private ButtonEditEx txtCardCode;

	private CalcEditEx txtDiscount;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem viewDToolStripMenuItem;

	private RepositoryItemCalcEdit rCal;

	private RepositoryItemLookUpEdit rLU_PriceList;

	private GridColumnEx gcCurrenCy;

	private GridColumnEx gcExpand;

	private RepositoryItemButtonEdit rBT_ItemCode;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridViewEx gvDoc;

	private LookUpEditEx luk_PriceList;

	private GridColumnEx gc_UoM;

	private GridColumnEx gc_PriCurPrice;

	private GridColumnEx gc_AddCur1Price;

	private GridColumnEx gc_AddCur2Price;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private RepositoryItemTextEditExPrice rPrice;

	private GridColumnEx gcPriceAfDis;

	private RepositoryItemCheckEdit rCheckEdit;

	private LookUpEditEx txtCardType;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

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
		case Enums.FormViewModeType.Add:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			break;
		}
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Add;
	}

	public frm_SpecialforBP()
	{
		InitializeComponent();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Apzon.Commons.Properties.Resources.OSPP_Titel_frm_SpecialforBP;
		lb_BPCode.Text = Apzon.Commons.Properties.Resources.lb_BPCode;
		lb_BPName.Text = Apzon.Commons.Properties.Resources.lb_BPName;
		lb_BPType.Text = Apzon.Commons.Properties.Resources.lb_BPType;
		lb_PriceList.Text = Apzon.Commons.Properties.Resources.lb_PriceList;
		lb_DiscountPersentage.Text = Apzon.Commons.Properties.Resources.lb_DiscountPersentage;
		gcItemCode.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
		gcItemName.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemName;
		gcPrice.Caption = Apzon.Commons.Properties.Resources.OSPP_Price;
		gcDiscount.Caption = Apzon.Commons.Properties.Resources.OSPP_Discount;
		gcListNum.Caption = Apzon.Commons.Properties.Resources.OSPP_ListNum;
		gcAutoUpdt.Caption = Apzon.Commons.Properties.Resources.OSPP_AutoUpdt;
		gcCurrenCy.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcCurrency;
	}

	private async void frm_SpecialforBP_Load(object sender, EventArgs e)
	{
		List<Common.Table_Code> dataSource = new List<Common.Table_Code>
		{
			new Common.Table_Code("C", "Customer"),
			new Common.Table_Code("S", "Vendor"),
			new Common.Table_Code("L", "Lead")
		};
		txtCardType.Properties.DataSource = dataSource;
		await RefreshData();
	}

	private async Task RefreshData()
	{
		txtCardCode.EditValue = string.Empty;
		txtCardName.ReadOnly = true;
		txtCardType.ReadOnly = true;
		txtDiscount.EditValue = 0;
		await LoadCb();
	}

	private async Task Search()
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			string cardCode = Function.ToString(txtCardCode.Text);
			_dsConfig = await hp.GetAsync("Search", new { cardCode });
			if (_dsConfig == null || _dsConfig.Tables.Count != 4)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			if (_dsConfig.Tables[0].IsNotNull())
			{
				txtCardName.EditValue = _dsConfig.Tables[0].Rows[0]["CardName"];
				txtCardType.EditValue = _dsConfig.Tables[0].Rows[0]["CardType"];
				grdDoc.DataSource = _dsConfig.Tables[1];
				grdDoc.RefreshDataSource();
				AddNewRowBlank();
			}
			else if (!string.IsNullOrEmpty(cardCode))
			{
				WinCommons.Error(Enums.MessageCode.VendorDoesNotExistsChooseOtherVendor, "");
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCb()
	{
		await LoadCbPriceList();
	}

	private async Task LoadCbPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			DataTable dataTable = await client.GetDataTableAsync("GetGroupNumPriceList");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_PriceList.Properties.DataSource = dataTable;
				luk_PriceList.Properties.ForceInitialize();
				rLU_PriceList.DataSource = dataTable;
				rLU_PriceList.ForceInitialize();
				luk_PriceList.EditValue = Function.ParseInt(dataTable.Rows[0]["ListNum"]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDoc, e.Point);
	}

	private void viewDToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string itemCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemCode));
		string idPrList = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcListNum));
		Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemName));
		frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: true, flagSearchFrm: false, itemCode);
		frm_ItemMasterData2.idPrList = idPrList;
		frm_ItemMasterData2.MdiParent = MdiParent;
		frm_ItemMasterData2.Show();
	}

	private void ChooseItem()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, null, Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count == 0)
			{
				return;
			}
			int num = gvDoc.FocusedRowHandle;
			int num2 = Function.ParseInt(luk_PriceList.EditValue);
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				string itemCode = Function.ToString(dataRow["ItemCode"]);
				if (!dataSourceTable.IsNotNull() || !dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(itemCode)))
				{
					if (i > 0)
					{
						DataRow dataRow2 = dataSourceTable.NewRow();
						dataRow2["ListNum"] = num2;
						dataSourceTable.Rows.InsertAt(dataRow2, num);
						num = gvDoc.FindRowHandleByDataRow(dataRow2);
					}
					gvDoc.SetRowCellValue(num, "ItemCode", itemCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.DataRowCount <= 1)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Update;
			List<DataRow> listDataSourceSelectedRow = gvDoc.GetListDataSourceSelectedRow();
			IEnumerable<string> lstItem = listDataSourceSelectedRow.Select((DataRow t) => Function.ToString(t["ItemCode"]));
			if (_dsConfig.Tables[2].IsNotNull())
			{
				List<DataRow> list = (from t in _dsConfig.Tables[2].AsEnumerable()
					where lstItem.Contains(Function.ToString(t["ItemCode"]))
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					_dsConfig.Tables[2].Rows.Remove(list[num]);
				}
			}
			if (_dsConfig.Tables[3].IsNotNull())
			{
				List<DataRow> list2 = (from t in _dsConfig.Tables[3].AsEnumerable()
					where lstItem.Contains(Function.ToString(t["ItemCode"]))
					select t).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					_dsConfig.Tables[3].Rows.Remove(list2[num2]);
				}
			}
			gvDoc.DeleteSelectedRows();
			_dsConfig.Tables[1].AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void Save()
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			grdDoc.Enabled = false;
			btOK.Enabled = true;
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			string text = await hp.PostAsJsonAsync("SaveDoc", _dsConfig);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				await Search();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		btOK.Enabled = true;
		grdDoc.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void gvDoc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
			string itemCode = Function.ToString(focusedDataRow[gcItemCode.FieldName]);
			string cardCode = Function.ToString(txtCardCode.EditValue);
			Function.ToString(focusedDataRow[gcListNum.FieldName]);
			Function.ParseDouble(focusedDataRow[gcDiscount.FieldName]);
			double priceItem = Function.ParseDouble(focusedDataRow[gcPrice.FieldName]);
			string autoUpdt = Function.ToString(focusedDataRow[gcAutoUpdt.FieldName]);
			int priceList = Function.ParseInt(focusedDataRow[gcListNum.FieldName]);
			string currency = Function.ToString(focusedDataRow[gcCurrenCy.FieldName]);
			if (string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(txtCardCode.Text.Trim()))
			{
				return;
			}
			DataTable table = _dsConfig.Tables[2].Clone();
			DataTable table2 = _dsConfig.Tables[3].Clone();
			List<DataRow> list = new List<DataRow>();
			List<DataRow> list2 = new List<DataRow>();
			if (_dsConfig.Tables[2].IsNotNull())
			{
				list = (from t in _dsConfig.Tables[2].AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).ToList();
				if (list.Any())
				{
					table = list.CopyToDataTable();
					if (_dsConfig.Tables[3].IsNotNull())
					{
						list2 = (from t in _dsConfig.Tables[3].AsEnumerable()
							where Function.ToString(t["ItemCode"]).Equals(itemCode)
							select t).ToList();
						if (list2.Any())
						{
							table2 = list2.CopyToDataTable();
						}
					}
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			frm_SpecialforBP_LineSPP1 frm_SpecialforBP_LineSPP3 = new frm_SpecialforBP_LineSPP1();
			frm_SpecialforBP_LineSPP3._itemCode = itemCode;
			frm_SpecialforBP_LineSPP3._cardCode = cardCode;
			frm_SpecialforBP_LineSPP3.priceItem = priceItem;
			frm_SpecialforBP_LineSPP3._currency = currency;
			frm_SpecialforBP_LineSPP3._autoUpdt = autoUpdt;
			frm_SpecialforBP_LineSPP3.PriceList = priceList;
			frm_SpecialforBP_LineSPP3.DsPriceConfig = dataSet;
			frm_SpecialforBP_LineSPP3.ShowDialog(this);
			if (frm_SpecialforBP_LineSPP3.DialogResult != DialogResult.OK)
			{
				return;
			}
			focusedDataRow["IsUpdate"] = "Y";
			FormMode = Enums.FormViewModeType.Update;
			for (int num = 0; num < list.Count; num++)
			{
				_dsConfig.Tables[2].Rows.Remove(list[num]);
			}
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				_dsConfig.Tables[3].Rows.Remove(list2[num2]);
			}
			for (int num3 = 0; num3 < frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows.Count; num3++)
			{
				if (Function.ParseDecimal(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]["Discount"]) != 0m)
				{
					_dsConfig.Tables[2].ImportRow(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]);
				}
			}
			for (int num4 = 0; num4 < frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[1].Rows.Count; num4++)
			{
				_dsConfig.Tables[3].ImportRow(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[1].Rows[num4]);
			}
			_dsConfig.Tables[2].AcceptChanges();
			_dsConfig.Tables[3].AcceptChanges();
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
			if (_CancelCellValueChangeEvent)
			{
				return;
			}
			gvDoc.GetDataRow(e.RowHandle)["IsUpdate"] = "Y";
			FormMode = Enums.FormViewModeType.Update;
			if (e.Column == gcListNum)
			{
				DataTable basePrice = HttpClient.PriceListSetup.GetBasePrice(Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, gcListNum)), Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, gcItemCode)));
				if (basePrice.IsNotNull())
				{
					gvDoc.SetRowCellValue(e.RowHandle, gcPrice, basePrice.Rows[0]["Price"]);
					gvDoc.SetRowCellValue(e.RowHandle, gcCurrenCy, basePrice.Rows[0]["Currency"]);
					_CancelCellValueChangeEvent = true;
					gvDoc.SetRowCellValue(e.RowHandle, gcPriceAfDis, Function.ParseDouble(basePrice.Rows[0]["Price"]) * (1.0 - Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcDiscount)) / 100.0));
					_CancelCellValueChangeEvent = false;
				}
			}
			else if (e.Column == gcDiscount)
			{
				double num = Function.ParseDouble(e.Value);
				double num2 = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcPrice)) * (1.0 - num / 100.0);
				_CancelCellValueChangeEvent = true;
				gvDoc.SetRowCellValue(e.RowHandle, gcPriceAfDis, num2);
				_CancelCellValueChangeEvent = false;
			}
			else if (e.Column == gcPriceAfDis)
			{
				double num3 = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcPrice));
				double num4 = Function.ParseDouble(e.Value);
				double num5 = 0.0;
				if (num3 != 0.0)
				{
					num5 = 100.0 - num4 * 100.0 / num3;
				}
				_CancelCellValueChangeEvent = true;
				gvDoc.SetRowCellValue(e.RowHandle, gcDiscount, num5);
				_CancelCellValueChangeEvent = false;
			}
			else
			{
				if (e.Column != gcItemCode)
				{
					return;
				}
				string itemCode = Function.ToString(e.Value);
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				if (!dataSourceTable.IsNotNull() || (from t in dataSourceTable.AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).Count() == 1)
				{
					int num6 = Function.ParseInt(luk_PriceList.EditValue);
					DataTable basePrice2 = HttpClient.PriceListSetup.GetBasePrice(num6, itemCode);
					if (basePrice2.IsNotNull())
					{
						DataRow dataRow = gvDoc.GetDataRow(e.RowHandle);
						dataRow["SrcPrice"] = 0;
						dataRow["Discount"] = 0;
						dataRow["AutoUpdt"] = "Y";
						dataRow["EXPAND"] = "N";
						dataRow["UserSign"] = GlobalData.UserID;
						dataRow["UserSign2"] = GlobalData.UserID;
						dataRow["Valid"] = "Y";
						dataRow["LogInstanc"] = 0;
						dataRow["CardCode"] = Function.ToString(txtCardCode.EditValue);
						dataRow["ListNum"] = num6;
						gvDoc.SetRowCellValue(e.RowHandle, gcItemName, basePrice2.Rows[0]["ItemName"]);
						gvDoc.SetRowCellValue(e.RowHandle, gcPrice, basePrice2.Rows[0]["Price"]);
						gvDoc.SetRowCellValue(e.RowHandle, gc_AddCur1Price, basePrice2.Rows[0]["AddPrice1"]);
						gvDoc.SetRowCellValue(e.RowHandle, gc_AddCur2Price, basePrice2.Rows[0]["AddPrice2"]);
						dataRow["Currency"] = basePrice2.Rows[0]["Currency"];
						gvDoc.SetRowCellValue(e.RowHandle, gc_UoM, basePrice2.Rows[0]["UomName"]);
						AddNewRowBlank();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.ItemCodeDoesNotExists, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ItemCodeExists, "", itemCode);
					gvDoc.SetRowCellValue(e.RowHandle, "ItemCode", "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (FormMode == Enums.FormViewModeType.Update)
		{
			Save();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txtCardCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseCarCode();
				return;
			}
			string text = txtCardCode.Text;
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oSpecialPrices, ApzObjectTypes.oBusinessPartners, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowBlank();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.Error, ex.Message);
		}
	}

	private void ChooseCarCode()
	{
		try
		{
			string txtSeach = Function.ToString(txtCardCode.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCardCode.EditValue = frm_CFL2.ChooseItem["CardCode"];
				txtCardName.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowBlank()
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || dataSourceTable.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["Discount"] = Function.ParseDouble(txtDiscount.EditValue);
				dataRow["ListNum"] = Function.ParseInt(luk_PriceList.EditValue);
				dataRow["AutoUpdt"] = "Y";
				dataRow["Expand"] = "N";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rBT_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "ItemCode"));
			if (e.Button.Index == 0 && !string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oSpecialPrices, ApzObjectTypes.oItems, text);
			}
			if (e.Button.Index == 1)
			{
				ChooseItem();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_SpecialforBP_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (e.FieldName.Equals("CardCode"))
			{
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_SpecialforBP_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_SpecialforBP));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rBT_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rPrice = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcPriceAfDis = new Apzon.Controls.Grid.GridColumnEx();
		this.gcListNum = new Apzon.Controls.Grid.GridColumnEx();
		this.rLU_PriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcAutoUpdt = new Apzon.Controls.Grid.GridColumnEx();
		this.rCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcCurrenCy = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpand = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UoM = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PriCurPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AddCur1Price = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AddCur2Price = new Apzon.Controls.Grid.GridColumnEx();
		this.rLU_VatGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtCardType = new Apzon.Controls.Editor.LookUpEditEx();
		this.luk_PriceList = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_BPType = new System.Windows.Forms.Label();
		this.txtCardName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_BPName = new System.Windows.Forms.Label();
		this.lb_DiscountPersentage = new Apzon.Controls.Controls.LabelLine();
		this.lb_PriceList = new Apzon.Controls.Controls.LabelLine();
		this.lb_BPCode = new System.Windows.Forms.Label();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtDiscount = new Apzon.Controls.Editor.CalcEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_VatGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtCardType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDiscount.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.panelControl1.Controls.Add(this.panelControl4);
		this.panelControl1.Controls.Add(this.panelControl3);
		this.panelControl1.Controls.Add(this.panelControl2);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(886, 422);
		this.panelControl1.TabIndex = 0;
		this.panelControl4.Controls.Add(this.grdDoc);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(2, 101);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(882, 287);
		this.panelControl4.TabIndex = 2;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rLU_VatGroup, this.rCal, this.rLU_PriceList, this.rBT_ItemCode, this.rPrice, this.rCheckEdit });
		this.grdDoc.Size = new System.Drawing.Size(878, 283);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem, this.viewDToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.viewDToolStripMenuItem.Name = "viewDToolStripMenuItem";
		this.viewDToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.viewDToolStripMenuItem.Text = "View Detail";
		this.viewDToolStripMenuItem.Click += new System.EventHandler(this.viewDToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcItemCode, this.gcItemName, this.gcPrice, this.gcDiscount, this.gcPriceAfDis, this.gcListNum, this.gcAutoUpdt, this.gcCurrenCy, this.gcExpand, this.gc_UoM,
			this.gc_PriCurPrice, this.gc_AddCur1Price, this.gc_AddCur2Price
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oSpecialPriceForBp";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OPRICE";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowFooter = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDoc_PopupMenuShowing);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.DoubleClick += new System.EventHandler(this.gvDoc_DoubleClick);
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.ColumnEdit = this.rBT_ItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 95;
		this.rBT_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rBT_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rBT_ItemCode.Name = "rBT_ItemCode";
		this.rBT_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBT_ItemCode_ButtonPressed);
		this.gcItemName.Caption = "ItemName";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 150;
		this.gcPrice.Caption = "Price";
		this.gcPrice.ColumnEdit = this.rPrice;
		this.gcPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = "Currency";
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice.FieldName = "PriceBefDis";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 2;
		this.gcPrice.Width = 80;
		this.rPrice.AutoHeight = false;
		this.rPrice.Name = "rPrice";
		this.gcDiscount.Caption = "Disc . in %";
		this.gcDiscount.ColumnEdit = this.rCal;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 3;
		this.gcDiscount.Width = 106;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.gcPriceAfDis.Caption = "Price after discount";
		this.gcPriceAfDis.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcPriceAfDis.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPriceAfDis.CurrencyDateField = null;
		this.gcPriceAfDis.CurrencyField = "Currency";
		this.gcPriceAfDis.CurrencyFixed = null;
		this.gcPriceAfDis.CurrencyFixedRate = 0.0;
		this.gcPriceAfDis.CurrencyRateField = null;
		this.gcPriceAfDis.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPriceAfDis.FieldName = "Price";
		this.gcPriceAfDis.Name = "gcPriceAfDis";
		this.gcPriceAfDis.PrimaryKey = false;
		this.gcPriceAfDis.UnitField = null;
		this.gcPriceAfDis.Visible = true;
		this.gcPriceAfDis.VisibleIndex = 4;
		this.gcPriceAfDis.Width = 119;
		this.gcListNum.Caption = "Price List No";
		this.gcListNum.ColumnEdit = this.rLU_PriceList;
		this.gcListNum.CurrencyDateField = null;
		this.gcListNum.CurrencyField = null;
		this.gcListNum.CurrencyFixed = null;
		this.gcListNum.CurrencyFixedRate = 0.0;
		this.gcListNum.CurrencyRateField = null;
		this.gcListNum.FieldName = "ListNum";
		this.gcListNum.Name = "gcListNum";
		this.gcListNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcListNum.PrimaryKey = false;
		this.gcListNum.UnitField = null;
		this.gcListNum.Visible = true;
		this.gcListNum.VisibleIndex = 5;
		this.gcListNum.Width = 148;
		this.rLU_PriceList.AutoHeight = false;
		this.rLU_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLU_PriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name")
		});
		this.rLU_PriceList.DisplayMember = "ListName";
		this.rLU_PriceList.Name = "rLU_PriceList";
		this.rLU_PriceList.NullText = "";
		this.rLU_PriceList.ValueMember = "ListNum";
		this.gcAutoUpdt.Caption = "Auto Update";
		this.gcAutoUpdt.ColumnEdit = this.rCheckEdit;
		this.gcAutoUpdt.CurrencyDateField = null;
		this.gcAutoUpdt.CurrencyField = null;
		this.gcAutoUpdt.CurrencyFixed = null;
		this.gcAutoUpdt.CurrencyFixedRate = 0.0;
		this.gcAutoUpdt.CurrencyRateField = null;
		this.gcAutoUpdt.FieldName = "AutoUpdt";
		this.gcAutoUpdt.Name = "gcAutoUpdt";
		this.gcAutoUpdt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcAutoUpdt.PrimaryKey = false;
		this.gcAutoUpdt.UnitField = null;
		this.gcAutoUpdt.Visible = true;
		this.gcAutoUpdt.VisibleIndex = 9;
		this.gcAutoUpdt.Width = 100;
		this.rCheckEdit.AutoHeight = false;
		this.rCheckEdit.Name = "rCheckEdit";
		this.rCheckEdit.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rCheckEdit.ValueChecked = "Y";
		this.rCheckEdit.ValueUnchecked = "N";
		this.gcCurrenCy.Caption = "Currency";
		this.gcCurrenCy.CurrencyDateField = null;
		this.gcCurrenCy.CurrencyField = null;
		this.gcCurrenCy.CurrencyFixed = null;
		this.gcCurrenCy.CurrencyFixedRate = 0.0;
		this.gcCurrenCy.CurrencyRateField = null;
		this.gcCurrenCy.FieldName = "Currency";
		this.gcCurrenCy.Name = "gcCurrenCy";
		this.gcCurrenCy.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCurrenCy.PrimaryKey = false;
		this.gcCurrenCy.UnitField = null;
		this.gcCurrenCy.Visible = true;
		this.gcCurrenCy.VisibleIndex = 10;
		this.gcCurrenCy.Width = 92;
		this.gcExpand.Caption = "gcExpand";
		this.gcExpand.CurrencyDateField = null;
		this.gcExpand.CurrencyField = null;
		this.gcExpand.CurrencyFixed = null;
		this.gcExpand.CurrencyFixedRate = 0.0;
		this.gcExpand.CurrencyRateField = null;
		this.gcExpand.FieldName = "Expand";
		this.gcExpand.Name = "gcExpand";
		this.gcExpand.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcExpand.PrimaryKey = false;
		this.gcExpand.UnitField = null;
		this.gcExpand.Width = 139;
		this.gc_UoM.Caption = "UoM Name";
		this.gc_UoM.CurrencyDateField = null;
		this.gc_UoM.CurrencyField = null;
		this.gc_UoM.CurrencyFixed = null;
		this.gc_UoM.CurrencyFixedRate = 0.0;
		this.gc_UoM.CurrencyRateField = null;
		this.gc_UoM.FieldName = "UoMName";
		this.gc_UoM.Name = "gc_UoM";
		this.gc_UoM.OptionsColumn.AllowEdit = false;
		this.gc_UoM.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_UoM.OptionsColumn.ReadOnly = true;
		this.gc_UoM.PrimaryKey = false;
		this.gc_UoM.UnitField = null;
		this.gc_UoM.Visible = true;
		this.gc_UoM.VisibleIndex = 11;
		this.gc_PriCurPrice.Caption = "Unit Price - Pri. Curr.";
		this.gc_PriCurPrice.CurrencyDateField = null;
		this.gc_PriCurPrice.CurrencyField = null;
		this.gc_PriCurPrice.CurrencyFixed = null;
		this.gc_PriCurPrice.CurrencyFixedRate = 0.0;
		this.gc_PriCurPrice.CurrencyRateField = null;
		this.gc_PriCurPrice.FieldName = "PrimPrice";
		this.gc_PriCurPrice.Name = "gc_PriCurPrice";
		this.gc_PriCurPrice.OptionsColumn.AllowEdit = false;
		this.gc_PriCurPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_PriCurPrice.OptionsColumn.ReadOnly = true;
		this.gc_PriCurPrice.PrimaryKey = false;
		this.gc_PriCurPrice.UnitField = null;
		this.gc_PriCurPrice.Visible = true;
		this.gc_PriCurPrice.VisibleIndex = 6;
		this.gc_AddCur1Price.Caption = "Unit Price - Add. Curr. 1";
		this.gc_AddCur1Price.CurrencyDateField = null;
		this.gc_AddCur1Price.CurrencyField = null;
		this.gc_AddCur1Price.CurrencyFixed = null;
		this.gc_AddCur1Price.CurrencyFixedRate = 0.0;
		this.gc_AddCur1Price.CurrencyRateField = null;
		this.gc_AddCur1Price.FieldName = "AddPrice1";
		this.gc_AddCur1Price.Name = "gc_AddCur1Price";
		this.gc_AddCur1Price.OptionsColumn.AllowEdit = false;
		this.gc_AddCur1Price.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_AddCur1Price.OptionsColumn.ReadOnly = true;
		this.gc_AddCur1Price.PrimaryKey = false;
		this.gc_AddCur1Price.UnitField = null;
		this.gc_AddCur1Price.Visible = true;
		this.gc_AddCur1Price.VisibleIndex = 7;
		this.gc_AddCur2Price.Caption = "Unit Price - Add. Curr. 2";
		this.gc_AddCur2Price.CurrencyDateField = null;
		this.gc_AddCur2Price.CurrencyField = null;
		this.gc_AddCur2Price.CurrencyFixed = null;
		this.gc_AddCur2Price.CurrencyFixedRate = 0.0;
		this.gc_AddCur2Price.CurrencyRateField = null;
		this.gc_AddCur2Price.FieldName = "AddPrice2";
		this.gc_AddCur2Price.Name = "gc_AddCur2Price";
		this.gc_AddCur2Price.OptionsColumn.AllowEdit = false;
		this.gc_AddCur2Price.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_AddCur2Price.OptionsColumn.ReadOnly = true;
		this.gc_AddCur2Price.PrimaryKey = false;
		this.gc_AddCur2Price.UnitField = null;
		this.gc_AddCur2Price.Visible = true;
		this.gc_AddCur2Price.VisibleIndex = 8;
		this.rLU_VatGroup.AutoHeight = false;
		this.rLU_VatGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLU_VatGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Rate", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLU_VatGroup.DisplayMember = "Code";
		this.rLU_VatGroup.Name = "rLU_VatGroup";
		this.rLU_VatGroup.NullText = "";
		this.rLU_VatGroup.ValueMember = "Code";
		this.panelControl3.Controls.Add(this.btOK);
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(2, 388);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(882, 32);
		this.panelControl3.TabIndex = 1;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(5, 7);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 40;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(108, 7);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 41;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.panelControl2.Controls.Add(this.txtCardType);
		this.panelControl2.Controls.Add(this.luk_PriceList);
		this.panelControl2.Controls.Add(this.lb_BPType);
		this.panelControl2.Controls.Add(this.txtCardName);
		this.panelControl2.Controls.Add(this.lb_BPName);
		this.panelControl2.Controls.Add(this.lb_DiscountPersentage);
		this.panelControl2.Controls.Add(this.lb_PriceList);
		this.panelControl2.Controls.Add(this.lb_BPCode);
		this.panelControl2.Controls.Add(this.txtCardCode);
		this.panelControl2.Controls.Add(this.txtDiscount);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(2, 2);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(882, 99);
		this.panelControl2.TabIndex = 0;
		this.txtCardType.EditControl = null;
		this.txtCardType.FieldName = "CardType";
		this.txtCardType.IsSearchControl = false;
		this.txtCardType.Location = new System.Drawing.Point(371, 23);
		this.txtCardType.Name = "txtCardType";
		this.txtCardType.Properties.AllowMouseWheel = false;
		this.txtCardType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCardType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtCardType.Properties.DisplayMember = "Name";
		this.txtCardType.Properties.NullText = "";
		this.txtCardType.Properties.ShowHeader = false;
		this.txtCardType.Properties.ValueMember = "Code";
		this.txtCardType.SettingInVisible = false;
		this.txtCardType.SettingReadonly = false;
		this.txtCardType.Size = new System.Drawing.Size(163, 20);
		this.txtCardType.TabIndex = 13;
		this.luk_PriceList.EditControl = null;
		this.luk_PriceList.FieldName = "ListNum";
		this.luk_PriceList.IsSearchControl = false;
		this.luk_PriceList.Location = new System.Drawing.Point(190, 46);
		this.luk_PriceList.Name = "luk_PriceList";
		this.luk_PriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name")
		});
		this.luk_PriceList.Properties.DisplayMember = "ListName";
		this.luk_PriceList.Properties.NullText = "";
		this.luk_PriceList.Properties.ValueMember = "ListNum";
		this.luk_PriceList.SettingInVisible = false;
		this.luk_PriceList.SettingReadonly = false;
		this.luk_PriceList.Size = new System.Drawing.Size(163, 20);
		this.luk_PriceList.TabIndex = 12;
		this.lb_BPType.AutoSize = true;
		this.lb_BPType.Location = new System.Drawing.Point(368, 7);
		this.lb_BPType.Name = "lb_BPType";
		this.lb_BPType.Size = new System.Drawing.Size(46, 13);
		this.lb_BPType.TabIndex = 9;
		this.lb_BPType.Text = "BP Type";
		this.txtCardName.EditControl = null;
		this.txtCardName.FieldName = "CardName";
		this.txtCardName.IsSearchControl = false;
		this.txtCardName.Location = new System.Drawing.Point(190, 23);
		this.txtCardName.Name = "txtCardName";
		this.txtCardName.SettingInVisible = false;
		this.txtCardName.SettingReadonly = false;
		this.txtCardName.Size = new System.Drawing.Size(163, 20);
		this.txtCardName.TabIndex = 8;
		this.lb_BPName.AutoSize = true;
		this.lb_BPName.Location = new System.Drawing.Point(187, 7);
		this.lb_BPName.Name = "lb_BPName";
		this.lb_BPName.Size = new System.Drawing.Size(49, 13);
		this.lb_BPName.TabIndex = 7;
		this.lb_BPName.Text = "BP Name";
		this.lb_DiscountPersentage.EditControl = null;
		this.lb_DiscountPersentage.EditValue = "Discount Persentage";
		this.lb_DiscountPersentage.FieldName = null;
		this.lb_DiscountPersentage.HideWhenEmpty = false;
		this.lb_DiscountPersentage.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DiscountPersentage.Location = new System.Drawing.Point(13, 75);
		this.lb_DiscountPersentage.Name = "lb_DiscountPersentage";
		this.lb_DiscountPersentage.ReadOnly = false;
		this.lb_DiscountPersentage.SettingInVisible = false;
		this.lb_DiscountPersentage.SettingReadonly = false;
		this.lb_DiscountPersentage.ShowUnderLine = true;
		this.lb_DiscountPersentage.Size = new System.Drawing.Size(177, 20);
		this.lb_DiscountPersentage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DiscountPersentage.TabIndex = 6;
		this.lb_DiscountPersentage.Utext = "Discount Persentage";
		this.lb_PriceList.EditControl = null;
		this.lb_PriceList.EditValue = "Price List";
		this.lb_PriceList.FieldName = null;
		this.lb_PriceList.HideWhenEmpty = false;
		this.lb_PriceList.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PriceList.Location = new System.Drawing.Point(13, 49);
		this.lb_PriceList.Name = "lb_PriceList";
		this.lb_PriceList.ReadOnly = false;
		this.lb_PriceList.SettingInVisible = false;
		this.lb_PriceList.SettingReadonly = false;
		this.lb_PriceList.ShowUnderLine = true;
		this.lb_PriceList.Size = new System.Drawing.Size(177, 20);
		this.lb_PriceList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PriceList.TabIndex = 4;
		this.lb_PriceList.Utext = "Price List";
		this.lb_BPCode.AutoSize = true;
		this.lb_BPCode.Location = new System.Drawing.Point(10, 7);
		this.lb_BPCode.Name = "lb_BPCode";
		this.lb_BPCode.Size = new System.Drawing.Size(47, 13);
		this.lb_BPCode.TabIndex = 0;
		this.lb_BPCode.Text = "BP Code";
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = "CardCode";
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(13, 23);
		this.txtCardCode.Name = "txtCardCode";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCardCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardCode_Properties_ButtonClick);
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(163, 20);
		this.txtCardCode.TabIndex = 1;
		this.txtDiscount.EditControl = null;
		this.txtDiscount.FieldName = "Discount";
		this.txtDiscount.IsSearchControl = false;
		this.txtDiscount.Location = new System.Drawing.Point(190, 72);
		this.txtDiscount.Name = "txtDiscount";
		this.txtDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDiscount.Properties.IsSearchControl = false;
		this.txtDiscount.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtDiscount.SettingInVisible = false;
		this.txtDiscount.SettingReadonly = false;
		this.txtDiscount.Size = new System.Drawing.Size(163, 20);
		this.txtDiscount.TabIndex = 11;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(886, 422);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oSpecialPriceForBp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SpecialforBP";
		this.Text = "Special prices for business partners";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_SpecialforBP_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SpecialforBP_FormClosing);
		base.Load += new System.EventHandler(this.frm_SpecialforBP_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_VatGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.panelControl2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtCardType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDiscount.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
