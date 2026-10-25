using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;

namespace Apzon.Forms.Purchase;

public class frm_GoodReceiptPO : VirFormPurchase
{
	private IContainer components;

	public frm_GoodReceiptPO(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_GoodReceiptPO()
	{
		InitComponents();
	}

	public frm_GoodReceiptPO(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_GoodReceiptPO"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseDeliveryNotes;
			FormId = ApzObjectTypes.oPurchaseDeliveryNotes.ToString();
			FormBpType = "S";
			FormDocumentTable = "OPDN";
			DocFreightTable = "PDN3";
			FormLineTable = "PDN1";
			FormController = "GoodReceiptPo";
			base.Text = "Goods Receipt PO";
			FormLayoutCode = "PDN2";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseDeliveryNotes);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.In;
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
			gv_Accounting.ItemId = "OPDN_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPDN_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oAPReserveInvoice
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oPurchaseInvoices
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private new void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "frm_GoodReceiptPO";
	}
}
