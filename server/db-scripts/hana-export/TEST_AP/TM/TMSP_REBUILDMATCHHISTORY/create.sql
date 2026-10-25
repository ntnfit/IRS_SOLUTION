-- B1 DEPENDS: AFTER:SP:TmSp_RebuildAcctMatch AFTER:PT:PROCESS_END

CREATE  procedure TmSp_RebuildMatchHistory  
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
As
errCode	int;
maxMatchNum	int;
mainCurr	nvarchar (3);
actCode	nvarchar (15);
actCurr	nvarchar (3);
useLocal	nchar (1);
external1	nchar (1);
isCard		nchar (1);
    cursor dbAccounts  for 
	SELECT JDT1."Account", max(OACT."ActCurr"), N'A'
	FROM OACT INNER JOIN
        	JDT1 ON OACT."AcctCode" = JDT1."Account"
	WHERE (JDT1."IntrnMatch" > 0 AND JDT1."ShortName" = JDT1."Account")
	GROUP by JDT1."Account"
	union
	SELECT JDT1."ShortName", max(OCRD."Currency"), N'C'
	FROM OCRD INNER JOIN
        	JDT1 ON OCRD."CardCode" = JDT1."ShortName"
	WHERE (JDT1."IntrnMatch" > 0 AND JDT1."ShortName" <> JDT1."Account")
	GROUP by JDT1."ShortName";
	cursor dbAccounts1  for 
	SELECT JDT1."Account", max(OACT."ActCurr"), N'A'
	FROM OACT INNER JOIN
        	JDT1 ON OACT."AcctCode" = JDT1."Account"
	WHERE (JDT1."ExtrMatch" > 0 AND JDT1."ShortName" = JDT1."Account")
	GROUP by JDT1."Account"
	union
	SELECT JDT1."ShortName", max(OCRD."Currency"), N'C'
	FROM OCRD INNER JOIN
        	JDT1 ON OCRD."CardCode" = JDT1."ShortName"
	WHERE (JDT1."ExtrMatch" > 0 AND JDT1."ShortName" <> JDT1."Account")
	GROUP by JDT1."ShortName";
begin



select "MainCurncy" into mainCurr from OADM;



delete from OMTH;

open dbAccounts;
Fetch dbAccounts into actCode, actCurr, isCard;
while NOT dbAccounts::NOTFOUND do
    if (:actCurr = N'' or :actCurr = N'##')
    THEN
	actCurr := :mainCurr;
    END IF;
    if (:actCurr = :mainCurr)
    THEN
	useLocal := N'Y';
    else
	useLocal := N'N';
	END IF;
   
    update OACT set "IntrMatch" = 0
	where "AcctCode" = :actCode;

    update OCRD set "InMatchNum" = 0
	where "CardCode" = :actCode;

    external1 := N'N';
	
	IF (:actCode <> NULL) Then
		CALL TmSp_RebuildAcctMatch(:actCode, :actCurr, :useLocal, :isCard, :external1);
	End IF;
	
    select max("MatchNum") INTO maxMatchNum from OMTH 
    where "MthAcctCod" = :actCode AND "IsInternal" = N'I';     

    update OACT set "IntrMatch" = :maxMatchNum 
    where "AcctCode" = :actCode;

    update OCRD set "InMatchNum" = :maxMatchNum
    where "CardCode" = :actCode;

    Fetch dbAccounts into actCode, actCurr, isCard;
end WHILE;

close dbAccounts;


--external


	
open dbAccounts1;
Fetch dbAccounts1 into actCode, actCurr,isCard;
while NOT dbAccounts1::NOTFOUND do


    if (:actCurr = N'' or :actCurr = N'##')
    THEN
	actCurr := :mainCurr;
     END IF;
    if (:actCurr = :mainCurr)
    THEN
	useLocal := N'Y';
    else
	useLocal := N'N';
	END IF;
   
    update OACT set "ExtrMatch" = 0
	where "AcctCode" = :actCode;

    update OCRD set "ExMatchNum" = 0
	where "CardCode" = :actCode;

    external1 := N'Y';

    CALL TmSp_RebuildAcctMatch(:actCode, :actCurr, :useLocal, :isCard, :external1);

    select max("MatchNum") INTO maxMatchNum from OMTH 
    where "MthAcctCod" = :actCode AND "IsInternal" = N'E' ;    

    update OACT set "ExtrMatch" = :maxMatchNum
    where "AcctCode" = :actCode;

    update OCRD set "ExMatchNum" = :maxMatchNum
    where "CardCode" = :actCode;

    Fetch dbAccounts1 into actCode, actCurr, isCard;
end WHILE;

close dbAccounts1;


end;


