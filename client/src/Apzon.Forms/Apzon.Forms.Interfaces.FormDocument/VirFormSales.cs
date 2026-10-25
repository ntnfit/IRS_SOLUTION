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
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;

namespace Apzon.Forms.Interfaces.FormDocument;

public class VirFormSales : VirFormDocument
{
	protected override Task<List<VatGroupsImpl>> LoadVatGroup(string category)
	{
		return base.LoadVatGroup("O");
	}

	protected override frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		return ShowFormChooseFromList(Enums.TypeCFL.Customer, multiselect: false, text);
	}

	public VirFormSales()
	{
	}

	public VirFormSales(int docentry = 0)
	{
		DocEntrySearch = docentry;
	}

	public VirFormSales(ApzObjectTypes from = ApzObjectTypes.oNone, int id = 0)
	{
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
			if (!(Type.GetType("Apzon.Commons.Properties.DocumentsSales, Apzon.Commons") != null))
			{
				return;
			}
			foreach (DictionaryEntry item in new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.DocumentsSales, Apzon.Commons")).GetResourceSet(CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE), createIfNotExists: true, tryParents: true))
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

	protected override void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
		base.gv_Line_ShowingEditor(sender, e);
		if (gvLine.FocusedColumn.FieldName == "LineVat" || gvLine.FocusedColumn.FieldName == "VatSumFrgn")
		{
			e.Cancel = true;
		}
	}
}
