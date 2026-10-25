-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_SetLineSeqAtPMN5(IN object nvarchar (3))
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

delete from SetLineSeqAtPMN5_tab;

tabName := 'TEMP_' || :object ||'5';

select count(*) into e1 
from "PUBLIC".sequences  
where sequence_name = 'MY_SEQ' and schema_name = current_schema; 

if :e1 <> 0 then
	exec('DROP SEQUENCE my_seq');
end if;

exec('CREATE SEQUENCE my_seq INCREMENT BY 1 START WITH 1');
exec('insert into SetLineSeqAtPMN5_tab select my_seq.nextval,"DocNum" from ' || :tabName 
||' order by "DocNum","InvEntry","LineNum","BaseObjArr","BaseObj","InstlmntId"');

select count(*) into rowCount from SetLineSeqAtPMN5_tab;

Oline := 0;
LBaseEnt := 0;
i := 1;
while :i <= :rowCount do
		select "DocNum" into BaseEnt from SetLineSeqAtPMN5_tab where "RowNum" = :i;
		
		if Not (:BaseEnt = :LBaseEnt)  then
			Oline := 0;
		end if;

		exec ('Update TEMP_' || :object || '5 Set "LineSeq" = ' || :Oline);

		LBaseEnt := :BaseEnt;
		Oline := :Oline + 1;
		
		i := :i + 1;
end while;

exec('DROP SEQUENCE my_seq');
delete from SetLineSeqAtPMN5_tab;
end;


