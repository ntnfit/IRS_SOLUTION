using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseRequest : VirFormPurchase
{
	public frm_PurchaseRequest()
	{
		InitComponents();
	}

	public frm_PurchaseRequest(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public frm_PurchaseRequest(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	private void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseRequest"))
		{
			FormId = ApzObjectTypes.oPurchaseRequest.ToString();
			FormBpType = "S";
			FormDocumentTable = "OPRQ";
			FormLineTable = "PRQ1";
			FormController = "PurchaseRequest";
			Text = "Purchase Request";
			FormLayoutCode = "PRQ2";
			FormLayoutCodeService = "PRQ1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseRequest);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.None;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.None;
			ctm_BatchSerialNumbers.Visible = false;
			GridViewEx gridViewEx = gvDoc;
			GridViewEx gridViewEx2 = gvLine;
			GridViewEx gridViewEx3 = gv_Freight;
			GridViewEx gridViewEx4 = gv_Accounting;
			GridViewEx gridViewEx5 = gvPayTerms;
			GridViewEx gridViewEx6 = gv_Logistic;
			ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
			string text = (ucAttachment1.FormId = FormId);
			string text2 = (ucBatchAndSerial2.FormId = text);
			string text4 = (gridViewEx6.FormId = text2);
			string text6 = (gridViewEx5.FormId = text4);
			string text8 = (gridViewEx4.FormId = text6);
			string text10 = (gridViewEx3.FormId = text8);
			string formId2 = (gridViewEx2.FormId = text10);
			gridViewEx.FormId = formId2;
			gvDoc.ItemId = FormDocumentTable;
			gvLine.ItemId = FormLineTable;
			gv_Freight.ItemId = "Freight";
			gv_Accounting.ItemId = "OPRQ_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPRQ_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders
			};
			GridColumnEx gridColumnEx = gcDoc_CardCode;
			GridColumnEx gridColumnEx2 = gcDoc_CardName;
			bool flag = (gcDoc_CntctCode.Visible = false);
			bool visible = (gridColumnEx2.Visible = flag);
			gridColumnEx.Visible = visible;
			GridColumnEx gridColumnEx3 = gcDoc_ReqType;
			GridColumnEx gridColumnEx4 = gcDoc_ReqName;
			GridColumnEx gridColumnEx5 = gcDoc_Requester;
			GridColumnEx gridColumnEx6 = gcDoc_Department;
			bool flag4 = (gcDoc_Branch.Visible = true);
			bool flag6 = (gridColumnEx6.Visible = flag4);
			flag = (gridColumnEx5.Visible = flag6);
			visible = (gridColumnEx4.Visible = flag);
			gridColumnEx3.Visible = visible;
			gcDoc_ReqType.VisibleIndex = 6;
			gcDoc_ReqName.VisibleIndex = 7;
			gcDoc_Department.VisibleIndex = 8;
			gcDoc_Branch.VisibleIndex = 9;
			gcLine_DelDate.Visible = false;
			gcLinePQTReqdate.Visible = true;
			LabelLine labelLine = lb_BusinessPartner;
			visible = (txtBusinessPartner.Visible = false);
			labelLine.Visible = visible;
			LabelLine labelLine2 = lb_Requester;
			LookUpEditEx lookUpEditEx = look_ReqType;
			flag = (txtRequester.Visible = true);
			visible = (lookUpEditEx.Visible = flag);
			labelLine2.Visible = visible;
			lb_Requester.Location = new Point(lb_DocNumber.Location.X, lb_FromDate.Location.Y);
			lb_Requester.Width = lb_DocNumber.Width;
			look_ReqType.Location = new Point(txtNoOfTrans.Location.X, txtFromDate.Location.Y);
			look_ReqType.Width = txtNoOfTrans.Width;
			txtRequester.Location = new Point(lb_DocStatus.Location.X, txtFromDate.Location.Y);
			XtraTabPageEx xtraTabPageEx = tab_Logistic;
			XtraTabPageEx xtraTabPageEx2 = tabP_Accounting;
			flag = (tab_PaymentTerms.PageVisible = false);
			visible = (xtraTabPageEx2.PageVisible = flag);
			xtraTabPageEx.PageVisible = visible;
			drlCopyFrom.Visible = false;
			gcUnitPrice.Caption = "Info Price";
			gcLine_VendorNum.Visible = true;
			gvDoc.CellValueChanging += gvDoc_CellValueChanging;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column == gcDoc_ReqType)
			{
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Requester", string.Empty);
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "ReqName", string.Empty);
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "ReqName", string.Empty);
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Department", -1);
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Branch", -1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task LoadRequestType()
	{
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "0", " " },
				{ "12", "User" },
				{ "171", "Employee" }
			};
			rep_ReqType.SetDataSource(dic);
			look_ReqType.SetDataSource(dic);
			look_ReqType.Properties.PopulateColumns();
			look_ReqType.Properties.ShowHeader = false;
			if (look_ReqType.Properties.Columns.Count > 0)
			{
				look_ReqType.Properties.Columns[0].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void ChooseFromListRequesterSearch()
	{
		try
		{
			switch (Function.ToString(look_ReqType.EditValue))
			{
			case "12":
			{
				frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.RequesterByUser, multiSelect: false, "");
				frm_ChooseFromList3.ShowDialog(this);
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					DataRow row2 = ((DataRowView)frm_ChooseFromList3.ChooseItem).Row;
					txtRequester.EditValue = Function.ToString(row2["Requester"]);
				}
				break;
			}
			case "171":
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.RequesterByEmployee, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
					txtRequester.EditValue = Function.ToString(row["Requester"]);
				}
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

	protected override void ChooseFromListRequester(string requester)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "ReqType"));
			if (!(text == "12"))
			{
				if (text == "171")
				{
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.RequesterByEmployee, multiSelect: false, requester);
					frm_ChooseFromList2.ShowDialog(this);
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						RequesterImpl requesterImpl = new RequesterImpl();
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						requesterImpl.Requester = Function.ToString(row["EmpID"]);
						requesterImpl.ReqName = Function.ToString(row["ReqName"]);
						requesterImpl.UserSign = Function.ParseInt(row["UserSign"]);
						requesterImpl.Department = Function.ParseInt(row["dept"]);
						requesterImpl.Branch = Function.ParseInt(row["branch"]);
						SetItemToRequesterEmployee(requesterImpl);
					}
				}
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.RequesterByUser, multiSelect: false, requester);
				frm_ChooseFromList3.ShowDialog(this);
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					RequesterImpl requesterImpl2 = new RequesterImpl();
					DataRow row2 = ((DataRowView)frm_ChooseFromList3.ChooseItem).Row;
					requesterImpl2.Requester = Function.ToString(row2["Requester"]);
					requesterImpl2.ReqName = Function.ToString(row2["ReqName"]);
					requesterImpl2.Department = Function.ParseInt(row2["Department"]);
					requesterImpl2.Branch = Function.ParseInt(row2["Branch"]);
					requesterImpl2.UserSign = Function.ParseInt(row2["UserSign"]);
					SetItemToRequester(requesterImpl2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetItemToRequester(RequesterImpl req)
	{
		try
		{
			gvDoc.SetFocusedRowCellValue("Requester", Function.ToString(req.Requester));
			gvDoc.SetFocusedRowCellValue("ReqName", Function.ToString(req.ReqName));
			gvDoc.SetFocusedRowCellValue("Department", Function.ToString(req.Department));
			gvDoc.SetFocusedRowCellValue("Branch", Function.ToString(req.Branch));
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetItemToRequesterEmployee(RequesterImpl req)
	{
		try
		{
			gvDoc.SetFocusedRowCellValue("Requester", req.Requester);
			gvDoc.SetFocusedRowCellValue("ReqName", req.ReqName);
			gvDoc.SetFocusedRowCellValue(gcDoc_Department, req.Department);
			gvDoc.SetFocusedRowCellValue(gcDoc_Branch, req.Branch);
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task GetDepartment()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseRequest");
			DataTable dataSource = await helper.GetDataTableAsync("GetListDepartment");
			rep_Department.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task GetBranch()
	{
		try
		{
			using HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseRequest");
			DataTable dataSource = await help.GetDataTableAsync("GetListBranch");
			rep_Branch.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SettingLanguage()
	{
		base.SettingLanguage();
		try
		{
			if (Type.GetType("Apzon.Commons.Properties.PurchaseQuotation, Apzon.Commons") != null)
			{
				foreach (DictionaryEntry item in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.PurchaseQuotation, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
				{
					string fieldName = item.Key.ToString();
					if (gvDoc.Columns[fieldName] != null)
					{
						gvDoc.Columns[fieldName].Caption = Function.ToString(item.Value);
					}
				}
			}
			if (!(Type.GetType("Apzon.Commons.Properties.PurchaseRequestLine, Apzon.Commons") != null))
			{
				return;
			}
			foreach (DictionaryEntry item2 in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.PurchaseRequestLine, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
			{
				string fieldName2 = item2.Key.ToString();
				if (gvLine.Columns[fieldName2] != null)
				{
					gvLine.Columns[fieldName2].Caption = Function.ToString(item2.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SetDefaultHeaderValueToAddNew(ref DocumentRow doc)
	{
		doc.DocDueDate = DateTime.Now.Date.AddMonths(1);
	}
}
