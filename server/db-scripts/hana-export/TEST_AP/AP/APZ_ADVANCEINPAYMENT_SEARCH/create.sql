

CREATE PROCEDURE APZ_AdvanceInPayment_Search(in DocEntry int default 0, DocDate date default null, EndDate date default null, DocStatus nvarchar(1) default '', DocNum int default 0)
AS 
BEGIN	
	if :DocEntry > 0 then
		select * from APZ_OARC where "DocEntry" = cast(:DocEntry as nvarchar);
	else
		if :DocNum > 0 then
			select * from APZ_OARC where "DocNum" = cast(:DocNum as nvarchar);
		else
			if :DocStatus <> '' then
				if :DocDate is not null and :EndDate is null then
					select * from APZ_OARC where "DocDate" >= to_date(:DocDate, 'yyyy-MM-dd') and "DocStatus" = :DocStatus;
					return;
				end if;
			
				if :DocDate is null and :EndDate is not null then
					select * from APZ_OARC where "DocDate" <= to_date(:EndDate,'yyyy-MM-dd') and "DocStatus" = :DocStatus;
					return;
				end if;
			
				if :DocDate is not null and :EndDate is not null then
				 	select * from APZ_OARC where "DocDate" >= to_date(:DocDate, 'yyyy-MM-dd') and "DocDate" <= to_date(:EndDate, 'yyyy-MM-dd') and "DocStatus" = :DocStatus;
				 	return;
				end if;
			else
				if :DocDate is not null and :EndDate is null then
					select * from APZ_OARC where "DocDate" >= to_date(:DocDate, 'yyyy-MM-dd');
					return;
				end if;
			
				if :DocDate is null and :EndDate is not null then
					select * from APZ_OARC where "DocDate" <= to_date(:DocDate, 'yyyy-MM-dd');
					return;
				end if;
			
				if :DocDate is not null and :EndDate is not null then
					 select * from APZ_OARC where "DocDate" >= to_date(:DocDate, 'yyyy-MM-dd') and "DocDate" <= to_date(:EndDate, 'yyyy-MM-dd');
				 	return;
				end if;
			end if;
		end if;
	end if;
END;




