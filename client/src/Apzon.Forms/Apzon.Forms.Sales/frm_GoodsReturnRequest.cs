using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

public class frm_GoodsReturnRequest : VirFormPurchase
{
	private IContainer components;

	public frm_GoodsReturnRequest()
	{
		InitComponents();
	}

	public frm_GoodsReturnRequest(ApzObjectTypes fromtype, int fromid)
		: base(fromtype, fromid)
	{
		InitComponents();
	}

	public frm_GoodsReturnRequest(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_GoodsReturnRequest(int docEntry, int lineFosus)
	{
		DocEntrySearch = docEntry;
		LineNumFocus = lineFosus;
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_GoodsReturnRequest"))
		{
			ChangeLogType = Enums.ChangeLog.clGoodsReturnRequest;
			FormBpType = "S";
			FormDocumentTable = "OPRR";
			FormLineTable = "PRR1";
			DocFreightTable = "PRR3";
			FormController = "GoodsReturnRequest";
			base.Text = "Good Return Request";
			FormId = "oGoodsReturnRequest";
			FormLayoutCode = "PRR2";
			FormLayoutCodeService = "PRR1";
			DocFreightTable = "PRR3";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oGoodsReturnRequest);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.Allocate;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.OutAlocate;
			ucBatchAndSerial1.GetDataFormDelivery = true;
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
			gv_Accounting.ItemId = "OPRR_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPRR_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			gcConfirmed.Visible = true;
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseInvoices
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseReturns,
				ApzObjectTypes.oPurchaseCreditNotes
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_GoodsReturnRequest));
		((System.ComponentModel.ISupportInitialize)base.LstLineEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.LstLineDelete).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.LstLineCopy).BeginInit();
		((System.ComponentModel.ISupportInitialize)base._currentRefDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.xtraTab_Line).BeginInit();
		base.xtraTab_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.txtBusinessPartner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtCardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtItemcode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.cbxTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rLookUpVatGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtWhseCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rlpShipType).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rShipTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rBillTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rlupContactPersonCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rILCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit6).BeginInit();
		base.tab_PaymentTerms.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.spin_Count.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.cal_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		base.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit8).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rILSalesEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ApprovalStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_bte_CtlAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_DistriMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_StatusFreight).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_GroupTax).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_LstPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_FreightCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_FreightTaxGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.bte_Accounting_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_GroupNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_PaymentMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_Indicator).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_PayDuMonth).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_UnitsOfMeasurment).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_InventoryQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUseBaseUnits).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rBpCatalog).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit9).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.split_Main).BeginInit();
		base.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.rep_LineSlpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Series).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ReqType).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Requester).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.cal_TotalFC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtRequester.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.look_ReqType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_PmtToDraw).BeginInit();
		base.SuspendLayout();
		base.txtFromDate.EditValue = null;
		base.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtFromDate.Properties.Appearance.Options.UseFont = true;
		base.txtToDate.EditValue = null;
		base.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtToDate.Properties.Appearance.Options.UseFont = true;
		base.txtNoOfTrans.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		base.xtraTab_Line.SelectedTabPage = base.tbDocumentLine;
		base.xtraTab_Line.Size = new System.Drawing.Size(1088, 161);
		base.tbDocumentLine.Size = new System.Drawing.Size(1086, 136);
		base.txtBusinessPartner.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtBusinessPartner.Properties.Appearance.Options.UseFont = true;
		base.tab_PaymentTerms.Size = new System.Drawing.Size(1086, 136);
		base.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btnSearch.Appearance.Options.UseBackColor = true;
		base.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		base.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btnCancel.Appearance.Options.UseBackColor = true;
		base.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		base.btnCancel.Location = new System.Drawing.Point(132, 583);
		base.drlCopyFrom.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.drlCopyFrom.Appearance.Options.UseBackColor = true;
		base.drlCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyFrom.ImageOptions.Image");
		base.drlCopyFrom.Location = new System.Drawing.Point(860, 583);
		base.drlCopyTo.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.drlCopyTo.Appearance.Options.UseBackColor = true;
		base.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		base.drlCopyTo.Location = new System.Drawing.Point(976, 583);
		base.split_Main.Size = new System.Drawing.Size(1088, 523);
		base.ucBatchAndSerial1.Size = new System.Drawing.Size(1088, 199);
		base.look_ReqType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.look_ReqType.Properties.Appearance.Options.UseFont = true;
		base.look_ReqType.Size = new System.Drawing.Size(103, 18);
		base.btn_CheckPromotion.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btn_CheckPromotion.Appearance.Options.UseBackColor = true;
		base.btn_CheckPromotion.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CheckPromotion.ImageOptions.Image");
		base.btn_CheckPromotion.Location = new System.Drawing.Point(364, 583);
		base.btn_ClearPromotion.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btn_ClearPromotion.Appearance.Options.UseBackColor = true;
		base.btn_ClearPromotion.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ClearPromotion.ImageOptions.Image");
		base.btn_ClearPromotion.Location = new System.Drawing.Point(485, 583);
		base.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btn_Save.Appearance.Options.UseBackColor = true;
		base.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		base.btn_Save.Location = new System.Drawing.Point(6, 583);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1022, 524);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_GoodsReturnRequest.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_GoodsReturnRequest";
		this.Text = "frm_GoodsReturnRequest";
		base.Controls.SetChildIndex(base.look_ReqType, 0);
		base.Controls.SetChildIndex(base.txtFromDate, 0);
		base.Controls.SetChildIndex(base.txtToDate, 0);
		base.Controls.SetChildIndex(base.txtNoOfTrans, 0);
		base.Controls.SetChildIndex(base.btnSearch, 0);
		base.Controls.SetChildIndex(base.btnCancel, 0);
		base.Controls.SetChildIndex(base.lb_FromDate, 0);
		base.Controls.SetChildIndex(base.lb_ToDate, 0);
		base.Controls.SetChildIndex(base.lb_DocNumber, 0);
		base.Controls.SetChildIndex(base.lb_BusinessPartner, 0);
		base.Controls.SetChildIndex(base.lb_DocStatus, 0);
		base.Controls.SetChildIndex(base.txtBusinessPartner, 0);
		base.Controls.SetChildIndex(base.cbxStatus, 0);
		base.Controls.SetChildIndex(base.drlCopyFrom, 0);
		base.Controls.SetChildIndex(base.drlCopyTo, 0);
		base.Controls.SetChildIndex(base.split_Main, 0);
		base.Controls.SetChildIndex(base.lb_Requester, 0);
		base.Controls.SetChildIndex(base.txtRequester, 0);
		base.Controls.SetChildIndex(base.btn_CheckPromotion, 0);
		base.Controls.SetChildIndex(base.btn_ClearPromotion, 0);
		base.Controls.SetChildIndex(base.btn_Save, 0);
		((System.ComponentModel.ISupportInitialize)base.LstLineEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)base.LstLineDelete).EndInit();
		((System.ComponentModel.ISupportInitialize)base.LstLineCopy).EndInit();
		((System.ComponentModel.ISupportInitialize)base._currentRefDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.xtraTab_Line).EndInit();
		base.xtraTab_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.txtBusinessPartner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtCardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtItemcode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.cbxTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rLookUpVatGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rtxtWhseCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rlpShipType).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rShipTo).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rBillTo).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rlupContactPersonCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rILCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit6).EndInit();
		base.tab_PaymentTerms.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.spin_Count.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.cal_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit2).EndInit();
		base.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit8).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rILSalesEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ApprovalStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_bte_CtlAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemDateEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_DistriMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_StatusFreight).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_GroupTax).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_LstPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_FreightCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_FreightTaxGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)base.bte_Accounting_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_GroupNum).EndInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_PaymentMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_Indicator).EndInit();
		((System.ComponentModel.ISupportInitialize)base.luk_Accounting_PayDuMonth).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_UnitsOfMeasurment).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_InventoryQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUseBaseUnits).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rBpCatalog).EndInit();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemCalcEdit9).EndInit();
		((System.ComponentModel.ISupportInitialize)base.split_Main).EndInit();
		base.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.rep_LineSlpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Series).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rLineUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ReqType).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Requester).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)base.cal_TotalFC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtRequester.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.look_ReqType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_PmtToDraw).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
