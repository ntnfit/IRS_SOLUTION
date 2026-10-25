-- B1 DEPENDS: AFTER:SP:TmSp_IVIInventoryValueDiffsFIFOItems AFTER:SP:TmSp_IVIInventoryValueDiffsMAPnSTDItems AFTER:PT:PROCESS_END

Create procedure TmSp_IVIVerifyInventoryRecalculation(out retCode int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
-- Return Codes
--(-1) = Empty Parameter Was Entered
--(-2)  --Enpty or invalid Line set
--(-3) -- Invalid Currency or Bad Account code
--(-4) -- Invalid Card Currency or Bad Account code
As

Result int;
flag int:=0;
Begin

Result := 0;
SELECT COUNT(*) INTO flag FROM "UITM" T0
	WHERE T0."InvntItem" ='Y' and T0."EvalSystem" in ('S','A');

IF :flag>0 THEN
CALL TmSp_IVIInventoryValueDiffsMAPnSTDItems(:Result);
END IF;

If :Result <> -1
THEN 

SELECT COUNT(*) INTO flag FROM "UITM" T0
	WHERE T0."InvntItem" ='Y' and T0."EvalSystem" = 'F';
IF :flag>0 THEN
CALL TmSp_IVIInventoryValueDiffsFIFOItems(:Result);
END IF;

END IF;

If :Result <> -1
THEN 

SELECT COUNT(*) INTO flag FROM "UITM" T0
	WHERE T0."InvntItem" ='Y' and T0."EvalSystem" = 'B';
IF :flag>0 THEN
CALL TmSp_IVIInventoryValueDiffsSNBItems(:Result);
END IF;

END IF;
If :Result <> -1
THEN
retCode := 0; 
return;



else 

retCode := -1; 
return;

END IF;
end


