
Create procedure APZ_IVCounting_CheckBatchSerial(in itemCode nvarchar(50) default '')
as
Begin
declare isbatch char(1) ;
declare isbatchSerNum char(1)  ;
declare isSerNum char(1);
select(select top 1 "ManBtchNum" from OITM where  "ItemCode"=:itemCode) into isbatch from dummy;
  
select(select top 1 "ManSerNum"  from OITM where  "ItemCode"=:itemCode) into isSerNum from dummy;

if(:isbatch='Y' or :isSerNum='Y')
then
select 'Y' into isbatchSerNum from dummy;
else
select 'N' into isbatchSerNum from dummy;
end if;
select :isbatchSerNum from dummy;
End;



