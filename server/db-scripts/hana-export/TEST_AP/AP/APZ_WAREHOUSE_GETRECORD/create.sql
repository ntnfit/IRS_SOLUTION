
Create Procedure APZ_WareHouse_GetRecord(in position nvarchar(1),in whsCode nvarchar(50) default '')
As
Begin
	declare CCode nvarchar(50);
	Declare firstCode nvarchar(255); 
	Declare lastCode nvarchar(255) ;
	select Ifnull((select top 1 "WhsCode" from OWHS ORDER by "WhsCode"),'') into firstCode from dummy;
	select Ifnull((select top 1 "WhsCode" from OWHS  ORDER by "WhsCode" desc),'') into lastCode from dummy;
	if position='F' OR (:whsCode = :lastCode AND :position = 'N')
	then
	select ifnull((select top 1 "WhsCode" from OWHS order by "WhsCode"),'') into CCode from dummy;
		
	end if;
	
	if position='L' OR (:whsCode = :firstCode AND :position = 'P')
	then
	select ifnull((select top 1 "WhsCode" from OWHS order by "WhsCode" desc),'') into CCode from dummy;
	end if;
	
	if position='P' and :whsCode <> :firstCode
	then
		WITH CTE AS (  select  ROW_NUMBER() OVER (ORDER BY "WhsCode") as RN, "WhsCode" FROM OWHS)
		SELECT 
	            Ifnull(prev."WhsCode",'') into CCode
					FROM CTE 
	            LEFT JOIN CTE prev ON prev.RN = CTE.RN - 1
	    WHERE CTE."WhsCode" = :whsCode;
	end if;
	
	if position='N' and :whsCode <> :lastCode
	then
		WITH CTE AS (  select  ROW_NUMBER() OVER (ORDER BY "WhsCode") as RN, "WhsCode" FROM OWHS)
		SELECT 
	               Ifnull(next."WhsCode",'') into CCode
					FROM CTE 
	            LEFT JOIN CTE next ON next.RN = CTE.RN + 1
	    WHERE CTE."WhsCode" =:whsCode ;
	end if;
	
	select 
		*
	FROM OWHS
	where "WhsCode"=:CCode;
End;




