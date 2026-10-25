using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using Apzon.Forms.Interfaces.FormInventory;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_GoodsReceipt : VirFormReceiptIssue
{
	private IContainer components;

	public frm_GoodsReceipt()
	{
		InitComponents();
	}

	public frm_GoodsReceipt(int baseType = -1)
		: base(baseType)
	{
		InitComponents();
	}

	public frm_GoodsReceipt(int docEntryProductionOrder = 0, int baseType = -1)
		: base(docEntryProductionOrder, baseType)
	{
		InitComponents();
	}

	public frm_GoodsReceipt(ApzObjectTypes fromType, int docEntry)
		: base(fromType, docEntry)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		ChangeLogType = Enums.ChangeLog.clInventoryGeneralEntry;
		FormController = "GoodReceiptIt";
		base.Text = "Goods Receipt";
		FormId = "oInventoryGenEntry";
		FormLayoutCode = "IGN1";
		ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oInventoryGenEntry);
		ObjectType = objectType;
		ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.In;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "OIGN";
		GridViewEx gridViewEx = gv_Lines;
		string formId = (ucAttachment1.FormId = FormId);
		gridViewEx.FormId = formId;
		gv_Lines.ItemId = "IGN1";
		ucBatchAndSerial1.FormId = FormId;
		ucBatchAndSerial1.ItemId = "OBTN";
		PopCopyFrom = new List<ApzObjectTypes> { ApzObjectTypes.oInventoryGenExit };
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "frm_GoodsReceipt";
	}
}
