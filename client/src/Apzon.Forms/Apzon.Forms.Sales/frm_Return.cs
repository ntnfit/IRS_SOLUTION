using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;

namespace Apzon.Forms.Sales;

public class frm_Return : VirFormSales
{
	public frm_Return(int docEntry = 0)
		: base(docEntry)
	{
		InitComponents();
	}

	public frm_Return()
	{
		InitComponents();
	}

	public frm_Return(ApzObjectTypes fromType, int fromId)
		: base(fromType, fromId)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Return"))
		{
			ChangeLogType = Enums.ChangeLog.clReturns;
			FormBpType = "C";
			FormDocumentTable = "ORDN";
			FormLineTable = "RDN1";
			DocFreightTable = "RDN3";
			FormController = "Return";
			base.Text = "Return";
			FormId = "oReturns";
			FormLayoutCode = "RDN2";
			FormLayoutCodeService = "RDN1";
			ApzObjectTypes formObjectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oReturns);
			FormObjectType = formObjectType;
			BatchSerialTransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.In;
			ucBatchAndSerial1.InBatchFromWhs = true;
			ctm_BatchSerialNumbers.Visible = false;
			GridColumnEx gridColumnEx = gcLineEnSetCost;
			bool visible = (gcLineRetCost.Visible = true);
			gridColumnEx.Visible = visible;
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
			gv_Accounting.ItemId = "ORDN_ACT";
			gvPayTerms.ItemId = "@PMT1";
			gv_Logistic.ItemId = "ORDN_LGT";
			ucBatchAndSerial1.ItemId = "OBTN";
			ucBatchAndSerial1.GetDataFormDelivery = true;
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oReturnRequest
			};
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oCreditNotes
			};
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
		base.gv_Line_ShowingEditor(sender, e);
		try
		{
			if (gvLine.FocusedColumn.FieldName.Equals(gcLineRetCost.FieldName) && !gvLine.GetFocusedRowCellValue(gcLineEnSetCost).Equals("Y"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override void LoadDefaultValuePerDocument(ref DocumentLineRow line)
	{
		try
		{
			DocumentLineRow obj = line;
			string value;
			if (_documentSetting.Enitemcost.Equals("Y"))
			{
				value = (_documentSetting.EnSetCost.Equals("Y") ? "Y" : "N");
			}
			else
			{
				value = "N";
			}
			obj["EnSetCost"] = value;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
