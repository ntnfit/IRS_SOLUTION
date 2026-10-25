
create procedure APZ_ServiceLevelAgreement_FindNextLastFrist
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
			select top 1* from APZ_OSLA  order by "SLAID" ASC;
	elseif :type = 1
		then
			select min("SLAID") into mina from APZ_OSLA;
			if :id <= :mina
				then
					select top 1*  from APZ_OSLA   order by "SLAID" DESC;
				else
				
					select top 1 * from  APZ_OSLA  where "SLAID" < :id order by "SLAID" DESC;
			end if;
	elseif :type = 2
			then
				select max("SLAID") into maxa from APZ_OSLA;
				if :id >= :maxa
					then
						select top 1 * from APZ_OSLA  order by "SLAID" ASC;
					else
					
						select top 1 *from APZ_OSLA  where "SLAID" > :id order by "SLAID" ASC;
			
					end if;
	else
			select top 1 * from APZ_OSLA   order by "SLAID" DESC;
	
	end if;
end




