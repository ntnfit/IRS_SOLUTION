using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class Mrp1Map : EntityTypeConfiguration<Mrp1>
{
	public Mrp1Map()
	{
		HasKey((Mrp1 t) => t.Id);
		Property((Mrp1 t) => t.ItemCode).HasMaxLength(50);
		Property((Mrp1 t) => t.DocumentType).HasMaxLength(50);
		Property((Mrp1 t) => t.Line).HasMaxLength(50);
		ToTable("apz_MRP1");
		Property((Mrp1 t) => t.Id).HasColumnName("Id");
		Property((Mrp1 t) => t.ItemCode).HasColumnName("ItemCode");
		Property((Mrp1 t) => t.ItemName).HasColumnName("ItemName");
		Property((Mrp1 t) => t.Qty).HasColumnName("Qty");
		Property((Mrp1 t) => t.ConfirmQty).HasColumnName("ConfirmQty");
		Property((Mrp1 t) => t.DueDate).HasColumnName("DueDate");
		Property((Mrp1 t) => t.DocumentType).HasColumnName("DocumentType");
		Property((Mrp1 t) => t.Notes).HasColumnName("Notes");
		Property((Mrp1 t) => t.OMrpId).HasColumnName("OMrpId");
		Property((Mrp1 t) => t.Type).HasColumnName("Type");
		Property((Mrp1 t) => t.PONo).HasColumnName("PONo");
		Property((Mrp1 t) => t.OB).HasColumnName("OB");
		Property((Mrp1 t) => t.Line).HasColumnName("Line");
		Property((Mrp1 t) => t.LTime).HasColumnName("LTime");
		Property((Mrp1 t) => t.MachineTime).HasColumnName("MachineTime");
		Property((Mrp1 t) => t.NumMachine).HasColumnName("NumMachine");
		Property((Mrp1 t) => t.GomNgay).HasColumnName("GomNgay");
	}
}
