using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.Business_Partners;
using Apzon.Forms.Administrator.SetUp;
using Apzon.Forms.Administrator.SetUp.Service;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.FormTemplate;
using Apzon.Forms.HumanResources;
using Apzon.Forms.HumanResources.Biometric;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Inventory.Item_Management.Batches;
using Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers;
using Apzon.Forms.Inventory.Pick_and_Pack;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.KPIs;
using Apzon.Forms.MTS_Manufacturing;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Production;
using Apzon.Forms.Promotions;
using Apzon.Forms.Properties;
using Apzon.Forms.Purchase;
using Apzon.Forms.RoutingAndResource;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.SalesOppotunity.SetUp.Opportunity;
using Apzon.Forms.Service;
using Apzon.Forms.Service.MarketingCampaign;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using SAPbobsCOM;

namespace Apzon.Forms.Helper;

public static class ExtenseionMethods
{
	private static HttpClientHelper<ExchangeRateAndIndexImpl> _httpHelperCurrRate = new HttpClientHelper<ExchangeRateAndIndexImpl>(Configuration.ApiServer, "ExRateAndIndex");

	private static HttpClientHelper<BusinessPartnersImpl> _bpClientHelper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "Commons");

	private static HttpClientHelper<BillToImpl> _billClientHelper = new HttpClientHelper<BillToImpl>(Configuration.ApiServer, "Commons");

	private static HttpClientHelper<ShipToImpl> _shipClientHelper = new HttpClientHelper<ShipToImpl>(Configuration.ApiServer, "Commons");

	private static HttpClientHelper<DocumentsImpl> _docClientHelper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "Commons");

	public static bool isCopyingFromExcel = false;

	public static bool isBindingData = false;

	public static bool isSetItemDocline = false;

	private static IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private static readonly List<ApzObjectTypes> _lstSalesObject = new List<ApzObjectTypes>
	{
		ApzObjectTypes.oQuotations,
		ApzObjectTypes.oOrders,
		ApzObjectTypes.oDeliveryNotes,
		ApzObjectTypes.oInvoices,
		ApzObjectTypes.oReturns,
		ApzObjectTypes.oReturnRequest,
		ApzObjectTypes.oCreditNotes,
		ApzObjectTypes.oReserveInvoices,
		ApzObjectTypes.oDownPayments
	};

	public static XtraFormEx SetDataSource(this XtraFormEx frm, DataRow row)
	{
		BindingDataForm(frm, row);
		return frm;
	}

	public static void BindingDataForm(Control ctr, DataRow row)
	{
		foreach (Control control in ctr.Controls)
		{
			if (row.IsExistsColumn(control.Name) && !string.IsNullOrEmpty(control.Name))
			{
				((BaseEdit)control).EditValue = row[control.Name];
			}
			else
			{
				BindingDataForm(control, row);
			}
		}
	}

	public static void GetDataSource(this XtraFormEx frm, DataRow row)
	{
		GetDataControl(frm, row);
	}

	public static void GetDataControl(Control ctr, DataRow row)
	{
		foreach (Control control in ctr.Controls)
		{
			if (row.IsExistsColumn(control.Name))
			{
				Type dataType = row.Table.Columns[control.Name].DataType;
				if (dataType == typeof(DateTime))
				{
					DateTime? dateTime = Function.ParseDateTime(((BaseEdit)control).EditValue);
					row[control.Name] = dateTime ?? new DateTime(1900, 1, 1);
				}
				else if (dataType == typeof(bool))
				{
					row[control.Name] = Function.ParseBool(((BaseEdit)control).EditValue);
				}
				else if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
				{
					row[control.Name] = Function.ParseDouble(((BaseEdit)control).EditValue);
				}
				else if (dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(long) || dataType == typeof(short))
				{
					row[control.Name] = Function.ParseInt(((BaseEdit)control).EditValue);
				}
				else if (control is CheckEdit)
				{
					row[control.Name] = (Function.ParseBool(((CheckEdit)control).EditValue) ? "Y" : "");
				}
				else
				{
					row[control.Name] = Function.ToString(((BaseEdit)control).EditValue);
				}
			}
			else
			{
				GetDataControl(control, row);
			}
		}
	}

	public static XtraFormEx ResetData(this XtraFormEx frm, bool resetAppearance = true)
	{
		ResetDataControl(frm, resetAppearance);
		return frm;
	}

	public static void ResetDataControl(Control ctr, bool resetAppearance)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit)
			{
				((CheckEdit)control).EditValue = "N";
				if (resetAppearance)
				{
					((CheckEdit)control).BackColor = Color.Transparent;
					((CheckEdit)control).Properties.AppearanceFocused.BackColor = Color.FromArgb(250, 249, 215);
				}
			}
			else if (control is LookUpEdit)
			{
				((LookUpEdit)control).EditValue = string.Empty;
				if (resetAppearance)
				{
					((LookUpEdit)control).BackColor = Color.White;
					((LookUpEdit)control).Properties.AppearanceFocused.BackColor = Color.FromArgb(250, 249, 215);
				}
			}
			else if (control is TextEdit)
			{
				((TextEdit)control).EditValue = string.Empty;
				if (resetAppearance)
				{
					((TextEdit)control).BackColor = Color.White;
					((TextEdit)control).Properties.AppearanceFocused.BackColor = Color.FromArgb(250, 249, 215);
				}
			}
			else if (control is RadioGroup)
			{
				((RadioGroup)control).EditValue = false;
				if (resetAppearance)
				{
					((RadioGroup)control).BackColor = Color.Transparent;
					((RadioGroup)control).Properties.AppearanceFocused.BackColor = Color.FromArgb(250, 249, 215);
				}
			}
			else
			{
				ResetDataControl(control, resetAppearance);
			}
		}
	}

	public static XtraFormEx SetFindMode(this XtraFormEx frm)
	{
		SetFindModeControl(frm);
		return frm;
	}

	public static void SetFindModeControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit)
			{
				((CheckEdit)control).EditValue = false;
				((CheckEdit)control).BackColor = Color.Transparent;
			}
			else if (control is LookUpEdit)
			{
				((LookUpEdit)control).EditValue = null;
				((LookUpEdit)control).BackColor = Color.FromArgb(250, 249, 215);
			}
			else if (control is TextEdit)
			{
				((TextEdit)control).EditValue = null;
				((TextEdit)control).BackColor = Color.FromArgb(250, 249, 215);
			}
			else
			{
				SetFindModeControl(control);
			}
		}
	}

	public static GridViewEx InitDrawIndicator(this GridViewEx gv)
	{
		gv.CustomDrawRowIndicator += (object sender, RowIndicatorCustomDrawEventArgs e) =>
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.Appearance.BackColor = Color.Orange;
			}
		};
		return gv;
	}

	public static GridViewEx InitShownEditor(this GridViewEx gv)
	{
		gv.ShownEditor += (object sender, EventArgs e) =>
		{
			try
			{
				if (gv.ActiveEditor.EditorTypeName.Equals("CalcEdit"))
				{
					((CalcEdit)gv.ActiveEditor).MouseWheel += (object s, MouseEventArgs events) =>
					{
						try
						{
							if (events.Delta != 0)
							{
								DXMouseEventArgs.GetMouseArgs(events).Handled = true;
							}
						}
						catch (Exception)
						{
						}
					};
				}
			}
			catch (Exception)
			{
			}
		};
		return gv;
	}

	public static GridViewEx InitFormular(this GridViewEx gv, List<Formular> formulars, Dictionary<string, GridViewEx> tblFormular)
	{
		gv.CellValueChanged += async (object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) =>
		{
			try
			{
				if (formulars != null && formulars.Count > 0 && tblFormular != null && tblFormular.Count > 0)
				{
					string dependTable = tblFormular.FirstOrDefault((KeyValuePair<string, GridViewEx> t) => t.Value == gv).Key;
					if (!string.IsNullOrEmpty(dependTable))
					{
						string colName = e.Column.FieldName;
						List<Formular> lst = formulars.Where((Formular t) => !t.DependField.Equals(t.FieldName) && (from x in t.DependField.Split(',')
							select x.ToUpper().Trim()).Contains(colName.ToUpper()) && t.TableName.Equals(dependTable)).ToList();
						if (lst.Count > 0)
						{
							int i;
							for (i = 0; i < lst.Count; i++)
							{
								string text = Function.ToString(lst[i].FormularString);
								if (lst[i].FormularType.ToUpper().Equals("DB"))
								{
									List<string> paramFromSqlString = Function.GetParamFromSqlString(text);
									if (paramFromSqlString != null && paramFromSqlString.Any())
									{
										foreach (string item in paramFromSqlString)
										{
											if (!string.IsNullOrEmpty(item))
											{
												string[] array = item.Replace("@", "").Replace(" ", "").Split('.');
												string key = lst[i].DependTable;
												string fieldName = "";
												if (array.Length > 1)
												{
													key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : lst[i].DependTable);
													fieldName = array[1];
												}
												else if (array.Length != 0)
												{
													fieldName = array[0];
												}
												GridViewEx gridViewEx = tblFormular[key];
												int rowHandle = ((!(gridViewEx.Name == gv.Name)) ? gridViewEx.FocusedRowHandle : e.RowHandle);
												string arg = Function.ToString(gridViewEx.GetRowCellValue(rowHandle, fieldName));
												text = text.Replace(item, $"'{arg}'");
											}
										}
									}
									new DataTable();
									DataTable data = (await _httpClient.Commons.ExecuteQuery(text)).Data;
									if (data != null && data.Rows.Count > 0)
									{
										dynamic val = lst[i].FieldType switch
										{
											"STR" => (dynamic)Function.ToString(data.Rows[0][0]), 
											"DEC" => Function.ParseDouble(data.Rows[0][0]), 
											"DOU" => Function.ParseDouble(data.Rows[0][0]), 
											"INT" => Function.ParseInt(data.Rows[0][0]), 
											"DAT" => Function.ParseDateTime(data.Rows[0][0]), 
											"TIM" => Function.ParseDateTime(data.Rows[0][0]), 
											"DTM" => Function.ParseDateTime(data.Rows[0][0]), 
											_ => Function.ToString(data.Rows[0][0]), 
										};
										GridViewEx gridViewEx2 = tblFormular[lst[i].TableName];
										int num = ((!(gridViewEx2.Name == gv.Name)) ? gridViewEx2.FocusedRowHandle : e.RowHandle);
										if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(lst[i].TableName) && (from x in t.DependField.Split(',')
											select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
												.Length - 1]).Contains(lst[i].FieldName.ToUpper())).ToList().Count > 0)
										{
											DataTable dataTable = (DataTable)gridViewEx2.GridControl.DataSource;
											if (dataTable != null && dataTable.Rows.Count > 0)
											{
												dataTable.Rows[num][lst[i].FieldName] = (object)val;
											}
										}
										else
										{
											gridViewEx2.SetRowCellValue(num, lst[i].FieldName, val);
										}
									}
								}
								else
								{
									DataTable dataTable2 = (DataTable)gv.GridControl.DataSource;
									if (dataTable2.Columns.Contains("temp"))
									{
										dataTable2.Columns.Remove("temp");
									}
									if (dataTable2.Rows.Count > 0)
									{
										switch (lst[i].FieldType)
										{
										case "STR":
											dataTable2.Columns.Add("temp", typeof(string));
											break;
										case "DEC":
											dataTable2.Columns.Add("temp", typeof(decimal));
											break;
										case "DOU":
											dataTable2.Columns.Add("temp", typeof(double));
											break;
										case "INT":
											dataTable2.Columns.Add("temp", typeof(int));
											break;
										case "DAT":
											dataTable2.Columns.Add("temp", typeof(DateTime));
											break;
										case "TIM":
											dataTable2.Columns.Add("temp", typeof(DateTime));
											break;
										case "DTM":
											dataTable2.Columns.Add("temp", typeof(DateTime));
											break;
										default:
											dataTable2.Columns.Add("temp", typeof(string));
											break;
										}
										try
										{
											dataTable2.Columns["temp"].Expression = text;
										}
										catch (Exception ex)
										{
											Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
											throw new Exception("Set formula failed:" + text);
										}
										GridViewEx gridViewEx3 = tblFormular[lst[i].TableName];
										if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(lst[i].TableName) && (from x in t.DependField.Split(',')
											select x.ToUpper().Trim()).Contains(lst[i].FieldName.ToUpper())).ToList().Count > 0)
										{
											DataTable dataTable3 = (DataTable)gridViewEx3.GridControl.DataSource;
											if (dataTable3 != null && dataTable3.Rows.Count > 0)
											{
												dataTable3.Rows[gridViewEx3.FocusedRowHandle][lst[i].FieldName] = dataTable2.Rows[e.RowHandle]["temp"];
											}
										}
										else
										{
											gridViewEx3.SetFocusedRowCellValue(lst[i].FieldName, dataTable2.Rows[e.RowHandle]["temp"]);
										}
										if (dataTable2.Columns.Contains("temp"))
										{
											dataTable2.Columns.Remove("temp");
										}
									}
									dataTable2.AcceptChanges();
								}
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				WinCommons.ErrorLineNumber(ex2);
			}
		};
		return gv;
	}

	public static async void DrillDown(this XtraFormEx baseForm, ApzObjectTypes fromType, ApzObjectTypes toType, string id, bool reserve = false, bool baseProduction = false, string udoCode = "")
	{
		_ = 10;
		try
		{
			switch (toType)
			{
			case ApzObjectTypes.oBusinessPartners:
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2.MdiParent = baseForm.MdiParent;
				frm_BusinessMasterData2._copyF_CodeCustomer = id;
				frm_BusinessMasterData2.Show();
				break;
			}
			case ApzObjectTypes.oItems:
			{
				frm_ItemMasterData frm_ItemMasterData3 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, id);
				frm_ItemMasterData3.MdiParent = baseForm.MdiParent;
				frm_ItemMasterData3.Show();
				break;
			}
			case ApzObjectTypes.oUsers:
			{
				frm_Users frm_Users2 = new frm_Users(Function.ParseInt(id));
				frm_Users2.MdiParent = baseForm.MdiParent;
				frm_Users2.Show();
				break;
			}
			case ApzObjectTypes.oInvoices:
				if (fromType != toType)
				{
					frm_ARInvoices frm_ARInvoices2 = new frm_ARInvoices(fromType, Function.ParseInt(id));
					frm_ARInvoices2.MdiParent = baseForm.MdiParent;
					frm_ARInvoices2.Show();
				}
				else if (await _httpClient.Commons.CheckIsIns(ApzObjectTypes.oInvoices, Function.ParseInt(id)) == "Y")
				{
					frm_AR_ReserveInvoices frm_AR_ReserveInvoices2 = new frm_AR_ReserveInvoices(Function.ParseInt(id));
					frm_AR_ReserveInvoices2.MdiParent = baseForm.MdiParent;
					frm_AR_ReserveInvoices2.Show();
				}
				else
				{
					frm_ARInvoices frm_ARInvoices3 = new frm_ARInvoices(Function.ParseInt(id));
					frm_ARInvoices3.MdiParent = baseForm.MdiParent;
					frm_ARInvoices3.Show();
				}
				break;
			case ApzObjectTypes.oReserveInvoices:
				if (fromType != toType)
				{
					frm_AR_ReserveInvoices frm_AR_ReserveInvoices3 = new frm_AR_ReserveInvoices(fromType, Function.ParseInt(id));
					frm_AR_ReserveInvoices3.MdiParent = baseForm.MdiParent;
					frm_AR_ReserveInvoices3.Show();
				}
				else
				{
					frm_AR_ReserveInvoices frm_AR_ReserveInvoices4 = new frm_AR_ReserveInvoices(Function.ParseInt(id));
					frm_AR_ReserveInvoices4.MdiParent = baseForm.MdiParent;
					frm_AR_ReserveInvoices4.Show();
				}
				break;
			case ApzObjectTypes.oCreditNotes:
				if (fromType != toType)
				{
					frm_ARCreditMemos frm_ARCreditMemos2 = new frm_ARCreditMemos(fromType, Function.ParseInt(id));
					frm_ARCreditMemos2.MdiParent = baseForm.MdiParent;
					frm_ARCreditMemos2.Show();
				}
				else
				{
					frm_ARCreditMemos frm_ARCreditMemos3 = new frm_ARCreditMemos(Function.ParseInt(id));
					frm_ARCreditMemos3.MdiParent = baseForm.MdiParent;
					frm_ARCreditMemos3.Show();
				}
				break;
			case ApzObjectTypes.oDeliveryNotes:
				if (fromType != toType)
				{
					frm_Delivery frm_Delivery2 = new frm_Delivery(fromType, Function.ParseInt(id));
					frm_Delivery2.MdiParent = baseForm.MdiParent;
					frm_Delivery2.Show();
				}
				else
				{
					frm_Delivery frm_Delivery3 = new frm_Delivery(Function.ParseInt(id));
					frm_Delivery3.MdiParent = baseForm.MdiParent;
					frm_Delivery3.Show();
				}
				break;
			case ApzObjectTypes.oReturnRequest:
				if (fromType != toType)
				{
					frm_ReturnRequest frm_ReturnRequest2 = new frm_ReturnRequest(fromType, Function.ParseInt(id));
					frm_ReturnRequest2.MdiParent = baseForm.MdiParent;
					frm_ReturnRequest2.Show();
				}
				else
				{
					frm_ReturnRequest frm_ReturnRequest3 = new frm_ReturnRequest(Function.ParseInt(id));
					frm_ReturnRequest3.MdiParent = baseForm.MdiParent;
					frm_ReturnRequest3.Show();
				}
				break;
			case ApzObjectTypes.oReturns:
				if (fromType != toType)
				{
					frm_Return frm_Return2 = new frm_Return(fromType, Function.ParseInt(id));
					frm_Return2.MdiParent = baseForm.MdiParent;
					frm_Return2.Show();
				}
				else
				{
					frm_Return frm_Return3 = new frm_Return(Function.ParseInt(id));
					frm_Return3.MdiParent = baseForm.MdiParent;
					frm_Return3.Show();
				}
				break;
			case ApzObjectTypes.oOrders:
				if (fromType != toType)
				{
					frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder(fromType, Function.ParseInt(id));
					frm_SalesOrder2.MdiParent = baseForm.MdiParent;
					frm_SalesOrder2.Show();
				}
				else
				{
					frm_SalesOrder frm_SalesOrder3 = new frm_SalesOrder(Function.ParseInt(id));
					frm_SalesOrder3.MdiParent = baseForm.MdiParent;
					frm_SalesOrder3.Show();
				}
				break;
			case ApzObjectTypes.oQuotations:
				if (fromType != toType)
				{
					frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch(fromType, Function.ParseInt(id));
					frm_SalesQuotationForBatch2.MdiParent = baseForm.MdiParent;
					frm_SalesQuotationForBatch2.Show();
				}
				else
				{
					frm_SalesQuotationForBatch frm_SalesQuotationForBatch3 = new frm_SalesQuotationForBatch(Function.ParseInt(id));
					frm_SalesQuotationForBatch3.MdiParent = baseForm.MdiParent;
					frm_SalesQuotationForBatch3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				if (fromType != toType)
				{
					frm_APInvoices frm_APInvoices2 = new frm_APInvoices(fromType, Function.ParseInt(id));
					frm_APInvoices2.MdiParent = baseForm.MdiParent;
					frm_APInvoices2.Show();
				}
				else
				{
					frm_APInvoices frm_APInvoices3 = new frm_APInvoices(Function.ParseInt(id));
					frm_APInvoices3.MdiParent = baseForm.MdiParent;
					frm_APInvoices3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				if (fromType != toType)
				{
					frm_APCreditMemos frm_APCreditMemos2 = new frm_APCreditMemos(fromType, Function.ParseInt(id));
					frm_APCreditMemos2.MdiParent = baseForm.MdiParent;
					frm_APCreditMemos2.Show();
				}
				else
				{
					frm_APCreditMemos frm_APCreditMemos3 = new frm_APCreditMemos(Function.ParseInt(id));
					frm_APCreditMemos3.MdiParent = baseForm.MdiParent;
					frm_APCreditMemos3.Show();
				}
				break;
			case ApzObjectTypes.oAPReserveInvoice:
				if (fromType != toType)
				{
					frm_APReserveInvoices frm_APReserveInvoices2 = new frm_APReserveInvoices(fromType, Function.ParseInt(id));
					frm_APReserveInvoices2.MdiParent = baseForm.MdiParent;
					frm_APReserveInvoices2.Show();
				}
				else
				{
					frm_APReserveInvoices frm_APReserveInvoices3 = new frm_APReserveInvoices(Function.ParseInt(id));
					frm_APReserveInvoices3.MdiParent = baseForm.MdiParent;
					frm_APReserveInvoices3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				if (fromType != toType)
				{
					frm_GoodReceiptPO frm_GoodReceiptPO2 = new frm_GoodReceiptPO(fromType, Function.ParseInt(id));
					frm_GoodReceiptPO2.MdiParent = baseForm.MdiParent;
					frm_GoodReceiptPO2.Show();
				}
				else
				{
					frm_GoodReceiptPO frm_GoodReceiptPO3 = new frm_GoodReceiptPO(Function.ParseInt(id));
					frm_GoodReceiptPO3.MdiParent = baseForm.MdiParent;
					frm_GoodReceiptPO3.Show();
				}
				break;
			case ApzObjectTypes.oGoodsReturnRequest:
				if (fromType != toType)
				{
					frm_GoodsReturnRequest frm_GoodsReturnRequest2 = new frm_GoodsReturnRequest(fromType, Function.ParseInt(id));
					frm_GoodsReturnRequest2.MdiParent = baseForm.MdiParent;
					frm_GoodsReturnRequest2.Show();
				}
				else
				{
					frm_GoodsReturnRequest frm_GoodsReturnRequest3 = new frm_GoodsReturnRequest(Function.ParseInt(id));
					frm_GoodsReturnRequest3.MdiParent = baseForm.MdiParent;
					frm_GoodsReturnRequest3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseReturns:
				if (fromType != toType)
				{
					frm_GoodsReturn frm_GoodsReturn2 = new frm_GoodsReturn(fromType, Function.ParseInt(id));
					frm_GoodsReturn2.MdiParent = baseForm.MdiParent;
					frm_GoodsReturn2.Show();
				}
				else
				{
					frm_GoodsReturn frm_GoodsReturn3 = new frm_GoodsReturn(Function.ParseInt(id));
					frm_GoodsReturn3.MdiParent = baseForm.MdiParent;
					frm_GoodsReturn3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseOrders:
				if (fromType != toType)
				{
					frm_PurchaseOder frm_PurchaseOder2 = new frm_PurchaseOder(fromType, Function.ParseInt(id));
					frm_PurchaseOder2.MdiParent = baseForm.MdiParent;
					frm_PurchaseOder2.Show();
				}
				else
				{
					frm_PurchaseOder frm_PurchaseOder3 = new frm_PurchaseOder(Function.ParseInt(id));
					frm_PurchaseOder3.MdiParent = baseForm.MdiParent;
					frm_PurchaseOder3.Show();
				}
				break;
			case ApzObjectTypes.oPaymentTermsTypes:
			{
				frm_PaymentTerms frm_PaymentTerms2 = new frm_PaymentTerms();
				if (baseForm is frm_GeneSetting)
				{
					frm_PaymentTerms2.RefreshGen += ((frm_GeneSetting)baseForm).ReloadPaymentTerm;
				}
				frm_PaymentTerms2.ShowDialog(baseForm);
				break;
			}
			case ApzObjectTypes.oItemGroups:
			{
				frm_ItemGroups frm_ItemGroups2 = new frm_ItemGroups();
				frm_ItemGroups2.MdiParent = baseForm.MdiParent;
				frm_ItemGroups2.Show();
				break;
			}
			case ApzObjectTypes.oInventoryGenEntry:
				if (baseProduction)
				{
					frm_ReceiptFromProduction frm_ReceiptFromProduction2 = new frm_ReceiptFromProduction(fromType, Function.ParseInt(Function.ParseInt(id)));
					frm_ReceiptFromProduction2.MdiParent = baseForm.MdiParent;
					frm_ReceiptFromProduction2.Show();
				}
				else if (fromType == ApzObjectTypes.oDrafts)
				{
					frm_GoodsReceipt frm_GoodsReceipt2 = new frm_GoodsReceipt(ApzObjectTypes.oDrafts, Function.ParseInt(id));
					frm_GoodsReceipt2.MdiParent = baseForm.MdiParent;
					frm_GoodsReceipt2.Show();
				}
				else if (Function.ParseInt((await _httpClient.ProductionOrders.CheckBaseProduction(Function.ParseInt(id), ApzObjectTypes.oInventoryGenEntry)).Content) != 0)
				{
					frm_ReceiptFromProduction frm_ReceiptFromProduction3 = new frm_ReceiptFromProduction(fromType, Function.ParseInt(id));
					frm_ReceiptFromProduction3.MdiParent = baseForm.MdiParent;
					frm_ReceiptFromProduction3.Show();
				}
				else
				{
					frm_GoodsReceipt frm_GoodsReceipt3 = new frm_GoodsReceipt(fromType, Function.ParseInt(id));
					frm_GoodsReceipt3.MdiParent = baseForm.MdiParent;
					frm_GoodsReceipt3.Show();
				}
				break;
			case ApzObjectTypes.oInventoryGenExit:
				if (baseProduction)
				{
					frm_IssueForProduction frm_IssueForProduction2 = new frm_IssueForProduction(fromType, Function.ParseInt(id));
					frm_IssueForProduction2.MdiParent = baseForm.MdiParent;
					frm_IssueForProduction2.Show();
				}
				else if (fromType == ApzObjectTypes.oDrafts)
				{
					frm_GoodsIssue frm_GoodsIssue2 = new frm_GoodsIssue(ApzObjectTypes.oDrafts, Function.ParseInt(id));
					frm_GoodsIssue2.MdiParent = baseForm.MdiParent;
					frm_GoodsIssue2.Show();
				}
				else if (Function.ParseInt((await _httpClient.ProductionOrders.CheckBaseProduction(Function.ParseInt(id), ApzObjectTypes.oInventoryGenExit)).Content) != 0)
				{
					frm_IssueForProduction frm_IssueForProduction3 = new frm_IssueForProduction(fromType, Function.ParseInt(id));
					frm_IssueForProduction3.MdiParent = baseForm.MdiParent;
					frm_IssueForProduction3.Show();
				}
				else
				{
					frm_GoodsIssue frm_GoodsIssue3 = new frm_GoodsIssue(fromType, Function.ParseInt(id));
					frm_GoodsIssue3.MdiParent = baseForm.MdiParent;
					frm_GoodsIssue3.Show();
				}
				break;
			case ApzObjectTypes.oWarehouses:
			{
				frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
				frm_WareHouses2.MdiParent = baseForm.MdiParent;
				frm_WareHouses2._whsSearchCode = id;
				frm_WareHouses2.Show();
				break;
			}
			case ApzObjectTypes.oProductTrees:
				if (!GlobalData.GeneralSetting.DriDownBOM.Equals("Y"))
				{
					frm_BillOfMaterials frm_BillOfMaterials2 = new frm_BillOfMaterials();
					frm_BillOfMaterials2.MdiParent = baseForm.MdiParent;
					frm_BillOfMaterials2._searchCode = id;
					frm_BillOfMaterials2.Show();
				}
				else
				{
					frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, id);
					frm_ItemMasterData2.MdiParent = baseForm.MdiParent;
					frm_ItemMasterData2.Show();
				}
				break;
			case ApzObjectTypes.oStockTransfer:
			{
				frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer(id, fromType);
				frm_InventoryTranfer2.MdiParent = baseForm.MdiParent;
				frm_InventoryTranfer2.Show();
				break;
			}
			case ApzObjectTypes.oInventoryTransferRequest:
			{
				frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest(id, fromType);
				frm_InventoryTranferRequest2.MdiParent = baseForm.MdiParent;
				frm_InventoryTranferRequest2.Show();
				break;
			}
			case ApzObjectTypes.oSalesOpportunities:
			{
				frm_SalesOpportunity frm_SalesOpportunity2 = new frm_SalesOpportunity(id);
				frm_SalesOpportunity2.MdiParent = baseForm.MdiParent;
				frm_SalesOpportunity2.Show();
				break;
			}
			case ApzObjectTypes.oDrafts:
			{
				int docTypeByDraftKey = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oOrders).Documents.GetDocTypeByDraftKey(Function.ParseInt(id));
				switch (docTypeByDraftKey)
				{
				case 13:
					if (string.Equals(await _httpClient.DocumentDraftClient.CheckIsIns(Function.ParseInt(id)), "Y"))
					{
						baseForm.DrillDown(ApzObjectTypes.oDrafts, ApzObjectTypes.oReserveInvoices, id);
					}
					else
					{
						baseForm.DrillDown(ApzObjectTypes.oDrafts, ApzObjectTypes.oInvoices, id);
					}
					break;
				default:
					baseForm.DrillDown(ApzObjectTypes.oDrafts, (ApzObjectTypes)docTypeByDraftKey, id);
					break;
				case 0:
					WinCommons.Error(Enums.MessageCode.InvalidDocObjectCode, "");
					break;
				}
				break;
			}
			case ApzObjectTypes.oUserDefinedObject:
				if (!string.IsNullOrEmpty(udoCode))
				{
					string text = await _httpClient.Udo.GetTypeByCode(udoCode);
					int docEntry = -1;
					string code = "";
					if (text.Equals("3"))
					{
						docEntry = Function.ParseInt(id);
					}
					else
					{
						code = id;
					}
					frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry, code);
					frm_UDO2.MdiParent = baseForm.MdiParent;
					frm_UDO2.Show();
				}
				break;
			case ApzObjectTypes.oContractTemplates:
			{
				frm_ContractTemplate frm_ContractTemplate2 = new frm_ContractTemplate(id);
				frm_ContractTemplate2.MdiParent = baseForm.MdiParent;
				frm_ContractTemplate2.Show();
				break;
			}
			case ApzObjectTypes.oEmployeesInfo:
			{
				frm_HumanResources frm_HumanResources2 = new frm_HumanResources();
				frm_HumanResources2.isCalledFromOutside = true;
				frm_HumanResources2.MdiParent = baseForm.MdiParent;
				frm_HumanResources2.EmpId = Function.ParseInt(id);
				frm_HumanResources2.Show();
				break;
			}
			case ApzObjectTypes.oKnowledgeBaseSolutions:
			{
				frm_Solution frm_Solution2 = new frm_Solution();
				frm_Solution2.MdiParent = baseForm.MdiParent;
				frm_Solution2.sltCode = Function.ParseInt(id);
				frm_Solution2.Show();
				break;
			}
			case ApzObjectTypes.oProductionOrders:
			{
				frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder(Function.ParseInt(id));
				frm_ProductionOrder2.MdiParent = baseForm.MdiParent;
				frm_ProductionOrder2.Show();
				break;
			}
			case ApzObjectTypes.oDownPayments:
				if (fromType != toType)
				{
					frm_DownPaymentRequest frm_DownPaymentRequest2 = new frm_DownPaymentRequest(fromType, Function.ParseInt(id));
					frm_DownPaymentRequest2.MdiParent = baseForm.MdiParent;
					frm_DownPaymentRequest2.Show();
				}
				else if (await _httpClient.Commons.CheckPosted(ApzObjectTypes.oDownPayments, Function.ParseInt(id)) == "Y")
				{
					frm_DownPaymentInvoice frm_DownPaymentInvoice2 = new frm_DownPaymentInvoice(Function.ParseInt(id));
					frm_DownPaymentInvoice2.MdiParent = baseForm.MdiParent;
					frm_DownPaymentInvoice2.Show();
				}
				else
				{
					frm_DownPaymentRequest frm_DownPaymentRequest3 = new frm_DownPaymentRequest(Function.ParseInt(id));
					frm_DownPaymentRequest3.MdiParent = baseForm.MdiParent;
					frm_DownPaymentRequest3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseDownPayments:
				if (fromType != toType)
				{
					frm_PurchaseDownPaymentRequest frm_PurchaseDownPaymentRequest2 = new frm_PurchaseDownPaymentRequest(fromType, Function.ParseInt(id));
					frm_PurchaseDownPaymentRequest2.MdiParent = baseForm.MdiParent;
					frm_PurchaseDownPaymentRequest2.Show();
				}
				else if (await _httpClient.Commons.CheckPosted(ApzObjectTypes.oPurchaseDownPayments, Function.ParseInt(id)) == "Y")
				{
					frm_PurchaseDownPaymentInvoice frm_PurchaseDownPaymentInvoice2 = new frm_PurchaseDownPaymentInvoice(Function.ParseInt(id));
					frm_PurchaseDownPaymentInvoice2.MdiParent = baseForm.MdiParent;
					frm_PurchaseDownPaymentInvoice2.Show();
				}
				else
				{
					frm_PurchaseDownPaymentRequest frm_PurchaseDownPaymentRequest3 = new frm_PurchaseDownPaymentRequest(Function.ParseInt(id));
					frm_PurchaseDownPaymentRequest3.MdiParent = baseForm.MdiParent;
					frm_PurchaseDownPaymentRequest3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				if (fromType != toType)
				{
					frm_PurchaseQuotation frm_PurchaseQuotation2 = new frm_PurchaseQuotation(fromType, Function.ParseInt(id));
					frm_PurchaseQuotation2.MdiParent = baseForm.MdiParent;
					frm_PurchaseQuotation2.Show();
				}
				else
				{
					frm_PurchaseQuotation frm_PurchaseQuotation3 = new frm_PurchaseQuotation(Function.ParseInt(id));
					frm_PurchaseQuotation3.MdiParent = baseForm.MdiParent;
					frm_PurchaseQuotation3.Show();
				}
				break;
			case ApzObjectTypes.oPurchaseRequest:
				if (fromType == toType)
				{
					frm_PurchaseRequest frm_PurchaseRequest2 = new frm_PurchaseRequest();
					frm_PurchaseRequest2.DocEntrySearch = Function.ParseInt(id);
					frm_PurchaseRequest2.MdiParent = baseForm.MdiParent;
					frm_PurchaseRequest2.Show();
				}
				else
				{
					frm_PurchaseRequest frm_PurchaseRequest3 = new frm_PurchaseRequest();
					frm_PurchaseRequest3.Fromtype = fromType;
					frm_PurchaseRequest3.Fromentry = id;
					frm_PurchaseRequest3.MdiParent = baseForm.MdiParent;
					frm_PurchaseRequest3.Show();
				}
				break;
			case ApzObjectTypes.oBlanketAgreement:
			{
				DocumentSBAImpl documentSBAImpl = new DocumentSBAImpl();
				documentSBAImpl.AbsID = Function.ParseInt(id);
				DataTable dataTable4 = await _httpClient.BlanketAgreement.GetDoc(documentSBAImpl);
				if (dataTable4.IsNotNull())
				{
					if (Function.ToString(dataTable4.Rows[0]["BpType"]).Equals("C"))
					{
						frm_SalesBlanketAgreement frm_SalesBlanketAgreement2 = new frm_SalesBlanketAgreement();
						frm_SalesBlanketAgreement2.MdiParent = baseForm.MdiParent;
						frm_SalesBlanketAgreement2.txtNoOfTrans.Text = Function.ToString(dataTable4.Rows[0]["Number"]);
						frm_SalesBlanketAgreement2.Show();
					}
					else
					{
						frm_PurchaseBlanketAgreement frm_PurchaseBlanketAgreement2 = new frm_PurchaseBlanketAgreement();
						frm_PurchaseBlanketAgreement2.MdiParent = baseForm.MdiParent;
						frm_PurchaseBlanketAgreement2.txtNoOfTrans.Text = Function.ToString(dataTable4.Rows[0]["Number"]);
						frm_PurchaseBlanketAgreement2.Show();
					}
				}
				break;
			}
			case ApzObjectTypes.oSerialNumberTransactionReport:
			{
				string[] array2 = id.Split(';');
				if (array2 != null && array2.Length == 2)
				{
					frm_SerialNumberTransaction frm_SerialNumberTransaction2 = new frm_SerialNumberTransaction();
					frm_SerialNumberTransaction2._itemCode = array2[0];
					frm_SerialNumberTransaction2._sysNumber = Function.ParseInt(array2[1]);
					frm_SerialNumberTransaction2.MdiParent = baseForm.MdiParent;
					frm_SerialNumberTransaction2.Show();
				}
				break;
			}
			case ApzObjectTypes.oBatchDetailsTransactionReport:
			{
				frm_BatchTransaction frm_BatchTransaction2 = new frm_BatchTransaction(id);
				frm_BatchTransaction2.MdiParent = baseForm.MdiParent;
				frm_BatchTransaction2.Show();
				break;
			}
			case ApzObjectTypes.oSerialNumberDetails:
			{
				frm_SerialNumberDetail frm_SerialNumberDetail2 = new frm_SerialNumberDetail();
				frm_SerialNumberDetail2._drillDownAbsEntry = Function.ParseInt(id);
				frm_SerialNumberDetail2.MdiParent = baseForm.MdiParent;
				frm_SerialNumberDetail2.Show();
				break;
			}
			case ApzObjectTypes.oBatchDetails:
			{
				frm_BatchDetail frm_BatchDetail2 = new frm_BatchDetail();
				frm_BatchDetail2._drillDownAbsEntry = Function.ParseInt(id);
				frm_BatchDetail2.MdiParent = baseForm.MdiParent;
				frm_BatchDetail2.Show();
				break;
			}
			case ApzObjectTypes.oShift:
			{
				FrmDefinedShift frmDefinedShift = new FrmDefinedShift();
				frmDefinedShift.findCode = id;
				frmDefinedShift.MdiParent = baseForm.MdiParent;
				frmDefinedShift.Show();
				break;
			}
			case ApzObjectTypes.oSchedule:
			{
				frm_Schedule frm_Schedule2 = new frm_Schedule(Function.ParseInt(id));
				frm_Schedule2.MdiParent = baseForm.MdiParent;
				frm_Schedule2.Show();
				break;
			}
			case ApzObjectTypes.oApplicants:
			{
				frm_JVApplicants frm_JVApplicants2 = new frm_JVApplicants();
				frm_JVApplicants2.AppID = Function.ParseInt(id);
				frm_JVApplicants2.MdiParent = baseForm.MdiParent;
				frm_JVApplicants2.Show();
				break;
			}
			case ApzObjectTypes.oQualifications:
			{
				frm_Qualifications frm_Qualifications2 = new frm_Qualifications();
				frm_Qualifications2.MdiParent = baseForm.MdiParent;
				frm_Qualifications2.QuaId = Function.ParseInt(id);
				frm_Qualifications2.Show();
				break;
			}
			case ApzObjectTypes.oAtp:
			{
				frm_AvailableToPromise frm_AvailableToPromise2 = new frm_AvailableToPromise(Function.ParseInt(id));
				frm_AvailableToPromise2.MdiParent = baseForm.MdiParent;
				frm_AvailableToPromise2.Show();
				break;
			}
			case ApzObjectTypes.oScheduleForAtp:
			{
				frm_ScheduleForAtp frm_ScheduleForAtp2 = new frm_ScheduleForAtp(Function.ParseInt(id));
				frm_ScheduleForAtp2.MdiParent = baseForm.MdiParent;
				frm_ScheduleForAtp2.Show();
				break;
			}
			case ApzObjectTypes.oPacking:
			{
				frm_Packing frm_Packing2 = new frm_Packing();
				frm_Packing2.MdiParent = baseForm.MdiParent;
				frm_Packing2.DocEntry = Function.ParseInt(id);
				frm_Packing2.Show();
				break;
			}
			case ApzObjectTypes.oShipping:
				if (fromType != toType)
				{
					frm_ShipDoc frm_ShipDoc2 = new frm_ShipDoc(fromType, Function.ParseInt(id));
					frm_ShipDoc2.MdiParent = baseForm.MdiParent;
					frm_ShipDoc2.Show();
				}
				else
				{
					frm_ShipDoc frm_ShipDoc3 = new frm_ShipDoc(Function.ParseInt(id));
					frm_ShipDoc3.MdiParent = baseForm.MdiParent;
					frm_ShipDoc3.Show();
				}
				break;
			case ApzObjectTypes.oVehicle:
			{
				frm_Vehicle frm_Vehicle2 = new frm_Vehicle();
				frm_Vehicle2.MdiParent = baseForm.MdiParent;
				frm_Vehicle2.DocCode = Function.ToString(id);
				frm_Vehicle2.Show();
				break;
			}
			case ApzObjectTypes.oMaterialShortageReport:
			{
				frm_MaterialShortageReport frm_MaterialShortageReport2 = new frm_MaterialShortageReport(id);
				frm_MaterialShortageReport2.MdiParent = baseForm.MdiParent;
				frm_MaterialShortageReport2.Show();
				break;
			}
			case ApzObjectTypes.oInterest:
			{
				frm_Interest frm_Interest2 = new frm_Interest();
				frm_Interest2.MdiParent = baseForm.MdiParent;
				frm_Interest2.Show();
				break;
			}
			case ApzObjectTypes.oInterestLevel:
			{
				frm_InterestLevel frm_InterestLevel2 = new frm_InterestLevel();
				frm_InterestLevel2.MdiParent = baseForm.MdiParent;
				frm_InterestLevel2.Show();
				break;
			}
			case ApzObjectTypes.oTrackingIssue:
			{
				frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue();
				frm_TrackingIssue2.MdiParent = baseForm.MdiParent;
				frm_TrackingIssue2.id = Function.ParseInt(id);
				frm_TrackingIssue2.Show();
				break;
			}
			case ApzObjectTypes.oProjectManager:
			{
				frm_ProjectManager frm_ProjectManager2 = new frm_ProjectManager();
				frm_ProjectManager2.MdiParent = baseForm.MdiParent;
				frm_ProjectManager2.ProjectId = Function.ParseInt(id);
				frm_ProjectManager2.Show();
				break;
			}
			case ApzObjectTypes.oReportCompletion:
			{
				frm_ProjectReportCompletion frm_ProjectReportCompletion2 = new frm_ProjectReportCompletion();
				frm_ProjectReportCompletion2.MdiParent = baseForm.MdiParent;
				frm_ProjectReportCompletion2.PjReportId = Function.ParseInt(id);
				frm_ProjectReportCompletion2.Show();
				break;
			}
			case ApzObjectTypes.oProjectConfirm:
			{
				frm_ProjectConfirm frm_ProjectConfirm2 = new frm_ProjectConfirm();
				frm_ProjectConfirm2.MdiParent = baseForm.MdiParent;
				frm_ProjectConfirm2.PjConfirmId = Function.ParseInt(id);
				frm_ProjectConfirm2.Show();
				break;
			}
			case ApzObjectTypes.oInventoryCountingRequest:
			{
				frm_InventoryCountingRequest frm_InventoryCountingRequest2 = new frm_InventoryCountingRequest();
				frm_InventoryCountingRequest2.MdiParent = baseForm.MdiParent;
				frm_InventoryCountingRequest2.Id = Function.ParseInt(id);
				frm_InventoryCountingRequest2.Show();
				break;
			}
			case ApzObjectTypes.oTaskManagement:
				if (Function.ParseInt(id) > 0)
				{
					DataTable dataTable3 = await _httpClient.SubActivities.LoadSingleTask(Function.ParseInt(id), GlobalData.UserCodeLogin);
					if (!dataTable3.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
						break;
					}
					frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
					frm_TaskManagement2.MdiParent = baseForm.MdiParent;
					frm_TaskManagement2.ClgCode = Function.ParseInt(id);
					frm_TaskManagement2.CallFromCal = true;
					frm_TaskManagement2.DataRowFromCal = dataTable3.Rows[0];
					frm_TaskManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
				break;
			case ApzObjectTypes.oNoteManagement:
				if (Function.ParseInt(id) > 0)
				{
					DataTable dataTable2 = await _httpClient.SubActivities.LoadNoteByCode(Function.ParseInt(id));
					if (!dataTable2.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
						break;
					}
					frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement();
					frm_NoteManagement2.MdiParent = baseForm.MdiParent;
					frm_NoteManagement2.ClgCode = Function.ParseInt(id);
					frm_NoteManagement2.CallFromCal = true;
					frm_NoteManagement2.DataRowFromCal = dataTable2.Rows[0];
					frm_NoteManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
				break;
			case ApzObjectTypes.oEventManagement:
				if (Function.ParseInt(id) > 0)
				{
					DataTable dataTable = await _httpClient.SubActivities.LoadEventByCode(Function.ParseInt(id), GlobalData.UserCodeLogin);
					if (!dataTable.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
						break;
					}
					frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
					frm_EventManagement2.MdiParent = baseForm.MdiParent;
					frm_EventManagement2.ClgCode = Function.ParseInt(id);
					frm_EventManagement2.CallFromCal = true;
					frm_EventManagement2.DataRowFromCal = dataTable.Rows[0];
					frm_EventManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
				break;
			case ApzObjectTypes.oWorkSchedule:
			{
				FrmWorkScheduling frmWorkScheduling = new FrmWorkScheduling();
				frmWorkScheduling.MdiParent = baseForm.MdiParent;
				frmWorkScheduling.idDrillDown = Function.ParseInt(id);
				frmWorkScheduling.Show();
				break;
			}
			case ApzObjectTypes.WorkCenter:
			{
				frm_ResourceWorkCenter frm_ResourceWorkCenter2 = new frm_ResourceWorkCenter();
				frm_ResourceWorkCenter2.MdiParent = baseForm.MdiParent;
				frm_ResourceWorkCenter2.resCode = id;
				frm_ResourceWorkCenter2.Show();
				break;
			}
			case ApzObjectTypes.oOperation:
			{
				frm_Operations frm_Operations2 = new frm_Operations();
				frm_Operations2.MdiParent = baseForm.MdiParent;
				frm_Operations2.OperationsCode = id;
				frm_Operations2.Show();
				break;
			}
			case ApzObjectTypes.oServiceLevelAgreement:
			{
				frm_ServiceLevelAgreement frm_ServiceLevelAgreement2 = new frm_ServiceLevelAgreement();
				frm_ServiceLevelAgreement2.MdiParent = baseForm.MdiParent;
				frm_ServiceLevelAgreement2.SLANo = Function.ParseInt(id);
				frm_ServiceLevelAgreement2.Show();
				break;
			}
			case ApzObjectTypes.oServiceContractTemplate:
			{
				frm_ServiceContractTemplate frm_ServiceContractTemplate2 = new frm_ServiceContractTemplate();
				frm_ServiceContractTemplate2.MdiParent = baseForm.MdiParent;
				frm_ServiceContractTemplate2.contractNo = id;
				frm_ServiceContractTemplate2.Show();
				break;
			}
			case ApzObjectTypes.oMarketingCampaign:
			{
				frm_MarketingCampaign frm_MarketingCampaign2 = new frm_MarketingCampaign();
				frm_MarketingCampaign2.MdiParent = baseForm.MdiParent;
				frm_MarketingCampaign2.CampaignId = Function.ParseInt(id);
				frm_MarketingCampaign2.Show();
				break;
			}
			case ApzObjectTypes.oFormWizard:
			{
				string[] array = id.Split('?');
				if (array.Length <= 1)
				{
					break;
				}
				string formId = array[0];
				Dictionary<string, string> query = array[1].Split('&').ToDictionary((string c) => c.Split('=')[0], (string c) => Uri.UnescapeDataString(c.Split('=')[1]));
				DataRow dataRow = GlobalData.FormWizard.AsEnumerable().FirstOrDefault((DataRow t) => "W" + Function.ToString(t["AbsEntry"]) == formId);
				if (dataRow != null)
				{
					switch (Function.ToString(dataRow["BaseForm"]))
					{
					case "frm_TempateControlGrid":
					{
						frm_TempateControlGrid frm_TempateControlGrid2 = new frm_TempateControlGrid(Function.ParseInt(dataRow["AbsEntry"]));
						frm_TempateControlGrid2.Text = Function.ToString(dataRow["Caption"]);
						frm_TempateControlGrid2.MdiParent = baseForm.MdiParent;
						frm_TempateControlGrid2.Show();
						break;
					}
					case "frm_TemplateControlGridGrid":
					{
						frm_TemplateControlGridGrid frm_TemplateControlGridGrid3 = new frm_TemplateControlGridGrid(Function.ParseInt(dataRow["AbsEntry"]), query);
						frm_TemplateControlGridGrid3.Text = Function.ToString(dataRow["Caption"]);
						frm_TemplateControlGridGrid3.MdiParent = baseForm.MdiParent;
						frm_TemplateControlGridGrid3.Show();
						break;
					}
					case "W":
					{
						frm_TemplateControlGridGrid frm_TemplateControlGridGrid2 = new frm_TemplateControlGridGrid(Function.ParseInt(dataRow["AbsEntry"]), query);
						frm_TemplateControlGridGrid2.Text = Function.ToString(dataRow["Caption"]);
						frm_TemplateControlGridGrid2.MdiParent = baseForm.MdiParent;
						frm_TemplateControlGridGrid2.Show();
						break;
					}
					}
				}
				break;
			}
			case ApzObjectTypes.oWBSTemplate:
			{
				frm_WBSTemplate frm_WBSTemplate2 = new frm_WBSTemplate();
				frm_WBSTemplate2.MdiParent = baseForm.MdiParent;
				frm_WBSTemplate2.Code = Function.ToString(id);
				frm_WBSTemplate2.Show();
				break;
			}
			case ApzObjectTypes.oSetIndicatorKPIs:
			{
				frm_SetIndicatorKPIs frm_SetIndicatorKPIs2 = new frm_SetIndicatorKPIs();
				frm_SetIndicatorKPIs2.MdiParent = baseForm.MdiParent;
				frm_SetIndicatorKPIs2.Code = Function.ToString(id);
				frm_SetIndicatorKPIs2.Show();
				break;
			}
			case ApzObjectTypes.oKPIsMasterData:
			{
				frm_KPIsMasterData frm_KPIsMasterData2 = new frm_KPIsMasterData();
				frm_KPIsMasterData2.MdiParent = baseForm.MdiParent;
				frm_KPIsMasterData2.Code = Function.ToString(id);
				frm_KPIsMasterData2.Show();
				break;
			}
			case ApzObjectTypes.oKPIsSetting:
			{
				frm_KPIsSetting frm_KPIsSetting2 = new frm_KPIsSetting();
				frm_KPIsSetting2.MdiParent = baseForm.MdiParent;
				frm_KPIsSetting2.AbsEntry = Function.ParseInt(id);
				frm_KPIsSetting2.Show();
				break;
			}
			case ApzObjectTypes.oPromotionCondition:
			{
				frm_PromotionCondition frm_PromotionCondition2 = new frm_PromotionCondition();
				frm_PromotionCondition2._code = id;
				frm_PromotionCondition2.MdiParent = baseForm.MdiParent;
				frm_PromotionCondition2.Show();
				break;
			}
			case ApzObjectTypes.oPromotionBenefit:
			{
				frm_PromotionBenefit frm_PromotionBenefit2 = new frm_PromotionBenefit();
				frm_PromotionBenefit2._code = id;
				frm_PromotionBenefit2.MdiParent = baseForm.MdiParent;
				frm_PromotionBenefit2.Show();
				break;
			}
			case ApzObjectTypes.oOutSourcing:
			{
				frm_OutSourcing frm_OutSourcing2 = new frm_OutSourcing(Function.ParseInt(id));
				frm_OutSourcing2.MdiParent = baseForm.MdiParent;
				frm_OutSourcing2.Show();
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void DrillDown(this XtraFormEx baseForm, ApzObjectTypes type, string num, bool reserve = false, bool baseProduction = false, string id = "")
	{
		baseForm.DrillDown(type, type, num, reserve, baseProduction, id);
	}

	public static void DrillDownOrigin(this XtraFormEx form, string originType, string originCode)
	{
		try
		{
			switch (originType)
			{
			case "700003":
				form.DrillDown(ApzObjectTypes.oMrp, originCode);
				break;
			case "800004":
				form.DrillDown(ApzObjectTypes.oSchedule, originCode);
				break;
			case "700013":
				form.DrillDown(ApzObjectTypes.oAtp, originCode);
				break;
			case "700015":
				form.DrillDown(ApzObjectTypes.oScheduleForAtp, originCode);
				break;
			case "700016":
				form.DrillDown(ApzObjectTypes.oMaterialShortageReport, originCode);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static GridViewEx InitPasteFromExcel(this GridViewEx gv)
	{
		gv.GridControl.EditorKeyDown += (object sender, KeyEventArgs events) =>
		{
			PasteFromExcel(events, gv.GridControl as GridControlEx, gv);
		};
		return gv;
	}

	public static async void PasteFromExcel(KeyEventArgs e, GridControlEx grd, GridViewEx gv)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				isCopyingFromExcel = true;
				gv.BeginInsertRow();
				DataObject dataObject = (DataObject)Clipboard.GetDataObject();
				if (grd == null)
				{
					WinCommons.Warning(Enums.MessageCode.CouldNotParseDataOnThisControl, "");
					isCopyingFromExcel = false;
					return;
				}
				DataTable dt;
				if (dataObject == null)
				{
					dt = grd.ClipboardData;
				}
				else if (dataObject.GetData(DataFormats.UnicodeText) != null)
				{
					string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
					dt = (dataObject.GetDataPresent(DataFormats.Text) ? Function.PasteFromExcel(clipboardData) : grd.ClipboardData);
				}
				else
				{
					dt = grd.ClipboardData;
				}
				if (dt != null && (dt.Rows.Count > 1 || dt.Columns.Count > 1))
				{
					string errorItemCode = "";
					int rowIndex = gv.FocusedRowHandle;
					List<DataRow> lstSourceRow = new List<DataRow>();
					int num = ((gv.RowCount - rowIndex < dt.Rows.Count) ? gv.RowCount : (dt.Rows.Count + rowIndex));
					for (int i = rowIndex; i < num; i++)
					{
						lstSourceRow.Add(gv.GetDataRow(i));
					}
					GridColumn column = gv.FocusedColumn;
					if (column.FieldName.ToUpper().Equals("ITEMCODE"))
					{
						DocumentLineTable lst = gv.GridControl.DataSource as DocumentLineTable;
						if (Function.ParseInt(gv.GetRowCellValue(rowIndex, "LineNum")) < 0)
						{
							gv.SetRowCellValue(rowIndex, gv.Columns["LineNum"], lst.GetNewLineNum(-1));
						}
						int lineCount = gv.RowCount;
						int rowCount = dt.Rows.Count;
						if (lineCount - rowIndex < rowCount)
						{
							for (int j = 0; j < rowCount - lineCount + rowIndex; j++)
							{
								string tmpCode = Function.ToString(dt.Rows[j][0]);
								if (!string.IsNullOrEmpty(tmpCode))
								{
									if (!(await CheckExistsItemCode(tmpCode)))
									{
										WinCommons.Error(Enums.MessageCode.ItemCodeDoesNotExistsWithCode, "", tmpCode);
										isCopyingFromExcel = false;
										return;
									}
									DocumentLineRow newRow = lst.GetNewRow();
									newRow.ItemCode = tmpCode;
									newRow.DocEntry = ((lst.Count == 0) ? 1 : lst.GetEnumerable().Max((DocumentLineRow x) => x.DocEntry));
									newRow.LineNum = ((lst.Count == 0) ? 1 : (lst.GetEnumerable().Max((DocumentLineRow x) => x.LineNum) + 1));
									newRow.Currency = GlobalData.MainCurrency;
									newRow.RateCurrency = 1.0;
									newRow.LineStatusName = "O";
									lst.Add(newRow);
									lst.AcceptChanges();
									lstSourceRow.Add(newRow);
									continue;
								}
								rowCount = j + 1;
								errorItemCode = tmpCode;
								break;
							}
							lst.AcceptChanges();
							grd.DataSource = lst;
							grd.RefreshDataSource();
						}
						int num2 = ((gv.Columns.Count > dt.Columns.Count) ? dt.Columns.Count : gv.Columns.Count);
						for (int num3 = 0; num3 < rowCount; num3++)
						{
							int colIndex = column.VisibleIndex;
							int num4 = gv.FindRowHandleByDataRow(lstSourceRow[num3]);
							if (num4 < 0)
							{
								continue;
							}
							gv.FocusedRowHandle = num4;
							for (int num5 = 0; num5 < num2; num5++)
							{
								GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
								if (gridColumn.ColumnType == typeof(DateTime))
								{
									gv.SetRowCellValue(num4, gridColumn, Function.ParseDateTime(dt.Rows[num3][num5], GlobalData.DateTimeFormat));
								}
								else
								{
									gv.SetRowCellValue(num4, gridColumn, dt.Rows[num3][num5]);
								}
								colIndex++;
							}
							lst[rowIndex].FromExcel = true;
						}
						gv.EndInsertRow();
					}
					else
					{
						int num6 = ((gv.RowCount > dt.Rows.Count) ? dt.Rows.Count : gv.RowCount);
						int num7 = ((gv.Columns.Count > dt.Columns.Count) ? dt.Columns.Count : gv.Columns.Count);
						for (int num8 = 0; num8 < num6; num8++)
						{
							int colIndex2 = column.VisibleIndex;
							for (int num9 = 0; num9 < num7; num9++)
							{
								GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex2);
								if (gridColumn2.ColumnType == typeof(DateTime))
								{
									gv.SetRowCellValue(rowIndex, gridColumn2, Function.ParseDateTime(dt.Rows[num8][num9], GlobalData.DateTimeFormat));
								}
								else
								{
									gv.SetRowCellValue(rowIndex, gridColumn2, dt.Rows[num8][num9]);
								}
								colIndex2++;
							}
							rowIndex++;
						}
						gv.EndInsertRow();
						grd.DataSourceTable().AcceptChanges();
					}
					if (errorItemCode != "")
					{
						SetFocusCellErrorItemCode(gv, errorItemCode);
					}
					Clipboard.Clear();
					isCopyingFromExcel = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv.EndInsertRow();
	}

	public static void SetFocusCellErrorItemCode(GridViewEx gv, string errorItemCode)
	{
		try
		{
			for (int i = 0; i < gv.DataRowCount; i++)
			{
				object rowCellValue = gv.GetRowCellValue(i, "ItemCode");
				if (rowCellValue != null && !string.IsNullOrEmpty(Function.ToString(rowCellValue)) && rowCellValue.ToString().ToUpper().Equals(errorItemCode.ToUpper()))
				{
					gv.FocusedRowHandle = i;
					gv.FocusedColumn = gv.VisibleColumns[1];
					gv.ShowEditor();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task<bool> CheckExistsItemCode(string ItemCode)
	{
		try
		{
			using HttpClientHelper<ItemsImpl> _ItemCodeClientHelper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> list = await _ItemCodeClientHelper.GetListAsyncByPr("CheckExistsItem", new
			{
				itemCode = ItemCode
			});
			if (list != null && list.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public static GridViewEx InitPasteFromExcelSaleOpportunity(this GridViewEx gv)
	{
		gv.GridControl.EditorKeyDown += (object sender, KeyEventArgs events) =>
		{
			PasteFromExcelSaleOpportunity(events, gv.GridControl, gv);
		};
		return gv;
	}

	public static async void PasteFromExcelSaleOpportunity(KeyEventArgs e, GridControl grd, GridViewEx gv)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			isCopyingFromExcel = true;
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				DataTable dt = Function.PasteFromExcel(clipboardData);
				if (dt != null && dt.Rows.Count > 0)
				{
					string errorItemCode = "";
					int rowIndex = gv.FocusedRowHandle;
					_ = gv.FocusedRowHandle;
					GridColumn column = gv.FocusedColumn;
					if (column.FieldName.ToUpper().Equals("ITEMCODE"))
					{
						int lineCount = gv.RowCount;
						int rowCount = dt.Rows.Count;
						if (lineCount - rowIndex < rowCount)
						{
							DataTable lst = (DataTable)gv.GridControl.DataSource;
							for (int i = 0; i < rowCount - lineCount + rowIndex; i++)
							{
								if (dt.Rows[i][0] != null)
								{
									string tmpCode = dt.Rows[i][0].ToString();
									if (string.IsNullOrEmpty(tmpCode))
									{
										rowCount = i + 1;
										errorItemCode = tmpCode;
										break;
									}
									if (!(await CheckExistsItemCode(tmpCode)))
									{
										rowCount = i + 1;
										errorItemCode = tmpCode;
										break;
									}
								}
								DataRow row = lst.NewRow();
								lst.Rows.Add(row);
							}
							grd.DataSource = lst;
							grd.RefreshDataSource();
						}
						int num = ((gv.Columns.Count > dt.Columns.Count) ? dt.Columns.Count : gv.Columns.Count);
						for (int j = 0; j < rowCount; j++)
						{
							int colIndex = column.VisibleIndex;
							for (int k = 0; k < num; k++)
							{
								GridColumn column2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
								gv.SetRowCellValue(rowIndex, column2, dt.Rows[j][k]);
								colIndex++;
							}
							rowIndex++;
						}
					}
					else
					{
						int num2 = ((gv.RowCount > dt.Rows.Count) ? dt.Rows.Count : gv.RowCount);
						int num3 = ((gv.Columns.Count > dt.Columns.Count) ? dt.Columns.Count : gv.Columns.Count);
						for (int num4 = 0; num4 < num2; num4++)
						{
							int colIndex2 = column.VisibleIndex;
							for (int num5 = 0; num5 < num3; num5++)
							{
								GridColumn column3 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex2);
								gv.SetRowCellValue(rowIndex, column3, dt.Rows[num4][num5]);
								colIndex2++;
							}
							rowIndex++;
						}
					}
					Clipboard.Clear();
					if (errorItemCode != "")
					{
						SetFocusCellErrorItemCode(gv, errorItemCode);
					}
				}
			}
			isCopyingFromExcel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void PasteFromExcelToDataTable(GridControl grd, GridViewEx gv, List<string> key)
	{
		try
		{
			gv.ExcelToDataSource(key);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		gv.EndInsertRow();
	}

	public static RepositoryItemLookUpEdit ShowExchangeRate(this RepositoryItemLookUpEdit luk, GridViewEx gv)
	{
		luk.EditValueChanged += (object sender, EventArgs events) =>
		{
			try
			{
				string text = (sender as LookUpEdit).Text;
				DateTime date = Convert.ToDateTime(gv.GetRowCellValue(gv.FocusedRowHandle, "DocDate").ToString());
				int? docNum = Function.ParseInt(gv.GetRowCellValue(gv.FocusedRowHandle, "DocEntry"));
				DocumentRow documentRow = (gv.GridControl.DataSource as DocumentTable).GetEnumerable().FirstOrDefault((DocumentRow x) => x.DocEntry == docNum);
				if (documentRow != null)
				{
					if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
					{
						documentRow.DocRate = 1.0;
					}
					else
					{
						double BPRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(text, date);
						if (BPRate > 0.0)
						{
							gv.SetFocusedRowCellValue("DocRate", BPRate);
						}
						else if (documentRow.DocCur != "##" && documentRow.DocCur.ToUpper() != GlobalData.MainCurrency.ToUpper() && !frm_ExchangeRatesAndIndexs._isOpen)
						{
							frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
							frm.Curr = documentRow.DocCur;
							frm._date = date;
							frm.FormClosing += (object s, FormClosingEventArgs e1) =>
							{
								BPRate = frm.rate;
								gv.SetFocusedRowCellValue("DocRate", BPRate);
							};
							frm.ShowDialog();
						}
						else if (documentRow.DocCur.ToUpper() == GlobalData.MainCurrency)
						{
							documentRow.DocRate = 1.0;
						}
						else
						{
							documentRow.DocRate = 0.0;
						}
						gv.GridControl.DataSourceTable().AcceptChanges();
						gv.GridControl.RefreshDataSource();
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		};
		return luk;
	}

	public static GridViewEx DisplayCurrency(this GridViewEx gv, GridViewEx gvDoc)
	{
		gv.CustomColumnDisplayText += (object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs events) =>
		{
			try
			{
				string text = events.Column.FieldName.ToUpper();
				if (Function.ToString(gv.GetListSourceRowCellValue(events.ListSourceRowIndex, "LineType")) == "T" && text == "ITEMCODE")
				{
					string displayText = Function.ToString(gv.GetListSourceRowCellValue(events.ListSourceRowIndex, "ItemCode"));
					events.DisplayText = displayText;
				}
				else
				{
					string currency = Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"));
					int num = -1;
					if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
					{
						CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
						if (currencyCodesImpl != null)
						{
							num = currencyCodesImpl.Decimals;
						}
					}
					if ((text.Equals("TOTALFRGN") || text.Equals("VATSUMFRGN") || text.Equals("GRSFC") || text.Equals("GTOTALFC")) && Function.ParseDouble(events.Value) != 0.0)
					{
						DocumentRow selectedDoc = GetSelectedDoc(gvDoc);
						if (selectedDoc != null)
						{
							string docCurrency = selectedDoc.DocCurrency;
							events.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num) + "} {1}", events.Value, docCurrency);
							events.Column.SummaryItem.DisplayFormat = "{0:n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num) + "}";
						}
					}
					int num2 = -1;
					if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
					{
						CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency.ToUpper()));
						if (currencyCodesImpl2 != null)
						{
							num2 = currencyCodesImpl2.Decimals;
						}
					}
					if (!events.Column.FieldName.StartsWith("U_") && events.Column.FieldName.Length > 3 && events.Column.FieldName.Substring(events.Column.FieldName.Length - 3, 3).StartsWith("_F"))
					{
						int num3 = Function.ParseInt(events.Column.FieldName.Last());
						if (num3 > 0)
						{
							if (events.Column.FieldName.ToUpper().Equals("LINETOTAL_F" + num3) || (events.Column.FieldName.ToUpper().Equals("VATSUM_F" + num3) && Function.ParseDouble(events.Value) != 0.0))
							{
								events.DisplayText = string.Format("{0:n" + ((num2 == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num2) + "} {1}", events.Value, GlobalData.MainCurrency.ToUpper());
								events.Column.SummaryItem.DisplayFormat = "{0:n" + ((num2 == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num2) + "}";
							}
							if (events.Column.FieldName.ToUpper().Equals("TOTALFRGN_F" + num3) || (events.Column.FieldName.ToUpper().Equals("VATSUMFRGN_F" + num3) && Function.ParseDouble(events.Value) != 0.0))
							{
								DocumentRow selectedDoc2 = GetSelectedDoc(gvDoc);
								if (selectedDoc2 != null)
								{
									string docCurrency2 = selectedDoc2.DocCurrency;
									events.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num) + "} {1}", events.Value, docCurrency2);
									events.Column.SummaryItem.DisplayFormat = "{0:n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num) + "}";
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		};
		return gv;
	}

	private static DocumentRow GetSelectedDoc(GridViewEx gvDoc)
	{
		try
		{
			DocumentTable dt = (DocumentTable)gvDoc.GridControl.DataSource;
			int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			return dt.GetByNum(docNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	public static GridViewEx HandleInvalidValueException(this GridViewEx gv)
	{
		gv.InvalidValueException += (object sender, InvalidValueExceptionEventArgs e) =>
		{
			e.ExceptionMode = ExceptionMode.Ignore;
		};
		return gv;
	}

	public static double GetExchangeRate(string curr, DateTime date)
	{
		try
		{
			double result = 1.0;
			if (curr.ToUpper() != GlobalData.MainCurrency.ToUpper())
			{
				result = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(curr.ToUpper(), date);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1.0;
		}
	}

	public static void DisplayFormatDocument(this GridViewEx gv)
	{
		gv.CustomColumnDisplayText += (object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs events) =>
		{
			try
			{
				string text = events.Column.FieldName.ToUpper();
				int num = -1;
				string docCurrency = Function.ToString(gv.GetRowCellValue(events.ListSourceRowIndex, "DocCur"));
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(docCurrency));
					if (currencyCodesImpl != null)
					{
						num = currencyCodesImpl.Decimals;
					}
				}
				if (text.Equals("TOTALBEFOREDIS") || text.Equals("TOTAL") || text.Equals("TAXTOTAL") || text.Equals("TOTALFREIGHT"))
				{
					events.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num) + "} {1}", events.Value, docCurrency.ToUpper());
				}
				if (text.Equals("DOCRATE"))
				{
					events.DisplayText = string.Format("{0:n" + GlobalData.GeneralSetting.RateDec.GetValueOrDefault() + "}", events.Value);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		};
	}

	public static void InitCellValueChanged(this GridViewEx gvDoc, GridViewEx gvLine, GridControl grdLogistic, RepositoryItemLookUpEdit rBillTo, RepositoryItemLookUpEdit rShipTo, RepositoryItemLookUpEdit rlpShipType, RepositoryItemLookUpEdit rlupContactPersonCode, CalcEdit paymentTotal = null, ApzObjectTypes type = ApzObjectTypes.oItems, CalcEdit paymentTotalFc = null, GridViewEx freight = null, GridViewEx gv_Accounting = null)
	{
		bool changeTotal = false;
		bool changeDiscSum = false;
		bool changeDpmTotal = false;
		gvDoc.CellValueChanged += async (object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) =>
		{
			try
			{
				VirFormDocument form = gvDoc.GridControl.FindForm() as VirFormDocument;
				Enums.FormViewModeType openFormViewMode = Enums.FormViewModeType.View;
				if (form != null)
				{
					openFormViewMode = form.OpenFormViewMode;
				}
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(type);
				if (!frm_ExchangeRatesAndIndexs.isClosing && e.RowHandle >= 0)
				{
					if (e.Column.FieldName == "TotalExpns")
					{
						decimal num = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalExpns"));
						decimal num2 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TaxTotal"));
						decimal num3 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalBeforeDis"));
						decimal num4 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalDiscount"));
						decimal num5 = num + num2 + num3 - num4;
						gvDoc.SetRowCellValue(e.RowHandle, "Total", num5);
					}
					if (e.Column.FieldName == "TaxTotal")
					{
						decimal num6 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalExpns"));
						decimal num7 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TaxTotal"));
						decimal num8 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalDiscount"));
						decimal num9 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "TotalBeforeDis"));
						decimal num10 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "DPMTotal"));
						decimal num11 = num6 + num7 + num9 - num8 - num10;
						gvDoc.SetRowCellValue(e.RowHandle, "Total", num11);
					}
					double docdiscount = Math.Round(Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, "DiscPrcnt")), 6, MidpointRounding.AwayFromZero);
					if (docdiscount > 100.0 && !new ApzObjectTypes[3]
					{
						ApzObjectTypes.oDrafts,
						ApzObjectTypes.oDownPayments,
						ApzObjectTypes.oPurchaseDownPayments
					}.Contains(type))
					{
						WinCommons.Error(Enums.MessageCode.EnterDiscountPercentageLessThan100, "");
					}
					else
					{
						if (e.Column.FieldName.ToUpper().Equals("SERIES"))
						{
							int series = Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "Series"));
							int num12 = await httpClient.Documents.GetDocumentNumber(series);
							if (num12 > 0)
							{
								gvDoc.SetRowCellValue(e.RowHandle, "DocNum", num12);
							}
						}
						string cur = (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("DocCur"))) ? GlobalData.MainCurrency : gvDoc.GetFocusedRowCellValue("DocCur").ToString());
						DateTime date = ((gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocDate").ToString()));
						int docNum = ((gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocNum") != DBNull.Value) ? Convert.ToInt32(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocNum").ToString()) : 0);
						DocumentTable dt = gvDoc.GridControl.DataSource as DocumentTable;
						DocumentRow doc = dt.GetByNum(docNum);
						int decimals = GlobalData.GeneralSetting.SumDec.Value;
						if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
						{
							CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(doc.DocCur.ToUpper()));
							if (currencyCodesImpl != null)
							{
								decimals = ((currencyCodesImpl.Decimals == -1) ? decimals : currencyCodesImpl.Decimals);
							}
						}
						if (e.Column.FieldName.ToUpper().Equals("DOCTOTAL") && paymentTotal != null)
						{
							paymentTotal.EditValue = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocTotal"));
						}
						if (e.Column.FieldName.ToUpper().Equals("DOCTOTALFC") && paymentTotalFc != null)
						{
							paymentTotalFc.EditValue = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocTotalFc"));
						}
						DocumentLineTable lst = (DocumentLineTable)gvLine.GridControl.DataSource;
						if (e.Column.FieldName.ToUpper().Equals("CARDCODE"))
						{
							bool mappingCtx = gvDoc.IsMappingContextAction;
							string cardCode = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "CardCode"));
							string docCurrency = WinFuntions.GetBPCurrency(cardCode);
							if (string.IsNullOrEmpty(docCurrency))
							{
								docCurrency = GlobalData.MainCurrency;
							}
							DataTable dataTable = await httpClient.BusinessMaster.GetADoc(cardCode);
							if (dataTable.IsNotNull() && dataTable.Rows.Count == 1)
							{
								form.PaymentTermInfo = null;
								SetBusinessToGridDoc(dataTable, gvDoc, e.RowHandle, docNum, type, grdLogistic, gv_Accounting, rBillTo, rShipTo, rlpShipType, rlupContactPersonCode, gvLine, form);
							}
							if (!mappingCtx)
							{
								gvDoc.SetRowCellValue(e.RowHandle, "DocCur", (docCurrency == "##") ? GlobalData.MainCurrency : docCurrency);
							}
							doc.Currency = docCurrency;
							if (string.Compare(docCurrency, "##", ignoreCase: true) != 0 && string.Compare(docCurrency, GlobalData.MainCurrency, ignoreCase: true) != 0)
							{
								doc.DocCur = docCurrency;
							}
							else if (string.Compare(docCurrency, GlobalData.MainCurrency, ignoreCase: true) == 0)
							{
								doc.DocRate = 1.0;
							}
							if (!string.IsNullOrEmpty(cardCode))
							{
								if (doc.DocCur.ToUpper() == GlobalData.MainCurrency)
								{
									doc.DocRate = 1.0;
								}
								else
								{
									double BPRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(doc.DocCur, date);
									if (BPRate > 0.0)
									{
										gvDoc.SetRowCellValue(e.RowHandle, "DocRate", BPRate);
									}
									else if (doc.DocCur != "##" && doc.DocCur.ToUpper() != GlobalData.MainCurrency.ToUpper() && !frm_ExchangeRatesAndIndexs._isOpen)
									{
										frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
										frm.Curr = doc.DocCur;
										frm._date = date;
										frm.FormClosing += (object s, FormClosingEventArgs e2) =>
										{
											BPRate = frm.rate;
											gvDoc.SetRowCellValue(e.RowHandle, "DocRate", BPRate);
										};
										frm.ShowDialog();
									}
									else if (doc.DocCur.ToUpper() == GlobalData.MainCurrency)
									{
										doc.DocRate = 1.0;
									}
								}
							}
							DataTable ownerByBpUser = HttpClientHelperFactory.GetHttpClient().Documents.GetOwnerByBpUser(cardCode, GlobalData.UserID);
							if (ownerByBpUser != null && ownerByBpUser.Rows.Count > 0)
							{
								if (Function.ParseInt(ownerByBpUser.Rows[0]["OwnerCode"]) > 0)
								{
									doc.OwnerCode = Function.ParseInt(ownerByBpUser.Rows[0]["OwnerCode"]);
									doc.OwnerName = Function.ToString(ownerByBpUser.Rows[0]["OwnerName"]);
									if (Function.ParseInt(ownerByBpUser.Rows[0]["SlpCode"]) > 0)
									{
										gvDoc.SetRowCellValue(e.RowHandle, "SlpCode", Function.ParseInt(ownerByBpUser.Rows[0]["SlpCode"]));
									}
								}
								else
								{
									doc.OwnerCode = -1;
								}
							}
							if (!string.IsNullOrEmpty(cardCode) && lst.AsEnumerable().Any((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))) && ((openFormViewMode == Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateBpData2Line, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes) || openFormViewMode == Enums.FormViewModeType.Add))
							{
								for (int num13 = 0; num13 < lst.Rows.Count; num13++)
								{
									DocumentLineRow documentLineRow = lst.Rows[num13] as DocumentLineRow;
									if (!string.IsNullOrEmpty(documentLineRow.ItemCode))
									{
										if (openFormViewMode != Enums.FormViewModeType.Add || string.IsNullOrEmpty(documentLineRow.SubCatNum))
										{
											documentLineRow.SubCatNum = _httpClient.BusinessPartnerCatalogNumbersClient.GetSubCatNumByItemAndCardCode(documentLineRow.ItemCode, cardCode);
										}
										double num14 = 0.0;
										string text = "";
										double num15 = 0.0;
										string value = "R";
										string text2 = "N";
										DataTable dataTable2 = _httpClient.Commons.DocumentPrice(documentLineRow.ItemCode, doc.CardCode, doc.DocDate, documentLineRow.Quantity, documentLineRow.UoMCode, Function.ParseInt(doc["InfoPriceList"]));
										if (dataTable2.IsNotNull())
										{
											num14 = Function.ParseDouble(dataTable2.Rows[0]["Price"]);
											text = Function.ToString(dataTable2.Rows[0]["Currency"]);
											num15 = Function.ParseDouble(dataTable2.Rows[0]["Discount"]);
											value = Function.ToString(dataTable2.Rows[0]["SpecPrice"]);
											text2 = Function.ToString(dataTable2.Rows[0]["IsGrossPrc"]);
											if (Function.ParseInt(documentLineRow["UgpCode"]) == -1)
											{
												num14 *= documentLineRow.UnitsOfMeasurment;
											}
										}
										if (Function.ToString(documentLineRow["SpecPrice"]) != "N" && (openFormViewMode == Enums.FormViewModeType.View || (openFormViewMode == Enums.FormViewModeType.Add && documentLineRow.PriceBefDi == 0.0)))
										{
											documentLineRow["SpecPrice"] = value;
											gvLine.SetRowCellValue(num13, "DiscPrcnt", num15);
											if (text2 == "Y")
											{
												documentLineRow.PriceBefDi = num14 / (1.0 + documentLineRow.VatPrcnt / 100.0);
											}
											else
											{
												documentLineRow.PriceBefDi = num14;
											}
											documentLineRow.Price = documentLineRow.PriceBefDi - documentLineRow.PriceBefDi * documentLineRow.DiscountPercent / 100.0;
											if (!string.IsNullOrEmpty(text))
											{
												documentLineRow.Currency = text;
												documentLineRow.RateCurrency = GetExchangeRate(text, Function.ParseDateTimes(doc.DocDate));
											}
											else if (!string.IsNullOrEmpty(doc.DocCurrency))
											{
												documentLineRow.Currency = doc.DocCurrency;
												documentLineRow.RateCurrency = GetExchangeRate(doc.DocCurrency, Function.ParseDateTimes(doc.DocDate));
											}
											else
											{
												documentLineRow.Currency = GlobalData.MainCurrency;
												documentLineRow.RateCurrency = 1.0;
											}
											double num16 = ApzonCommon.LineTotal(documentLineRow.Quantity, documentLineRow.PriceBefDi, documentLineRow.DiscountPercent);
											if (documentLineRow.RateCurrency == 0.0)
											{
												documentLineRow.LineTotal = num16 * 1.0;
											}
											else
											{
												documentLineRow.LineTotal = num16 * Function.ParseDouble(documentLineRow.RateCurrency);
											}
											double num17 = documentLineRow.LineTotal * documentLineRow.Rate;
											if (documentLineRow.Rate >= 1.0)
											{
												num17 = documentLineRow.LineTotal * documentLineRow.Rate / 100.0;
											}
											documentLineRow.LineVat = Function.ParseDouble(documentLineRow.RateCurrency) * Function.ParseDouble(num17);
										}
									}
								}
							}
							else
							{
								foreach (DocumentLineRow row in lst.Rows)
								{
									if (!string.IsNullOrEmpty(row.ItemCode))
									{
										row.SubCatNum = _httpClient.BusinessPartnerCatalogNumbersClient.GetSubCatNumByItemAndCardCode(row.ItemCode, cardCode);
									}
								}
							}
							SetDocTotal(gvDoc, gvLine.GridControl, decimals, type);
						}
						if (e.Column.FieldName.ToUpper().Equals("REQDATE") && ((openFormViewMode == Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateDeliveryDate2Line, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) || openFormViewMode == Enums.FormViewModeType.Add))
						{
							for (int num18 = 0; num18 < gvLine.RowCount; num18++)
							{
								gvLine.SetRowCellValue(num18, "PQTReqDate", Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, "ReqDate")));
							}
						}
						if (e.Column.FieldName.ToUpper().Equals("SLPCODE") && ((gvLine.RowCount == 1 && string.IsNullOrEmpty(Function.ToString(gvLine.GetRowCellValue(0, "Slpcode")))) || (openFormViewMode == Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateSalesEmployee2Line, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) || openFormViewMode == Enums.FormViewModeType.Add))
						{
							for (int num19 = 0; num19 < gvLine.RowCount; num19++)
							{
								gvLine.SetRowCellValue(num19, "SlpCode", Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "SlpCode")));
							}
						}
						if (e.Column.FieldName.ToUpper().Equals("DocDueDate".ToUpper()) && (type == ApzObjectTypes.oOrders || type == ApzObjectTypes.oPurchaseOrders) && Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, "DocDueDate")).HasValue && ((openFormViewMode == Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateDeliveryDate2Line, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) || openFormViewMode == Enums.FormViewModeType.Add))
						{
							for (int num20 = 0; num20 < gvLine.RowCount; num20++)
							{
								gvLine.SetRowCellValue(num20, "ShipDate", Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, "DocDueDate")));
							}
						}
						if (e.Column.FieldName.ToUpper().Equals("DOCDATE") || e.Column.FieldName.ToUpper().Equals("TAXDATE") || e.Column.FieldName.ToUpper().Equals("CLSDATE"))
						{
							if (e.Column.FieldName.ToUpper().Equals("DOCDATE"))
							{
								for (int num21 = 0; num21 < lst.Count; num21++)
								{
									lst[num21]["DocDate"] = date;
								}
							}
							form.CalculateDueDate();
						}
						if (e.Column.FieldName.ToUpper().Equals("TOTAL"))
						{
							if (type == ApzObjectTypes.oDownPayments || type == ApzObjectTypes.oPurchaseDownPayments)
							{
								double num22 = Function.ParseDouble(e.Value);
								DataTable source = (gvLine.GridControl as GridControlEx).DatasourceTable();
								double num23 = ((!(doc.Currency == GlobalData.MainCurrency)) ? Function.ParseDouble((from t in source.AsEnumerable()
									where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
									select t).Sum((DataRow t) => Function.ParseDouble(t["TotalFrgn"]) * Function.ParseDouble(t["VatPrcnt"]) / 100.0)) : Function.ParseDouble((from t in source.AsEnumerable()
									where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
									select t).Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]) * Function.ParseDouble(t["VatPrcnt"]) / 100.0)));
								double num24 = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, "TotalBeforeDis"));
								double num25 = num22 / (num24 / 100.0 + num23 / 100.0);
								changeTotal = true;
								gvDoc.SetRowCellValue(e.RowHandle, "DpmPrcnt", num25);
								changeTotal = false;
							}
							else if (gvLine.GridControl.DataSource is DataTable source2)
							{
								if (doc.DocCur == GlobalData.MainCurrency)
								{
									double num26 = (from r in source2.AsEnumerable()
										where Function.ToString(r["ItemCode"]) != ""
										select r).Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
									double num27 = (from r in source2.AsEnumerable()
										where Function.ToString(r["ItemCode"]) != ""
										select r).Sum((DataRow t) => Math.Round(Function.ParseDouble(t["LineTotal"]) * Function.ParseDouble(t["VatPrcnt"]) / 100.0, decimals, MidpointRounding.AwayFromZero));
									double value2 = (1.0 - Function.ParseDouble(e.Value) / (num26 + num27)) * 100.0;
									changeTotal = true;
									gvDoc.SetFocusedRowCellValue("DiscPrcnt", Math.Round(value2, 6, MidpointRounding.AwayFromZero));
									changeTotal = false;
								}
								else
								{
									double num28 = (from r in source2.AsEnumerable()
										where Function.ToString(r["ItemCode"]) != ""
										select r).Sum((DataRow t) => Function.ParseDouble(t["TotalFrgn"]));
									double num29 = (from r in source2.AsEnumerable()
										where Function.ToString(r["ItemCode"]) != ""
										select r).Sum((DataRow t) => Math.Round(Function.ParseDouble(t["TotalFrgn"]) * Function.ParseDouble(t["VatPrcnt"]) / 100.0, decimals, MidpointRounding.AwayFromZero));
									double value3 = (1.0 - Function.ParseDouble(e.Value) / (num28 + num29)) * 100.0;
									changeTotal = true;
									gvDoc.SetFocusedRowCellValue("DiscPrcnt", Math.Round(value3, 6, MidpointRounding.AwayFromZero));
									changeTotal = false;
								}
							}
						}
						if (type == ApzObjectTypes.oDownPayments || type == ApzObjectTypes.oPurchaseDownPayments)
						{
							if (e.Column.FieldName.ToUpper().Equals("DPMPRCNT"))
							{
								double num30 = Function.ParseDouble(e.Value);
								if (num30 < 0.0)
								{
									WinCommons.Error(Enums.MessageCode.InDpmDownPaymentPerFieldEnterGreaterThan0, "");
									return;
								}
								gvDoc.SetRowCellValue(e.RowHandle, "DiscPrcnt", 100.0 - num30);
							}
							if (e.Column.FieldName.ToUpper().Equals("DPMTOTAL"))
							{
								double num31 = Math.Round(Function.ParseDouble(e.Value), decimals, MidpointRounding.AwayFromZero);
								if (num31 < 0.0)
								{
									WinCommons.Error(Enums.MessageCode.InDpmDownPaymentFieldEnterGreaterThan0, "");
									return;
								}
								DataTable source3 = gvLine.GridControl.DataSource as DataTable;
								double num32 = (from r in source3.AsEnumerable()
									where Function.ToString(r["ItemCode"]) != ""
									select r).Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
								double num33 = (from r in source3.AsEnumerable()
									where Function.ToString(r["ItemCode"]) != ""
									select r).Sum((DataRow t) => Math.Round(Function.ParseDouble(t["LineTotal"]) * Function.ParseDouble(t["VatPrcnt"]) / 100.0, decimals, MidpointRounding.AwayFromZero));
								double num34 = Math.Round(num31 * num33 / num32, decimals, MidpointRounding.AwayFromZero);
								Math.Round(num31 / num32 * 100.0, 6, MidpointRounding.AwayFromZero);
								changeDpmTotal = true;
								gvDoc.SetRowCellValue(e.RowHandle, "Total", num31 + num34);
								changeDpmTotal = false;
							}
						}
						if (e.Column.FieldName.ToUpper().Equals("DOCCUR") || e.Column.FieldName.ToUpper().Equals("DOCRATE") || e.Column.FieldName.ToUpper().Equals("DISCPRCNT") || e.Column.FieldName.ToUpper().Equals("DOCDATE"))
						{
							new DataTable();
							if (e.Column.FieldName.ToUpper().Equals("DOCCUR") || e.Column.FieldName.ToUpper().Equals("DOCDATE"))
							{
								if (e.Column.FieldName.ToUpper().Equals("DOCCUR") && e.Value != null)
								{
									ShowHideSumColumn(gvLine, e.Value.ToString(), freight);
								}
								if (cur.ToUpper() == GlobalData.MainCurrency.ToUpper())
								{
									doc.DocRate = 1.0;
								}
								else
								{
									double BPRate2 = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(cur, date);
									if (BPRate2 > 0.0)
									{
										gvDoc.SetRowCellValue(e.RowHandle, "DocRate", BPRate2);
									}
									else if (doc.DocCur != "##" && doc.DocCur.ToUpper() != GlobalData.MainCurrency.ToUpper() && !frm_ExchangeRatesAndIndexs._isOpen)
									{
										frm_ExchangeRatesAndIndexs frm2 = new frm_ExchangeRatesAndIndexs();
										frm2.Curr = doc.DocCur;
										frm2._date = date;
										frm2.FormClosing += (object s, FormClosingEventArgs e2) =>
										{
											BPRate2 = frm2.rate;
											gvDoc.SetRowCellValue(e.RowHandle, "DocRate", BPRate2);
										};
										frm2.ShowDialog();
									}
									else if (doc.DocCur.ToUpper() == GlobalData.MainCurrency)
									{
										doc.DocRate = 1.0;
									}
									else
									{
										doc.DocRate = 0.0;
									}
								}
							}
							double num35 = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, "DocRate"));
							if (!e.Column.FieldName.ToUpper().Equals("DISCPRCNT"))
							{
								if (freight != null)
								{
									DataTable dataTable3 = freight.GridControl.DataSourceTable();
									foreach (DataRow row2 in dataTable3.Rows)
									{
										_ = row2;
										if (!string.IsNullOrEmpty(doc.Currency) && !(cur.ToUpper() == GlobalData.MainCurrency.ToUpper()) && num35 != 0.0)
										{
											for (int num36 = 0; num36 < dataTable3.Rows.Count; num36++)
											{
												double num37 = Function.ParseDouble(dataTable3.Rows[num36]["TotalFrgn"]) / num35;
												freight.SetRowCellValue(freight.GetRowHandle(num36), "TotalFrgn", num37);
												double num38 = Function.ParseDouble(dataTable3.Rows[num36]["VatSumFrgn"]) / num35;
												freight.SetRowCellValue(freight.GetRowHandle(num36), "VatSumFrgn", num38);
											}
										}
									}
								}
								int num39 = GlobalData.GeneralSetting.SumDec.Value;
								int num40 = GlobalData.GeneralSetting.SumDec.Value;
								if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
								{
									CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency));
									if (currencyCodesImpl2 != null)
									{
										num40 = ((currencyCodesImpl2.Decimals == -1) ? num40 : currencyCodesImpl2.Decimals);
									}
								}
								for (int num41 = 0; num41 < lst.Rows.Count; num41++)
								{
									DocumentLineRow item = lst.Rows[num41] as DocumentLineRow;
									if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
									{
										CurrencyCodesImpl currencyCodesImpl3 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(item.Currency.ToUpper()));
										if (currencyCodesImpl3 != null)
										{
											num39 = ((currencyCodesImpl3.Decimals == -1) ? num39 : currencyCodesImpl3.Decimals);
										}
									}
									string cardCode2 = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
									DataTable dataTable4 = httpClient.Commons.DocumentPrice(item.ItemCode, cardCode2, doc.DocDate, item.Quantity, item.UoMCode, Function.ParseInt(doc["InfoPriceList"]));
									if (dataTable4.IsNotNull())
									{
										double num42 = Function.ParseDouble(dataTable4.Rows[0]["Price"]);
										string text3 = Function.ToString(dataTable4.Rows[0]["Currency"]);
										double num43 = Function.ParseDouble(dataTable4.Rows[0]["Discount"]);
										if (Function.ParseInt(item["UgpCode"]) == -1)
										{
											num42 *= item.UnitsOfMeasurment;
										}
										if (Math.Round(num42, num39, MidpointRounding.AwayFromZero) == Math.Round(item.PriceBefDi, num39, MidpointRounding.AwayFromZero) && text3.Equals(item.Currency) && (item.BaseType == -1 || item.BaseType == 0))
										{
											gvLine.SetRowCellValue(num41, "DiscPrcnt", num43);
										}
									}
									if (item.PriceBefDi == 0.0)
									{
										item.Currency = cur;
									}
									if (!string.IsNullOrEmpty(item.Currency))
									{
										if (item.Currency.ToUpper().Equals(cur.ToUpper()))
										{
											item.RateCurrency = num35;
											double num44 = ApzonCommon.LineTotal(Function.ParseDouble(item.Quantity), item.PriceBefDi, item.DiscountPercent);
											item.LineTotal = Math.Round(num44 * Math.Pow(num35, GlobalData.ExchangeRatePow), num40, MidpointRounding.AwayFromZero);
											item.LineVat = Math.Round(item.PriceBefDi * ((item.Rate == 0.0) ? 0.0 : Math.Pow(item.Rate, GlobalData.ExchangeRatePow)) * item.Quantity / 100.0 * (1.0 - docdiscount / 100.0), num40, MidpointRounding.AwayFromZero);
											if (cur.ToUpper() == GlobalData.MainCurrency.ToUpper())
											{
												item.TotalFrgn = 0.0;
												item.VatSumFrgn = 0.0;
											}
											else
											{
												item.TotalFrgn = Math.Round(ApzonCommon.LineTotal(Function.ParseDouble(item.Quantity), item.PriceBefDi, item.DiscountPercent), num39, MidpointRounding.AwayFromZero);
												item.VatSumFrgn = Math.Round(item.TotalFrgn * item.Rate / 100.0 * (1.0 - docdiscount / 100.0), num39, MidpointRounding.AwayFromZero);
											}
										}
										else
										{
											double exchangeRate = GetExchangeRate(item.Currency, date);
											if (exchangeRate == 0.0)
											{
												frm_ExchangeRatesAndIndexs frm3 = new frm_ExchangeRatesAndIndexs();
												frm3.Curr = item.Currency;
												frm3._date = date;
												frm3.FormClosing += (object s, FormClosingEventArgs e2) =>
												{
													item.RateCurrency = Function.ParseDouble(frm3.rate);
												};
												frm3.ShowDialog();
											}
											else
											{
												item.RateCurrency = exchangeRate;
											}
											double num45 = ApzonCommon.LineTotal(Function.ParseDouble(item.Quantity), item.PriceBefDi, item.DiscountPercent);
											item.LineTotal = num45 * Math.Pow(item.RateCurrency, GlobalData.ExchangeRatePow);
											item.LineVat = item.PriceBefDi * item.Rate * item.Quantity / 100.0 * (1.0 - docdiscount / 100.0);
											if (cur.ToUpper() == GlobalData.MainCurrency.ToUpper())
											{
												doc.TotalExpns = item.LineTotal;
												if (freight != null)
												{
													DataTable dataTable5 = (DataTable)freight.GridControl.DataSource;
													if (dataTable5.IsNotNull())
													{
														double num46 = 0.0;
														for (int num47 = 0; num47 < dataTable5.Rows.Count; num47++)
														{
															DataRow dataRow = dataTable5.Rows[num41];
															num46 += Function.ParseDouble(dataRow["LineTotal"]);
														}
														doc.TotalExpns += num46;
													}
													lst.Columns.Contains("LineTotal1");
												}
												item.TotalFrgn = 0.0;
												item.VatSumFrgn = 0.0;
											}
											else if (num35 != 0.0)
											{
												double num48 = num45;
												item.TotalFrgn = Math.Round(num48 * Math.Pow(item.RateCurrency / num35, GlobalData.ExchangeRatePow), num39, MidpointRounding.AwayFromZero);
												item.VatSumFrgn = Math.Round(Math.Pow(item.RateCurrency / num35, GlobalData.ExchangeRatePow) * item.LineVat, num39, MidpointRounding.AwayFromZero);
												_ = freight;
											}
										}
									}
								}
								lst.AcceptChanges();
								if (freight != null)
								{
									SetDocTotal(gvDoc, gvLine.GridControl, decimals, type, freight);
								}
								else
								{
									SetDocTotal(gvDoc, gvLine.GridControl, decimals, type);
								}
							}
							else
							{
								if (doc.DocCur == GlobalData.MainCurrency)
								{
									for (int num49 = 0; num49 < lst.Count; num49++)
									{
										lst[num49].LineVat = Math.Round(lst[num49].LineTotal * lst[num49].Rate / 100.0 * (1.0 - docdiscount / 100.0), decimals, MidpointRounding.AwayFromZero);
									}
								}
								else
								{
									for (int num50 = 0; num50 < lst.Count; num50++)
									{
										lst[num50].VatSumFrgn = Math.Round(lst[num50].TotalFrgn * lst[num50].Rate / 100.0 * (1.0 - docdiscount / 100.0), decimals, MidpointRounding.AwayFromZero);
									}
								}
								lst.AcceptChanges();
								if (freight != null)
								{
									SetDocTotal(gvDoc, gvLine.GridControl, decimals, type, freight, changeTotal, changeDpmTotal);
								}
								else
								{
									SetDocTotal(gvDoc, gvLine.GridControl, decimals, type, null, changeTotal, changeDpmTotal);
								}
								if (!changeDiscSum)
								{
									doc.Discount = Math.Round(doc.TotalBeforeDis - doc.Total + doc.TaxTotal, decimals, MidpointRounding.AwayFromZero);
								}
							}
							gvLine.GridControl.DataSourceTable().AcceptChanges();
							gvDoc.GridControl.DataSourceTable().AcceptChanges();
						}
						if (e.Column.FieldName == "Discount")
						{
							double num51 = Math.Round(Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, e.Column)) * 100.0 / doc.TotalBeforeDis, 6, MidpointRounding.AwayFromZero);
							changeDiscSum = true;
							gvDoc.SetRowCellValue(e.RowHandle, "DiscPrcnt", num51);
							changeDiscSum = false;
						}
						if (e.Column.FieldName.Equals("Project") && gv_Accounting != null && !Function.ToString(gv_Accounting.GetFocusedRowCellValue("Project")).Equals(Function.ToString(e.Value)))
						{
							gv_Accounting.SetFocusedRowCellValue("Project", e.Value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		};
	}

	private static bool IsInvoiceType(ApzObjectTypes type)
	{
		return new ApzObjectTypes[6]
		{
			ApzObjectTypes.oInvoices,
			ApzObjectTypes.oReserveInvoices,
			ApzObjectTypes.oPurchaseInvoices,
			ApzObjectTypes.oAPReserveInvoice,
			ApzObjectTypes.oCreditNotes,
			ApzObjectTypes.oPurchaseCreditNotes
		}.Contains(type);
	}

	public static void SetDocTotal(GridViewEx gvDoc, GridControl grdLine, int decimals, ApzObjectTypes objType = ApzObjectTypes.oItems, GridViewEx gvFreight = null, bool changeTotal = false, bool changeDpmTotal = false)
	{
		try
		{
			DocumentLineTable lst = (DocumentLineTable)grdLine.DataSource;
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocCur"));
			if (string.IsNullOrEmpty(text))
			{
				text = GlobalData.MainCurrency;
			}
			double num = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocRate"));
			double dis = ((gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DiscPrcnt") == DBNull.Value) ? 0.0 : Convert.ToDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DiscPrcnt")));
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = 0.0;
			double num11 = 0.0;
			double num12 = 0.0;
			double num13 = 0.0;
			double num14 = 0.0;
			double num15 = 0.0;
			double dpmTotal = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DPMTotal"));
			if (text == GlobalData.MainCurrency)
			{
				num7 = ((!_lstSalesObject.Contains(objType)) ? lst.GetEnumerable().Sum((DocumentLineRow x) => x.LineVat) : (from r in lst.GetEnumerable()
					group r by new
					{
						Col1 = r["VatGroup"],
						Col2 = Math.Sign(Function.ParseDouble(r["LineTotal"]))
					}).Select(g =>
				{
					DataRow dataRow = lst.NewRow();
					dataRow["VatPrcnt"] = g.Select((DocumentLineRow r) => r["VatPrcnt"]).FirstOrDefault();
					dataRow["LineTotal"] = g.Sum((DocumentLineRow r) => Function.ParseDouble(r["LineTotal"]));
					return dataRow;
				}).Sum((DataRow r) => Math.Round(Function.ParseDouble(r["VatPrcnt"]) * Function.ParseDouble(r["LineTotal"]) * (1.0 - dis / 100.0) / 100.0, decimals, MidpointRounding.AwayFromZero)));
				num2 = (from x in lst.GetEnumerable()
					where Function.ToString(x["TaxOnly"]) != "Y"
					select x).Sum((DocumentLineRow x) => Function.ParseDouble(x.LineTotal));
				num9 = Math.Round((1.0 - dis / 100.0) * num2, decimals, MidpointRounding.AwayFromZero);
				try
				{
					num5 = (from x in lst.GetEnumerable()
						where Function.ToString(x["TaxOnly"]) != "Y"
						select x).Sum((DocumentLineRow x) => Function.ParseDouble(x["LineTotal_F1"]) + Function.ParseDouble(x["LineTotal_F2"]) + Function.ParseDouble(x["LineTotal_F3"]));
					num7 += lst.GetEnumerable().Sum((DocumentLineRow x) => Function.ParseDouble(x["VatSum_F1"]) + Function.ParseDouble(x["VatSum_F2"]) + Function.ParseDouble(x["VatSum_F3"]));
				}
				catch (Exception)
				{
				}
				num15 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DpmVat"));
				num7 -= num15;
				if (gvFreight != null)
				{
					num5 += gvFreight.GridControl.DataSourceTable().AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
					num7 += gvFreight.GridControl.DataSourceTable().AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["VatSum"]));
				}
				num3 = ApzonCommon.DocTotal(num2, num7, dis, num5, dpmTotal, decimals);
				num4 = 0.0;
			}
			else
			{
				num8 = ((!_lstSalesObject.Contains(objType)) ? lst.GetEnumerable().Sum((DocumentLineRow x) => x.VatSumFrgn) : (from r in lst.GetEnumerable()
					group r by new
					{
						Col1 = r["VatGroup"],
						Col2 = Math.Sign(Function.ParseDouble(r["TotalFrgn"]))
					}).Select(g =>
				{
					DataRow dataRow = lst.NewRow();
					dataRow["VatPrcnt"] = g.Select((DocumentLineRow r) => r["VatPrcnt"]).FirstOrDefault();
					dataRow["TotalFrgn"] = g.Sum((DocumentLineRow r) => Function.ParseDouble(r["TotalFrgn"]));
					return dataRow;
				}).Sum((DataRow r) => Math.Round(Function.ParseDouble(r["VatPrcnt"]) * Function.ParseDouble(r["TotalFrgn"]) * (1.0 - dis / 100.0) / 100.0, decimals, MidpointRounding.AwayFromZero)));
				num2 = (from x in lst.GetEnumerable()
					where Function.ToString(x["TaxOnly"]) != "Y"
					select x).Sum((DocumentLineRow x) => x.TotalFrgn);
				num10 = Math.Round((1.0 - dis / 100.0) * num2, decimals, MidpointRounding.AwayFromZero);
				try
				{
					num6 = lst.GetEnumerable().Sum((DocumentLineRow x) => Function.ParseDouble(x["TotalFrgn_F1"]) + Function.ParseDouble(x["TotalFrgn_F2"]) + Function.ParseDouble(x["TotalFrgn_F3"]));
					num8 += lst.GetEnumerable().Sum((DocumentLineRow x) => Function.ParseDouble(x["VatSumFrgn_F1"]) + Function.ParseDouble(x["VatSumFrgn_F2"]) + Function.ParseDouble(x["VatSumFrgn_F3"]));
				}
				catch (Exception)
				{
				}
				num15 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DpmVatFc"));
				num8 -= num15;
				if (gvFreight != null)
				{
					num6 += gvFreight.GridControl.DataSourceTable().AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TotalFrgn"]));
					num8 += gvFreight.GridControl.DataSourceTable().AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["VatSumFrgn"]));
				}
				num3 = ApzonCommon.DocTotal((num2 * num != 0.0) ? Math.Pow(num, GlobalData.ExchangeRatePow) : 0.0, num7, dis, num5, dpmTotal, decimals);
				num4 = ApzonCommon.DocTotal(num2, num8, dis, num6, dpmTotal, decimals);
			}
			if (text.ToUpper() != GlobalData.MainCurrency.ToUpper())
			{
				num11 = num4;
				num12 = num8;
				num13 = num6;
				num14 = num10;
			}
			else
			{
				num11 = num3;
				num12 = num7;
				num13 = num5;
				num14 = num9;
			}
			DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
			focusedDataRow["TotalBeforeDis"] = num2;
			focusedDataRow["VatSum"] = num12;
			focusedDataRow["DocTotal"] = num3;
			focusedDataRow["DocTotalFc"] = num4;
			focusedDataRow["VatSumFc"] = num8;
			if (!changeTotal)
			{
				focusedDataRow["Total"] = num11;
			}
			focusedDataRow["TaxTotal"] = num12;
			focusedDataRow["TotalExpns"] = num5;
			focusedDataRow["TotalExpFC"] = num6;
			focusedDataRow["TotalFreight"] = num13;
			if (objType == ApzObjectTypes.oDownPayments || objType == ApzObjectTypes.oPurchaseDownPayments)
			{
				if (!changeDpmTotal)
				{
					focusedDataRow["DPMTotal"] = num14;
				}
				focusedDataRow["DpmAmnt"] = num9;
				focusedDataRow["DpmAmntFC"] = num10;
				focusedDataRow["DpmPrcnt"] = 100.0 - dis;
				if (!changeTotal)
				{
					focusedDataRow["Total"] = num14 + num12;
				}
			}
			gvDoc.GridControl.DataSourceTable().AcceptChanges();
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.ErrorLineNumber(ex3);
		}
	}

	public static void SetBusinessToGridDoc(DataTable buss, GridViewEx gv, int rowHandle, int docNum, ApzObjectTypes objType, GridControl grdLogistic = null, GridViewEx gv_Accounting = null, RepositoryItemLookUpEdit rBillTo = null, RepositoryItemLookUpEdit rShipTo = null, RepositoryItemLookUpEdit rlpShipType = null, RepositoryItemLookUpEdit rlupContactPersonCode = null, GridViewEx gvLine = null, VirFormDocument form = null)
	{
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(objType);
			DocumentTable documentTable = (DocumentTable)gv.GridControl.DataSource;
			DocumentRow doc = documentTable[gv.GetDataSourceRowIndex(rowHandle)];
			if (buss.IsExistsColumn("CardCode"))
			{
				doc.CardCode = Function.ToString(buss.Rows[0]["CardCode"]);
				if (buss.IsExistsColumn("CardName"))
				{
					gv.SetRowCellValue(rowHandle, "CardName", buss.Rows[0]["CardName"]);
					if (buss.IsExistsColumn("DebPayAcct"))
					{
						doc.CtlAccount = Function.ToString(buss.Rows[0]["DebPayAcct"]);
						if (buss.IsExistsColumn("CntctCode"))
						{
							gv.SetRowCellValue(rowHandle, "CntctCode", buss.Rows[0]["CntctCode"]);
							if (buss.IsExistsColumn("SlpCode"))
							{
								gv.SetRowCellValue(rowHandle, "SlpCode", buss.Rows[0]["SlpCode"]);
								if (buss.IsExistsColumn("ShipType"))
								{
									doc.TransportationCode = Function.ParseInt(buss.Rows[0]["ShipType"]);
									gv.SetRowCellValue(rowHandle, "ShipType", buss.Rows[0]["ShipType"]);
									if (buss.IsExistsColumn("ShipToDef"))
									{
										doc.ShipToCode = Function.ToString(buss.Rows[0]["ShipToDef"]);
										gv.SetRowCellValue(rowHandle, "ShipToDef", buss.Rows[0]["ShipToDef"]);
										if (buss.IsExistsColumn("BillToDef"))
										{
											doc.PayToCode = Function.ToString(buss.Rows[0]["BillToDef"]);
											gv.SetRowCellValue(rowHandle, "BillToDef", buss.Rows[0]["BillToDef"]);
											doc.JournalMemo = objType.ToString() + " - " + doc.CardCode;
											if (buss.IsExistsColumn("Discount"))
											{
												gv.SetRowCellValue(rowHandle, "DiscPrcnt", buss.Rows[0]["Discount"]);
												if (buss.IsExistsColumn("PriceMode"))
												{
													gv.SetRowCellValue(rowHandle, "PriceMode", buss.Rows[0]["PriceMode"]);
												}
												bool flag = true;
												if (form != null && form.OpenFormViewMode == Enums.FormViewModeType.Add && form.MappingDocument.Any((ContextActionMappingImpl t) => t.TargetField == "DocDueDate"))
												{
													flag = false;
												}
												if (flag && IsInvoiceType(objType) && buss.IsExistsColumn("PayDuMonth"))
												{
													Function.ToString(buss.Rows[0]["PayDuMonth"]);
													gv.SetRowCellValue(rowHandle, "PayDuMonth", buss.Rows[0]["PayDuMonth"]);
													Function.ToString(buss.Rows[0]["BslineDate"]);
													gv.SetRowCellValue(rowHandle, "BslineDate", buss.Rows[0]["BslineDate"]);
													Function.ParseInt(buss.Rows[0]["ExtraMonth"]);
													gv.SetRowCellValue(rowHandle, "ExtraMonth", buss.Rows[0]["ExtraMonth"]);
													Function.ParseInt(buss.Rows[0]["ExtraDays"]);
													gv.SetRowCellValue(rowHandle, "ExtraDays", buss.Rows[0]["ExtraDays"]);
													DateTime? dateTime = SetDueDateByPaymentTerm(doc);
													gv.SetRowCellValue(rowHandle, "DocDueDate", dateTime);
												}
												if (gv_Accounting != null)
												{
													if (!buss.IsExistsColumn("GroupNum"))
													{
														throw new Exception("Could not get \"Payment term\" data");
													}
													gv_Accounting.SetFocusedRowCellValue("GroupNum", Function.ParseInt(buss.Rows[0]["GroupNum"]));
													if (!buss.IsExistsColumn("UseShpdGd"))
													{
														throw new Exception("Could not get \"Payment term\" data");
													}
													gv_Accounting.SetFocusedRowCellValue("UseShpdGd", Function.ToString(buss.Rows[0]["UseShpdGd"]));
													if (!buss.IsExistsColumn("ProjectCod"))
													{
														throw new Exception("Could not get \"Payment term\" data");
													}
													gv_Accounting.SetFocusedRowCellValue("Project", Function.ToString(buss.Rows[0]["ProjectCod"]));
													if (!buss.IsExistsColumn("Indicator"))
													{
														throw new Exception("Could not get \"Payment term\" data");
													}
													gv_Accounting.SetFocusedRowCellValue("Indicator", Function.ToString(buss.Rows[0]["Indicator"]));
												}
												if (buss.IsExistsColumn("ListNum"))
												{
													gv.SetRowCellValue(rowHandle, "InfoPriceList", buss.Rows[0]["ListNum"]);
													string text = Function.ToString(buss.Rows[0]["CardType"]);
													gv.GridControl.DataSource = documentTable;
													documentTable.AcceptChanges();
													if (grdLogistic != null)
													{
														DataSet dataCombobox = httpClient.Commons.GetDataCombobox(doc.CardCode);
														if (dataCombobox != null && dataCombobox.Tables.Count > 3)
														{
															if (rBillTo != null)
															{
																rBillTo.DataSource = dataCombobox.Tables[0];
																DataRow dataRow = dataCombobox.Tables[0].AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Address"]).Equals(doc.PayToCode));
																if (dataRow != null)
																{
																	doc.Address = Function.ToString(dataRow["Street"]).Replace("\r", "\n");
																	gv.SetRowCellValue(rowHandle, "Address", Function.ToString(dataRow["Street"]).Replace("\r", "\n"));
																}
															}
															if (rShipTo != null)
															{
																rShipTo.DataSource = dataCombobox.Tables[1];
																DataRow dataRow2 = dataCombobox.Tables[1].AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Address"]).Equals(doc.ShipToCode));
																if (dataRow2 != null)
																{
																	doc.Address2 = Function.ToString(dataRow2["Street"]).Replace("\r", "\n");
																	gv.SetRowCellValue(rowHandle, "Address", Function.ToString(dataRow2["Street"]).Replace("\r", "\n"));
																}
															}
															if (text.Equals("S"))
															{
																doc.Address2 = GlobalData.GeneralSetting.CompnyAddr.Replace("\r", "\n");
																gv.SetRowCellValue(rowHandle, "Address2", GlobalData.GeneralSetting.CompnyAddr.Replace("\r", "\n"));
															}
															if (rlpShipType != null)
															{
																rlpShipType.DataSource = dataCombobox.Tables[2];
															}
															if (rlupContactPersonCode != null)
															{
																rlupContactPersonCode.DataSource = dataCombobox.Tables[3];
															}
														}
														documentTable.AcceptChanges();
														DocumentTable clone = DocumentsHelper.Document.GetClone();
														clone.ImportRow(doc);
														clone.AcceptChanges();
														grdLogistic.DataSource = clone;
														grdLogistic.RefreshDataSource();
														if (grdLogistic.MainView is GridViewEx gridViewEx)
														{
															gridViewEx.SetFocusedRowCellValue("BPChCode", buss.Rows[0]["ChannlBP"]);
														}
													}
													gv.GridControl.DataSourceTable().AcceptChanges();
													gv.GridControl.RefreshDataSource();
													gv.FocusedColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.FieldName.ToUpper().Equals("CARDNAME"));
													if (gvLine.Columns["ShipToCode"] != null && gvLine.Columns["ShipToCode"].ColumnEdit is RepositoryItemLookUpEditSimple repositoryItemLookUpEditSimple && text != "" && text != "S")
													{
														IHttpClient htClient = HttpClientHelperFactory.GetHttpClient();
														DataTable result = Task.Run(async () => await htClient.Documents.GetLineShipTo(doc.CardCode)).Result;
														if (result != null && !result.AsEnumerable().Any((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["Address"]))))
														{
															DataRow dataRow3 = result.NewRow();
															dataRow3["Address"] = "";
															result.Rows.InsertAt(dataRow3, 0);
															result.AcceptChanges();
														}
														repositoryItemLookUpEditSimple.DataSource = result;
													}
													return;
												}
												throw new Exception("Could not get \"Discount\" data");
											}
											throw new Exception("Could not get \"Discount\" data");
										}
										throw new Exception("Could not get \"Default Bill Address\" data");
									}
									throw new Exception("Could not get \"Default Shipping Address\" data");
								}
								throw new Exception("Could not get \"Shipping Type\" data");
							}
							throw new Exception("Could not get \"Sale Employee\" data");
						}
						throw new Exception("Could not get \"Contact person\" data");
					}
					throw new Exception("Could not get \"Business Partner Receivable/Payable Accounts\" data");
				}
				throw new Exception("Could not get \"Business partner Name\" data");
			}
			throw new Exception("Could not get \"Business partner Code\" data");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static DateTime? SetDueDateByPaymentTerm(DocumentRow doc)
	{
		DateTime? dateTime = DateTime.Today;
		switch (doc.BslineDate)
		{
		case "P":
			dateTime = doc.DocDate;
			break;
		case "S":
			dateTime = GlobalData.SystemDate;
			break;
		case "T":
			dateTime = doc.TaxDate;
			break;
		case "C":
			dateTime = doc.CloseDate;
			break;
		}
		DateTime dateTime2 = dateTime ?? DateTime.Today;
		DateTime? result = doc.PayDuMonth switch
		{
			"E" => (DateTime?)new DateTime(dateTime2.Year, dateTime2.Month, DateTime.DaysInMonth(dateTime2.Year, dateTime2.Month)).AddMonths(doc.ExtraMonth).AddDays(doc.ExtraDays), 
			"H" => new DateTime(dateTime2.Year, dateTime2.Month, 15).AddMonths(doc.ExtraMonth).AddDays(doc.ExtraDays), 
			"Y" => new DateTime(dateTime2.Year, dateTime2.Month, 1).AddMonths(doc.ExtraMonth).AddDays(doc.ExtraDays), 
			_ => dateTime2.AddMonths(doc.ExtraMonth).AddDays(doc.ExtraDays), 
		};
		return result;
	}

	public static void InitLineCellValueChanged(this GridViewEx gvLine, VirFormDocument formDocument, GridViewEx gvDoc, ucBatchAndSerial batchSerial, RepositoryItemLookUpEdit rLookUpVatGroup, GridControl gridLogistic = null, List<Formular> formulars = null, Dictionary<string, GridViewEx> tblFormular = null, string vatType = "", ApzObjectTypes objType = ApzObjectTypes.oNone, List<Common.Table_ID_Code> _lstBomChilds = null, GridViewEx gvFreight = null, string ctd = "")
	{
		string sourceActionField = "";
		bool changeLineTotal = false;
		gvLine.CellValueChanged += async (object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) =>
		{
			try
			{
				bool flag = isSetItemDocline;
				if (frm_ExchangeRatesAndIndexs.isClosing)
				{
					return;
				}
				DocumentLineTable documentLineTable = (DocumentLineTable)gvLine.GridControl.DataSource;
				DocumentTable dt = gvDoc.GridControl.DataSource as DocumentTable;
				int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
				DocumentRow doc = dt.GetByNum(docNum);
				if (doc == null)
				{
					return;
				}
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(objType);
				double num = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "DiscPrcnt"));
				if (num > 100.0)
				{
					WinCommons.Error(Enums.MessageCode.EnterDiscountPercentageLessThan100, "");
					return;
				}
				int lineNum = Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "LineNum"));
				DocumentLineRow byNum = documentLineTable.GetByNum(lineNum);
				if (byNum == null)
				{
					return;
				}
				double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscPrcnt"));
				double num3 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Quantity"));
				double num4 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "PriceBefDi"));
				double num5 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "PriceAfVAT"));
				int num6 = GlobalData.GeneralSetting.SumDec.Value;
				int num7 = GlobalData.GeneralSetting.SumDec.Value;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(doc.DocCur.ToUpper()));
					if (currencyCodesImpl != null)
					{
						num6 = ((currencyCodesImpl.Decimals == -1) ? num6 : currencyCodesImpl.Decimals);
						num7 = ((currencyCodesImpl.Decimals == -1) ? num7 : currencyCodesImpl.Decimals);
					}
				}
				int num8 = GlobalData.GeneralSetting.SumDec.Value;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency));
					if (currencyCodesImpl2 != null)
					{
						num8 = ((currencyCodesImpl2.Decimals == -1) ? num8 : currencyCodesImpl2.Decimals);
					}
				}
				double num9 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "VatPrcnt"));
				double num10 = ((Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Rate")) == 0.0) ? 1.0 : Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Rate")));
				string docCurrency = doc.DocCurrency;
				DateTime dateTime = ((gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "DocDate").ToString()));
				int baseType = Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "BaseType"));
				string text = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "TreeType"));
				if ((e.Column.FieldName == "BaseType" || e.Column.FieldName == "BaseRef" || e.Column.FieldName == "BaseEntry") && text == "S")
				{
					for (int num11 = e.RowHandle; num11 < gvLine.RowCount; num11++)
					{
						if (Function.ToString(gvLine.GetRowCellValue(num11, "TreeType")) == "I")
						{
							int dataSourceRowIndex = gvLine.GetDataSourceRowIndex(num11);
							documentLineTable[dataSourceRowIndex][e.Column.FieldName] = e.Value;
						}
					}
				}
				if (e.Column.FieldName == "BaseLine" && text == "S")
				{
					int baseEntry = Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "BaseEntry"));
					int baseRow = Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "BaseLine"));
					DataTable dataTable = WinFuntions.CopyChildItemsInBom(Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "ItemCode")), baseType, baseEntry, baseRow);
					if (dataTable.IsNotNull())
					{
						int chilNum = 1;
						int i;
						for (i = e.RowHandle + 1; i < gvLine.RowCount; i++)
						{
							if (Function.ToString(gvLine.GetRowCellValue(i, "TreeType")) == "I")
							{
								int dataSourceRowIndex2 = gvLine.GetDataSourceRowIndex(i);
								DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ChildNum"]) == chilNum && Function.ToString(t["ItemCode"]) == Function.ToString(gvLine.GetRowCellValue(i, "ItemCode")));
								if (dataRow != null)
								{
									documentLineTable[dataSourceRowIndex2][e.Column.FieldName] = dataRow["BaseRow"];
									documentLineTable[dataSourceRowIndex2]["Quantity"] = Function.ParseDouble(dataRow["Quantity"]) / Function.ParseDouble(dataRow["BaseQuantity"]) * num3;
									documentLineTable[dataSourceRowIndex2]["PriceBefDi"] = Function.ParseDouble(dataRow["PriceBefDi"]);
									documentLineTable[dataSourceRowIndex2]["VatGroup"] = Function.ToString(dataRow["VatGroup"]);
									documentLineTable[dataSourceRowIndex2]["TaxCode"] = Function.ToString(dataRow["TaxCode"]);
									documentLineTable[dataSourceRowIndex2]["VatPrcnt"] = Function.ParseDouble(dataRow["VatPrcnt"]);
									documentLineTable[dataSourceRowIndex2]["Project"] = Function.ToString(dataRow["Project"]);
									documentLineTable[dataSourceRowIndex2]["WhsCode"] = Function.ToString(dataRow["WhsCode"]);
									documentLineTable[dataSourceRowIndex2]["OcrCode"] = Function.ToString(dataRow["OcrCode"]);
									documentLineTable[dataSourceRowIndex2]["OcrCode2"] = Function.ToString(dataRow["OcrCode2"]);
									documentLineTable[dataSourceRowIndex2]["OcrCode3"] = Function.ToString(dataRow["OcrCode3"]);
									documentLineTable[dataSourceRowIndex2]["OcrCode4"] = Function.ToString(dataRow["OcrCode4"]);
									documentLineTable[dataSourceRowIndex2]["OcrCode5"] = Function.ToString(dataRow["OcrCode5"]);
									documentLineTable[dataSourceRowIndex2]["FreeTxt"] = Function.ToString(dataRow["FreeTxt"]);
									foreach (DataColumn item in from DataColumn t in dataTable.Columns
										where t.ColumnName.StartsWith("U_")
										select t)
									{
										documentLineTable[dataSourceRowIndex2][item.ColumnName] = dataRow[item.ColumnName];
									}
								}
							}
							chilNum++;
						}
					}
				}
				if (e.Column.FieldName.ToLower().Equals("quantity"))
				{
					string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
					if (!sourceActionField.ToLower().Equals("invqty"))
					{
						sourceActionField = "quantity";
						gvLine.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(e.Value) * Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumPerMsr")));
					}
					else
					{
						sourceActionField = "";
					}
					gvLine.SetRowCellValue(e.RowHandle, "OpenInvQty", Function.ParseDouble(e.Value) * Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumPerMsr")));
					if (vatType == "O")
					{
						double num12 = Function.ParseDouble(byNum["SalPackUn"]);
						if (Math.Abs(num12) < 1E-07)
						{
							num12 = 1.0;
						}
						double num13 = Function.ParseDouble(e.Value) / num12;
						double num14 = Math.Truncate(num13);
						byNum["PackQty"] = ((num13 - num14 > 1E-07) ? (num14 + 1.0) : num14);
					}
					else if (vatType == "I")
					{
						double num15 = Function.ParseDouble(byNum["PurPackUn"]);
						if (Math.Abs(num15) < 1E-07)
						{
							num15 = 1.0;
						}
						double num16 = Function.ParseDouble(e.Value) / num15;
						double num17 = Math.Truncate(num16);
						byNum["PackQty"] = ((num16 - num17 > 1E-07) ? (num17 + 1.0) : num17);
					}
					if (!Function.ToString(byNum["SpecPrice"]).Equals("N"))
					{
						DataTable dataTable2 = httpClient.Commons.DocumentPrice(byNum.ItemCode, cardCode, doc.DocDate, Function.ParseDouble(e.Value), byNum.UoMCode, Function.ParseInt(doc["InfoPriceList"]));
						formDocument.IsLoadPrice = true;
						if (dataTable2.IsNotNull())
						{
							num = Function.ParseDouble(dataTable2.Rows[0]["Discount"]);
							string text2 = Function.ToString(dataTable2.Rows[0]["IsGrossPrc"]);
							gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", num);
							gvLine.SetRowCellValue(e.RowHandle, "Currency", (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0]["Currency"]))) ? dataTable2.Rows[0]["Currency"] : doc.DocCurrency);
							double num18 = Function.ParseDouble(dataTable2.Rows[0]["Price"]);
							if (Function.ParseInt(byNum["UgpCode"]) == -1)
							{
								num18 *= byNum.UnitsOfMeasurment;
							}
							if (text2 == "Y")
							{
								num18 /= 1.0 + byNum.VatPrcnt / 100.0;
							}
							if (num18 != 0.0 && num4 == 0.0)
							{
								gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", num18);
								gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", dataTable2.Rows[0]["Discount"]);
								byNum["SpecPrice"] = dataTable2.Rows[0]["SpecPrice"];
							}
						}
						else if (num4 == 0.0)
						{
							gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", 0);
							gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", 0);
							byNum["SpecPrice"] = "R";
						}
						formDocument.IsLoadPrice = false;
					}
				}
				else if (e.Column.FieldName.ToLower().Equals("invqty"))
				{
					double num19 = Function.ParseDouble(e.Value) / Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumPerMsr"));
					if (!sourceActionField.ToLower().Equals("quantity") && !sourceActionField.ToLower().Equals("uomcode"))
					{
						sourceActionField = "invqty";
						gvLine.SetRowCellValue(e.RowHandle, "Quantity", num19);
					}
					else
					{
						sourceActionField = "";
					}
				}
				else if (e.Column.FieldName.ToLower().Equals("linetype"))
				{
					string text3 = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "LineType"));
					DataRow dataRow2 = documentLineTable.NewRow();
					dataRow2["LineType"] = text3;
					DataRow dataRow3 = gvLine.GetDataRow(e.RowHandle);
					if (dataRow3 != null)
					{
						dataRow3.ItemArray = dataRow2.ItemArray;
					}
					if (text3 != "R" && text3 != "")
					{
						if (e.RowHandle == 0)
						{
							dataRow3["LineNum"] = -1;
						}
						else
						{
							int num20 = -1;
							for (int num21 = e.RowHandle; num21 >= 0; num21--)
							{
								if (Function.ToString(gvLine.GetRowCellValue(num21, "LineType")) == "R")
								{
									num20++;
								}
							}
							dataRow3["LineNum"] = num20;
						}
						if (e.RowHandle == gvLine.RowCount - 1)
						{
							AddNewRow(gvLine.GridControl, gvDoc, gvLine, gridLogistic, doc.DocEntry);
						}
					}
					if (text3 == "T")
					{
						if (!gvLine.IsMappingContextAction)
						{
							frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
							if (frm_RowTextDetails2.ShowDialog() == DialogResult.OK)
							{
								gvLine.SetRowCellValue(e.RowHandle, "ItemCode", frm_RowTextDetails2.RowText);
							}
						}
					}
					else
					{
						if (text3 == "S")
						{
							dataRow3["Dscription"] = "Subtotal";
						}
						gvLine.Columns["ItemCode"].Width++;
						gvLine.Columns["ItemCode"].Width--;
						ReloadSubTotal(gvLine, e.RowHandle);
					}
				}
				if (e.Column.FieldName.ToUpper().Equals("LINETOTAL") || e.Column.FieldName.ToUpper().Equals("TotalFrgn".ToUpper()))
				{
					changeLineTotal = true;
					double num22 = ((!(docCurrency.ToUpper() == GlobalData.MainCurrency.ToUpper())) ? Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "TotalFrgn")) : Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "LineTotal")));
					if (num3 == 0.0 && objType != ApzObjectTypes.oPurchaseQuotations)
					{
						WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterGreaterThan0, "");
						return;
					}
					if (num4.Equals(0.0))
					{
						byNum.DiscountPercent = 0.0;
						gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", num22 / num3);
					}
					else
					{
						gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", (num3 * num4 - num22) * 100.0 / (num3 * num4));
					}
					ReloadSubTotal(gvLine, e.RowHandle);
					changeLineTotal = false;
				}
				if (e.Column.FieldName.ToUpper().Equals("GTOTAL") || e.Column.FieldName.ToUpper().Equals("GTOTALFC"))
				{
					double num23 = ((!(docCurrency.ToUpper() == GlobalData.MainCurrency.ToUpper())) ? Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "GTotalFC")) : Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "GTotal")));
					if (num3 == 0.0 && objType != ApzObjectTypes.oPurchaseQuotations)
					{
						WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterGreaterThan0, "");
						return;
					}
					if (num5.Equals(0.0))
					{
						byNum.DiscountPercent = 0.0;
						gvLine.SetRowCellValue(e.RowHandle, "PriceAfVAT", num23 / num3);
					}
					else
					{
						gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", (1.0 - num23 / ((1.0 + num9 / 100.0) * num4)) * 100.0);
					}
					ReloadSubTotal(gvLine, e.RowHandle);
				}
				if (!e.Column.FieldName.StartsWith("U_") && e.Column.FieldName.Length > 3 && e.Column.FieldName.Substring(e.Column.FieldName.Length - 3, 3).StartsWith("_F"))
				{
					int num24 = Function.ParseInt(e.Column.FieldName.Last());
					if (num24 > 0)
					{
						string text4 = "ExpnsCode_F" + num24;
						if (e.Column.FieldName.Equals(text4))
						{
							string expnsCode = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, text4));
							DataTable expnsByCode = httpClient.Documents.GetExpnsByCode(expnsCode);
							gvLine.SetExpnsToLine(expnsByCode, num24, e.RowHandle);
						}
						text4 = "LineTotal_F" + num24;
						if (e.Column.FieldName.Equals(text4))
						{
							double num25 = Function.ParseDouble(e.Value);
							double num26 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "VatPrcnt_F" + num24));
							gvLine.SetRowCellValue(e.RowHandle, "VatSum_F" + num24, num25 * num26 / 100.0);
						}
						text4 = "TotalFrgn_F" + num24;
						if (e.Column.FieldName.Equals(text4))
						{
							double num27 = Function.ParseDouble(e.Value);
							double num28 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "VatPrcnt_F" + num24));
							gvLine.SetRowCellValue(e.RowHandle, "VatSumFrgn_F" + num24, num27 * num28 / 100.0);
						}
						text4 = "VatGroup_F" + num24;
						if (e.Column.FieldName.Equals(text4))
						{
							List<VatGroupsImpl> source = (List<VatGroupsImpl>)rLookUpVatGroup.DataSource;
							string vatCode = Function.ToString(e.Value);
							VatGroupsImpl vatGroupsImpl = source.FirstOrDefault((VatGroupsImpl t) => t.Code == vatCode);
							if (vatGroupsImpl != null)
							{
								gvLine.SetRowCellValue(e.RowHandle, "VatPrcnt_F" + num24, Function.ParseDouble(vatGroupsImpl.Rate));
							}
						}
						text4 = "VatPrcnt_F" + num24;
						if (e.Column.FieldName.Equals(text4))
						{
							double num29 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "LineTotal_F" + num24));
							double num30 = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "TotalFrgn_F" + num24));
							double num31 = Function.ParseDouble(e.Value);
							gvLine.SetRowCellValue(e.RowHandle, "VatSum_F" + num24, num29 * num31 / 100.0);
							gvLine.SetRowCellValue(e.RowHandle, "VatSumFrgn_F" + num24, num30 * num31 / 100.0);
						}
						if (new string[4]
						{
							"VatPrcnt_F" + num24,
							"LineTotal_F" + num24,
							"VatSum_F" + num24,
							"TotalFrgn_F" + num24
						}.Contains(e.Column.FieldName))
						{
							SetDocTotal(gvDoc, gvLine.GridControl, num6, objType, gvFreight);
						}
					}
				}
				if (e.Column.FieldName.Equals("PriceAfVAT"))
				{
					gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", num5 / ((1.0 + num9 / 100.0) * (1.0 - num / 100.0)));
				}
				else if (new string[4] { "Quantity", "PriceBefDi", "DiscPrcnt", "VatPrcnt" }.Contains(e.Column.FieldName))
				{
					if (e.Column.FieldName == "DiscPrcnt" && Function.ParseDouble(e.Value) != Math.Round(Function.ParseDouble(e.Value), GlobalData.PercentageRateFormat.NumberDecimalDigits, MidpointRounding.AwayFromZero))
					{
						gvLine.SetRowCellValue(e.RowHandle, e.Column, Math.Round(Function.ParseDouble(e.Value), GlobalData.PercentageRateFormat.NumberDecimalDigits, MidpointRounding.AwayFromZero));
						return;
					}
					double num32 = ((!changeLineTotal) ? ApzonCommon.LineTotal(num3, num4, num) : ((docCurrency.ToUpper() == GlobalData.MainCurrency.ToUpper()) ? byNum.LineTotal : byNum.TotalFrgn));
					byNum.RemainingOpenQuantity = num3;
					byNum.Price = num4 - num4 * num / 100.0;
					byNum.PriceAfVAT = Math.Round((1.0 + num9 / 100.0) * byNum.Price, num6, MidpointRounding.AwayFromZero);
					double num33 = byNum.PriceAfVAT * num3;
					if (!changeLineTotal)
					{
						if (num10 == 0.0)
						{
							byNum.LineTotal = Math.Round(num32, num8, MidpointRounding.AwayFromZero);
						}
						else
						{
							byNum.LineTotal = Math.Round(num32 * Math.Pow(num10, GlobalData.ExchangeRatePow), num8, MidpointRounding.AwayFromZero);
						}
					}
					if (num10 == 0.0)
					{
						byNum.GTotal = Math.Round(num33, num8, MidpointRounding.AwayFromZero);
					}
					else
					{
						byNum.GTotal = Math.Round(num33 * Math.Pow(num10, GlobalData.ExchangeRatePow), num8, MidpointRounding.AwayFromZero);
					}
					double num34 = num32 * num9 / 100.0;
					if (num9 >= 1.0)
					{
						num34 = num32 * num9 / 100.0;
					}
					byNum.LineVat = Math.Round(Math.Pow(num10, GlobalData.ExchangeRatePow) * num34 * (1.0 - num2 / 100.0), num8, MidpointRounding.AwayFromZero);
					if (docCurrency.ToUpper() == GlobalData.MainCurrency.ToUpper())
					{
						byNum.TotalFrgn = 0.0;
						byNum.VatSumFrgn = 0.0;
						byNum.GTotalFC = 0.0;
					}
					else
					{
						byNum.TotalFrgn = Math.Round(num32, num7, MidpointRounding.AwayFromZero);
						byNum.GTotalFC = Math.Round(num33, num7, MidpointRounding.AwayFromZero);
						if (byNum.Currency == docCurrency)
						{
							if (num34 == 0.0)
							{
								byNum.VatSumFrgn = 0.0;
							}
							else
							{
								byNum.VatSumFrgn = num34;
							}
						}
						else
						{
							byNum.TotalFrgn = Math.Round(Math.Pow(byNum.RateCurrency / doc.DocRate, GlobalData.ExchangeRatePow) * num32, num6, MidpointRounding.AwayFromZero);
							byNum.GTotalFC = Math.Round(Math.Pow(byNum.RateCurrency / doc.DocRate, GlobalData.ExchangeRatePow) * num33, num6, MidpointRounding.AwayFromZero);
							if (num34 == 0.0)
							{
								byNum.VatSumFrgn = 0.0;
							}
							else
							{
								byNum.VatSumFrgn = Math.Round(Math.Pow(byNum.RateCurrency / doc.DocRate, GlobalData.ExchangeRatePow) * num34, num6, MidpointRounding.AwayFromZero);
							}
						}
					}
					SetDocTotal(gvDoc, gvLine.GridControl, num6, objType, gvFreight);
					if (e.Column.FieldName.Equals("Quantity"))
					{
						string pBType = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "BOMTreeType"));
						ChangeChildBOMItemsQuantity(gvLine, _lstBomChilds, Function.ParseDecimal(e.Value), pBType, Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "LineNum")));
					}
					ReloadSubTotal(gvLine, e.RowHandle);
				}
				if ((objType == ApzObjectTypes.oPurchaseInvoices || objType == ApzObjectTypes.oPurchaseRequest || objType == ApzObjectTypes.oPurchaseOrders || objType == ApzObjectTypes.oPurchaseQuotations || objType == ApzObjectTypes.oPurchaseDownPayments || objType == ApzObjectTypes.oPurchaseDeliveryNotes) && (e.Column.FieldName == "LineVat" || e.Column.FieldName == "VatSumFrgn"))
				{
					SetDocTotal(gvDoc, gvLine.GridControl, num6, objType, gvFreight);
				}
				if (e.Column.FieldName.Equals("Rate"))
				{
					ApzonCommon.LineTotal(num3, num4, num);
					double num35 = byNum.PriceAfVAT * num3;
					if (num10 == 0.0)
					{
						byNum.GTotal = Function.ParseDouble(Math.Round(Function.ParseDecimal(num35), num8, MidpointRounding.AwayFromZero));
					}
					else
					{
						byNum.GTotal = Function.ParseDouble(Math.Round(Function.ParseDecimal(num35 * Math.Pow(num10, GlobalData.ExchangeRatePow)), num8, MidpointRounding.AwayFromZero));
					}
					if (docCurrency.ToUpper() == GlobalData.MainCurrency.ToUpper())
					{
						byNum.GTotalFC = 0.0;
					}
					else
					{
						byNum.GTotalFC = Math.Round(num35, num7, MidpointRounding.AwayFromZero);
						if (byNum.Currency != docCurrency)
						{
							byNum.GTotalFC = Math.Round(Math.Pow(byNum.RateCurrency / doc.DocRate, GlobalData.ExchangeRatePow) * num35, num6, MidpointRounding.AwayFromZero);
						}
					}
					SetDocTotal(gvDoc, gvLine.GridControl, num6, objType, gvFreight);
					ReloadSubTotal(gvLine, e.RowHandle);
				}
				else if (e.Column.FieldName.Equals("VatGroup") || e.Column.FieldName.Equals("TaxCode"))
				{
					List<VatGroupsImpl> source2 = (List<VatGroupsImpl>)rLookUpVatGroup.DataSource;
					string vatCode2 = Function.ToString(e.Value);
					VatGroupsImpl vatGroupsImpl2 = source2.FirstOrDefault((VatGroupsImpl t) => t.Code == vatCode2);
					num9 = 0.0;
					if (vatGroupsImpl2 != null)
					{
						num9 = Function.ParseDouble(vatGroupsImpl2.Rate);
					}
					gvLine.SetRowCellValue(e.RowHandle, "VatPrcnt", num9);
					gvLine.GridControl.DataSourceTable().AcceptChanges();
				}
				else if (e.Column.FieldName == "TaxOnly")
				{
					SetDocTotal(gvDoc, gvLine.GridControl, num6, objType);
				}
				if (!formDocument.IsLoadPrice && new string[3] { "PriceBefDi", "DiscPrcnt", "Currency" }.Contains(e.Column.FieldName))
				{
					byNum["SpecPrice"] = "N";
				}
				if (e.Column.FieldName == "UomCode")
				{
					DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "UgpCode")), Function.ToString(gvLine.GetFocusedRowCellValue("UomCode")));
					if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
					{
						byNum["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
						byNum.UoMName = Function.ToString(dataUomByCode.Rows[0]["UomName"]);
						byNum.UnitsOfMeasurment = Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
						sourceActionField = "uomcode";
						gvLine.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Quantity")) * Function.ParseDouble(gvLine.GetFocusedRowCellValue("NumPerMsr")));
						gvLine.SetRowCellValue(e.RowHandle, "OpenInvQty", Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "OpenQty")) * Function.ParseDouble(gvLine.GetFocusedRowCellValue("NumPerMsr")));
						if (!Function.ToString(byNum["SpecPrice"]).Equals("N"))
						{
							DataTable dataTable3 = _httpClient.Commons.DocumentPrice(byNum.ItemCode, doc.CardCode, doc.DocDate, byNum.Quantity, byNum.UoMCode, Function.ParseInt(doc["InfoPriceList"]));
							formDocument.IsLoadPrice = true;
							if (dataTable3.IsNotNull())
							{
								string text5 = Function.ToString(dataTable3.Rows[0]["IsGrossPrc"]);
								gvLine.SetRowCellValue(e.RowHandle, "Currency", (!string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0]["Currency"]))) ? dataTable3.Rows[0]["Currency"] : doc.DocCurrency);
								double num36 = Function.ParseDouble(dataTable3.Rows[0]["Price"]);
								if (Function.ParseInt(byNum["UgpCode"]) == -1)
								{
									num36 *= byNum.UnitsOfMeasurment;
								}
								if (text5 == "Y")
								{
									num36 /= 1.0 + byNum.VatPrcnt / 100.0;
								}
								if (num36 != 0.0 && num4 == 0.0)
								{
									gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", num36);
									gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", dataTable3.Rows[0]["Discount"]);
									byNum["SpecPrice"] = dataTable3.Rows[0]["SpecPrice"];
								}
							}
							else if (num4 == 0.0)
							{
								gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", 0);
								gvLine.SetRowCellValue(e.RowHandle, "DiscPrcnt", 0);
								byNum["SpecPrice"] = "R";
							}
							formDocument.IsLoadPrice = false;
						}
					}
				}
				else if (e.Column.FieldName.Equals("NumPerMsr"))
				{
					if (gvLine.Columns.Cast<GridColumn>().FirstOrDefault((GridColumn t) => t.FieldName.Equals("InvQty")) != null)
					{
						if (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "UgpCode")) == -1 && Math.Abs(Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "PriceBefDi"))) > 1E-07 && !gvLine.AnyMappingField())
						{
							gvLine.SetRowCellValue(e.RowHandle, "PriceBefDi", Function.ParseDouble(e.Value) * Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "PriceBefDi")) * byNum.Quantity / ((Math.Abs(byNum.InventoryQuantity) > 1E-07) ? byNum.InventoryQuantity : 1.0));
						}
						double num37 = Function.ParseDouble(e.Value);
						gvLine.SetRowCellValue(e.RowHandle, "InvQty", num3 * num37);
					}
				}
				else if (e.Column.FieldName.Equals("UseBaseUnits"))
				{
					string text6 = Function.ToString(e.Value);
					if (gvLine.Columns.Cast<GridColumn>().FirstOrDefault((GridColumn t) => t.FieldName.Equals("NumPerMsr")) != null)
					{
						if (text6.Equals("Y"))
						{
							gvLine.Columns["NumPerMsr"].OptionsColumn.ReadOnly = true;
							gvLine.SetRowCellValue(e.RowHandle, "NumPerMsr", 1);
						}
						else
						{
							gvLine.Columns["NumPerMsr"].OptionsColumn.ReadOnly = false;
							if (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "UgpCode")) <= 0)
							{
								gvLine.SetRowCellValue(e.RowHandle, "NumPerMsr", gvLine.GetRowCellValue(e.RowHandle, (vatType == "O") ? "NumInSale" : "NumInBuy"));
								gvLine.SetRowCellValue(e.RowHandle, "unitMsr", gvLine.GetRowCellValue(e.RowHandle, (vatType == "O") ? "SalUnitMsr" : "BuyUnitMsr"));
							}
							else
							{
								DataTable baseUomByUomGroup = WinFuntions.GetBaseUomByUomGroup(Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "UgpCode")));
								gvLine.SetRowCellValue(e.RowHandle, "UomCode", baseUomByUomGroup.Rows[0]["UomCode"]);
								gvLine.SetRowCellValue(e.RowHandle, "NumPerMsr", Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(baseUomByUomGroup.Rows[0]["BaseQty"]) / ((Function.ParseDouble(baseUomByUomGroup.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(baseUomByUomGroup.Rows[0]["AltQty"])));
								gvLine.SetRowCellValue(e.RowHandle, "unitMsr", baseUomByUomGroup.Rows[0]["UomName"]);
							}
						}
					}
				}
				else if (e.Column.FieldName.Equals("UseBaseUn"))
				{
					if (Function.ToString(e.Value).Equals("Y"))
					{
						DataTable inventoryUoMByItemCode = WinFuntions.GetInventoryUoMByItemCode(Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "ItemCode")), Enums.ItemUomType.InventoryUom);
						if (inventoryUoMByItemCode != null && inventoryUoMByItemCode.Rows.Count > 0)
						{
							gvLine.SetRowCellValue(e.RowHandle, "NumPerMsr", 1);
							gvLine.SetRowCellValue(e.RowHandle, "UomCode", inventoryUoMByItemCode.Rows[0]["UomCode"]);
							gvLine.SetRowCellValue(e.RowHandle, "unitMsr", inventoryUoMByItemCode.Rows[0]["InvntryUom"]);
						}
					}
					else
					{
						gvLine.SetRowCellValue(e.RowHandle, "UomCode", (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "UgpCode")) != -1) ? gvLine.GetRowCellValue(e.RowHandle, (vatType == "O") ? "SalUnitMsr" : "BuyUnitMsr") : Function.ToString(gvLine.GetRowCellDisplayText(e.RowHandle, "UgpCode")));
						gvLine.SetRowCellValue(e.RowHandle, "NumPerMsr", gvLine.GetRowCellValue(e.RowHandle, (vatType == "O") ? "NumInSale" : "NumInBuy"));
						gvLine.SetRowCellValue(e.RowHandle, "unitMsr", gvLine.GetRowCellValue(e.RowHandle, (vatType == "O") ? "SalUnitMsr" : "BuyUnitMsr"));
					}
					gvLine.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Quantity")) * Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumPerMsr")));
					gvLine.SetRowCellValue(e.RowHandle, "OpenInvQty", Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "Quantity")) * Function.ParseDouble(gvLine.GetRowCellValue(e.RowHandle, "NumPerMsr")));
				}
				if (e.Column.FieldName.Equals("ItemCode"))
				{
					gvLine.BeginInsertRow();
					string text7 = Function.ToString(e.Value);
					if (string.IsNullOrWhiteSpace(text7))
					{
						text7 = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "ItemCode"));
					}
					if (!string.IsNullOrWhiteSpace(text7))
					{
						Enums.ItemObjectType itObjType = Enums.ItemObjectType.InventoryItem;
						string text8 = vatType;
						if (!(text8 == "O"))
						{
							if (text8 == "I")
							{
								itObjType = Enums.ItemObjectType.PurchaseItem;
							}
						}
						else
						{
							itObjType = Enums.ItemObjectType.SellItem;
						}
						string cardcode = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
						string subCatNum = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "SubCatNum"));
						DataTable dataTable4 = httpClient.Commons.ItemMaster(text7, GlobalData.DefaultBranch, itObjType, cardcode, dateTime, 1.0, subCatNum);
						if (dataTable4 != null && dataTable4.Rows.Count == 1 && !flag)
						{
							batchSerial.ClearLineDataSource();
							SetItemToDocLine(formDocument, dataTable4.Rows[0], gvLine, gvDoc.GridControl, batchSerial, docNum, e.RowHandle, _lstBomChilds, vatType, objType);
							AddNewRow(gvLine.GridControl, gvDoc, gvLine, gridLogistic, doc.DocEntry);
						}
					}
					gvLine.GridControl.DataSourceTable().AcceptChanges();
					batchSerial.SetFocusLine(isChangeItem: true);
					gvLine.ShowEditor();
				}
				if (e.Column.FieldName.Equals("WhsCode"))
				{
					string text9 = Function.ToString(gvLine.GetRowCellValue(e.RowHandle, "ItemCode"));
					string text10 = Function.ToString(e.Value);
					int focusedDataSourceRowIndex = gvLine.GetFocusedDataSourceRowIndex();
					if (string.IsNullOrEmpty(text10) || string.IsNullOrEmpty(text9))
					{
						documentLineTable.Rows[focusedDataSourceRowIndex]["QuantityOnStock"] = 0;
						documentLineTable.Rows[focusedDataSourceRowIndex]["QuantityOnWareHouse"] = 0;
						documentLineTable.Rows[focusedDataSourceRowIndex]["IsCommited"] = 0;
						documentLineTable.Rows[focusedDataSourceRowIndex]["IsCommitedInWareHouse"] = 0;
						documentLineTable.Rows[focusedDataSourceRowIndex]["OnOrder"] = 0;
						documentLineTable.Rows[focusedDataSourceRowIndex]["OnOrderInWareHouse"] = 0;
					}
					else
					{
						DataTable onstockItem = HttpClientHelperFactory.GetHttpClient().Commons.GetOnstockItem(text9, text10);
						if (onstockItem.IsNotNull())
						{
							documentLineTable.Rows[focusedDataSourceRowIndex]["QuantityOnWareHouse"] = onstockItem.Rows[0]["InWhs"];
							documentLineTable.Rows[focusedDataSourceRowIndex]["IsCommitedInWareHouse"] = onstockItem.Rows[0]["IsCommitedInWhs"];
							documentLineTable.Rows[focusedDataSourceRowIndex]["OnOrderInWareHouse"] = onstockItem.Rows[0]["OnOrderInWhs"];
						}
					}
				}
				else if (e.Column.FieldName.Equals("Currency"))
				{
					if (string.IsNullOrEmpty(byNum.Currency) || byNum.Currency.ToUpper() == GlobalData.MainCurrency.ToUpper())
					{
						byNum.RateCurrency = 1.0;
						byNum.TotalFrgn = 0.0;
						byNum.VatSumFrgn = 0.0;
					}
					else
					{
						if (byNum.Currency == doc.DocCurrency && doc.DocRate > 0.0)
						{
							byNum.RateCurrency = doc.DocRate;
						}
						else
						{
							double exchangeRate = GetExchangeRate(byNum.Currency, dateTime);
							byNum.RateCurrency = exchangeRate;
						}
						byNum.TotalFrgn = byNum.Quantity * byNum.PriceBefDi * (1.0 - byNum.DiscountPercent);
						byNum.VatSumFrgn = byNum.TotalFrgn * byNum.Rate;
						byNum.LineTotal = byNum.TotalFrgn * Math.Pow(byNum.RateCurrency, GlobalData.ExchangeRatePow);
						byNum.LineVat = byNum.VatSumFrgn * Math.Pow(byNum.RateCurrency, GlobalData.ExchangeRatePow);
					}
					if (objType == ApzObjectTypes.oPurchaseRequest && byNum.Currency != GlobalData.MainCurrency)
					{
						byNum.TotalFrgn = 0.0;
						byNum.LineVat = 0.0;
						byNum.LineTotal = 0.0;
						byNum.VatSumFrgn = 0.0;
					}
				}
				if (objType == ApzObjectTypes.oPurchaseRequest && e.Column.FieldName.Equals("LineVendor"))
				{
					byNum.SubCatNum = _httpClient.BusinessPartnerCatalogNumbersClient.GetSubCatNumByItemAndCardCode(byNum.ItemCode, Function.ToString(e.Value));
				}
				if (formulars != null && formulars.Count > 0 && tblFormular != null && tblFormular.Count > 0)
				{
					string dependTable = tblFormular.FirstOrDefault((KeyValuePair<string, GridViewEx> t) => t.Value == gvLine).Key;
					if (string.IsNullOrEmpty(dependTable))
					{
						return;
					}
					string colName = e.Column.FieldName;
					List<Formular> list = formulars.Where((Formular t) => !string.IsNullOrEmpty(t.DependField) && !t.DependField.Equals(t.FieldName) && (from x in t.DependField.Split(',')
						select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
							.Length - 1]).Contains(colName.ToUpper()) && !string.IsNullOrEmpty(t.TableName) && t.TableName.Equals(dependTable) && !string.IsNullOrEmpty(t.FormularType) && !t.FormularType.ToUpper().Equals("CBX") && !t.FormularType.ToUpper().Equals("CFL")).ToList();
					if (list.Count > 0)
					{
						int i2;
						for (i2 = 0; i2 < list.Count; i2++)
						{
							string text11 = Function.ToString(list[i2].FormularString);
							if (list[i2].FormularType.ToUpper().Equals("DB"))
							{
								List<string> paramFromSqlString = Function.GetParamFromSqlString(text11);
								if (paramFromSqlString != null && paramFromSqlString.Any())
								{
									foreach (string item2 in paramFromSqlString)
									{
										if (!string.IsNullOrEmpty(item2))
										{
											string[] array = item2.Replace("@", "").Replace(" ", "").Split('.');
											string key = list[i2].DependTable;
											string fieldName = "";
											if (array.Length > 1)
											{
												key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : list[i2].DependTable);
												fieldName = array[1];
											}
											else if (array.Length != 0)
											{
												fieldName = array[0];
											}
											GridViewEx gridViewEx = tblFormular[key];
											int rowHandle = ((!(gridViewEx.Name == gvLine.Name)) ? gridViewEx.FocusedRowHandle : e.RowHandle);
											string arg = Function.ToString(gridViewEx.GetRowCellValue(rowHandle, fieldName));
											text11 = text11.Replace(item2, $"'{arg}'");
										}
									}
								}
								new DataTable();
								DataTable data = (await _httpClient.Commons.ExecuteQuery(text11)).Data;
								if (data != null && data.Rows.Count > 0)
								{
									dynamic val = list[i2].FieldType switch
									{
										"STR" => (dynamic)Function.ToString(data.Rows[0][0]), 
										"DEC" => Function.ParseDouble(data.Rows[0][0]), 
										"DOU" => Function.ParseDouble(data.Rows[0][0]), 
										"INT" => Function.ParseInt(data.Rows[0][0]), 
										"DAT" => Function.ParseDateTime(data.Rows[0][0]), 
										"TIM" => Function.ParseDateTime(data.Rows[0][0]), 
										"DTM" => Function.ParseDateTime(data.Rows[0][0]), 
										_ => Function.ToString(data.Rows[0][0]), 
									};
									GridViewEx gridViewEx2 = tblFormular[list[i2].TableName];
									int num38 = ((!(gridViewEx2.Name == gvLine.Name)) ? gridViewEx2.FocusedRowHandle : e.RowHandle);
									if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(list[i2].TableName) && (from x in t.DependField.Split(',')
										select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
											.Length - 1]).Contains(list[i2].FieldName.ToUpper())).ToList().Count > 0)
									{
										DataTable dataTable5 = (DataTable)gridViewEx2.GridControl.DataSource;
										if (dataTable5 != null && dataTable5.Rows.Count > 0)
										{
											dataTable5.Rows[num38][list[i2].FieldName] = (object)val;
										}
									}
									else
									{
										gridViewEx2.SetRowCellValue(num38, list[i2].FieldName, val);
									}
								}
							}
							else
							{
								DataTable dataTable6 = (DataTable)gvLine.GridControl.DataSource;
								if (dataTable6.Columns.Contains("temp"))
								{
									dataTable6.Columns.Remove("temp");
								}
								if (dataTable6.Rows.Count > 0)
								{
									switch (list[i2].FieldType)
									{
									case "STR":
										dataTable6.Columns.Add("temp", typeof(string));
										break;
									case "DEC":
										dataTable6.Columns.Add("temp", typeof(decimal));
										break;
									case "DOU":
										dataTable6.Columns.Add("temp", typeof(double));
										break;
									case "INT":
										dataTable6.Columns.Add("temp", typeof(int));
										break;
									case "DAT":
										dataTable6.Columns.Add("temp", typeof(DateTime));
										break;
									case "TIM":
										dataTable6.Columns.Add("temp", typeof(DateTime));
										break;
									case "DTM":
										dataTable6.Columns.Add("temp", typeof(DateTime));
										break;
									default:
										dataTable6.Columns.Add("temp", typeof(string));
										break;
									}
									try
									{
										dataTable6.Columns["temp"].Expression = text11;
									}
									catch (Exception ex)
									{
										Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
										throw new Exception("Set formula failed:" + text11);
									}
									GridViewEx gridViewEx3 = tblFormular[list[i2].TableName];
									if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(list[i2].TableName) && (from x in t.DependField.Split(',')
										select x.ToUpper().Trim()).Contains(list[i2].FieldName.ToUpper())).ToList().Count > 0)
									{
										DataTable dataTable7 = (DataTable)gridViewEx3.GridControl.DataSource;
										if (dataTable7 != null && dataTable7.Rows.Count > 0)
										{
											dataTable7.Rows[gridViewEx3.FocusedRowHandle][list[i2].FieldName] = dataTable6.Rows[e.RowHandle]["temp"];
										}
									}
									else
									{
										gridViewEx3.SetFocusedRowCellValue(list[i2].FieldName, dataTable6.Rows[e.RowHandle]["temp"]);
									}
									if (dataTable6.Columns.Contains("temp"))
									{
										dataTable6.Columns.Remove("temp");
									}
								}
								dataTable6.AcceptChanges();
							}
						}
					}
				}
				if (e.Column.FieldName.Equals("EnSetCost") && Function.ToString(e.Value).Equals("N"))
				{
					gvLine.SetRowCellValue(e.RowHandle, "RetCost", DBNull.Value);
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				WinCommons.ErrorLineNumber(ex2);
			}
			gvLine.EndInsertRow();
		};
	}

	public static void ReloadSubTotal(GridViewEx gvLine, int rowHandle)
	{
		DataTable dataTable = gvLine.GridControl.DatasourceTable();
		if (!dataTable.IsNotNull() || ((DocumentLineTable)dataTable).AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["LineType"]) == "S") == null)
		{
			return;
		}
		int subLevel = GetSubLevel(gvLine, rowHandle);
		for (int num = rowHandle; num < gvLine.RowCount; num++)
		{
			int subLevel2 = GetSubLevel(gvLine, num);
			if (subLevel2 > 0)
			{
				if (subLevel2 == subLevel + 1)
				{
					SetSubTotal(gvLine, num, subLevel + 1);
				}
				else if (subLevel2 == subLevel)
				{
					SetSubTotal(gvLine, num, subLevel);
				}
				ReloadSubTotal(gvLine, num + 1);
				break;
			}
		}
	}

	public static int GetSubLevel(GridViewEx gvLine, int rowhandle)
	{
		int num = 0;
		if (Function.ToString(gvLine.GetRowCellValue(rowhandle, "LineType")) == "S")
		{
			num++;
			for (int num2 = rowhandle - 1; num2 >= 0; num2--)
			{
				string text = Function.ToString(gvLine.GetRowCellValue(num2, "LineType"));
				if (text == "R")
				{
					break;
				}
				if (!(text == "T"))
				{
					num++;
				}
			}
		}
		return num;
	}

	public static void SetSubTotal(GridViewEx gvLine, int rowhandle, int level)
	{
		DocumentLineTable documentLineTable = (DocumentLineTable)gvLine.GridControl.DatasourceTable();
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		for (int num5 = rowhandle - 1; num5 >= 0; num5--)
		{
			string text = Function.ToString(gvLine.GetRowCellValue(num5, "LineType"));
			int subLevel = GetSubLevel(gvLine, num5);
			if (text == "S" && subLevel >= level)
			{
				break;
			}
			if (subLevel == level - 1)
			{
				num += Function.ParseDouble(gvLine.GetRowCellValue(num5, "LineTotal"));
				num2 += Function.ParseDouble(gvLine.GetRowCellValue(num5, "TotalFrgn"));
				num3 += Function.ParseDouble(gvLine.GetRowCellValue(num5, "GTotal"));
				num4 += Function.ParseDouble(gvLine.GetRowCellValue(num5, "GTotalFC"));
			}
		}
		documentLineTable[gvLine.GetDataSourceRowIndex(rowhandle)]["LineTotal"] = num;
		documentLineTable[gvLine.GetDataSourceRowIndex(rowhandle)]["TotalFrgn"] = num2;
		documentLineTable[gvLine.GetDataSourceRowIndex(rowhandle)]["GTotal"] = num3;
		documentLineTable[gvLine.GetDataSourceRowIndex(rowhandle)]["GTotalFC"] = num4;
	}

	public static void SetExpnsToLine(this GridViewEx gv, DataTable dtExpn, int expnIndex, int rowhandle)
	{
		try
		{
			bool flag = dtExpn.IsNotNull();
			DataTable dataTable = gv.GridControl.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				int dataSourceRowIndex = gv.GetDataSourceRowIndex(rowhandle);
				dataTable.Rows[dataSourceRowIndex]["VatGroup_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["VatGroup"]) : "");
				dataTable.Rows[dataSourceRowIndex]["VatPrcnt_F" + expnIndex] = (flag ? Function.ParseDouble(dtExpn.Rows[0]["VatRate"]) : 0.0);
				dataTable.Rows[dataSourceRowIndex]["VatSum_F" + expnIndex] = 0;
				dataTable.Rows[dataSourceRowIndex]["IsAcquistn_F" + expnIndex] = "N";
				dataTable.Rows[dataSourceRowIndex]["OcrCode_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["OcrCode"]) : "");
				dataTable.Rows[dataSourceRowIndex]["OcrCode2_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["OcrCode2"]) : "");
				dataTable.Rows[dataSourceRowIndex]["OcrCode3_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["OcrCode3"]) : "");
				dataTable.Rows[dataSourceRowIndex]["OcrCode4_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["OcrCode4"]) : "");
				dataTable.Rows[dataSourceRowIndex]["OcrCode5_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["OcrCode5"]) : "");
				dataTable.Rows[dataSourceRowIndex]["Project_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["Project"]) : "");
				dataTable.Rows[dataSourceRowIndex]["LstPchPrce_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["LstPchPrce"]) : "N");
				dataTable.Rows[dataSourceRowIndex]["Stock_F" + expnIndex] = (flag ? Function.ToString(dtExpn.Rows[0]["Stock"]) : "N");
				dataTable.Rows[dataSourceRowIndex]["LineTotal_F" + expnIndex] = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void AddNewRow(GridControl grdControl, GridViewEx gvDoc, GridViewEx gvLine, GridControl grdLogistic = null, int newDocEntry = 0)
	{
		try
		{
			if (grdControl.Name == gvDoc.GridControl.Name)
			{
				DocumentsImpl documentsImpl = new DocumentsImpl();
				int docEntry = (documentsImpl.DocNum = newDocEntry);
				documentsImpl.DocEntry = docEntry;
				CopyDocToListDoc(documentsImpl, isDuplicate: false, isAddNew: true, newDocEntry, (DocumentLineTable)grdControl.DataSource, gvDoc, grdLogistic, gvLine.GridControl);
				return;
			}
			DocumentLineTable documentLineTable = (DocumentLineTable)grdControl.DataSource;
			try
			{
				documentLineTable.Remove(documentLineTable.GetEnumerable().FirstOrDefault((DocumentLineRow t) => string.IsNullOrEmpty(t.ItemCode) && t.LineType == "R"));
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			if (documentLineTable == null)
			{
				documentLineTable = DocumentsHelper.Lines.GetClone();
			}
			DocumentLineRow newRow = documentLineTable.GetNewRow();
			newRow.LineType = "R";
			newRow.DocEntry = newDocEntry;
			newRow.LineNum = -1;
			string itemCode = (newRow.ItemDescription = string.Empty);
			newRow.ItemCode = itemCode;
			newRow.Quantity = 0.0;
			DocumentTable dt = gvDoc.GridControl.DataSource as DocumentTable;
			DocumentRow doc = dt.GetByEntry(newDocEntry);
			if (doc != null && doc.DocCurrency != null)
			{
				if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper() == doc.DocCurrency.ToUpper()))
				{
					newRow.Currency = doc.DocCurrency;
				}
				else
				{
					newRow.Currency = GlobalData.MainCurrency;
				}
			}
			else
			{
				newRow.Currency = GlobalData.MainCurrency;
			}
			newRow.BaseType = -1;
			newRow.UnitsOfMeasurment = 1.0;
			newRow.UseBaseUnits = "N";
			newRow.InventoryQuantity = 0.0;
			documentLineTable.Add(newRow);
			documentLineTable.AcceptChanges();
			gvLine.GridControl.DataSource = documentLineTable;
			gvLine.GridControl.RefreshDataSource();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public static void ReloadDocumentVisOrderAndLineNum(DocumentLineTable lines)
	{
		int num = -1;
		for (int i = 0; i < lines.Rows.Count; i++)
		{
			if (lines[i].LineType.Equals("R") || string.IsNullOrEmpty(lines[i].LineType))
			{
				num = (lines[i].VisOrder = num + 1);
				continue;
			}
			lines[i].LineNum = num;
			lines[i].VisOrder = num;
		}
		lines.AcceptChanges();
	}

	public static void SetItemToDocLine(VirFormDocument formDocument, DataRow r, GridViewEx gvLine, GridControl grdDoc, ucBatchAndSerial batchSerial, int docNum, int rowHandle = -1, List<Common.Table_ID_Code> _lstBomChilds = null, string vatType = "O", ApzObjectTypes objTypes = ApzObjectTypes.oItems)
	{
		isSetItemDocline = true;
		try
		{
			DocumentTable dt = (DocumentTable)grdDoc.DataSource;
			DocumentRow doc = dt.GetByNum(docNum);
			int num = Function.ParseInt(doc.SalesPersonCode);
			DocumentLineTable documentLineTable = (DocumentLineTable)gvLine.GridControl.DataSource;
			int currentLine = Function.ParseInt(gvLine.GetRowCellValue(rowHandle, "LineNum"));
			WinFuntions.DeleteBomChildsItem(gvLine, rowHandle, batchSerial, isChooseItem: true);
			gvLine.GridControl.DataSource = documentLineTable;
			gvLine.GridControl.RefreshDataSource();
			DocumentLineRow line = documentLineTable.GetNewRow();
			line.LineNum = documentLineTable.GetNewLineNum(currentLine);
			documentLineTable.Insert(rowHandle, line);
			line.DocEntry = doc.DocEntry;
			bool fromExcel = line.FromExcel;
			if (string.IsNullOrEmpty(line.ItemCode) && !fromExcel)
			{
				line.ItemCode = Function.ToString(r["ItemCode"]);
			}
			if (string.IsNullOrEmpty(line.ItemDescription))
			{
				line.ItemDescription = Function.ToString(r["ItemName"]);
			}
			if (Function.ParseDouble(line.Quantity).Equals(0.0) && !fromExcel)
			{
				line.Quantity = 1.0;
			}
			string text = "N";
			if (r.IsExistsColumn("IsGrossPrc"))
			{
				text = Function.ToString(r["IsGrossPrc"]);
			}
			line.RemainingOpenQuantity = line.Quantity;
			if (string.IsNullOrEmpty(Function.ToString(line.WarehouseCode)))
			{
				line.WarehouseCode = Function.ToString(r["DefaultWarehouse"]);
			}
			if (objTypes == ApzObjectTypes.oPurchaseRequest)
			{
				line.PQTReqDate = doc.ReqDate;
			}
			else
			{
				line.ShipDate = doc.DocDueDate;
			}
			DataTable vatByCardCode = GetVatByCardCode(doc.CardCode);
			bool flag = true;
			if (vatByCardCode != null && vatByCardCode.Rows.Count > 0)
			{
				object obj = vatByCardCode.Rows[0]["VatGroup"];
				object obj2 = vatByCardCode.Rows[0]["Rate"];
				if (obj == null || obj == DBNull.Value || string.IsNullOrWhiteSpace(obj.ToString()))
				{
					flag = false;
				}
				else
				{
					line.Rate = Function.ParseDouble(obj2);
					line.TaxPercentagePerRow = Function.ParseDouble(obj2);
					line.VatGroup = obj.ToString();
				}
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				if (vatType == "O")
				{
					line.VatGroup = Function.ToString(r["SalesVATGroup"]) ?? "";
					line.Rate = Function.ParseDouble(r["RateSa"]);
					line.TaxPercentagePerRow = Function.ParseDouble(r["RateSa"]);
				}
				else
				{
					line.VatGroup = Function.ToString(r["VatGroupPu"]) ?? "";
					line.Rate = Function.ParseDouble(r["RatePu"]);
					line.TaxPercentagePerRow = Function.ParseDouble(r["RatePu"]);
				}
			}
			line.LineType = "R";
			line.ItemType = 4;
			line.LineVat = 0.0;
			line.LineStatusName = "O";
			line.UseBaseUnits = "N";
			line["SalPackUn"] = r["SalPackUn"];
			line["PurPackUn"] = r["PurPackUn"];
			if (vatType == "I")
			{
				line.UoMCode = Function.ToString(r["PUomCode"]);
				line.UoMName = Function.ToString(r["BuyUnitMsr"]);
				line.UnitsOfMeasurment = Function.ParseDouble(r["NumInBuy"]);
				line["OpenInvQty"] = line.UnitsOfMeasurment * line.Quantity;
				line["InvQty"] = line.UnitsOfMeasurment * line.Quantity;
				double num2 = Function.ParseDouble(line["PurPackUn"]);
				if (Math.Abs(num2) < 1E-07)
				{
					num2 = 1.0;
				}
				double num3 = line.Quantity / num2;
				double num4 = Math.Truncate(num3);
				line["PackQty"] = ((num3 - num4 > 1E-07) ? (num4 + 1.0) : num4);
			}
			else if (vatType == "O")
			{
				line.UoMCode = Function.ToString(r["SUomCode"]);
				line.UoMName = Function.ToString(r["SalUnitMsr"]);
				line.UnitsOfMeasurment = Function.ParseDouble(r["NumInSale"]);
				line["OpenInvQty"] = line.UnitsOfMeasurment * line.Quantity;
				line["InvQty"] = line.UnitsOfMeasurment * line.Quantity;
				double num5 = Function.ParseDouble(line["SalPackUn"]);
				if (Math.Abs(num5) < 1E-07)
				{
					num5 = 1.0;
				}
				double num6 = line.Quantity / num5;
				double num7 = Math.Truncate(num6);
				line["PackQty"] = ((num6 - num7 > 1E-07) ? (num7 + 1.0) : num7);
			}
			else
			{
				line.UseBaseUnits = "Y";
				line.UoMCode = Function.ToString(r["IUomCode"]);
				line.UoMName = Function.ToString(r["InvntryUom"]);
				line.UnitsOfMeasurment = 1.0;
				line["OpenInvQty"] = line.Quantity;
				line["InvQty"] = line.Quantity;
			}
			line.InventoryQuantity = line.Quantity * line.UnitsOfMeasurment;
			line["UgpCode"] = Function.ParseInt(r["UgpCode"]);
			double num8 = 0.0;
			string text2 = "";
			double discountPercent = 0.0;
			string value = "R";
			DataTable dataTable = _httpClient.Commons.DocumentPrice(line.ItemCode, doc.CardCode, doc.DocDate, line.Quantity, line.UoMCode, Function.ParseInt(doc["InfoPriceList"]));
			if (dataTable.IsNotNull())
			{
				num8 = Function.ParseDouble(dataTable.Rows[0]["Price"]);
				text2 = Function.ToString(dataTable.Rows[0]["Currency"]);
				discountPercent = Function.ParseDouble(dataTable.Rows[0]["Discount"]);
				value = Function.ToString(dataTable.Rows[0]["SpecPrice"]);
				if (Function.ParseInt(line["UgpCode"]) == -1)
				{
					num8 *= line.UnitsOfMeasurment;
				}
			}
			line["SpecPrice"] = value;
			line.DiscountPercent = discountPercent;
			if (!string.IsNullOrEmpty(text2))
			{
				line.Currency = text2;
				line.RateCurrency = ((text2 == doc.DocCurrency && doc.DocRate > 0.0) ? doc.DocRate : GetExchangeRate(text2, Function.ParseDateTimes(doc.DocDate)));
			}
			else if (!string.IsNullOrEmpty(doc.DocCurrency))
			{
				line.Currency = doc.DocCurrency;
				line.RateCurrency = ((doc.DocRate > 0.0) ? doc.DocRate : GetExchangeRate(doc.DocCurrency, Function.ParseDateTimes(doc.DocDate)));
			}
			else
			{
				line.Currency = GlobalData.MainCurrency;
				line.RateCurrency = 1.0;
			}
			int num9 = GlobalData.GeneralSetting.PriceDec.Value;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(line.Currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num9 = ((currencyCodesImpl.Decimals == -1) ? num9 : currencyCodesImpl.Decimals);
				}
			}
			int value2 = GlobalData.GeneralSetting.SumDec.Value;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency));
				if (currencyCodesImpl2 != null)
				{
					value2 = ((currencyCodesImpl2.Decimals == -1) ? value2 : currencyCodesImpl2.Decimals);
				}
			}
			if (text == "Y")
			{
				line.PriceBefDi = Math.Round(num8 / (1.0 + line.VatPrcnt / 100.0), num9, MidpointRounding.AwayFromZero);
			}
			else
			{
				line.PriceBefDi = Math.Round(num8, num9, MidpointRounding.AwayFromZero);
			}
			line.Price = Math.Round(line.PriceBefDi - line.PriceBefDi * line.DiscountPercent / 100.0, num9, MidpointRounding.AwayFromZero);
			if (Function.ToString(r["BOMTreeType"]) == "S" || Function.ToString(r["BOMTreeType"]) == "T")
			{
				line.BOMTreeType = Function.ToString(r["BOMTreeType"]);
				line.TreeType = Function.ToString(r["BOMTreeType"]);
				DataTable dataTable2 = _httpClient.BillOfMasterials.SearchADoc(Function.ToString(r["ItemCode"]));
				if (dataTable2.IsNotNull())
				{
					string text3 = Function.ToString(dataTable2.Rows[0]["OcrCode"]);
					if (!string.IsNullOrEmpty(text3))
					{
						line.OcrCode = text3;
					}
					string text4 = Function.ToString(dataTable2.Rows[0]["OcrCode2"]);
					if (!string.IsNullOrEmpty(text4))
					{
						line.OcrCode2 = text4;
					}
					string text5 = Function.ToString(dataTable2.Rows[0]["OcrCode3"]);
					if (!string.IsNullOrEmpty(text5))
					{
						line.OcrCode3 = text5;
					}
					string text6 = Function.ToString(dataTable2.Rows[0]["OcrCode4"]);
					if (!string.IsNullOrEmpty(text6))
					{
						line.OcrCode4 = text6;
					}
					string text7 = Function.ToString(dataTable2.Rows[0]["OcrCode5"]);
					if (!string.IsNullOrEmpty(text7))
					{
						line.OcrCode5 = text7;
					}
					string text8 = Function.ToString(dataTable2.Rows[0]["Project"]);
					if (!string.IsNullOrEmpty(text8))
					{
						line.Project = text8;
					}
				}
				double num10 = Function.ParseDouble(dataTable2.Rows[0]["Qauntity"]);
				DataTable dataTable3 = new DataTable();
				dataTable3 = WinFuntions.GetChildItemsInBom(Function.ToString(r["ItemCode"]), _lstBomChilds);
				int num11 = rowHandle;
				num11++;
				if (dataTable3 != null && dataTable3.Rows.Count > 0)
				{
					foreach (DataRow bomitem in dataTable3.Rows)
					{
						DocumentLineRow newRow = documentLineTable.GetNewRow();
						newRow.LineType = "R";
						((IEnumerable<DocumentLineRow>)documentLineTable.GetEnumerable()).Max((Func<DocumentLineRow, int?>)((DocumentLineRow x) => x.LineNum));
						int newLineNum = documentLineTable.GetNewLineNum(-1);
						if (!_lstBomChilds.Exists((Common.Table_ID_Code x) => x.Name == line.LineNum.ToString() && x.Code == Function.ToString(bomitem["ItemCode"])))
						{
							Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
							table_ID_Code.ID = Function.ParseDouble(bomitem["OnHand"]);
							table_ID_Code.Code = Function.ToString(bomitem["ItemCode"]);
							table_ID_Code.Name = line.LineNum.ToString();
							_lstBomChilds.Add(table_ID_Code);
						}
						if (Function.ToString(r["BOMTreeType"]) == "S")
						{
							newRow.BOMTreeType = line.LineNum.ToString();
							newRow.RateBomSales = Function.ParseDecimal(bomitem["OnHand"]);
							newRow.TreeType = "I";
						}
						else
						{
							newRow.BOMTreeType = "-1";
							newRow.TreeType = "N";
						}
						if (string.IsNullOrEmpty(Function.ToString(bomitem["Currency"])))
						{
							newRow.Currency = doc.DocCurrency;
							newRow.RateCurrency = doc.DocRate;
						}
						else
						{
							newRow.Currency = Function.ToString(bomitem["Currency"]);
							newRow.RateCurrency = GetExchangeRate(newRow.Currency, Function.ParseDateTimes(doc.DocDate));
						}
						newRow["SalUnitMsr"] = Function.ToString(bomitem["SalUnitMsr"]);
						newRow["NumInSale"] = Function.ParseDouble(bomitem["NumInSale"]);
						newRow["BuyUnitMsr"] = Function.ToString(r["BuyUnitMsr"]);
						newRow["NumInBuy"] = Function.ParseDouble(r["NumInBuy"]);
						newRow.UnitsOfMeasurment = 1.0;
						newRow["UgpCode"] = Function.ParseInt(bomitem["UgpCode"]);
						newRow.UseBaseUnits = "Y";
						newRow.UoMName = Function.ToString(bomitem["InvntryUom"]);
						newRow["InvUnitMsr"] = Function.ToString(bomitem["InvntryUom"]);
						newRow["NumInInven"] = Function.ParseDouble(bomitem["NumInInven"]);
						newRow["OpenInvQty"] = 1;
						newRow["InvQty"] = 1;
						newRow["SlpCode"] = num;
						newRow["OcrCode"] = bomitem["OcrCode"];
						newRow["OcrCode2"] = bomitem["OcrCode2"];
						newRow["OcrCode3"] = bomitem["OcrCode3"];
						newRow["OcrCode4"] = bomitem["OcrCode4"];
						newRow["OcrCode5"] = bomitem["OcrCode5"];
						newRow["FreeTxt"] = bomitem["Comment"];
						if (newRow.IsExistsColumn("ManBatchSerial"))
						{
							newRow["ManBatchSerial"] = "N";
						}
						if (Function.ParseInt(newRow["UgpCode"]) <= 0)
						{
							newRow.UoMCode = newRow.UoMName;
						}
						else
						{
							newRow.UoMCode = WinFuntions.GetUoMCodeByName(Function.ParseInt(newRow["UgpCode"]), newRow.UoMName);
						}
						newRow["SalPackUn"] = bomitem["SalPackUn"];
						newRow["PurPackUn"] = bomitem["PurPackUn"];
						double num12 = Function.ParseDouble(bomitem["OnHand"]);
						newRow.Quantity = num12 / num10 * line.Quantity;
						documentLineTable.Rows.InsertAt(newRow, num11);
						documentLineTable.AcceptChanges();
						gvLine.GridControl.DataSource = documentLineTable;
						gvLine.GridControl.RefreshDataSource();
						gvLine.BeginUpdatingData();
						gvLine.SetRowCellValue(num11, "LineNum", newLineNum);
						gvLine.SetRowCellValue(num11, "DocEntry", doc.DocEntry);
						gvLine.SetRowCellValue(num11, "ItemCode", Function.ToString(bomitem["ItemCode"]));
						gvLine.SetRowCellValue(num11, "Dscription", Function.ToString(bomitem["ItemName"]));
						gvLine.SetRowCellValue(num11, "WhsCode", Function.ToString(bomitem["DefaultWarehouse"]));
						gvLine.SetRowCellValue(num11, "ShipDate", doc.DocDueDate);
						gvLine.SetRowCellValue(num11, "UnitPrice", Function.ParseDouble(bomitem["Price"]));
						gvLine.SetRowCellValue(num11, "PriceBefDi", Function.ParseDouble(bomitem["Price"]));
						gvLine.SetRowCellValue(num11, "VatGroup", Function.ToString(bomitem["SalesVATGroup"]));
						gvLine.SetRowCellValue(num11, "TaxCode", Function.ToString(bomitem["SalesVATGroup"]));
						gvLine.SetRowCellValue(num11, "VatPrcnt", Function.ParseDouble(bomitem["RateSa"]));
						gvLine.SetRowCellValue(num11, "OpenQty", 1);
						gvLine.SetRowCellValue(num11, "LineStatus", "O");
						gvLine.SetRowCellValue(num11, "Project", Function.ToString(bomitem["Project"]));
						gvLine.SetRowCellValue(num11, "WhsCode", Function.ToString(bomitem["Warehouse"]));
						WinFuntions.SetUDFFieldValueBOMItemToDoc(line.ItemCode, Function.ParseInt(bomitem["ChildNum"]), gvLine, num11);
						gvLine.EndUpdatingData();
						gvLine.FocusedRowHandle = num11;
						num11++;
					}
				}
			}
			double num13 = ApzonCommon.LineTotal(line.Quantity, line.PriceBefDi, line.DiscountPercent);
			double num14 = ApzonCommon.LineTotal(line.Quantity, num8, line.DiscountPercent);
			if (line.RateCurrency == 0.0)
			{
				line.LineTotal = num13 * 1.0;
				line.GTotal = num14;
			}
			else
			{
				line.LineTotal = num13 * Function.ParseDouble(line.RateCurrency);
				line.GTotal = num14 * Function.ParseDouble(line.RateCurrency);
			}
			double num15 = line.LineTotal * line.Rate;
			if (line.Rate >= 1.0)
			{
				num15 = line.LineTotal * line.Rate / 100.0;
			}
			line.LineVat = Function.ParseDouble(line.RateCurrency) * Function.ParseDouble(num15);
			line["SalUnitMsr"] = Function.ToString(r["SalUnitMsr"]);
			line["NumInSale"] = Function.ParseDouble(r["NumInSale"]);
			line["BuyUnitMsr"] = Function.ToString(r["BuyUnitMsr"]);
			line["NumInBuy"] = Function.ParseDouble(r["NumInBuy"]);
			line["InvUnitMsr"] = Function.ToString(r["InvntryUom"]);
			line["NumInInven"] = Function.ParseDouble(r["NumInInven"]);
			if (line.IsExistsColumn("ManBatchSerial") && r.IsExistsColumn("ManBatchSerial"))
			{
				line["ManBatchSerial"] = Function.ToString(r["ManBatchSerial"]);
			}
			if (string.IsNullOrEmpty(Function.ToString(line["SlpCode"])) || Function.ToString(line["SlpCode"]).Equals("-1"))
			{
				line["SlpCode"] = num;
			}
			line["SubCatNum"] = Function.ToString(r["SubCatNum"]);
			line["QuantityOnStock"] = r["OnStock"];
			line["QuantityOnWareHouse"] = r["OnWhs"];
			line["IsCommited"] = r["IsCommited"];
			line["IsCommitedInWareHouse"] = r["IsCommitedInWhs"];
			line["OnOrder"] = r["OnOrder"];
			line["OnOrderInWareHouse"] = r["OnOrderInWhs"];
			line["LineVendor"] = r["CardCode"];
			formDocument.LoadDefaultValuePerDocument(ref line);
			ReloadDocumentVisOrderAndLineNum(documentLineTable);
			gvLine.GridControl.DataSource = documentLineTable;
			gvLine.GridControl.RefreshDataSource();
			gvLine.FocusedRowHandle = rowHandle;
			gvLine.SetFocusedRowCellValue("Quantity", line.Quantity);
			int num16 = GlobalData.GeneralSetting.SumDec.Value;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl3 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(doc.DocCur.ToUpper()));
				if (currencyCodesImpl3 != null)
				{
					num16 = ((currencyCodesImpl3.Decimals == -1) ? num16 : currencyCodesImpl3.Decimals);
				}
			}
			SetDocTotal(grdDoc.DefaultView as GridViewEx, gvLine.GridControl, num16, objTypes);
			grdDoc.RefreshDataSource();
			gvLine.ShowEditor();
			gvLine.GridControl.DataSourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message + WinFuntions.GetExceptionLineNumber(ex));
			WinCommons.ErrorLineNumber(ex);
		}
		isSetItemDocline = false;
	}

	public static void CopyDocToListDoc(DocumentsImpl docCurrent, bool isDuplicate, bool isAddNew, int newDocEntry, DocumentLineTable linesEdit, GridViewEx gvDoc, GridControl grdLogistic, GridControl grdLine)
	{
		if (docCurrent == null)
		{
			return;
		}
		DocumentTable documentTable = (DocumentTable)gvDoc.GridControl.DataSource;
		DocumentRow newRow = documentTable.GetNewRow();
		newRow.DocEntry = docCurrent.DocEntry;
		newRow.DocNum = docCurrent.DocEntry;
		newRow.DocDate = DateTime.Now;
		newRow.DocDueDate = DateTime.Now;
		newRow.TaxDate = DateTime.Now;
		newRow.ObjType = Convert.ToInt32(ApzObjectTypes.oInvoices);
		newRow.ObjTypeFrom = docCurrent.ObjTypeFrom;
		newRow.TypeFrom = (int)docCurrent.TypeFrom;
		newRow.DocTotal = docCurrent.DocTotal;
		newRow.TotalBeforeDis = docCurrent.TotalBeforeDis;
		newRow.DiscountPercent = docCurrent.DiscPrcnt;
		newRow.VatSum = docCurrent.VatSum;
		newRow.Comments = docCurrent.Comments;
		newRow.DocCurrency = docCurrent.DocCur;
		newRow.CardCode = docCurrent.CardCode;
		newRow.CardName = docCurrent.CardName;
		newRow.DocCurrency = GlobalData.MainCurrency;
		newRow.DocumentStatus = "O";
		newRow.DocStatusName = "Open";
		newRow.SalesPersonCode = docCurrent.SlpCode;
		newRow.ContactPersonCode = docCurrent.CntctCode;
		documentTable.Clear();
		if (isAddNew)
		{
			int num = ((documentTable.Rows.Count == 0) ? 1 : (documentTable.MaxEntry() + 1));
			newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
			newRow.DocEntry = newDocEntry;
			newRow.DocNum = newDocEntry;
			newRow.CardCode = string.Empty;
			newRow.CardName = string.Empty;
			newRow.ContactPersonCode = -1;
			newRow.DocCurrency = GlobalData.MainCurrency;
			newRow.DocRate = 1.0;
			newRow.DocStatusName = "Open";
			newRow.DocumentStatus = "O";
			newRow.SalesPersonCode = -1;
		}
		else if (isDuplicate)
		{
			documentTable.Remove(documentTable.GetByEntry(newDocEntry));
			newRow.DocEntry = newDocEntry;
			newRow.DocNum = newDocEntry;
			foreach (DocumentLineRow row in linesEdit.Rows)
			{
				row.DocEntry = newDocEntry;
			}
		}
		documentTable.Rows.InsertAt(newRow, 0);
		BindDataToDoc(documentTable, linesEdit, gvDoc, grdLogistic, grdLine);
	}

	public static void BindDataToDoc(DocumentTable lst, DocumentLineTable linesEdit, GridViewEx gvDoc, GridControl grdLogistic = null, GridControl grdLine = null)
	{
		if (grdLogistic != null)
		{
			grdLogistic.DataSource = lst;
			grdLogistic.RefreshDataSource();
		}
		gvDoc.GridControl.DataSource = lst;
		gvDoc.GridControl.RefreshDataSource();
		if (grdLine != null)
		{
			grdLine.DataSource = linesEdit;
			grdLine.RefreshDataSource();
		}
		gvDoc.FocusedColumn = gvDoc.Columns.FirstOrDefault((GridColumn t) => t.FieldName.Equals("CardName"));
		gvDoc.FocusedRowHandle = 0;
	}

	public static void ChangeChildBOMItemsQuantity(GridViewEx gvLine, List<Common.Table_ID_Code> lstBomChilds, decimal pQty, string pBType, int lineNum)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(pBType) || string.IsNullOrEmpty(pBType) || !pBType.Equals("S"))
			{
				return;
			}
			DocumentLineTable documentLineTable = gvLine.GridControl.DataSource as DocumentLineTable;
			if (!documentLineTable.IsNotNull())
			{
				return;
			}
			List<DataRow> list = (from t in documentLineTable.AsEnumerable()
				where Function.ToString(t["BOMTreeType"]) != "" && Function.ToString(t["BOMTreeType"]).ToUpper() != "S" && Function.ToString(t["BOMTreeType"]).ToUpper() != "I" && Function.ParseInt(t["BOMTreeType"]) == lineNum
				select t).ToList();
			decimal num = 1m;
			if (gvLine.ActiveEditor != null)
			{
				num = Function.ParseDecimal(gvLine.ActiveEditor.OldEditValue);
			}
			if (num == 0m)
			{
				num = 1m;
			}
			foreach (DocumentLineRow item in list)
			{
				decimal num2 = pQty * item.RateBomSales;
				gvLine.SetRowCellValue(gvLine.GetRowHandle(documentLineTable.Rows.IndexOf(item)), "Quantity", num2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task SaveAsDraft(GridViewEx gv_Doc, GridViewEx gv_Lines, ApzObjectTypes apzObjectTypes)
	{
		try
		{
			int docNum = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocEntry"));
			DocumentTable documentTable = gv_Doc.GridControl.DataSource as DocumentTable;
			DocumentRow documentRow = documentTable.GetEnumerable().SingleOrDefault((DocumentRow x) => x.DocEntry == docNum);
			DocumentLineTable dt = gv_Lines.GridControl.DataSource as DocumentLineTable;
			List<Document_LinesImpl> lines = dt.ToDocumentLine();
			lines.RemoveAll((Document_LinesImpl x) => string.IsNullOrEmpty(x.ItemCode));
			new DocumentsImpl();
			Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
			if (documentTable != null && documentTable.Count > 0)
			{
				documentRow = documentTable.GetEnumerable().SingleOrDefault((DocumentRow x) => x.DocEntry == docNum);
			}
			if (documentRow == null)
			{
				return;
			}
			DocumentsImpl documentsImpl = documentRow.ToDocument();
			if (apzObjectTypes == ApzObjectTypes.oInvoices || apzObjectTypes == ApzObjectTypes.oPurchaseInvoices)
			{
				documentsImpl.IsRIV = "Y";
			}
			else
			{
				documentsImpl.IsRIV = "N";
			}
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oAPReserveInvoice:
				documentsImpl.ObjType = 18;
				break;
			case ApzObjectTypes.oInvoices:
				documentsImpl.ObjType = 13;
				break;
			default:
				documentsImpl.ObjType = (int)apzObjectTypes;
				break;
			}
			documentsImpl.JrnlMemo = apzObjectTypes.ToString() + " - " + documentsImpl.CardCode;
			documentsImpl.ObjType = (int)apzObjectTypes;
			for (int num = 0; num < lines.Count; num++)
			{
				lines[num].LineNum = num + 1;
				lines[num].ObjType = documentsImpl.ObjType;
				if (apzObjectTypes == ApzObjectTypes.oDrafts)
				{
					lines[num].PQTReqDate = document_LinesImpl.PQTReqDate;
				}
			}
			documentsImpl.Lines = lines;
			WinCommons._OpenProcessing();
			if (string.IsNullOrWhiteSpace(await _docClientHelper.PostParamsAsJsonAsync("SaveAsDraft", documentsImpl)))
			{
				WinCommons.Success(Enums.MessageCode.SaveDraftSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SaveError, "");
			}
			WinCommons._CloseProcessing();
			lines.Clear();
			gv_Lines.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static void ShowHideSumColumn(GridViewEx gvLine, string docCurrency, GridViewEx gvFreight = null)
	{
		try
		{
			List<string> list = new List<string>();
			if (GlobalData.FormSettings.ContainsKey(gvLine.FormId) && GlobalData.FormSettings[gvLine.FormId].Tables.Contains(gvLine.ItemId))
			{
				list = (from t in GlobalData.FormSettings[gvLine.FormId].Tables[gvLine.ItemId].AsEnumerable()
					where Function.ParseBool(t["Visible"])
					select Function.ToString(t["ColumnId"])).ToList();
			}
			if (string.IsNullOrEmpty(docCurrency) || docCurrency.ToUpper().Equals(GlobalData.MainCurrency.ToUpper()))
			{
				gvLine.Columns["LineTotal"].Visible = gvLine.Columns["TotalFrgn"].Visible || list.Contains("LineTotal");
				gvLine.Columns["LineVat"].Visible = gvLine.Columns["VatSumFrgn"].Visible || list.Contains("LineVat");
				if (gvLine.Columns["VatSumFrgn"].Visible)
				{
					gvLine.Columns["LineVat"].VisibleIndex = gvLine.Columns["VatSumFrgn"].VisibleIndex;
				}
				if (gvLine.Columns["TotalFrgn"].Visible)
				{
					gvLine.Columns["LineTotal"].VisibleIndex = gvLine.Columns["TotalFrgn"].VisibleIndex;
				}
				gvLine.Columns["TotalFrgn"].Visible = false;
				gvLine.Columns["VatSumFrgn"].Visible = false;
				if (gvFreight != null)
				{
					gvFreight.Columns["LineTotal"].Visible = true;
					gvFreight.Columns["VatSum"].Visible = true;
					gvFreight.Columns["GrsAmount"].Visible = true;
					gvFreight.Columns["VatSum"].VisibleIndex = gvFreight.Columns["VatGroup"].VisibleIndex + 1;
					gvFreight.Columns["LineTotal"].VisibleIndex = gvFreight.Columns["VatPrcnt"].VisibleIndex + 1;
					gvFreight.Columns["GrsAmount"].VisibleIndex = gvFreight.Columns["Status"].VisibleIndex + 1;
					gvFreight.Columns["TotalFrgn"].Visible = false;
					gvFreight.Columns["VatSumFrgn"].Visible = false;
					gvFreight.Columns["GrsFC"].Visible = false;
				}
				for (int num = 1; num < 4; num++)
				{
					gvLine.Columns["LineTotal_F" + num].Visible = list.Contains("LineTotal_F" + num);
					gvLine.Columns["VatSum_F" + num].Visible = list.Contains("VatSum_F" + num);
					gvLine.Columns["VatSum_F" + num].VisibleIndex = (list.Contains("VatSum_F" + num) ? (gvLine.Columns["VatGroup_F" + num].VisibleIndex + 1) : (-1));
					gvLine.Columns["LineTotal_F" + num].VisibleIndex = (list.Contains("LineTotal_F" + num) ? (gvLine.Columns["ExpnsCode_F" + num].VisibleIndex + 1) : (-1));
					gvLine.Columns["TotalFrgn_F" + num].Visible = false;
					gvLine.Columns["VatSumFrgn_F" + num].Visible = false;
				}
			}
			else
			{
				gvLine.Columns["TotalFrgn"].Visible = gvLine.Columns["LineTotal"].Visible || list.Contains("TotalFrgn");
				gvLine.Columns["VatSumFrgn"].Visible = gvLine.Columns["LineVat"].Visible || list.Contains("VatSumFrgn");
				if (gvLine.Columns["LineTotal"].Visible)
				{
					gvLine.Columns["TotalFrgn"].VisibleIndex = gvLine.Columns["LineTotal"].VisibleIndex;
				}
				if (gvLine.Columns["LineVat"].Visible)
				{
					gvLine.Columns["VatSumFrgn"].VisibleIndex = gvLine.Columns["LineVat"].VisibleIndex;
				}
				gvLine.Columns["LineTotal"].Visible = false;
				gvLine.Columns["LineVat"].Visible = false;
				if (gvFreight != null)
				{
					gvFreight.Columns["LineTotal"].Visible = false;
					gvFreight.Columns["VatSum"].Visible = false;
					gvFreight.Columns["GrsAmount"].Visible = false;
					gvFreight.Columns["TotalFrgn"].Visible = true;
					gvFreight.Columns["VatSumFrgn"].Visible = true;
					gvFreight.Columns["GrsFC"].Visible = true;
					gvFreight.Columns["VatSumFrgn"].VisibleIndex = gvFreight.Columns["VatGroup"].VisibleIndex + 1;
					gvFreight.Columns["TotalFrgn"].VisibleIndex = gvFreight.Columns["VatPrcnt"].VisibleIndex + 1;
					gvFreight.Columns["GrsFC"].VisibleIndex = gvFreight.Columns["Status"].VisibleIndex + 1;
				}
				for (int num2 = 1; num2 < 4; num2++)
				{
					gvLine.Columns["TotalFrgn_F" + num2].Visible = list.Contains("TotalFrgn_F" + num2);
					gvLine.Columns["VatSumFrgn_F" + num2].Visible = list.Contains("VatSumFrgn_F" + num2);
					gvLine.Columns["VatSumFrgn_F" + num2].VisibleIndex = (list.Contains("VatSumFrgn_F" + num2) ? (gvLine.Columns["VatGroup_F" + num2].VisibleIndex + 1) : (-1));
					gvLine.Columns["TotalFrgn_F" + num2].VisibleIndex = (list.Contains("TotalFrgn_F" + num2) ? (gvLine.Columns["ExpnsCode_F" + num2].VisibleIndex + 1) : (-1));
					gvLine.Columns["LineTotal_F" + num2].Visible = false;
					gvLine.Columns["VatSum_F" + num2].Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task<ItemsImpl> GetItem(ItemsImpl doc)
	{
		try
		{
			using HttpClientHelper<ItemsImpl> _helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "ItemsMaster");
			List<ItemsImpl> list = await _helper.PostListParameter("GetOITM", doc);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	public static async Task<bool> CheckExistItem(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			DataTable dataTable = await _helper.PostAsync("CheckExistItem", itemCode);
			if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0][0] != null && dataTable.Rows[0][0] != DBNull.Value && !string.IsNullOrWhiteSpace(dataTable.Rows[0][0].ToString()))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public static DataTable GetItemByItemCode(string ItemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetItemByItemCode", new { ItemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static async Task<int> CheckItemCodeGrid(GridViewEx gv)
	{
		int ret = -1;
		try
		{
			for (int i = 0; i < gv.DataRowCount; i++)
			{
				if (gv.GetRowCellValue(i, "ItemCode") != null && !string.IsNullOrWhiteSpace(gv.GetRowCellValue(i, "ItemCode").ToString()) && !(await CheckExistItem(gv.GetRowCellValue(i, "ItemCode").ToString())))
				{
					ret = i;
					break;
				}
			}
			return ret;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ret;
		}
	}

	public static async Task<DataTable> GetBPByCardCode(string CardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			return await _helper.GetDataTableAsync("GetBPByCardCode", new
			{
				CardCode = CardCode,
				BPLId = GlobalData.DefaultBranch
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static async Task<DataTable> CheckExistCardCode(string CardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			return await _helper.GetDataTableAsync("CheckExistCardCode", new { CardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static async Task<int> CheckCardCodeGrid(GridViewEx gv)
	{
		int ret = -1;
		try
		{
			for (int i = 0; i < gv.DataRowCount; i++)
			{
				if (gv.GetRowCellValue(i, "CardCode") != null && gv.GetRowCellValue(i, "CardCode") != DBNull.Value && gv.GetRowCellValue(i, "CardCode").ToString() != string.Empty)
				{
					DataTable dataTable = await CheckExistCardCode(gv.GetRowCellValue(i, "CardCode").ToString());
					if (dataTable == null || dataTable.Rows.Count <= 0)
					{
						ret = i;
						break;
					}
				}
			}
			return ret;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ret;
		}
	}

	public static async Task<bool> CheckExistsCarCodetxt(string CardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			if (!string.IsNullOrEmpty(CardCode))
			{
				if (await _helper.GetDataTableAsync("GetBPByCardCode", new { CardCode }) != null)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return true;
		}
	}

	public static async Task<DataTable> GetEmployeeCodeBySlpCode(int slpCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			return await _helper.GetAsync("GetEmployeeCodeBySlpCode", new { slpCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static DataTable GetVatByCardCode(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			return httpClientHelper.Get("GetVatByCardCode", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public static async Task<int> CheckExistWareHouse(GridViewEx gv, string itemFieldName)
	{
		int ret = -1;
		try
		{
			for (int i = 0; i < gv.DataRowCount; i++)
			{
				object rowCellValue = gv.GetRowCellValue(i, itemFieldName);
				if (rowCellValue != null && !string.IsNullOrWhiteSpace(rowCellValue.ToString()))
				{
					DataTable dataTable = await GetWareHouseByCode(rowCellValue.ToString());
					if (dataTable == null || dataTable.Rows.Count <= 0 || (dataTable != null && dataTable.Rows.Count > 0 && Function.ToString(dataTable.Rows[0]["Locked"]) != "N"))
					{
						ret = i;
						break;
					}
					continue;
				}
				ret = i;
				break;
			}
			return ret;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ret;
		}
	}

	internal static async Task<DataTable> GetWareHouseByCode(string whsCode)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await hp.GetDataTableAsync("GetWareHouseByCode", new { whsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	internal static async Task<DataTable> GetDocumentHeader(int docNum, ApzObjectTypes docType)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await hp.GetDataTableAsync("GetDocumentHeader", new
			{
				docNum = docNum,
				type = (int)docType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	internal static async Task<bool> CheckExistProject(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await hp.GetBoolAsync("CheckExistProject", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	public static async void LoadBoMFromGridItem(string itemCode, Form mdi)
	{
		try
		{
			DataTable dataTable = await frm_BillOfMaterials.GetBoMByItemCode(itemCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (GlobalData.GeneralSetting.DriDownBOM != "N")
				{
					frm_BillOfMaterials frm_BillOfMaterials2 = new frm_BillOfMaterials();
					frm_BillOfMaterials2._searchCode = itemCode;
					frm_BillOfMaterials2.MdiParent = mdi;
					frm_BillOfMaterials2.Show();
				}
				else
				{
					frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, itemCode);
					frm_ItemMasterData2.MdiParent = mdi;
					frm_ItemMasterData2.Show();
				}
				return;
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			DataTable dataTable2 = httpClientHelper.Get("GetItemType", new { itemCode });
			if (dataTable2 != null && dataTable2.Rows.Count > 0 && (Function.ToString(dataTable2.Rows[0][0]).ToUpper() == "I" || Function.ToString(dataTable2.Rows[0][0]).ToUpper() == "L"))
			{
				frm_ItemMasterData frm_ItemMasterData3 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, itemCode);
				frm_ItemMasterData3.MdiParent = mdi;
				frm_ItemMasterData3.StartPosition = FormStartPosition.CenterScreen;
				frm_ItemMasterData3.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static ApzObjectTypes GetObjectType(this XtraFormEx frm)
	{
		ApzObjectTypes result = ApzObjectTypes.oNone;
		try
		{
			if (frm.FormObjectType != ApzObjectTypes.oNone)
			{
				result = frm.FormObjectType;
			}
			else if (!string.IsNullOrEmpty(frm.FormId))
			{
				foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
				{
					if (Function.ToString(value) == frm.FormId)
					{
						result = value;
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return result;
	}

	public static async Task<UsersImpl> GetUsersByCode(string userCode)
	{
		try
		{
			using HttpClientHelper<List<UsersImpl>> hp = new HttpClientHelper<List<UsersImpl>>(Configuration.ApiServer, "Users");
			List<UsersImpl> list = await hp.GetAsync("GetUsersByCode", new { userCode });
			return (list == null || list[0] == null) ? new UsersImpl() : list[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new UsersImpl();
		}
	}

	public static async Task<bool> GetApprovalTemplate(this DocumentsImpl doc, HttpClientHelper<DataTable> dtClientHelper, string viewMode = "3", Form form = null)
	{
		_ = 1;
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			DataTable dt = await dtClientHelper.GetDataTableAsJsonAsync("GetApprovalTemplates", new Tuple<DocumentsImpl, string, string>(doc, GlobalData.UserName, viewMode));
			if (dt == null || dt.Columns.Count == 0)
			{
				return false;
			}
			if (dt.Rows.Count > 0)
			{
				WinCommons._CloseProcessing();
				DataTable dtTemp = (from r in dt.AsEnumerable()
					group r by Function.ToString(r["WtmCode"])).Select((IGrouping<string, DataRow> g) =>
				{
					DataRow dataRow2 = dt.NewRow();
					dataRow2["WtmCode"] = g.Key;
					dataRow2["Always"] = g.Select((DataRow r) => r["Always"]).FirstOrDefault();
					return dataRow2;
				}).CopyToDataTable();
				List<int> lstTemp = new List<int>();
				int i;
				for (i = 0; i < dtTemp.Rows.Count; i++)
				{
					try
					{
						bool passTemplate = true;
						if (!Function.ParseBool(dtTemp.Rows[i]["Always"]))
						{
							DataRow[] dtQuery = (from r in dt.AsEnumerable()
								where Function.ParseInt(r["WtmCode"]) == Function.ParseInt(dtTemp.Rows[i]["WtmCode"])
								select r).ToArray();
							if (dtQuery.Length == 0)
							{
								passTemplate = false;
							}
							else
							{
								for (int k = 0; k < dtQuery.Length; k++)
								{
									string query = Function.ToString(dtQuery[k]["QueryName"]);
									if (string.IsNullOrEmpty(query))
									{
										continue;
									}
									List<string> prms = Function.GetParamFromSqlString(query);
									if (prms == null)
									{
										continue;
									}
									bool flag = false;
									foreach (string item2 in prms)
									{
										string[] array = item2.Replace("@", "").Replace(" ", "").Split('.');
										string tbl = string.Empty;
										if (array.Length > 1)
										{
											tbl = array[0];
										}
										if (DocumentMappingTable.Line.Exists((string t) => t.Equals(tbl)))
										{
											flag = true;
											break;
										}
									}
									if (flag)
									{
										bool passLine = false;
										if (doc.Lines != null && doc.Lines.Count > 0)
										{
											for (int j = 0; j < doc.Lines.Count; j++)
											{
												string text = query;
												foreach (string item3 in prms)
												{
													if (string.IsNullOrEmpty(item3))
													{
														continue;
													}
													string[] array2 = item3.Replace("@", "").Replace(" ", "").Split('.');
													string text2 = string.Empty;
													string field = string.Empty;
													if (array2.Length > 1)
													{
														text2 = array2[0];
														field = array2[1];
													}
													else
													{
														field = array2[0];
													}
													bool flag2 = true;
													if (!string.IsNullOrEmpty(text2) && !DocumentMappingTable.Header.Contains(text2))
													{
														flag2 = false;
													}
													if (flag2)
													{
														Type type;
														if (field.StartsWith("U_"))
														{
															UserFieldsImpl userFieldsImpl = doc.UserFields.FirstOrDefault((UserFieldsImpl m) => m.Name.Equals(field));
															type = ((userFieldsImpl == null) ? typeof(string) : GetUdfFieldType(userFieldsImpl.Type, userFieldsImpl.SubType));
															text = ReplaceParam(type, text, item3, userFieldsImpl.Value);
														}
														else
														{
															object valueByName = doc.GetValueByName(field, out type);
															if (valueByName != null)
															{
																text = ReplaceParam(type, text, item3, valueByName);
															}
														}
														continue;
													}
													Document_LinesImpl obj = doc.Lines[j];
													Type type2;
													if (field.StartsWith("U_"))
													{
														UserFieldsImpl userFieldsImpl2 = doc.Lines[j].UserFields.FirstOrDefault((UserFieldsImpl m) => m.Name.Equals(field));
														type2 = ((userFieldsImpl2 == null) ? typeof(string) : GetUdfFieldType(userFieldsImpl2.Type, userFieldsImpl2.SubType));
														text = ReplaceParam(type2, text, item3, userFieldsImpl2.Value);
													}
													else
													{
														object valueByName2 = obj.GetValueByName(field, out type2);
														text = ((valueByName2 == null) ? ReplaceParam(type2, text, item3, ClassHelper.GetDefaultSqlValue(type2)) : ReplaceParam(type2, text, item3, valueByName2));
													}
												}
												new DataTable();
												DataTable data = (await HttpClientHelperFactory.GetHttpClient().Commons.ExecuteQuery(text)).Data;
												if (data.Rows.Count == 0 || Function.ToString(data.Rows[0][0]).ToUpper().Equals("TRUE"))
												{
													passLine = true;
													break;
												}
											}
										}
										if (!passLine)
										{
											passTemplate = false;
											break;
										}
										continue;
									}
									foreach (string item4 in prms)
									{
										if (string.IsNullOrEmpty(item4))
										{
											continue;
										}
										string[] array3 = item4.Replace("@", "").Replace(" ", "").Split('.');
										string text3 = string.Empty;
										string field2 = string.Empty;
										if (array3.Length > 1)
										{
											text3 = array3[0];
											field2 = array3[1];
										}
										else
										{
											field2 = array3[0];
										}
										bool flag3 = true;
										if (!string.IsNullOrEmpty(text3) && !DocumentMappingTable.Header.Contains(text3))
										{
											flag3 = false;
										}
										if (!flag3)
										{
											continue;
										}
										Type type3 = typeof(string);
										if (field2.StartsWith("U_"))
										{
											UserFieldsImpl userFieldsImpl3 = doc.UserFields.FirstOrDefault((UserFieldsImpl m) => m.Name.Equals(field2));
											if (userFieldsImpl3 == null)
											{
												WinCommons.Error(Enums.MessageCode.CouldNotGetValueUdfField, "", field2);
												return false;
											}
											type3 = GetUdfFieldType(userFieldsImpl3.Type, userFieldsImpl3.SubType);
											query = ReplaceParam(type3, query, item4, userFieldsImpl3.Value);
										}
										else
										{
											object valueByName3 = doc.GetValueByName(field2, out type3);
											if (valueByName3 != null)
											{
												query = ReplaceParam(type3, query, item4, valueByName3);
											}
										}
									}
									DataTable dataTable = new DataTable();
									DataSet dataSet = httpClient.Commons.ExecuteSql(query);
									if (dataSet.Tables.Count > 1)
									{
										DataTable dataTable2 = dataSet.Tables["Message"];
										if (dataTable2.IsNotNull())
										{
											string text4 = Function.ToString(dataTable2.Rows[0]["MsgType"]);
											string text5 = Function.ToString(dataTable2.Rows[0]["Message"]);
											if (!(text4 == "200") && text4 == "5xx")
											{
												Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text5, query);
												return false;
											}
										}
										dataTable = dataSet.Tables[1];
									}
									if (dataTable.Rows.Count == 0 || !Function.ToString(dataTable.Rows[0][0]).ToUpper().Equals("TRUE"))
									{
										passTemplate = false;
										break;
									}
								}
							}
						}
						if (passTemplate)
						{
							lstTemp.Add(Function.ParseInt(dtTemp.Rows[i]["WtmCode"]));
						}
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
				}
				if (!lstTemp.Any())
				{
					return true;
				}
				DataTable source = dt.Select("WtmCode in (" + string.Join(",", lstTemp.ToArray()) + ")").CopyToDataTable().DefaultView.ToTable(true, "ReqId", "Description", "Remarks", "Name", "WtmCode");
				FrmApprovalRequest frmApprovalRequest = new FrmApprovalRequest(doc, source);
				if (form != null)
				{
					frmApprovalRequest.ShowDialog(form);
				}
				else
				{
					frmApprovalRequest.ShowDialog();
				}
				if (frmApprovalRequest.DialogResult != DialogResult.Yes)
				{
					return false;
				}
				if (frmApprovalRequest._source != null && frmApprovalRequest._source.Rows.Count > 0)
				{
					doc.Document_ApprovalRequests = new List<Document_ApprovalRequestsImpl>();
					DataRow[] array4 = dt.Select("WtmCode in (" + string.Join(",", lstTemp.ToArray()) + ")");
					foreach (DataRow row in array4)
					{
						DataRow dataRow = frmApprovalRequest._source.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["WtmCode"]) == Function.ParseInt(row["WtmCode"]));
						Document_ApprovalRequestsImpl item = new Document_ApprovalRequestsImpl
						{
							ApprovalTemplatesID = Function.ParseInt(row["WtmCode"]),
							StageId = Function.ParseInt(row["StageId"]),
							Name = Function.ToString(row["Name"]),
							ReceiptId = Function.ParseInt(row["ReceiptId"]),
							ReceiptName = Function.ToString(row["ReceiptName"]),
							Remarks = ((dataRow != null) ? Function.ToString(dataRow["Remarks"]) : null),
							ReqId = Function.ParseInt(row["ReqId"])
						};
						doc.Document_ApprovalRequests.Add(item);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return false;
		}
		return true;
	}

	public static Type GetUdfFieldType(BoFieldTypes boType, BoFldSubTypes boSubType)
	{
		switch (boType)
		{
		case BoFieldTypes.db_Alpha:
			return typeof(string);
		case BoFieldTypes.db_Date:
			if (boSubType == BoFldSubTypes.st_Time)
			{
				return typeof(string);
			}
			return typeof(DateTime);
		case BoFieldTypes.db_Float:
			return typeof(float);
		case BoFieldTypes.db_Memo:
			return typeof(string);
		case BoFieldTypes.db_Numeric:
			return typeof(double);
		default:
			return typeof(string);
		}
	}

	private static async Task<string> GetFieldNameFromTable(bool header, string field)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await helper.GetStringAsync("GetFieldNameFromTable", new { header, field });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return field;
	}

	private static string ReplaceParam(Type fieldType, string query, string prm, object ret)
	{
		try
		{
			query = ((ret != null && ret != DBNull.Value) ? ((!(fieldType == typeof(double)) && !(fieldType == typeof(double?))) ? ((!(fieldType == typeof(bool)) && !(fieldType == typeof(bool?))) ? ((!(fieldType == typeof(DateTime)) && !(fieldType == typeof(DateTime?))) ? query.Replace(prm, "'" + Function.ToString(ret) + "'") : ((!Function.ParseDateTime(ret).HasValue) ? query.Replace(prm, "") : query.Replace(prm, "'" + Convert.ToDateTime(ret).ToString("yyyyMMdd") + "'"))) : query.Replace(prm, "'" + (Function.ParseBool(ret) ? 1 : 0) + "'")) : query.Replace(prm, Function.ParseDouble(ret).ToString())) : query.Replace(prm, "null"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return query;
	}

	public static void ExpandAllTreeNodes(this TreeList tv)
	{
		try
		{
			for (int i = 0; i < tv.Nodes.Count; i++)
			{
				ExpandAllNode(tv.Nodes[i]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void ExpandAllNode(TreeListNode oParentNode)
	{
		try
		{
			oParentNode.ExpandAll();
			foreach (TreeListNode node in oParentNode.Nodes)
			{
				ExpandAllNode(node);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task LoadUdfColumns(this TreeEx tv, string tablename, Dictionary<string, ViewEditModel> formulaList = null)
	{
		try
		{
			ApzObjectTypes formObjectType = ApzObjectTypes.oNone;
			if (tv.FindForm() is XtraFormEx xtraFormEx)
			{
				if (xtraFormEx.FormObjectType != ApzObjectTypes.oNone)
				{
					formObjectType = xtraFormEx.FormObjectType;
				}
				else if (!string.IsNullOrEmpty(xtraFormEx.FormId))
				{
					foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
					{
						if (Function.ToString(value) == xtraFormEx.FormId)
						{
							formObjectType = value;
							break;
						}
					}
				}
			}
			List<UserFieldsImpl> lst = new List<UserFieldsImpl>();
			Task t1 = Task.Run(async () =>
			{
				using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
				lst = await helper.GetListAsyncByPr("GetListField", new { tablename });
			});
			DataTable dtFormula = new DataTable();
			Task t2 = Task.Run(async () =>
			{
				using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
				dtFormula = await client.GetAsync("GetFormulaSearch", new
				{
					fieldName = "",
					tblName = tablename
				});
			});
			if (tv.FirstLoadFormSetting == null)
			{
				tv.FirstLoadFormSetting = tv.ReloadFormSetting();
			}
			await tv.FirstLoadFormSetting;
			await Task.WhenAll(t1, t2);
			if (lst == null || lst.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < lst.OrderBy((UserFieldsImpl userFieldsImpl) => userFieldsImpl.FieldID).ToList().Count; i++)
			{
				TreeListColumn treeListColumn = tv.Columns["U_" + lst[i].Name];
				TreeListColumnEx col = treeListColumn as TreeListColumnEx;
				if (col == null)
				{
					col = new TreeListColumnEx
					{
						FieldName = "U_" + lst[i].Name,
						Visible = (!tv.HasFormSetting || !tv.LstOldUDF.Contains("U_" + lst[i].Name)),
						Caption = lst[i].Description,
						Width = 100
					};
				}
				col.ShowUnboundExpressionMenu = true;
				if (!string.IsNullOrEmpty(lst[i].LinkedUDO))
				{
					col.UnboundType = UnboundColumnType.String;
					RepositoryItemButtonEdit btn = new RepositoryItemButtonEdit();
					btn.Buttons[0].Kind = ButtonPredefines.Search;
					string udoDescrip = lst[i].Description;
					btn.Buttons.Add(new EditorButton
					{
						Kind = ButtonPredefines.Glyph,
						Image = Resources.right,
						IsLeft = true,
						ToolTip = udoDescrip
					});
					string udoCode = lst[i].LinkedUDO;
					string udoType = await HttpClientHelperFactory.GetHttpClient().Udo.GetTypeByCode(udoCode);
					btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
					{
						try
						{
							e.Button.Enabled = false;
							int num = -1;
							string code = "";
							if (udoType.Equals("3"))
							{
								num = Function.ParseInt(tv.GetFocusedRowCellValue(col));
							}
							else
							{
								code = Function.ToString(tv.GetFocusedRowCellValue(col));
								num = 0;
							}
							frm_UDO frm_UDO2 = new frm_UDO(udoCode, num, code);
							frm_UDO2.Text = udoDescrip;
							frm_UDO2.dataRow = tv.GetFocusedDataRow();
							frm_UDO2.ShowDialog();
							if (frm_UDO2.ChoosedEntry != null)
							{
								tv.SetFocusedRowCellValue(col.FieldName, frm_UDO2.ChoosedEntry);
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
						finally
						{
							e.Button.Enabled = true;
						}
					};
					tv.RepositoryItems.Add(btn);
					col.ColumnEdit = btn;
				}
				else if (!string.IsNullOrEmpty(lst[i].LinkedObject))
				{
					col.UnboundType = UnboundColumnType.String;
					col.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
					RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
					repositoryItemButtonEdit.Buttons.Clear();
					repositoryItemButtonEdit.Buttons.Add(new EditorButton
					{
						Kind = ButtonPredefines.Glyph,
						Image = Resources.right,
						IsLeft = true,
						ToolTip = "Linked to SystemObject " + Function.ToString((ApzObjectTypes)Function.ParseInt(lst[i].LinkedObject)),
						Tag = lst[i].LinkedObject
					});
					repositoryItemButtonEdit.Buttons.Add(new EditorButton
					{
						Kind = ButtonPredefines.Ellipsis,
						IsLeft = false,
						Tag = lst[i].LinkedObject
					});
					repositoryItemButtonEdit.Tag = lst[i].LinkedObject;
					ApzObjectTypes objType = (ApzObjectTypes)Function.ParseInt(lst[i].LinkedObject);
					repositoryItemButtonEdit.ButtonClick += async (object s, ButtonPressedEventArgs e) =>
					{
						try
						{
							e.Button.Enabled = false;
							if (e.Button.IsLeft)
							{
								string id = Function.ToString(tv.GetFocusedRowCellValue(col));
								((XtraFormEx)tv.FindForm()).DrillDown(objType, objType, id);
							}
							else
							{
								Tuple<string, DataTable> tuple = await HttpClientHelperFactory.GetHttpClient().Commons.GetDataByObject(objType, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
								DataTable item = tuple.Item2;
								string item2 = tuple.Item1;
								if (item.IsNotNull() && !string.IsNullOrEmpty(item2))
								{
									DataSet dataSource = new DataSet
									{
										Tables = { item }
									};
									frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", Enums.TypeCFL.UserDefinedData.ToString() + "-" + objType, Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL2.DataSource = dataSource;
									frm_CFL2.ShowDialog(tv.FindForm());
									if (frm_CFL2.ChooseItem != null)
									{
										tv.SetFocusedRowCellValue(col.FieldName, frm_CFL2.ChooseItem[item2]);
										tv.CloseEditor();
									}
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
						finally
						{
							e.Button.Enabled = true;
						}
					};
					tv.RepositoryItems.Add(repositoryItemButtonEdit);
					col.ColumnEdit = repositoryItemButtonEdit;
				}
				else
				{
					switch (lst[i].Type)
					{
					case BoFieldTypes.db_Alpha:
						col.UnboundType = UnboundColumnType.String;
						if (!string.IsNullOrEmpty(lst[i].LinkedTable))
						{
							RepositoryItemLookUpEdit repositoryItemLookUpEdit = await WinFuntions.CreateEditorLinkedTable(tv, col, lst[i].LinkedTable);
							tv.RepositoryItems.Add(repositoryItemLookUpEdit);
							col.ColumnEdit = repositoryItemLookUpEdit;
						}
						break;
					case BoFieldTypes.db_Date:
						col.UnboundType = UnboundColumnType.String;
						if (lst[i].SubType == BoFldSubTypes.st_Time)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Time;
							RepositoryItemTimeEdit repositoryItemTimeEdit = new RepositoryItemTimeEdit();
							tv.RepositoryItems.Add(repositoryItemTimeEdit);
							col.ColumnEdit = repositoryItemTimeEdit;
						}
						else
						{
							RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
							col.DisplayFormatType = Enums.DisplayFormatType.DateTime;
							repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.DateTime;
							repositoryItemDateEdit.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
							tv.RepositoryItems.Add(repositoryItemDateEdit);
							col.ColumnEdit = repositoryItemDateEdit;
						}
						break;
					case BoFieldTypes.db_Float:
					{
						col.UnboundType = UnboundColumnType.Decimal;
						if (lst[i].SubType == BoFldSubTypes.st_Percentage)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Percent;
						}
						else if (lst[i].SubType == BoFldSubTypes.st_Price)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Price;
						}
						else if (lst[i].SubType == BoFldSubTypes.st_Rate)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Rate;
						}
						else if (lst[i].SubType == BoFldSubTypes.st_Sum)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Amount;
						}
						else
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Qty;
						}
						RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
						repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
						repositoryItemCalcEdit2.AllowMouseWheel = false;
						tv.RepositoryItems.Add(repositoryItemCalcEdit2);
						col.ColumnEdit = repositoryItemCalcEdit2;
						break;
					}
					case BoFieldTypes.db_Memo:
						col.UnboundType = UnboundColumnType.String;
						if (lst[i].SubType == BoFldSubTypes.st_Link)
						{
							col.UnboundType = UnboundColumnType.String;
							col.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
							RepositoryItemAttachFile repositoryItemAttachFile = new RepositoryItemAttachFile();
							repositoryItemAttachFile.ObjType = formObjectType;
							tv.RepositoryItems.Add(repositoryItemAttachFile);
							col.ColumnEdit = repositoryItemAttachFile;
						}
						else
						{
							RepositoryItemMemoExEdit repositoryItemMemoExEdit = new RepositoryItemMemoExEdit();
							repositoryItemMemoExEdit.NullText = "";
							repositoryItemMemoExEdit.ShowIcon = false;
							repositoryItemMemoExEdit.BestFitWidth = col.Width + 50;
							repositoryItemMemoExEdit.WordWrap = true;
							repositoryItemMemoExEdit.AcceptsReturn = true;
							repositoryItemMemoExEdit.ParseEditValue += WinFuntions.Memo_ParseEditValue;
							repositoryItemMemoExEdit.FormatEditValue += WinFuntions.Memo_FormatEditValue;
							repositoryItemMemoExEdit.Popup += WinFuntions.Memo_Popup;
							tv.RepositoryItems.Add(repositoryItemMemoExEdit);
							col.ColumnEdit = repositoryItemMemoExEdit;
						}
						break;
					case BoFieldTypes.db_Numeric:
					{
						if (lst[i].SubType == BoFldSubTypes.st_Time)
						{
							col.UnboundType = UnboundColumnType.String;
							col.DisplayFormatType = Enums.DisplayFormatType.Time;
							RepositoryItemTimeEditEx repositoryItemTimeEditEx = new RepositoryItemTimeEditEx();
							tv.RepositoryItems.Add(repositoryItemTimeEditEx);
							col.ColumnEdit = repositoryItemTimeEditEx;
							break;
						}
						if (lst[i].SubType == BoFldSubTypes.st_Percentage)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Percent;
						}
						else if (lst[i].SubType == BoFldSubTypes.st_Price)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Price;
						}
						else if (lst[i].SubType == BoFldSubTypes.st_Rate)
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Rate;
						}
						else
						{
							col.DisplayFormatType = Enums.DisplayFormatType.Qty;
						}
						col.UnboundType = UnboundColumnType.Decimal;
						RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
						repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
						repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.UnitPriceFormat;
						tv.RepositoryItems.Add(repositoryItemCalcEdit);
						col.ColumnEdit = repositoryItemCalcEdit;
						break;
					}
					}
				}
				DataRow[] array = (from dataRow in dtFormula.AsEnumerable()
					where Function.ToString(dataRow["FieldName"]) == col.FieldName && formulaList.Keys.Any((string k) => k == Function.ToString(dataRow["DependTable"]))
					select dataRow).ToArray();
				DataTable dt = new DataTable();
				if (array != null && array.Length != 0)
				{
					dt = array.CopyToDataTable();
				}
				if (dt != null && dt.Rows.Count > 0)
				{
					string text = Function.ToString(dt.Rows[0]["FormulaType"]).ToUpper();
					if (!(text == "CBX"))
					{
						if (text == "CFL")
						{
							RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
							repositoryItemButtonEdit2.Buttons[0].Kind = ButtonPredefines.Search;
							repositoryItemButtonEdit2.ButtonClick += (object s, ButtonPressedEventArgs v) =>
							{
								try
								{
									v.Button.Enabled = false;
									frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, multiSelect: false, "", "", Enums.ItemObjectType.NoItemType, tv.FormId + tv.ItemId + col.FieldName);
									string text2 = Function.ToString(dt.Rows[0]["Formula"]);
									List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
									if (paramFromSqlString != null && paramFromSqlString.Any())
									{
										foreach (string item3 in paramFromSqlString)
										{
											if (!string.IsNullOrEmpty(item3))
											{
												string[] array2 = item3.Replace("@", "").Replace(" ", "").Split('.');
												string key = Function.ToString(dt.Rows[0]["DependTable"]);
												string fieldName = "";
												if (array2.Length > 1)
												{
													key = ((!string.IsNullOrEmpty(array2[0])) ? array2[0] : Function.ToString(dt.Rows[0]["DependTable"]));
													fieldName = array2[1];
												}
												else if (array2.Length != 0)
												{
													fieldName = array2[0];
												}
												if (formulaList != null)
												{
													ViewEditModel viewEditModel = formulaList[key];
													if (viewEditModel.Grid is TreeEx treeEx)
													{
														viewEditModel.IndexEditRow = treeEx.GetFocusedDataSourceRowIndex();
													}
													object focusedRowCellValue = viewEditModel.GetFocusedRowCellValue(fieldName);
													string text3 = "";
													if (focusedRowCellValue == null || focusedRowCellValue == DBNull.Value)
													{
														text3 = "";
													}
													text3 = ((!(focusedRowCellValue is DateTime)) ? Function.ToString(focusedRowCellValue) : Function.ParseDateTimes(focusedRowCellValue).ToString("yyyy-MM-dd"));
													text2 = text2.Replace(item3, string.IsNullOrEmpty(text3) ? "NULL" : $"'{text3}'");
												}
											}
										}
									}
									frm_ChooseFromList2.FormattedSearch = text2;
									frm_ChooseFromList2.ShowDialog();
									dynamic chooseItem = frm_ChooseFromList2.ChooseItem;
									tv.FocusedNode.SetValue(col.FieldName, Function.ToString(((DataRow)chooseItem.Row)[0]));
									tv.CloseEditor();
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								}
								finally
								{
									v.Button.Enabled = true;
								}
							};
							tv.RepositoryItems.Add(repositoryItemButtonEdit2);
							col.ColumnEdit = repositoryItemButtonEdit2;
						}
					}
					else
					{
						RepositoryItemLookUpEdit r = new RepositoryItemLookUpEdit();
						IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
						string sql = Function.ToString(dt.Rows[0]["Formula"]);
						DataTable dataTable = (DataTable)(r.DataSource = (await httpClient.Commons.ExecuteQuery(sql)).Data);
						if (dataTable != null && dataTable.Columns.Count >= 2)
						{
							r.ValueMember = dataTable.Columns[0].ColumnName;
							r.DisplayMember = dataTable.Columns[1].ColumnName;
						}
						r.NullText = string.Empty;
						tv.RepositoryItems.Add(r);
						col.ColumnEdit = r;
					}
				}
				if (lst[i].ValidValues != null && lst[i].ValidValues.Count > 0)
				{
					RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
					{
						ValueMember = "Value",
						DisplayMember = "Description",
						NullText = "",
						BestFitMode = BestFitMode.BestFitResizePopup
					};
					switch (lst[i].Type)
					{
					case BoFieldTypes.db_Alpha:
						repositoryItemLookUpEdit2.DataSource = lst[i].ValidValues;
						break;
					case BoFieldTypes.db_Numeric:
						repositoryItemLookUpEdit2.DataSource = lst[i].ValidValues.Select((ValidValuesMDImpl item) => new
						{
							Value = Function.ParseInt(item.Value),
							Description = item.Description
						});
						break;
					case BoFieldTypes.db_Float:
						repositoryItemLookUpEdit2.DataSource = lst[i].ValidValues.Select((ValidValuesMDImpl item) => new
						{
							Value = Function.ParseDecimal(item.Value),
							Description = item.Description
						});
						break;
					default:
						repositoryItemLookUpEdit2.DataSource = lst[i].ValidValues;
						break;
					}
					tv.RepositoryItems.Add(repositoryItemLookUpEdit2);
					col.ColumnEdit = repositoryItemLookUpEdit2;
				}
				if (tv.Columns != null && !tv.Columns.Any((TreeListColumn n) => n.FieldName == col.FieldName))
				{
					tv.Columns.Add(col);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static async Task LoadUdfColumns(this DataTable source, string tablename)
	{
		try
		{
			new List<UserFieldsImpl>();
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			List<UserFieldsImpl> list = await helper.GetListAsyncByPr("GetListField", new { tablename });
			if (list == null || list.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < list.OrderBy((UserFieldsImpl t) => t.FieldID).ToList().Count; i++)
			{
				DataColumn dtCol = new DataColumn();
				dtCol.ColumnName = "U_" + list[i].Name;
				switch (list[i].Type)
				{
				case BoFieldTypes.db_Alpha:
					dtCol.DataType = typeof(string);
					break;
				case BoFieldTypes.db_Date:
					if (list[i].SubType == BoFldSubTypes.st_Time)
					{
						dtCol.DataType = typeof(string);
					}
					else
					{
						dtCol.DataType = typeof(DateTime);
					}
					break;
				case BoFieldTypes.db_Float:
					dtCol.DataType = typeof(float);
					break;
				case BoFieldTypes.db_Memo:
					dtCol.DataType = typeof(string);
					break;
				case BoFieldTypes.db_Numeric:
					dtCol.DataType = typeof(double);
					break;
				}
				if (source != null && source.Columns.Cast<DataColumn>().FirstOrDefault((DataColumn t) => t.ColumnName.ToUpper().Equals(dtCol.ColumnName.ToUpper())) == null)
				{
					source.Columns.Add(dtCol);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static async Task<double> CheckQuantityInventory(string item)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await _helper.PostDataTableAsync("GetCheckQuantityInventory", item);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDouble(dataTable.Rows[0]["Avaiable"]);
			}
			return 0.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	public static async Task<double> GetQuantityInWarehouse(string item, string warehouse)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await _helper.GetAsync("GetQuantityInWarehouse", new { item, warehouse });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDouble(dataTable.Rows[0][0]);
			}
			return 0.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	internal static async Task<string> GetCurrencyByListNum(int listNum)
	{
		string ret = GlobalData.MainCurrency;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = await hp.GetAsync("GetCurrencyByListNum", new { listNum });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return ret;
	}

	public static DataTable GetFormular(this XtraFormEx frm, Enums.EventId eventId)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Formula");
			return httpClientHelper.Get("GetFormula", new
			{
				formCode = frm.GetType().Name,
				enumEvent = eventId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public static string GetFormularText(this XtraFormEx frm, Enums.EventId eventId)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Formula");
			return httpClientHelper.Get("GetFormularText", new
			{
				formCode = frm.GetType().Name,
				enumEvent = eventId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public static GridView InitUdoLineCellValueChanged(this GridViewEx gvLine, GridViewEx gvDoc, List<Formular> formulars = null, Dictionary<string, GridViewEx> tblFormular = null)
	{
		gvLine.CellValueChanged += async (object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) =>
		{
			try
			{
				if (formulars != null && formulars.Count > 0 && tblFormular != null && tblFormular.Count > 0)
				{
					string dependTable = tblFormular.FirstOrDefault((KeyValuePair<string, GridViewEx> t) => t.Value == gvLine).Key;
					if (string.IsNullOrEmpty(dependTable))
					{
						return;
					}
					string colName = e.Column.FieldName;
					List<Formular> list = formulars.Where((Formular t) => !string.IsNullOrEmpty(t.DependField) && !t.DependField.Equals(t.FieldName) && (from x in t.DependField.Split(',')
						select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
							.Length - 1]).Contains(colName.ToUpper()) && !string.IsNullOrEmpty(t.TableName) && t.TableName.Equals(dependTable) && !string.IsNullOrEmpty(t.FormularType) && !t.FormularType.ToUpper().Equals("CBX") && !t.FormularType.ToUpper().Equals("CFL")).ToList();
					if (list.Count > 0)
					{
						int i;
						for (i = 0; i < list.Count; i++)
						{
							string text = Function.ToString(list[i].FormularString);
							if (list[i].FormularType.ToUpper().Equals("DB"))
							{
								List<string> paramFromSqlString = Function.GetParamFromSqlString(text);
								if (paramFromSqlString != null && paramFromSqlString.Any())
								{
									foreach (string item in paramFromSqlString)
									{
										if (!string.IsNullOrEmpty(item))
										{
											string[] array = item.Substring(1, item.Length - 1).Split('.');
											string key = list[i].DependTable;
											string fieldName = "";
											if (array.Length > 1)
											{
												key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : list[i].DependTable);
												fieldName = array[1];
											}
											else if (array.Length != 0)
											{
												fieldName = array[0];
											}
											GridView gridView = tblFormular[key];
											int rowHandle = ((!(gridView.Name == gvLine.Name)) ? gridView.FocusedRowHandle : e.RowHandle);
											object rowCellValue = gridView.GetRowCellValue(rowHandle, fieldName);
											string arg = ((!(rowCellValue is double) && !(rowCellValue is float) && !(rowCellValue is decimal)) ? Function.ToString(rowCellValue) : Function.ParseDecimal(rowCellValue).ToString(CultureInfo.InvariantCulture));
											text = text.Replace(item, $"'{arg}'");
										}
									}
								}
								new DataTable();
								DataTable data = (await _httpClient.Commons.ExecuteQuery(text)).Data;
								if (data != null && data.Rows.Count > 0)
								{
									dynamic val;
									switch (list[i].FieldType)
									{
									case "STR":
										val = Function.ToString(data.Rows[0][0]);
										break;
									case "DEC":
										val = Function.ParseDouble(data.Rows[0][0]);
										break;
									case "DOU":
										val = Function.ParseDouble(data.Rows[0][0]);
										break;
									case "INT":
										val = Function.ParseInt(data.Rows[0][0]);
										break;
									case "DAT":
										val = Function.ParseDateTime(data.Rows[0][0]);
										break;
									case "TIM":
										val = Function.ParseDateTime(data.Rows[0][0]);
										break;
									case "DTM":
										val = Function.ParseDateTime(data.Rows[0][0]);
										break;
									case "MUL":
										_ = (object)Function.ParseDateTime(data.Rows[0][0]);
										if (data != null && data.Rows.Count > 0 && data.Columns.Count > 0)
										{
											GridViewEx gridViewEx = tblFormular[list[i].TableName];
											if (gridViewEx.Name == gvLine.Name)
											{
												_ = e.RowHandle;
											}
											else
											{
												_ = gridViewEx.FocusedRowHandle;
											}
											DataTable dataTable = (DataTable)gridViewEx.GridControl.DataSource;
											if (dataTable != null && dataTable.Rows.Count > 0)
											{
												DataRow dataRow = gridViewEx.GetDataRow(gridViewEx.FocusedRowHandle);
												WinFuntions.DataRowToDataRow(data.Rows[0], dataRow);
												gridViewEx.GridControl.RefreshDataSource();
											}
										}
										return;
									default:
										val = Function.ToString(data.Rows[0][0]);
										break;
									}
									GridViewEx gridViewEx2 = tblFormular[list[i].TableName];
									int num = ((!(gridViewEx2.Name == gvLine.Name)) ? gridViewEx2.FocusedRowHandle : e.RowHandle);
									if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(list[i].TableName) && (from x in t.DependField.Split(',')
										select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
											.Length - 1]).Contains(list[i].FieldName.ToUpper())).ToList().Count > 0)
									{
										DataTable dataTable2 = (DataTable)gridViewEx2.GridControl.DataSource;
										if (dataTable2 != null && dataTable2.Rows.Count > 0)
										{
											dataTable2.Rows[num][list[i].FieldName] = (object)val;
										}
									}
									else
									{
										gridViewEx2.SetRowCellValue(num, list[i].FieldName, val);
									}
								}
							}
							else
							{
								DataTable dataTable3 = (DataTable)gvLine.GridControl.DataSource;
								if (dataTable3.Columns.Contains("temp"))
								{
									dataTable3.Columns.Remove("temp");
								}
								if (dataTable3.Rows.Count > 0)
								{
									switch (list[i].FieldType)
									{
									case "STR":
										dataTable3.Columns.Add("temp", typeof(string));
										break;
									case "DEC":
										dataTable3.Columns.Add("temp", typeof(decimal));
										break;
									case "DOU":
										dataTable3.Columns.Add("temp", typeof(double));
										break;
									case "INT":
										dataTable3.Columns.Add("temp", typeof(int));
										break;
									case "DAT":
										dataTable3.Columns.Add("temp", typeof(DateTime));
										break;
									case "TIM":
										dataTable3.Columns.Add("temp", typeof(DateTime));
										break;
									case "DTM":
										dataTable3.Columns.Add("temp", typeof(DateTime));
										break;
									default:
										dataTable3.Columns.Add("temp", typeof(string));
										break;
									}
									try
									{
										dataTable3.Columns["temp"].Expression = text;
									}
									catch (Exception ex)
									{
										Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
										throw new Exception("Set formula failed:" + text);
									}
									GridViewEx gridViewEx3 = tblFormular[list[i].TableName];
									if (formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(list[i].TableName) && (from x in t.DependField.Split(',')
										select x.ToUpper().Trim()).Contains(list[i].FieldName.ToUpper())).ToList().Count > 0)
									{
										DataTable dataTable4 = (DataTable)gridViewEx3.GridControl.DataSource;
										if (dataTable4 != null && dataTable4.Rows.Count > 0)
										{
											dataTable4.Rows[gridViewEx3.FocusedRowHandle][list[i].FieldName] = dataTable3.Rows[e.RowHandle]["temp"];
										}
									}
									else
									{
										gridViewEx3.SetFocusedRowCellValue(list[i].FieldName, dataTable3.Rows[e.RowHandle]["temp"]);
									}
									if (dataTable3.Columns.Contains("temp"))
									{
										dataTable3.Columns.Remove("temp");
									}
								}
								dataTable3.AcceptChanges();
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				WinCommons.ErrorLineNumber(ex2);
			}
			WinCommons._CloseProcessing();
		};
		return gvLine;
	}

	public static async Task<DocumentTable> GetDocCopy(int docEntry, string objType, string to)
	{
		try
		{
			using HttpClientHelper<DocumentTable> client = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("CopyFrom", new
			{
				type = objType,
				entry = docEntry,
				to = to,
				bplId = GlobalData.DefaultBranch,
				username = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static async Task<DocumentLineTable> GetDocLineCopy(int docEntry, string objType, string to)
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> client = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("CopyFromLine", new
			{
				type = objType,
				entry = docEntry,
				to = to
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static async Task<DataTable> GetColumnSchema(string tableName)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("GetColumnSchema", new { tableName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public static async Task<string> SaveDocument(Tuple<DocumentsImpl, ApzObjectTypes> model)
	{
		try
		{
			using HttpClientHelper<Tuple<DocumentsImpl, ApzObjectTypes>> client = new HttpClientHelper<Tuple<DocumentsImpl, ApzObjectTypes>>(Configuration.ApiServer, "Commons");
			return await client.PostAsJsonAsync("SaveDocument", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static async Task<HttpResult> AddListDocument(Tuple<List<DocumentsImpl>, ApzObjectTypes> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Commons");
			return await helper.PostAsync("AddListDocument", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static async Task<HttpResult> SaveListDocument(List<DocumentsImpl> lst)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Commons");
			return await helper.PostAsync("UpdateListDocument", lst);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static async Task<string> Cancel(int docentry, ApzObjectTypes objType)
	{
		try
		{
			using HttpClientHelper<string> client = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("Cancel", new { docentry, objType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static async Task<string> Close(int docentry, ApzObjectTypes objType)
	{
		try
		{
			using HttpClientHelper<string> client = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("Close", new { docentry, objType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public static async void SetReadOnlyColumns(this GridViewEx gv, bool isReadOnly, params string[] ignoreCols)
	{
		if (gv == null)
		{
			return;
		}
		DataTable dataTable = await _httpClient.FormSettingClient.GetFormSetting(gv.FormId, gv.ItemId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
		List<string> list = new List<string>();
		if (dataTable.IsNotNull())
		{
			list = (from t in dataTable.AsEnumerable()
				where Function.ParseBool(t["IsReadOnly"])
				select Function.ToString(t["ColumnId"])).ToList();
		}
		foreach (GridColumn column in gv.Columns)
		{
			if ((ignoreCols == null || ignoreCols.Length == 0 || !ignoreCols.Contains(column.FieldName)) && (list.Count <= 0 || !list.Contains(column.FieldName)))
			{
				column.OptionsColumn.ReadOnly = isReadOnly;
			}
		}
	}

	public static void CreateColumByStructure(this GridViewEx gv, DataTable dtStruc)
	{
		try
		{
			if (!dtStruc.IsNotNull())
			{
				return;
			}
			gv.Columns.Clear();
			for (int i = 0; i < dtStruc.Rows.Count; i++)
			{
				GridColumnEx gridColumnEx = gv.ColumnsEx.AddEx();
				gridColumnEx.Caption = Function.ToString(dtStruc.Rows[i]["Caption"]);
				gridColumnEx.FieldName = Function.ToString(dtStruc.Rows[i]["ColumnName"]);
				gridColumnEx.Fixed = (Function.ToString(dtStruc.Rows[i]["Fixed"]).Equals("L") ? DevExpress.XtraGrid.Columns.FixedStyle.Left : (Function.ToString(dtStruc.Rows[i]["Fixed"]).Equals("R") ? DevExpress.XtraGrid.Columns.FixedStyle.Right : DevExpress.XtraGrid.Columns.FixedStyle.None));
				gridColumnEx.Width = Function.ParseInt(dtStruc.Rows[i]["Width"]);
				gridColumnEx.Name = $"gc_{gv.Name}_{i}";
				gridColumnEx.VisibleIndex = i;
				gridColumnEx.Visible = Function.ParseBool(dtStruc.Rows[i]["Visible"]);
				gridColumnEx.GroupIndex = Function.ParseInt(dtStruc.Rows[i]["GroupIndex"]);
				gridColumnEx.OptionsColumn.AllowSort = ((!Function.ParseBool(dtStruc.Rows[i]["AllowSort"])) ? DefaultBoolean.False : DefaultBoolean.True);
				gridColumnEx.OptionsColumn.AllowMove = Function.ParseBool(dtStruc.Rows[i]["AllowMove"]);
				gridColumnEx.OptionsColumn.AllowSize = Function.ParseBool(dtStruc.Rows[i]["AllowSize"]);
				gridColumnEx.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
				gridColumnEx.OptionsColumn.ReadOnly = Function.ParseBool(dtStruc.Rows[i]["ReadOnly"]);
				switch (Function.ToString(dtStruc.Rows[i]["DataType"]))
				{
				case "numeric":
				case "qty":
				{
					RepositoryItemCalcEdit repositoryItemCalcEdit4 = new RepositoryItemCalcEdit();
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Qty;
					gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit4);
					gridColumnEx.ColumnEdit = repositoryItemCalcEdit4;
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Qty;
					gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
					break;
				}
				case "unit":
				{
					RepositoryItemCalcEdit repositoryItemCalcEdit3 = new RepositoryItemCalcEdit();
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Unit;
					gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit3);
					gridColumnEx.ColumnEdit = repositoryItemCalcEdit3;
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Unit;
					gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
					break;
				}
				case "price":
				{
					RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Price;
					gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit2);
					gridColumnEx.ColumnEdit = repositoryItemCalcEdit2;
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Price;
					gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
					break;
				}
				case "int":
				{
					RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Integer;
					gv.GridControl.RepositoryItems.Add(repositoryItemCalcEdit);
					gridColumnEx.ColumnEdit = repositoryItemCalcEdit;
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Integer;
					gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
					break;
				}
				case "datetime":
				{
					RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					gv.GridControl.RepositoryItems.Add(repositoryItemDateEdit);
					gridColumnEx.ColumnEdit = repositoryItemDateEdit;
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static List<string> GetNormalFields(this GridViewEx gridView, List<string> requiredFields, string idField)
	{
		try
		{
			List<string> list = new List<string>();
			foreach (GridColumn column in gridView.Columns)
			{
				if (!requiredFields.Contains(column.FieldName) && column.FieldName != idField)
				{
					list.Add(column.FieldName);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<string>();
	}

	public static bool ValidateFields(this GridViewEx gridView, string requiredFields, string idField, string ignoredFields = "", string additionMessage = "")
	{
		try
		{
			List<string> requiredFields2 = requiredFields.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> ignoredFields2 = ignoredFields.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			return gridView.ValidateFields(requiredFields2, idField, ignoredFields2, additionMessage);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public static bool ValidateFields(this GridViewEx gridView, List<string> requiredFields, string idField, List<string> ignoredFields = null, string additionMessage = "")
	{
		try
		{
			List<string> normalFields = gridView.GetNormalFields(requiredFields, idField);
			if (ignoredFields == null)
			{
				ignoredFields = new List<string>();
			}
			DataTable dataTable = gridView.GridControl.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return false;
			}
			string text = "";
			foreach (string requiredField in requiredFields)
			{
				text = text + gridView.Columns[requiredField].Caption + ", ";
			}
			text = text.Remove(text.LastIndexOf(','), 2);
			text = text + " can't be null. " + additionMessage;
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				bool flag = false;
				foreach (string requiredField2 in requiredFields)
				{
					if (Function.ToString(item[requiredField2]) == "" || Function.ToString(item[requiredField2]) == "0")
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (Function.ParseInt(item[idField]) == 0)
				{
					bool flag2 = false;
					foreach (string requiredField3 in requiredFields)
					{
						if (Function.ToString(item[requiredField3]) != "" && Function.ToString(item[requiredField3]) != "0")
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						foreach (string item2 in normalFields)
						{
							if (item.Table.Columns.Contains(item2))
							{
								string text2 = Function.ToString(item[item2]);
								if (text2 != "" && text2 != "0" && !ignoredFields.Contains(item2))
								{
									flag2 = true;
									break;
								}
							}
						}
					}
					if (flag2)
					{
						WinCommons.Error(Enums.MessageCode.None, text + "(" + Function.ToString(dataTable.Rows.IndexOf(item) + 1) + ")");
						return false;
					}
					continue;
				}
				WinCommons.Error(Enums.MessageCode.None, text + "(" + Function.ToString(dataTable.Rows.IndexOf(item) + 1) + ")");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public static async Task InitChooseFromList(this GridViewEx gv, DataTable formula, Dictionary<string, GridViewEx> tblFormular = null)
	{
		Dictionary<string, RepositoryItem> dicRepository = new Dictionary<string, RepositoryItem>();
		for (int i = 0; i < formula.Rows.Count; i++)
		{
			string fieldName = Function.ToString(formula.Rows[i]["FieldName"]);
			string text = Function.ToString(formula.Rows[i]["FormulaType"]).ToUpper();
			string stFormula = Function.ToString(formula.Rows[i]["Formula"]);
			string dependTable = Function.ToString(formula.Rows[i]["DependTable"]);
			if (!(text == "CBX"))
			{
				if (!(text == "CFL"))
				{
					continue;
				}
				RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
				repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
				repositoryItemButtonEdit.TextEditStyle = TextEditStyles.Standard;
				repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs v) =>
				{
					try
					{
						frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, multiSelect: false, "", "", Enums.ItemObjectType.NoItemType, gv.FormId + gv.ItemId + fieldName);
						string text2 = stFormula;
						List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
						if (paramFromSqlString != null && paramFromSqlString.Any())
						{
							foreach (string item in paramFromSqlString)
							{
								if (!string.IsNullOrEmpty(item))
								{
									string[] array = item.Replace("@", "").Replace(" ", "").Split('.');
									string key = dependTable;
									string fieldName2 = "";
									if (array.Length > 1)
									{
										key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : dependTable);
										fieldName2 = array[1];
									}
									else if (array.Length != 0)
									{
										fieldName2 = array[0];
									}
									if (tblFormular != null)
									{
										GridViewEx gridViewEx = tblFormular[key];
										int rowHandle = ((!(gridViewEx.Name == gv.Name)) ? gridViewEx.FocusedRowHandle : gv.FocusedRowHandle);
										string text3 = Function.ToString(gridViewEx.GetRowCellValue(rowHandle, fieldName2));
										if (string.IsNullOrEmpty(text3))
										{
											WinCommons._CloseProcessing();
											break;
										}
										text2 = text2.Replace(item, $"'{text3}'");
									}
								}
							}
						}
						frm_ChooseFromList2.FormattedSearch = text2;
						frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
						dynamic chooseItem = frm_ChooseFromList2.ChooseItem;
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, Function.ToString(((DataRow)chooseItem.Row)[0]));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				};
				dicRepository[fieldName] = repositoryItemButtonEdit;
			}
			else
			{
				RepositoryItemLookUpEdit r = new RepositoryItemLookUpEdit();
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
				string sql = stFormula;
				DataTable dataTable = (DataTable)(r.DataSource = (await httpClient.Commons.ExecuteQuery(sql)).Data);
				if (dataTable != null && dataTable.Columns.Count >= 2)
				{
					r.ValueMember = dataTable.Columns[0].ColumnName;
					r.DisplayMember = dataTable.Columns[1].ColumnName;
				}
				r.NullText = string.Empty;
				dicRepository[fieldName] = r;
			}
		}
		gv.CustomRowCellEditForEditing += (object s, CustomRowCellEditEventArgs e) =>
		{
			try
			{
				if (dicRepository.ContainsKey(e.Column.FieldName))
				{
					e.RepositoryItem = dicRepository[e.Column.FieldName];
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
			}
		};
	}

	public static void SetReadOnly(this ButtonEdit bte, bool isReadOnly)
	{
		bte.ReadOnly = isReadOnly;
		for (int i = 0; i < bte.Properties.Buttons.Count; i++)
		{
			bte.Properties.Buttons[i].Visible = !isReadOnly;
		}
	}

	public static void SetReadOnly(this LookUpEdit bte, bool isReadOnly)
	{
		bte.ReadOnly = isReadOnly;
		for (int i = 0; i < bte.Properties.Buttons.Count; i++)
		{
			bte.Properties.Buttons[i].Visible = !isReadOnly;
		}
	}

	private static void GetFormatControl(Enums.DisplayFormatType type, out FormatType formatType, out string formatString)
	{
		formatType = FormatType.None;
		formatString = "";
		switch (type)
		{
		case Enums.DisplayFormatType.Amount:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.DateTime:
			formatType = FormatType.DateTime;
			formatString = GlobalData.DateTimeFormat;
			break;
		case Enums.DisplayFormatType.Percent:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}\\%";
			break;
		case Enums.DisplayFormatType.Price:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Qty:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Rate:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Unit:
			formatType = FormatType.Numeric;
			formatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			break;
		case Enums.DisplayFormatType.Time:
			formatType = FormatType.DateTime;
			formatString = GlobalData.TimeFormat;
			break;
		case Enums.DisplayFormatType.Integer:
			formatType = FormatType.Numeric;
			formatString = "n0";
			break;
		}
	}

	public static void SetFormatControl(this Control con, Enums.DisplayFormatType type)
	{
		try
		{
			if (con != null)
			{
				GetFormatControl(type, out var formatType, out var formatString);
				if (con is DateEdit)
				{
					DateEdit obj = con as DateEdit;
					obj.Properties.DisplayFormat.FormatType = formatType;
					obj.Properties.DisplayFormat.FormatString = formatString;
				}
				else if (con is CalcEdit)
				{
					CalcEdit obj2 = con as CalcEdit;
					obj2.Properties.DisplayFormat.FormatType = formatType;
					obj2.Properties.DisplayFormat.FormatString = formatString;
				}
				else if (con is TimeEdit)
				{
					TimeEdit obj3 = con as TimeEdit;
					obj3.Properties.DisplayFormat.FormatType = formatType;
					obj3.Properties.DisplayFormat.FormatString = formatString;
				}
				else if (con is TextEdit)
				{
					TextEdit obj4 = con as TextEdit;
					obj4.Properties.DisplayFormat.FormatType = formatType;
					obj4.Properties.DisplayFormat.FormatString = formatString;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void SetFormatControl(this EditorRow con, Enums.DisplayFormatType type)
	{
		try
		{
			if (con != null)
			{
				GetFormatControl(type, out var formatType, out var formatString);
				con.Properties.Format.FormatType = formatType;
				con.Properties.Format.FormatString = formatString;
			}
		}
		catch (Exception)
		{
		}
	}

	public static void SetFormatControl(this PivotGridField con, Enums.DisplayFormatType type)
	{
		try
		{
			if (con != null)
			{
				GetFormatControl(type, out var formatType, out var formatString);
				con.CellFormat.FormatType = formatType;
				con.CellFormat.FormatString = formatString;
				con.ValueFormat.FormatType = formatType;
				con.ValueFormat.FormatString = formatString;
			}
		}
		catch (Exception)
		{
		}
	}

	public static GridViewEx GetGridViewFromEditor(this BaseEdit editor)
	{
		try
		{
			if (editor == null)
			{
				return null;
			}
			if (editor.Parent is GridControlEx gridControlEx)
			{
				return (GridViewEx)gridControlEx.MainView;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static DataTable DataSourceTable(this VGridControl grd)
	{
		try
		{
			if (!(grd.DataSource is DataTable result))
			{
				return new DataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public static DataTable DataSourceTable(this PivotGridControlEx pgc)
	{
		try
		{
			if (pgc != null && pgc.DataSource != null)
			{
				return pgc.DataSource as DataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public static void LoadDataBatch(this List<Document_LinesImpl> line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		int i;
		for (i = 0; i < line.Count; i++)
		{
			List<DataRow> list = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["BaseLineNumber"]) == line[i].LineNum
				select t).ToList();
			if (list.Count > 0)
			{
				line[i].LoadDataBatch(list.CopyToDataTable(), formObjectType);
			}
		}
	}

	public static void LoadDataBatch(this Document_LinesImpl line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		List<DataColumn> list = (from DataColumn t in dtBatchSerial.Columns
			where Function.ToString(t.ColumnName).StartsWith("U_")
			select t).ToList();
		string text = Function.ToString(dtBatchSerial.Rows[0]["ObjType"]);
		DataTable dtBatchResult = new DataTable();
		dtBatchResult.Columns.Add("DistNumber", typeof(string));
		dtBatchResult.Columns.Add("LotNumber", typeof(string));
		dtBatchResult.Columns.Add("MnfSerial", typeof(string));
		dtBatchResult.Columns.Add("LogQty", typeof(double));
		if (!(text == "10000044"))
		{
			if (!(text == "10000045"))
			{
				return;
			}
			line.SerialNumbers = new List<SerialNumbersImpl>();
			if (line.BinAllocations == null)
			{
				line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
			}
			DataRow[] array = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).ToArray();
			foreach (DataRow dataRow in array)
			{
				SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
				serialNumbersImpl.BatchID = Function.ToString(dataRow["LotNumber"]);
				serialNumbersImpl.BaseLineNumber = Function.ParseInt(dataRow["BaseLineNumber"]);
				serialNumbersImpl.ReceptionDate = Function.ParseDateTime(dataRow["InDate"]);
				serialNumbersImpl.ExpiryDate = Function.ParseDateTime(dataRow["ExpDate"]);
				serialNumbersImpl.InternalSerialNumber = Function.ToString(dataRow["DistNumber"]);
				serialNumbersImpl.Location = Function.ToString(dataRow["Location"]);
				serialNumbersImpl.ManufacturerSerialNumber = Function.ToString(dataRow["MnfSerial"]);
				serialNumbersImpl.ManufactureDate = Function.ParseDateTime(dataRow["MnfDate"]);
				serialNumbersImpl.Notes = Function.ToString(dataRow["Notes"]);
				serialNumbersImpl.Quantity = 1.0;
				serialNumbersImpl.TrackingNote = Function.ParseInt(dataRow["TrackingNt"]);
				serialNumbersImpl.TrackingNoteLine = Function.ParseInt(dataRow["TrackiNtLn"]);
				serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item8 in list)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = item8.ColumnName;
						Type dataType = item8.DataType;
						if (dataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							userFieldsImpl.Value = Function.ParseDateTime(dataRow[item8.ColumnName]);
						}
						else if (dataType == typeof(float))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item8.ColumnName]);
						}
						else if (dataType == typeof(double))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item8.ColumnName]);
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl.Value = Function.ToString(dataRow[item8.ColumnName]);
						}
						serialNumbersImpl.UserFields.Add(userFieldsImpl);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl);
			}
			DataRow[] array2 = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == -1
				select t).ToArray();
			foreach (DataRow dataRow2 in array2)
			{
				SerialNumbersImpl serialNumbersImpl2 = new SerialNumbersImpl();
				serialNumbersImpl2.BatchID = Function.ToString(dataRow2["LotNumber"]);
				serialNumbersImpl2.BaseLineNumber = Function.ParseInt(dataRow2["BaseLineNumber"]);
				serialNumbersImpl2.ReceptionDate = Function.ParseDateTime(dataRow2["InDate"]);
				serialNumbersImpl2.ExpiryDate = Function.ParseDateTime(dataRow2["ExpDate"]);
				serialNumbersImpl2.InternalSerialNumber = Function.ToString(dataRow2["DistNumber"]);
				serialNumbersImpl2.Location = Function.ToString(dataRow2["Location"]);
				serialNumbersImpl2.ManufacturerSerialNumber = Function.ToString(dataRow2["MnfSerial"]);
				serialNumbersImpl2.ManufactureDate = Function.ParseDateTime(dataRow2["MnfDate"]);
				serialNumbersImpl2.Notes = Function.ToString(dataRow2["Notes"]);
				serialNumbersImpl2.Quantity = 0.0;
				serialNumbersImpl2.TrackingNote = Function.ParseInt(dataRow2["TrackingNt"]);
				serialNumbersImpl2.TrackingNoteLine = Function.ParseInt(dataRow2["TrackiNtLn"]);
				serialNumbersImpl2.SystemSerialNumber = Function.ParseInt(dataRow2["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl2.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item9 in list)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						userFieldsImpl2.Name = item9.ColumnName;
						Type dataType2 = item9.DataType;
						if (dataType2 == typeof(DateTime))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
							userFieldsImpl2.Value = Function.ParseDateTime(dataRow2[item9.ColumnName]);
						}
						else if (dataType2 == typeof(float))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Float;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item9.ColumnName]);
						}
						else if (dataType2 == typeof(double))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item9.ColumnName]);
						}
						else
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl2.Value = Function.ToString(dataRow2[item9.ColumnName]);
						}
						serialNumbersImpl2.UserFields.Add(userFieldsImpl2);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl2);
			}
			switch (formObjectType)
			{
			case ApzObjectTypes.oStockTransfer:
				if (dtBatchSerial.Columns.Contains("ToBinAbs"))
				{
					int j;
					for (j = 0; j < dtBatchSerial.Rows.Count; j++)
					{
						SerialNumbersImpl serialNumbersImpl3 = line.SerialNumbers.AsEnumerable().FirstOrDefault((SerialNumbersImpl t) => Function.ToString(t.InternalSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["DistNumber"]) && Function.ToString(t.BatchID) == Function.ToString(dtBatchSerial.Rows[j]["LotNumber"]) && Function.ToString(t.ManufacturerSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["MnfSerial"]));
						if (serialNumbersImpl3 != null)
						{
							if (Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]) > 0)
							{
								DocumentLinesBinAllocationsImpl item = new DocumentLinesBinAllocationsImpl
								{
									BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j]["BaseLineNumber"]),
									BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]),
									Quantity = Function.ParseDouble(dtBatchSerial.Rows[j]["LogQty"]),
									SerialAndBatchNumbersBaseLine = line.SerialNumbers.IndexOf(serialNumbersImpl3),
									BinActionType = BinActionTypeEnum.batFromWarehouse
								};
								line.BinAllocations.Add(item);
							}
							if (Function.ParseInt(dtBatchSerial.Rows[j]["ToBinAbs"]) > 0)
							{
								DocumentLinesBinAllocationsImpl item2 = new DocumentLinesBinAllocationsImpl
								{
									BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j]["BaseLineNumber"]),
									BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j]["ToBinAbs"]),
									Quantity = Function.ParseDouble(dtBatchSerial.Rows[j]["LogQty"]),
									SerialAndBatchNumbersBaseLine = line.SerialNumbers.IndexOf(serialNumbersImpl3),
									BinActionType = BinActionTypeEnum.batToWarehouse
								};
								line.BinAllocations.Add(item2);
							}
						}
					}
					break;
				}
				goto default;
			default:
			{
				int j2;
				for (j2 = 0; j2 < dtBatchSerial.Rows.Count; j2++)
				{
					if (Function.ParseInt(dtBatchSerial.Rows[j2]["BinAbs"]) > 0)
					{
						SerialNumbersImpl item3 = line.SerialNumbers.AsEnumerable().FirstOrDefault((SerialNumbersImpl t) => Function.ToString(t.InternalSerialNumber) == Function.ToString(dtBatchSerial.Rows[j2]["DistNumber"]) && Function.ToString(t.BatchID) == Function.ToString(dtBatchSerial.Rows[j2]["LotNumber"]) && Function.ToString(t.ManufacturerSerialNumber) == Function.ToString(dtBatchSerial.Rows[j2]["MnfSerial"]));
						DocumentLinesBinAllocationsImpl item4 = new DocumentLinesBinAllocationsImpl
						{
							BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j2]["BaseLineNumber"]),
							BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j2]["BinAbs"]),
							Quantity = Function.ParseDouble(dtBatchSerial.Rows[j2]["LogQty"]),
							SerialAndBatchNumbersBaseLine = line.SerialNumbers.IndexOf(item3),
							BinActionType = ((Function.ParseInt(dtBatchSerial.Rows[j2]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
						};
						line.BinAllocations.Add(item4);
					}
				}
				break;
			}
			case ApzObjectTypes.oOrders:
				break;
			}
			return;
		}
		line.BatchNumbers = new List<BatchNumbersImpl>();
		if (line.BinAllocations == null)
		{
			line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
		}
		DataRow[] batchResult = (from row in (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).AsEnumerable()
			group row by new
			{
				DistNumber = Function.ToString(row["DistNumber"])
			} into g
			select dtBatchResult.LoadDataRow(new object[4]
			{
				g.Key.DistNumber,
				"",
				"",
				Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"])))
			}, fAcceptChanges: false)).ToArray();
		int j3;
		for (j3 = 0; j3 < batchResult.Length; j3++)
		{
			DataRow dataRow3 = dtBatchSerial.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]) == Function.ToString(batchResult[j3]["DistNumber"]) && Function.ParseInt(t["TranType"]) == 1);
			if (dataRow3 == null)
			{
				continue;
			}
			BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(dataRow3["DistNumber"]),
				BaseLineNumber = Function.ParseInt(dataRow3["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(dataRow3["InDate"]),
				ExpiryDate = Function.ParseDateTime(dataRow3["ExpDate"]),
				InternalSerialNumber = Function.ToString(dataRow3["LotNumber"]),
				Location = Function.ToString(dataRow3["Location"]),
				ManufacturerSerialNumber = Function.ToString(dataRow3["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(dataRow3["MnfDate"]),
				Notes = Function.ToString(dataRow3["Notes"]),
				Quantity = Function.ParseDouble(batchResult[j3]["LogQty"]),
				TrackingNote = Function.ParseInt(dataRow3["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(dataRow3["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item10 in list)
				{
					UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl
					{
						Name = item10.ColumnName
					};
					Type dataType3 = item10.DataType;
					if (dataType3 == typeof(DateTime))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Date;
						userFieldsImpl3.Value = Function.ParseDateTime(dataRow3[item10.ColumnName]);
					}
					else if (dataType3 == typeof(float))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Float;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item10.ColumnName]);
					}
					else if (dataType3 == typeof(double))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item10.ColumnName]);
					}
					else
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl3.Value = Function.ToString(dataRow3[item10.ColumnName]);
					}
					batchNumbersImpl.UserFields.Add(userFieldsImpl3);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl);
		}
		DataRow[] array3 = (from t in dtBatchSerial.AsEnumerable()
			where Function.ParseInt(t["TranType"]) == -1
			select t).ToArray();
		for (int num3 = 0; num3 < array3.Length; num3++)
		{
			BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(array3[num3]["DistNumber"]),
				BaseLineNumber = Function.ParseInt(array3[num3]["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(array3[num3]["InDate"]),
				ExpiryDate = Function.ParseDateTime(array3[num3]["ExpDate"]),
				InternalSerialNumber = Function.ToString(array3[num3]["LotNumber"]),
				Location = Function.ToString(array3[num3]["Location"]),
				ManufacturerSerialNumber = Function.ToString(array3[num3]["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(array3[num3]["MnfDate"]),
				Notes = Function.ToString(array3[num3]["Notes"]),
				Quantity = 0.0,
				TrackingNote = Function.ParseInt(array3[num3]["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(array3[num3]["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl2.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item11 in list)
				{
					UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl
					{
						Name = item11.ColumnName
					};
					Type dataType4 = item11.DataType;
					if (dataType4 == typeof(DateTime))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Date;
						userFieldsImpl4.Value = Function.ParseDateTime(array3[num3][item11.ColumnName]);
					}
					else if (dataType4 == typeof(float))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Float;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item11.ColumnName]);
					}
					else if (dataType4 == typeof(double))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item11.ColumnName]);
					}
					else
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl4.Value = Function.ToString(array3[num3][item11.ColumnName]);
					}
					batchNumbersImpl2.UserFields.Add(userFieldsImpl4);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl2);
		}
		switch (formObjectType)
		{
		case ApzObjectTypes.oStockTransfer:
			if (dtBatchSerial.Columns.Contains("ToBinAbs"))
			{
				int j4;
				for (j4 = 0; j4 < dtBatchSerial.Rows.Count; j4++)
				{
					BatchNumbersImpl batchNumbersImpl3 = line.BatchNumbers.AsEnumerable().FirstOrDefault((BatchNumbersImpl t) => Function.ToString(t.BatchNumber) == Function.ToString(dtBatchSerial.Rows[j4]["DistNumber"]));
					if (batchNumbersImpl3 != null)
					{
						if (Function.ParseInt(dtBatchSerial.Rows[j4]["BinAbs"]) > 0)
						{
							DocumentLinesBinAllocationsImpl item5 = new DocumentLinesBinAllocationsImpl
							{
								BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j4]["BaseLineNumber"]),
								BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j4]["BinAbs"]),
								Quantity = Function.ParseDouble(dtBatchSerial.Rows[j4]["LogQty"]),
								SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl3),
								BinActionType = BinActionTypeEnum.batFromWarehouse
							};
							line.BinAllocations.Add(item5);
						}
						if (Function.ParseInt(dtBatchSerial.Rows[j4]["ToBinAbs"]) > 0)
						{
							DocumentLinesBinAllocationsImpl item6 = new DocumentLinesBinAllocationsImpl
							{
								BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j4]["BaseLineNumber"]),
								BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j4]["ToBinAbs"]),
								Quantity = Function.ParseDouble(dtBatchSerial.Rows[j4]["LogQty"]),
								SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl3),
								BinActionType = BinActionTypeEnum.batToWarehouse
							};
							line.BinAllocations.Add(item6);
						}
					}
				}
				break;
			}
			goto default;
		default:
		{
			int j5;
			for (j5 = 0; j5 < dtBatchSerial.Rows.Count; j5++)
			{
				BatchNumbersImpl batchNumbersImpl4 = line.BatchNumbers.AsEnumerable().FirstOrDefault((BatchNumbersImpl t) => Function.ToString(t.BatchNumber) == Function.ToString(dtBatchSerial.Rows[j5]["DistNumber"]));
				if (batchNumbersImpl4 != null && Function.ParseInt(dtBatchSerial.Rows[j5]["BinAbs"]) > 0)
				{
					DocumentLinesBinAllocationsImpl item7 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j5]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j5]["BinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j5]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl4),
						BinActionType = ((Function.ParseInt(dtBatchSerial.Rows[j5]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
					};
					line.BinAllocations.Add(item7);
				}
			}
			break;
		}
		case ApzObjectTypes.oOrders:
			break;
		}
	}

	public static void LoadDataBatch(this List<ProductionOder_LineImpl_WOR1> line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		int i;
		for (i = 0; i < line.Count; i++)
		{
			List<DataRow> list = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["BaseLineNumber"]) == line[i].LineNum
				select t).ToList();
			if (list.Count > 0)
			{
				line[i].LoadDataBatch(list.CopyToDataTable(), formObjectType);
			}
		}
	}

	public static void LoadDataBatch(this ProductionOder_LineImpl_WOR1 line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		List<DataColumn> list = (from DataColumn t in dtBatchSerial.Columns
			where Function.ToString(t.ColumnName).StartsWith("U_")
			select t).ToList();
		string text = Function.ToString(dtBatchSerial.Rows[0]["ObjType"]);
		DataTable dtBatchResult = new DataTable();
		dtBatchResult.Columns.Add("DistNumber", typeof(string));
		dtBatchResult.Columns.Add("LotNumber", typeof(string));
		dtBatchResult.Columns.Add("MnfSerial", typeof(string));
		dtBatchResult.Columns.Add("LogQty", typeof(double));
		if (!(text == "10000044"))
		{
			if (!(text == "10000045"))
			{
				return;
			}
			line.SerialNumbers = new List<SerialNumbersImpl>();
			DataRow[] array = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).ToArray();
			foreach (DataRow dataRow in array)
			{
				SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
				serialNumbersImpl.BatchID = Function.ToString(dataRow["LotNumber"]);
				serialNumbersImpl.BaseLineNumber = Function.ParseInt(dataRow["BaseLineNumber"]);
				serialNumbersImpl.ReceptionDate = Function.ParseDateTime(dataRow["InDate"]);
				serialNumbersImpl.ExpiryDate = Function.ParseDateTime(dataRow["ExpDate"]);
				serialNumbersImpl.InternalSerialNumber = Function.ToString(dataRow["DistNumber"]);
				serialNumbersImpl.Location = Function.ToString(dataRow["Location"]);
				serialNumbersImpl.ManufacturerSerialNumber = Function.ToString(dataRow["MnfSerial"]);
				serialNumbersImpl.ManufactureDate = Function.ParseDateTime(dataRow["MnfDate"]);
				serialNumbersImpl.Notes = Function.ToString(dataRow["Notes"]);
				serialNumbersImpl.Quantity = 1.0;
				serialNumbersImpl.TrackingNote = Function.ParseInt(dataRow["TrackingNt"]);
				serialNumbersImpl.TrackingNoteLine = Function.ParseInt(dataRow["TrackiNtLn"]);
				serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item in list)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = item.ColumnName;
						Type dataType = item.DataType;
						if (dataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							userFieldsImpl.Value = Function.ParseDateTime(dataRow[item.ColumnName]);
						}
						else if (dataType == typeof(float))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item.ColumnName]);
						}
						else if (dataType == typeof(double))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item.ColumnName]);
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl.Value = Function.ToString(dataRow[item.ColumnName]);
						}
						serialNumbersImpl.UserFields.Add(userFieldsImpl);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl);
			}
			DataRow[] array2 = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == -1
				select t).ToArray();
			foreach (DataRow dataRow2 in array2)
			{
				SerialNumbersImpl serialNumbersImpl2 = new SerialNumbersImpl();
				serialNumbersImpl2.BatchID = Function.ToString(dataRow2["LotNumber"]);
				serialNumbersImpl2.BaseLineNumber = Function.ParseInt(dataRow2["BaseLineNumber"]);
				serialNumbersImpl2.ReceptionDate = Function.ParseDateTime(dataRow2["InDate"]);
				serialNumbersImpl2.ExpiryDate = Function.ParseDateTime(dataRow2["ExpDate"]);
				serialNumbersImpl2.InternalSerialNumber = Function.ToString(dataRow2["DistNumber"]);
				serialNumbersImpl2.Location = Function.ToString(dataRow2["Location"]);
				serialNumbersImpl2.ManufacturerSerialNumber = Function.ToString(dataRow2["MnfSerial"]);
				serialNumbersImpl2.ManufactureDate = Function.ParseDateTime(dataRow2["MnfDate"]);
				serialNumbersImpl2.Notes = Function.ToString(dataRow2["Notes"]);
				serialNumbersImpl2.Quantity = 0.0;
				serialNumbersImpl2.TrackingNote = Function.ParseInt(dataRow2["TrackingNt"]);
				serialNumbersImpl2.TrackingNoteLine = Function.ParseInt(dataRow2["TrackiNtLn"]);
				serialNumbersImpl2.SystemSerialNumber = Function.ParseInt(dataRow2["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl2.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item2 in list)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						userFieldsImpl2.Name = item2.ColumnName;
						Type dataType2 = item2.DataType;
						if (dataType2 == typeof(DateTime))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
							userFieldsImpl2.Value = Function.ParseDateTime(dataRow2[item2.ColumnName]);
						}
						else if (dataType2 == typeof(float))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Float;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item2.ColumnName]);
						}
						else if (dataType2 == typeof(double))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item2.ColumnName]);
						}
						else
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl2.Value = Function.ToString(dataRow2[item2.ColumnName]);
						}
						serialNumbersImpl2.UserFields.Add(userFieldsImpl2);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl2);
			}
			return;
		}
		line.BatchNumbers = new List<BatchNumbersImpl>();
		DataRow[] batchResult = (from row in (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).AsEnumerable()
			group row by new
			{
				DistNumber = Function.ToString(row["DistNumber"])
			} into g
			select dtBatchResult.LoadDataRow(new object[4]
			{
				g.Key.DistNumber,
				"",
				"",
				Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"])))
			}, fAcceptChanges: false)).ToArray();
		int j;
		for (j = 0; j < batchResult.Length; j++)
		{
			DataRow dataRow3 = dtBatchSerial.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]) == Function.ToString(batchResult[j]["DistNumber"]) && Function.ParseInt(t["TranType"]) == 1);
			if (dataRow3 == null)
			{
				continue;
			}
			BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(dataRow3["DistNumber"]),
				BaseLineNumber = Function.ParseInt(dataRow3["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(dataRow3["InDate"]),
				ExpiryDate = Function.ParseDateTime(dataRow3["ExpDate"]),
				InternalSerialNumber = Function.ToString(dataRow3["LotNumber"]),
				Location = Function.ToString(dataRow3["Location"]),
				ManufacturerSerialNumber = Function.ToString(dataRow3["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(dataRow3["MnfDate"]),
				Notes = Function.ToString(dataRow3["Notes"]),
				Quantity = Function.ParseDouble(batchResult[j]["LogQty"]),
				TrackingNote = Function.ParseInt(dataRow3["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(dataRow3["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item3 in list)
				{
					UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl
					{
						Name = item3.ColumnName
					};
					Type dataType3 = item3.DataType;
					if (dataType3 == typeof(DateTime))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Date;
						userFieldsImpl3.Value = Function.ParseDateTime(dataRow3[item3.ColumnName]);
					}
					else if (dataType3 == typeof(float))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Float;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item3.ColumnName]);
					}
					else if (dataType3 == typeof(double))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item3.ColumnName]);
					}
					else
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl3.Value = Function.ToString(dataRow3[item3.ColumnName]);
					}
					batchNumbersImpl.UserFields.Add(userFieldsImpl3);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl);
		}
		DataRow[] array3 = (from t in dtBatchSerial.AsEnumerable()
			where Function.ParseInt(t["TranType"]) == -1
			select t).ToArray();
		for (int num3 = 0; num3 < array3.Length; num3++)
		{
			BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(array3[num3]["DistNumber"]),
				BaseLineNumber = Function.ParseInt(array3[num3]["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(array3[num3]["InDate"]),
				ExpiryDate = Function.ParseDateTime(array3[num3]["ExpDate"]),
				InternalSerialNumber = Function.ToString(array3[num3]["LotNumber"]),
				Location = Function.ToString(array3[num3]["Location"]),
				ManufacturerSerialNumber = Function.ToString(array3[num3]["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(array3[num3]["MnfDate"]),
				Notes = Function.ToString(array3[num3]["Notes"]),
				Quantity = 0.0,
				TrackingNote = Function.ParseInt(array3[num3]["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(array3[num3]["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl2.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item4 in list)
				{
					UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl
					{
						Name = item4.ColumnName
					};
					Type dataType4 = item4.DataType;
					if (dataType4 == typeof(DateTime))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Date;
						userFieldsImpl4.Value = Function.ParseDateTime(array3[num3][item4.ColumnName]);
					}
					else if (dataType4 == typeof(float))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Float;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item4.ColumnName]);
					}
					else if (dataType4 == typeof(double))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item4.ColumnName]);
					}
					else
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl4.Value = Function.ToString(array3[num3][item4.ColumnName]);
					}
					batchNumbersImpl2.UserFields.Add(userFieldsImpl4);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl2);
		}
	}

	public static void LoadBinAllowcation(this List<Document_LinesImpl> line, DataTable dtBinAllowcate, ApzObjectTypes formObjectType)
	{
		if (!dtBinAllowcate.IsNotNull())
		{
			return;
		}
		int i;
		for (i = 0; i < line.Count; i++)
		{
			List<DataRow> list = (from t in dtBinAllowcate.AsEnumerable()
				where Function.ParseInt(t["BaseLineNumber"]) == line[i].LineNum
				select t).ToList();
			if (list.Count > 0)
			{
				line[i].LoadDataBin(list.CopyToDataTable(), formObjectType);
			}
		}
	}

	public static void LoadDataBin(this Document_LinesImpl line, DataTable dtBinLine, ApzObjectTypes formObjectType)
	{
		if (!dtBinLine.IsNotNull())
		{
			return;
		}
		if (line.BinAllocations == null)
		{
			line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
		}
		for (int i = 0; i < dtBinLine.Rows.Count; i++)
		{
			if (Function.ParseInt(dtBinLine.Rows[i]["BinAbs"]) > 0)
			{
				DocumentLinesBinAllocationsImpl item = new DocumentLinesBinAllocationsImpl
				{
					BaseLineNumber = Function.ParseInt(dtBinLine.Rows[i]["BaseLineNumber"]),
					BinAbsEntry = Function.ParseInt(dtBinLine.Rows[i]["BinAbs"]),
					Quantity = Function.ParseDouble(dtBinLine.Rows[i]["LogQty"]),
					BinActionType = ((Function.ParseInt(dtBinLine.Rows[i]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
				};
				line.BinAllocations.Add(item);
			}
		}
	}

	public static void ShowProgressBar(this XtraFormEx frm, int maximum, int minimum = 0, int step = 1, bool percentview = true)
	{
		try
		{
			Main main = frm.MdiParent as Main;
			if (main != null)
			{
				if (main.InvokeRequired)
				{
					main.BeginInvoke((MethodInvoker)(() =>
					{
						main.bar_ProgessControl.Visibility = BarItemVisibility.Always;
						main.bar_ProgessControl.Reset();
						main.repositoryItemProgressBar1.Maximum = maximum;
						main.repositoryItemProgressBar1.Minimum = minimum;
						main.repositoryItemProgressBar1.Step = step;
						main.repositoryItemProgressBar1.PercentView = percentview;
					}));
				}
				else
				{
					main.bar_ProgessControl.Visibility = BarItemVisibility.Always;
					main.bar_ProgessControl.Reset();
					main.repositoryItemProgressBar1.Maximum = maximum;
					main.repositoryItemProgressBar1.Minimum = minimum;
					main.repositoryItemProgressBar1.Step = step;
					main.repositoryItemProgressBar1.PercentView = percentview;
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Main Form is NULL");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void SetValueProgessBar(this XtraFormEx frm, int value)
	{
		try
		{
			Main main = frm.MdiParent as Main;
			if (main != null)
			{
				if (main.InvokeRequired)
				{
					main.BeginInvoke((MethodInvoker)(() =>
					{
						main.bar_ProgessControl.Reset();
						main.bar_ProgessControl.EditValue = value;
					}));
				}
				else
				{
					main.bar_ProgessControl.Reset();
					main.bar_ProgessControl.EditValue = value;
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Main Form is NULL");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static async Task HideProgressBar(this XtraFormEx frm, int sleep = 0)
	{
		try
		{
			Thread.Sleep(sleep);
			Main main = frm.MdiParent as Main;
			if (main != null)
			{
				if (main.InvokeRequired)
				{
					main.BeginInvoke((MethodInvoker)(() =>
					{
						main.bar_ProgessControl.Reset();
						main.bar_ProgessControl.Visibility = BarItemVisibility.Never;
					}));
				}
				else
				{
					main.bar_ProgessControl.Reset();
					main.bar_ProgessControl.Visibility = BarItemVisibility.Never;
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Main Form is NULL");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static void ShowDocumentForm(this System.Windows.Forms.UserControl uc, ApzObjectTypes type, bool checkMaximized = false)
	{
		try
		{
			XtraFormEx xtraFormEx = uc.FindForm() as XtraFormEx;
			VirFormDocument virFormDocument = null;
			switch (type)
			{
			case ApzObjectTypes.oQuotations:
				virFormDocument = new frm_SalesQuotationForBatch();
				break;
			case ApzObjectTypes.oOrders:
				virFormDocument = new frm_SalesOrder();
				break;
			case ApzObjectTypes.oReserveInvoices:
				virFormDocument = new frm_AR_ReserveInvoices();
				break;
			case ApzObjectTypes.oDeliveryNotes:
				virFormDocument = new frm_Delivery();
				break;
			case ApzObjectTypes.oReturns:
				virFormDocument = new frm_Return();
				break;
			case ApzObjectTypes.oInvoices:
				virFormDocument = new frm_ARInvoices();
				break;
			case ApzObjectTypes.oCreditNotes:
				virFormDocument = new frm_ARCreditMemos();
				break;
			case ApzObjectTypes.oPurchaseRequest:
				virFormDocument = new frm_PurchaseRequest();
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				virFormDocument = new frm_PurchaseQuotation();
				break;
			case ApzObjectTypes.oPurchaseOrders:
				virFormDocument = new frm_PurchaseOder();
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				virFormDocument = new frm_GoodReceiptPO();
				break;
			case ApzObjectTypes.oPurchaseReturns:
				virFormDocument = new frm_GoodsReturn();
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				virFormDocument = new frm_APInvoices();
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				virFormDocument = new frm_APCreditMemos();
				break;
			case ApzObjectTypes.oAPReserveInvoice:
				virFormDocument = new frm_APReserveInvoices();
				break;
			}
			if (virFormDocument != null)
			{
				if (xtraFormEx != null)
				{
					virFormDocument.MdiParent = xtraFormEx.MdiParent;
				}
				if (virFormDocument.MdiParent != null)
				{
					virFormDocument.MdiParent.SuspendLayout();
				}
				if ((xtraFormEx.WindowState == FormWindowState.Maximized) & checkMaximized)
				{
					xtraFormEx.WindowState = FormWindowState.Normal;
					virFormDocument.Show();
					xtraFormEx.WindowState = FormWindowState.Maximized;
				}
				else
				{
					virFormDocument.Show();
				}
				if (virFormDocument.MdiParent != null)
				{
					virFormDocument.MdiParent.ResumeLayout();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static DataTable ConvertPivotDrillDownDataSourceToDataTable(this PivotDrillDownDataSource source)
	{
		if (source == null || source.RowCount == 0)
		{
			return new DataTable();
		}
		DataTable dataTable = new DataTable();
		if (source == null)
		{
			return dataTable;
		}
		foreach (PropertyDescriptor itemProperty in ((ITypedList)source).GetItemProperties((PropertyDescriptor[])null))
		{
			dataTable.Columns.Add(itemProperty.Name, itemProperty.PropertyType);
		}
		for (int i = 0; i < source.RowCount; i++)
		{
			List<object> list = new List<object>();
			foreach (DataColumn column in dataTable.Columns)
			{
				list.Add(source.GetValue(i, column.ColumnName));
			}
			dataTable.Rows.Add(list.ToArray());
		}
		return dataTable;
	}

	public static async Task<bool> ConnectDevice(this ZkemClient objZkeeper, int machineNumber)
	{
		bool rs = false;
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetAllBiometricConfig();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["MachineID"]) == machineNumber);
				if (dataRow == null)
				{
					return false;
				}
				string sCom = "";
				int baudRate = 115200;
				string text = Function.ToString(dataRow["CommType"]);
				string text2 = Function.ToString(dataRow["MachineID"]);
				if (string.IsNullOrWhiteSpace(text2))
				{
					Logging.Write("ERROR", "Machine ID can not be null!");
					return false;
				}
				if (Function.ParseInt(text2) < 0 || Function.ParseInt(text2) > 256)
				{
					Logging.Write("ERROR", "*Device ID illegal!");
					return false;
				}
				int num = Function.ParseInt(dataRow["CommKey"]);
				if (num < 0 || num > 999999)
				{
					Logging.Write("ERROR", "*CommKey illegal!");
					return false;
				}
				objZkeeper.SetCommPassword(num);
				switch (text)
				{
				case "T":
				{
					string text3 = Function.ToString(dataRow["IPAddress"]);
					string text4 = Function.ToString(dataRow["TCPPort"]);
					if (text3 == "" || text4 == "")
					{
						Logging.Write("ERROR", "*Name, IP, Port can not be null");
						return false;
					}
					if (Function.ParseInt(text4) <= 0 || Function.ParseInt(text4) > 65535)
					{
						Logging.Write("ERROR", "*Port illegal!");
						return false;
					}
					rs = objZkeeper.Connect_Net(text3, Function.ParseInt(text4));
					break;
				}
				case "R":
				{
					int num2 = Function.ParseInt(dataRow["COMPort"]);
					baudRate = Function.ParseInt(dataRow["BaudRate"]);
					if (Function.ToString(num2) == "" || Function.ToString(baudRate) == "")
					{
						Logging.Write("ERROR", "*Port, Baudrate cannot be null !");
						return false;
					}
					rs = objZkeeper.Connect_Com(num2, Function.ParseInt(text2), baudRate);
					break;
				}
				case "U":
				{
					if (!WinFuntions.SearchforCom(ref sCom))
					{
						Logging.Write("ERROR", "*Can not find the virtual serial port that can be used !");
						return false;
					}
					int num2;
					for (num2 = 1; num2 < 10; num2++)
					{
						if (sCom.IndexOf(num2.ToString()) > -1)
						{
							break;
						}
					}
					rs = objZkeeper.Connect_Com(num2, Function.ParseInt(text2), baudRate);
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return rs;
	}

	public static async Task LoadBioMachines(this LookUpEditEx luk)
	{
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetAllBiometricConfig();
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ToString(t["Status"]) != "N"
				select t;
			DataTable dataTable2 = (source.Any() ? source.CopyToDataTable() : dataTable.Clone());
			if (dataTable2.Rows.Count > 0)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["MachineName"] = "Tất cả";
				dataRow["MachineID"] = "-1";
				dataTable2.Rows.Add(dataRow);
			}
			luk.Properties.DataSource = dataTable2;
			luk.Properties.DisplayMember = "MachineName";
			luk.Properties.ValueMember = "MachineID";
			luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.Properties.ForceInitialize();
			luk.Properties.PopulateColumns();
			for (int num = 0; num < luk.Properties.Columns.Count; num++)
			{
				string fieldName = luk.Properties.Columns[num].FieldName;
				if (fieldName == "MachineID" || fieldName == "MachineName")
				{
					luk.Properties.Columns[num].Visible = true;
				}
				else
				{
					luk.Properties.Columns[num].Visible = false;
				}
			}
			luk.Properties.DropDownRows = dataTable.Rows.Count;
			if (dataTable.IsNotNull())
			{
				luk.EditValue = Function.ParseInt(dataTable.Rows[0]["MachineID"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static List<string> SortAsc(this List<string> lst)
	{
		lst.Sort(Function.CompareString);
		return lst;
	}

	public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where((Type t) => t != null);
		}
	}
}
