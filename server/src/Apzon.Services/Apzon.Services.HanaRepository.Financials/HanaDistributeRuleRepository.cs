using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaDistributeRuleRepository : DistributeRuleRepository
{
	public HanaDistributeRuleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string SearchQuery(string ocrCode, string ocrName, int dimesion)
	{
		string text = "select a.*,(select \"ValidFrom\" from (Select \"ValidFrom\", row_number() over(order by t.\"PrcCode\") LineId  From OCR1 t Where t.\"OcrCode\" = a.\"OcrCode\") x where x.lineId = 1) \"ValidFrom\"\r\n\t       ,(select \"ValidTo\" from(Select \"ValidTo\", row_number() over(order by t.\"PrcCode\") LineId  From OCR1 t Where t.\"OcrCode\" = a.\"OcrCode\") x where x.lineId = 1) \"ValidTo\"\r\n                            from OOCR a WHERE 1=1 ";
		if (!string.IsNullOrEmpty(ocrCode))
		{
			text += "AND  a.\"OcrCode\" like '%' || @OcrCode || '%'";
		}
		if (!string.IsNullOrEmpty(ocrName))
		{
			text += "AND  a.\"OcrName\" like '%' || @OcrName || '%'";
		}
		if (dimesion > 0)
		{
			text += "AND  a.\"DimCode\" = @DimCode";
		}
		return text + ";";
	}

	protected override string GetSQLCheckQuery(string tableName, string column)
	{
		return $"select (select count(1) from {tableName} where \"{column}\"=@key) into countDist from DUMMY;\r\n                                    if countDist > 0 then\r\n                                        select 'This distribute rule is used in table'  as result from DUMMY;\r\n                                    end if;";
	}

	protected override string DeclareVariables(string key)
	{
		return $"declare countDist int=0;";
	}
}
