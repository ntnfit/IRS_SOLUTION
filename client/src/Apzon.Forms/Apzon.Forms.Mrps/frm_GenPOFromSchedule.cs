using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using Apzon.Forms.Interfaces.Mrp;

namespace Apzon.Forms.Mrps;

public class frm_GenPOFromSchedule : VirGenProduction
{
	public frm_GenPOFromSchedule()
	{
		InitComponents();
	}

	private void InitComponents()
	{
		Controler = "Schedule";
		FormMode = Enums.FormViewModeType.View;
		GridViewEx gridViewEx = gv_Report;
		GridViewEx gridViewEx2 = gv_Release;
		GridViewEx gridViewEx3 = gv_Plan;
		GridViewEx gridViewEx4 = gv_Close;
		string text = (gv_Cancel.FormId = "GenPOFromSchedule");
		string text3 = (gridViewEx4.FormId = text);
		string text5 = (gridViewEx3.FormId = text3);
		string text7 = (gridViewEx2.FormId = text5);
		string formId = (gridViewEx.FormId = text7);
		FormId = formId;
		gv_Report.ItemId = "Gen_Report";
		gv_Release.ItemId = "Gen_Release";
		gv_Plan.ItemId = "Gen_Plan";
		gv_Close.ItemId = "Gen_Close";
		gv_Cancel.ItemId = "Gen_Cancel";
	}
}
