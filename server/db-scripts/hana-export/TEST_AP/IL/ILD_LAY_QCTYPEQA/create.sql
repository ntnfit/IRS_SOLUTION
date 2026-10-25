CREATE procedure ILD_Lay_QCTypeQA
as
begin

		select 'I' as "Code",'In QC' as "Name"   from dummy
		union all
		
		select 'O' as "Code",'Out QC' as "Name"   from dummy
		union all
		
		select 'P' as "Code",'Production' as "Name"   from dummy
		union all
		
		select 'OTH' as "Code",'Other' as "Name"   from dummy;
	 
		 
		 
end;

