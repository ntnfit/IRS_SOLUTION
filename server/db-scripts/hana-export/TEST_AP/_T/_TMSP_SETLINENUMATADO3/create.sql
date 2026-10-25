-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE  procedure _TmSp_SetLineNumAtADO3
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
-------------------------------------------------------------
Oline     int;
BaseEnt   int;
LBaseEnt  int;
lobject	  smallint;
lloginst  smallint;
objectType	  smallint;
loginst	  smallint;
hasNext   int := 1;

--CURSOR  Currs for 
--	select  "DocEntry", "ObjType", "LogInstanc" 
--	From TEMP_ADO3
--	Order By "DocEntry", "ObjType", "LogInstanc";

begin

	Oline := 0;
	LBaseEnt := 0;
	lobject := 0;
	lloginst := 0;

--	Open Currs;
	
--	Fetch Currs Into BaseEnt, objectType, loginst;
--	if not Currs::NOTFOUND then
--		hasNext := -1;
--	end if;
	
	While :hasNext > 0 do
		
		if (Not (:BaseEnt = :LBaseEnt)) OR (Not (:objectType = :lobject))
		then
			Oline := 0;
		end if;
		
		if (:Oline > 0) AND NOT(:lloginst = :loginst)
		then
			Oline := :Oline - 1;
		end if;

		exec ('Update TEMP_ADO3 Set "LineNum" = ' || :Oline );
		
		LBaseEnt := :BaseEnt;
		lobject := :objectType;
		Oline := :Oline + 1;
		lloginst := :loginst;
		
--		Fetch Currs Into BaseEnt, objectType, loginst;
--		if not Currs::NOTFOUND then
--			hasNext :=-1;
--		end if;

	End while;

--	Close Currs;

end;


