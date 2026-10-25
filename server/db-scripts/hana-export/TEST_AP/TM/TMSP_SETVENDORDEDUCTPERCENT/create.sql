-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_SetVendorDeductPercent(
	in jdtAbs   int, 
	in PeriodCat nvarchar(11),
	out result int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	 cardCode		nvarchar(15);
	 mainVendLoc		nvarchar(15);
	 mainVendFrgn		nvarchar(15);
	 locSum		float;
	 currentPayments	float;
	 ddctPrcnt		float;
	 ddgPrcnt		float;
	 DisNegativeSign	nchar(1);
	 currentDDT		int;
	 today        TIMESTAMP;
	 dateFrom		TIMESTAMP;
	 dateTo		TIMESTAMP;
	 c_next int := 1;
	 cursor Currs (jdtAbs   int) for 
				select JDT1."ShortName", ODDG."DdctPrcnt", sum(IfNull(JDT1."Credit",0)) - sum(IfNull(JDT1."Debit",0))
				From JDT1 inner join OCRD on OCRD."CardCode" = JDT1."ShortName" 
					inner join ODDG on OCRD."DdgKey" = ODDG."Numerator"
				Where JDT1."TransId" = :jdtAbs AND JDT1."ShortName" <> JDT1."Account" AND OCRD."CardType" = N'S'
					group by JDT1."ShortName", ODDG."DdctPrcnt";

begin

	if (:jdtAbs = null) then
		result := -1; 
		return;
	end if;


--	today := CONVERT(TIMESTAMP, cast(DATEPART(yyyy, getdate()) as nchar(4)) || REPLACE( STR(MONTH(getdate()), 2), ' ' , '0')|| REPLACE( STR(DAY(getdate()), 2), ' ' , '0'), 112);

    today := CURRENT_TIMESTAMP;

	select "DispPosDeb" into DisNegativeSign from OADM; 
	select "LinkAct_10" into mainVendLoc from OACP where "PeriodCat" = :PeriodCat;
	select "LinkAct_11" into mainVendFrgn from OACP where "PeriodCat" = :PeriodCat;
	
	--/***********************	Main Query	*************************/

	Open Currs (:jdtAbs);
	Fetch Currs Into  cardCode, ddgPrcnt,  locSum;

	--/***********************	Validations	*************************/
	if  Currs::NOTFOUND  then
			Close Currs;
			result := -2;
			return;  --Enpty or invalid Line set
	end if;
	--/*************************************************************************/
 

	While ( :c_next > 0) do
		Fetch Currs Into  cardCode, ddgPrcnt, locSum;  
		
		if (:DisNegativeSign = N'Y') then
			locSum := :locSum * -1;
		END if;

		--  Check if local sum > 0
		if (:locSum = 0) then
			Fetch Currs 
			Into  cardCode, ddgPrcnt, locSum;
			continue;
		end if;

		-- Get current DDT
		currentDDT := -1;

		select "Numerator" into currentDDT
		FROM ODDT  
		WHERE "CardCode" = :cardCode  AND  
			"DateFrom" <= :today  AND  
			"DateTo" >= :today;

		select "DateFrom" into dateFrom
		FROM ODDT 
		WHERE "CardCode" = :cardCode  AND  
			"DateFrom" <= :today  AND  
			"DateTo" >= :today;
			
		select "DateTo" into dateTo
		FROM ODDT 
		WHERE "CardCode" = :cardCode  AND  
			"DateFrom" <= :today  AND  
			"DateTo" >= :today;
			
		if (:currentDDT = -1)
		then
			dateFrom := '';

			select TOP 1  "Numerator" into currentDDT
	    	FROM ODDT 
			WHERE "CardCode" = :cardCode  AND  
				"DateFrom" <= :today  AND  
				"DateTo" >= :today
			ORDER BY "DateTo" DESC;
			
--			select TOP 1 DATEADD(dd, 1,"DateTo") into dateFrom
			select TOP 1 ADD_DAYS(To_Date("DateTo"),1) into dateFrom
			FROM ODDT 
			WHERE "CardCode" = :cardCode  AND  
				"DateFrom" <= :today  AND  
				"DateTo" >= :today
			ORDER BY "DateTo" DESC;
			
		--	dateFrom := ADD_DAYS()
		end if; 

		-- Get current payments to card
		SELECT sum(IfNull(T0."Debit", 0)) - sum(IfNull(T0."Credit", 0)) into currentPayments
		FROM JDT1 T0 
		WHERE ((T0."TransType" IN (30, 57, 25, 76, 46 ) AND  
				T0."RefDate" >= :dateFrom  AND  T0."RefDate" <= :dateTo)
				OR  T0."RelType" = N'D' ) AND  
			(T0."Account" = :mainVendLoc  OR  T0."Account" = :mainVendFrgn ) AND  
			T0."ShortName" <> T0."Account"  AND  T0."Debit" <> 0  AND  
			T0."ShortName" = :cardCode AND T0."RelType" <> N'D';  

		-- Get appropriate DDT1
		ddctPrcnt := -1;

		if (:currentDDT <> -1)
		then
			select IfNULL (MIN (T0."DdctPrcnt"), -1) into ddctPrcnt
			FROM DDT1 T0
			WHERE T0."DdtKey" = :currentDDT AND T0."MaxSum" >= :currentPayments;

			if (:ddctPrcnt = -1) then		
					select IFNULL (T0."DdctPrcnt", -1) into ddctPrcnt
					FROM DDT1 T0
					WHERE T0."DdtKey" = :currentDDT AND (T0."MaxSum" = 0 OR T0."MaxSum" Is Null);
			end if;
		end if;

		if (:ddctPrcnt = -1) then
			ddctPrcnt := :ddgPrcnt;
		end if;



		/****************	Update the card	*****************/
    	UPDATE OCRD
	    SET "DdctPrcnt" = :ddctPrcnt
		WHERE "CardCode"  = :cardCode;
		
		IF Currs::NOTFOUND THEN
			c_next := -1;
		END IF;	
	End While; 
 
	Close Currs;
	
	result := 0;
	return;
end;


