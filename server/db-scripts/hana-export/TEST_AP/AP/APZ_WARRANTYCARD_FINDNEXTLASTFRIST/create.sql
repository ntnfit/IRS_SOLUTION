
create procedure APZ_WarrantyCard_FindNextLastFrist
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
			select top 1* from APZ_OINS  order by "WrntyID" ASC;
	elseif :type = 1
		then
			select min("WrntyID") into mina from APZ_OINS;
			if :id <= :mina
				then
					select top 1*  from APZ_OINS   order by "WrntyID" DESC;
				else
				
					select top 1 * from  APZ_OINS  where "WrntyID" < :id order by "WrntyID" DESC;
			end if;
	elseif :type = 2
			then
				select max("WrntyID") into maxa from APZ_OINS;
				if :id >= :maxa
					then
						select top 1 * from APZ_OINS  order by "WrntyID" ASC;
					else
					
						select top 1 *from APZ_OINS  where "WrntyID" > :id order by "WrntyID" ASC;
			
					end if;
	else
			select top 1 * from APZ_OINS   order by "WrntyID" DESC;
	
	end if;
end;




