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

public class frm_GoodsReturn : VirFormPurchase
{
	public frm_GoodsReturn()
	{
		InitComponents();
	}

	public frm_GoodsReturn(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public frm_GoodsReturn(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	private void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_GoodsReturn"))
		{
			ChangeLogType = Enums.ChangeLog.clPurchaseReturns;
			FormId = ApzObjectTypes.oPurchaseReturns.ToString();
			FormBpType = "S";
			FormDocumentTable = "ORPD";
			FormLineTable = "RPD1";
			DocFreightTable = "RPD3";
			FormController = "GoodsReturn";
			Text = "Goods Return";
			FormLayoutCode = "RPD2";
			FormLayoutCodeService = "RPD1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oPurchaseReturns);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.Out;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.Out;
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
			gv_Accounting.ItemId = "ORPD_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "ORPD_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oGoodsReturnRequest,
				ApzObjectTypes.oPurchaseDeliveryNotes
			};
			PopCopyTo = new List<ApzObjectTypes> { ApzObjectTypes.oPurchaseCreditNotes };
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}
}
