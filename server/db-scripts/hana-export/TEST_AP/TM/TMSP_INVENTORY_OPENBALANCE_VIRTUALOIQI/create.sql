Create PROCEDURE TMSP_Inventory_OpenBalance_VirtualOIQI(
in DocKey  INT ,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '310000001') then
		select * from OIQI T1 left outer join OPLN T2 on T1."PriceSrc" = 1 and T1."PriceList" <>-1 and T1."PriceList" = T2."ListNum" where "DocEntry" = :DocKey;
	Else
		select * from OIOD T1 left outer join OPLN T2 on T1."PriceSrc" = 1 and T1."PriceList" <>-1 and T1."PriceList" = T2."ListNum" where "DocEntry" = :DocKey;
	End if;
End


