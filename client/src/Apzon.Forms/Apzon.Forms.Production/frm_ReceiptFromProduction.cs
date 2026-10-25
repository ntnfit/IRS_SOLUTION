using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Inventory.InventoryTransaction;

namespace Apzon.Forms.Production;

public class frm_ReceiptFromProduction : frm_GoodsReceipt
{
	private const string FormName = "frm_ReceiptFromProduction";

	public frm_ReceiptFromProduction()
	{
		InitComponents();
	}

	public frm_ReceiptFromProduction(int baseType = 202)
		: base(baseType)
	{
		InitComponents();
	}

	public frm_ReceiptFromProduction(int productionDocNum = 0, int baseType = 202)
		: base(productionDocNum, baseType)
	{
		InitComponents();
	}

	public frm_ReceiptFromProduction(ApzObjectTypes fromType, int docEntry)
		: base(fromType, docEntry)
	{
		InitComponents();
	}

	public new void InitComponents()
	{
		FormController = "GoodReceiptIt";
		Text = "Receipt from production";
		FormId = "oReceiptProduction";
		FormLayoutCode = "IGN2";
		ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oInventoryGenEntry);
		ObjectType = objectType;
		BaseType = 202;
		TypeProductionOrder = Enums.TypeCFL.ReceiptProductionOrder;
		ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.In;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "OIGN";
		GridViewEx gridViewEx = gv_Lines;
		string formId = (ucAttachment1.FormId = FormId);
		gridViewEx.FormId = formId;
		gv_Lines.ItemId = "IGN1";
		ucBatchAndSerial1.FormId = FormId;
		ucBatchAndSerial1.ItemId = "OBTN";
		docSaveAsDraftToolStripMenuItem.Visible = false;
		DropDownButtonEx dropDownButtonEx = drlCopyFrom;
		bool visible = (drlCopyTo.Visible = false);
		dropDownButtonEx.Visible = visible;
		btn_RetCom.Visible = true;
	}

	public override async Task<bool> ApprovalDocuments(DocumentsImpl doc, bool isDraft)
	{
		try
		{
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
