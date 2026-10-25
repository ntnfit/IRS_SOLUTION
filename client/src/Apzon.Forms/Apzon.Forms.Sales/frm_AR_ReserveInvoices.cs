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
using DevExpress.Utils.Menu;

namespace Apzon.Forms.Sales;

public class frm_AR_ReserveInvoices : VirFormSales
{
	private IContainer components;

	public frm_AR_ReserveInvoices()
	{
		Init();
	}

	public frm_AR_ReserveInvoices(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		Init();
	}

	public frm_AR_ReserveInvoices(int docEntry = 0)
		: base(docEntry)
	{
		Init();
	}

	public void Init()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_AR_ReserveInvoices"))
		{
			ChangeLogType = Enums.ChangeLog.clInvoices;
			CheckAri = "Y";
			FormBpType = "C";
			FormDocumentTable = "OINV";
			FormLineTable = "INV1";
			DocFreightTable = "INV3";
			FormController = "ARInvoice";
			base.Text = "A/R Reserve Invoice";
			FormId = ApzObjectTypes.oReserveInvoices.ToString();
			FormLayoutCode = "INV2";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oReserveInvoices);
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
			gv_Accounting.ItemId = "OINV_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OINV_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oQuotations,
				ApzObjectTypes.oOrders
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oCreditNotes
			};
			gcDpmTotal.ColumnEdit = rep_PmtToDraw;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void disableCopyTobyRejected()
	{
		string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
		string text2 = Function.ToString(gvDoc.GetFocusedRowCellValue("WddStatus"));
		drlCopyTo.SettingReadonly = false;
		drlCopyTo.Enabled = (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && (text.Equals("O") || Function.ToString(gvDoc.GetFocusedRowCellValue("InvntSttus")) == "O") && !text2.Equals("N");
		drlCopyTo.SettingReadonly = !drlCopyTo.Enabled;
		if (drlCopyTo.DropDownControl is DXPopupMenu dXPopupMenu)
		{
			dXPopupMenu.Items[MenuCaption.Delivery].Visible = Function.ToString(gvDoc.GetFocusedRowCellValue("InvntSttus")) == "O";
			dXPopupMenu.Items[MenuCaption.ARCreditMemos].Visible = text == "O";
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

	private new void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "frm_AR_ReserveInvoices";
	}
}
