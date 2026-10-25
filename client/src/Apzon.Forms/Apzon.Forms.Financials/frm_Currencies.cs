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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Financials;

public class frm_Currencies : XtraFormEx
{
	private const string tableName = "OCRN";

	private List<string> lstChanged = new List<string>();

	public DataTable dt = new DataTable();

	private DataTable dtObj = new DataTable();

	private DataTable dtOriginal = new DataTable();

	public static Dictionary<string, string> _ListISOCurrCod = new Dictionary<string, string>
	{
		{ "AED", "United Arab Emirates, Dirhams" },
		{ "AFN", "Afghanistan, Afghanis" },
		{ "ALL", "Albania, Leke" },
		{ "AMD", "Armenia, Drams" },
		{ "ANG", "Netherlands Antilles, Guilders (also called Florins)" },
		{ "AOA", "Angola, Kwanzas" },
		{ "ARS", "Argentina, Pesos" },
		{ "AUD", "Australia, Dollars" },
		{ "AWG", "Aruba, Guilders (also called Florins)" },
		{ "AZM", "Azerbaijan, Manats [obsolete]" },
		{ "AZN", "Azerbaijan, New Manats" },
		{ "BAM", "Bosnia and Herzegovina, Convertible Marks" },
		{ "BBD", "Barbados, Dollars" },
		{ "BDT", "Bangladesh, Taka " },
		{ "BGN", "Bulgaria, Leva" },
		{ "BHD", "Bahrain, Dinars" },
		{ "BIF", "Burundi, Francs" },
		{ "BMD", "Bermuda, Dollars" },
		{ "BND", "Brunei, Ringgits" },
		{ "BOB", "Bolivia, Bolivianos" },
		{ "BRL", "Brazil, Brazilian Reals" },
		{ "BSD", "Bahamas, Dollars" },
		{ "BTN", "Bhutan, Ngultrum" },
		{ "BWP", "Botswana, Pulas" },
		{ "BYR", "Belarus, Rubles " },
		{ "BZD", "Belize, Dollars" },
		{ "CAD", "Canada, Dollars" },
		{ "CDF", "Congo/Kinshasa, Congolese Francs" },
		{ "CHF", "Switzerland, Francs" },
		{ "CLP", "Chile, Pesos" },
		{ "CNY", "China, Yuan Renminbi" },
		{ "COP", "Colombia, Pesos" },
		{ "CRC", "Costa Rica, Colones" },
		{ "CUP", "Cuba, Pesos" },
		{ "CVE", "Cape Verde, Escudos" },
		{ "CYP", "Cyprus, Pounds" },
		{ "CZK", "Czech Republic, Koruny" },
		{ "DJF", "Djibouti, Francs" },
		{ "DKK", "Denmark, Kroner" },
		{ "DOP", "Dominican Republic, Pesos" },
		{ "DZD", "Algeria, Algerian Dinars" },
		{ "EEK", "Estonia, Krooni" },
		{ "EGP", "Egypt, Pounds" },
		{ "ERN", "Eritrea, Nakfa" },
		{ "ETB", "Ethiopia, Birr" },
		{ "EUR", "EU Member Countries, Euro" },
		{ "FJD", "Fiji, Dollars" },
		{ "FKP", "Falkland Islands (Malvinas), Pounds" },
		{ "GBP", "United Kingdom, Pounds" },
		{ "GEL", "Georgia, Lari" },
		{ "GGP", "Guernsey, Pounds" },
		{ "GHC", "Ghana, Cedis" },
		{ "GHS", "Ghana, Cedis" },
		{ "GIP", "Gibraltar, Pounds" },
		{ "GMD", "Gambia, Dalasi" },
		{ "GNF", "Guinea, Francs" },
		{ "GTQ", "Guatemala, Quetzales" },
		{ "GYD", "Guyana, Dollars" },
		{ "HKD", "Hong Kong, Dollars" },
		{ "HNL", "Honduras, Lempiras" },
		{ "HRK", "Croatia, Kuna" },
		{ "HTG", "Haiti, Gourdes" },
		{ "HUF", "Hungary, Forint" },
		{ "IDR", "Indonesia, Rupiahs" },
		{ "ILS", "Israel, New Shekels" },
		{ "IMP", "Isle of Man, Pounds" },
		{ "INR", "India, Rupees" },
		{ "IQD", "Iraq, Dinars" },
		{ "IRR", "Iran, Rials" },
		{ "ISK", "Iceland, Kronur" },
		{ "JEP", "Jersey, Pounds" },
		{ "JMD", "Jamaica, Dollars" },
		{ "JOD", "Jordan, Dinars" },
		{ "JPY", "Japan, Yen" },
		{ "KES", "Kenya, Shillings" },
		{ "KGS", "Kyrgyzstan, Soms" },
		{ "KHR", "Cambodia, Riels" },
		{ "KMF", "Comoros, Francs" },
		{ "KPW", "Korea (North), Won" },
		{ "KRW", "Korea (South), Won" },
		{ "KWD", "Kuwait, Dinars" },
		{ "KYD", "Cayman Islands, Dollars" },
		{ "KZT", "Kazakhstan, Tenge" },
		{ "LAK", "Laos, Kips" },
		{ "LBP", "Lebanon, Pounds" },
		{ "LKR", "Sri Lanka, Rupees" },
		{ "LRD", "Liberia, Dollars" },
		{ "LSL", "Lesotho, Maloti" },
		{ "LTL", "Lithuania, Litai" },
		{ "LVL", "Latvia, Lati" },
		{ "LYD", "Libya, Dinars" },
		{ "MAD", "Morocco, Dirhams" },
		{ "MDL", "Moldova, Lei" },
		{ "MGA", "Madagascar, Ariary" },
		{ "MKD", "Macedonia, Denars" },
		{ "MMK", "Myanmar (Burma), Kyats" },
		{ "MNT", "Mongolia, Tugriks" },
		{ "MOP", "Macau, Patacas" },
		{ "MRO", "Mauritania, Ouguiyas" },
		{ "MTL", "Malta, Liri" },
		{ "MUR", "Mauritius, Rupees" },
		{ "MVR", "Maldives (Maldive Islands), Rufiyaa" },
		{ "MWK", "Malawi, Kwachas" },
		{ "MXN", "Mexico, Pesos" },
		{ "MYR", "Malaysia, Ringgits" },
		{ "MZM", "Mozambique, Meticais [obsolete]" },
		{ "MZN", "Mozambique, Meticais [newer unit, same name]" },
		{ "NAD", "Namibia, Dollars" },
		{ "NGN", "Nigeria, Nairas" },
		{ "NIO", "Nicaragua, Cordobas" },
		{ "NOK", "Norway, Krone" },
		{ "NPR", "Nepal, Nepal Rupees" },
		{ "NZD", "New Zealand, Dollars" },
		{ "OMR", "Oman, Rials" },
		{ "PAB", "Panama, Balboa" },
		{ "PEN", "Peru, Nuevos Soles" },
		{ "PGK", "Papua New Guinea, Kina" },
		{ "PHP", "Philippines, Pesos" },
		{ "PKR", "Pakistan, Rupees" },
		{ "PLN", "Poland, Zlotych" },
		{ "PYG", "Paraguay, Guarani" },
		{ "QAR", "Qatar, Rials" },
		{ "ROL", "Romania, Lei [obsolete]" },
		{ "RON", "Romania, New Lei" },
		{ "RSD", "Serbia, Dinars" },
		{ "RUB", "Russia, Rubles" },
		{ "RWF", "Rwanda, Rwandan Francs" },
		{ "SAR", "Saudi Arabia, Riyals" },
		{ "SBD", "Solomon Islands, Dollars" },
		{ "SCR", "Seychelles, Rupees" },
		{ "SDD", "Sudan, Dinars [obsolete]" },
		{ "SDG", "Sudan, Pounds" },
		{ "SEK", "Sweden, Kronor" },
		{ "SGD", "Singapore, Dollars" },
		{ "SHP", "Saint Helena, Pounds" },
		{ "SIT", "Slovenia, Tolars [obsolete]" },
		{ "SKK", "Slovakia, Koruny" },
		{ "SLL", "Sierra Leone, Leones" },
		{ "SOS", "Somalia, Shillings" },
		{ "SPL", "Seborga, Luigini" },
		{ "SRD", "Suriname, Dollars" },
		{ "STD", "Sao Tome and Principe, Dobras" },
		{ "SVC", "El Salvador, Colones" },
		{ "SYP", "Syria, Pounds" },
		{ "SZL", "Swaziland, Emalangeni" },
		{ "THB", "Thailand, Baht" },
		{ "TJS", "Tajikistan, Somoni" },
		{ "TMM", "Turkmenistan, Manats" },
		{ "TND", "Tunisia, Dinars" },
		{ "TOP", "Tonga, Pa'anga" },
		{ "TRY", "Turkey, New Lira" },
		{ "TTD", "Trinidad and Tobago, Dollars" },
		{ "TVD", "Tuvalu, Tuvalu Dollars" },
		{ "TWD", "Taiwan, New Dollars" },
		{ "TZS", "Tanzania, Shillings" },
		{ "UAH", "Ukraine, Hryvnia" },
		{ "UGX", "Uganda, Shillings" },
		{ "USD", "United States of America, Dollars" },
		{ "UYU", "Uruguay, Pesos" },
		{ "UZS", "Uzbekistan, Sums" },
		{ "VEB", "Venezuela, Bolivares" },
		{ "VND", "Viet Nam, Dong" },
		{ "VUV", "Vanuatu, Vatu" },
		{ "WST", "Samoa, Tala" },
		{ "XAF", "Communaute Financiere Africaine BEAC, Francs" },
		{ "XAG", "Silver, Ounces" },
		{ "XAU", "Gold, Ounces" },
		{ "XCD", "East Caribbean Dollars" },
		{ "XDR", "International Monetary Fund (IMF) Special Drawing Rights" },
		{ "XOF", "Communaute Financiere Africaine BCEAO, Francs" },
		{ "XPD", "Palladium Ounces" },
		{ "XPF", "Comptoirs Francais du Pacifique Francs" },
		{ "XPT", "Platinum, Ounces" },
		{ "YER", "Yemen, Rials" },
		{ "ZAR", "South Africa, Rand" },
		{ "ZMK", "Zambia, Kwacha" },
		{ "ZWD", "Zimbabwe, Zimbabwe Dollars" }
	};

