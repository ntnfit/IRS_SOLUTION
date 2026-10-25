using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class WorkSchedulingLinesMap : EntityTypeConfiguration<WorkSchedulingLines>
{
	public WorkSchedulingLinesMap()
	{
		HasKey((WorkSchedulingLines t) => new { t.WorkDate, t.ID, t.LinID });
		Property((WorkSchedulingLines t) => t.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
		Property((WorkSchedulingLines t) => t.Shift).HasMaxLength(50);
		Property((WorkSchedulingLines t) => t.Remark).HasMaxLength(100);
		ToTable("apz_WSH1");
		Property((WorkSchedulingLines t) => t.WorkDate).HasColumnName("WorkDate");
		Property((WorkSchedulingLines t) => t.ID).HasColumnName("ID");
		Property((WorkSchedulingLines t) => t.Shift).HasColumnName("Shift");
		Property((WorkSchedulingLines t) => t.Remark).HasColumnName("Remark");
		Property((WorkSchedulingLines t) => t.LinID).HasColumnName("LinID");
	}
}
