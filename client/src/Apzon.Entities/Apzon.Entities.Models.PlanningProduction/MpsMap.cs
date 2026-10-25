using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class MpsMap : EntityTypeConfiguration<Mps>
{
	public MpsMap()
	{
		HasKey((Mps t) => t.Id);
		Property((Mps t) => t.MpsCode).HasMaxLength(50);
		ToTable("apz_OMPS");
		Property((Mps t) => t.Id).HasColumnName("Id");
		Property((Mps t) => t.BeginDate).HasColumnName("BeginDate");
		Property((Mps t) => t.MpsCode).HasColumnName("MpsCode");
		Property((Mps t) => t.EndDate).HasColumnName("EndDate");
		Property((Mps t) => t.Status).HasColumnName("Status");
		Property((Mps t) => t.CreateDate).HasColumnName("CreateDate");
		Property((Mps t) => t.Notes).HasColumnName("Notes");
		Property((Mps t) => t.ForCastID).HasColumnName("ForCastID");
		Property((Mps t) => t.FixDate).HasColumnName("FixDate");
	}
}
