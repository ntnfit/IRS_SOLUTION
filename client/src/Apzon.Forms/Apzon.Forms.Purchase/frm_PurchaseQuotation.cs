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
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseQuotation : VirFormPurchase
{
	public frm_PurchaseQuotation()
	{
		InitComponents();
	}

	public frm_PurchaseQuotation(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public frm_PurchaseQuotation(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	private void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseQuotation"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseQuotation;
			FormId = ApzObjectTypes.oPurchaseQuotations.ToString();
			FormBpType = "S";
			FormDocumentTable = "OPQT";
			FormLineTable = "PQT1";
			DocFreightTable = "PQT3";
			FormController = "PPurchaseQuotation";
			Text = "Purchase Quotation";
			FormLayoutCode = "PQT2";
			FormLayoutCodeService = "PQT1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseQuotations);
			FormObjectType = formObjectType;
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
			gv_Accounting.ItemId = "OPQT_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "OPQT_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oBlanketAgreement,
				ApzObjectTypes.oBlanketAgreement
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oAPReserveInvoice
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void SettingLanguage()
	{
		base.SettingLanguage();
		try
		{
			if (Type.GetType("Apzon.Commons.Properties.PurchaseQuotation, Apzon.Commons") != null)
			{
				foreach (DictionaryEntry item in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.PurchaseQuotation, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
				{
					string fieldName = item.Key.ToString();
					if (gvDoc.Columns[fieldName] != null)
					{
						gvDoc.Columns[fieldName].Caption = Function.ToString(item.Value);
					}
				}
			}
			if (!(Type.GetType("Apzon.Commons.Properties.PurchaseQuotationLine, Apzon.Commons") != null))
			{
				return;
			}
			foreach (DictionaryEntry item2 in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.PurchaseQuotationLine, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
			{
				string fieldName2 = item2.Key.ToString();
				if (gvLine.Columns[fieldName2] != null)
				{
					gvLine.Columns[fieldName2].Caption = Function.ToString(item2.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
