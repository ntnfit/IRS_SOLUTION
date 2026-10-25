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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_STDPriceListDetails : XtraFormEx
{
	public int _ListNum = -10;

	public string _ListName = "";

	public string _strSearch = "";

	public DataTable DtPriceList = new DataTable();

	private DataTable _dtDoc;

	private List<string> _lstChangedItCode;

	private string _priceOldValue = "";

	private string _formController = "STDPriceLists";

	private List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private bool _isLoading;

	private bool _checkChangingPrice;

	private bool _isChangingCheckGrossPrice;

	private bool _checkChangingPrice1;

	private bool _checkChangingPrice2;

	private bool _changingPrice;

	private bool _changingPrice1;

	private bool _changingPrice2;

	private bool _isValidatingCurrency;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<PriceListCurrencyImpl> _lstPriceListCurr = new List<PriceListCurrencyImpl>();

	private IContainer components;

	private GridControlEx grd_Doc;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx btn_SelCriterial;

	private SimpleButtonEx btn_Clear;

	private LabelLine lb_Find;

	private TextEditEx txt_Find;

	private CheckEditEx chk_IsGrossPrice;

	private BandedGridViewEx gv_Doc;

	private BandedGridColumnEx gcItemCode;

	private BandedGridColumnEx gcItemName;

	private BandedGridColumnEx gcBasePLNum;

	private BandedGridColumnEx gcFactor;

	private BandedGridColumnEx gcBasePrice;

	private BandedGridColumnEx gcPrice;

	private BandedGridColumnEx gcOvrwritten;

	private BandedGridColumnEx gcPriceList;

	private BandedGridColumnEx gcITMSGRPCOD;

	private BandedGridColumnEx gcCardCode;

	private BandedGridColumnEx gridColumn1;

	private BandedGridColumnEx gridColumn2;

	private BandedGridColumnEx gcInvntryUom;

	private BandedGridColumnEx gridColumn3;

	private BandedGridColumnEx gcAddPrice1;

	private BandedGridColumnEx gridColumn5;

	private BandedGridColumnEx gridColumn6;

	private BandedGridColumnEx gcAddPrice2;

	private BandedGridColumnEx gridColumn8;

	private GridBand gridBand1;

	private GridBand gridBand2;

	private GridBand gridBand3;

	private GridBand gridBand4;

	private GridBand gridBand5;

	private BandedGridColumnEx bandedGridColumn1;

	private BandedGridColumnEx bandedGridColumn2;

	private BandedGridColumnEx bandedGridColumn3;

	private BandedGridColumnEx bandedGridColumn4;

	private RepositoryItemCheckEdit rep_chk_Manual;

	private RepositoryItemLookUpEdit rep_luk_basePricelist;

	private RepositoryItemButtonEdit rep_bte_ItemCode;

	private RepositoryItemCheckEdit rep_chk_Manual1;

	private RepositoryItemCheckEdit rep_chk_Manual2;

	private RepositoryItemTextEditExPrice rep_Price;

	private RepositoryItemTextEditExPrice rep_AddPrice1;

	private RepositoryItemTextEditExPrice rep_AddPrice2;

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
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_STDPriceListDetails()
	{
		InitializeComponent();
		SettingLanguage();
		_lstChangedItCode = new List<string>();
		OnLoadAsync += frm_PriceListDetails_Load;
	}

	private async Task frm_PriceListDetails_Load(object sender, EventArgs e)
	{
		try
		{
			FormId = "oPriceListDetails";
			if (!string.IsNullOrWhiteSpace(_ListName))
			{
				Text = _ListName;
			}
			RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ExtenseionMethods.LoadBoMFromGridItem(Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode")), MdiParent);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode"));
			if (_lstChangedItCode == null || !_lstChangedItCode.Contains(text))
			{
				int ugpEntry = GetUgpEntry(text);
				if (ugpEntry > 0)
				{
					frm_UoMPrices frm_UoMPrices2 = new frm_UoMPrices();
					frm_UoMPrices2._ItemCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode"));
					frm_UoMPrices2._UgpEntry = ugpEntry;
					frm_UoMPrices2._PriceList = _ListNum;
					frm_UoMPrices2.ShowDialog(this);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataTable dataTable = grd_Doc.DataSource as DataTable;
				DataTable dataTable2 = dataTable.Clone();
				for (int i = 0; i < _lstChangedItCode.Count; i++)
				{
					DataRow[] array = dataTable.Select("ItemCode='" + _lstChangedItCode[i] + "'");
					if (array != null && array.Length != 0)
					{
						dataTable2.ImportRow(array[0]);
					}
				}
				dataTable2.TableName = "DOC";
				DataSet dataSet = dataTable2.DataSet;
				if (dataSet != null)
				{
					dataSet.Tables.Remove("DOC");
					dataSet.AcceptChanges();
				}
				dataTable2.AcceptChanges();
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					if (Function.ParseInt(dataTable2.Rows[j]["PriceList"]) == Function.ParseInt(dataTable2.Rows[j]["BasePLNum"]))
					{
						dataTable2.Rows[j]["Factor"] = 1;
					}
				}
				dataTable2.AcceptChanges();
				DataTable dataTable3 = new DataTable();
				dataTable3.TableName = "IsGross";
				dataTable3.Columns.Add("ListNum", typeof(int));
				dataTable3.Columns.Add("IsGross", typeof(string));
				DataRow dataRow = dataTable3.NewRow();
				dataRow["ListNum"] = _ListNum;
				dataRow["IsGross"] = (chk_IsGrossPrice.Checked ? "Y" : "N");
				dataTable3.Rows.Add(dataRow);
				dataTable3.AcceptChanges();
				DataSet dataSet2 = dataTable3.DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove("IsGross");
					dataSet2.AcceptChanges();
				}
				DataSet dataSet3 = new DataSet();
				dataSet3.Tables.Clear();
				dataSet3.Tables.Add(dataTable2);
				dataSet3.Tables.Add(dataTable3);
				dataSet3.AcceptChanges();
				if (ValidateData(dataTable2))
				{
					SaveData(dataSet3);
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading || _isValidatingCurrency)
			{
				return;
			}
			string itCode = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ItemCode"));
			if (!_lstChangedItCode.Exists((string t) => t == itCode))
			{
				_lstChangedItCode.Add(itCode);
			}
			FormMode = Enums.FormViewModeType.Update;
			if (e.Column.FieldName == "BasePLNum" || e.Column.FieldName == "Factor")
			{
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "BasePLNum"));
				double num2 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "Factor"));
				if (_ListNum == num)
				{
					num2 = 1.0;
				}
				if (e.Column.FieldName == "BasePLNum")
				{
					DataTable basePrice = GetBasePrice(num, itCode);
					if (basePrice != null && basePrice.Rows.Count > 0)
					{
						string value = Function.ToString(basePrice.Rows[0]["Currency"]);
						string value2 = Function.ToString(basePrice.Rows[0]["Currency1"]);
						string value3 = Function.ToString(basePrice.Rows[0]["Currency2"]);
						double num3 = Function.ParseDouble(basePrice.Rows[0]["Price"]);
						double num4 = Function.ParseDouble(basePrice.Rows[0]["AddPrice1"]);
						double num5 = Function.ParseDouble(basePrice.Rows[0]["AddPrice2"]);
						gv_Doc.SetRowCellValue(e.RowHandle, "Currency", value);
						gv_Doc.SetRowCellValue(e.RowHandle, "Currency1", value2);
						gv_Doc.SetRowCellValue(e.RowHandle, "Currency1", value2);
						gv_Doc.SetRowCellValue(e.RowHandle, "Currency2", value3);
						gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice", num3);
						_checkChangingPrice = true;
						gv_Doc.SetRowCellValue(e.RowHandle, "Price", num3 * num2);
						_checkChangingPrice = false;
						gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice1", num4);
						_checkChangingPrice1 = true;
						gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice1", num4 * num2);
						_checkChangingPrice1 = false;
						gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice2", num5);
						_checkChangingPrice2 = true;
						gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice2", num5 * num2);
						_checkChangingPrice2 = false;
					}
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwritten", "N");
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwrite1", "N");
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwrite2", "N");
				}
				if (e.Column.FieldName == "Factor")
				{
					double num6 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "BasePrice"));
					_checkChangingPrice = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Price", num6 * num2);
					_checkChangingPrice = false;
					double num7 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "BasePrice1"));
					_checkChangingPrice1 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice1", num7 * num2);
					_checkChangingPrice1 = false;
					double num8 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "BasePrice2"));
					_checkChangingPrice2 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice2", num8 * num2);
					_checkChangingPrice2 = false;
				}
			}
			if (e.Column.FieldName == "Ovrwritten" && !_changingPrice)
			{
				double num9 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "Factor"));
				int priceList = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "BasePLNum"));
				DataTable basePrice2 = GetBasePrice(priceList, itCode);
				string value4 = Function.ToString(basePrice2.Rows[0]["Currency"]);
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency", value4);
				if (Function.ToString(e.Value) == "N")
				{
					double num10 = Function.ParseDouble(basePrice2.Rows[0]["Price"]);
					_checkChangingPrice = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Price", num10 * num9);
					_checkChangingPrice = false;
				}
				else
				{
					_checkChangingPrice = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Price", 0);
					_checkChangingPrice = false;
				}
			}
			else if (e.Column.FieldName == "Ovrwrite1" && !_changingPrice1)
			{
				double num11 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "Factor"));
				int priceList2 = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "BasePLNum"));
				DataTable basePrice3 = GetBasePrice(priceList2, itCode);
				string value5 = Function.ToString(basePrice3.Rows[0]["Currency1"]);
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency1", value5);
				if (Function.ToString(e.Value) == "N")
				{
					double num12 = Function.ParseDouble(basePrice3.Rows[0]["AddPrice1"]);
					_checkChangingPrice1 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice1", num12 * num11);
					_checkChangingPrice1 = false;
				}
				else
				{
					_checkChangingPrice1 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice1", 0);
					_checkChangingPrice1 = false;
				}
			}
			else if (e.Column.FieldName == "Ovrwrite2" && !_changingPrice2)
			{
				double num13 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "Factor"));
				int priceList3 = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "BasePLNum"));
				DataTable basePrice4 = GetBasePrice(priceList3, itCode);
				string value6 = Function.ToString(basePrice4.Rows[0]["Currency2"]);
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency2", value6);
				if (Function.ToString(e.Value) == "N")
				{
					double num14 = Function.ParseDouble(basePrice4.Rows[0]["AddPrice2"]);
					_checkChangingPrice2 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice2", num14 * num13);
					_checkChangingPrice2 = false;
				}
				else
				{
					_checkChangingPrice2 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice2", 0);
					_checkChangingPrice2 = false;
				}
			}
			else if (e.Column.FieldName == "Price" && !_checkChangingPrice)
			{
				string value7 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency"));
				double num15 = Function.ParseDouble(e.Value);
				if (num15 > 0.0 && num15 != Function.ParseDouble(_priceOldValue))
				{
					_changingPrice = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwritten", "Y");
					_changingPrice = false;
				}
				int num16 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BasePLNum"));
				if (_ListNum == num16)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice", Function.ParseDouble(e.Value));
					gv_Doc.SetRowCellValue(e.RowHandle, "BaseCurrency", value7);
				}
			}
			else if (e.Column.FieldName == "AddPrice1" && !_checkChangingPrice1)
			{
				string value8 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency1"));
				double num17 = Function.ParseDouble(e.Value);
				if (num17 > 0.0 && num17 != Function.ParseDouble(_priceOldValue))
				{
					_changingPrice1 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwrite1", "Y");
					_changingPrice1 = false;
				}
				int num18 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BasePLNum"));
				if (_ListNum == num18)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice1", Function.ParseDouble(e.Value));
					gv_Doc.SetRowCellValue(e.RowHandle, "BaseCurrency1", value8);
				}
			}
			else if (e.Column.FieldName == "AddPrice2" && !_checkChangingPrice2)
			{
				string value9 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency2"));
				double num19 = Function.ParseDouble(e.Value);
				if (num19 > 0.0 && num19 != Function.ParseDouble(_priceOldValue))
				{
					_changingPrice2 = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Ovrwrite2", "Y");
					_changingPrice2 = false;
				}
				int num20 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BasePLNum"));
				if (_ListNum == num20)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "BasePrice2", Function.ParseDouble(e.Value));
					gv_Doc.SetRowCellValue(e.RowHandle, "BaseCurrency2", value9);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_SelCriterial_Click(object sender, EventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				string text = "[ItemCode] in (";
				for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
				{
					text = text + "'" + frm_CFL2.ChooseItems[i]["ItemCode"]?.ToString() + "',";
				}
				text = text.Remove(text.Length - 1);
				text += ")";
				_lstChangedItCode.Clear();
				DataTable dataTable = _dtDoc.Clone();
				DataRow[] array = _dtDoc.Select(text);
				foreach (DataRow row in array)
				{
					dataTable.ImportRow(row);
				}
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (Function.ParseDouble(e.Value) == 0.0 && (string.Compare(e.Column.FieldName, "PRICE", ignoreCase: true) == 0 || string.Compare(e.Column.FieldName, "BASEPRICE", ignoreCase: true) == 0 || string.Compare(e.Column.FieldName, "ADDPRICE1", ignoreCase: true) == 0 || string.Compare(e.Column.FieldName, "BASEPRICE1", ignoreCase: true) == 0 || string.Compare(e.Column.FieldName, "ADDPRICE2", ignoreCase: true) == 0 || string.Compare(e.Column.FieldName, "BASEPRICE2", ignoreCase: true) == 0))
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "BasePLNum" && e.Column.FieldName != "Factor" && e.Column.FieldName != "Price" && e.Column.FieldName != "AddPrice1" && e.Column.FieldName != "AddPrice2")
			{
				e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(dataObject.GetData(DataFormats.UnicodeText).ToString());
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			int num = gv_Doc.FocusedRowHandle;
			GridColumn focusedColumn = gv_Doc.FocusedColumn;
			int num2 = ((gv_Doc.RowCount > dataTable.Rows.Count) ? dataTable.Rows.Count : gv_Doc.RowCount);
			for (int i = 0; i < num2; i++)
			{
				if (!(focusedColumn.FieldName == "Price") && !(focusedColumn.FieldName == "AddPrice1") && !(focusedColumn.FieldName == "AddPrice2"))
				{
					continue;
				}
				List<string> currency = Function.GetCurrency(Function.ToString(dataTable.Rows[i][0]));
				if (currency.Count <= 0)
				{
					continue;
				}
				decimal num3 = Function.ParseDecimal(currency[0]);
				string alpha = Function.ToString(currency[1]);
				if (!string.IsNullOrEmpty(alpha) && !GlobalData.CurrencyCodes.Any((CurrencyCodesImpl t) => t.CurrCode.ToUpper() == alpha.ToUpper()))
				{
					break;
				}
				gv_Doc.SetRowCellValue(num, focusedColumn, num3);
				if (!string.IsNullOrWhiteSpace(alpha))
				{
					switch (focusedColumn.FieldName)
					{
					case "Price":
						gv_Doc.SetRowCellValue(num, "Currency", alpha.ToUpper());
						break;
					case "AddPrice1":
						gv_Doc.SetRowCellValue(num, "Currency1", alpha.ToUpper());
						break;
					case "AddPrice2":
						gv_Doc.SetRowCellValue(num, "Currency2", alpha.ToUpper());
						break;
					}
				}
				num++;
			}
			Clipboard.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_chk_Manuals_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Find_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			int dataSourceIndex = 0;
			if (!string.IsNullOrWhiteSpace(txt_Find.Text))
			{
				DataRow[] array = dataTable.Select("ItemCode like '" + txt_Find.Text + "%'");
				if (array != null && array.Length != 0)
				{
					dataSourceIndex = dataTable.Rows.IndexOf(array[0]);
				}
			}
			gv_Doc.FocusedRowHandle = gv_Doc.GetRowHandle(dataSourceIndex);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!gv_Doc.FocusedColumn.FieldName.Equals(gcPrice.FieldName) && !gv_Doc.FocusedColumn.FieldName.Equals(gcAddPrice1.FieldName) && !gv_Doc.FocusedColumn.FieldName.Equals(gcAddPrice2.FieldName))
			{
				return;
			}
			string[] dataCurrency = Function.ToString(e.Value).GetDataCurrency();
			if (dataCurrency == null || dataCurrency.Length <= 1 || !string.IsNullOrEmpty(Function.ToString(dataCurrency[1])))
			{
				return;
			}
			int priceList = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BasePLNum"));
			PriceListCurrencyImpl priceListCurrencyImpl = _lstPriceListCurr.FirstOrDefault((PriceListCurrencyImpl t) => t.PriceList == priceList);
			if (priceListCurrencyImpl == null)
			{
				priceListCurrencyImpl = _httpClient.PriceListSetup.GetDataCurrencyByPriceList(priceList);
				if (priceListCurrencyImpl != null)
				{
					_lstPriceListCurr.Add(priceListCurrencyImpl);
				}
			}
			if (priceListCurrencyImpl != null)
			{
				BaseContainerValidateEditorEventArgs e2 = e;
				string arg = dataCurrency[0];
				string arg2;
				if (gv_Doc.FocusedColumn.FieldName.Equals(gcPrice.FieldName))
				{
					arg2 = priceListCurrencyImpl.PrimCurr;
				}
				else
				{
					arg2 = (gv_Doc.FocusedColumn.FieldName.Equals(gcAddPrice1.FieldName) ? priceListCurrencyImpl.AddCurr1 : priceListCurrencyImpl.AddCurr2);
				}
				e2.Value = $"{arg} {arg2}";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			btn_Cancel.Text = Resources.ButtonCancel;
			lb_Find.Utext = Resources.lb_Find;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveData(DataSet ds)
	{
		try
		{
			bt_OK.Enabled = false;
			string text = _httpClient.PriceListSetup.DetailsSaveDoc(ds);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				RefreshData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			bt_OK.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			bt_OK.Enabled = true;
		}
	}

	private int GetUgpEntry(string itemCode)
	{
		try
		{
			return _httpClient.PriceListSetup.GetUgpEntry(itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	private bool ValidateData(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoRowToUpdate, "");
				return false;
			}
			foreach (DataRow row in dt.Rows)
			{
				if ((Function.ParseDouble(row["Price"]) > 0.0 && !GlobalData.CurrencyCodes.Any((CurrencyCodesImpl t) => t.CurrCode.ToUpper() == Function.ToString(row["Currency"]).ToUpper())) || (Function.ParseDouble(row["AddPrice1"]) > 0.0 && !GlobalData.CurrencyCodes.Any((CurrencyCodesImpl t) => t.CurrCode.ToUpper() == Function.ToString(row["Currency1"]).ToUpper())) || (Function.ParseDouble(row["AddPrice2"]) > 0.0 && !GlobalData.CurrencyCodes.Any((CurrencyCodesImpl t) => t.CurrCode.ToUpper() == Function.ToString(row["Currency2"]).ToUpper())))
				{
					WinCommons.Error(Enums.MessageCode.InvalidCurrency, "");
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private DataTable GetBasePrice(int priceList, string itemCode)
	{
		try
		{
			return _httpClient.PriceListSetup.GetBasePrice(priceList, itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private string GetValueByDecDigits(int decDigits, string source)
	{
		string result = source;
		try
		{
			string[] array = source.Split('.');
			if (array != null)
			{
				if (array.Length == 1)
				{
					result = array[0];
				}
				else if (array.Length == 2)
				{
					if (decDigits == 0)
					{
						result = array[0];
					}
					else if (array[1].Length > decDigits)
					{
						string text = Math.Round(Function.ParseDecimal("0." + array[1].ToString()), decDigits).ToString();
						result = array[0] + "." + text.Remove(0, 2);
					}
					else
					{
						result = source;
					}
				}
				else
				{
					result = "0";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void RefreshData()
	{
		DisableControls(p: true);
		_lstChangedItCode.Clear();
		_isLoading = true;
		LoadData();
		LoadComBoBox();
		_isLoading = false;
		DisableControls();
	}

	private void DisableControls(bool p = false)
	{
		try
		{
			if (p)
			{
				bt_OK.Enabled = false;
				btn_Cancel.Enabled = false;
				btn_Clear.Enabled = false;
				btn_SelCriterial.Enabled = false;
				chk_IsGrossPrice.Enabled = false;
			}
			else
			{
				bt_OK.Enabled = true;
				btn_Cancel.Enabled = true;
				btn_Clear.Enabled = true;
				btn_SelCriterial.Enabled = true;
				chk_IsGrossPrice.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadData()
	{
		try
		{
			DataTable dataTable = _httpClient.PriceListSetup.DetailsLoadData(_ListNum);
			_dtDoc = dataTable.Copy();
			if (_strSearch != "")
			{
				DataTable dataTable2 = _dtDoc.Clone();
				DataRow[] array = _dtDoc.Select(_strSearch);
				foreach (DataRow row in array)
				{
					dataTable2.ImportRow(row);
				}
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
			}
			else
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
			DataTable dataTable3 = _httpClient.PriceListSetup.DetailsLoadIsGross(_ListNum);
			string strA = "N";
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				strA = ((string.Compare(Function.ToString(dataTable3.Rows[0][0]), "y", ignoreCase: true) == 0) ? "Y" : "N");
			}
			_isChangingCheckGrossPrice = true;
			chk_IsGrossPrice.Checked = string.Compare(strA, "y", ignoreCase: true) == 0;
			_isChangingCheckGrossPrice = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComBoBox()
	{
		try
		{
			DataTable groupNumPriceList = _httpClient.PriceListSetup.GetGroupNumPriceList();
			rep_luk_basePricelist.Columns.Clear();
			rep_luk_basePricelist.Columns.Add(new LookUpColumnInfo
			{
				FieldName = "ListName",
				Caption = "Name"
			});
			rep_luk_basePricelist.DisplayMember = "ListName";
			rep_luk_basePricelist.ValueMember = "ListNum";
			rep_luk_basePricelist.DataSource = groupNumPriceList;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_IsGrossPrice_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isChangingCheckGrossPrice)
			{
				FormMode = Enums.FormViewModeType.Update;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_STDPriceListDetails));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.BandedGridViewEx();
		this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcItemCode = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_bte_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcBasePLNum = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_luk_basePricelist = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFactor = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcPriceList = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcITMSGRPCOD = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridBand2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcBasePrice = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcPrice = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_Price = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcOvrwritten = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_chk_Manual = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridBand3 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gridColumn3 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcAddPrice1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_AddPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gridColumn5 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_chk_Manual1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridBand4 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gridColumn6 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcAddPrice2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_AddPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gridColumn8 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_chk_Manual2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridBand5 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcInvntryUom = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.bandedGridColumn1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.bandedGridColumn3 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.bandedGridColumn2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.bandedGridColumn4 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SelCriterial = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.txt_Find = new Apzon.Controls.Editor.TextEditEx();
		this.chk_IsGrossPrice = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_basePricelist).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AddPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AddPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsGrossPrice.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(7, 38);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_chk_Manual, this.rep_luk_basePricelist, this.rep_bte_ItemCode, this.rep_chk_Manual1, this.rep_chk_Manual2, this.rep_Price, this.rep_AddPrice1, this.rep_AddPrice2 });
		this.grd_Doc.Size = new System.Drawing.Size(875, 403);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.grd_Doc.DoubleClick += new System.EventHandler(this.grd_Doc_DoubleClick);
		this.gv_Doc.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[5] { this.gridBand1, this.gridBand2, this.gridBand3, this.gridBand4, this.gridBand5 });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[23]
		{
			this.gcItemCode, this.gcItemName, this.gcBasePLNum, this.gcFactor, this.gcBasePrice, this.gcPrice, this.gcOvrwritten, this.gcPriceList, this.gcITMSGRPCOD, this.gcCardCode,
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gcAddPrice1, this.gridColumn5, this.gridColumn6, this.gcAddPrice2, this.gridColumn8, this.gcInvntryUom, this.bandedGridColumn1,
			this.bandedGridColumn3, this.bandedGridColumn2, this.bandedGridColumn4
		});
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FormId = "oPriceListDetails";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 60;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "ITM1";
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Doc_CustomColumnDisplayText);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gridBand1.Columns.Add(this.gcItemCode);
		this.gridBand1.Columns.Add(this.gcItemName);
		this.gridBand1.Columns.Add(this.gcBasePLNum);
		this.gridBand1.Columns.Add(this.gcFactor);
		this.gridBand1.Columns.Add(this.gcPriceList);
		this.gridBand1.Columns.Add(this.gcITMSGRPCOD);
		this.gridBand1.Columns.Add(this.gcCardCode);
		this.gridBand1.Columns.Add(this.gridColumn1);
		this.gridBand1.Columns.Add(this.gridColumn2);
		this.gridBand1.Name = "gridBand1";
		this.gridBand1.VisibleIndex = 0;
		this.gridBand1.Width = 410;
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.ColumnEdit = this.rep_bte_ItemCode;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.ReadOnly = true;
		this.gcItemCode.Visible = true;
		this.gcItemCode.Width = 85;
		this.rep_bte_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_bte_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_ItemCode.Name = "rep_bte_ItemCode";
		this.rep_bte_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_ItemCode_ButtonPressed);
		this.gcItemName.Caption = "Item Description";
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.AllowEdit = false;
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.Visible = true;
		this.gcItemName.Width = 177;
		this.gcBasePLNum.Caption = "Base Price List";
		this.gcBasePLNum.ColumnEdit = this.rep_luk_basePricelist;
		this.gcBasePLNum.CurrencyField = null;
		this.gcBasePLNum.CurrencyFixed = null;
		this.gcBasePLNum.CurrencyFixedRate = 0.0;
		this.gcBasePLNum.CurrencyRateField = null;
		this.gcBasePLNum.FieldName = "BasePLNum";
		this.gcBasePLNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBasePLNum.Name = "gcBasePLNum";
		this.gcBasePLNum.Visible = true;
		this.gcBasePLNum.Width = 81;
		this.rep_luk_basePricelist.AutoHeight = false;
		this.rep_luk_basePricelist.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_basePricelist.Name = "rep_luk_basePricelist";
		this.rep_luk_basePricelist.NullText = "";
		this.gcFactor.Caption = "Factor";
		this.gcFactor.CurrencyField = null;
		this.gcFactor.CurrencyFixed = null;
		this.gcFactor.CurrencyFixedRate = 0.0;
		this.gcFactor.CurrencyRateField = null;
		this.gcFactor.FieldName = "Factor";
		this.gcFactor.Name = "gcFactor";
		this.gcFactor.Visible = true;
		this.gcFactor.Width = 67;
		this.gcPriceList.Caption = "Price List";
		this.gcPriceList.CurrencyField = null;
		this.gcPriceList.CurrencyFixed = null;
		this.gcPriceList.CurrencyFixedRate = 0.0;
		this.gcPriceList.CurrencyRateField = null;
		this.gcPriceList.FieldName = "PriceList";
		this.gcPriceList.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcPriceList.Name = "gcPriceList";
		this.gcPriceList.OptionsColumn.AllowEdit = false;
		this.gcPriceList.OptionsColumn.ReadOnly = true;
		this.gcITMSGRPCOD.Caption = "Item Groups";
		this.gcITMSGRPCOD.CurrencyField = null;
		this.gcITMSGRPCOD.CurrencyFixed = null;
		this.gcITMSGRPCOD.CurrencyFixedRate = 0.0;
		this.gcITMSGRPCOD.CurrencyRateField = null;
		this.gcITMSGRPCOD.FieldName = "ITMSGRPCOD";
		this.gcITMSGRPCOD.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcITMSGRPCOD.Name = "gcITMSGRPCOD";
		this.gcITMSGRPCOD.OptionsColumn.AllowEdit = false;
		this.gcITMSGRPCOD.OptionsColumn.ReadOnly = true;
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.OptionsColumn.AllowEdit = false;
		this.gcCardCode.OptionsColumn.ReadOnly = true;
		this.gridColumn1.Caption = "Currency";
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Currency";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn2.Caption = "Base Currency";
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "BaseCurrency";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridBand2.Caption = "Primary Currency";
		this.gridBand2.Columns.Add(this.gcBasePrice);
		this.gridBand2.Columns.Add(this.gcPrice);
		this.gridBand2.Columns.Add(this.gcOvrwritten);
		this.gridBand2.Name = "gridBand2";
		this.gridBand2.VisibleIndex = 1;
		this.gridBand2.Width = 306;
		this.gcBasePrice.Caption = "Base Price";
		this.gcBasePrice.CurrencyField = null;
		this.gcBasePrice.CurrencyFixed = null;
		this.gcBasePrice.CurrencyFixedRate = 0.0;
		this.gcBasePrice.CurrencyRateField = null;
		this.gcBasePrice.FieldName = "BasePrice";
		this.gcBasePrice.Name = "gcBasePrice";
		this.gcBasePrice.OptionsColumn.AllowEdit = false;
		this.gcBasePrice.OptionsColumn.ReadOnly = true;
		this.gcBasePrice.Visible = true;
		this.gcBasePrice.Width = 108;
		this.gcPrice.Caption = "Unit Price";
		this.gcPrice.ColumnEdit = this.rep_Price;
		this.gcPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice.CurrencyField = "Currency";
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.Visible = true;
		this.gcPrice.Width = 114;
		this.rep_Price.AutoHeight = false;
		this.rep_Price.Mask.UseMaskAsDisplayFormat = true;
		this.rep_Price.Name = "rep_Price";
		this.gcOvrwritten.Caption = "Manual";
		this.gcOvrwritten.ColumnEdit = this.rep_chk_Manual;
		this.gcOvrwritten.CurrencyField = null;
		this.gcOvrwritten.CurrencyFixed = null;
		this.gcOvrwritten.CurrencyFixedRate = 0.0;
		this.gcOvrwritten.CurrencyRateField = null;
		this.gcOvrwritten.FieldName = "Ovrwritten";
		this.gcOvrwritten.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOvrwritten.Name = "gcOvrwritten";
		this.gcOvrwritten.Visible = true;
		this.gcOvrwritten.Width = 84;
		this.rep_chk_Manual.AutoHeight = false;
		this.rep_chk_Manual.DisplayValueChecked = "Y";
		this.rep_chk_Manual.DisplayValueGrayed = "N";
		this.rep_chk_Manual.DisplayValueUnchecked = "N";
		this.rep_chk_Manual.Name = "rep_chk_Manual";
		this.rep_chk_Manual.ValueChecked = "Y";
		this.rep_chk_Manual.ValueGrayed = "N";
		this.rep_chk_Manual.ValueUnchecked = "N";
		this.rep_chk_Manual.CheckedChanged += new System.EventHandler(this.rep_chk_Manuals_CheckedChanged);
		this.gridBand3.Caption = "Additional Currency 1";
		this.gridBand3.Columns.Add(this.gridColumn3);
		this.gridBand3.Columns.Add(this.gcAddPrice1);
		this.gridBand3.Columns.Add(this.gridColumn5);
		this.gridBand3.Name = "gridBand3";
		this.gridBand3.VisibleIndex = 2;
		this.gridBand3.Width = 270;
		this.gridColumn3.Caption = "Base Price";
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "BasePrice1";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.Visible = true;
		this.gridColumn3.Width = 83;
		this.gcAddPrice1.Caption = "Unit Price";
		this.gcAddPrice1.ColumnEdit = this.rep_AddPrice1;
		this.gcAddPrice1.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcAddPrice1.CurrencyField = "Currency1";
		this.gcAddPrice1.CurrencyFixed = null;
		this.gcAddPrice1.CurrencyFixedRate = 0.0;
		this.gcAddPrice1.CurrencyRateField = null;
		this.gcAddPrice1.FieldName = "AddPrice1";
		this.gcAddPrice1.Name = "gcAddPrice1";
		this.gcAddPrice1.Visible = true;
		this.gcAddPrice1.Width = 84;
		this.rep_AddPrice1.AutoHeight = false;
		this.rep_AddPrice1.Mask.UseMaskAsDisplayFormat = true;
		this.rep_AddPrice1.Name = "rep_AddPrice1";
		this.gridColumn5.Caption = "Manual";
		this.gridColumn5.ColumnEdit = this.rep_chk_Manual1;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Ovrwrite1";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.Visible = true;
		this.gridColumn5.Width = 103;
		this.rep_chk_Manual1.AutoHeight = false;
		this.rep_chk_Manual1.DisplayValueChecked = "Y";
		this.rep_chk_Manual1.DisplayValueGrayed = "N";
		this.rep_chk_Manual1.DisplayValueUnchecked = "N";
		this.rep_chk_Manual1.Name = "rep_chk_Manual1";
		this.rep_chk_Manual1.ValueChecked = "Y";
		this.rep_chk_Manual1.ValueGrayed = "N";
		this.rep_chk_Manual1.ValueUnchecked = "N";
		this.rep_chk_Manual1.CheckedChanged += new System.EventHandler(this.rep_chk_Manuals_CheckedChanged);
		this.gridBand4.Caption = "Additional Currency 2";
		this.gridBand4.Columns.Add(this.gridColumn6);
		this.gridBand4.Columns.Add(this.gcAddPrice2);
		this.gridBand4.Columns.Add(this.gridColumn8);
		this.gridBand4.Name = "gridBand4";
		this.gridBand4.VisibleIndex = 3;
		this.gridBand4.Width = 330;
		this.gridColumn6.Caption = "Base Price";
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "BasePrice2";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowEdit = false;
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.Visible = true;
		this.gridColumn6.Width = 103;
		this.gcAddPrice2.Caption = "Unit Price";
		this.gcAddPrice2.ColumnEdit = this.rep_AddPrice2;
		this.gcAddPrice2.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcAddPrice2.CurrencyField = "Currency2";
		this.gcAddPrice2.CurrencyFixed = null;
		this.gcAddPrice2.CurrencyFixedRate = 0.0;
		this.gcAddPrice2.CurrencyRateField = null;
		this.gcAddPrice2.FieldName = "AddPrice2";
		this.gcAddPrice2.Name = "gcAddPrice2";
		this.gcAddPrice2.Visible = true;
		this.gcAddPrice2.Width = 107;
		this.rep_AddPrice2.AutoHeight = false;
		this.rep_AddPrice2.Mask.UseMaskAsDisplayFormat = true;
		this.rep_AddPrice2.Name = "rep_AddPrice2";
		this.gridColumn8.Caption = "Manual";
		this.gridColumn8.ColumnEdit = this.rep_chk_Manual2;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Ovrwrite2";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.Visible = true;
		this.gridColumn8.Width = 120;
		this.rep_chk_Manual2.AutoHeight = false;
		this.rep_chk_Manual2.DisplayValueChecked = "Y";
		this.rep_chk_Manual2.DisplayValueGrayed = "N";
		this.rep_chk_Manual2.DisplayValueUnchecked = "N";
		this.rep_chk_Manual2.Name = "rep_chk_Manual2";
		this.rep_chk_Manual2.ValueChecked = "Y";
		this.rep_chk_Manual2.ValueGrayed = "N";
		this.rep_chk_Manual2.ValueUnchecked = "N";
		this.rep_chk_Manual2.CheckedChanged += new System.EventHandler(this.rep_chk_Manuals_CheckedChanged);
		this.gridBand5.Columns.Add(this.gcInvntryUom);
		this.gridBand5.Name = "gridBand5";
		this.gridBand5.VisibleIndex = 4;
		this.gridBand5.Width = 97;
		this.gcInvntryUom.Caption = "Inventory UoM";
		this.gcInvntryUom.CurrencyField = null;
		this.gcInvntryUom.CurrencyFixed = null;
		this.gcInvntryUom.CurrencyFixedRate = 0.0;
		this.gcInvntryUom.CurrencyRateField = null;
		this.gcInvntryUom.FieldName = "InvntryUom";
		this.gcInvntryUom.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcInvntryUom.Name = "gcInvntryUom";
		this.gcInvntryUom.OptionsColumn.AllowEdit = false;
		this.gcInvntryUom.OptionsColumn.ReadOnly = true;
		this.gcInvntryUom.Visible = true;
		this.gcInvntryUom.Width = 97;
		this.bandedGridColumn1.Caption = "Currency1";
		this.bandedGridColumn1.CurrencyField = null;
		this.bandedGridColumn1.CurrencyFixed = null;
		this.bandedGridColumn1.CurrencyFixedRate = 0.0;
		this.bandedGridColumn1.CurrencyRateField = null;
		this.bandedGridColumn1.FieldName = "Currency1";
		this.bandedGridColumn1.Name = "bandedGridColumn1";
		this.bandedGridColumn1.OptionsColumn.AllowEdit = false;
		this.bandedGridColumn1.OptionsColumn.ReadOnly = true;
		this.bandedGridColumn3.Caption = "BaseCurrency1";
		this.bandedGridColumn3.CurrencyField = null;
		this.bandedGridColumn3.CurrencyFixed = null;
		this.bandedGridColumn3.CurrencyFixedRate = 0.0;
		this.bandedGridColumn3.CurrencyRateField = null;
		this.bandedGridColumn3.FieldName = "BaseCurrency1";
		this.bandedGridColumn3.Name = "bandedGridColumn3";
		this.bandedGridColumn3.OptionsColumn.AllowEdit = false;
		this.bandedGridColumn3.OptionsColumn.ReadOnly = true;
		this.bandedGridColumn2.Caption = "Currency2";
		this.bandedGridColumn2.CurrencyField = null;
		this.bandedGridColumn2.CurrencyFixed = null;
		this.bandedGridColumn2.CurrencyFixedRate = 0.0;
		this.bandedGridColumn2.CurrencyRateField = null;
		this.bandedGridColumn2.FieldName = "Currency2";
		this.bandedGridColumn2.Name = "bandedGridColumn2";
		this.bandedGridColumn2.OptionsColumn.AllowEdit = false;
		this.bandedGridColumn2.OptionsColumn.ReadOnly = true;
		this.bandedGridColumn4.Caption = "BaseCurrency2";
		this.bandedGridColumn4.CurrencyField = null;
		this.bandedGridColumn4.CurrencyFixed = null;
		this.bandedGridColumn4.CurrencyFixedRate = 0.0;
		this.bandedGridColumn4.CurrencyRateField = null;
		this.bandedGridColumn4.FieldName = "BaseCurrency2";
		this.bandedGridColumn4.Name = "bandedGridColumn4";
		this.bandedGridColumn4.OptionsColumn.AllowEdit = false;
		this.bandedGridColumn4.OptionsColumn.ReadOnly = true;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(85, 447);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(7, 447);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 3;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.btn_SelCriterial.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SelCriterial.EditControl = null;
		this.btn_SelCriterial.EditValue = "Selection Criterial";
		this.btn_SelCriterial.FieldName = null;
		this.btn_SelCriterial.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SelCriterial.ImageOptions.Image");
		this.btn_SelCriterial.IsSearchControl = false;
		this.btn_SelCriterial.Location = new System.Drawing.Point(765, 447);
		this.btn_SelCriterial.Name = "btn_SelCriterial";
		this.btn_SelCriterial.ReadOnly = false;
		this.btn_SelCriterial.SettingInVisible = false;
		this.btn_SelCriterial.SettingReadonly = false;
		this.btn_SelCriterial.Size = new System.Drawing.Size(117, 23);
		this.btn_SelCriterial.TabIndex = 6;
		this.btn_SelCriterial.Text = "Selection Criterial";
		this.btn_SelCriterial.Click += new System.EventHandler(this.btn_SelCriterial_Click);
		this.btn_Clear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Clear.EditControl = null;
		this.btn_Clear.EditValue = "Clear";
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(687, 447);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.ReadOnly = false;
		this.btn_Clear.SettingInVisible = false;
		this.btn_Clear.SettingReadonly = false;
		this.btn_Clear.Size = new System.Drawing.Size(75, 23);
		this.btn_Clear.TabIndex = 5;
		this.btn_Clear.Text = "Clear";
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Find";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(7, 11);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(106, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 7;
		this.lb_Find.Utext = "Find";
		this.txt_Find.EditControl = null;
		this.txt_Find.FieldName = null;
		this.txt_Find.IsSearchControl = false;
		this.txt_Find.Location = new System.Drawing.Point(109, 8);
		this.txt_Find.Name = "txt_Find";
		this.txt_Find.SettingInVisible = false;
		this.txt_Find.SettingReadonly = false;
		this.txt_Find.Size = new System.Drawing.Size(130, 20);
		this.txt_Find.TabIndex = 8;
		this.txt_Find.EditValueChanged += new System.EventHandler(this.txt_Find_EditValueChanged);
		this.chk_IsGrossPrice.EditControl = null;
		this.chk_IsGrossPrice.FieldName = null;
		this.chk_IsGrossPrice.IsSearchControl = false;
		this.chk_IsGrossPrice.Location = new System.Drawing.Point(258, 9);
		this.chk_IsGrossPrice.Name = "chk_IsGrossPrice";
		this.chk_IsGrossPrice.Properties.Caption = "Gross Price";
		this.chk_IsGrossPrice.SettingInVisible = false;
		this.chk_IsGrossPrice.SettingReadonly = false;
		this.chk_IsGrossPrice.Size = new System.Drawing.Size(75, 20);
		this.chk_IsGrossPrice.TabIndex = 9;
		this.chk_IsGrossPrice.CheckedChanged += new System.EventHandler(this.chk_IsGrossPrice_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(890, 476);
		base.Controls.Add(this.chk_IsGrossPrice);
		base.Controls.Add(this.txt_Find);
		base.Controls.Add(this.lb_Find);
		base.Controls.Add(this.btn_SelCriterial);
		base.Controls.Add(this.btn_Clear);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oPriceListDetails";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_STDPriceListDetails";
		this.Text = "Price Lists";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_basePricelist).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AddPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AddPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Manual2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsGrossPrice.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
