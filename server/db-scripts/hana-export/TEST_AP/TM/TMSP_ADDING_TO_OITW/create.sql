-- B1 DEPENDS: AFTER:PT:PROCESS_END

Create procedure TmSp_adding_To_OITW(
in SourceDB nvarchar(128),
in UtilityDB    nvarchar(128)
)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
begin
 exec (' INSERT INTO  '|| :SourceDB  ||'.OITW SELECT T1.* FROM   '|| :UtilityDB ||'.OITW T1 LEFT OUTER JOIN '|| :SourceDB  ||'.OITW T0 ON T0."ItemCode" = T1."ItemCode" and  T0."WhsCode" = T1."WhsCode"  WHERE T0."ItemCode" is NULL and T1."OnHand"!=0');
end;


