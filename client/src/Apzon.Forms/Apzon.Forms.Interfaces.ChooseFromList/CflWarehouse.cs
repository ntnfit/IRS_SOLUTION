using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons.CFL;

namespace Apzon.Forms.Interfaces.ChooseFromList;

public class CflWarehouse : VirChooseFromList
{
	public CflWarehouse()
	{
		MultiSelect = false;
		TypeCfl = Enums.TypeCFL.Warehouse;
	}

	public override void BindData(frm_ChooseFromList frm)
	{
		try
		{
			WarehouseImpl warehouseImpl = (WarehouseImpl)frm.ChooseItem;
			if (warehouseImpl != null)
			{
				EditValue = warehouseImpl.WhsCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
