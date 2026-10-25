Create PROCEDURE TMSP_Inventory_Posting_VirtualOIQR(
in DocKey  int,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '10000071') then
		select * from OIQR T1 left outer join OPLN T2 on T1."PriceSrc" = 1 and T1."PriceList" <>-1 and T1."PriceList" = T2."ListNum" where "DocEntry" = :DocKey;
	Else
		select * from OIPD T1 left outer join OPLN T2 on T1."PriceSrc" = 1 and T1."PriceList" <>-1 and T1."PriceList" = T2."ListNum" where "DocEntry" = :DocKey;
	End if;
End


