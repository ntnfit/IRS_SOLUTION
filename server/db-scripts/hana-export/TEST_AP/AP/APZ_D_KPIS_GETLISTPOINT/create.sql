
CREATE PROCEDURE APZ_D_KPIs_GetListPoint(in code nvarchar(50) default '')
AS
BEGIN
	SELECT B."AbsEntry", A."Code", A."Name", B."Remarks", B."CreateDate", B."CreateTime", B."UpdateDate", B."UpdateTime", B."UserSign", B."UserSign2", C."Period", C."ViewType"
			, (SELECT "FromDate" from (select SETTING."FromDate", row_number() over(order by SETTING."Code") LineId FROM APZ_D_OKPM SETTING WHERE "Code" = C."Code" AND "Period" = C."Period") xx where LineId  = 1) AS "FromDate"
			, (SELECT "ToDate" from (select SETTING."ToDate", row_number() over(order by SETTING."Code") LineId FROM APZ_D_OKPM SETTING WHERE "Code" = C."Code" AND "Period" = C."Period") xx where LineId = 1) AS "ToDate" 
		FROM APZ_D_KPIS A 
			INNER JOIN (SELECT DISTINCT "Code", "Period", "ViewType" FROM APZ_D_OKPM) AS C ON A."Code" = C."Code" 
			LEFT OUTER JOIN APZ_D_OKPR B ON A."Code" = B."Code" AND B."Period" = C."Period" 
		WHERE (:code = '' OR A."Code" = :code);
	--order by Code,Period,FromDate,ToDate
END
--exec APZ_D_KPIs_GetListPoint



