using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class Prs2Map : EntityTypeConfiguration<Prs2>
{
	public Prs2Map()
	{
		HasKey((Prs2 t) => t.Id);
		Property((Prs2 t) => t.Item).HasMaxLength(50);
		Property((Prs2 t) => t.Shift).HasMaxLength(2);
		Property((Prs2 t) => t.PSCode).HasMaxLength(50);
		ToTable("apz_PRS2");
		Property((Prs2 t) => t.Id).HasColumnName("Id");
		Property((Prs2 t) => t.Item).HasColumnName("Item");
		Property((Prs2 t) => t.Type).HasColumnName("Type");
		Property((Prs2 t) => t.WorkDay).HasColumnName("WorkDay");
		Property((Prs2 t) => t.Amount).HasColumnName("Amount");
		Property((Prs2 t) => t.Shift).HasColumnName("Shift");
		Property((Prs2 t) => t.Notes).HasColumnName("Notes");
		Property((Prs2 t) => t.PSCode).HasColumnName("PSCode");
	}
}
