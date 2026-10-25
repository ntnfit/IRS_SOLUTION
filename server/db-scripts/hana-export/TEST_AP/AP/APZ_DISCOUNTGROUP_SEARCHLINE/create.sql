
Create procedure APZ_DiscountGroup_SearchLine(in Entry int default 0)
As
Begin
select * from edg1 where "AbsEntry"=:Entry;
End;




