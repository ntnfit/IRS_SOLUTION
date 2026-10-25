using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterChooseFormListUDF")]
public class RegisterChooseFormListUDF : RepositoryItemButtonEditCFL
{
	public int docType;

	public string fileName;

	public GridView GridView { get; set; }

	public event EventHandler<ChooseFormListUDFEventArgs> SelectedFromUDF;

	public RegisterChooseFormListUDF()
	{
		ButtonPressed += ButtonEditUDF_ButtonPressed;
	}

	public void ButtonEditUDF_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (GridView != null)
			{
				docType = Function.ParseInt(GridView.GetFocusedRowCellValue("U_RelatedTyp"));
			}
			ButtonEditCFL editor = sender as ButtonEditCFL;
			if (e.Button.Index == 0)
			{
				editor.BeginInvoke((Action)(() =>
				{
					ShowCFLUDF(editor, GridView);
				}));
			}
			if (e.Button.Index == 1)
			{
				XtraFormEx baseForm = new XtraFormEx();
				string text = Function.ToString(GridView.GetFocusedRowCellValue(fileName));
				if (!string.IsNullOrEmpty(Function.ToString(text)))
				{
					ApzObjectTypes docTypeByName = GetDocTypeByName(docType);
					baseForm.DrillDown(docTypeByName, docTypeByName, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowCFLUDF(ButtonEditCFL editor, GridView gv)
	{
		if (gv != null)
		{
			docType = Function.ParseInt(gv.GetFocusedRowCellValue("U_RelatedTyp"));
		}
		if (docType != 1470000113 && docType != 1470000145 && docType != 1470000130)
		{
			frm_List_Document frm_List_Document2 = new frm_List_Document(GetDocTypeByName(docType), multiSelect: false, string.Empty, isNoStatus: true);
			frm_List_Document2.ShowDialog();
			if (frm_List_Document2.ChooseItem != null)
			{
				DocumentsImpl chooseItem = frm_List_Document2.ChooseItem;
				gv.SetFocusedRowCellValue("U_RelatedEnt", chooseItem.DocNum);
			}
		}
		else if (docType == 1470000113)
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ListPurchaseRequest, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog();
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				dynamic chooseItem2 = frm_ChooseFromList2.ChooseItem;
				gv.SetFocusedRowCellValue("U_RelatedEnt", chooseItem2.DocNum);
			}
		}
		else if (docType == 1470000145)
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MarketingCampaign, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog();
			if (frm_CFL2.ChooseItem != null)
			{
				gv.SetFocusedRowCellValue("U_RelatedEnt", frm_CFL2.ChooseItem["DocEntry"]);
			}
		}
		else if (docType == 1470000130)
		{
			frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.TrackingIssue, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL3.ShowDialog();
			if (frm_CFL3.ChooseItem != null)
			{
				gv.SetFocusedRowCellValue("U_RelatedEnt", frm_CFL3.ChooseItem["Id"]);
			}
		}
	}

	private ApzObjectTypes GetDocTypeByName(int docName)
	{
		try
		{
			ApzObjectTypes result = ApzObjectTypes.oSalesOpportunities;
			switch (docName)
			{
			case 23:
				result = ApzObjectTypes.oQuotations;
				break;
			case 17:
				result = ApzObjectTypes.oOrders;
				break;
			case 15:
				result = ApzObjectTypes.oDeliveryNotes;
				break;
			case 16:
				result = ApzObjectTypes.oReturns;
				break;
			case 13:
				result = ApzObjectTypes.oInvoices;
				break;
			case 14:
				result = ApzObjectTypes.oCreditNotes;
				break;
			case 203:
				result = ApzObjectTypes.oDownPayments;
				break;
			case 1470000113:
				result = ApzObjectTypes.oPurchaseRequest;
				break;
			case 540000006:
				result = ApzObjectTypes.oPurchaseQuotations;
				break;
			case 22:
				result = ApzObjectTypes.oPurchaseOrders;
				break;
			case 20:
				result = ApzObjectTypes.oPurchaseDeliveryNotes;
				break;
			case 21:
				result = ApzObjectTypes.oPurchaseReturns;
				break;
			case 18:
				result = ApzObjectTypes.oPurchaseInvoices;
				break;
			case 19:
				result = ApzObjectTypes.oPurchaseCreditNotes;
				break;
			case 48:
				result = ApzObjectTypes.oLandedCostsCodes;
				break;
			case 204:
				result = ApzObjectTypes.oPurchaseDownPayments;
				break;
			case 24:
				result = ApzObjectTypes.oIncomingPayments;
				break;
			case 46:
				result = ApzObjectTypes.oVendorPayments;
				break;
			case 1250000001:
				result = ApzObjectTypes.oInventoryTransferRequest;
				break;
			case 67:
				result = ApzObjectTypes.oStockTransfer;
				break;
			case 60:
				result = ApzObjectTypes.oInventoryGenExit;
				break;
			case 59:
				result = ApzObjectTypes.oInventoryGenEntry;
				break;
			case 162:
				result = ApzObjectTypes.oMaterialRevaluation;
				break;
			case 202:
				result = ApzObjectTypes.oProductionOrders;
				break;
			case 1250000025:
				result = ApzObjectTypes.oBlanketAgreement;
				break;
			case 1470000130:
				result = ApzObjectTypes.oTrackingIssue;
				break;
			case 191:
				result = ApzObjectTypes.oServiceCalls;
				break;
			case 1470000145:
				result = ApzObjectTypes.oMarketingCampaign;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ApzObjectTypes.oSalesOpportunities;
		}
	}

	public override void OnSelectedFromCFL(ButtonEditCFL editor, DataRow chooseItem = null, List<DataRow> chooseItems = null, GridView view = null, frm_CFL formCFL = null)
	{
		ChooseFormListUDFEventArgs e = new ChooseFormListUDFEventArgs(chooseItem, chooseItems, view, formCFL);
		if (SelectedFromUDF != null)
		{
			SelectedFromUDF(editor, e);
		}
	}
}
