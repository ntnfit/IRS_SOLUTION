using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class MrpMap : EntityTypeConfiguration<Mrp>
{
	public MrpMap()
	{
		HasKey((Mrp t) => t.Id);
		Property((Mrp t) => t.MrpCode).HasMaxLength(50);
		Property((Mrp t) => t.MrpName).HasMaxLength(300);
		Property((Mrp t) => t.MpsCode).HasMaxLength(50);
		Property((Mrp t) => t.Whs).HasMaxLength(50);
		Property((Mrp t) => t.Notes).HasMaxLength(300);
		ToTable("apz_OMRP");
		Property((Mrp t) => t.Id).HasColumnName("Id");
		Property((Mrp t) => t.BeginDate).HasColumnName("BeginDate");
		Property((Mrp t) => t.MrpCode).HasColumnName("MrpCode");
		Property((Mrp t) => t.MrpName).HasColumnName("MrpName");
		Property((Mrp t) => t.EndDate).HasColumnName("EndDate");
		Property((Mrp t) => t.Status).HasColumnName("Status");
		Property((Mrp t) => t.CreateDate).HasColumnName("CreateDate");
		Property((Mrp t) => t.MpsCode).HasColumnName("MpsCode");
		Property((Mrp t) => t.Whs).HasColumnName("Whs");
		Property((Mrp t) => t.Notes).HasColumnName("Notes");
		Property((Mrp t) => t.FixDate).HasColumnName("FixDate");
	}
}
