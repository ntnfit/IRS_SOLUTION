-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_SetLineNumAtDOC3 (IN object nvarchar (3))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
Oline    int;	
BaseEnt  int;
LBaseEnt int;
tabName  nvarchar(9);
e1 int;
rowCount int;
i int;

begin

delete from SetLineNumAtDOC3_tab;

tabName := 'TEMP_' || :object ||'3';

select count(*) into e1 
from "PUBLIC".sequences  
where sequence_name = 'MY_SEQ' and schema_name = current_schema; 

if :e1 <> 0 then
	exec('DROP SEQUENCE my_seq');
end if;

exec('CREATE SEQUENCE my_seq INCREMENT BY 1 START WITH 1');
exec('insert into SetLineNumAtDOC3_tab select my_seq.nextval,"DocEntry" from ' || :tabName ||' order by "DocEntry"');

select count(*) into rowCount from SetLineNumAtDOC3_tab;

Oline := 0;
LBaseEnt := 0;
i := 1;
while :i <= :rowCount do
		select "DocEntry" into BaseEnt from SetLineNumAtDOC3_tab where "RowNum" = :i;
		
		if Not (:BaseEnt = :LBaseEnt)  then
			Oline := 0;
		end if;

		exec ('Update TEMP_' || :object || '3 Set "LineNum" = ' || :Oline);

		LBaseEnt := :BaseEnt;
		Oline := :Oline + 1;
		
		i := :i + 1;
end while;

exec('DROP SEQUENCE my_seq');
delete from SetLineNumAtDOC3_tab;	
end;


