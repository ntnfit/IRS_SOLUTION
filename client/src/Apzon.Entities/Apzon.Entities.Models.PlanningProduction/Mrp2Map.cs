using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class Mrp2Map : EntityTypeConfiguration<Mrp2>
{
	public Mrp2Map()
	{
		HasKey((Mrp2 t) => t.Id);
		Property((Mrp2 t) => t.Item).HasMaxLength(50);
		Property((Mrp2 t) => t.Notes).HasMaxLength(300);
		Property((Mrp2 t) => t.Line).HasMaxLength(50);
		Property((Mrp2 t) => t.TypeName).HasMaxLength(250);
		ToTable("apz_MRP2");
		Property((Mrp2 t) => t.Id).HasColumnName("Id");
		Property((Mrp2 t) => t.Type).HasColumnName("Type");
		Property((Mrp2 t) => t.WorkDay).HasColumnName("WorkDay");
		Property((Mrp2 t) => t.Qty).HasColumnName("Qty");
		Property((Mrp2 t) => t.Item).HasColumnName("Item");
		Property((Mrp2 t) => t.Notes).HasColumnName("Notes");
		Property((Mrp2 t) => t.MRPID).HasColumnName("MRPID");
		Property((Mrp2 t) => t.Line).HasColumnName("Line");
		Property((Mrp2 t) => t.TypeName).HasColumnName("TypeName");
	}
}
