-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE   procedure TmSp_SetGlStamp(IN FldType int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
begin
	if (:FldType = 1) then 
		UPDATE CINF SET "ADMStamp" = "ADMStamp" + 1; 
	end if;
	if (:FldType = 2) then 
		UPDATE CINF SET "ACTStamp" = "ACTStamp" + 1;
	end if;
	if (:FldType = 3) then 
		UPDATE CINF SET "RTTStamp" = "RTTStamp" + 1;
	end if;
end;


