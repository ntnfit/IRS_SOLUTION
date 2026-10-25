

CREATE PROCEDURE APZ_PackingManagement_GetPO
AS
BEGIN
	select top 1
	"POCode" + 1 as "POCode",
	"TypePO",
	"Unit",
	"TypeCode",
	"ChildType",
	"CreateDate",
	"USERID",
	"PostingDate",
	"Status",
	"Remark",
	"AmountItem",
	"WhsCode",
	"Amountbundle",
	"BaseEntry",
	"LineNum"
	 from APZ_DMPO order by "POCode" desc;
END;




