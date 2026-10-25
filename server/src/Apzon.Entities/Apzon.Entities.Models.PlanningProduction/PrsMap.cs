using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class PrsMap : EntityTypeConfiguration<Prs>
{
	public PrsMap()
	{
		HasKey((Prs t) => new { t.Id, t.PSCode });
		Property((Prs t) => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
		Property((Prs t) => t.MrpCode).HasMaxLength(50);
		Property((Prs t) => t.PSCode).IsRequired().HasMaxLength(50);
		ToTable("apz_OPRS");
		Property((Prs t) => t.Id).HasColumnName("Id");
		Property((Prs t) => t.BeginDate).HasColumnName("BeginDate");
		Property((Prs t) => t.MrpCode).HasColumnName("MrpCode");
		Property((Prs t) => t.EndDate).HasColumnName("EndDate");
		Property((Prs t) => t.Status).HasColumnName("Status");
		Property((Prs t) => t.CreateDate).HasColumnName("CreateDate");
		Property((Prs t) => t.Notes).HasColumnName("Notes");
		Property((Prs t) => t.PSCode).HasColumnName("PSCode");
	}
}
