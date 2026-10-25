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

internal class frm_DownPaymentRequest : VirFormSales
{
	public frm_DownPaymentRequest()
	{
		InitComponents();
	}

	public frm_DownPaymentRequest(ApzObjectTypes fromtype, int fromid)
		: base(fromtype, fromid)
	{
		InitComponents();
	}

	public frm_DownPaymentRequest(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_DownPaymentRequest(int docEntry, int lineFosus)
	{
		DocEntrySearch = docEntry;
		LineNumFocus = lineFosus;
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DownPaymentRequest"))
		{
			ChangeLogType = Enums.ChangeLog.clDownPayments;
			DownPaymentType = Enums.DownPaymentType.dptRequest;
			FormBpType = "C";
			FormDocumentTable = "ODPI";
			FormLineTable = "DPI1";
			DocFreightTable = "DPI3";
			FormController = "DownPayment";
			base.Text = "A/R Down Payment Request";
			FormId = "DownPaymentRequest";
			FormLayoutCode = "DPI2";
			FormLayoutCodeService = "DPI1";
			DocFreightTable = "DPI3";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oDownPayments);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.None;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.None;
			GridViewEx gridViewEx = gvDoc;
			GridViewEx gridViewEx2 = gvLine;
			GridViewEx gridViewEx3 = gv_Freight;
			GridViewEx gridViewEx4 = gv_Logistic;
			GridViewEx gridViewEx5 = gv_Accounting;
			GridViewEx gridViewEx6 = gvPayTerms;
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
			gv_Accounting.ItemId = "ODPI_ACT";
			gvPayTerms.ItemId = "@PMT1";
			ucBatchAndSerial1.ItemId = "OBTN";
			gv_Logistic.ItemId = "ODPI_LGT";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oBlanketAgreement
			};
			PopCopyTo = new List<ApzObjectTypes>();
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		return base.ShowFormChooseFromList(Enums.TypeCFL.Customer, multiselect: false, text);
	}
}
