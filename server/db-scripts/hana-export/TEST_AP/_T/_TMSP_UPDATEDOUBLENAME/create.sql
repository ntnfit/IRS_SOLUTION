-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables BEFORE:PT:PROCESS_START AFTER:SP:_TmSp_ValidateSpParam

--by using sequence and temporary table instead of dynamic cursor used in procedure MSSQL 2005;
--CREATE SEQUENCE my_s INCREMENT BY 1 START WITH 1;
--CREATE SEQUENCE my_s2 INCREMENT BY 1 START WITH 1;
CREATE PROCEDURE _TmSp_UpdateDoubleName (
   in	tableName nvarchar(128),
   in	groupfield nvarchar(128))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
     Name	nvarchar (4000);
     Name1	nvarchar (4000);
     NameTmp	nvarchar (4000);
     count1		int;
     number		int;
     maxlen		int;
     next1   int := 1;
     max1 int;
     max2 int;
     next2   int := 1;
     cond int := 0;
     tmpTableName nvarchar(5000);

begin

--Parameters validation for security issues
	call _TmSp_ValidateSpParam(:tableName);
	call _TmSp_ValidateSpParam(:groupfield);
-------------------------------------------------------------
--	CREATE SEQUENCE my_s INCREMENT BY 1 START WITH 1;

	delete from "UpdateDoubleName_tmp_table1";
	delete from "UpdateDoubleName_tmp_table2";
	
	select "PUBLIC"."TABLE_COLUMNS".length into maxlen from
                 "PUBLIC"."TABLE_COLUMNS" inner Join "PUBLIC".objects on
                  "PUBLIC".objects.object_oid = "PUBLIC"."TABLE_COLUMNS".table_oid
                  where "PUBLIC".objects.object_name = :tableName--'ARCT' 
				  and  "PUBLIC"."TABLE_COLUMNS".column_name = :groupfield--'DdctSum' 
				  and "PUBLIC".objects.schema_name = current_schema;
--select @maxlen
    tmpTableName := 'TEMP_'||:tableName;
	exec ('Update "'|| :tmpTableName 
	  ||'" Set "'|| :groupfield ||'" = '''''||' Where "'
	  || :groupfield ||'" is NULL');
 
	exec ('Update "'|| :tmpTableName 
	  ||'" Set "'|| :groupfield ||'" = REPLACE ("'|| :groupfield ||'",''39'',''70'')');
	    
/*	exec ('
		declare Currs 		CURSOR 
		for 
		select '+ @groupfield +',count (1)
		from TEMP_'+ @table +'
		group by '+ @groupfield +'
		having count (1)>1
	');
*/	
	exec ('insert into "UpdateDoubleName_tmp_table1" '
			||' SELECT my_s.NEXTVAL, "'||:groupfield||'", count(1) from "'||:tmpTableName
				||'" group by "'||:groupfield
					||'" having count (1)>1');
	
--	Open Currs;
    select max("seq") into max1 from "UpdateDoubleName_tmp_table1";
   
    max1 := :max1 + 1;
    next1 := 1;
    
    while(next1 < max1) 
    do
	--	Fetch Currs Into Name,count1;
		select "count11" into count1 from "UpdateDoubleName_tmp_table1" T0 where T0."seq" = next1;
		select "gfield" into Name  from "UpdateDoubleName_tmp_table1" T0 where T0."seq" = next1;
	--	call innerSP(tableName, Name);
/*		exec ('
		 declare Currs1 		CURSOR 
		 for 
		 select '+ @groupfield +'
		 from TEMP_'+ @table +'
		 where '+ @groupfield +' = '''+ @Name +'''
		 For Update
		');
*/	
	--	 Open Currs1;
				
	    exec ('insert into "UpdateDoubleName_tmp_table2" '
	    	 ||'  select my_s2.NEXTVAL,  "'||:groupfield
	   		 ||'"     from "'||:tmpTableName
	    		||'"    where "'||:groupfield||'" = '||:Name);
	        
	    select max("seq") into max2 from "UpdateDoubleName_tmp_table2";		
	    		
		number := 0;
		
		 next2 :=1;
		 
		 While ( :next2 < max2) 
		 do
		 --select @Name N,@Name1 N1
		 --	Fetch  Currs1 Into Name1;
		 	select "gfield" into Name1 from "UpdateDoubleName_tmp_table2" where "seq" = :next2;
			
			NameTmp := :Name1;
			
			IF :number > 0 then
				Name1 := :Name1 || '_' || cast(:number as nchar(6));
			end if;
			 
			IF length(Name1) > :maxlen then
				 Name1 := right (:Name1,:maxlen);
		   	end if;
			   	
			exec ('Update "'|| :tmpTableName 
			   	||'" Set "'|| :groupfield ||'" = '||:Name1 
			   	||' Where "'||:groupfield||'" = '||:NameTmp);
			   	
			number := :number + 1;			  
			next2 := :next2 + 1;			
		 end while;
		 
		 next1 := :next1 + 1;
    end while;

--    close Currs;
    
    exec ('Update "'|| :tmpTableName   --treated as normal table, not temporary table, in order to update; 
    	||'" Set "'|| :groupfield ||'" = REPLACE ("'|| :groupfield ||'",''70'',''39'')');
    	
    delete from "UpdateDoubleName_tmp_table1";
	delete from "UpdateDoubleName_tmp_table2";
end;


