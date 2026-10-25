using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Sales;

public class frm_SalesQuotationForBatch : VirFormSales
{
	public frm_SalesQuotationForBatch(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_SalesQuotationForBatch()
	{
		InitComponents();
	}

	public frm_SalesQuotationForBatch(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesQuotationForBatch"))
		{
			ChangeLogType = Enums.ChangeLog.clQuotations;
			FormBpType = "C";
			FormDocumentTable = "OQUT";
			FormLineTable = "QUT1";
			DocFreightTable = "QUT3";
			FormController = "SalesQuotationForBatch";
			base.Text = "Sales Quotation";
			FormId = "oQuotations";
			FormLayoutCode = "QUT2";
			FormLayoutCodeService = "QUT1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oQuotations);
			FormObjectType = formObjectType;
			DocFreightTable = "QUT3";
			BatchSerialTransType = Enums.BatchSerialTransType.None;
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
			gv_Accounting.ItemId = "OQUT_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OQUT_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes> { ApzObjectTypes.oBlanketAgreement };
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oOrders,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oReserveInvoices,
				ApzObjectTypes.oInvoices
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		return base.ShowFormChooseFromList(Enums.TypeCFL.CustomerLead, multiselect: false, text);
	}

	protected override void SettingLanguage()
	{
		base.SettingLanguage();
		try
		{
			if (!(Type.GetType("Apzon.Commons.Properties.Quotation, Apzon.Commons") != null))
			{
				return;
			}
			foreach (DictionaryEntry item in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.Quotation, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
			{
				string fieldName = item.Key.ToString();
				if (gvDoc.Columns[fieldName] != null)
				{
					gvDoc.Columns[fieldName].Caption = Function.ToString(item.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		base.gvDoc_FocusedRowObjectChanged(sender, e);
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DocStatus"));
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				drlCopyTo.Enabled = text.Equals("O") || GlobalData.DocSettingsClosedQuot.Equals("Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
