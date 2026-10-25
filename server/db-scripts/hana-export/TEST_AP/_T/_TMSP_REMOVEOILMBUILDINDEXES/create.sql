-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_RemoveOILMBuildIndexes
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

as
   ntmp int;
begin
	SELECT count(*) into ntmp FROM "PUBLIC".indexes WHERE Table_name = N'OILM' AND index_name = N'OILM_CARD';

	IF  :ntmp >0 then 
		exec('DROP INDEX "OILM_CARD"');
	end if;

	SELECT count(*) into ntmp FROM "PUBLIC".indexes WHERE Table_name = N'OILM' AND index_name = N'OILM_DocLine';

	IF  :ntmp > 0 then 
		exec('DROP INDEX "OILM_DocLine"');
	end if;
	SELECT count(*) into ntmp FROM "PUBLIC".indexes WHERE Table_name = N'OILM' AND index_name = N'OILM_APPOBJ';

	IF  :ntmp > 0 then 
		exec('DROP INDEX "OILM_APPOBJ"');
	end if;
end;


