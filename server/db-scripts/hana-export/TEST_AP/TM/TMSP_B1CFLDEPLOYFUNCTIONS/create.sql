-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_B1CFLDeployFunctions
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_sql nclob;
	B1CFLSetTimeToZero varchar(1000);
	B1CFLMonthEnd varchar(1000);
	B1CFLMonthStart varchar(1000);
	B1CFLMonthHalf varchar(1000);
	B1CFLAddMonthsAndDays varchar(1000);
	B1CFLCreateDate varchar(1000);
	B1CFLDayOfWeek varchar(1000);
	v_cnt integer;
BEGIN
	
	select count(*) into v_cnt from "PUBLIC".procedures where procedure_name = 'B1CFLCONSIDERHOLIDAY' and schema_name = current_schema;
	
	if :v_cnt > 0 then
		exec ('drop procedure B1CFLCONSIDERHOLIDAY');
	END if;

	--here is the workaround for HANA because HANA doesnt support standard ANSI sql function yet.  here is only one complex sql procedure B1CFLConsiderHoliday
	-- other sql functions are converted to inline builtin function as indicated below:
	-- once HANA function is ready , consider refactor them.\
	-- the only block will call those functions is TmSp_B1CFLGetDocuments 

	B1CFLSetTimeToZero := 'to_date(:v_datetime)';
	B1CFLMonthEnd := 'last_day(:v_datetime)';
	B1CFLMonthStart := 'next_day(' || :B1CFLMonthEnd || ')';
	B1CFLMonthHalf := 'floor(DAYOFMONTH(' || :B1CFLMonthEnd|| '))/2+1';
	B1CFLAddMonthsAndDays := 'to_date(add_days(add_months(:v_datetime,:v_addmonth),:v_addday))';
	B1CFLCreateDate := 'add_months(add_days(to_date(:v_year),:v_day - 1),:v_month - 1)';
	B1CFLDayOfWeek := 'MAP(WEEKDAY(:v_datetime),0,2,1,3,2,4,3,5,4,6,5,7,6,1)';
	
	v_sql := '
	CREATE PROCEDURE B1CFLConsiderHoliday(v_dtIn DATE, v_nvHldCode NVARCHAR(20), out v_dtOut DATE)
	AS
		v_siWndFrm SMALLINT; -- Although OHLD.WndFrm is NVARCHAR(1) !!!
		v_siWndTo SMALLINT; -- Although OHLD.WndTo is NVARCHAR(1) !!!
		v_cIsCurYear NVARCHAR(1);
		v_cIgnrWnd NVARCHAR(1);
		v_siWeekDay SMALLINT;
		v_dtLast DATE;
		l_dtIn DATE;
		v_cnt integer := 0;
BEGIN
	IF IFNULL(:v_nvHldCode,'''') = '''' THEN
		v_dtOut := :v_dtIn;
	ELSE 
		
		select count(*) into v_cnt FROM OHLD WHERE "HldCode" = :v_nvHldCode;
		if :v_cnt > 0 then
			SELECT to_smallint("WndFrm"),to_smallint("WndTo"),"isCurYear","ignrWnd" INTO v_siWndFrm, v_siWndTo, 
			v_cIsCurYear,v_cIgnrWnd FROM OHLD WHERE "HldCode" = :v_nvHldCode;
		end if;
		v_dtLast := :v_dtIn;
		l_dtIn := :v_dtIn;
		
		WHILE :v_dtLast is not null do
		
			IF :v_cIsCurYear = ''Y'' then
				select count(*) into v_cnt FROM HLD1 WHERE "HldCode" = :v_nvHldCode AND :l_dtIn >= "StrDate" AND :l_dtIn <= "EndDate";
				if :v_cnt > 0 then
					SELECT MAX("EndDate") into v_dtLast FROM HLD1 WHERE "HldCode" = :v_nvHldCode AND :l_dtIn >= "StrDate" AND :l_dtIn <= "EndDate";
				else 
					v_dtLast := Null;
				end if;
			ELSE
				select count(*) into v_cnt FROM HLD1
					WHERE "HldCode" = :v_nvHldCode AND
					:l_dtIn >= add_months(add_days(to_date(to_varchar(YEAR(:l_dtIn))),EXTRACT (DAY FROM "StrDate") - 1),MONTH("StrDate") - 1) AND
					:l_dtIn <= add_months(add_days(to_date(to_varchar(YEAR(:l_dtIn))),EXTRACT (DAY FROM "EndDate") - 1),MONTH("EndDate") - 1);
				if :v_cnt > 0 then
					SELECT MAX(add_months(add_days(to_date(to_varchar(YEAR(:l_dtIn))),EXTRACT (DAY FROM "EndDate") - 1),MONTH("EndDate") - 1)) INTO v_dtLast FROM HLD1
					WHERE "HldCode" = :v_nvHldCode AND
					:l_dtIn >= add_months(add_days(to_date(to_varchar(YEAR(:l_dtIn))),EXTRACT (DAY FROM "StrDate") - 1),MONTH("StrDate") - 1) AND
					:l_dtIn <= add_months(add_days(to_date(to_varchar(YEAR(:l_dtIn))),EXTRACT (DAY FROM "EndDate") - 1),MONTH("EndDate") - 1);
				else 
					v_dtLast := null;
				end if;

			END IF;
			
			IF :v_dtLast is not null then 
				l_dtIn := ADD_DAYS(:l_dtIn,DAYS_BETWEEN(:l_dtIn, :v_dtLast) + 1);
			ELSE
				IF :v_cIgnrWnd = ''N'' then						
					select MAP(WEEKDAY(:l_dtIn),0,2,1,3,2,4,3,5,4,6,5,7,6,1) into v_siWeekDay from dummy;
					IF :v_siWndFrm <= :v_siWndTo THEN
						IF :v_siWeekDay >= :v_siWndFrm AND :v_siWeekDay <= :v_siWndTo THEN
							l_dtIn := ADD_DAYS(:l_dtIn,:v_siWndTo - :v_siWeekDay + 1);
							v_dtLast := :l_dtIn;
						END IF;
					ELSE
						IF :v_siWeekDay >= :v_siWndFrm then
							l_dtIn := ADD_DAYS(:l_dtIn,8 + :v_siWndTo - :v_siWeekDay);
							v_dtLast := l_dtIn;
						ELSE
							IF :v_siWeekDay <= :v_siWndTo THEN
								l_dtIn := ADD_DAYS(:l_dtIn, 1 + :v_siWndTo - :v_siWeekDay);
								v_dtLast := :l_dtIn;
							END IF;
						END if;
					END IF;
				END IF;
			END if;
		end while;
		v_dtOut := to_date(:l_dtIn);
	END IF;
END';
	
	
	EXEC (:v_sql);
END;


