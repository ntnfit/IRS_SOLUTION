using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class ScheduleDetailMap : EntityTypeConfiguration<ScheduleDetailImpl>
{
	public ScheduleDetailMap()
	{
		HasKey((ScheduleDetailImpl t) => t.Id);
		Property((ScheduleDetailImpl t) => t.Item).HasMaxLength(50);
		Property((ScheduleDetailImpl t) => t.Shift).HasMaxLength(2);
		Property((ScheduleDetailImpl t) => t.PSCode).HasMaxLength(50);
		Property((ScheduleDetailImpl t) => t.Line).HasMaxLength(50);
		ToTable("apz_PRS1");
		Property((ScheduleDetailImpl t) => t.Id).HasColumnName("Id");
		Property((ScheduleDetailImpl t) => t.Item).HasColumnName("Item");
		Property((ScheduleDetailImpl t) => t.Type).HasColumnName("Type");
		Property((ScheduleDetailImpl t) => t.WorkDay).HasColumnName("WorkDay");
		Property((ScheduleDetailImpl t) => t.Qty).HasColumnName("Qty");
		Property((ScheduleDetailImpl t) => t.Notes).HasColumnName("Notes");
		Property((ScheduleDetailImpl t) => t.Shift).HasColumnName("Shift");
		Property((ScheduleDetailImpl t) => t.PSCode).HasColumnName("PSCode");
		Property((ScheduleDetailImpl t) => t.ProductionOrder).HasColumnName("ProductionOrder");
		Property((ScheduleDetailImpl t) => t.Line).HasColumnName("Line");
		Property((ScheduleDetailImpl t) => t.MachineTime).HasColumnName("MachineTime");
		Property((ScheduleDetailImpl t) => t.NumMachine).HasColumnName("NumMachine");
	}
}
