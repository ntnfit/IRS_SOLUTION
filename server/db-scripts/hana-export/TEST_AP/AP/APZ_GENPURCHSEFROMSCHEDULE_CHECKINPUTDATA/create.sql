

CREATE PROCEDURE APZ_GenPurchseFromSchedule_CheckInputData (IN code nvarchar(20), type char(20), genType char(1))
AS
BEGIN
	DECLARE status char(1);
	
	if :genType = 'O' then

		if :type = 'S' then
			
			if exists(select 1 from APZ_CGRC where "ScheCode" = :code) then

				select 'Schedule had been gen Capacity. Can not gen Production order from this schedule' from dummy;
				return;

			END if;

			status = ifnull((select "Status" from APZ_OPRS where "PSCode" = :code and "Type" = 'O'),'');
			if :status = '' then
				
					select 'You must be choose production schedule' from dummy;
					return;

			else if :status <> 'A' then
					select 'You must be choose approval production schedule' from dummy;
					return;
				 END if;
				
			end if;
 
			select '' from dummy;
			return;
		
		else IF :type = 'C' then

			if not exists(select 1 from APZ_CGRC where "ScheCode" = :code) then
				select 'You can not choose this schedule. You need create capacity for this schedule' from dummy;
				return;
			END if;
			
		end if;
		
			select '' from dummy;
			return;

		END if;

	END IF;

	if :type = 'M' then
			status = ifnull((select "Status" from APZ_OMRP where "MrpCode" = :code and ("Type" = 3 or "Type" = 4)), '');
			if :status = '' then

				select 'You must be choose mrp 3 or 4' from dummy;
				return;

			else if :status <> 'A' then

				select 'You must be choose approval mrp' from dummy;
				return;

				END if;
			end if;
			
			select '' from dummy;
			return;

	ELSE 

		 IF :genType = 'T' then

				status = ifnull((select "Status" from APZ_OPRS where "PSCode" = :code and ("Type" = 'P' or "Type" = 'T')), '');
				if :status = '' then
						select 'You must be choose purchase schedule or transfer schedule' from dummy;
						return;

					else if :status <> 'A' then

						select 'You must be choose approval schedule' from dummy;
						return;

						END if;
					end if;

					select '' from dummy;
					return;

			END if;

			status = ifnull((select "Status" from APZ_OPRS where "PSCode" = :code and "Type" = 'P'), '');

			if :status = '' then
				
				select 'You must be choose purchase schedule' from dummy;
				return;

			else if :status <> 'A' then

				select 'You must be choose approval schedule' from dummy;
				return;

				END if;
			end if;

			select '' from dummy;
			return;

	END IF;
	
END



