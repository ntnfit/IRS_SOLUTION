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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
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
using Apzon.Forms.Interfaces.ChooseFromList;
using Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using SAPbobsCOM;

namespace Apzon.Forms.Sales;

public class frm_Atp : XtraFormEx
{
	private string _privilege = "";

	public bool ExpandAll = true;

	public DataTable UdoTable = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lb_FromDate;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine lb_ToDate;

	private LabelLine lb_BusinessPartner;

	private ButtonEditEx bte_OrderNo;

	private LabelLine lb_SalesOrder;

	private DateEditEx date_Delivery;

	private LabelLine lb_DeliveryDate;

	private LabelLine lb_Warehouse;

	private TreeList tv_Atp;

	private TreeListColumnEx tlc_ItemNo;

	private TreeListColumnEx tlc_ItemDescription;

	private TreeListColumnEx tlc_Document;

	private TreeListColumnEx tlc_ObjType;

	private TreeListColumnEx tlc_CardCode;

	private TreeListColumnEx tlc_OrderDate;

	private TreeListColumnEx tlc_DelDate;

	private TreeListColumnEx tlc_Orderd;

	private TreeListColumnEx tlc_OnCommitted;

	private TreeListColumnEx tlc_Available;

	private TreeListColumnEx tlc_UoM;

	private TreeListColumnEx tlc_WhsCode;

	private TreeListColumnEx lb_ID;

	private TreeListColumnEx tlc_FatherId;

	private RepCflWarehouse rep_WhsCode;

	private TreeListColumnEx tlc_Batch;

	private CflCustomer bte_CardCode;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private TreeListColumnEx tlc_FromWarehouse;

	private TreeListColumnEx tlc_InfoPrice;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Search;

	private SimpleButtonEx btn_Expand;

	private SimpleButtonEx btn_SaveTransfer;

	private LabelLine labelLine1;

	private CflWarehouse bte_WhsCode;

	private CflWarehouse bte_ItemCode;

	private TreeListColumnEx treeListColumn1;

	private RepositoryItemLookUpEdit rep_SlpCode;

	public int OrderNo { get; set; }

	public int OrderEntry { get; set; }

