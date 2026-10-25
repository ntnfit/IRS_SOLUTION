
create PROCEDURE APZ_CycleCountDeterminationSubtable_GetByCycles(in cyclesBy int)
AS 
BEGIN
if :cyclesBy=2
		then
		select "AbsEntry","SLCode" from OBSL where "FldAbs"=1;
		
	elseif :cyclesBy=3
		then
		select "AbsEntry","SLCode" from OBSL where "FldAbs"=2;
		
	elseif :cyclesBy=4
		then
		select "AbsEntry","SLCode" from OBSL where "FldAbs"=3;
		
	elseif :cyclesBy=5
		then
		select "AbsEntry","SLCode" from OBSL where "FldAbs"=4;
		
	else 
		select "ItmsGrpCod","ItmsGrpNam" from OITB;
	end if;
END;




