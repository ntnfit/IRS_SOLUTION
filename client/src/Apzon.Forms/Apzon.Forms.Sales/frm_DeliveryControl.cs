using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Reports;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using SAPbobsCOM;

namespace Apzon.Forms.Sales;

public class frm_DeliveryControl : XtraFormEx
{
	private const string FormController = "DeliveryControl";

	private List<UserFieldsImpl> _lstUdf;

	private List<UserFieldsImpl> _lstUdfDocumentLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataSet _dsOrder;

	private DataTable dtSearch;

	private IContainer components;

	private XtraTabControlEx tab_DeliveryControl;

	private XtraTabPageEx tab_ListOrder;

	private SimpleButtonEx btn_AddDelivery;

	private TreeEx tv_DeliveryControl;

	public ButtonEditEx txtBusinessPartner;

	public LabelLine lb_DocStatus;

	public LabelLine lb_BusinessPartner;

	public LabelLine lb_BatchNo;

	public LabelLine lb_ToDate;

	public LabelLine lb_FromDate;

	public SimpleButtonEx btnSearch;

	public TextEditEx txt_BatchNo;

	private XtraTabPageEx tab_ListProctionOrder;

	private TreeEx tre_ListPO;

	private SimpleButtonEx btn_Print;

	private XtraTabPageEx tab_ListDelivery;

	private XtraTabPageEx tab_ListInvoice;

	private TreeEx tv_ListOfInvoice;

	private TreeEx tv_ListOfDelivery;

	private SimpleButtonEx btn_AddInvoice;

	private DropDownButton cb_PrintInvoice;

	private SimpleButtonEx btn_PrintDelivery;

	private SimpleButtonEx btn_DelRequest;

	private SimpleButtonEx btn_PrintTem;

	private CheckedComboBoxEdit chb_Status;

	private SimpleButtonEx btn_PrintInvoice;

	private XtraTabPageEx tab_ListInvoiceForPayment;

	private TreeEx tv_ListInvoiceForPay;

	private SimpleButtonEx btn_PrintGTT;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	public LabelLine labelLine4;

	private CheckedComboBoxEdit chb_Whs;

	private List<Common.Table_Code> ListStatus { get; set; }

	public frm_DeliveryControl()
	{
		InitializeComponent();
	}

	private async void frm_DeliveryControl_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btnSearch.Enabled = false;
			cb_PrintInvoice.DropDownControl = CreatePopupMenuPrint();
			ListStatus = await GetStatus();
			ListStatus.Add(new Common.Table_Code
			{
				Code = "",
				Name = ""
			});
			chb_Status.Properties.Items.Clear();
			chb_Status.Properties.Items.Add("O", "Open");
			chb_Status.Properties.Items.Add("C", "Closed");
			chb_Status.Properties.Items.Add("N", "Canceled");
			chb_Status.SetEditValue("O");
			txtFromDate.EditValue = DateTime.Today;
			txtToDate.EditValue = DateTime.Today;
			await LoadWhs();
			_lstUdf = await _httpClient.Udf.GetListField("OBTN");
			_lstUdfDocumentLine = await _httpClient.Udf.GetListField("DLN1");
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btnSearch.Enabled = true;
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private void btn_Print_Click(object sender, EventArgs e)
	{
	}

