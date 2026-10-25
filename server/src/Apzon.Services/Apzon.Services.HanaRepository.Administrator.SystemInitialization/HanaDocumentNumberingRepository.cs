using System.Collections.Generic;
using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.HanaRepository.Administrator.SystemInitialization;

public class HanaDocumentNumberingRepository : DocumentNumberingRepository
{
	public HanaDocumentNumberingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetLoadDocQuery(IEnumerable<string> lstDocNumber, int userId)
	{
		return "SELECT a.\"ObjectCode\" as \"ObjectCode\", a.\"ObjectCode\" || IFNULL(NULLIF(a.\"DocSubType\",'--'),'') \"Code\", \"SeriesName\", \"InitialNum\",\"Series\",\"NextNumber\",A.\"DocSubType\",\"LastNum\",\"DocAlias\"\r\n                    ,case a.\"ObjectCode\" when '2' then 'B' when '4' then 'I' when '290' then 'R' else 'D' end \"SeriesType\" \r\n                    FROM ONNM a\r\n                    INNER JOIN NNM1 b ON a.\"ObjectCode\" = b.\"ObjectCode\" AND b.\"DocSubType\" = a.\"DocSubType\" AND b.\"Series\" = IFNULL((select min(\"Series\") from NNM2 where \"ObjectCode\" = a.\"ObjectCode\" and \"UserSign\" = @userId and \"DocSubType\" = a.\"DocSubType\"), a.\"DfltSeries\")\r\n                    WHERE a.\"ObjectCode\" || IFNULL(NULLIF(a.\"DocSubType\",'--'),'') in (" + string.Join(",", lstDocNumber) + ") ORDER BY \"ObjectCode\" ASC;";
	}
}
