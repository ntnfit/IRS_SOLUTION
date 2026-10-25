
create procedure APZ_InventoryCyles_FindNextLastFrist
(
in id bigint,
in type int
)
as
begin
	Declare mina int =0;
	Declare maxa int =0;
	if :type = 0
		then
			select top 1* from OCYC  order by "Code" ASC;
	elseif :type = 1
		then
			select min("Code") into mina from OCYC;
			if :id <= :mina
				then
					select top 1*  from OCYC   order by "Code" DESC;
				else
				
					select top 1 * from  OCYC  where "Code" < :id order by "Code" DESC;
			end if;
	elseif :type = 2
			then
				select max("Code") into maxa from OCYC;
				if :id >= :maxa
					then
						select top 1 * from OCYC  order by "Code" ASC;
					else
					
						select top 1 *from OCYC  where "Code" > :id order by "Code" ASC;
			
					end if;
	else
			select top 1 * from OCYC   order by "Code" DESC;
	
	end if;
end




