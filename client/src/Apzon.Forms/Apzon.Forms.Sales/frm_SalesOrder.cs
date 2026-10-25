using System;
using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;

namespace Apzon.Forms.Sales;

public class frm_SalesOrder : VirFormSales
{
	public frm_SalesOrder()
	{
		InitComponents();
	}

	public frm_SalesOrder(ApzObjectTypes fromtype, int fromid)
		: base(fromtype, fromid)
	{
		InitComponents();
	}

	public frm_SalesOrder(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_SalesOrder(int docEntry, int lineFosus)
	{
		DocEntrySearch = docEntry;
		LineNumFocus = lineFosus;
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesOrder"))
		{
			ChangeLogType = Enums.ChangeLog.clOrders;
			FormBpType = "C";
			FormDocumentTable = "ORDR";
			FormLineTable = "RDR1";
			DocFreightTable = "RDR3";
			FormController = "Sales";
			base.Text = "Sales Order";
			FormId = "oOrders";
			FormLayoutCode = "RDR2";
			FormLayoutCodeService = "RDR1";
			DocFreightTable = "RDR3";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oOrders);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.Allocate;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.OutAlocate;
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
			gv_Accounting.ItemId = "ORDR_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "ORDR_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			gcConfirmed.Visible = true;
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oBlanketAgreement
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oDownPaymentInvoice
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		return base.ShowFormChooseFromList(Enums.TypeCFL.CustomerLead, multiselect: false, text);
	}
}