	private DataView clone;

	private string _privilege = "";

	private IContainer components;

	private GridControlEx grd_Currencies;

	private GridViewEx gv_Currencies;

	private GridColumnEx gcCurrCode;

	private GridColumnEx gcCurrName;

	private GridColumnEx gcDocCurrCod;

	private GridColumnEx gcChkName;

	private GridColumnEx gcChk100Name;

	private GridColumnEx gcFrgnName;

	private GridColumnEx gcF100Name;

	private GridColumnEx gcISOCurrCod;

	private GridColumnEx gcMaxInDiff;

	private GridColumnEx gcMaxOutDiff;

	private GridColumnEx gcMaxInPcnt;

	private GridColumnEx gcMaxOutPcnt;

	private GridColumnEx gcRoundSys;

	private GridColumnEx gcDecimals;

	private GridColumnEx gcRoundPym;

	private RepositoryItemCheckEdit rIChk_RoundPym;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private RepositoryItemLookUpEdit rILUE_ISOCurrCod;

	private RepositoryItemLookUpEdit rILUE_Decimal;

	private RepositoryItemLookUpEdit rILUE_Rounding;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gcLocked;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcUserSign2;

	private GridColumnEx gcISRCalc;

	private GridColumnEx gcConvUnit;

	private GridColumnEx gcBaseCurr;

	private GridColumnEx gcFactor;

	private GridColumnEx gcChkNamePI;

	private GridColumnEx gcChk100NPI;

	private GridColumnEx gcFrgnNamePI;

	private GridColumnEx gcF100NamePI;

	private GridColumnEx gcISOCurrNum;

	private RepositoryItemCalcEdit rep_MaxInDiff;

	private RepositoryItemCalcEdit rep_MaxOutDiff;

	private RepositoryItemCalcEdit rep_MaxInPcnt;

	private RepositoryItemCalcEdit rep_MaxOutPcnt;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			bt_Cancel.Enabled = true;
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

	public frm_Currencies()
	{
		InitializeComponent();
	}

	private void gv_Currencies_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void frm_Currencies_Load(object sender, EventArgs e)
	{
		await RefreshData();
	}

