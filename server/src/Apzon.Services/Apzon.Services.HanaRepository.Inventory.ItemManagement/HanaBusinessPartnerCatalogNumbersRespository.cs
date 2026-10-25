using Apzon.Commons.Enums;
using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.HanaRepository.Inventory.ItemManagement;

public class HanaBusinessPartnerCatalogNumbersRespository : BusinessPartnerCatalogNumbersRespository
{
	public HanaBusinessPartnerCatalogNumbersRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDataQuery(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType)
	{
		switch (type)
		{
		case Enums.NavigateType.Previous:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                            INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" \r\n                                    FROM oscn WHERE \"CardCode\" < @parm ORDER BY \"CardCode\" DESC), '');\r\n                                SELECT a.\"CardCode\", b.\"CardName\", a.\"ItemCode\", c.\"ItemName\", a.\"Substitute\", a.\"ShowSCN\" FROM oscn a \r\n                                        INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" \r\n                                WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" FROM oscn ORDER BY \"CardCode\" DESC), '');";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "SELECT TOP 1 a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                        INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn\r\n                                    WHERE \"ItemCode\" < @parm ORDER BY \"ItemCode\" DESC), '');\r\n                                SELECT a.\"CardCode\", b.\"CardName\", a.\"ItemCode\", c.\"ItemName\", a.\"Substitute\", a.\"ShowSCN\" FROM oscn a \r\n                                        INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" \r\n                                WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn ORDER BY \"ItemCode\" DESC), '');";
			}
			break;
		case Enums.NavigateType.Next:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                        INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" FROM oscn\r\n                                    WHERE \"CardCode\" > @parm ORDER BY \"CardCode\" ASC), '');\r\n                                SELECT a.\"CardCode\", b.\"CardName\", a.\"ItemCode\", c.\"ItemName\", a.\"Substitute\", a.\"ShowSCN\" FROM oscn a \r\n                                    INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" \r\n                                WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" FROM oscn ORDER BY \"CardCode\" ASC), '');";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                        INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn\r\n                                WHERE \"ItemCode\" > @parm ORDER BY \"ItemCode\" ASC), '');\r\n                                SELECT a.\"CardCode\", b.\"CardName\", a.\"ItemCode\", c.\"ItemName\", a.\"Substitute\", a.\"ShowSCN\" FROM oscn a \r\n                                    INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" \r\n                                WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn ORDER BY \"ItemCode\" ASC), '');";
			}
			break;
		case Enums.NavigateType.Last:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                        INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" FROM oscn ORDER BY \"CardCode\" DESC), '');";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn ORDER BY \"ItemCode\" DESC), '');\r\n";
			}
			break;
		case Enums.NavigateType.First:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                                INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"CardCode\" = IFNULL((SELECT TOP 1 \"CardCode\" FROM oscn ORDER BY \"CardCode\" ASC), '');";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "SELECT a.*, b.\"CardName\", c.\"ItemName\" FROM oscn a INNER JOIN ocrd b ON a.\"CardCode\" = b.\"CardCode\" \r\n                            INNER JOIN OITM c ON a.\"ItemCode\" = c.\"ItemCode\" WHERE a.\"ItemCode\" = IFNULL((SELECT TOP 1 \"ItemCode\" FROM oscn ORDER BY \"ItemCode\" ASC), '');";
			}
			break;
		}
		return "";
	}
}
