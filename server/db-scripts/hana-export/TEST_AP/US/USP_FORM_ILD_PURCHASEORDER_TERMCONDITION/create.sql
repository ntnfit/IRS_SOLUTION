CREATE PROCEDURE USP_FORM_ILD_PurchaseOrder_TermCondition(
	in docID				int
)
AS
BEGIN

	
	select a."DocEntry",'1.' || cast(cast(row_number() over (order by b."U_TernCode") as int) as nvarchar(50)) ID--,'1.' || cast(cast(row_number() over (order by b."U_TernCode") as int) as nvarchar(50))
			,b."U_Term" "TermName"
	from 
		OPOR a
		inner join "@PO_TERM_COND" b on b."U_Active" = 'Y'
	where
		a."DocEntry" = :docID
	order by b."U_TernCode";

	
END