	private async void gv_Currencies_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Currencies.DataSourceTable;
			if (dataSourceTable.IsNotNull() && (e.Column.FieldName == "CurrCode" || e.Column.FieldName == "CurrName" || e.Column.FieldName == "DocCurrCod" || e.Column.FieldName == "ChkName" || e.Column.FieldName == "Chk100Name" || e.Column.FieldName == "FrgnName" || e.Column.FieldName == "F100Name" || e.Column.FieldName == "ISOCurrCod" || e.Column.FieldName == "MaxInDiff" || e.Column.FieldName == "RoundSys" || e.Column.FieldName == "Decimals" || e.Column.FieldName == "RoundPym"))
			{
				_ = e.RowHandle;
				_ = dataSourceTable.Rows.Count - 1;
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Currencies_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Currencies.FocusedColumn.FieldName.Equals(gcCurrCode.FieldName) && Function.ToString(gv_Currencies.GetFocusedRowCellValue("IsExist")).Equals("Y"))
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes || !dt.IsNotNull())
			{
				return;
			}
			string text = Function.ToString(gv_Currencies.GetFocusedRowCellValue("CurrCode"));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string text2 = ExistInDocument(text);
			if (string.IsNullOrEmpty(text2))
			{
				using (HttpClientHelper<HttpResult> hr = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Currency"))
				{
					string pr = Function.ToString(gv_Currencies.GetRowCellValue(gv_Currencies.FocusedRowHandle, "CurrCode"));
					WinCommons._OpenProcessing();
					HttpResult httpResult = await hr.PostAsync("Remove", pr);
					WinCommons._CloseProcessing();
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
					}
				}
				gv_Currencies.BeginUpdate();
				gv_Currencies.DeleteSelectedRows();
				gv_Currencies.EndUpdate();
				DataTable dataTable = (DataTable)grd_Currencies.DataSource;
				dataTable.AcceptChanges();
				grd_Currencies.DataSource = dataTable;
				grd_Currencies.RefreshDataSource();
			}
			else
			{
				string text3 = Function.ToString(text2.Split(',')[0]);
				WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
				Logging.Write("ERROR", "Can not remove this currency because it has been used in table " + text3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				await Add();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void bt_Cancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			await Add();
		}
		Close();
	}

	private async void gv_Currencies_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Currencies.DataSource as DataTable;
			if (dataTable.IsNotNull() && (e.Column.FieldName == "CurrCode" || e.Column.FieldName == "CurrName" || e.Column.FieldName == "DocCurrCod" || e.Column.FieldName == "ChkName" || e.Column.FieldName == "Chk100Name" || e.Column.FieldName == "FrgnName" || e.Column.FieldName == "F100Name" || e.Column.FieldName == "ISOCurrCod" || e.Column.FieldName == "MaxInDiff" || e.Column.FieldName == "RoundSys" || e.Column.FieldName == "Decimals" || e.Column.FieldName == "RoundPym"))
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					await AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void grd_Currencies_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				await PasteFromExcelToDataTable(grd_Currencies, gv_Currencies, new List<string> { "CurrCode" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task PasteFromExcelToDataTable(GridControl grd, GridView gv, List<string> key)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(clipboardData);
			if (dataTable != null && dataTable.Rows.Count == 1 && dataTable.Columns.Count > 0)
			{
				int num = gv.FocusedRowHandle;
				GridColumn focusedColumn = gv.FocusedColumn;
				if (key.Contains(focusedColumn.FieldName))
				{
					int focusedRowHandle = gv.FocusedRowHandle;
					for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow row = grd.DataSourceTable().NewRow();
						grd.DataSourceTable().Rows.InsertAt(row, focusedRowHandle);
						grd.DataSourceTable().AcceptChanges();
						int colunmIndex = focusedColumn.VisibleIndex;
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							gv.FocusedRowHandle = focusedRowHandle;
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
							if (gridColumn == null)
							{
								break;
							}
							gv.SetRowCellValue(num, gridColumn, dataTable.Rows[num2][i]);
							colunmIndex++;
						}
					}
				}
				else
				{
					int focusedRowHandle2 = gv.FocusedRowHandle;
					for (int num3 = focusedRowHandle2; num3 < gv.RowCount; num3++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						bool flag = true;
						for (int num4 = 0; num4 < key.Count; num4++)
						{
							if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num3, key[num4]))))
							{
								flag = false;
								break;
							}
						}
						if (!flag || num3 - focusedRowHandle2 >= dataTable.Rows.Count)
						{
							break;
						}
						for (int num5 = 0; num5 < dataTable.Columns.Count; num5++)
						{
							GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn2 == null)
							{
								break;
							}
							gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle2][num5]);
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
				}
				Clipboard.Clear();
				gv_Currencies.SetFocusedRowCellValue("Locked", "N");
				gv_Currencies.SetFocusedRowCellValue("DataSource", "N");
				int num6 = await GetUserSignByUserCode_tableOUSR();
				gv_Currencies.SetFocusedRowCellValue("UserSign", num6);
				gv_Currencies.SetFocusedRowCellValue("UserSign2", num6);
				gv_Currencies.SetFocusedRowCellValue("ISRCalc", "N");
				gv_Currencies.SetFocusedRowCellValue("RoundPym", "N");
				gv_Currencies.SetFocusedRowCellValue("ConvUnit", "N");
				gv_Currencies.SetFocusedRowCellValue("RoundSys", 0);
				gv_Currencies.SetFocusedRowCellValue("Decimals", -1);
				gv_Currencies.SetFocusedRowCellValue("Factor", 0);
				gv_Currencies.SetFocusedRowCellValue("MaxInDiff", 0);
				gv_Currencies.SetFocusedRowCellValue("MaxOutDiff", 0);
				gv_Currencies.SetFocusedRowCellValue("MaxOutPcnt", 0);
				gv_Currencies.SetFocusedRowCellValue("MaxInPcnt", 0);
				grd.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.TheTargetTableCannotAddNewRowAutomaticallyContentBelowRowNotCopied, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void GetListRouding()
	{
		List<NameID_Short> list = new List<NameID_Short>();
		list.Add(new NameID_Short
		{
			Name = "No Rounding",
			ID = 0
		});
		list.Add(new NameID_Short
		{
			Name = "Round to Ten Hundredth",
			ID = 1
		});
		list.Add(new NameID_Short
		{
			Name = "Round to One",
			ID = 2
		});
		list.Add(new NameID_Short
		{
			Name = "Round to Ten",
			ID = 3
		});
		list.Add(new NameID_Short
		{
			Name = "Round to Five Hundredth",
			ID = 4
		});
		rILUE_Rounding.DataSource = list;
	}

	private void GetListDecimals()
	{
		List<NameID_Short> list = new List<NameID_Short>();
		list.Add(new NameID_Short
		{
			Name = "Default",
			ID = -1
		});
		list.Add(new NameID_Short
		{
			Name = "Without Decimals",
			ID = 0
		});
		list.Add(new NameID_Short
		{
			Name = "1 Digit",
			ID = 1
		});
		list.Add(new NameID_Short
		{
			Name = "2 Digits",
			ID = 2
		});
		list.Add(new NameID_Short
		{
			Name = "3 Digits",
			ID = 3
		});
		list.Add(new NameID_Short
		{
			Name = "4 Digits",
			ID = 4
		});
		list.Add(new NameID_Short
		{
			Name = "5 Digits",
			ID = 5
		});
		list.Add(new NameID_Short
		{
			Name = "6 Digits",
			ID = 6
		});
		rILUE_Decimal.DataSource = list;
	}

	private async Task RefreshData()
	{
		GetListDecimals();
		GetListISOCurrCod();
		GetListRouding();
		await LoadData();
	}

	private void GetListISOCurrCod()
	{
		try
		{
			List<Common.Table_Code> dataSource = _ListISOCurrCod.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rILUE_ISOCurrCod.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Currency");
			DataTable dataTable = await hp.GetDataTableAsync("GetData");
			dtOriginal = dataTable.Copy();
			grd_Currencies.DataSource = dataTable;
			await AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Currency");
			DataTable result = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = "OCRN",
				codeMax = ""
			});
			if (result.IsNotNull())
			{
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private async Task AddNewRow()
	{
		_ = 4;
		try
		{
			dt = grd_Currencies.DataSource as DataTable;
			if (!dt.IsNotNull())
			{
				dt = (await GetDtAddNew()).Clone();
				DataRow dr = dt.NewRow();
				dr["Locked"] = "N";
				dr["DataSource"] = "N";
				DataRow dataRow = dr;
				dataRow["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				dr["RoundSys"] = 0;
				dataRow = dr;
				dataRow["UserSign2"] = await GetUserSignByUserCode_tableOUSR();
				dr["Decimals"] = -1;
				dr["ISRCalc"] = "N";
				dr["RoundPym"] = "N";
				dr["ConvUnit"] = "N";
				dr["Factor"] = 0;
				dr["MaxInDiff"] = 0;
				dr["MaxOutDiff"] = 0;
				dr["MaxInPcnt"] = 0;
				dr["MaxOutPcnt"] = 0;
				dr["ISOCurrNum"] = string.Empty;
				dr["IsExist"] = "N";
				dt.Rows.Add(dr);
				dt.AcceptChanges();
				grd_Currencies.DataSource = dt;
				grd_Currencies.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[dt.Rows.Count - 1]["CurrCode"])))
			{
				DataRow dr = dt.NewRow();
				dr["Locked"] = "N";
				dr["DataSource"] = "N";
				DataRow dataRow = dr;
				dataRow["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				dr["RoundSys"] = 0;
				dataRow = dr;
				dataRow["UserSign2"] = await GetUserSignByUserCode_tableOUSR();
				dr["Decimals"] = -1;
				dr["ISRCalc"] = "N";
				dr["RoundPym"] = "N";
				dr["ConvUnit"] = "N";
				dr["Factor"] = 0;
				dr["MaxInDiff"] = 0;
				dr["MaxOutDiff"] = 0;
				dr["MaxInPcnt"] = 0;
				dr["MaxOutPcnt"] = 0;
				dr["ISOCurrNum"] = string.Empty;
				dt.Rows.Add(dr);
				dt.AcceptChanges();
				grd_Currencies.DataSource = dt;
				grd_Currencies.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string ExistInDocument(string curr)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Currency");
			return httpClientHelper.Get("CheckExistGroupCode", new { curr });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task Add()
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrWhiteSpace(CheckValid()))
			{
				WinCommons.Error(Enums.MessageCode.CharacterNumberIsGreaterThanAllowed, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataTable dataSourceTable = grd_Currencies.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					DataTable dataTable = dataSourceTable.Copy();
					DataRow[] array = (from t in dataTable.AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(t["CurrCode"]))
						select t).ToArray();
					if (array.Length != 0)
					{
						dataTable = array.CopyToDataTable();
					}
					if ((from i in dataTable.AsEnumerable()
						group i by Function.ToString(i.Field<string>("CurrCode")).ToUpper() into gr
						where gr.Count() > 1
						select gr).ToList().Count > 0)
					{
						WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableCurrencyCode, "");
						return;
					}
					if ((from t in dataTable.AsEnumerable()
						where string.IsNullOrEmpty(Function.ToString(t["DocCurrCod"]))
						select t).ToArray().Length != 0)
					{
						WinCommons.Error(Enums.MessageCode.EnterValidCodeInternationalCode, "");
						return;
					}
					using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Currency");
					HttpResult httpResult = hp.Post("Save", dataTable);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), httpResult.Message);
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormMode = Enums.FormViewModeType.View;
						await RefreshData();
						await LoadData();
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string CheckValid()
	{
		string result = "";
		try
		{
			for (int i = 0; i < gv_Currencies.RowCount; i++)
			{
				if (Function.ToString(gv_Currencies.GetRowCellValue(i, "CurrCode")).Length > 3)
				{
					result = Function.ToString(gv_Currencies.GetRowCellValue(i, "CurrCode"));
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_Currencies));
		this.grd_Currencies = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Currencies = new Apzon.Controls.Grid.GridViewEx();
		this.gcCurrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCurrName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCurrCod = new Apzon.Controls.Grid.GridColumnEx();
		this.gcChkName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcChk100Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFrgnName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcF100Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gcISOCurrCod = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_ISOCurrCod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcMaxInDiff = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxInDiff = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcMaxOutDiff = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxOutDiff = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcMaxInPcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxInPcnt = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcMaxOutPcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxOutPcnt = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcRoundSys = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_Rounding = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDecimals = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_Decimal = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRoundPym = new Apzon.Controls.Grid.GridColumnEx();
		this.rIChk_RoundPym = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcISRCalc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcConvUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFactor = new Apzon.Controls.Grid.GridColumnEx();
		this.gcChkNamePI = new Apzon.Controls.Grid.GridColumnEx();
		this.gcChk100NPI = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFrgnNamePI = new Apzon.Controls.Grid.GridColumnEx();
		this.gcF100NamePI = new Apzon.Controls.Grid.GridColumnEx();
		this.gcISOCurrNum = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Currencies).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Currencies).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_ISOCurrCod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxInDiff).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxOutDiff).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxInPcnt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxOutPcnt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Rounding).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Decimal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIChk_RoundPym).BeginInit();
		base.SuspendLayout();
		this.grd_Currencies.AllowPasteExcel = true;
		this.grd_Currencies.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Currencies.ClipboardData = null;
		this.grd_Currencies.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Currencies.Location = new System.Drawing.Point(3, 1);
		this.grd_Currencies.MainView = this.gv_Currencies;
		this.grd_Currencies.Name = "grd_Currencies";
		this.grd_Currencies.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rIChk_RoundPym, this.rILUE_ISOCurrCod, this.rILUE_Decimal, this.rILUE_Rounding, this.rep_MaxInDiff, this.rep_MaxOutDiff, this.rep_MaxInPcnt, this.rep_MaxOutPcnt });
		this.grd_Currencies.Size = new System.Drawing.Size(682, 295);
		this.grd_Currencies.TabIndex = 0;
		this.grd_Currencies.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Currencies });
		this.grd_Currencies.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Currencies_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Currencies.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Currencies.ColumnKey = null;
		this.gv_Currencies.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[28]
		{
			this.gcCurrCode, this.gcCurrName, this.gcDocCurrCod, this.gcChkName, this.gcChk100Name, this.gcFrgnName, this.gcF100Name, this.gcISOCurrCod, this.gcMaxInDiff, this.gcMaxOutDiff,
			this.gcMaxInPcnt, this.gcMaxOutPcnt, this.gcRoundSys, this.gcDecimals, this.gcRoundPym, this.gcLocked, this.gcDataSource, this.gcUserSign, this.gcUserSign2, this.gcISRCalc,
			this.gcConvUnit, this.gcBaseCurr, this.gcFactor, this.gcChkNamePI, this.gcChk100NPI, this.gcFrgnNamePI, this.gcF100NamePI, this.gcISOCurrNum
		});
		this.gv_Currencies.DisableFormSetting = false;
		this.gv_Currencies.DisplayException = false;
		this.gv_Currencies.EditColumn = null;
		this.gv_Currencies.FieldCurrency = null;
		this.gv_Currencies.FieldRate = null;
		this.gv_Currencies.FormId = "oCurrencies";
		this.gv_Currencies.GridControl = this.grd_Currencies;
		this.gv_Currencies.IgnoreFilter = false;
		this.gv_Currencies.IndexEditRow = -1;
		this.gv_Currencies.IndicatorWidth = 35;
		this.gv_Currencies.IsApzTable = false;
		this.gv_Currencies.IsInsertNewRow = false;
		this.gv_Currencies.IsLoadingData = false;
		this.gv_Currencies.IsMappingContextAction = false;
		this.gv_Currencies.ItemId = "OCRN";
		this.gv_Currencies.LstColumnAllowBatch = null;
		this.gv_Currencies.LstColumnNotAllowBatch = null;
		this.gv_Currencies.LstColumnNotEditable = null;
		this.gv_Currencies.LstColumNotSetting = null;
		this.gv_Currencies.Name = "gv_Currencies";
		this.gv_Currencies.OptionsView.ColumnAutoWidth = false;
		this.gv_Currencies.OptionsView.ShowGroupPanel = false;
		this.gv_Currencies.UseBatchMod = false;
		this.gv_Currencies.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Currencies_CustomDrawRowIndicator);
		this.gv_Currencies.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Currencies_ShowingEditor);
		this.gv_Currencies.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Currencies_CellValueChanged);
		this.gv_Currencies.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Currencies_CellValueChanging);
		this.gcCurrCode.Caption = "Code";
		this.gcCurrCode.CurrencyDateField = null;
		this.gcCurrCode.CurrencyField = null;
		this.gcCurrCode.CurrencyFixed = null;
		this.gcCurrCode.CurrencyFixedRate = 0.0;
		this.gcCurrCode.CurrencyRateField = null;
		this.gcCurrCode.FieldName = "CurrCode";
		this.gcCurrCode.Name = "gcCurrCode";
		this.gcCurrCode.PrimaryKey = true;
		this.gcCurrCode.UnitField = null;
		this.gcCurrCode.Visible = true;
		this.gcCurrCode.VisibleIndex = 0;
		this.gcCurrCode.Width = 40;
		this.gcCurrName.Caption = "Currency";
		this.gcCurrName.CurrencyDateField = null;
		this.gcCurrName.CurrencyField = null;
		this.gcCurrName.CurrencyFixed = null;
		this.gcCurrName.CurrencyFixedRate = 0.0;
		this.gcCurrName.CurrencyRateField = null;
		this.gcCurrName.FieldName = "CurrName";
		this.gcCurrName.Name = "gcCurrName";
		this.gcCurrName.PrimaryKey = false;
		this.gcCurrName.UnitField = null;
		this.gcCurrName.Visible = true;
		this.gcCurrName.VisibleIndex = 1;
		this.gcCurrName.Width = 90;
		this.gcDocCurrCod.Caption = "International Code";
		this.gcDocCurrCod.CurrencyDateField = null;
		this.gcDocCurrCod.CurrencyField = null;
		this.gcDocCurrCod.CurrencyFixed = null;
		this.gcDocCurrCod.CurrencyFixedRate = 0.0;
		this.gcDocCurrCod.CurrencyRateField = null;
		this.gcDocCurrCod.FieldName = "DocCurrCod";
		this.gcDocCurrCod.Name = "gcDocCurrCod";
		this.gcDocCurrCod.PrimaryKey = false;
		this.gcDocCurrCod.UnitField = null;
		this.gcDocCurrCod.Visible = true;
		this.gcDocCurrCod.VisibleIndex = 2;
		this.gcDocCurrCod.Width = 100;
		this.gcChkName.Caption = "International Description";
		this.gcChkName.CurrencyDateField = null;
		this.gcChkName.CurrencyField = null;
		this.gcChkName.CurrencyFixed = null;
		this.gcChkName.CurrencyFixedRate = 0.0;
		this.gcChkName.CurrencyRateField = null;
		this.gcChkName.FieldName = "ChkName";
		this.gcChkName.Name = "gcChkName";
		this.gcChkName.PrimaryKey = false;
		this.gcChkName.UnitField = null;
		this.gcChkName.Visible = true;
		this.gcChkName.VisibleIndex = 3;
		this.gcChkName.Width = 132;
		this.gcChk100Name.Caption = "Hunderth Name";
		this.gcChk100Name.CurrencyDateField = null;
		this.gcChk100Name.CurrencyField = null;
		this.gcChk100Name.CurrencyFixed = null;
		this.gcChk100Name.CurrencyFixedRate = 0.0;
		this.gcChk100Name.CurrencyRateField = null;
		this.gcChk100Name.FieldName = "Chk100Name";
		this.gcChk100Name.Name = "gcChk100Name";
		this.gcChk100Name.PrimaryKey = false;
		this.gcChk100Name.UnitField = null;
		this.gcChk100Name.Visible = true;
		this.gcChk100Name.VisibleIndex = 4;
		this.gcChk100Name.Width = 85;
		this.gcFrgnName.Caption = "English";
		this.gcFrgnName.CurrencyDateField = null;
		this.gcFrgnName.CurrencyField = null;
		this.gcFrgnName.CurrencyFixed = null;
		this.gcFrgnName.CurrencyFixedRate = 0.0;
		this.gcFrgnName.CurrencyRateField = null;
		this.gcFrgnName.FieldName = "FrgnName";
		this.gcFrgnName.Name = "gcFrgnName";
		this.gcFrgnName.PrimaryKey = false;
		this.gcFrgnName.UnitField = null;
		this.gcFrgnName.Visible = true;
		this.gcFrgnName.VisibleIndex = 5;
		this.gcFrgnName.Width = 42;
		this.gcF100Name.Caption = "English Hunderdth Name";
		this.gcF100Name.CurrencyDateField = null;
		this.gcF100Name.CurrencyField = null;
		this.gcF100Name.CurrencyFixed = null;
		this.gcF100Name.CurrencyFixedRate = 0.0;
		this.gcF100Name.CurrencyRateField = null;
		this.gcF100Name.FieldName = "F100Name";
		this.gcF100Name.Name = "gcF100Name";
		this.gcF100Name.PrimaryKey = false;
		this.gcF100Name.UnitField = null;
		this.gcF100Name.Visible = true;
		this.gcF100Name.VisibleIndex = 6;
		this.gcF100Name.Width = 125;
		this.gcISOCurrCod.Caption = "ISO Currency Code";
		this.gcISOCurrCod.ColumnEdit = this.rILUE_ISOCurrCod;
		this.gcISOCurrCod.CurrencyDateField = null;
		this.gcISOCurrCod.CurrencyField = null;
		this.gcISOCurrCod.CurrencyFixed = null;
		this.gcISOCurrCod.CurrencyFixedRate = 0.0;
		this.gcISOCurrCod.CurrencyRateField = null;
		this.gcISOCurrCod.FieldName = "ISOCurrCod";
		this.gcISOCurrCod.Name = "gcISOCurrCod";
		this.gcISOCurrCod.PrimaryKey = false;
		this.gcISOCurrCod.UnitField = null;
		this.gcISOCurrCod.Visible = true;
		this.gcISOCurrCod.VisibleIndex = 7;
		this.gcISOCurrCod.Width = 104;
		this.rILUE_ISOCurrCod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_ISOCurrCod.DisplayMember = "Code";
		this.rILUE_ISOCurrCod.Name = "rILUE_ISOCurrCod";
		this.rILUE_ISOCurrCod.NullText = "";
		this.rILUE_ISOCurrCod.ShowFooter = false;
		this.rILUE_ISOCurrCod.ShowHeader = false;
		this.rILUE_ISOCurrCod.ValueMember = "Code";
		this.gcMaxInDiff.Caption = "Incoming Amt Diff. Allowed";
		this.gcMaxInDiff.ColumnEdit = this.rep_MaxInDiff;
		this.gcMaxInDiff.CurrencyDateField = null;
		this.gcMaxInDiff.CurrencyField = null;
		this.gcMaxInDiff.CurrencyFixed = null;
		this.gcMaxInDiff.CurrencyFixedRate = 0.0;
		this.gcMaxInDiff.CurrencyRateField = null;
		this.gcMaxInDiff.FieldName = "MaxInDiff";
		this.gcMaxInDiff.Name = "gcMaxInDiff";
		this.gcMaxInDiff.PrimaryKey = false;
		this.gcMaxInDiff.UnitField = null;
		this.gcMaxInDiff.Visible = true;
		this.gcMaxInDiff.VisibleIndex = 8;
		this.gcMaxInDiff.Width = 140;
		this.rep_MaxInDiff.AutoHeight = false;
		this.rep_MaxInDiff.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxInDiff.Name = "rep_MaxInDiff";
		this.gcMaxOutDiff.Caption = "Outgoing Amt Diff. Allowed";
		this.gcMaxOutDiff.ColumnEdit = this.rep_MaxOutDiff;
		this.gcMaxOutDiff.CurrencyDateField = null;
		this.gcMaxOutDiff.CurrencyField = null;
		this.gcMaxOutDiff.CurrencyFixed = null;
		this.gcMaxOutDiff.CurrencyFixedRate = 0.0;
		this.gcMaxOutDiff.CurrencyRateField = null;
		this.gcMaxOutDiff.FieldName = "MaxOutDiff";
		this.gcMaxOutDiff.Name = "gcMaxOutDiff";
		this.gcMaxOutDiff.PrimaryKey = false;
		this.gcMaxOutDiff.UnitField = null;
		this.gcMaxOutDiff.Visible = true;
		this.gcMaxOutDiff.VisibleIndex = 9;
		this.gcMaxOutDiff.Width = 154;
		this.rep_MaxOutDiff.AutoHeight = false;
		this.rep_MaxOutDiff.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxOutDiff.Name = "rep_MaxOutDiff";
		this.gcMaxInPcnt.Caption = "Incoming % Diff. Allowed";
		this.gcMaxInPcnt.ColumnEdit = this.rep_MaxInPcnt;
		this.gcMaxInPcnt.CurrencyDateField = null;
		this.gcMaxInPcnt.CurrencyField = null;
		this.gcMaxInPcnt.CurrencyFixed = null;
		this.gcMaxInPcnt.CurrencyFixedRate = 0.0;
		this.gcMaxInPcnt.CurrencyRateField = null;
		this.gcMaxInPcnt.FieldName = "MaxInPcnt";
		this.gcMaxInPcnt.Name = "gcMaxInPcnt";
		this.gcMaxInPcnt.PrimaryKey = false;
		this.gcMaxInPcnt.UnitField = null;
		this.gcMaxInPcnt.Visible = true;
		this.gcMaxInPcnt.VisibleIndex = 10;
		this.gcMaxInPcnt.Width = 135;
		this.rep_MaxInPcnt.AutoHeight = false;
		this.rep_MaxInPcnt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxInPcnt.Name = "rep_MaxInPcnt";
		this.gcMaxOutPcnt.Caption = "Outgoing % Diff. Allowed";
		this.gcMaxOutPcnt.ColumnEdit = this.rep_MaxOutPcnt;
		this.gcMaxOutPcnt.CurrencyDateField = null;
		this.gcMaxOutPcnt.CurrencyField = null;
		this.gcMaxOutPcnt.CurrencyFixed = null;
		this.gcMaxOutPcnt.CurrencyFixedRate = 0.0;
		this.gcMaxOutPcnt.CurrencyRateField = null;
		this.gcMaxOutPcnt.FieldName = "MaxOutPcnt";
		this.gcMaxOutPcnt.Name = "gcMaxOutPcnt";
		this.gcMaxOutPcnt.PrimaryKey = false;
		this.gcMaxOutPcnt.UnitField = null;
		this.gcMaxOutPcnt.Visible = true;
		this.gcMaxOutPcnt.VisibleIndex = 11;
		this.gcMaxOutPcnt.Width = 138;
		this.rep_MaxOutPcnt.AutoHeight = false;
		this.rep_MaxOutPcnt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxOutPcnt.Name = "rep_MaxOutPcnt";
		this.gcRoundSys.Caption = "Rounding";
		this.gcRoundSys.ColumnEdit = this.rILUE_Rounding;
		this.gcRoundSys.CurrencyDateField = null;
		this.gcRoundSys.CurrencyField = null;
		this.gcRoundSys.CurrencyFixed = null;
		this.gcRoundSys.CurrencyFixedRate = 0.0;
		this.gcRoundSys.CurrencyRateField = null;
		this.gcRoundSys.FieldName = "RoundSys";
		this.gcRoundSys.Name = "gcRoundSys";
		this.gcRoundSys.PrimaryKey = false;
		this.gcRoundSys.UnitField = null;
		this.gcRoundSys.Visible = true;
		this.gcRoundSys.VisibleIndex = 12;
		this.gcRoundSys.Width = 83;
		this.rILUE_Rounding.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_Rounding.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rILUE_Rounding.DisplayMember = "Name";
		this.rILUE_Rounding.Name = "rILUE_Rounding";
		this.rILUE_Rounding.NullText = "";
		this.rILUE_Rounding.ShowFooter = false;
		this.rILUE_Rounding.ValueMember = "ID";
		this.gcDecimals.Caption = "Decimals";
		this.gcDecimals.ColumnEdit = this.rILUE_Decimal;
		this.gcDecimals.CurrencyDateField = null;
		this.gcDecimals.CurrencyField = null;
		this.gcDecimals.CurrencyFixed = null;
		this.gcDecimals.CurrencyFixedRate = 0.0;
		this.gcDecimals.CurrencyRateField = null;
		this.gcDecimals.FieldName = "Decimals";
		this.gcDecimals.Name = "gcDecimals";
		this.gcDecimals.PrimaryKey = false;
		this.gcDecimals.UnitField = null;
		this.gcDecimals.Visible = true;
		this.gcDecimals.VisibleIndex = 13;
		this.gcDecimals.Width = 59;
		this.rILUE_Decimal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_Decimal.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rILUE_Decimal.DisplayMember = "Name";
		this.rILUE_Decimal.Name = "rILUE_Decimal";
		this.rILUE_Decimal.NullText = "";
		this.rILUE_Decimal.ShowFooter = false;
		this.rILUE_Decimal.ValueMember = "ID";
		this.gcRoundPym.Caption = "Rounding in Pmnt";
		this.gcRoundPym.ColumnEdit = this.rIChk_RoundPym;
		this.gcRoundPym.CurrencyDateField = null;
		this.gcRoundPym.CurrencyField = null;
		this.gcRoundPym.CurrencyFixed = null;
		this.gcRoundPym.CurrencyFixedRate = 0.0;
		this.gcRoundPym.CurrencyRateField = null;
		this.gcRoundPym.FieldName = "RoundPym";
		this.gcRoundPym.Name = "gcRoundPym";
		this.gcRoundPym.PrimaryKey = false;
		this.gcRoundPym.UnitField = null;
		this.gcRoundPym.Visible = true;
		this.gcRoundPym.VisibleIndex = 14;
		this.gcRoundPym.Width = 95;
		this.rIChk_RoundPym.AutoHeight = false;
		this.rIChk_RoundPym.Name = "rIChk_RoundPym";
		this.rIChk_RoundPym.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rIChk_RoundPym.ValueChecked = "Y";
		this.rIChk_RoundPym.ValueUnchecked = "N";
		this.gcLocked.Caption = "Locked";
		this.gcLocked.ColumnEdit = this.rIChk_RoundPym;
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.ColumnEdit = this.rIChk_RoundPym;
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
		this.gcDataSource.PrimaryKey = false;
		this.gcDataSource.UnitField = null;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gcUserSign2.Caption = "UserSign2";
		this.gcUserSign2.CurrencyDateField = null;
		this.gcUserSign2.CurrencyField = null;
		this.gcUserSign2.CurrencyFixed = null;
		this.gcUserSign2.CurrencyFixedRate = 0.0;
		this.gcUserSign2.CurrencyRateField = null;
		this.gcUserSign2.FieldName = "UserSign2";
		this.gcUserSign2.Name = "gcUserSign2";
		this.gcUserSign2.PrimaryKey = false;
		this.gcUserSign2.UnitField = null;
		this.gcISRCalc.Caption = "ISRCalc";
		this.gcISRCalc.ColumnEdit = this.rIChk_RoundPym;
		this.gcISRCalc.CurrencyDateField = null;
		this.gcISRCalc.CurrencyField = null;
		this.gcISRCalc.CurrencyFixed = null;
		this.gcISRCalc.CurrencyFixedRate = 0.0;
		this.gcISRCalc.CurrencyRateField = null;
		this.gcISRCalc.FieldName = "ISRCalc";
		this.gcISRCalc.Name = "gcISRCalc";
		this.gcISRCalc.PrimaryKey = false;
		this.gcISRCalc.UnitField = null;
		this.gcConvUnit.Caption = "ConvUnit";
		this.gcConvUnit.ColumnEdit = this.rIChk_RoundPym;
		this.gcConvUnit.CurrencyDateField = null;
		this.gcConvUnit.CurrencyField = null;
		this.gcConvUnit.CurrencyFixed = null;
		this.gcConvUnit.CurrencyFixedRate = 0.0;
		this.gcConvUnit.CurrencyRateField = null;
		this.gcConvUnit.FieldName = "ConvUnit";
		this.gcConvUnit.Name = "gcConvUnit";
		this.gcConvUnit.PrimaryKey = false;
		this.gcConvUnit.UnitField = null;
		this.gcBaseCurr.Caption = "BaseCurr";
		this.gcBaseCurr.CurrencyDateField = null;
		this.gcBaseCurr.CurrencyField = null;
		this.gcBaseCurr.CurrencyFixed = null;
		this.gcBaseCurr.CurrencyFixedRate = 0.0;
		this.gcBaseCurr.CurrencyRateField = null;
		this.gcBaseCurr.FieldName = "BaseCurr";
		this.gcBaseCurr.Name = "gcBaseCurr";
		this.gcBaseCurr.PrimaryKey = false;
		this.gcBaseCurr.UnitField = null;
		this.gcFactor.Caption = "Factor";
		this.gcFactor.CurrencyDateField = null;
		this.gcFactor.CurrencyField = null;
		this.gcFactor.CurrencyFixed = null;
		this.gcFactor.CurrencyFixedRate = 0.0;
		this.gcFactor.CurrencyRateField = null;
		this.gcFactor.FieldName = "Factor";
		this.gcFactor.Name = "gcFactor";
		this.gcFactor.PrimaryKey = false;
		this.gcFactor.UnitField = null;
		this.gcChkNamePI.Caption = "ChkNamePI";
		this.gcChkNamePI.CurrencyDateField = null;
		this.gcChkNamePI.CurrencyField = null;
		this.gcChkNamePI.CurrencyFixed = null;
		this.gcChkNamePI.CurrencyFixedRate = 0.0;
		this.gcChkNamePI.CurrencyRateField = null;
		this.gcChkNamePI.FieldName = "ChkNamePI";
		this.gcChkNamePI.Name = "gcChkNamePI";
		this.gcChkNamePI.PrimaryKey = false;
		this.gcChkNamePI.UnitField = null;
		this.gcChk100NPI.Caption = "Chk100NPI";
		this.gcChk100NPI.CurrencyDateField = null;
		this.gcChk100NPI.CurrencyField = null;
		this.gcChk100NPI.CurrencyFixed = null;
		this.gcChk100NPI.CurrencyFixedRate = 0.0;
		this.gcChk100NPI.CurrencyRateField = null;
		this.gcChk100NPI.FieldName = "Chk100NPI";
		this.gcChk100NPI.Name = "gcChk100NPI";
		this.gcChk100NPI.PrimaryKey = false;
		this.gcChk100NPI.UnitField = null;
		this.gcFrgnNamePI.Caption = "FrgnNamePI";
		this.gcFrgnNamePI.CurrencyDateField = null;
		this.gcFrgnNamePI.CurrencyField = null;
		this.gcFrgnNamePI.CurrencyFixed = null;
		this.gcFrgnNamePI.CurrencyFixedRate = 0.0;
		this.gcFrgnNamePI.CurrencyRateField = null;
		this.gcFrgnNamePI.FieldName = "FrgnNamePI";
		this.gcFrgnNamePI.Name = "gcFrgnNamePI";
		this.gcFrgnNamePI.PrimaryKey = false;
		this.gcFrgnNamePI.UnitField = null;
		this.gcF100NamePI.Caption = "F100NamePI";
		this.gcF100NamePI.CurrencyDateField = null;
		this.gcF100NamePI.CurrencyField = null;
		this.gcF100NamePI.CurrencyFixed = null;
		this.gcF100NamePI.CurrencyFixedRate = 0.0;
		this.gcF100NamePI.CurrencyRateField = null;
		this.gcF100NamePI.FieldName = "F100NamePI";
		this.gcF100NamePI.Name = "gcF100NamePI";
		this.gcF100NamePI.PrimaryKey = false;
		this.gcF100NamePI.UnitField = null;
		this.gcISOCurrNum.Caption = "ISOCurrNum";
		this.gcISOCurrNum.CurrencyDateField = null;
		this.gcISOCurrNum.CurrencyField = null;
		this.gcISOCurrNum.CurrencyFixed = null;
		this.gcISOCurrNum.CurrencyFixedRate = 0.0;
		this.gcISOCurrNum.CurrencyRateField = null;
		this.gcISOCurrNum.FieldName = "ISOCurrNum";
		this.gcISOCurrNum.Name = "gcISOCurrNum";
		this.gcISOCurrNum.PrimaryKey = false;
		this.gcISOCurrNum.UnitField = null;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(3, 302);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(93, 23);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(99, 302);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(99, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(685, 327);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_Currencies);
		base.FormId = "oCurrencies";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Currencies";
		this.Text = "Currencies - Setup";
		base.Load += new System.EventHandler(this.frm_Currencies_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Currencies).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Currencies).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_ISOCurrCod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxInDiff).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxOutDiff).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxInPcnt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxOutPcnt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Rounding).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Decimal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIChk_RoundPym).EndInit();
		base.ResumeLayout(false);
	}
}
