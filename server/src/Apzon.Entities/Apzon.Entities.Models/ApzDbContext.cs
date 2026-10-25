using System.Data.Entity;
using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.Entities.Models;

public class ApzDbContext : DbContext
{
	public DbSet<Shift> Shifts { get; set; }

	public DbSet<WorkSchedulingLines> WorkSchedulingLineses { get; set; }

	public DbSet<WorkScheduling> WorkSchedulings { get; set; }

	public DbSet<Mps> Mpses { get; set; }

	public DbSet<Mrp> Mrps { get; set; }

	public DbSet<Mrp1> Mrp1s { get; set; }

	public DbSet<Mrp2> Mrp2s { get; set; }

	public DbSet<MrpRunCapacity> MrpRunCapacities { get; set; }

	public DbSet<Prs> Prses { get; set; }

	public DbSet<Prs1> Prs1s { get; set; }

	public DbSet<Prs2> Prs2s { get; set; }

	static ApzDbContext()
	{
		Database.SetInitializer<ApzDbContext>(null);
	}

	public ApzDbContext(string connectionString)
		: base(connectionString)
	{
		Database.Connection.ConnectionString = connectionString;
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		modelBuilder.Configurations.Add(new ShiftMap());
		modelBuilder.Configurations.Add(new WorkSchedulingLinesMap());
		modelBuilder.Configurations.Add(new WorkSchedulingMap());
		modelBuilder.Configurations.Add(new MpsMap());
		modelBuilder.Configurations.Add(new MrpMap());
		modelBuilder.Configurations.Add(new Mrp1Map());
		modelBuilder.Configurations.Add(new Mrp2Map());
		modelBuilder.Configurations.Add(new MrpRunCapacityMap());
		modelBuilder.Configurations.Add(new PrsMap());
		modelBuilder.Configurations.Add(new Prs1Map());
		modelBuilder.Configurations.Add(new Prs2Map());
	}
}
