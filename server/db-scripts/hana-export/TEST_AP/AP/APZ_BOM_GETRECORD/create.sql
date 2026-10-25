
Create Procedure apz_BOM_GetRecord(in position nvarchar(1) default 'F',in Code nvarchar(50) default '')
As
Begin
declare CCode nvarchar(50);

Declare firstCode nvarchar(255); 
Declare lastCode nvarchar(255) ;
select Ifnull((select top 1 "CardCode" from OCRD ORDER by "CardCode"),'') into firstCode from dummy;
select Ifnull((select top 1 "CardCode" from OCRD  ORDER by "CardCode" desc),'') into lastCode from dummy;
if position='F' OR (:Code = :lastCode AND :position = 'N')
then
select ifnull((select top 1 "Code" from OITT order by "Code"),'') into CCode from dummy;
	
end if;

if position='L' OR (:Code = :firstCode AND :position = 'P')
then
select ifnull((select top 1 "Code" from OITT order by "Code" desc),'') into CCode from dummy;
end if;

if position='P'
then
	WITH CTE AS (  select  ROW_NUMBER() OVER (ORDER BY "Code") as RN, "Code" FROM OITT)
	SELECT 
            Ifnull(prev."Code",'') into CCode
				FROM CTE 
            LEFT JOIN CTE prev ON prev.RN = CTE.RN - 1
    WHERE CTE."Code" = :Code;
end if;

if position='N'
then
	WITH CTE AS (  select  ROW_NUMBER() OVER (ORDER BY "Code") as RN, "Code" FROM OITT)
	SELECT 
               Ifnull(next."Code",'') into CCode
				FROM CTE 
            LEFT JOIN CTE next ON next.RN = CTE.RN + 1
    WHERE CTE."Code" =:Code ;
end if;

SELECT 
	a.*
	,a."Name" as "ItemName"
FROM OITT a left join oitm b on a."Code"=b."ItemCode" 
WHERE "Code" = :CCode;
End;




