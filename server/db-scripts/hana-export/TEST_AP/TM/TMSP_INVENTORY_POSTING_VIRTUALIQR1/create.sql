Create PROCEDURE TMSP_Inventory_Posting_VirtualIQR1(
in DocKey  int,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
Begin
	IF (:ObjectId = '10000071') then
		select * from "IQR1" T1 left outer join OBIN T2 on T1."BinEntry" = T2."AbsEntry" where T1."DocEntry" = :DocKey;
	Else
		select * from "IPD1" T1 left outer join OBIN T2 on T1."BinEntry" = T2."AbsEntry" where T1."DocEntry" = :DocKey;
	End if;
End


