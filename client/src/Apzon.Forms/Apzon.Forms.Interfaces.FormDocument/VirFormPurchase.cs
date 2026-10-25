using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;

namespace Apzon.Forms.Interfaces.FormDocument;

[TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<VirFormPurchase, XtraFormEx>))]
public class VirFormPurchase : VirFormDocument
{
	protected override Task<List<VatGroupsImpl>> LoadVatGroup(string category)
	{
		return base.LoadVatGroup("I");
	}

	protected override frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		return base.ShowFormChooseFromList(Enums.TypeCFL.Vendor, multiselect: false, text);
	}

	public VirFormPurchase()
	{
		IsSalesDocument = false;
		Init();
	}

	public void Init()
	{
		gcDoc_CardCode.Caption = "Vendor Code";
		gcDoc_CardName.Caption = "Vendor Name";
		col_DueDate.Caption = "Due Date";
		gcLine_SlpCode.Caption = "Buyer";
		gcBillTo.Caption = "Pay to";
		gcItemCode.CflType = Enums.TypeCFL.PurchaseItem;
	}

	public VirFormPurchase(int docentry = 0)
	{
		Init();
		DocEntrySearch = docentry;
	}

	public VirFormPurchase(ApzObjectTypes from = ApzObjectTypes.oNone, int id = 0)
	{
		Init();
		if (from != ApzObjectTypes.oNone)
		{
			Fromtype = from;
			if (id != 0)
			{
				Fromentry = id.ToString();
			}
		}
		else if (id != 0)
		{
			DocEntrySearch = id;
		}
	}

	protected override void SettingLanguage()
	{
		base.SettingLanguage();
		try
		{
			if (!(Type.GetType("Apzon.Commons.Properties.DocumentsPurchase, Apzon.Commons") != null))
			{
				return;
			}
			foreach (DictionaryEntry item in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.DocumentsPurchase, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
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
}
