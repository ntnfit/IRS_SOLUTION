using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons.CFL;

namespace Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem;

public class RepCflWarehouse : VirRepChooseFromList
{
	public RepCflWarehouse()
	{
		TypeCfl = Enums.TypeCFL.Warehouse;
		MultiSelect = false;
	}

	public override void BindData(frm_ChooseFromList frm)
	{
		try
		{
			WarehouseImpl warehouseImpl = (WarehouseImpl)frm.ChooseItem;
			if (warehouseImpl != null)
			{
				if (GridView != null)
				{
					GridView.SetFocusedRowCellValue(FieldName, warehouseImpl.WhsCode);
				}
				else if (TreeList != null)
				{
					TreeList.FocusedNode.SetValue(FieldName, warehouseImpl.WhsCode);
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
