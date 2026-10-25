using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons.CFL;

namespace Apzon.Forms.Interfaces.ChooseFromList;

public class CflCustomer : VirChooseFromList
{
	public CflCustomer()
	{
		TypeCfl = Enums.TypeCFL.Customer;
		MultiSelect = false;
	}

	public override void BindData(frm_ChooseFromList frm)
	{
		try
		{
			BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm.ChooseItem;
			if (businessPartnersImpl != null)
			{
				EditValue = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
