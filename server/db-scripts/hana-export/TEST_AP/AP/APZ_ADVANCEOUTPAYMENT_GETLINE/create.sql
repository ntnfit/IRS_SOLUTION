

CREATE PROCEDURE APZ_AdvanceOutPayment_GetLine(in DocEntry int)
AS
BEGIN
	select * from APZ_APM1 where "DocEntry" = :DocEntry order by "VisOrder";
END;



