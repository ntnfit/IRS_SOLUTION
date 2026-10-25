using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class ScheduleCapacityMap : EntityTypeConfiguration<ScheduleCapacityImpl>
{
	public ScheduleCapacityMap()
	{
		HasKey((ScheduleCapacityImpl t) => t.Id);
		Property((ScheduleCapacityImpl t) => t.Item).HasMaxLength(50);
		Property((ScheduleCapacityImpl t) => t.Shift).HasMaxLength(2);
		Property((ScheduleCapacityImpl t) => t.PSCode).HasMaxLength(50);
		ToTable("apz_PRS2");
		Property((ScheduleCapacityImpl t) => t.Id).HasColumnName("Id");
		Property((ScheduleCapacityImpl t) => t.Item).HasColumnName("Item");
		Property((ScheduleCapacityImpl t) => t.Type).HasColumnName("Type");
		Property((ScheduleCapacityImpl t) => t.WorkDay).HasColumnName("WorkDay");
		Property((ScheduleCapacityImpl t) => t.Amount).HasColumnName("Amount");
		Property((ScheduleCapacityImpl t) => t.Shift).HasColumnName("Shift");
		Property((ScheduleCapacityImpl t) => t.Notes).HasColumnName("Notes");
		Property((ScheduleCapacityImpl t) => t.PSCode).HasColumnName("PSCode");
	}
}
