using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class MRP2ImplMap : EntityTypeConfiguration<MRP2Impl>
{
	public MRP2ImplMap()
	{
		HasKey((MRP2Impl t) => t.Id);
		Property((MRP2Impl t) => t.Item).HasMaxLength(50);
		Property((MRP2Impl t) => t.Notes).HasMaxLength(300);
		Property((MRP2Impl t) => t.Line).HasMaxLength(50);
		Property((MRP2Impl t) => t.TypeName).HasMaxLength(250);
		ToTable("apz_MRP2");
		Property((MRP2Impl t) => t.Id).HasColumnName("Id");
		Property((MRP2Impl t) => t.Type).HasColumnName("Type");
		Property((MRP2Impl t) => t.WorkDay).HasColumnName("WorkDay");
		Property((MRP2Impl t) => t.Qty).HasColumnName("Qty");
		Property((MRP2Impl t) => t.Item).HasColumnName("Item");
		Property((MRP2Impl t) => t.Notes).HasColumnName("Notes");
		Property((MRP2Impl t) => t.MRPID).HasColumnName("MRPID");
		Property((MRP2Impl t) => t.Line).HasColumnName("Line");
		Property((MRP2Impl t) => t.TypeName).HasColumnName("TypeName");
	}
}
