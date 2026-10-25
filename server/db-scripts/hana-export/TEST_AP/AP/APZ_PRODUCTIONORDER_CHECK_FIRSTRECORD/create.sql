
create procedure Apz_ProductionOrder_Check_FirstRecord

As
Begin
SELECT "DocEntry" FROM OWOR WHERE "DocEntry" = (SELECT MIN("DocEntry") FROM OWOR);
End;




