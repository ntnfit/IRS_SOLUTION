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

namespace Apzon.Forms.Sales;

public class frm_ARCreditMemos : VirFormSales
{
	private IContainer components;

	public frm_ARCreditMemos()
	{
		InitComponents();
	}

	public frm_ARCreditMemos(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_ARCreditMemos(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ARCreditMemos"))
		{
			ChangeLogType = Enums.ChangeLog.clCreditCards;
			FormBpType = "C";
			FormDocumentTable = "ORIN";
			FormLineTable = "RIN1";
			DocFreightTable = "RIN3";
			FormController = "ARCreditMemo";
			Text = "A/R CreditMemo";
			FormId = "oCreditNotes";
			FormLayoutCode = "RIN2";
			FormLayoutCodeService = "RIN1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oCreditNotes);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.InBatchFromWhs = true;
			ucBatchAndSerial1.CheckNoInvtryMv = true;
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
			gv_Accounting.ItemId = "ORIN_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "ORIN_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			ucBatchAndSerial1.GetDataFormDelivery = true;
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oReturns,
				ApzObjectTypes.oInvoices
			};
			PopCopyTo = new List<ApzObjectTypes>();
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
		this.Text = "frm_ARCreditMemos";
	}
}
