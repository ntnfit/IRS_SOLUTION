
Create Procedure apz_BOM_SearchADoc(in Code nvarchar(50))
As
Begin
SELECT 
	a.*
	,b."ItemName" as "ItemName"
FROM OITT a left join oitm b on a."Code"=b."ItemCode" 
WHERE "Code" = :Code;
End;




