
create procedure apz_ProductionOrder_GetIsIssued(in docEntry int default 0)
As
Begin
SELECT a."BaseLine" AS "LineNum" FROM ign1 a WHERE a."BaseEntry" = :docEntry AND "BaseType" = 202 
UNION ALL 
SELECT a."BaseLine" AS "LineNum" FROM ige1 a WHERE a."BaseEntry" = :docEntry AND "BaseType" = 202;

End;





