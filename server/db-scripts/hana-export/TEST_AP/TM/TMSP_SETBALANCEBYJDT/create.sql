-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_SetBalanceByJdt (IN jdtAbs INT, OUT retCode INT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
-- Return Codes
--(-1) = Empty Parameter Was Entered
--(-2)  --Enpty or invalid Line set
--(-3) -- Invalid Currency or Bad Account code
--(-4) -- Invalid Card Currency or Bad Account code
--****************
acctCode nvarchar(15);
cardCode nvarchar(15);
Acctcurr nchar(3);
Cardcurr nchar(3);
mainCurr  nchar(3);
sys DECIMAL(21,6);
loc DECIMAL(21,6);
fc  DECIMAL(21,6);
IsCard int;
IsAcctFc int;
IsCardFc int;
DisNegativeSign nchar(1);

CURSOR Currs FOR
select	JDT1."Account", OACT."ActCurr", JDT1."ShortName", OCRD."Currency",
	sum(IFNULL(JDT1."Credit", 0)) - sum(IFNULL(JDT1."Debit", 0)),
	sum(IFNULL(JDT1."SYSCred", 0)) - sum(IFNULL(JDT1."SYSDeb", 0)),
	sum(IFNULL(JDT1."FCCredit", 0)) - sum(IFNULL(JDT1."FCDebit", 0))
From JDT1 
	inner join OACT on OACT."AcctCode" = JDT1."Account"
	Left Outer join OCRD on OCRD."CardCode" = JDT1."ShortName"
Where JDT1."TransId" = :jdtAbs 
group by JDT1."Account", OACT."ActCurr", JDT1."ShortName", OCRD."Currency";

begin
	if :jdtAbs = 0 then
		retCode := -1;
		return;
	end if;
	
	select top 1 "MainCurncy", "DispPosDeb" into mainCurr, DisNegativeSign from OADM;
	Open Currs;
	Fetch Currs into acctCode, Acctcurr, cardCode, Cardcurr, loc, sys, fc;
	
	if (Currs::NOTFOUND) then
		Close Currs;
		retCode := -2;
		return ;
	end if;
	
	While not Currs::NOTFOUND do
		if :DisNegativeSign = 'Y' then
			loc := :loc * -1;
			sys := :sys * -1;
			fc := :fc * -1;
		END if;
		
	--Check to see if this involves a CARD
		IsCard := 0;
		IsCardFc := 0;
		if :acctCode <> :cardCode then
		   	IsCard := 1;
		
			if :Cardcurr <> '##' then
				if :mainCurr <> :Cardcurr then
				      IsCardFc := 1; --FC balance
				end	if;
			end if;
		end if;
	-- Validations **************************
		if :Acctcurr Is null then
			Close Currs;
			retCode := -3;
			return; -- Invalid Account Currency or Bad Account code
		end if;
	
		if :Cardcurr Is null and :IsCard = 1 then
			Close Currs;
			retCode := -4;
			return ; -- Invalid Card Currency or Bad Account code
		end if;
	
		IsAcctFc := 0;
		if :Acctcurr <> '##' then
			if :mainCurr <> :Acctcurr then 
				IsAcctFc := 1; --FC balance
			end if;
		end if;
	-- Do not update FC balance if account is multicurrency or account's currency is as a main currency
		if :IsAcctFc = 0 then
	        UPDATE OACT SET "CurrTotal" = IFNULL("CurrTotal", 0) + :loc, "SysTotal" = IFNULL("SysTotal", 0) + :sys WHERE  OACT."AcctCode"  = :acctCode;
	-- update FC
		Else
	        UPDATE OACT SET "CurrTotal" = IFNULL("CurrTotal",0) + :loc, "SysTotal" = IFNULL("SysTotal", 0) + :sys, "FcTotal" = IFNULL("FcTotal", 0) + :fc WHERE OACT."AcctCode"  = :acctCode;
		End if;
	--Update A card  ***********************************************
		if :IsCard = 1 then
			if :IsCardFc = 0 then
				UPDATE OCRD SET "Balance" = IFNULL("Balance", 0) + :loc, "BalanceSys" = IFNULL("BalanceSys", 0) + :sys WHERE  OCRD."CardCode"  = :cardCode;
			Else
				UPDATE OCRD SET "Balance" = IFNULL("Balance", 0) + :loc, "BalanceSys" = IFNULL("BalanceSys", 0) + :sys, "BalanceFC" = IFNULL("BalanceFC", 0) + :fc WHERE  OCRD."CardCode"  = :cardCode;
			end if;
		end if;
	
		Fetch Currs Into acctCode, Acctcurr, cardCode, Cardcurr, loc, sys, fc;
	End while;
	
	Close Currs;
	
	retCode := 0;
end;


