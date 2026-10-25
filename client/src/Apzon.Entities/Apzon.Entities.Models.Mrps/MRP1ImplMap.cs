using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class MRP1ImplMap : EntityTypeConfiguration<MRP1Impl>
{
	public MRP1ImplMap()
	{
		HasKey((MRP1Impl t) => t.Id);
		Property((MRP1Impl t) => t.ItemCode).HasMaxLength(50);
		Property((MRP1Impl t) => t.DocumentType).HasMaxLength(50);
		Property((MRP1Impl t) => t.Line).HasMaxLength(50);
		ToTable("apz_MRP1");
		Property((MRP1Impl t) => t.Id).HasColumnName("Id");
		Property((MRP1Impl t) => t.ItemCode).HasColumnName("ItemCode");
		Property((MRP1Impl t) => t.ItemName).HasColumnName("ItemName");
		Property((MRP1Impl t) => t.Qty).HasColumnName("Qty");
		Property((MRP1Impl t) => t.ConfirmQty).HasColumnName("ConfirmQty");
		Property((MRP1Impl t) => t.DueDate).HasColumnName("DueDate");
		Property((MRP1Impl t) => t.DocumentType).HasColumnName("DocumentType");
		Property((MRP1Impl t) => t.Notes).HasColumnName("Notes");
		Property((MRP1Impl t) => t.OMrpId).HasColumnName("OMrpId");
		Property((MRP1Impl t) => t.Type).HasColumnName("Type");
		Property((MRP1Impl t) => t.PONo).HasColumnName("PONo");
		Property((MRP1Impl t) => t.OB).HasColumnName("OB");
		Property((MRP1Impl t) => t.Line).HasColumnName("Line");
		Property((MRP1Impl t) => t.LTime).HasColumnName("LTime");
		Property((MRP1Impl t) => t.MachineTime).HasColumnName("MachineTime");
		Property((MRP1Impl t) => t.NumMachine).HasColumnName("NumMachine");
		Property((MRP1Impl t) => t.GomNgay).HasColumnName("GomNgay");
		Property((MRP1Impl t) => t.ControlArea).HasColumnName("ControlArea");
	}
}
