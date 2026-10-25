Create procedure ILD_Lay_VendorQA
as
begin

		select "CardCode","CardName"   from OCRD 
		where "CardType"  = 'S'
		order by  "CardCode";
end;