	private void txt_BatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BatchNo.Text))
			{
				return;
			}
			tab_DeliveryControl.SelectedTabPage = tab_ListOrder;
			DataTable dataTable = tv_DeliveryControl.DataSourceTable();
			string arg = txt_BatchNo.Text;
			DataRow[] array = dataTable.Select($"U_BatchNo='{arg}'");
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					if (!Function.ParseBool(dataRow["Checked"]))
					{
						dataRow["Checked"] = "True";
					}
				}
			}
			btn_AddDelivery.Enabled = true;
			txt_BatchNo.Text = string.Empty;
			txt_BatchNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtBusinessPartner_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, txtBusinessPartner.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				txtBusinessPartner.Text = Function.ToString(chooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtFromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txtFromDate.EditValue != null)
			{
				txtToDate.Properties.MinValue = (DateTime)txtFromDate.EditValue;
			}
			else if (txtFromDate.EditValue == null || string.IsNullOrEmpty(txtFromDate.Text))
			{
				txtToDate.Properties.MinValue = default;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void txtToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (txtToDate.EditValue != null)
			{
				txtFromDate.Properties.MaxValue = (DateTime)txtToDate.EditValue;
			}
			else if (txtToDate.EditValue == null || string.IsNullOrEmpty(txtToDate.Text))
			{
				txtFromDate.Properties.MaxValue = DateTime.Today.AddMonths(1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void tv_DeliveryControl_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		btn_AddDelivery.Enabled = true;
		btn_Print.Enabled = true;
	}

	private void tv_DeliveryControl_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("Checked"))
			{
				int num = Function.ParseInt(tv_DeliveryControl.FocusedNode.GetValue("U_FatherId"));
				int num2 = Function.ParseInt(tv_DeliveryControl.FocusedNode.GetValue("U_ID"));
				if (num == num2 && e.Node.Nodes != null)
				{
					e.Node.SetValue("Checked", e.Value);
					int num3 = Function.ParseInt(e.Node.GetValue("U_DocEntry"));
					DataTable dataTable = tv_DeliveryControl.DataSourceTable();
					DataRow[] array = dataTable.Select($"U_DocEntry='{num3}' AND U_BatchNo <> ''");
					for (int i = 0; i < array.Length; i++)
					{
						array[i]["Checked"] = e.Value;
					}
					dataTable.AcceptChanges();
				}
			}
			btn_AddDelivery.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_DeliveryControl_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private async Task Search()
	{
		_ = 9;
		try
		{
			WinCommons._OpenProcessing();
			btnSearch.Enabled = false;
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["fromdate"] = Function.ParseDateTime(txtFromDate.EditValue);
			dataRow["todate"] = Function.ParseDateTime(txtToDate.EditValue);
			dataRow["cardcode"] = Function.ToString(txtBusinessPartner.EditValue);
			dataRow["batchno"] = Function.ToString(txt_BatchNo.Text);
			dataRow["username"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			dataRow["whs"] = Function.ToString(chb_Whs.EditValue);
			dtSearch.Rows.Add(dataRow);
			string text = "";
			for (int i = 0; i < chb_Status.Properties.Items.Count; i++)
			{
				if (chb_Status.Properties.Items[i].CheckState == CheckState.Checked)
				{
					text = text + "," + chb_Status.Properties.Items[i].Value;
				}
			}
			text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			dataRow["status"] = text;
			switch (tab_DeliveryControl.SelectedTabPage.Name)
			{
			case "tab_ListProctionOrder":
			{
				DataSet ds = await GetProduction();
				await TreeListHelper.GenerateTreeList(tre_ListPO, ds, _lstUdf, expand: true, filterRow: true);
				ds.Tables.Clear();
				ds.Dispose();
				break;
			}
			case "tab_ListOrder":
				if (_dsOrder.IsNotNull())
				{
					_dsOrder.Tables.Clear();
					_dsOrder.Dispose();
				}
				_dsOrder = await ListOfOrder();
				if (_dsOrder.IsHasValidValue())
				{
					await TreeListHelper.GenerateTreeList(tv_DeliveryControl, _dsOrder, _lstUdf, expand: true);
				}
				break;
			case "tab_ListDelivery":
			{
				DataSet dsDeli = await ListOfDelivery();
				if (dsDeli.IsHasValidValue())
				{
					await TreeListHelper.GenerateTreeList(tv_ListOfDelivery, dsDeli, _lstUdf, expand: true);
				}
				dsDeli.Tables.Clear();
				dsDeli.Dispose();
				break;
			}
			case "tab_ListInvoice":
			{
				DataSet dsInvoice = await ListOfInvoice();
				if (dsInvoice.IsHasValidValue())
				{
					await TreeListHelper.GenerateTreeList(tv_ListOfInvoice, dsInvoice, null, expand: true);
				}
				dsInvoice.Tables.Clear();
				dsInvoice.Dispose();
				break;
			}
			case "tab_ListInvoiceForPayment":
			{
				DataSet dsInvoicePay = await ListInvoiceForPayment();
				if (dsInvoicePay.IsHasValidValue())
				{
					await TreeListHelper.GenerateTreeList(tv_ListInvoiceForPay, dsInvoicePay, _lstUdf, expand: true);
				}
				dsInvoicePay.Tables.Clear();
				dsInvoicePay.Dispose();
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btnSearch.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("todate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("cardcode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("batchno", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("status", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("username", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("whs", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async Task<DataSet> ListOfOrder()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.PostAsync("Get", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetProduction()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.PostAsync("GetProduction", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> ListOfDelivery()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.PostAsync("ListOfDelivery", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> ListOfInvoice()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.PostAsync("ListOfInvoice", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> ListInvoiceForPayment()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.PostAsync("ListInvoiceForPayment", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<List<Common.Table_Code>> GetStatus()
	{
		try
		{
			using HttpClientHelper<Common.Table_Code> helper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "UDF");
			return await helper.GetListAsync("GetValidValue", new
			{
				tableId = "RDR1",
				fieldId = "Satus"
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<Common.Table_Code>();
	}

	private async Task<string> ReturnDelRequestBatch(DataTable dtBatch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryControl");
			return await _help.PostAsJsonAsync("ReturnDelRequestBatch", dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async void PrintTem(string batch)
	{
		bool flag = FormAuth == Enums.FormAuthorities.Read;
		if (!string.IsNullOrEmpty(batch))
		{
			if (batch.EndsWith(","))
			{
				batch = batch.Substring(0, batch.Length - 1);
			}
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "batch",
					Value = batch
				}
			};
			string text = await WinFuntions.ReportViewer(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\Tem TP theo Batch.rpt", lstParam, null, !flag, !flag);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_AddDelivery.Visible = false;
		btn_AddInvoice.Visible = false;
		btn_DelRequest.Visible = false;
		btn_Print.Enabled = true;
		btn_PrintDelivery.Enabled = true;
		btn_PrintInvoice.Enabled = true;
		btn_PrintGTT.Enabled = true;
		btn_PrintTem.Enabled = true;
		tv_DeliveryControl.ReadOnly = false;
		tv_ListOfDelivery.ReadOnly = false;
		tv_ListOfInvoice.ReadOnly = false;
		tv_ListInvoiceForPay.ReadOnly = false;
		txtFromDate.ReadOnly = false;
		txtToDate.ReadOnly = false;
		txt_BatchNo.ReadOnly = false;
		txtBusinessPartner.ReadOnly = false;
		chb_Status.ReadOnly = false;
	}

	private async Task LoadWhs()
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionForBatch"))
			{
				dataTable = await helper.GetAsync("LoadWhs", new
				{
					userName = GlobalData.UserName,
					branch = GlobalData.DefaultBranch
				});
			}
			if (dataTable.IsNotNull())
			{
				chb_Whs.Properties.DataSource = dataTable;
				chb_Whs.Properties.DisplayMember = "Display";
				chb_Whs.Properties.ValueMember = "Value";
			}
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ParseBool(row["Checked"]))
				{
					CheckedComboBoxEdit checkedComboBoxEdit = chb_Whs;
					CheckedComboBoxEdit checkedComboBoxEdit2 = chb_Whs;
					object editValue = (checkedComboBoxEdit2.EditValue = checkedComboBoxEdit2.EditValue?.ToString() + "," + Function.ToString(row["Value"]));
					checkedComboBoxEdit.EditValue = editValue;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		await SaveData();
	}

	private async Task SaveData()
	{
		_ = 5;
		try
		{
			new DataSet();
			_dsOrder.Copy();
			DataRow[] array = tv_DeliveryControl.DataSourceTable().Select("U_FatherId<>'-1' And Checked='True' And U_BatchNo <> ''");
			if (array.Length != 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				btn_AddDelivery.Enabled = false;
				WinCommons._OpenProcessing();
				DataTable dataTable = array.CopyToDataTable();
				if (dataTable.IsNotNull())
				{
					List<DocumentsImpl> lst = new List<DocumentsImpl>();
					DataRow[] array2 = tv_DeliveryControl.DataSourceTable().Select("U_FatherId<>'-1' And Checked='True' And U_BatchNo <> '' AND U_UnitUoM = 'H'");
					if (array2.Length != 0)
					{
						DataRow[] array3 = array2;
						foreach (DataRow dataRow in array3)
						{
							DocumentsImpl documentsImpl = new DocumentsImpl();
							documentsImpl.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
							documentsImpl.Comments = "Nhập batch hao hụt của SO " + Function.ToString(dataRow["U_DocEntry"]);
							documentsImpl.Lines = new List<Document_LinesImpl>();
							Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
							document_LinesImpl.ItemCode = Function.ToString(dataRow["U_ItemCode"]);
							document_LinesImpl.Quantity = Math.Round(Function.ParseDouble(dataRow["U_Quantity"]), 3, MidpointRounding.AwayFromZero);
							document_LinesImpl.WhsCode = Function.ToString(dataRow["U_WhsCode"]);
							document_LinesImpl.Price = 0.0;
							document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
							BatchNumbersImpl item = new BatchNumbersImpl
							{
								Quantity = Math.Round(Function.ParseDouble(dataRow["U_Quantity"]), 3, MidpointRounding.AwayFromZero),
								BatchNumber = Function.ToString(dataRow["U_BatchNo"]),
								UserFields = new List<UserFieldsImpl>
								{
									new UserFieldsImpl
									{
										Name = "U_Width",
										Value = Function.ParseDouble(dataRow["U_WidthAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									},
									new UserFieldsImpl
									{
										Name = "U_Length",
										Value = Function.ParseDouble(dataRow["U_LengthAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									},
									new UserFieldsImpl
									{
										Name = "U_QtyUoM",
										Value = Function.ParseDouble(dataRow["U_QtyUoMAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									}
								}
							};
							document_LinesImpl.BatchNumbers.Add(item);
							documentsImpl.Lines.Add(document_LinesImpl);
							documentsImpl.ObjType = 59;
							lst.Add(documentsImpl);
						}
					}
					DataRow[] array4 = tv_DeliveryControl.DataSourceTable().Select("U_FatherId<>'-1' And Checked='True' And U_BatchNo <> '' AND U_UnitUoM = 'HH' AND DiffHHQty IS NOT NULL AND DiffHHQty <> 0");
					if (array4.Length != 0)
					{
						DataRow[] array3 = array4;
						foreach (DataRow dataRow2 in array3)
						{
							double num = Math.Round(Function.ParseDouble(dataRow2["DiffHHQty"]), 3, MidpointRounding.AwayFromZero);
							DocumentsImpl documentsImpl2 = new DocumentsImpl();
							documentsImpl2.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
							documentsImpl2.Comments = "Add chênh lệch tồn kho batch hao hụt của SO " + Function.ToString(dataRow2["U_DocEntry"]);
							documentsImpl2.Lines = new List<Document_LinesImpl>();
							documentsImpl2.UserFields = new List<UserFieldsImpl>
							{
								new UserFieldsImpl
								{
									Name = "U_paymentEntry",
									Value = ((num > 0.0) ? 59 : 60),
									Type = BoFieldTypes.db_Numeric
								}
							};
							Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
							document_LinesImpl2.ItemCode = Function.ToString(dataRow2["U_ItemCode"]);
							document_LinesImpl2.Quantity = ((num > 0.0) ? num : (0.0 - num));
							document_LinesImpl2.WhsCode = Function.ToString(dataRow2["U_WhsCode"]);
							document_LinesImpl2.Price = 0.0;
							document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
							BatchNumbersImpl item2 = new BatchNumbersImpl
							{
								Quantity = ((num > 0.0) ? num : (0.0 - num)),
								BatchNumber = Function.ToString(dataRow2["U_BatchNo"]),
								UserFields = new List<UserFieldsImpl>
								{
									new UserFieldsImpl
									{
										Name = "U_Width",
										Value = Function.ParseDouble(dataRow2["U_WidthAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									},
									new UserFieldsImpl
									{
										Name = "U_Length",
										Value = Function.ParseDouble(dataRow2["U_LengthAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									},
									new UserFieldsImpl
									{
										Name = "U_QtyUoM",
										Value = Function.ParseDouble(dataRow2["U_QtyUoMAct"]),
										Type = BoFieldTypes.db_Numeric,
										SubType = BoFldSubTypes.st_Quantity
									}
								}
							};
							document_LinesImpl2.BatchNumbers.Add(item2);
							documentsImpl2.Lines.Add(document_LinesImpl2);
							documentsImpl2.ObjType = ((num > 0.0) ? 59 : 60);
							lst.Add(documentsImpl2);
						}
					}
					var enumerable = from t in dataTable.AsEnumerable()
						group t by new
						{
							docentry = Function.ParseInt(t["U_DocEntry"])
						} into g
						select new
						{
							Key = g.Key,
							Items = g
						};
					foreach (var group in enumerable)
					{
						var linegroup = from t in @group.Items
							group t by new
							{
								line = Function.ParseInt(t["U_LineNum"])
							} into g
							select new
							{
								Key = g.Key,
								Items = g
							};
						DocumentTable tableDoc = await ExtenseionMethods.GetDocCopy(group.Key.docentry, 17.ToString(), 15.ToString());
						DocumentLineTable documentLineTable = await ExtenseionMethods.GetDocLineCopy(group.Key.docentry, 17.ToString(), 15.ToString());
						new DocumentsImpl();
						DocumentsImpl documentsImpl3;
						if (tableDoc != null && tableDoc.Rows.Count > 0)
						{
							documentsImpl3 = tableDoc[0].ToDocument();
							documentsImpl3.DocEntry = 0;
							documentsImpl3.Comments = "";
							documentsImpl3.JrnlMemo = "";
							if (documentsImpl3.Lines == null)
							{
								documentsImpl3.Lines = new List<Document_LinesImpl>();
							}
						}
						else
						{
							documentsImpl3 = null;
						}
						List<Document_LinesImpl> source = new List<Document_LinesImpl>();
						if (documentLineTable != null && documentLineTable.Rows.Count > 0)
						{
							source = documentLineTable.ToDocumentLine(_lstUdfDocumentLine);
							source = source.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
						}
						foreach (var item3 in linegroup)
						{
							Document_LinesImpl document_LinesImpl3 = source.Where((Document_LinesImpl x) => x.BaseLine == Function.ParseInt(item3.Key.line)).SingleOrDefault();
							double num2 = 0.0;
							int num3 = 0;
							foreach (DataRow item4 in item3.Items)
							{
								document_LinesImpl3.WhsCode = Function.ToString(item4["U_WhsCode"]);
								BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
								{
									Quantity = Function.ParseDouble(item4["U_Quantity"]),
									BatchNumber = Function.ToString(item4["U_BatchNo"]),
									BaseLineNumber = document_LinesImpl3.LineNum
								};
								num2 += batchNumbersImpl.Quantity;
								document_LinesImpl3.BatchNumbers.Add(batchNumbersImpl);
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl
								{
									BaseLineNumber = document_LinesImpl3.LineNum,
									BinAbsEntry = Function.ParseInt(item4["U_BinId"]),
									Quantity = batchNumbersImpl.Quantity,
									SerialAndBatchNumbersBaseLine = 0
								};
								if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
								{
									document_LinesImpl3.BinAllocations.Add(documentLinesBinAllocationsImpl);
								}
								num3++;
							}
							document_LinesImpl3.Quantity = num2;
							documentsImpl3.Lines.Add(document_LinesImpl3);
						}
						lst.Add(documentsImpl3);
					}
					if (lst.Count > 0)
					{
						HttpResult httpResult = await ExtenseionMethods.SaveListDocument(lst);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						}
						else if (httpResult.Content != null)
						{
							string docEntry = Function.ToString(httpResult.Content);
							await WinFuntions.PrintLayout(docEntry, "DLN2", null, showPrintButton: false, showExportButton: false);
							string text = await Print("", docEntry, "DLN2", "ODLN", "HD", showPrinter: true);
							if (text.Equals("OK"))
							{
								WinCommons.Success(Enums.MessageCode.Success, "");
							}
							else if (!string.IsNullOrEmpty(text))
							{
								WinCommons.Error(Enums.MessageCode.None, text);
							}
						}
					}
				}
				await Search();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Bạn cần chọn ít nhất một dòng.");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_AddDelivery.Enabled = true;
	}

	private async void btn_DelRequest_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataRow[] array = tv_DeliveryControl.DataSourceTable().Select("U_FatherId<>'-1' And Checked='True' And U_BatchNo <> ''");
			if (array.Length != 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
				{
					WinCommons._OpenProcessing();
					string text = await ReturnDelRequestBatch(array.CopyToDataTable());
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						await Search();
					}
					else
					{
						WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
					}
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				MessageBox.Show("Chưa chọn hàng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PreviewDelivery(string docentry)
	{
		try
		{
			await WinFuntions.PrintLayout(docentry, "DLN2");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PrintTem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			DataRow[] array = tv_DeliveryControl.DataSourceTable().Select($"Checked='{true}' AND U_BatchNo <> ''");
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					text = text + Function.ToString(dataRow["U_BatchNo"]) + ",";
				}
			}
			PrintTem(text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListOfDelivery_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("Checked"))
			{
				int num = Function.ParseInt(tv_ListOfDelivery.FocusedNode.GetValue("U_FatherId"));
				int num2 = Function.ParseInt(tv_ListOfDelivery.FocusedNode.GetValue("U_ID"));
				if (num == num2 && e.Node.Nodes != null)
				{
					e.Node.SetValue("Checked", e.Value);
					int num3 = Function.ParseInt(e.Node.GetValue("U_DocEntry"));
					DataTable dataTable = tv_ListOfDelivery.DataSourceTable();
					DataRow[] array = dataTable.Select($"U_DocEntry='{num3}' AND U_BatchNo <> ''");
					for (int i = 0; i < array.Length; i++)
					{
						array[i]["Checked"] = e.Value;
					}
					dataTable.AcceptChanges();
				}
			}
			btn_AddInvoice.Enabled = true;
			btn_PrintDelivery.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListOfDelivery_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		btn_AddInvoice.Enabled = true;
		btn_PrintDelivery.Enabled = true;
	}

	private void tv_ListOfDelivery_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
	}

	private async void btn_PrintDelivery_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			bool isReadOnly = FormAuth == Enums.FormAuthorities.Read;
			DataTable dt = tv_ListOfDelivery.DataSourceTable();
			DataRow[] drs = dt.Select("U_FatherId = U_ID And Checked='True'");
			if (drs.Length != 0)
			{
				string docEntry = "";
				bool isPrinted = false;
				DataRow[] array = drs;
				foreach (DataRow dataRow in array)
				{
					string value = Function.ToString(dataRow["Printed"]);
					if ("HD".Equals(value) || "HDGTT".Equals(value))
					{
						isPrinted = true;
					}
					docEntry = docEntry + Function.ToString(dataRow["U_DocEntry"]) + ",";
				}
				if (!string.IsNullOrEmpty(docEntry))
				{
					docEntry = string.Join(",", docEntry.Split(',').ToList().Distinct()
						.ToArray());
					await WinFuntions.PrintLayout(docEntry, "DLN2", null, showPrintButton: false, showExportButton: false);
					if (!isReadOnly && !isPrinted)
					{
						string text = await Print("", docEntry, "DLN2", "ODLN", "HD", showPrinter: true);
						if (text.Equals("OK"))
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							array = drs;
							for (int i = 0; i < array.Length; i++)
							{
								array[i]["Printed"] = "HD";
							}
						}
						else if (!string.IsNullOrEmpty(text))
						{
							WinCommons.Error(Enums.MessageCode.None, text);
						}
					}
				}
			}
			DataRow[] array2 = dt.Select("Checked='True'");
			if (array2.Length != 0)
			{
				DataRow[] array = array2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]["Checked"] = false;
				}
				btn_PrintDelivery.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_AddInvoice_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			DataRow[] array = tv_ListOfDelivery.DataSourceTable().Select("U_FatherId = U_ID AND LineStatus <> 'C' And Checked='True'");
			if (array.Length == 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			List<DocumentsImpl> lst = new List<DocumentsImpl>();
			DataRow[] array2 = array;
			foreach (DataRow dataRow in array2)
			{
				int docEntry = Function.ParseInt(dataRow["U_DocEntry"]);
				if (docEntry <= 0)
				{
					continue;
				}
				DocumentTable tableDoc = await ExtenseionMethods.GetDocCopy(docEntry, 15.ToString(), 13.ToString());
				DocumentLineTable documentLineTable = await ExtenseionMethods.GetDocLineCopy(docEntry, 15.ToString(), 13.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (tableDoc != null && tableDoc.Rows.Count > 0)
				{
					documentsImpl = tableDoc[0].ToDocument();
					documentsImpl.DocEntry = 0;
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> lines = new List<Document_LinesImpl>();
				if (documentLineTable != null && documentLineTable.Rows.Count > 0)
				{
					lines = documentLineTable.ToDocumentLine(_lstUdfDocumentLine);
					lines = lines.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = lines;
					documentsImpl.DocDueDate = default;
					documentsImpl.ObjType = 13;
					lst.Add(documentsImpl);
				}
			}
			if (lst.Count > 0)
			{
				HttpResult httpResult = await ExtenseionMethods.SaveListDocument(lst);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListOfInvoice_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("Checked"))
			{
				int num = Function.ParseInt(tv_ListOfInvoice.FocusedNode.GetValue("U_FatherId"));
				int num2 = Function.ParseInt(tv_ListOfInvoice.FocusedNode.GetValue("U_ID"));
				if (num == num2 && e.Node.Nodes != null)
				{
					e.Node.SetValue("Checked", e.Value);
					int num3 = Function.ParseInt(e.Node.GetValue("U_DocEntry"));
					DataTable dataTable = tv_ListOfInvoice.DataSourceTable();
					DataRow[] array = dataTable.Select($"U_DocEntry='{num3}'");
					for (int i = 0; i < array.Length; i++)
					{
						array[i]["Checked"] = e.Value;
					}
					dataTable.AcceptChanges();
				}
			}
			cb_PrintInvoice.Enabled = true;
			btn_PrintInvoice.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DXPopupMenu CreatePopupMenuPrint()
	{
		return new DXPopupMenu
		{
			Items = 
			{
				new DXMenuItem("Hóa đơn đỏ", VAT_Click),
				new DXMenuItem("Giấy giới thiệu", GiayGioiThieu_Click)
			}
		};
	}

	private async void GiayGioiThieu_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow[] array = tv_ListOfInvoice.DataSourceTable().Select("U_FatherId = U_ID AND Checked='True'");
			if (array.Length == 0)
			{
				return;
			}
			DataRow[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int num = Function.ParseInt(array2[i]["U_DocEntry"]);
				if (num > 0)
				{
					await WinFuntions.PrintLayout("", num, "INV1");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void VAT_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataRow[] array = tv_ListOfInvoice.DataSourceTable().Select("U_FatherId = U_ID AND Checked='True'");
			if (array.Length == 0)
			{
				return;
			}
			DataRow[] array2 = array;
			foreach (DataRow row in array2)
			{
				int docEntry = Function.ParseInt(row["U_DocEntry"]);
				string value = Function.ToString(row["Printed"]);
				if (docEntry <= 0)
				{
					continue;
				}
				if ("N".Equals(value))
				{
					await WinFuntions.PrintLayout("", docEntry, "INV2");
					string text = await UpdatePrintedInvoice(docEntry.ToString(), "OINV", "HD");
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						continue;
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
					row["Printed"] = "Y";
				}
				else if ("Y".Equals(value))
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoTicketPrinted, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> UpdatePrintedInvoice(string docentry, string table, string type)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "PrintLayout");
			return await helper.GetAsync("UpdatePrinted", new
			{
				docentry,
				table,
				type,
				GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async void btn_PrintInvoice_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			bool isReadOnly = FormAuth == Enums.FormAuthorities.Read;
			DataTable dt = tv_ListOfInvoice.DataSourceTable();
			DataRow[] drs = dt.Select("U_FatherId = U_ID AND Checked='True'");
			if (drs.Length != 0)
			{
				string docEntry = "";
				bool isPrinted = false;
				DataRow[] array = drs;
				foreach (DataRow dataRow in array)
				{
					string value = Function.ToString(dataRow["Printed"]);
					if ("HD".Equals(value) || "HDGTT".Equals(value))
					{
						isPrinted = true;
					}
					docEntry = docEntry + Function.ToString(dataRow["U_DocEntry"]) + ",";
				}
				if (!string.IsNullOrEmpty(docEntry))
				{
					docEntry = string.Join(",", docEntry.Split(',').ToList().Distinct()
						.ToArray());
					await WinFuntions.PrintLayout(docEntry, "INV2", null, showPrintButton: false, showExportButton: false);
					if (!isReadOnly && !isPrinted)
					{
						string text = await Print("", docEntry, "INV2", "OINV", "HD", showPrinter: true);
						if (text.Equals("OK"))
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							array = drs;
							for (int i = 0; i < array.Length; i++)
							{
								array[i]["Printed"] = "HD";
							}
						}
						else if (!string.IsNullOrEmpty(text))
						{
							WinCommons.Error(Enums.MessageCode.None, text);
						}
					}
				}
			}
			DataRow[] array2 = dt.Select("Checked='True'");
			if (array2.Length != 0)
			{
				DataRow[] array = array2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]["Checked"] = false;
				}
				cb_PrintInvoice.Enabled = false;
				btn_PrintInvoice.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListInvoiceForPay_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!e.Column.FieldName.Equals("Checked"))
			{
				return;
			}
			int num = Function.ParseInt(tv_ListInvoiceForPay.FocusedNode.GetValue("U_FatherId"));
			int num2 = Function.ParseInt(tv_ListInvoiceForPay.FocusedNode.GetValue("U_ID"));
			if (num == num2 && e.Node.Nodes != null)
			{
				e.Node.SetValue("Checked", e.Value);
				int num3 = Function.ParseInt(e.Node.GetValue("U_DocEntry"));
				DataTable dataTable = tv_ListInvoiceForPay.DataSourceTable();
				DataRow[] array = dataTable.Select($"U_DocEntry='{num3}'");
				for (int i = 0; i < array.Length; i++)
				{
					array[i]["Checked"] = e.Value;
				}
				dataTable.AcceptChanges();
			}
			btn_PrintGTT.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_PrintGTT_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			bool isReadOnly = FormAuth == Enums.FormAuthorities.Read;
			DataTable dt = tv_ListInvoiceForPay.DataSourceTable();
			DataRow[] array = dt.Select("U_FatherId = U_ID AND Checked='True'");
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow row in array2)
				{
					int docEntry = Function.ParseInt(row["U_DocEntry"]);
					string printed = Function.ToString(row["Printed"]);
					if (docEntry <= 0)
					{
						continue;
					}
					ReportDocument rp = await PrintLayout(docEntry.ToString(), "INV1", null, showPrintButton: false, showExportButton: false);
					if (isReadOnly || "GTT".Equals(printed) || "HDGTT".Equals(printed))
					{
						continue;
					}
					string text = ReportPrinter(rp, showPrinter: true);
					if (text.Equals("OK"))
					{
						string text2 = await UpdatePrintedInvoice(docEntry.ToString(), "OINV", "GTT");
						if (!string.IsNullOrEmpty(text2))
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
							continue;
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						row["Printed"] = "GTT";
					}
					else if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
			DataRow[] array3 = dt.Select("Checked='True'");
			if (array3.Length != 0)
			{
				DataRow[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					array4[j]["Checked"] = false;
				}
				btn_PrintGTT.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<string> Print(string cardCode, string docEntry, string formLayoutCode, string table, string type, bool showPrinter = false)
	{
		_ = 1;
		try
		{
			PrintLayout printLayout = await WinFuntions.GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, Function.ToString(cardCode));
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
				{
					new Common.ParameterValue
					{
						Name = "DOCKEY@",
						Value = docEntry
					}
				};
				return await ReportPrinter($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{printLayout.FileName}", lstParam, docEntry, table, type, showPrinter);
			}
			return "Không tìm thấy layout!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	public async Task<string> ReportPrinter(string pathFile, List<Common.ParameterValue> lstParam, string docEntry, string table, string type, bool showPrinter = false)
	{
		_ = 1;
		try
		{
			string isprint = "";
			new ReportDocument();
			ReportDocument _rp = await WinFuntions.ReportToPrinter(pathFile, lstParam);
			if (_rp == null)
			{
				return null;
			}
			if (showPrinter)
			{
				PrinterSettings printerSettings = new PrinterSettings();
				PrintDialog printDialog = new PrintDialog
				{
					PrinterSettings = printerSettings,
					AllowPrintToFile = false,
					AllowSomePages = false,
					UseEXDialog = true,
					AllowCurrentPage = false
				};
				PageSettings pageSetting = new PageSettings(printerSettings);
				if (printDialog.ShowDialog(this) == DialogResult.OK)
				{
					string text = await UpdatePrintedInvoice(docEntry, table, type);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						isprint = text;
					}
					else
					{
						printerSettings.Copies = 1;
						_rp.PrintOptions.PrinterName = printerSettings.PrinterName;
						_rp.PrintToPrinter(printerSettings, pageSetting, reformatReportPageSettings: false);
						isprint = "OK";
					}
				}
			}
			else
			{
				_rp.PrintToPrinter(1, collated: true, 1, _rp.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
			}
			foreach (Table table2 in _rp.Database.Tables)
			{
				table2.Dispose();
			}
			_rp.Database.Dispose();
			_rp.Close();
			_rp.Dispose();
			GC.Collect();
			return isprint;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public async Task<ReportDocument> PrintLayout(string docEntry, string formLayoutCode, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true)
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrEmpty(docEntry))
			{
				PrintLayout printLayout = await WinFuntions.GetDefaultPrintLayout(GlobalData.UserName, formLayoutCode, "");
				if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
				{
					List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
					{
						new Common.ParameterValue
						{
							Name = "DOCKEY@",
							Value = docEntry
						}
					};
					return await ReportViewer(Application.StartupPath + "\\Report\\" + printLayout.FileName, lstParam, frm_Parent, showPrintButton, showExportButton);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public static async Task<ReportDocument> ReportViewer(string pathFile, List<Common.ParameterValue> lstParam, Form frm_Parent = null, bool showPrintButton = true, bool showExportButton = true)
	{
		ReportDocument _rp = new ReportDocument();
		try
		{
			pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + Path.GetFileName(pathFile);
			frm_ReportViewer _frm = new frm_ReportViewer
			{
				_ShowPrintButton = showPrintButton,
				_ShowExportButton = showExportButton
			};
			_rp = await WinFuntions.ReportToPrinter(pathFile, lstParam);
			if (_rp == null)
			{
				return null;
			}
			_frm._rp = _rp;
			_frm._Dispose = false;
			if (frm_Parent != null)
			{
				_frm.MdiParent = frm_Parent.MdiParent;
				_frm.MinimizeBox = true;
				_frm.Show();
			}
			else
			{
				_frm.ShowDialog();
			}
			_rp = _frm._rp;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return _rp;
	}

	public string ReportPrinter(ReportDocument _rp, bool showPrinter = false)
	{
		try
		{
			string result = "";
			if (showPrinter)
			{
				PrinterSettings printerSettings = new PrinterSettings();
				PrintDialog printDialog = new PrintDialog
				{
					PrinterSettings = printerSettings,
					AllowPrintToFile = false,
					AllowSomePages = false,
					UseEXDialog = true,
					AllowCurrentPage = false
				};
				PageSettings pageSettings = new PageSettings(printerSettings);
				if (printDialog.ShowDialog(this) == DialogResult.OK)
				{
					printerSettings.Copies = 1;
					_rp.PrintOptions.PrinterName = printerSettings.PrinterName;
					_rp.PrintToPrinter(printerSettings, pageSettings, reformatReportPageSettings: false);
					result = "OK";
				}
			}
			else
			{
				_rp.PrintToPrinter(1, collated: true, 1, _rp.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
			}
			foreach (Table table in _rp.Database.Tables)
			{
				table.Dispose();
			}
			_rp.Database.Dispose();
			_rp.Close();
			_rp.Dispose();
			GC.Collect();
			return result;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			switch (tab_DeliveryControl.SelectedTabPage.Name)
			{
			case "tab_ListProctionOrder":
				btn_Print_Click(null, null);
				break;
			case "tab_ListOrder":
				btn_PrintTem_Click(null, null);
				break;
			case "tab_ListDelivery":
				btn_PrintDelivery_Click(null, null);
				break;
			case "tab_ListInvoice":
				btn_PrintInvoice_Click(null, null);
				break;
			case "tab_ListInvoiceForPayment":
				btn_PrintGTT_Click(null, null);
				break;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_DeliveryControl));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.tab_DeliveryControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_ListOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PrintTem = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_DelRequest = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_DeliveryControl = new Apzon.Controls.Tree.TreeEx();
		this.btn_AddDelivery = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tab_ListProctionOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tre_ListPO = new Apzon.Controls.Tree.TreeEx();
		this.tab_ListDelivery = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PrintDelivery = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_AddInvoice = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_ListOfDelivery = new Apzon.Controls.Tree.TreeEx();
		this.tab_ListInvoice = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PrintInvoice = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cb_PrintInvoice = new DevExpress.XtraEditors.DropDownButton();
		this.tv_ListOfInvoice = new Apzon.Controls.Tree.TreeEx();
		this.tab_ListInvoiceForPayment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PrintGTT = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_ListInvoiceForPay = new Apzon.Controls.Tree.TreeEx();
		this.txtBusinessPartner = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.lb_BatchNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_BatchNo = new Apzon.Controls.Editor.TextEditEx();
		this.chb_Status = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.chb_Whs = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		((System.ComponentModel.ISupportInitialize)this.tab_DeliveryControl).BeginInit();
		this.tab_DeliveryControl.SuspendLayout();
		this.tab_ListOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_DeliveryControl).BeginInit();
		this.tab_ListProctionOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_ListPO).BeginInit();
		this.tab_ListDelivery.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListOfDelivery).BeginInit();
		this.tab_ListInvoice.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListOfInvoice).BeginInit();
		this.tab_ListInvoiceForPayment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListInvoiceForPay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Whs.Properties).BeginInit();
		base.SuspendLayout();
		this.tab_DeliveryControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_DeliveryControl.Location = new System.Drawing.Point(0, 55);
		this.tab_DeliveryControl.Name = "tab_DeliveryControl";
		this.tab_DeliveryControl.SelectedTabPage = this.tab_ListOrder;
		this.tab_DeliveryControl.Size = new System.Drawing.Size(1083, 456);
		this.tab_DeliveryControl.TabIndex = 9;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.tab_DeliveryControl.TabPages;
		DevExpress.XtraTab.XtraTabPage[] pages = new Apzon.Controls.Controls.XtraTabPageEx[5] { this.tab_ListProctionOrder, this.tab_ListOrder, this.tab_ListDelivery, this.tab_ListInvoice, this.tab_ListInvoiceForPayment };
		tabPages.AddRange(pages);
		this.tab_ListOrder.Controls.Add(this.btn_PrintTem);
		this.tab_ListOrder.Controls.Add(this.btn_DelRequest);
		this.tab_ListOrder.Controls.Add(this.tv_DeliveryControl);
		this.tab_ListOrder.Controls.Add(this.btn_AddDelivery);
		this.tab_ListOrder.Name = "tab_ListOrder";
		this.tab_ListOrder.Size = new System.Drawing.Size(1077, 428);
		this.tab_ListOrder.Text = "List of Batch";
		this.tab_ListOrder.Tooltip = "Danh sách hàng thành phẩm chờ đi giao";
		this.btn_PrintTem.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintTem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintTem.ImageOptions.Image");
		this.btn_PrintTem.Location = new System.Drawing.Point(860, 402);
		this.btn_PrintTem.Name = "btn_PrintTem";
		this.btn_PrintTem.Size = new System.Drawing.Size(93, 23);
		this.btn_PrintTem.TabIndex = 12;
		this.btn_PrintTem.Text = "Print Batch";
		this.btn_PrintTem.ToolTip = "In lại tem thành phẩm";
		this.btn_PrintTem.Click += new System.EventHandler(this.btn_PrintTem_Click);
		this.btn_DelRequest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_DelRequest.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_DelRequest.ImageOptions.Image");
		this.btn_DelRequest.Location = new System.Drawing.Point(959, 402);
		this.btn_DelRequest.Name = "btn_DelRequest";
		this.btn_DelRequest.Size = new System.Drawing.Size(111, 23);
		this.btn_DelRequest.TabIndex = 11;
		this.btn_DelRequest.Text = "Return Request";
		this.btn_DelRequest.ToolTip = "Trả hàng lại bộ phận sản xuất";
		this.btn_DelRequest.Click += new System.EventHandler(this.btn_DelRequest_Click);
		this.tv_DeliveryControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_DeliveryControl.KeyFieldName = "U_ID";
		this.tv_DeliveryControl.Location = new System.Drawing.Point(0, 0);
		this.tv_DeliveryControl.Name = "tv_DeliveryControl";
		this.tv_DeliveryControl.ParentFieldName = "U_FatherId";
		this.tv_DeliveryControl.Size = new System.Drawing.Size(1074, 400);
		this.tv_DeliveryControl.TabIndex = 0;
		this.tv_DeliveryControl.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_DeliveryControl_CellValueChanging);
		this.tv_DeliveryControl.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_DeliveryControl_CellValueChanged);
		this.tv_DeliveryControl.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_DeliveryControl_ShowingEditor);
		this.btn_AddDelivery.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AddDelivery.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddDelivery.ImageOptions.Image");
		this.btn_AddDelivery.Location = new System.Drawing.Point(3, 402);
		this.btn_AddDelivery.Name = "btn_AddDelivery";
		this.btn_AddDelivery.Size = new System.Drawing.Size(103, 23);
		this.btn_AddDelivery.TabIndex = 10;
		this.btn_AddDelivery.Text = "Add Delivery";
		this.btn_AddDelivery.ToolTip = "Tạo phiếu giao hàng";
		this.btn_AddDelivery.Click += new System.EventHandler(this.btn_OK_Click);
		this.tab_ListProctionOrder.Controls.Add(this.btn_Print);
		this.tab_ListProctionOrder.Controls.Add(this.tre_ListPO);
		this.tab_ListProctionOrder.Name = "tab_ListProctionOrder";
		this.tab_ListProctionOrder.Size = new System.Drawing.Size(1077, 428);
		this.tab_ListProctionOrder.Text = "List Of PO";
		this.tab_ListProctionOrder.Tooltip = "Danh sách các lệnh sản xuất";
		this.btn_Print.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Print.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Print.ImageOptions.Image");
		this.btn_Print.Location = new System.Drawing.Point(3, 402);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.Size = new System.Drawing.Size(113, 23);
		this.btn_Print.TabIndex = 36;
		this.btn_Print.Text = "In phiếu tìm hàng";
		this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
		this.tre_ListPO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tre_ListPO.Location = new System.Drawing.Point(0, 0);
		this.tre_ListPO.Name = "tre_ListPO";
		this.tre_ListPO.Size = new System.Drawing.Size(1074, 400);
		this.tre_ListPO.TabIndex = 0;
		this.tab_ListDelivery.Controls.Add(this.btn_PrintDelivery);
		this.tab_ListDelivery.Controls.Add(this.btn_AddInvoice);
		this.tab_ListDelivery.Controls.Add(this.tv_ListOfDelivery);
		this.tab_ListDelivery.Name = "tab_ListDelivery";
		this.tab_ListDelivery.Size = new System.Drawing.Size(1077, 428);
		this.tab_ListDelivery.Text = "List of Delivery";
		this.tab_ListDelivery.Tooltip = "Danh sách phiếu giao hàng";
		this.btn_PrintDelivery.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintDelivery.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintDelivery.ImageOptions.Image");
		this.btn_PrintDelivery.Location = new System.Drawing.Point(964, 402);
		this.btn_PrintDelivery.Name = "btn_PrintDelivery";
		this.btn_PrintDelivery.Size = new System.Drawing.Size(106, 23);
		this.btn_PrintDelivery.TabIndex = 37;
		this.btn_PrintDelivery.Text = "Print Delivery";
		this.btn_PrintDelivery.ToolTip = "In Phiếu giao hàng";
		this.btn_PrintDelivery.Click += new System.EventHandler(this.btn_PrintDelivery_Click);
		this.btn_AddInvoice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AddInvoice.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddInvoice.ImageOptions.Image");
		this.btn_AddInvoice.Location = new System.Drawing.Point(3, 402);
		this.btn_AddInvoice.Name = "btn_AddInvoice";
		this.btn_AddInvoice.Size = new System.Drawing.Size(113, 23);
		this.btn_AddInvoice.TabIndex = 11;
		this.btn_AddInvoice.Text = "Add A/R Invoice";
		this.btn_AddInvoice.ToolTip = "Tạo Hóa Đơn Đỏ";
		this.btn_AddInvoice.Click += new System.EventHandler(this.btn_AddInvoice_Click);
		this.tv_ListOfDelivery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_ListOfDelivery.KeyFieldName = "U_ID";
		this.tv_ListOfDelivery.Location = new System.Drawing.Point(0, 0);
		this.tv_ListOfDelivery.Name = "tv_ListOfDelivery";
		this.tv_ListOfDelivery.ParentFieldName = "U_FatherId";
		this.tv_ListOfDelivery.Size = new System.Drawing.Size(1074, 400);
		this.tv_ListOfDelivery.TabIndex = 1;
		this.tv_ListOfDelivery.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_ListOfDelivery_NodeCellStyle);
		this.tv_ListOfDelivery.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListOfDelivery_CellValueChanging);
		this.tv_ListOfDelivery.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListOfDelivery_CellValueChanged);
		this.tab_ListInvoice.Controls.Add(this.btn_PrintInvoice);
		this.tab_ListInvoice.Controls.Add(this.cb_PrintInvoice);
		this.tab_ListInvoice.Controls.Add(this.tv_ListOfInvoice);
		this.tab_ListInvoice.Name = "tab_ListInvoice";
		this.tab_ListInvoice.Size = new System.Drawing.Size(1077, 428);
		this.tab_ListInvoice.Text = "List of AR Invoice";
		this.tab_ListInvoice.Tooltip = "Danh sách hóa đơn của đơn hàng cần xuất hóa đơn ngay";
		this.btn_PrintInvoice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintInvoice.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintInvoice.ImageOptions.Image");
		this.btn_PrintInvoice.Location = new System.Drawing.Point(965, 405);
		this.btn_PrintInvoice.Name = "btn_PrintInvoice";
		this.btn_PrintInvoice.Size = new System.Drawing.Size(109, 23);
		this.btn_PrintInvoice.TabIndex = 38;
		this.btn_PrintInvoice.Text = "Print Invoice";
		this.btn_PrintInvoice.ToolTip = "In Hóa Đơn Đỏ";
		this.btn_PrintInvoice.Click += new System.EventHandler(this.btn_PrintInvoice_Click);
		this.cb_PrintInvoice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.cb_PrintInvoice.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("cb_PrintInvoice.ImageOptions.Image");
		this.cb_PrintInvoice.Location = new System.Drawing.Point(845, 405);
		this.cb_PrintInvoice.Name = "cb_PrintInvoice";
		this.cb_PrintInvoice.Size = new System.Drawing.Size(114, 23);
		this.cb_PrintInvoice.TabIndex = 2;
		this.cb_PrintInvoice.Text = "In Hóa đơn";
		this.cb_PrintInvoice.Visible = false;
		this.tv_ListOfInvoice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_ListOfInvoice.KeyFieldName = "U_ID";
		this.tv_ListOfInvoice.Location = new System.Drawing.Point(0, 0);
		this.tv_ListOfInvoice.Name = "tv_ListOfInvoice";
		this.tv_ListOfInvoice.ParentFieldName = "U_FatherId";
		this.tv_ListOfInvoice.Size = new System.Drawing.Size(1074, 400);
		this.tv_ListOfInvoice.TabIndex = 1;
		this.tv_ListOfInvoice.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListOfInvoice_CellValueChanging);
		this.tab_ListInvoiceForPayment.Controls.Add(this.btn_PrintGTT);
		this.tab_ListInvoiceForPayment.Controls.Add(this.tv_ListInvoiceForPay);
		this.tab_ListInvoiceForPayment.Name = "tab_ListInvoiceForPayment";
		this.tab_ListInvoiceForPayment.Size = new System.Drawing.Size(1077, 428);
		this.tab_ListInvoiceForPayment.Text = "List of Invoice For Payment";
		this.tab_ListInvoiceForPayment.Tooltip = "Danh sách hóa đơn của đơn hàng cần thu tiền ngay";
		this.btn_PrintGTT.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintGTT.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintGTT.ImageOptions.Image");
		this.btn_PrintGTT.Location = new System.Drawing.Point(961, 402);
		this.btn_PrintGTT.Name = "btn_PrintGTT";
		this.btn_PrintGTT.Size = new System.Drawing.Size(109, 23);
		this.btn_PrintGTT.TabIndex = 39;
		this.btn_PrintGTT.Text = "Print Bills";
		this.btn_PrintGTT.ToolTip = "In Giấy giới thiệu kiêm biên lai thu tiền";
		this.btn_PrintGTT.Click += new System.EventHandler(this.btn_PrintGTT_Click);
		this.tv_ListInvoiceForPay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_ListInvoiceForPay.KeyFieldName = "U_ID";
		this.tv_ListInvoiceForPay.Location = new System.Drawing.Point(0, 0);
		this.tv_ListInvoiceForPay.Name = "tv_ListInvoiceForPay";
		this.tv_ListInvoiceForPay.ParentFieldName = "U_FatherId";
		this.tv_ListInvoiceForPay.Size = new System.Drawing.Size(1074, 400);
		this.tv_ListInvoiceForPay.TabIndex = 2;
		this.tv_ListInvoiceForPay.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListInvoiceForPay_CellValueChanging);
		this.txtBusinessPartner.Location = new System.Drawing.Point(323, 31);
		this.txtBusinessPartner.Name = "txtBusinessPartner";
		this.txtBusinessPartner.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtBusinessPartner.Properties.Appearance.Options.UseFont = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txtBusinessPartner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions, DevExpress.XtraEditors.Controls.ButtonPredefines.Search, null)
		});
		this.txtBusinessPartner.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBusinessPartner_Properties_ButtonClick);
		this.txtBusinessPartner.Size = new System.Drawing.Size(140, 18);
		this.txtBusinessPartner.TabIndex = 33;
		this.txtBusinessPartner.ToolTip = "Tìm kiếm theo Khách hàng";
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(235, 5);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(91, 20);
		this.lb_DocStatus.TabIndex = 32;
		this.lb_DocStatus.Utext = "Document Status";
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(235, 31);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(88, 20);
		this.lb_BusinessPartner.TabIndex = 31;
		this.lb_BusinessPartner.Utext = "Business Partner";
		this.lb_BatchNo.HideWhenEmpty = false;
		this.lb_BatchNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BatchNo.Location = new System.Drawing.Point(605, 32);
		this.lb_BatchNo.Name = "lb_BatchNo";
		this.lb_BatchNo.ShowUnderLine = true;
		this.lb_BatchNo.Size = new System.Drawing.Size(88, 20);
		this.lb_BatchNo.TabIndex = 30;
		this.lb_BatchNo.Utext = "Batch No";
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(9, 32);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.TabIndex = 29;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(9, 7);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.TabIndex = 28;
		this.lb_FromDate.Utext = "From Date";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.Location = new System.Drawing.Point(471, 29);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(99, 23);
		this.btnSearch.TabIndex = 27;
		this.btnSearch.Text = "Search";
		this.btnSearch.ToolTip = "Tìm kiếm";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.txt_BatchNo.Location = new System.Drawing.Point(693, 31);
		this.txt_BatchNo.Name = "txt_BatchNo";
		this.txt_BatchNo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_BatchNo.Properties.Appearance.Options.UseFont = true;
		this.txt_BatchNo.Size = new System.Drawing.Size(232, 18);
		this.txt_BatchNo.TabIndex = 26;
		this.txt_BatchNo.ToolTip = "Bắn barcode chọn batch cần giao";
		this.txt_BatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BatchNo_KeyDown);
		this.chb_Status.Location = new System.Drawing.Point(323, 4);
		this.chb_Status.Name = "chb_Status";
		this.chb_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_Status.Size = new System.Drawing.Size(140, 20);
		this.chb_Status.TabIndex = 58;
		this.chb_Status.ToolTip = "Tìm kiếm theo Trạng thái";
		this.txtFromDate.EditValue = null;
		this.txtFromDate.Location = new System.Drawing.Point(89, 4);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Size = new System.Drawing.Size(140, 20);
		this.txtFromDate.TabIndex = 59;
		this.txtFromDate.EditValueChanged += new System.EventHandler(this.txtFromDate_EditValueChanged);
		this.txtToDate.EditValue = null;
		this.txtToDate.Location = new System.Drawing.Point(89, 31);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Size = new System.Drawing.Size(140, 20);
		this.txtToDate.TabIndex = 60;
		this.txtToDate.EditValueChanged += new System.EventHandler(this.txtToDate_EditValueChanged);
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(469, 5);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(63, 20);
		this.labelLine4.TabIndex = 63;
		this.labelLine4.Utext = "Warehouse";
		this.chb_Whs.Location = new System.Drawing.Point(536, 2);
		this.chb_Whs.Name = "chb_Whs";
		this.chb_Whs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_Whs.Size = new System.Drawing.Size(318, 20);
		this.chb_Whs.TabIndex = 62;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1083, 511);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.chb_Whs);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.chb_Status);
		base.Controls.Add(this.tab_DeliveryControl);
		base.Controls.Add(this.txtBusinessPartner);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_BusinessPartner);
		base.Controls.Add(this.lb_BatchNo);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txt_BatchNo);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_DeliveryControl";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Delivery Control";
		base.Load += new System.EventHandler(this.frm_DeliveryControl_Load);
		((System.ComponentModel.ISupportInitialize)this.tab_DeliveryControl).EndInit();
		this.tab_DeliveryControl.ResumeLayout(false);
		this.tab_ListOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_DeliveryControl).EndInit();
		this.tab_ListProctionOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_ListPO).EndInit();
		this.tab_ListDelivery.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListOfDelivery).EndInit();
		this.tab_ListInvoice.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListOfInvoice).EndInit();
		this.tab_ListInvoiceForPayment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListInvoiceForPay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Whs.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
