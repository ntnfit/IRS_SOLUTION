CREATE procedure ILD_LayCostCenter
as
begin

	select "PrcCode"  ,"PrcName"  
	from OPRC 
	where "Active"  = 'Y' and "PrcName" not like  '%General%'
	Order by "PrcCode";
end;


