Create PROCEDURE TMSP_Inventory_OpenBalance_VirtualBin(
in DocKey  int,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '310000001') then
		select count(*) as "Count" from IQI1 where "DocEntry" = :DocKey  and "BinEntry" is not null;
	else
		select count(*) as "Count" from IOD1 where "DocEntry" = :DocKey  and "BinEntry" is not null;
	ENd if;
End


