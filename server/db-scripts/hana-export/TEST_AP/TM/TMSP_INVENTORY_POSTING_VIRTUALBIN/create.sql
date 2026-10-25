Create PROCEDURE TMSP_Inventory_Posting_VirtualBin(
in DocKey  int,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '10000071') then
		select count(*) as "Count" from IQR1 where "DocEntry" = :DocKey  and "BinEntry" is not null;
	else
		select count(*) as "Count" from IPD1 where "DocEntry" = :DocKey  and "BinEntry" is not null;
	ENd if;
End


