using System.Data.Entity.ModelConfiguration;

namespace Apzon.Entities.Models.PlanningProduction;

public class ShiftMap : EntityTypeConfiguration<Shift>
{
	public ShiftMap()
	{
		HasKey((Shift t) => t.Code);
		ToTable("apz_OSHP");
		Property((Shift t) => t.Code).HasColumnName("Code");
		Property((Shift t) => t.Name).HasColumnName("Name");
		Property((Shift t) => t.Time).HasColumnName("Time");
		Property((Shift t) => t.Notes).HasColumnName("Notes");
		Property((Shift t) => t.Start1).HasColumnName("Start1");
		Property((Shift t) => t.End1).HasColumnName("End1");
		Property((Shift t) => t.StartBreak1).HasColumnName("StartBreak1");
		Property((Shift t) => t.EndBreak1).HasColumnName("EndBreak1");
		Property((Shift t) => t.Start2).HasColumnName("Start2");
		Property((Shift t) => t.End2).HasColumnName("End2");
		Property((Shift t) => t.StartBreak2).HasColumnName("StartBreak2");
		Property((Shift t) => t.EndBreak2).HasColumnName("EndBreak2");
		Property((Shift t) => t.Time2).HasColumnName("Time2");
	}
}