	public frm_Atp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Atp"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Atp" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			rep_WhsCode.TreeList = tv_Atp;
			rep_WhsCode.FieldName = "FromWarehouse";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_Atp_Load(object sender, EventArgs e)
	{
		try
		{
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OATP"
			});
			LoadRepositorySource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			await SearchData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void tv_Atp_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.ToUpper().Equals("AVAILABLE"))
			{
				double num = Function.ParseDouble(e.Node.GetValue(e.Column));
				int num2 = Function.ParseInt(e.Node.GetValue("ObjType"));
				if (num < 0.0 && num2 == 17)
				{
					e.Appearance.ForeColor = Color.Red;
				}
			}
			switch (Function.ParseInt(e.Node.GetValue("ObjType")))
			{
			case -1:
				e.Appearance.BackColor = Color.Yellow;
				break;
			case -3:
				e.Appearance.BackColor = Color.MediumSpringGreen;
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Expand_Click(object sender, EventArgs e)
	{
		try
		{
			if (ExpandAll)
			{
				tv_Atp.CollapseAll();
				ExpandAll = false;
				return;
			}
			for (int i = 0; i < tv_Atp.Nodes.Count; i++)
			{
				ExpandAllNode(tv_Atp.Nodes[i]);
				ExpandAll = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (btn_Save.Text.Equals(ControlText.ButtonOk))
			{
				Close();
				return;
			}
			DataTable source = (DataTable)tv_Atp.DataSource;
			List<DataRow> dr = (from t in source.AsEnumerable()
				where Function.ParseInt(t["ObjType"]) == -1 && Math.Abs(Function.ParseDouble(t["Orderd"])) > 0.0
				select t).ToList();
			if (dr.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreatePurchaseRepuest, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DocumentsImpl doc = new DocumentsImpl
				{
					DocDate = DateTime.Today,
					DocDueDate = DateTime.Today,
					VatDate = DateTime.Today,
					ReqDate = DateTime.Today
				};
				bool flag = false;
				doc.Lines = new List<Document_LinesImpl>();
				int i;
				for (i = 0; i < dr.Count; i++)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ID"]) == Function.ParseInt(dr[i]["FatherId"]));
					if (dataRow != null)
					{
						document_LinesImpl.ItemCode = Function.ToString(dataRow["ItemCode"]);
						document_LinesImpl.Dscription = Function.ToString(dataRow["Dscription"]);
					}
					DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FatherId"]) == Function.ParseInt(dr[i]["FatherId"]) && Function.ParseInt(t["ObjType"]) == 17);
					if (!flag && dataRow2 != null)
					{
						doc.UserFields = new List<UserFieldsImpl>
						{
							new UserFieldsImpl
							{
								Name = "U_SoDocNo",
								Type = BoFieldTypes.db_Alpha,
								Value = OrderEntry
							}
						};
						flag = true;
					}
					document_LinesImpl.PQTReqDate = Function.ParseDateTime(dr[i]["DelDate"]) ?? DateTime.Today;
					document_LinesImpl.Quantity = Function.ParseDouble(dr[i]["Orderd"]);
					document_LinesImpl.WhsCode = Function.ToString(dr[i]["WhsCode"]);
					document_LinesImpl.PriceBefDi = Function.ParseDouble(dr[i]["InfoPrice"]);
					document_LinesImpl.SlpCode = Function.ParseInt(dr[i]["SlpCode"]);
					document_LinesImpl.LineNum = i;
					doc.Lines.Add(document_LinesImpl);
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					doc.DocDueDate = doc.Lines.Min((Document_LinesImpl t) => t.PQTReqDate);
				}
				HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseRequest");
				if (!(await doc.GetApprovalTemplate(dtClientHelper, "3", this)))
				{
					WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
					WinCommons._CloseProcessing();
					return;
				}
				HttpResult httpResult = await SaveData(doc);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					await SaveUdo(Function.ToString(1470000113));
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void tv_Atp_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_privilege == "F")
			{
				btn_Save.Enabled = true;
				btn_SaveTransfer.Enabled = true;
			}
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Atp_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (!btn_Save.Text.Equals(ControlText.ButtonSave))
			{
				e.Cancel = false;
				return;
			}
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				btn_Save_Click(null, null);
				e.Cancel = true;
				break;
			case DialogResult.No:
				btn_Save.Text = ControlText.ButtonOk;
				e.Cancel = false;
				break;
			default:
				e.Cancel = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_SaveTransfer_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dt = (DataTable)tv_Atp.DataSource;
			List<DataRow> list = (from t in dt.AsEnumerable()
				where Function.ParseInt(t["ObjType"]) == -3 && Math.Abs(Function.ParseDouble(t["Orderd"])) > 0.0
				select t).ToList();
			if (list.Count > 0)
			{
				if (list.Where((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["FromWarehouse"]))).ToList().Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ChooseFromWhs, "");
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateTransferRequest, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					var enumerable = from row in list
						group row by row.Field<DateTime>("DelDate") into grp
						select new
						{
							DelDate = grp.Key,
							Items = grp
						};
					foreach (var item2 in enumerable)
					{
						if (item2.Items.ToList().Count <= 0)
						{
							continue;
						}
						DocumentsImpl doc = new DocumentsImpl
						{
							DocDate = item2.DelDate,
							DocDueDate = item2.DelDate,
							VatDate = item2.DelDate,
							ObjType = 1250000001,
							Lines = new List<Document_LinesImpl>()
						};
						bool flag = false;
						foreach (DataRow item in item2.Items.ToList())
						{
							Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
							DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ID"]) == Function.ParseInt(item["FatherId"]));
							DataRow dataRow2 = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FatherId"]) == Function.ParseInt(item["FatherId"]) && Function.ParseInt(t["ObjType"]) == 17);
							if (!flag && dataRow2 != null)
							{
								doc.UserFields = new List<UserFieldsImpl>
								{
									new UserFieldsImpl
									{
										Name = "U_SoDocNo",
										Type = BoFieldTypes.db_Alpha,
										Value = OrderEntry
									}
								};
								flag = true;
							}
							if (dataRow != null)
							{
								document_LinesImpl.ItemCode = Function.ToString(dataRow["ItemCode"]);
								document_LinesImpl.Dscription = Function.ToString(dataRow["Dscription"]);
							}
							document_LinesImpl.Quantity = Function.ParseDouble(item["Orderd"]);
							document_LinesImpl.WhsCode = Function.ToString(item["WhsCode"]);
							document_LinesImpl.FromWhsCod = Function.ToString(item["FromWarehouse"]);
							doc.Lines.Add(document_LinesImpl);
						}
						using (HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TranferRequest"))
						{
							if (!(await doc.GetApprovalTemplate(dtClientHelper, "3", this)))
							{
								WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
								WinCommons._CloseProcessing();
								return;
							}
						}
						HttpResult httpResult = await SaveInv(doc);
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							continue;
						}
						await SaveUdo(Function.ToString(1250000001));
						WinCommons.Success(Enums.MessageCode.Success, "");
						btn_Search_Click(null, null);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void bte_WhsCode_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: true, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				List<object> chooseItems = frm_ChooseFromList2.ChooseItems;
				if (chooseItems != null && chooseItems.Count > 0)
				{
					string text = "";
					for (int i = 0; i < chooseItems.Count; i++)
					{
						text = text + "," + ((DataRow)(dynamic)chooseItems[i])["WhsCode"];
					}
					bte_WhsCode.EditValue = ((text != "") ? text.Remove(0, 1) : bte_WhsCode.EditValue);
				}
			}
			if (e.Button.Index == 1)
			{
				string text2 = Function.ToString(bte_WhsCode.EditValue).Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oWarehouses, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_ItemCode_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				if (chooseItems != null && chooseItems.Count > 0)
				{
					string text = "";
					for (int i = 0; i < chooseItems.Count; i++)
					{
						text = text + "," + Function.ToString(chooseItems[i]["ItemCode"]);
					}
					bte_ItemCode.EditValue = ((text != "") ? text.Remove(0, 1) : bte_WhsCode.EditValue);
				}
			}
			if (e.Button.Index == 1)
			{
				string text2 = Function.ToString(bte_ItemCode.EditValue);
				if (!string.IsNullOrEmpty(text2))
				{
					this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text2);
				}
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
		Text = Resources.Titel_frm_Atp;
		lb_FromDate.Utext = Resources.Doc_lb_FromDate;
		lb_ToDate.Utext = Resources.Doc_lb_ToDate;
		lb_BusinessPartner.Utext = Resources.Doc_lb_BusinessPartner;
		lb_SalesOrder.Utext = Resources.lb_SalesOrder;
		lb_Warehouse.Utext = Resources.lb_Warehouse;
		lb_DeliveryDate.Utext = Resources.lb_DeliveryDate;
		btn_Search.Text = Resources.ButtonSearch;
		btn_Save.Text = Resources.ButtonCreatePurchaseRequest;
		btn_SaveTransfer.Text = Resources.ButtonCreateTransferRequest;
		btn_Expand.Text = Resources.ButtonExpandCollapseAll;
		btn_Cancel.Text = Resources.ButtonCancel;
		tlc_ItemNo.Caption = Resources.frmAtp_tlc_ItemCode;
		tlc_ItemDescription.Caption = Resources.frmAtp_tlc_ItemDescription;
		tlc_Document.Caption = Resources.frmAtp_tlc_Document;
		tlc_CardCode.Caption = Resources.frmAtp_tlc_CardCode;
		tlc_OrderDate.Caption = Resources.frmAtp_tlc_OrderDate;
		tlc_DelDate.Caption = Resources.frmAtp_tlc_DelDate;
		tlc_Orderd.Caption = Resources.frmAtp_tlc_Orderd;
		tlc_OnCommitted.Caption = Resources.frmAtp_tlc_OnCommitted;
		tlc_Available.Caption = Resources.frmAtp_tlc_Available;
		tlc_WhsCode.Caption = Resources.Line_gvLine_gcWhseCode;
		tlc_FromWarehouse.Caption = Resources.Line_gvLine_gcFromWhs;
	}

	private async void LoadRepositorySource()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			DataTable dataTable = await helper.GetDataTableAsync("ShowSaleEmployee");
			if (dataTable.IsNotNull())
			{
				rep_SlpCode.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<HttpResult> SaveInv(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TranferRequest");
		return await help.PostAsync("SaveDoc", doc);
	}

	private async Task<HttpResult> SaveData(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PurchaseRequest");
		return await help.PostAsync("SaveDoc", doc);
	}

	private async Task SearchData()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("fDate", typeof(DateTime));
		dataTable.Columns.Add("tDate", typeof(DateTime));
		dataTable.Columns.Add("cardCode", typeof(string));
		dataTable.Columns.Add("whsCode", typeof(string));
		dataTable.Columns.Add("itemCode", typeof(string));
		dataTable.Columns.Add("OrderNo", typeof(int));
		dataTable.Columns.Add("delDate", typeof(DateTime));
		DataRow dataRow = dataTable.NewRow();
		dataRow["fDate"] = Function.ParseDateTime(date_From.EditValue) ?? new DateTime(1900, 1, 1);
		dataRow["tDate"] = Function.ParseDateTime(date_To.EditValue) ?? new DateTime(1900, 1, 1);
		dataRow["cardCode"] = Function.ToString(bte_CardCode.EditValue);
		dataRow["whsCode"] = Function.ToString(bte_WhsCode.EditValue);
		dataRow["itemCode"] = Function.ToString(bte_ItemCode.EditValue);
		dataRow["OrderNo"] = Function.ParseInt(bte_OrderNo.EditValue);
		dataRow["delDate"] = Function.ParseDateTime(date_Delivery.EditValue) ?? new DateTime(1900, 1, 1);
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		DataSet dataSet;
		using (HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Atp"))
		{
			dataSet = httpClientHelper.Post("SearchData", dataTable);
		}
		if (dataSet.Tables.Count > 1)
		{
			DataTable dataTable2 = dataSet.Tables[0];
			dataTable2.ChangeColumnDataType("OrderDate", typeof(DateTime));
			dataTable2.ChangeColumnDataType("DelDate", typeof(DateTime));
			dataTable2.ChangeColumnDataType("Orderd", typeof(double));
			dataTable2.ChangeColumnDataType("OnCommitted", typeof(double));
			dataTable2.ChangeColumnDataType("Available", typeof(double));
			tv_Atp.DataSource = dataTable2;
			tv_Atp.RefreshDataSource();
			OrderEntry = Function.ParseInt(dataSet.Tables[1].Rows[0][0]);
		}
		for (int i = 0; i < tv_Atp.Nodes.Count; i++)
		{
			ExpandAllNode(tv_Atp.Nodes[i]);
		}
		await SaveUdo("");
		btn_Save.Enabled = false;
		btn_SaveTransfer.Enabled = false;
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task SaveUdo(string type)
	{
		try
		{
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			DataSet ds = new DataSet
			{
				Tables = { UdoTable.Copy() }
			};
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("U_FromDate", typeof(DateTime));
			dataTable.Columns.Add("U_ToDate", typeof(DateTime));
			dataTable.Columns.Add("U_CardCode", typeof(string));
			dataTable.Columns.Add("U_WhsCode", typeof(string));
			dataTable.Columns.Add("U_OrderNo", typeof(int));
			dataTable.Columns.Add("U_DelDate", typeof(DateTime));
			dataTable.Columns.Add("U_ItemCode", typeof(string));
			dataTable.Columns.Add("Remark", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["U_FromDate"] = Function.ParseDateTimes(date_From.EditValue);
			dataRow["U_ToDate"] = Function.ParseDateTimes(date_To.EditValue);
			dataRow["U_CardCode"] = Function.ToString(bte_CardCode.EditValue);
			dataRow["U_WhsCode"] = Function.ToString(bte_WhsCode.EditValue);
			dataRow["U_OrderNo"] = Function.ParseInt(bte_OrderNo.EditValue);
			dataRow["U_Deldate"] = Function.ParseDateTimes(date_Delivery.EditValue);
			dataRow["U_ItemCode"] = Function.ToString(bte_ItemCode.EditValue);
			dataRow["Remark"] = type;
			dataTable.Rows.Add(dataRow);
			ds.Tables.Add(dataTable.Copy());
			DataTable dt = new DataTable("ATP1");
			dt.Columns.Add("U_ID", typeof(int));
			dt.Columns.Add("U_ROW_NUM", typeof(int));
			dt.Columns.Add("U_ItemCode", typeof(string));
			dt.Columns.Add("U_Dscription", typeof(string));
			dt.Columns.Add("U_Document", typeof(string));
			dt.Columns.Add("U_ObjType", typeof(string));
			dt.Columns.Add("U_CardCode", typeof(string));
			dt.Columns.Add("U_Orderd", typeof(double));
			dt.Columns.Add("U_OnCommitted", typeof(double));
			dt.Columns.Add("U_Avaiable", typeof(double));
			dt.Columns.Add("U_UoM", typeof(string));
			dt.Columns.Add("U_WhsCode", typeof(string));
			dt.Columns.Add("U_FatherId", typeof(int));
			dt.Columns.Add("U_Batch", typeof(string));
			dt.Columns.Add("U_FromWarehouse", typeof(string));
			dt.Columns.Add("U_OrderDate", typeof(DateTime));
			dt.Columns.Add("U_DelDate", typeof(DateTime));
			dt.Columns.Add("U_InfoPrice", typeof(double));
			dt.Columns.Add("U_SlpCode", typeof(int));
			for (int i = 0; i < tv_Atp.Nodes.Count; i++)
			{
				BindDataNode(tv_Atp.Nodes[i], ref dt);
			}
			ds.Tables.Add(dt.Copy());
			HttpResult httpResult = await _httpClient.Udo.Save(ds);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			ds.Tables.Clear();
			ds.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindDataNode(TreeListNode node, ref DataTable dt)
	{
		DataRow dataRow = dt.NewRow();
		dataRow["U_ID"] = Function.ParseInt(node.GetValue("ID"));
		dataRow["U_ROW_NUM"] = Function.ParseInt(node.GetValue("ROW_NUM"));
		dataRow["U_ItemCode"] = Function.ToString(node.GetValue("ItemCode"));
		dataRow["U_Dscription"] = Function.ToString(node.GetValue("Dscription"));
		dataRow["U_Document"] = Function.ToString(node.GetValue("Document"));
		dataRow["U_ObjType"] = Function.ToString(node.GetValue("ObjType"));
		dataRow["U_CardCode"] = Function.ToString(node.GetValue("CardCode"));
		dataRow["U_Orderd"] = Function.ParseDouble(node.GetValue("Orderd"));
		dataRow["U_OnCommitted"] = Function.ParseDouble(node.GetValue("OnCommitted"));
		dataRow["U_Avaiable"] = Function.ParseDouble(node.GetValue("Available"));
		dataRow["U_UoM"] = Function.ToString(node.GetValue("UoM"));
		dataRow["U_WhsCode"] = Function.ToString(node.GetValue("WhsCode"));
		dataRow["U_FatherId"] = Function.ParseInt(node.GetValue("FatherId"));
		dataRow["U_Batch"] = Function.ToString(node.GetValue("Batch"));
		dataRow["U_FromWarehouse"] = Function.ToString(node.GetValue("FromWarehouse"));
		dataRow["U_OrderDate"] = Function.ParseDateTime(node.GetValue("OrderDate")) ?? new DateTime(1900, 1, 1);
		dataRow["U_DelDate"] = Function.ParseDateTime(node.GetValue("DelDate")) ?? new DateTime(1900, 1, 1);
		dataRow["U_InfoPrice"] = Function.ParseDouble(node.GetValue("InfoPrice"));
		dataRow["U_SlpCode"] = Function.ParseInt(node.GetValue("SlpCode"));
		dt.Rows.Add(dataRow);
		dt.AcceptChanges();
		if (node.Nodes == null || node.Nodes.Count <= 0)
		{
			return;
		}
		foreach (TreeListNode node2 in node.Nodes)
		{
			BindDataNode(node2, ref dt);
		}
	}

	public void ExpandAllNode(TreeListNode oParentNode)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_Atp));
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.bte_CardCode = new Apzon.Forms.Interfaces.ChooseFromList.CflCustomer();
		this.bte_OrderNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_SalesOrder = new Apzon.Controls.Controls.LabelLine();
		this.date_Delivery = new Apzon.Controls.Editor.DateEditEx();
		this.lb_DeliveryDate = new Apzon.Controls.Controls.LabelLine();
		this.bte_WhsCode = new Apzon.Forms.Interfaces.ChooseFromList.CflWarehouse();
		this.lb_Warehouse = new Apzon.Controls.Controls.LabelLine();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_Atp = new DevExpress.XtraTreeList.TreeList();
		this.tlc_ItemNo = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_ItemDescription = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Document = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_ObjType = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_CardCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_OrderDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.tlc_DelDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.tlc_InfoPrice = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Orderd = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_OnCommitted = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Available = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_UoM = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_WhsCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_WhsCode = new Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem.RepCflWarehouse();
		this.lb_ID = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_FatherId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Batch = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_FromWarehouse = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_SlpCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Expand = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SaveTransfer = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_ItemCode = new Apzon.Forms.Interfaces.ChooseFromList.CflWarehouse();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OrderNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Delivery.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Delivery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Atp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SlpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).BeginInit();
		base.SuspendLayout();
		this.lb_FromDate.Location = new System.Drawing.Point(0, 12);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.TabIndex = 0;
		this.lb_FromDate.Utext = "From Date";
		this.date_From.EditValue = null;
		this.date_From.Location = new System.Drawing.Point(79, 9);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Size = new System.Drawing.Size(100, 20);
		this.date_From.TabIndex = 1;
		this.date_To.EditValue = null;
		this.date_To.Location = new System.Drawing.Point(79, 33);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Size = new System.Drawing.Size(100, 20);
		this.date_To.TabIndex = 3;
		this.lb_ToDate.Location = new System.Drawing.Point(0, 36);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.TabIndex = 2;
		this.lb_ToDate.Utext = "To Date";
		this.lb_BusinessPartner.Location = new System.Drawing.Point(186, 12);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.Size = new System.Drawing.Size(100, 20);
		this.lb_BusinessPartner.TabIndex = 4;
		this.lb_BusinessPartner.Utext = "Business Partner";
		this.bte_CardCode.Location = new System.Drawing.Point(285, 10);
		this.bte_CardCode.Name = "bte_CardCode";
		this.bte_CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_CardCode.Size = new System.Drawing.Size(100, 20);
		this.bte_CardCode.TabIndex = 5;
		this.bte_OrderNo.Location = new System.Drawing.Point(285, 33);
		this.bte_OrderNo.Name = "bte_OrderNo";
		this.bte_OrderNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_OrderNo.Size = new System.Drawing.Size(100, 20);
		this.bte_OrderNo.TabIndex = 7;
		this.lb_SalesOrder.Location = new System.Drawing.Point(186, 36);
		this.lb_SalesOrder.Name = "lb_SalesOrder";
		this.lb_SalesOrder.Size = new System.Drawing.Size(100, 20);
		this.lb_SalesOrder.TabIndex = 6;
		this.lb_SalesOrder.Utext = "Sales Order";
		this.date_Delivery.EditValue = null;
		this.date_Delivery.Location = new System.Drawing.Point(470, 30);
		this.date_Delivery.Name = "date_Delivery";
		this.date_Delivery.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Delivery.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Delivery.Size = new System.Drawing.Size(100, 20);
		this.date_Delivery.TabIndex = 9;
		this.lb_DeliveryDate.Location = new System.Drawing.Point(391, 33);
		this.lb_DeliveryDate.Name = "lb_DeliveryDate";
		this.lb_DeliveryDate.Size = new System.Drawing.Size(80, 20);
		this.lb_DeliveryDate.TabIndex = 8;
		this.lb_DeliveryDate.Utext = "Delivery Date";
		this.bte_WhsCode.Location = new System.Drawing.Point(470, 7);
		this.bte_WhsCode.Name = "bte_WhsCode";
		this.bte_WhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_WhsCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_WhsCode_Properties_ButtonPressed);
		this.bte_WhsCode.Size = new System.Drawing.Size(100, 20);
		this.bte_WhsCode.TabIndex = 11;
		this.lb_Warehouse.Location = new System.Drawing.Point(391, 9);
		this.lb_Warehouse.Name = "lb_Warehouse";
		this.lb_Warehouse.Size = new System.Drawing.Size(100, 20);
		this.lb_Warehouse.TabIndex = 10;
		this.lb_Warehouse.Utext = "Warehouse";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Image = (System.Drawing.Image)resources.GetObject("btn_Save.Image");
		this.btn_Save.Location = new System.Drawing.Point(1, 406);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(153, 23);
		this.btn_Save.TabIndex = 13;
		this.btn_Save.Text = "Create Purchase Request";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(326, 406);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.tv_Atp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		DevExpress.XtraTreeList.Columns.TreeListColumnCollection columns = this.tv_Atp.Columns;
		DevExpress.XtraTreeList.Columns.TreeListColumn[] columns2 = new Apzon.Controls.Tree.TreeListColumnEx[18]
		{
			this.tlc_ItemNo, this.tlc_ItemDescription, this.tlc_Document, this.tlc_ObjType, this.tlc_CardCode, this.tlc_OrderDate, this.tlc_DelDate, this.tlc_InfoPrice, this.tlc_Orderd, this.tlc_OnCommitted,
			this.tlc_Available, this.tlc_UoM, this.tlc_WhsCode, this.lb_ID, this.tlc_FatherId, this.tlc_Batch, this.tlc_FromWarehouse, this.treeListColumn1
		};
		columns.AddRange(columns2);
		this.tv_Atp.Location = new System.Drawing.Point(1, 56);
		this.tv_Atp.Name = "tv_Atp";
		this.tv_Atp.OptionsBehavior.EnableFiltering = true;
		this.tv_Atp.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Matches;
		this.tv_Atp.OptionsFilter.ShowAllValuesInFilterPopup = true;
		this.tv_Atp.OptionsView.AutoWidth = false;
		this.tv_Atp.ParentFieldName = "FatherId";
		this.tv_Atp.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_WhsCode, this.repositoryItemDateEdit1, this.repositoryItemDateEdit2, this.rep_SlpCode });
		this.tv_Atp.Size = new System.Drawing.Size(919, 344);
		this.tv_Atp.TabIndex = 15;
		this.tv_Atp.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_Atp_NodeCellStyle);
		this.tv_Atp.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Atp_CellValueChanged);
		this.tlc_ItemNo.Caption = "Item No.";
		this.tlc_ItemNo.FieldName = "ItemCode";
		this.tlc_ItemNo.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
		this.tlc_ItemNo.Name = "tlc_ItemNo";
		this.tlc_ItemNo.OptionsColumn.AllowSort = false;
		this.tlc_ItemNo.OptionsColumn.ReadOnly = true;
		this.tlc_ItemNo.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_ItemNo.Visible = true;
		this.tlc_ItemNo.VisibleIndex = 0;
		this.tlc_ItemNo.Width = 100;
		this.tlc_ItemDescription.Caption = "Item Description";
		this.tlc_ItemDescription.FieldName = "Dscription";
		this.tlc_ItemDescription.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
		this.tlc_ItemDescription.Name = "tlc_ItemDescription";
		this.tlc_ItemDescription.OptionsColumn.AllowSort = false;
		this.tlc_ItemDescription.OptionsColumn.ReadOnly = true;
		this.tlc_ItemDescription.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_ItemDescription.Visible = true;
		this.tlc_ItemDescription.VisibleIndex = 1;
		this.tlc_ItemDescription.Width = 100;
		this.tlc_Document.Caption = "Document";
		this.tlc_Document.FieldName = "Document";
		this.tlc_Document.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
		this.tlc_Document.Name = "tlc_Document";
		this.tlc_Document.OptionsColumn.AllowSort = false;
		this.tlc_Document.OptionsColumn.ReadOnly = true;
		this.tlc_Document.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_Document.Visible = true;
		this.tlc_Document.VisibleIndex = 2;
		this.tlc_Document.Width = 100;
		this.tlc_ObjType.Caption = "ObjType";
		this.tlc_ObjType.FieldName = "ObjType";
		this.tlc_ObjType.Name = "tlc_ObjType";
		this.tlc_ObjType.OptionsColumn.AllowSort = false;
		this.tlc_ObjType.OptionsColumn.ReadOnly = true;
		this.tlc_ObjType.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_ObjType.Width = 100;
		this.tlc_CardCode.Caption = "Customer/Vendor";
		this.tlc_CardCode.FieldName = "CardCode";
		this.tlc_CardCode.Name = "tlc_CardCode";
		this.tlc_CardCode.OptionsColumn.AllowSort = false;
		this.tlc_CardCode.OptionsColumn.ReadOnly = true;
		this.tlc_CardCode.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_CardCode.Visible = true;
		this.tlc_CardCode.VisibleIndex = 3;
		this.tlc_CardCode.Width = 100;
		this.tlc_OrderDate.Caption = "Order Date";
		this.tlc_OrderDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.tlc_OrderDate.FieldName = "OrderDate";
		this.tlc_OrderDate.Format.FormatString = "d";
		this.tlc_OrderDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.tlc_OrderDate.Name = "tlc_OrderDate";
		this.tlc_OrderDate.OptionsColumn.AllowSort = false;
		this.tlc_OrderDate.OptionsColumn.ReadOnly = true;
		this.tlc_OrderDate.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_OrderDate.Visible = true;
		this.tlc_OrderDate.VisibleIndex = 4;
		this.tlc_OrderDate.Width = 100;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.tlc_DelDate.Caption = "Delivery Date";
		this.tlc_DelDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.tlc_DelDate.FieldName = "DelDate";
		this.tlc_DelDate.Format.FormatString = "d";
		this.tlc_DelDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.tlc_DelDate.Name = "tlc_DelDate";
		this.tlc_DelDate.OptionsColumn.AllowSort = false;
		this.tlc_DelDate.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_DelDate.Visible = true;
		this.tlc_DelDate.VisibleIndex = 5;
		this.tlc_DelDate.Width = 100;
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.tlc_InfoPrice.Caption = "InfoPrice";
		this.tlc_InfoPrice.FieldName = "InfoPrice";
		this.tlc_InfoPrice.Format.FormatString = "n3";
		this.tlc_InfoPrice.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.tlc_InfoPrice.Name = "tlc_InfoPrice";
		this.tlc_InfoPrice.OptionsColumn.AllowSort = false;
		this.tlc_InfoPrice.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_InfoPrice.Visible = true;
		this.tlc_InfoPrice.VisibleIndex = 6;
		this.tlc_Orderd.Caption = "Ordered";
		this.tlc_Orderd.FieldName = "Orderd";
		this.tlc_Orderd.Format.FormatString = "n3";
		this.tlc_Orderd.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.tlc_Orderd.Name = "tlc_Orderd";
		this.tlc_Orderd.OptionsColumn.AllowSort = false;
		this.tlc_Orderd.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_Orderd.Visible = true;
		this.tlc_Orderd.VisibleIndex = 7;
		this.tlc_Orderd.Width = 100;
		this.tlc_OnCommitted.Caption = "Committed";
		this.tlc_OnCommitted.FieldName = "OnCommitted";
		this.tlc_OnCommitted.Format.FormatString = "n3";
		this.tlc_OnCommitted.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.tlc_OnCommitted.Name = "tlc_OnCommitted";
		this.tlc_OnCommitted.OptionsColumn.AllowSort = false;
		this.tlc_OnCommitted.OptionsColumn.ReadOnly = true;
		this.tlc_OnCommitted.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_OnCommitted.Visible = true;
		this.tlc_OnCommitted.VisibleIndex = 8;
		this.tlc_OnCommitted.Width = 100;
		this.tlc_Available.Caption = "Available";
		this.tlc_Available.FieldName = "Available";
		this.tlc_Available.Format.FormatString = "n3";
		this.tlc_Available.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.tlc_Available.Name = "tlc_Available";
		this.tlc_Available.OptionsColumn.AllowSort = false;
		this.tlc_Available.OptionsColumn.ReadOnly = true;
		this.tlc_Available.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_Available.Visible = true;
		this.tlc_Available.VisibleIndex = 9;
		this.tlc_Available.Width = 100;
		this.tlc_UoM.Caption = "UoM";
		this.tlc_UoM.FieldName = "UoM";
		this.tlc_UoM.Name = "tlc_UoM";
		this.tlc_UoM.OptionsColumn.AllowSort = false;
		this.tlc_UoM.OptionsColumn.ReadOnly = true;
		this.tlc_UoM.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_UoM.Visible = true;
		this.tlc_UoM.VisibleIndex = 11;
		this.tlc_UoM.Width = 100;
		this.tlc_WhsCode.Caption = "Warehouse";
		this.tlc_WhsCode.ColumnEdit = this.rep_WhsCode;
		this.tlc_WhsCode.FieldName = "WhsCode";
		this.tlc_WhsCode.Name = "tlc_WhsCode";
		this.tlc_WhsCode.OptionsColumn.AllowSort = false;
		this.tlc_WhsCode.OptionsColumn.ReadOnly = true;
		this.tlc_WhsCode.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_WhsCode.Visible = true;
		this.tlc_WhsCode.VisibleIndex = 12;
		this.tlc_WhsCode.Width = 100;
		this.rep_WhsCode.AutoHeight = false;
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_WhsCode.FieldName = null;
		this.rep_WhsCode.GridView = null;
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.TreeList = null;
		this.lb_ID.Caption = "treeListColumn13";
		this.lb_ID.FieldName = "ID";
		this.lb_ID.Name = "lb_ID";
		this.lb_ID.OptionsColumn.AllowSort = false;
		this.lb_ID.OptionsColumn.ReadOnly = true;
		this.lb_ID.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_FatherId.Caption = "treeListColumn14";
		this.tlc_FatherId.FieldName = "FatherId";
		this.tlc_FatherId.Name = "tlc_FatherId";
		this.tlc_FatherId.OptionsColumn.AllowSort = false;
		this.tlc_FatherId.OptionsColumn.ReadOnly = true;
		this.tlc_FatherId.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.tlc_Batch.Caption = "Khách Hàng";
		this.tlc_Batch.FieldName = "Batch";
		this.tlc_Batch.Name = "tlc_Batch";
		this.tlc_Batch.Visible = true;
		this.tlc_Batch.VisibleIndex = 10;
		this.tlc_FromWarehouse.Caption = "From Warehouse";
		this.tlc_FromWarehouse.ColumnEdit = this.rep_WhsCode;
		this.tlc_FromWarehouse.FieldName = "FromWarehouse";
		this.tlc_FromWarehouse.Name = "tlc_FromWarehouse";
		this.tlc_FromWarehouse.Visible = true;
		this.tlc_FromWarehouse.VisibleIndex = 13;
		this.treeListColumn1.Caption = "Buyer";
		this.treeListColumn1.ColumnEdit = this.rep_SlpCode;
		this.treeListColumn1.FieldName = "SlpCode";
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.OptionsColumn.AllowSort = false;
		this.treeListColumn1.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 14;
		this.rep_SlpCode.AutoHeight = false;
		this.rep_SlpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SlpCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.rep_SlpCode.DisplayMember = "SlpName";
		this.rep_SlpCode.Name = "rep_SlpCode";
		this.rep_SlpCode.NullText = "";
		this.rep_SlpCode.ValueMember = "SlpCode";
		this.btn_Search.Image = (System.Drawing.Image)resources.GetObject("btn_Search.Image");
		this.btn_Search.Location = new System.Drawing.Point(576, 30);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(81, 20);
		this.btn_Search.TabIndex = 16;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_Expand.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Expand.Image = (System.Drawing.Image)resources.GetObject("btn_Expand.Image");
		this.btn_Expand.Location = new System.Drawing.Point(747, 406);
		this.btn_Expand.Name = "btn_Expand";
		this.btn_Expand.Size = new System.Drawing.Size(172, 23);
		this.btn_Expand.TabIndex = 17;
		this.btn_Expand.Text = "Expand/Collapse All";
		this.btn_Expand.Click += new System.EventHandler(this.btn_Expand_Click);
		this.btn_SaveTransfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveTransfer.Image = (System.Drawing.Image)resources.GetObject("btn_SaveTransfer.Image");
		this.btn_SaveTransfer.Location = new System.Drawing.Point(160, 406);
		this.btn_SaveTransfer.Name = "btn_SaveTransfer";
		this.btn_SaveTransfer.Size = new System.Drawing.Size(160, 23);
		this.btn_SaveTransfer.TabIndex = 18;
		this.btn_SaveTransfer.Text = "Create Transfer Request";
		this.btn_SaveTransfer.Click += new System.EventHandler(this.btn_SaveTransfer_Click);
		this.bte_ItemCode.Location = new System.Drawing.Point(655, 7);
		this.bte_ItemCode.Name = "bte_ItemCode";
		this.bte_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ItemCode_Properties_ButtonPressed);
		this.bte_ItemCode.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemCode.TabIndex = 20;
		this.labelLine1.Location = new System.Drawing.Point(576, 9);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(100, 20);
		this.labelLine1.TabIndex = 19;
		this.labelLine1.Utext = "Item Code";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(921, 434);
		base.Controls.Add(this.bte_ItemCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_SaveTransfer);
		base.Controls.Add(this.btn_Expand);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.tv_Atp);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.bte_WhsCode);
		base.Controls.Add(this.lb_Warehouse);
		base.Controls.Add(this.date_Delivery);
		base.Controls.Add(this.lb_DeliveryDate);
		base.Controls.Add(this.bte_OrderNo);
		base.Controls.Add(this.lb_SalesOrder);
		base.Controls.Add(this.bte_CardCode);
		base.Controls.Add(this.lb_BusinessPartner);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.lb_FromDate);
		base.Name = "frm_Atp";
		base.ShowIcon = false;
		this.Text = "ATP (Available to Promise)";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Atp_FormClosing);
		base.Load += new System.EventHandler(this.frm_Atp_Load);
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OrderNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Delivery.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Delivery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Atp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SlpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
