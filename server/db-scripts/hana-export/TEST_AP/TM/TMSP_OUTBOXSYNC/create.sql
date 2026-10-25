-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_OutboxSync 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
aCode int;
aUnsentCount int;
aVar int;

cursor dbHdr for 
        select "AlertCode"
		from OAOB
		where "WasSent" = 'N'
		group by "AlertCode";
begin
	
	open dbHdr;
	fetch dbHdr into aCode;

	while NOT dbHdr::NOTFOUND do
		select count(*) into aVar from ALR1 where ALR1."Code" = :aCode;
		if :aVar <> 0 then
			select count("AlertCode") into aUnsentCount
			from AOB1
			where AOB1."AlertCode" = :aCode and (
				"SendEMail" <> "Confirmed2" And NOT "Confirmed2" = 'E' Or
				"SendSMS" <> "Confirmed3" And NOT "Confirmed3" = 'E'  Or
				"SendFax" <> "Confirmed4" And NOT "Confirmed4" = 'E' );
		
			if (:aUnsentCount = 0) then
				update OAOB set "WasSent" = 'Y' where "AlertCode" = :aCode;
			end if;
		end if;
	fetch dbHdr into aCode;
	end while;
	close dbHdr;
end;


