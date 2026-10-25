
create procedure Apz_ProductionOrder_GetList_SalesOder()
As
Begin
SELECT "DocEntry", "DocNum", "DocDate", "CardCode", "CardName", "Comments", "DocDueDate" FROM ordr WHERE "DocStatus" = 'O';

End;





