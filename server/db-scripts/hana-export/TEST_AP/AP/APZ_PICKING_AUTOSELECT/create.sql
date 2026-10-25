
CREATE PROCEDURE apz_picking_AutoSelect(in ItemCode NVARCHAR(50),
	in WhsCode NVARCHAR(50),
	in Quantity numeric(19,6),
	in LineNum INT,
	in DocEntry INT,
	in OrderNum INT default 0,
	in OrderLine INT default -1)
AS
BEGIN
	
	create local temporary table #tempAuto ("DocEntry" int);
	call apz_fun_picking_AutoSelect(:ItemCode, :whsCode, :quantity, :lineNum, :docEntry, :orderNum, :orderLine, '#tempAuto');
	select * from #tempAuto;
	drop table #tempAuto;
				
END;




