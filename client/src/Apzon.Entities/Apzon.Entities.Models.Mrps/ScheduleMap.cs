using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class ScheduleMap : EntityTypeConfiguration<ScheduleImpl>
{
	public ScheduleMap()
	{
		HasKey((ScheduleImpl t) => new { t.Id, t.PSCode });
		Property((ScheduleImpl t) => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		Property((ScheduleImpl t) => t.MrpCode).HasMaxLength(50);
		Property((ScheduleImpl t) => t.PSCode).IsRequired().HasMaxLength(50);
		ToTable("apz_OPRS");
		Property((ScheduleImpl t) => t.Id).HasColumnName("Id");
		Property((ScheduleImpl t) => t.BeginDate).HasColumnName("BeginDate");
		Property((ScheduleImpl t) => t.MrpCode).HasColumnName("MrpCode");
		Property((ScheduleImpl t) => t.EndDate).HasColumnName("EndDate");
		Property((ScheduleImpl t) => t.Status).HasColumnName("Status");
		Property((ScheduleImpl t) => t.CreateDate).HasColumnName("CreateDate");
		Property((ScheduleImpl t) => t.Notes).HasColumnName("Notes");
		Property((ScheduleImpl t) => t.PSCode).HasColumnName("PSCode");
		Property((ScheduleImpl t) => t.Remark).HasColumnName("Remark");
	}
}
