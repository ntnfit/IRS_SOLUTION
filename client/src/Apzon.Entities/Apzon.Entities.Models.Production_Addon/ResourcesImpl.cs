using System;

namespace Apzon.Entities.Models.Production_Addon;

public class ResourcesImpl
{
	public string ResCode { get; set; }

	public string ResFName { get; set; }

	public string ResType { get; set; }

	public short? ResGrpCod { get; set; }

	public string UnitOfMsr { get; set; }

	public string Status { get; set; }

	public string IssueMthd { get; set; }

	public string CodeBars { get; set; }

	public int? NumResUnit { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string Plant { get; set; }

	public string CtrlArea { get; set; }

	public string Ctrlkey { get; set; }

	public string Remark { get; set; }

	public string Calendar { get; set; }

	public decimal? SetupTime { get; set; }

	public string SetupUoM { get; set; }

	public decimal? MachineTime { get; set; }

	public string MachineUoM { get; set; }

	public decimal? LaborTime { get; set; }

	public string LaborUoM { get; set; }

	public decimal? Quantity { get; set; }

	public int empID { get; set; }

	public TimeSpan? StartTime { get; set; }

	public TimeSpan? FinishTime { get; set; }

	public decimal? BreaksTime { get; set; }

	public int StdCost { get; set; }

	public decimal? DefaultCost { get; set; }

	public string CostExpAct { get; set; }

	public string WipAct { get; set; }

	public string SetupCost { get; set; }

	public string MachineCost { get; set; }

	public string LaborCost { get; set; }

	public string OtherCost { get; set; }

	public string ItemCode { get; set; }

	public int LineNum { get; set; }

	public DateTime validFrom { get; set; }

	public DateTime validTo { get; set; }

	public string ItemName { get; set; }

	public string AsstStatus { get; set; }

	public int Location { get; set; }

	public int Technician { get; set; }

	public int Employee { get; set; }

	public string UserText { get; set; }
}
