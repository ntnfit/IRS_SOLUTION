Create PROCEDURE TMSP_Inventory_OpenBalance_VirtualIQI1(
in DocKey  int ,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '310000001') then
		select * from IQI1 T1 left outer join OBIN T2 on T1."BinEntry" = T2."AbsEntry" where T1."DocEntry" = :DocKey;
	Else
		select * from IOD1 T1 left outer join OBIN T2 on T1."BinEntry" = T2."AbsEntry" where T1."DocEntry" = :DocKey;
	End if;
End


