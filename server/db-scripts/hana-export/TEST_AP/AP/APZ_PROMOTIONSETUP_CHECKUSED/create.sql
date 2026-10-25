

CREATE PROCEDURE APZ_PromotionSetup_CheckUsed (IN type char(1), IN id int)
AS
BEGIN
	declare fieldName nvarchar(15);
	declare query nvarchar(200) = '';

	if :type = 'C' then
		if exists (select top 1 1 from APZ_D_OPMO where "CategoryID" = :id) then
			select 'Used' from dummy;
			return;
		end if;
		
	else if :type = 'T' then
		if exists (select top 1 1 from APZ_D_OPMO where "TypeID" = :id) then
			select 'Used' from dummy;
			return;
		end if;
		
	else 
		if exists (select top 1 1 from APZ_D_OPMO where "GroupID" = :id) then
			select 'Used' from dummy;
			return;
		end if;
	end if;
	end if;
	
	select '' from dummy;
END




