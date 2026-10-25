using System;

namespace Apzon.Entities.Models.PM;

public class ProjectManagementImpl
{
	public int LineNum { get; set; }

	public int FatherNum { get; set; }

	public string TaskCode { get; set; }

	public string TaskName { get; set; }

	public string Wbs { get; set; }

	public DateTime FromDate { get; set; }

	public DateTime ToDate { get; set; }

	public int AbsEntry { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public int Priority { get; set; }

	public string Milestone { get; set; }

	public string Notes { get; set; }

	public string Status { get; set; }

	public int VisOrder { get; set; }
}
