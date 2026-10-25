using System.Data;
using Apzon.Commons;
using Apzon.Services.Repository.Document;

namespace Apzon.Services.HanaRepository.Document;

public class HanaDocumentRepository : DocumentRepository
{
	public HanaDocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetFreightCodeQuery()
	{
		return "SELECT \"ExpnsCode\" AS \"Code\",\"ExpnsName\" AS \"Name\" FROM OEXD \r\n                        UNION ALL SELECT 0,'' from DUMMY;";
	}

	protected override string GetLineShipToQuery()
	{
		return "select *, \"Street\" || ' ' || \"ZipCode\" || ' ' || \"City\" || ifnull((Select \"Name\" from OCRY t where t.\"Code\"=a.\"Country\"), '') \"FullAddress\" \r\n                    from crd1 a where \"CardCode\" = cardCode and \"AdresType\"='S';";
	}

	protected override string GetDocumentNumberQuery()
	{
		return "select \"NextNumber\" from NNM1 WHERE \"Series\"=Series;";
	}

	protected override void MakeSourceView(ref DataTable dt)
	{
		foreach (DataRow row in dt.Rows)
		{
			if (Function.ToString(row["LineType"]) == "T")
			{
				row["ItemCode"] = row["LineText"];
			}
		}
		dt.AcceptChanges();
	}
}
