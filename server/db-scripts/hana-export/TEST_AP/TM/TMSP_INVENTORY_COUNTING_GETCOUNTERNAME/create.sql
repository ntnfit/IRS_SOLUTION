CREATE PROCEDURE TMSP_Inventory_Counting_GetCounterName(
in DocKey  nvarchar(12),
in ObjectId nvarchar(12),
in MultipleCounterType nvarchar(1),
in MultipleCounterOrder nvarchar(12)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
selectStr nvarchar(256);
fromStr nvarchar(256);
whereStr nvarchar(64);

CountingType nvarchar(1);

taker1Type int;

typeUSR int := 12;
typeHEM int := 171;
CountTypeSingle nvarchar(1) := '1';
CountTypeMultiple nvarchar(1) := '2';

CounterTypeTeam nvarchar(1) := 'T';
CounterTypeIndividual nvarchar(1) := 'I';

MainTable nvarchar(16);
Counter nvarchar(64);
joinTbl nvarchar(16);
joinCol nvarchar(16);

ResultCounterName nvarchar(16) := 'CounterName';

DocEntry nvarchar(16) := :DocKey;
CounterOrder nvarchar(16) := :MultipleCounterOrder;
BEGIN
if ObjectId = '1470000065' then --added doc

	whereStr := ' where "T0"."DocEntry"=' || :DocEntry;

	select "T0"."CountType" INTO CountingType from "OINC" "T0" where "T0"."DocEntry"= :DocKey;
	select "Taker1Type" INTO Taker1Type from "OINC" where "DocEntry"= :DocKey;

	IF (:CountingType = :CountTypeSingle )
	THEN
			SELECT "Taker1Type" INTO Taker1Type from "OINC" where "DocEntry"= :DocKey;
			
			IF(:taker1Type = :typeUSR)
			THEN
				 Counter := ' "T1"."U_NAME" ';
				 joinTbl := ' "OUSR" ';
				 joinCol := ' "USERID" ';
			ELSE
				 Counter := ' "T1"."lastName"||'' ''||"T1"."firstName" ';
				 joinTbl := ' "OHEM" ';
				 joinCol := ' "empID" ';
			End IF;

			selectStr := 'select ' || :Counter || ' as ' || :ResultCounterName || ' from "OINC"  "T0" ' ||  ' join ' || :joinTbl || ' "T1" on "T0"."Taker1Id" = "T1".' || :joinCol || :whereStr;
	ELSE
			IF(:MultipleCounterType = :CounterTypeTeam)
			THEN
				  MainTable :=' "INC4" "T0" ';
			ELSE
				  MainTable :=' "INC8" "T0" ';
			End IF;
		   
			selectStr := 'select ' ||  ' "T0"."CounteName" '|| ' as ' || :ResultCounterName || ' from '|| :MainTable  ||  :whereStr || ' and "T0"."VisOrder" = ' || :CounterOrder;
	END IF;
else
	whereStr := ' where "T0"."DocEntry"=' || :DocEntry;

	select "T0"."CountType" INTO CountingType from "OICD" "T0" where "T0"."DocEntry"= :DocKey;
	select "Taker1Type" INTO Taker1Type from "OICD" where "DocEntry"= :DocKey;

	IF (:CountingType = :CountTypeSingle )
	THEN
			SELECT "Taker1Type" INTO Taker1Type from "OICD" where "DocEntry"= :DocKey;
			
			IF(:taker1Type = :typeUSR)
			THEN
				 Counter := ' "T1"."U_NAME" ';
				 joinTbl := ' "OUSR" ';
				 joinCol := ' "USERID" ';
			ELSE
				 Counter := ' "T1"."lastName"||'' ''||"T1"."firstName" ';
				 joinTbl := ' "OHEM" ';
				 joinCol := ' "empID" ';
			End IF;

			selectStr := 'select ' || :Counter || ' as ' || :ResultCounterName || ' from "OICD"  "T0" ' ||  ' join ' || :joinTbl || ' "T1" on "T0"."Taker1Id" = "T1".' || :joinCol || :whereStr;
	ELSE
			IF(:MultipleCounterType = :CounterTypeTeam)
			THEN
				  MainTable :=' "ICD4" "T0" ';
			ELSE
				  MainTable :=' "ICD8" "T0" ';
			End IF;
		   
			selectStr := 'select ' ||  ' "T0"."CounteName" '|| ' as ' || :ResultCounterName || ' from '|| :MainTable  ||  :whereStr || ' and "T0"."VisOrder" = ' || :CounterOrder;
	END IF;
End if;
EXECUTE IMMEDIATE (:selectStr);

END;


