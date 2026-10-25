using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Global;

namespace Apzon.Forms.Production;

public class frm_IssueForProduction : frm_GoodsIssue
{
	private const string FormName = "frm_IssueForProduction";

	public frm_IssueForProduction()
	{
		InitComponents();
	}

	public frm_IssueForProduction(int baseType = 202)
		: base(baseType)
	{
		InitComponents();
	}

	public frm_IssueForProduction(int productionDocNum = 0, int baseType = 202)
		: base(productionDocNum, baseType)
	{
		InitComponents();
	}

	public frm_IssueForProduction(ApzObjectTypes fromType, int docEntry)
		: base(fromType, docEntry)
	{
		InitComponents();
	}

	public new void InitComponents()
	{
		if (GlobalData.ListAccessForm != null && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_IssueForProduction"))
		{
			FormController = "GoodsIssueIt";
			Text = "Issue for production";
			FormId = "oIssueProduction";
			FormLayoutCode = "IGE2";
			ApzObjectTypes objectType = (ucBatchAndSerial1.FormObjectType = ApzObjectTypes.oInventoryGenExit);
			ObjectType = objectType;
			TypeProductionOrder = Enums.TypeCFL.IssueProductionOrder;
			BaseType = 202;
			ucBatchAndSerial1.TransType = Enums.BatchSerialTransType.Out;
			gv_Doc.FormId = FormId;
			gv_Doc.ItemId = "OIGE";
			GridViewEx gridViewEx = gv_Lines;
			string formId = (ucAttachment1.FormId = FormId);
			gridViewEx.FormId = formId;
			gv_Lines.ItemId = "IGE1";
			ucBatchAndSerial1.FormId = FormId;
			ucBatchAndSerial1.ItemId = "OBTN";
			docSaveAsDraftToolStripMenuItem.Visible = false;
			DropDownButtonEx dropDownButtonEx = drlCopyFrom;
			bool visible = (drlCopyTo.Visible = false);
			dropDownButtonEx.Visible = visible;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
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
