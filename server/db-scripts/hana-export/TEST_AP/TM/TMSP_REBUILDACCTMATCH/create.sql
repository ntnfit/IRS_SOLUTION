-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE   procedure TmSp_RebuildAcctMatch(IN actCode nvarchar (15), IN actCurr nvarchar (3),IN useLocal nchar (1), IN isCard nvarchar (1), IN external1 nvarchar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
maxMatch  	int;
count1     	int;
maxMatchBnk    int;
begin



if (:external1 = N'N')
THEN
	if (:useLocal = N'Y')
	THEN
		    insert into OMTH 
			select "ShortName", N'I', "IntrnMatch", sum ("Debit"),
			:isCard, N'0', -1, CURRENT_TIMESTAMP, :actCurr, NULL, N'1' ,CURRENT_TIMESTAMP
			from JDT1 where "ShortName" = :actCode  and "IntrnMatch" > 0
			group by "ShortName", "IntrnMatch" ; 

	else
	
		    insert into OMTH 
			select "ShortName", N'I', "IntrnMatch", sum ("FCDebit"),
			:isCard, N'0', -1, CURRENT_TIMESTAMP, :actCurr, NULL, N'1' ,CURRENT_TIMESTAMP
			from JDT1 where "ShortName" = :actCode  and "IntrnMatch" > 0
			group by "ShortName", "IntrnMatch" ; 
	end IF;
END IF;

if (:external1 = N'Y')
THEN
	if (:useLocal = N'Y')
	THEN
		    insert into OMTH 
			select "ShortName", N'E', "ExtrMatch", (sum("Debit")-sum("Credit")),
			:isCard, N'0', -1, CURRENT_TIMESTAMP, :actCurr, NULL, N'1' ,CURRENT_TIMESTAMP
			from JDT1 where "ShortName" = :actCode  and "ExtrMatch" >0
			group by "ShortName", "ExtrMatch"  ;
	
	else
	
		    insert into OMTH 
			select "ShortName", N'E', "ExtrMatch", (sum("FCDebit")-sum("FCCredit")),
			:isCard, N'0', -1, CURRENT_TIMESTAMP, :actCurr, NULL, N'1' ,CURRENT_TIMESTAMP
			from JDT1 where "ShortName" = :actCode  and "ExtrMatch" >0
			group by "ShortName", "ExtrMatch" ; 		
	end	IF;	
end IF;

end;


