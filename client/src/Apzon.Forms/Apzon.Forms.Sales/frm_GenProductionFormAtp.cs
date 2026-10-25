using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.Mrp;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Forms.Sales;

public class frm_GenProductionFormAtp : VirGenProduction
{
	public frm_GenProductionFormAtp()
	{
		InitComponents();
	}

	private void InitComponents()
	{
		Controler = "Atp";
		FormMode = Enums.FormViewModeType.View;
		GridViewEx gridViewEx = gv_Cancel;
		GridViewEx gridViewEx2 = gv_Close;
		GridViewEx gridViewEx3 = gv_Plan;
		GridViewEx gridViewEx4 = gv_Release;
		string text = (gv_Report.FormId = "GenProductionFormAtp");
		string text3 = (gridViewEx4.FormId = text);
		string text5 = (gridViewEx3.FormId = text3);
		string text7 = (gridViewEx2.FormId = text5);
		string formId = (gridViewEx.FormId = text7);
		FormId = formId;
		gv_Cancel.ItemId = "Gen_Cancel";
		gv_Close.ItemId = "Gen_Close";
		gv_Plan.ItemId = "Gen_Plan";
		gv_Release.ItemId = "Gen_Release";
		gv_Report.ItemId = "Gen_Report";
	}

	protected override async Task SetDataWhs()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rWhs;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Warehouse.GetAllCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void LoadDatasourceType()
	{
		List<Common.Table_Code> dataSource = new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Name = "Atp Schedule",
				Code = "A"
			}
		};
		txtInput.Properties.DataSource = dataSource;
		txtInput.EditValue = "A";
		txtInput.ReadOnly = true;
	}

	protected override void txt_Schedule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListAtp, false, Function.ToString(txt_Code.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					AtpScheduleStatus = "A",
					AtpType = "I"
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txt_Code.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
					tabControl.Focus();
				}
			}
			else
			{
				int num = Function.ParseInt(txt_Code.EditValue);
				if (num != 0)
				{
					this.DrillDown(ApzObjectTypes.oAtp, ApzObjectTypes.oAtp, Function.ToString(num));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
