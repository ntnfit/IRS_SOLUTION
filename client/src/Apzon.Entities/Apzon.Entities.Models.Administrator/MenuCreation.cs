using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Administrator;

public class MenuCreation
{
	public int PluginId { get; set; }

	public string MenuCode { get; set; }

	public string Caption { get; set; }

	public string ParentMenuCode { get; set; }

	public int Position { get; set; }

	public Enums.MenuCreationType Type { get; set; }
}
