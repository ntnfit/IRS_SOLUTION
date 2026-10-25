
Create procedure APZ_DiscountGroup_LoadTabPagesData
As
Begin
select "ItmsGrpCod","ItmsGrpNam",0 as "Discount" from OITB ;

select "ItmsTypCod","ItmsGrpNam",0 as "Discount" from OITG;

select "FirmCode","FirmName",0 as "Discount" from OMRC;
End;




