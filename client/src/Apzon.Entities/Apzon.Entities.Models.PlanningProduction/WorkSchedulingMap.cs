using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class WorkSchedulingMap : EntityTypeConfiguration<WorkScheduling>
{
	public WorkSchedulingMap()
	{
		HasKey((WorkScheduling t) => t.Id);
		Property((WorkScheduling t) => t.Name).HasMaxLength(300);
		ToTable("apz_OWSH");
		Property((WorkScheduling t) => t.Id).HasColumnName("Id");
		Property((WorkScheduling t) => t.Name).HasColumnName("Name");
		Property((WorkScheduling t) => t.FromDate).HasColumnName("FromDate");
		Property((WorkScheduling t) => t.ToDate).HasColumnName("ToDate");
		Property((WorkScheduling t) => t.Notes).HasColumnName("Notes");
		Property((WorkScheduling t) => t.Year).HasColumnName("Year");
	}
}
