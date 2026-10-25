using System;

namespace Apzon.Entities.Models.Administrator;

public class CustomAppointment
{
	public DateTime StartTime { get; set; }

	public DateTime EndTime { get; set; }

	public string Subject { get; set; }

	public int Status { get; set; }

	public string Description { get; set; }

	public int Label { get; set; }

	public string Location { get; set; }

	public bool AllDay { get; set; }

	public int EventType { get; set; }

	public string RecurrenceInfo { get; set; }

	public string ReminderInfo { get; set; }

	public object OwnerId { get; set; }

	public int ClgCode { get; set; }
}
