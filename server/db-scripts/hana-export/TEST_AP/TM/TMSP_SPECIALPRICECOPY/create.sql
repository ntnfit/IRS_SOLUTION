-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:SP:_TmSp_ValidateSpParam

create procedure TmSp_SpecialPriceCopy (in targetCrd nvarchar(15),in sourceCrd nvarchar(15),in userID int,In class int,in tempTableSuffix nvarchar(10))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
tempname nvarchar(100);
table_nm nvarchar(100);
sql_str nvarchar(200);
cnt int;
ospp nvarchar(100);
spp1 nvarchar(100);
spp2 nvarchar(100);
targetCrd1 nvarchar(15);
sourceCrd1 nvarchar(15);
SQ NVARCHAR(1000);
i int;
begin
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
call _TmSp_ValidateSpParam(:targetCrd);
call _TmSp_ValidateSpParam(:sourceCrd);
call _TmSp_ValidateSpParam(:tempTableSuffix);

targetCrd1 := :targetCrd;
sourceCrd1 := :sourceCrd;
targetCrd1 := ltrim(:targetCrd1);
targetCrd1 := rtrim(:targetCrd1);
sourceCrd1 := ltrim(:sourceCrd1);
sourceCrd1 := rtrim(:sourceCrd1);
SELECT 'temp_'|| CURRENT_CONNECTION into tempname FROM DUMMY;

delete from "TMP_SpecialPriceCopy_RowCount";

if length (:tempTableSuffix) = 0
then
	--ins_msg_proc ('tempTableSuffix = 0');
	ospp := 'OSPP';
	spp1 := 'SPP1';
	spp2 := 'SPP2';
else
	--ins_msg_proc ('tempTableSuffix = ' || :tempTableSuffix);
	ospp := '#ospp_' || :tempTableSuffix;
	spp1 := '#spp1_' || :tempTableSuffix;
	spp2 := '#spp2_' || :tempTableSuffix;
end if;

