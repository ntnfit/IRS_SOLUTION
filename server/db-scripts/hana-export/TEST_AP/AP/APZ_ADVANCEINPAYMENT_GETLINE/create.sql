

CREATE PROCEDURE APZ_AdvanceInPayment_GetLine(in DocEntry int)
AS
BEGIN
	select * from APZ_ARC1 where "DocEntry" = :DocEntry order by "LineNum";
END;



