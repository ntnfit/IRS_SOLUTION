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

internal class frm_PurchaseDownPaymentRequest : VirFormPurchase
{
	public frm_PurchaseDownPaymentRequest()
	{
		InitComponents();
	}

	public frm_PurchaseDownPaymentRequest(ApzObjectTypes fromtype, int fromId)
		: base(fromtype, fromId)
	{
		InitComponents();
	}

	public frm_PurchaseDownPaymentRequest(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	private void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseDownPaymentRequest"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseDownPayments;
			DownPaymentType = Enums.DownPaymentType.dptRequest;
			FormId = "PurchaseDownPaymentRequest";
			FormBpType = "S";
			FormDocumentTable = "ODPO";
			FormLineTable = "DPO1";
			DocFreightTable = "DPO3";
			FormController = "PurchaseDownPayment";
			Text = "A/P Down Payment Request";
			FormLayoutCode = "DPO2";
			FormLayoutCodeService = "DPO1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseDownPayments);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.None;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.None;
			ctm_BatchSerialNumbers.Visible = false;
			GridViewEx gridViewEx = gvDoc;
			GridViewEx gridViewEx2 = gvLine;
			GridViewEx gridViewEx3 = gv_Freight;
			GridViewEx gridViewEx4 = gv_Accounting;
			GridViewEx gridViewEx5 = gvPayTerms;
			ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
			string text = (ucAttachment1.FormId = FormId);
			string text2 = (ucBatchAndSerial2.FormId = text);
			string text4 = (gridViewEx5.FormId = text2);
			string text6 = (gridViewEx4.FormId = text4);
			string text8 = (gridViewEx3.FormId = text6);
			string formId2 = (gridViewEx2.FormId = text8);
			gridViewEx.FormId = formId2;
			gvDoc.ItemId = FormDocumentTable;
			gvLine.ItemId = FormLineTable;
			gv_Freight.ItemId = DocFreightTable;
			gv_Accounting.ItemId = "ODPO_ACT";
			gvPayTerms.ItemId = "@PMT1";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oBlanketAgreement
			};
			PopCopyTo = new List<ApzObjectTypes>();
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}
}
