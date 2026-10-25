CREATE PROCEDURE TMSP_Inventory_Counting_VirtualBin (
   in DocKey  nvarchar(12),
   in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '1470000065') then
		select count(*)  as "Count" from "INC1" where "DocEntry"= :DocKey  and "BinEntry" is not null;
	else
		select count(*)  as "Count"  from "ICD1" where "DocEntry"= :DocKey  and "BinEntry" is not null;
	End if;
End


