-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2

CREATE PROCEDURE TmSp_SetBgtAccumulators_ByJdt (in jdtAbs int, in Alert nchar(1), in CurPeriod nvarchar(11)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
month int;
tmpStr nvarchar(20);
AcctCode nvarchar(15);
RefDate datetime;
RefYear nchar(4);
sysCredit dec(19,6);
sysDebit dec(19,6);
locCredit dec(19,6);
locDebit dec(19,6);
BgtNum int;
YearStart datetime;
AlrType nchar(1);
YFmonth int;
TotsysDebit dec(19,6);
TotlocDebit dec(19,6);
YearRsum dec(19,6);
monthRsum dec(19,6);
YearDebTotal dec(19,6);
YearCredTotal dec(19,6);
monthDebTotal dec(19,6);
YOverSum dec(19,6);
MOverSum dec(19,6);
DoAlert nchar(1);
DisNegativeSign nchar(1);
IsBgtCredit nchar(1);
cnt int;
bgtCount int;

CURSOR Currs for
	select   JDT1."Account", JDT1."RefDate", sum(ifnull(JDT1."Credit", 0)), sum(ifnull(JDT1."Debit", 0)),	sum(ifnull(JDT1."SYSCred", 0)), sum(ifnull(JDT1."SYSDeb", 0))
	From JDT1 inner join OACT on OACT."AcctCode" = JDT1."Account"
	Where JDT1."TransId" = :jdtAbs  AND OACT."Budget" = N'Y' And JDT1."ShortName" = JDT1."Account"
	group by JDT1."Account", JDT1."RefDate";
	
begin
	delete from "TMP_SetBgtAcc_BgAlrChk";

	select "BgtWarning", "DispPosDeb" into AlrType, DisNegativeSign from OADM;
	select "FinancYear" into YearStart from OACP  Where "PeriodCat" = :CurPeriod;	

	Open Currs;
	Fetch Currs Into AcctCode, RefDate, locCredit, locDebit, sysCredit, sysDebit;
    
	While not Currs::NOTFOUND do
		RefYear := TO_NCHAR(year(:RefDate));
		BgtNum := 0;

		select count(*) into bgtCount
		From OBGT Inner Join OBGS on OBGS."AbsId" = OBGT."Instance"
		Where
			:RefDate BETWEEN OBGT."FinancYear" AND ADD_DAYS(ADD_YEARS(OBGT."FinancYear", 1), -1)
			And OBGT."AcctCode" = :AcctCode
			And OBGS."IsMain" = N'Y';
		if(:bgtCount = 0) then
			Fetch Currs Into AcctCode, RefDate, locCredit, locDebit, sysCredit, sysDebit;
			continue;
		end if;
			
		select OBGT."AbsId", ifnull(OBGT."DebLTotal", 0), ifnull(OBGT."CredLTotal", 0), ifnull(OBGT."DebRLTotal", 0) - ifnull(OBGT."CrdRLTotal", 0)
		 into BgtNum, YearDebTotal, YearCredTotal, YearRsum
		From OBGT Inner Join OBGS on OBGS."AbsId" = OBGT."Instance"
		Where
			:RefDate BETWEEN OBGT."FinancYear" AND ADD_DAYS(ADD_YEARS(OBGT."FinancYear", 1), -1)
			And OBGT."AcctCode" = :AcctCode
			And OBGS."IsMain" = N'Y';
       
		If :YearCredTotal > 0 then
			IsBgtCredit := N'Y';
		ELSE
			IsBgtCredit := N'N';
		end if;

		month := month(:RefDate);		
		
		YFmonth := month(:YearStart);
		month := :month - :YFmonth;

		if (:month < 0) then
			month := :month + 12;
		end if;
		
		select ifnull(BGT1."DebLTotal", 0), ifnull(BGT1."DebRLTotal", 0) - ifnull(BGT1."CrdRLTotal", 0)
		into monthDebTotal, monthRsum
		From BGT1
		Where "BudgId" = :BgtNum And "Line_ID" = :month;

	--Alert Validation
		if not :Alert = N'N' then
			if :IsBgtCredit = N'N' then
				if not (:locDebit = 0) then
					 YOverSum := :YearRsum - :YearDebTotal + :locDebit - :locCredit;
				Else
					YOverSum := :YearRsum - :YearDebTotal - :locCredit + :locCredit;
				end if;
				
				--MONTH accumulators		
				if not (:locDebit = 0) then
					MOverSum := :monthRsum - :monthDebTotal + :locDebit - :locCredit;
				Else
					MOverSum := :monthRsum - :monthDebTotal - :locCredit + :locCredit;
				end if;
				--*****************************		
				if (:AlrType = N'A') then
					if (:YOverSum <= 0) then
						DoAlert := N'N';
					Else
						DoAlert := N'Y';
					end if;
				Else
					if (:MOverSum <= 0 ) then
						DoAlert := N'N';
					Else
						DoAlert := N'Y';
					end if;
				End if;
				
		--if (:DoAlert = 'Y')
				if not (:BgtNum <= 0) then
					insert into "TMP_SetBgtAcc_BgAlrChk" (AcctCode , DoAlert , AlrType, MOverSum, YOverSum)
						values ( :AcctCode, :DoAlert, :AlrType, :MOverSum, :YOverSum);
				end if;
			end if;
		end if;

		if not :Alert = N'W' then
			select count(*) into cnt
			From OBGT Inner Join OBGS on OBGS."AbsId" = OBGT."Instance"
			Where
				:RefDate BETWEEN OBGT."FinancYear" AND
				ADD_DAYS(ADD_YEARS(OBGT."FinancYear", 1), -1)
				And OBGT."AcctCode" = :AcctCode
				And OBGS."IsMain" = N'Y';
				
			Update OBGT  Set
				"DebRLTotal" = ifnull("DebRLTotal", 0) + :locDebit,
				"CrdRLTotal" = ifnull("CrdRLTotal", 0) + :locCredit,
				"DebRSTotal" = ifnull("DebRSTotal", 0) + :sysDebit,
				"CrdRSTotal" = ifnull("CrdRSTotal", 0) + :sysCredit			
			Where
				:RefDate BETWEEN OBGT."FinancYear" AND
				ADD_DAYS(ADD_YEARS(OBGT."FinancYear", 1), -1)
				And OBGT."AcctCode" = :AcctCode
				AND "Instance" IN (select  "AbsId" from OBGS where "IsMain" = N'Y') ;

			If not :cnt = 0 then
				Update BGT1 Set
					"DebRLTotal" = ifnull("DebRLTotal", 0) + :locDebit  ,
					"CrdRLTotal" = ifnull("CrdRLTotal", 0) + :locCredit  ,
					"DebRSTotal" = ifnull("DebRSTotal", 0) + :sysDebit ,
					"CrdRSTotal" = ifnull("CrdRSTotal", 0) + :sysCredit
				Where "BudgId" = :BgtNum And "Line_ID" = :month;
			End if;
		End if;

		Fetch Currs Into AcctCode, RefDate, locCredit, locDebit, sysCredit, sysDebit;
	
	End while;

	select * from "TMP_SetBgtAcc_BgAlrChk";
	delete from "TMP_SetBgtAcc_BgAlrChk";
	CLOSE Currs;
end;


