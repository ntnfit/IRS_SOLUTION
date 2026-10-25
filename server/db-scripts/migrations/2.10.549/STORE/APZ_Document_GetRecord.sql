
CALL APZ_EXISTSPROC('APZ_Document_GetRecord');

GO

CREATE  PROCEDURE APZ_Document_GetRecord(
in ObjType int default 0,
in tbl NVARCHAR(10) ,
in DocNum int default 0,
in TypeRecord nvarchar(1) default '',
in userCode nvarchar(25)
)
as
BEGIN
	 DECLARE SQLQUERY NVARCHAR(50000) = '';
	 Declare id int;
	 declare isInsQuery nvarchar(100) = '';
	 
	 if :ObjType in (13, 18) then
	 	isInsQuery := ' and "isIns"=''N'' ';
	 elseif :ObjType in (13000, 25) then
	 	isInsQuery := ' and "isIns"=''Y'' ';
	 end if;
	 
	 if :TypeRecord = 'F' or :TypeRecord = 'L' then
	 	exec 'DO BEGIN
	 			create local temporary table #tblOwner("UserId" int, "UserCode" nvarchar(40), "OwnerId" int, "SlpId" int, "ObjType" nvarchar(20), "Authorize" nvarchar(1));
	 			call apz_func_OwnerOverShip(N''' || :userCode || ''',''' || :ObjType || ''', OwnerView);
	 			insert into #tblOwner select * from :OwnerView;
	 		END';
	 		
	 	SQLQUERY := 'SELECT top 1 a."DocEntry" , a."DocNum" FROM ' || :tbl || ' a
		 		WHERE  (IFNULL(a."OwnerCode", -1) = -1 or a."OwnerCode" in (select distinct "OwnerId" from #tblOwner))' || :isInsQuery ||
				'AND (IFNULL(a."BPLId", -1) = -1 or a."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = ''N'' and y."UserCode" = ''' || :userCode || '''))
				ORDER BY ' || case when :TypeRecord = 'F' then '"DocNum", "DocEntry";' else '"DocNum" desc, "DocEntry" desc;' end;
				
		execute immediate (:SQLQUERY);
		
	 else -- 'N', 'P'
	 	declare tempNum int;
	 	
	 	exec 'DO BEGIN
				call apz_func_OwnerOverShip(N''' || :userCode || ''',''' || :ObjType || ''', OwnerView);
				create local temporary table #CTE ("DocNum" int, "DocEntry" int);
				insert into #cte select "DocNum", "DocEntry" from ' || :tbl || ' a
				WHERE (IFNULL(a."OwnerCode", -1) = -1 or a."OwnerCode" in (select distinct "OwnerId" from :OwnerView))' || :isInsQuery ||
				'AND (IFNULL(a."BPLId", -1) = -1 or a."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = ''N'' and y."UserCode" = ''' || :userCode || '''));
			END';
			
		if :TypeRecord = 'N' then
			exec 'SELECT MAX("DocNum") from #cte' into tempNum default 0;
			if :docNum >= :tempNum then
				execute immediate ('SELECT TOP 1 * FROM #cte ORDER BY "DocNum", "DocEntry"');
				return;
			end if;
			
			SQLQUERY := 'SELECT TOP 1 * FROM #cte WHERE "DocNum" > $1 ORDER BY "DocNum", "DocEntry"';
		else
			exec 'SELECT MIN("DocNum") from #cte' into tempNum default 0;
			if :docNum <= :tempNum then
				execute immediate ('SELECT TOP 1 * FROM #cte ORDER BY "DocNum" DESC, "DocEntry" DESC');
				return;
			end if;
			
			SQLQUERY := 'SELECT TOP 1 * FROM #cte WHERE "DocNum" < $1 ORDER BY "DocNum" DESC, "DocEntry" DESC';
		end if;
		
		execute immediate (:SQLQUERY) using :docNum;
		
	 end if;         
END