if :class = 1 
then
	-- Check availability
	if :ospp <> 'OSPP' then
		delete from "TMP_SpecialPriceCopy_RowCount";
		SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like ''' || :ospp ||'%''';
	    --ins_msg_proc (:SQ);
        EXEC (:SQ);
        select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
      	if :cnt = 0 then
      		--ins_msg_proc ('we don not have it');
			delete from "TMP_SpecialPriceCopy_RowCount";
			return;
		end if;
	end if;

	delete from "TMP_SpecialPriceCopy_RowCount";
	SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || ''')';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
	if :cnt = 0 then
	    delete from "TMP_SpecialPriceCopy_RowCount";
		return;
	end if;

	cnt := 1;
	i := -1;
	WHILE :cnt <> 0 DO
		i := :i + 1;
		table_nm := :tempname || :i;
		SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
	END WHILE;

	SQ := 'create column table ' || :table_nm || ' as (select * from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
 	--ins_msg_proc (:SQ);
    EXEC (:SQ);
 
    SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || ''', "UserSign" =  N''' || :userID || '''';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);


	-- Remove old data
    SQ :='delete from "SPP2" where "CardCode" = N''' || :targetCrd1 || ''' and "ItemCode" in (select "ItemCode" from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''')';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
    
    SQ :='delete from "SPP1" where "CardCode" = N''' || :targetCrd1 || ''' and "ItemCode" in (select "ItemCode" from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''')';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
    
    SQ :='delete from "OSPP" where "CardCode" = N''' || :targetCrd1 || ''' and "ItemCode" in (select "ItemCode" from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''')';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
    
	-- Copy new data
    SQ :='insert into "OSPP" select * from ' || :table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
    SQ :='drop table ' || :table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	-- Check if there is temp table

    	
	if :spp1 <> 'SPP1' 	then
		delete from "TMP_SpecialPriceCopy_RowCount";
	    SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like ''' || :spp1 || '%''';
        --ins_msg_proc (:SQ);
        EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
		if :cnt = 0 then
        	delete from "TMP_SpecialPriceCopy_RowCount";
			return;
		end if;
	end if;
	

	cnt := 1;
	i := -1;
	WHILE :cnt <> 0 DO
		i := :i + 1;
		table_nm := :tempname || :i;
		SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
	END WHILE;

	SQ := 'create column table ' || :table_nm || ' as(select * from "' || :spp1 || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || '''';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	SQ :='insert into "SPP1" select * from ' || :table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	SQ :='drop table ' || table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);

		
	-- Check if there is temp table
	if :spp2 <> 'SPP2' THEN
		delete from "TMP_SpecialPriceCopy_RowCount"; 
		SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like N''' || :spp2 || '%''';
        --ins_msg_proc (:SQ);
        EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
		if :cnt = 0 then
       		delete from "TMP_SpecialPriceCopy_RowCount";
			return;
		end if;
	end if;
	
    cnt := 1;
	i := -1;
	WHILE :cnt <> 0 DO
		i := :i + 1;
		table_nm := :tempname || :i;
		SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
	END WHILE;
	
    SQ := 'create column table ' || :table_nm || ' as(select * from "' || :spp2 || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
   	
	SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || '''';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	SQ :='insert into "SPP2" select * from ' || :table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
	SQ :='drop table ' || table_nm || '';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
	
end if;

if :class = 2 
then
	-- Check availability
	if :ospp <> 'OSPP' then
		delete from "TMP_SpecialPriceCopy_RowCount";
		SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like N''' || :ospp || '%''';
        --ins_msg_proc (:SQ);
        EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
		if :cnt = 0 then
        	delete from "TMP_SpecialPriceCopy_RowCount";
			return;
		end if;
	end if;
	
	delete from "TMP_SpecialPriceCopy_RowCount";
	SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" not in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || ''')';
    --ins_msg_proc (:SQ);
    EXEC (:SQ);
    select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
	if :cnt = 0 then
        delete from "TMP_SpecialPriceCopy_RowCount";
		return;
	end if;
	
	
	if :spp1 <> 'SPP1'
	then
		delete from "TMP_SpecialPriceCopy_RowCount";
	    SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like N''' || :spp1 || '%''';
	    --ins_msg_proc (:SQ);
        EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
	else
		cnt := 1;
	end if;

	
	if :cnt <> 0
	then
	     cnt := 1;
		 i := -1;
		 WHILE :cnt <> 0 DO
			i := :i + 1;
			table_nm := :tempname || :i;
			SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
		 END WHILE;
	     SQ := 'create column table ' || :table_nm || ' as(select * from "' || :spp1 || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" not in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
         --ins_msg_proc (:SQ);
         EXEC (:SQ);
	     
	     SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || '''';
         --ins_msg_proc (:SQ);
         EXEC (:SQ);
	     
	     SQ :='insert into "SPP1" select * from ' || :table_nm || '';
         --ins_msg_proc (:SQ);
         EXEC (:SQ);
	     
	     SQ :='drop table ' || table_nm || '';
         --ins_msg_proc (:SQ);
         EXEC (:SQ);
	end if;

	if :spp2 <> 'SPP2' then
		delete from "TMP_SpecialPriceCopy_RowCount";
		SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like N''' || :spp2 || '%''';
    	EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
	else
		cnt := 1;
	end if;

	if :cnt <> 0 then
	    cnt := 1;
		i := -1;
		WHILE :cnt <> 0 DO
			i := :i + 1;
			table_nm := :tempname || :i;
			SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
		END WHILE;
	  	SQ := 'create column table ' || :table_nm || ' as(select * from "' || :spp2 || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" not in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
	    
	    SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || '''';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
	    
	    SQ :='insert into "SPP2" select * from ' || :table_nm || '';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
	   
	    SQ :='drop table ' || table_nm || '';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
	end if;

	if :ospp <> 'OSPP' then
		delete from "TMP_SpecialPriceCopy_RowCount";
		SQ :='insert into "TMP_SpecialPriceCopy_RowCount" select count(*) from "PUBLIC"."M_TEMPORARY_TABLES" where table_name like N''' || :ospp || '%''';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
		select top 1 "rownum" into cnt from "TMP_SpecialPriceCopy_RowCount";
	else
		cnt := 1;
	end if;
	
	if :cnt <> 0 then
		cnt := 1;
		i := -1;
		WHILE :cnt <> 0 DO
			i := :i + 1;
			table_nm := :tempname || :i;
			SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" where "TABLE_NAME" = :table_nm;		
		END WHILE;
		 SQ := 'create column table ' || :table_nm || ' as (select * from "' || :ospp || '" where "CardCode" = N''' || :sourceCrd1 || ''' and "ItemCode" not in (select "ItemCode" from "OSPP" where "CardCode" = N''' || :targetCrd1 || '''))' ;
    	--ins_msg_proc (:SQ);
		EXEC (:SQ);
     
	    SQ :='update ' || :table_nm || ' set "CardCode" = N''' || :targetCrd1 || ''', "UserSign" =  N''' || :userID || '''';
    	--ins_msg_proc (:SQ);
   	 	EXEC (:SQ);
	     
	    SQ :='insert into "OSPP" select * from ' || :table_nm || '';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);
	     
     	SQ :=' drop table ' || table_nm || '';
    	--ins_msg_proc (:SQ);
    	EXEC (:SQ);

	end if;
end if;

delete from "TMP_SpecialPriceCopy_RowCount";

end;


