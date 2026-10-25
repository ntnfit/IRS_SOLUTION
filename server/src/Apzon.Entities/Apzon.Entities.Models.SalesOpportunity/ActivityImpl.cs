using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.SalesOpportunity;

public class ActivityImpl
{
	public int ClgCode { get; set; }

	public string CardCode { get; set; }

	public string Notes { get; set; }

	public DateTime? CntctDate { get; set; }

	public int? CntctTime { get; set; }

	public DateTime? Recontact { get; set; }

	public string Closed { get; set; }

	public DateTime? CloseDate { get; set; }

	public string ContactPer { get; set; }

	public string Tel { get; set; }

	public string Fax { get; set; }

	public short? CntctSbjct { get; set; }

	public string Transfered { get; set; }

	public string DocType { get; set; }

	public string DocNum { get; set; }

	public string DocEntry { get; set; }

	public string Attachment { get; set; }

	public string DataSource { get; set; }

	public short? AttendUser { get; set; }

	public int? CntctCode { get; set; }

	public short? UserSign { get; set; }

	public int? SlpCode { get; set; }

	public string Action { get; set; }

	public string Details { get; set; }

	public short? CntctType { get; set; }

	public short? Location { get; set; }

	public int? BeginTime { get; set; }

	public decimal? Duration { get; set; }

	public string DurType { get; set; }

	public int? ENDTime { get; set; }

	public string Priority { get; set; }

	public string Reminder { get; set; }

	public decimal? RemQty { get; set; }

	public string RemType { get; set; }

	public int? OprId { get; set; }

	public short? OprLine { get; set; }

	public DateTime? RemDate { get; set; }

	public short? RemTime { get; set; }

	public string RemSented { get; set; }

	public short? Instance { get; set; }

	public DateTime? endDate { get; set; }

	public int? status { get; set; }

	public string personal { get; set; }

	public string inactive { get; set; }

	public string tentative { get; set; }

	public string street { get; set; }

	public string city { get; set; }

	public string country { get; set; }

	public string state { get; set; }

	public string room { get; set; }

	public string parentType { get; set; }

	public int? parentId { get; set; }

	public int? prevActvty { get; set; }

	public int? AtcEntry { get; set; }

	public string RecurPat { get; set; }

	public string EndType { get; set; }

	public DateTime? SeStartDat { get; set; }

	public DateTime? SeEndDat { get; set; }

	public int? MaxOccur { get; set; }

	public int? Interval { get; set; }

	public string Sunday { get; set; }

	public string Monday { get; set; }

	public string Tuesday { get; set; }

	public string Wednesday { get; set; }

	public string Thursday { get; set; }

	public string Friday { get; set; }

	public string Saturday { get; set; }

	public string SubOption { get; set; }

	public int? DayInMonth { get; set; }

	public int? Month { get; set; }

	public int? DayOfWeek { get; set; }

	public int? Week { get; set; }

	public int? SeriesNum { get; set; }

	public DateTime? OrigDate { get; set; }

	public string IsRemoved { get; set; }

	public DateTime? LastRemind { get; set; }

	public short? AssignedBy { get; set; }

	public string AddrName { get; set; }

	public string AddrType { get; set; }

	public int? AttendEmpl { get; set; }

	public DateTime? NextDate { get; set; }

	public short? NextTime { get; set; }

	public List<ActivityTypeImpl> activityType { get; set; }

	public List<ActivitySubjectsImpl> activitySubject { get; set; }

	public List<ActivitySubjectsImpl> activityMeetingLocation { get; set; }
}
