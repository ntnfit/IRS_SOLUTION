
create procedure APZ_JobVacancy_FindNextLastFrist
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
			select top 1* from APZ_OHJV  order by "DocEntry" ASC;
	elseif :type = 1
		then
			select min("DocEntry") into mina from APZ_OHJV;
			if :id <= :mina
				then
					select top 1*  from APZ_OHJV   order by "DocEntry" DESC;
				else
				
					select top 1 * from  APZ_OHJV  where "DocEntry" < :id order by "DocEntry" DESC;
			end if;
	elseif :type = 2
			then
				select max("DocEntry") into maxa from APZ_OHJV;
				if :id >= :maxa
					then
						select top 1 * from APZ_OHJV  order by "DocEntry" ASC;
					else
					
						select top 1 *from APZ_OHJV  where "DocEntry" > :id order by "DocEntry" ASC;
			
					end if;
	else
			select top 1 * from APZ_OHJV   order by "DocEntry" DESC;
	
	end if;
end




