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

public class frm_APInvoices : VirFormPurchase
{
	private IContainer components;

	public frm_APInvoices()
	{
		InitComponents();
	}

	public frm_APInvoices(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_APInvoices(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_APInvoices"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseInvoices;
			FormId = ApzObjectTypes.oPurchaseInvoices.ToString();
			FormBpType = "S";
			FormDocumentTable = "OPCH";
			FormLineTable = "PCH1";
			DocFreightTable = "PCH3";
			FormController = "APInvoice";
			base.Text = "A/P Invoice";
			FormLayoutCode = "PCH2";
			FormLayoutCodeService = "PCH1";
			FormObjectType = ApzObjectTypes.oPurchaseInvoices;
			BatchSerialTransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseInvoices;
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
			gv_Accounting.ItemId = "OPCH_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPCH_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseDeliveryNotes
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oGoodsReturnRequest,
				ApzObjectTypes.oPurchaseCreditNotes
			};
			gcDpmTotal.ColumnEdit = rep_PmtToDraw;
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
		this.Text = "frm_APInvoices";
	}
}
