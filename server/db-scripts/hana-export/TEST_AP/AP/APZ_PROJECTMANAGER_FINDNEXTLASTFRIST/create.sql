
create procedure APZ_ProjectManager_FindNextLastFrist
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
			select top 1* from APZ_OPRG  order by "AbsEntry" ASC;
	elseif :type = 1
		then
			select min("AbsEntry") into mina from APZ_OPRG;
			if :id <= :mina
				then
					select top 1*  from APZ_OPRG   order by "AbsEntry" DESC;
				else
				
					select top 1 * from  APZ_OPRG  where "AbsEntry" < :id order by "AbsEntry" DESC;
			end if;
	elseif :type = 2
			then
				select max("AbsEntry") into maxa from APZ_OPRG;
				if :id >= :maxa
					then
						select top 1 * from APZ_OPRG  order by "AbsEntry" ASC;
					else
					
						select top 1 *from APZ_OPRG  where "AbsEntry" > :id order by "AbsEntry" ASC;
			
					end if;
	else
			select top 1 * from APZ_OPRG   order by "AbsEntry" DESC;
	
	end if;
end




