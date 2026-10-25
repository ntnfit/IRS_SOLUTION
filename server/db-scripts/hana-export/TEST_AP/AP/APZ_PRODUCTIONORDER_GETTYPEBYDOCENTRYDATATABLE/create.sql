
create procedure Apz_ProductionOrder_GetTypeByDocEntryDataTable(in DocEntry int)
As
Begin
select "Type"  from owor where "DocEntry"=:DocEntry;
End;




