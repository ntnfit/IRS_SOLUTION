using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.Mrps;

public class MrpMap : EntityTypeConfiguration<MRPImpl>
{
	public MrpMap()
	{
		HasKey((MRPImpl t) => t.Id);
		Property((MRPImpl t) => t.MrpCode).HasMaxLength(50);
		Property((MRPImpl t) => t.MrpName).HasMaxLength(300);
		Property((MRPImpl t) => t.MpsCode).HasMaxLength(50);
		Property((MRPImpl t) => t.Whs).HasMaxLength(50);
		Property((MRPImpl t) => t.Notes).HasMaxLength(300);
		ToTable("apz_OMRP");
		Property((MRPImpl t) => t.Id).HasColumnName("Id");
		Property((MRPImpl t) => t.BeginDate).HasColumnName("BeginDate");
		Property((MRPImpl t) => t.MrpCode).HasColumnName("MrpCode");
		Property((MRPImpl t) => t.MrpName).HasColumnName("MrpName");
		Property((MRPImpl t) => t.EndDate).HasColumnName("EndDate");
		Property((MRPImpl t) => t.Status).HasColumnName("Status");
		Property((MRPImpl t) => t.CreateDate).HasColumnName("CreateDate");
		Property((MRPImpl t) => t.MpsCode).HasColumnName("MpsCode");
		Property((MRPImpl t) => t.Whs).HasColumnName("Whs");
		Property((MRPImpl t) => t.Notes).HasColumnName("Notes");
		Property((MRPImpl t) => t.FixDate).HasColumnName("FixDate");
		Property((MRPImpl t) => t.ForeCast).HasColumnName("ForeCast");
		Property((MRPImpl t) => t.Plant).HasColumnName("Plant");
		Property((MRPImpl t) => t.Calendar).HasColumnName("Calendar");
		Property((MRPImpl t) => t.Remark).HasColumnName("Remark");
		Property((MRPImpl t) => t.Type).HasColumnName("Type");
	}
}
