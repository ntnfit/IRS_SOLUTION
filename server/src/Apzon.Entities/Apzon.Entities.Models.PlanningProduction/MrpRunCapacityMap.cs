using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class MrpRunCapacityMap : EntityTypeConfiguration<MrpRunCapacity>
{
	public MrpRunCapacityMap()
	{
		HasKey((MrpRunCapacity t) => t.Id);
		Property((MrpRunCapacity t) => t.Item).HasMaxLength(50);
		Property((MrpRunCapacity t) => t.TypeName).HasMaxLength(250);
		ToTable("APZ_MRPRun_Capacity");
		Property((MrpRunCapacity t) => t.Id).HasColumnName("Id");
		Property((MrpRunCapacity t) => t.Type).HasColumnName("Type");
		Property((MrpRunCapacity t) => t.WorkDay).HasColumnName("WorkDay");
		Property((MrpRunCapacity t) => t.Qty).HasColumnName("Qty");
		Property((MrpRunCapacity t) => t.Item).HasColumnName("Item");
		Property((MrpRunCapacity t) => t.MRPID).HasColumnName("MRPID");
		Property((MrpRunCapacity t) => t.TypeName).HasColumnName("TypeName");
	}
}
