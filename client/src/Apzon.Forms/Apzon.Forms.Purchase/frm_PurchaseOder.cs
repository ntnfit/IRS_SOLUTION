using System;
using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseOder : VirFormPurchase
{
	public frm_PurchaseOder()
	{
		InitComponents();
	}

	public frm_PurchaseOder(ApzObjectTypes fromtype, int fromId)
		: base(fromtype, fromId)
	{
		InitComponents();
	}

	public frm_PurchaseOder(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	private void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseOder"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseOrders;
			FormId = ApzObjectTypes.oPurchaseOrders.ToString();
			FormBpType = "S";
			FormDocumentTable = "OPOR";
			FormLineTable = "POR1";
			DocFreightTable = "POR3";
			FormController = "PurchaseOder";
			Text = "Purchase Order";
			FormLayoutCode = "POR2";
			FormLayoutCodeService = "POR1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseOrders);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.Allocate;
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
			gv_Freight.ItemId = DocFreightTable;
			gv_Accounting.ItemId = "OPOR_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPOR_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			gcConfirmed.Visible = true;
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oBlanketAgreement
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oAPReserveInvoice,
				ApzObjectTypes.oPurchaseDownPayments,
				ApzObjectTypes.oPurchaseDownPaymentInvoice
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}
}
