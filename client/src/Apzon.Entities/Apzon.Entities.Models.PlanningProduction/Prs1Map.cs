using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class Prs1Map : EntityTypeConfiguration<Prs1>
{
	public Prs1Map()
	{
		HasKey((Prs1 t) => t.Id);
		Property((Prs1 t) => t.Item).HasMaxLength(50);
		Property((Prs1 t) => t.Shift).HasMaxLength(2);
		Property((Prs1 t) => t.PSCode).HasMaxLength(50);
		Property((Prs1 t) => t.Line).HasMaxLength(50);
		ToTable("apz_PRS1");
		Property((Prs1 t) => t.Id).HasColumnName("Id");
		Property((Prs1 t) => t.Item).HasColumnName("Item");
		Property((Prs1 t) => t.Type).HasColumnName("Type");
		Property((Prs1 t) => t.WorkDay).HasColumnName("WorkDay");
		Property((Prs1 t) => t.Qty).HasColumnName("Qty");
		Property((Prs1 t) => t.Notes).HasColumnName("Notes");
		Property((Prs1 t) => t.Shift).HasColumnName("Shift");
		Property((Prs1 t) => t.PSCode).HasColumnName("PSCode");
		Property((Prs1 t) => t.ProductionOrder).HasColumnName("ProductionOrder");
		Property((Prs1 t) => t.Line).HasColumnName("Line");
		Property((Prs1 t) => t.MachineTime).HasColumnName("MachineTime");
		Property((Prs1 t) => t.NumMachine).HasColumnName("NumMachine");
	}
}
