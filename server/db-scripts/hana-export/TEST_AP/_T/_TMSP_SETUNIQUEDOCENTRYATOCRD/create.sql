-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_SetUniqueDocEntryAtOCRD
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
-------------------------------------------------------------
Oline     int;
cardCode  nvarchar (15);
hasNext   int := 1;
 
--CURSOR Currs for 
--	select  "CardCode"
--	From  TEMP_OCRD;
--  Order By "CardCode"

begin
--	Open Currs;
	
	Oline := 1;

--	Fetch Currs Into cardCode;
--	if not Currs%found then
--		hasNext := -1;
--	end if;
	
--	While ( :hasNext > 0) do

--		Update TEMP_OCRD
--		Set "DocEntry" = :Oline;
--
--		Oline := :Oline + 1;
--		Fetch Currs Into cardCode;
--		if not Currs%found then
--			hasNext := -1;
--		end if;
		
--	End while;

--	Close Currs;
end;


