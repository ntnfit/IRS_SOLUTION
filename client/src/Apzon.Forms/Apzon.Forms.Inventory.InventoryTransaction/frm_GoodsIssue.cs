using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using Apzon.Forms.Interfaces.FormInventory;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_GoodsIssue : VirFormReceiptIssue
{
	public frm_GoodsIssue()
	{
		InitComponents();
	}

	public frm_GoodsIssue(int baseType = -1)
		: base(baseType)
	{
		InitComponents();
	}

	public frm_GoodsIssue(int docEntryProductionOrder = 0, int baseType = -1)
		: base(docEntryProductionOrder, baseType)
	{
		InitComponents();
	}

	public frm_GoodsIssue(ApzObjectTypes fromType, int docEntry)
		: base(fromType, docEntry)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		ChangeLogType = Enums.ChangeLog.clInventoryGeneralExit;
		FormController = "GoodsIssueIt";
		base.Text = "Goods Issue";
		FormId = "oInventoryGenExit";
		FormLayoutCode = "IGE1";
		ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oInventoryGenExit);
		ObjectType = objectType;
		ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.Out;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "OIGE";
		GridViewEx gridViewEx = gv_Lines;
		string formId = (ucAttachment1.FormId = FormId);
		gridViewEx.FormId = formId;
		gv_Lines.ItemId = "IGE1";
		ucBatchAndSerial1.FormId = FormId;
		ucBatchAndSerial1.ItemId = "OBTN";
		PopCopyTo = new List<ApzObjectTypes> { ApzObjectTypes.oInventoryGenEntry };
	}
}
