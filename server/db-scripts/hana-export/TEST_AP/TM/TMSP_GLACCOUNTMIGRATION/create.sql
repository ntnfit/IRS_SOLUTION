-- B1 DEPENDS: AFTER:PT:PROCESS_END
CREATE PROCEDURE TmSp_GLAccountMigration () 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
BEGIN
		DECLARE TABLE_EXISTS INT;
		DECLARE countOfByWHS INT;
		
		DECLARE countOfByIG INT;
		DECLARE countOfByITEM INT;
		DECLARE sql1 CLOB;
		DECLARE sql2 CLOB;
		DECLARE sql3 CLOB;
		DECLARE accountLevel NVARCHAR(100);
		DECLARE accountCompany NVARCHAR(100);
		DECLARE lastAcct NVARCHAR(100);
		DECLARE accountLevelStr NVARCHAR(100);
		DECLARE accountCompanyStr NVARCHAR(100);
		
		DECLARE s_periodCat NVARCHAR(100); 
		DECLARE s_financYear NVARCHAR(100);
		DECLARE s_year NVARCHAR(100);
		DECLARE s_periodName NVARCHAR(100);
		DECLARE s_subType NVARCHAR(100);
		DECLARE s_periodNum NVARCHAR(100);
		DECLARE f_RefDate NVARCHAR(100);
		DECLARE t_RefDate NVARCHAR(100);
		DECLARE f_DueDate NVARCHAR(100);
		DECLARE t_DueDate NVARCHAR(100);
		DECLARE f_TaxDate NVARCHAR(100);
		DECLARE t_TaxDate NVARCHAR(100);
		
		DECLARE FILEMAPCNT INT;
		DECLARE TEMP3CNT INT;
		DECLARE i INT;
		DECLARE j INT;
		
		-------DECLARE FOR STEP 5------
		DECLARE absEntry INT;
		DECLARE ruleCodeIndex INT;
		DECLARE ruleCode NVARCHAR(100);
		DECLARE periodCatTmp NVARCHAR(100);
		DECLARE periodCatTmpPre NVARCHAR(100);
		DECLARE TEMP4CNT INT;
		
		
		/* Prepare table OGAR_TMP to save temp result */
		SELECT COUNT(*) INTO TABLE_EXISTS FROM M_TEMPORARY_TABLES WHERE TABLE_NAME = '#OGAR_TMP' AND SCHEMA_NAME = CURRENT_SCHEMA;
		IF TABLE_EXISTS > 0 THEN
			EXEC ('DROP TABLE #OGAR_TMP');
		END IF; 

		/* Create table OGAR_TMP - all fields are allowed to be NULL */
		CREATE LOCAL TEMPORARY TABLE #OGAR_TMP(
			AbsEntry INT,
			PeriodCat NVARCHAR(10),
			FinancYear DATETIME,
			Year SMALLINT,
			PeriodName NVARCHAR(20),
			SubType NVARCHAR(1),
			PeriodNum INT,
			F_RefDate DATETIME,
			T_RefDate DATETIME,
			F_DueDate DATETIME,
			T_DueDate DATETIME,
			F_TaxDate DATETIME,
			T_TaxDate DATETIME,
			LogInstanc SMALLINT,
			UpdateDate DATETIME,
			UserSign SMALLINT,
			UserSign2 SMALLINT,
			ItemCode NVARCHAR(50),
			ItmsGrpCod SMALLINT,
			WhsCode NVARCHAR(8),
			BPGrpCod SMALLINT,
			LicTradNum NVARCHAR(32),
			ShipCountr NVARCHAR(3),
			ShipState NVARCHAR(3),
			Comments NVARCHAR(254),
			CreateDate DATETIME,
			RuleCode NVARCHAR(20),
			GLMethod NVARCHAR(1), 
			Transfered NVARCHAR(1),
			FromDate DATETIME,
			ToDate DATETIME,
			DfltExpn NVARCHAR(15),
			DfltIncom NVARCHAR(15),
			ExmptIncom NVARCHAR(15),
			StockAct NVARCHAR(15),
			COGM_Act NVARCHAR(15),
			AlocCstAct NVARCHAR(15),
			VariancAct NVARCHAR(15),
			PricDifAct NVARCHAR(15),
			NegStckAct NVARCHAR(15),
			DfltLoss NVARCHAR(15),
			DfltProfit NVARCHAR(15),
			RturnngAct NVARCHAR(15),
			ECIncome NVARCHAR(15),
			ECExepnses NVARCHAR(15),
			ForgnIncm NVARCHAR(15),
			ForgnExpn NVARCHAR(15),
			PurchseAct NVARCHAR(15),
			PaReturnAc NVARCHAR(15),
			PaOffsetAc NVARCHAR(15),
			ExDiffAct NVARCHAR(15),
			BalanceAct NVARCHAR(15),
			DecresGlAc NVARCHAR(15),
			IncresGlAc NVARCHAR(15),
			WipAcct NVARCHAR(15),
			WipVarAcct NVARCHAR(15),
			WipOffset NVARCHAR(15),
			StockOffst NVARCHAR(15),
			StockRvAct NVARCHAR(15),
			StkRvOfAct NVARCHAR(15),
			CostRevAct NVARCHAR(15),
			CostOffAct NVARCHAR(15),
			ExpClrAct NVARCHAR(15),
			ExpOfstAct NVARCHAR(15),
			StkInTnAct NVARCHAR(15),
			ShpdGdsAct NVARCHAR(15),
			VatRevAct NVARCHAR(15),
			ARCMAct NVARCHAR(15),
			APCMAct NVARCHAR(15),
			ARCMExpAct NVARCHAR(15),
			ARCMFrnAct NVARCHAR(15),
			APCMFrnAct NVARCHAR(15),
			ARCMEUAct NVARCHAR(15),
			APCMEUAct NVARCHAR(15),
			PurBalAct NVARCHAR(15),
			WhICenAct NVARCHAR(15),
			WhOCenAct NVARCHAR(15),
			Active NVARCHAR(1),
			CmpPrivate NVARCHAR(1),
			VatGroup NVARCHAR(8),
			CardCode NVARCHAR(15),
			Usage INT,
			FreeChrgSA NVARCHAR(15),
			FreeChrgPU NVARCHAR(15),
			UDF1 NVARCHAR(50),
			UDF2 NVARCHAR(50),
			UDF3 NVARCHAR(50),
			UDF4 NVARCHAR(50),
			UDF5 NVARCHAR(50));
			
			SELECT COUNT(*) INTO TABLE_EXISTS FROM M_TEMPORARY_TABLES WHERE TABLE_NAME = '#FIELDMAP_TMP' AND SCHEMA_NAME = CURRENT_SCHEMA;
			IF TABLE_EXISTS > 0 THEN
				EXEC ('DROP TABLE #FIELDMAP_TMP');
			END IF; 
		
			CREATE LOCAL TEMPORARY TABLE #FIELDMAP_TMP (
			W NVARCHAR(100), 
			A NVARCHAR(100), 
			L NVARCHAR(100),
			ID INT);
			
		INSERT INTO #FIELDMAP_TMP VALUES ('"ExpensesAc"',    '"DfltExpn"','N','1');
		INSERT INTO #FIELDMAP_TMP VALUES ('"RevenuesAc"',       '"DfltIncom"','N','2');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ExmptIncom"',       '"ExmptIncom"','N','3');
		INSERT INTO #FIELDMAP_TMP VALUES ('"BalInvntAc"',       '"StockAct"','N','4');
		INSERT INTO #FIELDMAP_TMP VALUES ('"SaleCostAc"',       '"COGM_Act"','N','5');
		INSERT INTO #FIELDMAP_TMP VALUES ('"TransferAc"',       '"AlocCstAct"','N','6');
		INSERT INTO #FIELDMAP_TMP VALUES ('"VarianceAc"',       '"VariancAct"','N','7');
		INSERT INTO #FIELDMAP_TMP VALUES ('"PriceDifAc"',       '"PricDifAct"','N','8');
		INSERT INTO #FIELDMAP_TMP VALUES ('"NegStckAct"',       '"NegStckAct"','N','9');
		INSERT INTO #FIELDMAP_TMP VALUES ('"DecreasAc"',        '"DfltLoss"','N','10');
		INSERT INTO #FIELDMAP_TMP VALUES ('"IncreasAc"',        '"DfltProfit"','N','11');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ReturnAc"',         '"RturnngAct"','N','12');
		INSERT INTO #FIELDMAP_TMP VALUES ('"EURevenuAc"',       '"ECIncome"','N','13');
		INSERT INTO #FIELDMAP_TMP VALUES ('"EUExpensAc"',       '"ECExepnses"','N','14');
		INSERT INTO #FIELDMAP_TMP VALUES ('"FrRevenuAc"',       '"ForgnIncm"','N','15');
		INSERT INTO #FIELDMAP_TMP VALUES ('"FrExpensAc"',       '"ForgnExpn"','N','16');
		INSERT INTO #FIELDMAP_TMP VALUES ('"PurchaseAc"',       '"PurchseAct"','N','17');
		INSERT INTO #FIELDMAP_TMP VALUES ('"PAReturnAc"',       '"PaReturnAc"','N','18');
		INSERT INTO #FIELDMAP_TMP VALUES ('"PurchOfsAc"',       '"PaOffsetAc"','N','19');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ExchangeAc"',       '"ExDiffAct"','N','20');
		INSERT INTO #FIELDMAP_TMP VALUES ('"BalanceAcc"',       '"BalanceAct"','N','21');
		INSERT INTO #FIELDMAP_TMP VALUES ('"DecresGlAc"',       '"DecresGlAc"','N','22');
		INSERT INTO #FIELDMAP_TMP VALUES ('"IncresGlAc"',       '"IncresGlAc"','N','23');
		INSERT INTO #FIELDMAP_TMP VALUES ('"WipAcct"',          '"WipAcct"','N','24');
		INSERT INTO #FIELDMAP_TMP VALUES ('"WipVarAcct"',       '"WipVarAcct"','N','25');
		INSERT INTO #FIELDMAP_TMP VALUES ('"WipOffset"',        '"WipOffset"','N','26');
		INSERT INTO #FIELDMAP_TMP VALUES ('"StockOffst"',       '"StockOffst"','N','27');
		INSERT INTO #FIELDMAP_TMP VALUES ('"StokRvlAct"',       '"StockRvAct"','N','28');
		INSERT INTO #FIELDMAP_TMP VALUES ('"StkOffsAct"',       '"StkRvOfAct"','N','29');
		INSERT INTO #FIELDMAP_TMP VALUES ('"CostRvlAct"',       '"CostRevAct"','N','30');
		INSERT INTO #FIELDMAP_TMP VALUES ('"CstOffsAct"',       '"CostOffAct"','N','31');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ExpClrAct"',        '"ExpClrAct"','N','32');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ExpOfstAct"',       '"ExpOfstAct"','N','33');
		INSERT INTO #FIELDMAP_TMP VALUES ('"StkInTnAct"',       '"StkInTnAct"','N','34');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ShpdGdsAct"',       '"ShpdGdsAct"','N','35');
		INSERT INTO #FIELDMAP_TMP VALUES ('"VatRevAct"',        '"VatRevAct"','N','36');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ARCMAct"',          '"ARCMAct"','N','37');
		INSERT INTO #FIELDMAP_TMP VALUES ('"APCMAct"',          '"APCMAct"','N','38');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ARCMExpAct"',       '"ARCMExpAct"','N','39');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ARCMFrnAct"',       '"ARCMFrnAct"','N','40');
		INSERT INTO #FIELDMAP_TMP VALUES ('"APCMFrnAct"',       '"APCMFrnAct"','N','41');
		INSERT INTO #FIELDMAP_TMP VALUES ('"ARCMEUAct"',        '"ARCMEUAct"','N','42');
		INSERT INTO #FIELDMAP_TMP VALUES ('"APCMEUAct"',        '"APCMEUAct"','N','43');
		INSERT INTO #FIELDMAP_TMP VALUES ('"PurBalAct"',        '"PurBalAct"','N','44');
		INSERT INTO #FIELDMAP_TMP VALUES ('"WhICenAct"',        '"WhICenAct"','N','45');
		INSERT INTO #FIELDMAP_TMP VALUES ('"WhOCenAct"',        '"WhOCenAct"','Y','46');

				
		SELECT COUNT(*) INTO TABLE_EXISTS FROM M_TEMPORARY_TABLES WHERE TABLE_NAME = '#TEMP3' AND SCHEMA_NAME = CURRENT_SCHEMA;
		IF TABLE_EXISTS > 0 THEN
			EXEC ('DROP TABLE #TEMP3');
		END IF; 
		
		CREATE LOCAL TEMPORARY TABLE #TEMP3 AS (
		SELECT ROW_NUMBER() OVER(ORDER BY OACP."PeriodCat") AS ID, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName",	OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", 
			   OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate" 
			   FROM OACP  
			   WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1  
			   ORDER BY OACP."PeriodCat");

		SELECT COUNT(1) INTO countOfByWHS FROM OITM WHERE "GLMethod" = 'W';
		SELECT COUNT(1) INTO countOfByIG FROM OITM WHERE "GLMethod" = 'C';
		SELECT COUNT(1) INTO countOfByITEM FROM OITM WHERE "GLMethod" = 'L';
		SELECT COUNT(1) INTO FILEMAPCNT FROM #FIELDMAP_TMP;
		SELECT COUNT(1) INTO TEMP3CNT FROM #TEMP3;
		
		
		IF countOfByWHS > 0 AND countOfByIG + countOfByITEM = 0 THEN 
			/* Overall query SQL */
			sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', ''-1'', OWHS."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';
	
			/* 46 accounts selection */
			sql2 := '';
	
			/* 46 accounts condition */
			sql3 := '';
	
			i := 1;
			WHILE :i <= :FILEMAPCNT DO
				SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
				
				sql2 := CONCAT(sql2, ', ');
				accountLevelStr := CONCAT('OWHS.', accountLevel);
				accountCompanyStr := CONCAT('OACP.', accountCompany);
				sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
				sql2 := CONCAT(sql2, accountLevelStr);
				sql2 := CONCAT(sql2, '), LCASE(');
				sql2 := CONCAT(sql2, accountCompanyStr);
				sql2 := CONCAT(sql2, '))');
				
				/* build condition */
				sql3 := CONCAT(sql3, '((OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' OR OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' IS NULL');
				sql3 := CONCAT(sql3, ' OR OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' = '''') AND OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != '''' AND OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' IS NOT NULL)');
				IF lastAcct = 'N' THEN
					sql3 := CONCAT(sql3, ' OR ');
				END IF;
				i :=:i+1;
			END WHILE;
			/* Build overall query SQL */
			sql1 := CONCAT(sql1, sql2);
			sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OWHS INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
			sql1 := CONCAT(sql1, sql3);

			/* Add order by in query SQL */
			sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

			/* Execute SQL */
			EXEC sql1;
			
		
		/*
		Case II : If all value are 'C', which means all items of the company were set G/L account by Item Group

		1. Take all types of the accounts from itemgroups whose values are different from company level.
		*/
		
		ELSEIF countOfByIG > 0 AND countOfByWHS + countOfByITEM = 0 THEN
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', OITB."ItmsGrpCod", ''!^|'', ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';
		
		/* Loop 46 accounts */
		i := 1;
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
			/* build selection */
			sql2 := CONCAT(sql2, ', ');
			accountLevelStr := CONCAT('OITB.', accountLevel);
			accountCompanyStr := CONCAT('OACP.', accountCompany);
			sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
			sql2 := CONCAT(sql2, accountLevelStr);
			sql2 := CONCAT(sql2, '), LCASE(');
			sql2 := CONCAT(sql2, accountCompanyStr);
			sql2 := CONCAT(sql2, '))');
			
			/* build condition */
			sql3 := CONCAT(sql3, '((OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' IS NULL');
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' = '''') AND OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != '''' AND OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' IS NOT NULL)');

			IF lastAcct = 'N' THEN
				sql3 := CONCAT(sql3, ' OR ');
			END IF;
			i :=:i+1;
		END WHILE;
		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2);
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITB INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
		
		/*
		Case III : If all value are 'W' or 'L', which means all items were set by Warehouse or by item

		1. Take all types of the accounts from warehouses whose values are different from company level.
		2. Take all types of the accounts from table OITW whose values are different from warehouses.
		*/
		
		ELSEIF countOfByWHS + countOfByITEM > 0 AND countOfByIG = 0 THEN  
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType",	OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', ''-1'', OWHS."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';
		
		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';
		i := 1;
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
			/* build selection */
			sql2 := CONCAT(sql2, ', ');
			accountLevelStr := CONCAT('OWHS.', accountLevel);
			accountCompanyStr := CONCAT('OACP.', accountCompany);
			sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
			sql2 := CONCAT(sql2, accountLevelStr);
			sql2 := CONCAT(sql2, '), LCASE(');
			sql2 := CONCAT(sql2, accountCompanyStr);
			sql2 := CONCAT(sql2, '))');
			
			/* build condition */
			sql3 := CONCAT(sql3, '((OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' IS NULL');
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' = '''') AND OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != '''' AND OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' IS NOT NULL)');
			IF lastAcct = 'N' THEN
				sql3 := CONCAT(sql3, ' OR ');
			END IF;
		i := :i+1;
		END WHILE;
		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2);
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OWHS INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
		
		
		/* Overall query SQL */
		--sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, OITW."ItemCode", OITM."ItmsGrpCod", OITW."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

		/* 46 accounts selection */
		--sql2 := '';

		/* 46 accounts condition */
		--sql3 := '';
		
		/* Loop 46 accounts */
	    
	    j :=1;
	    
		WHILE :j <= :TEMP3CNT DO
		
			/* Overall query SQL */
			sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, OITW."ItemCode", OITM."ItmsGrpCod", OITW."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

			/* 46 accounts selection */
			sql2 := '';

			/* 46 accounts condition */
			sql3 := '';
		
			SELECT "PeriodCat", "FinancYear", "Year", "PeriodName","SubType" , "PeriodNum", "F_RefDate","T_RefDate", "F_DueDate","T_DueDate","F_TaxDate","T_TaxDate"  
						INTO s_periodCat, s_financYear, s_year, s_periodName, s_subType, s_periodNum, f_RefDate, t_RefDate, f_DueDate, t_DueDate, f_TaxDate, t_TaxDate 
		    FROM #TEMP3 WHERE ID=:j;
			
			i :=1;
			
			WHILE :i <= :FILEMAPCNT DO
				SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
				/* build selection */
				sql2 := CONCAT(sql2, ', ');
				accountLevelStr := CONCAT('OITW.', accountLevel);
				accountCompanyStr := CONCAT('OWHS.', accountLevel);
				sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
				sql2 := CONCAT(sql2, accountLevelStr);
				sql2 := CONCAT(sql2, '), LCASE(');
				sql2 := CONCAT(sql2, accountCompanyStr);
				sql2 := CONCAT(sql2, '))');
				
				/* build condition */
				sql3 := CONCAT(sql3, '((OITW.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' OR OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' IS NULL');
				sql3 := CONCAT(sql3, ' OR OWHS.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' = '''') AND OITW.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != '''' AND OITW.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' IS NOT NULL)');
				IF lastAcct = 'N' THEN
					sql3 := CONCAT(sql3, ' OR ');
				END IF;
				i :=:i+1;
			END WHILE;
			/* Build overall query SQL */
			sql1 := CONCAT(sql1, sql2);
			sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITW INNER JOIN OITM ON OITW."ItemCode" = OITM."ItemCode" INNER JOIN OWHS ON OITW."WhsCode" = OWHS."WhsCode" WHERE OITM."GLMethod" = ''L'' AND ');
			sql1 := CONCAT(sql1, sql3);

			/* Execute SQL */
			EXEC sql1;
			--select sql1 from dummy;
			
			sql2 := '';
			sql3 := '';

			/* Fill values for PeriodCat, FinancYear ... */
			UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODCAT = s_periodCat WHERE #OGAR_TMP.PERIODCAT = '' OR #OGAR_TMP.PERIODCAT IS NULL;
			UPDATE #OGAR_TMP SET #OGAR_TMP.FINANCYEAR = s_financYear WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.FINANCYEAR = '' OR #OGAR_TMP.FINANCYEAR IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.YEAR = TO_SMALLINT(s_year)  WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND #OGAR_TMP.YEAR IS NULL;
			UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODNAME = s_periodName  WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.PERIODNAME = '' OR #OGAR_TMP.PERIODNAME IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.SUBTYPE = s_subType WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.SUBTYPE = '' OR #OGAR_TMP.SUBTYPE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODNUM = s_periodNum WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND #OGAR_TMP.PERIODNUM IS NULL;
			UPDATE #OGAR_TMP SET #OGAR_TMP.F_REFDATE = f_RefDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_REFDATE = '' OR #OGAR_TMP.F_REFDATE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.T_REFDATE = t_RefDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_REFDATE = '' OR #OGAR_TMP.T_REFDATE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.F_DUEDATE = f_DueDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_DUEDATE = '' OR #OGAR_TMP.F_DUEDATE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.T_DUEDATE = t_DueDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_DUEDATE = '' OR #OGAR_TMP.T_DUEDATE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.F_TAXDATE = f_TaxDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_TAXDATE = '' OR #OGAR_TMP.F_TAXDATE IS NULL);
			UPDATE #OGAR_TMP SET #OGAR_TMP.T_TAXDATE = t_TaxDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_TAXDATE = '' OR #OGAR_TMP.T_TAXDATE IS NULL);
		j :=:j+1;
		END WHILE;

------------------------------------------------------------------------------------------------------------------------------------------------
/*
	Case IV : If all value are 'C' or 'L', which means all items were set by Item Group or by item

	1. Take all types of the accounts from itemgroups whose values are different from company level.
	2. Take all types of the accounts from table OITW whose values are different from itemgroups.
*/
		ELSEIF countOfByIG + countOfByITEM > 0 AND countOfByWHS = 0 THEN
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', OITB."ItmsGrpCod", ''!^|'', ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';
		i := 1;
		/* Loop 46 accounts */
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
				/* build selection */
				sql2 := CONCAT(sql2, ', ');
				accountLevelStr := CONCAT('OITB.', accountLevel);
				accountCompanyStr := CONCAT('OACP.', accountCompany);
				sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
				sql2 := CONCAT(sql2, accountLevelStr);
				sql2 := CONCAT(sql2, '), LCASE(');
				sql2 := CONCAT(sql2, accountCompanyStr);
				sql2 := CONCAT(sql2, '))');
				
				/* build condition */
				sql3 := CONCAT(sql3, '((OITB.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' OR OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' IS NULL');
				sql3 := CONCAT(sql3, ' OR OACP.');
				sql3 := CONCAT(sql3, accountCompany);
				sql3 := CONCAT(sql3, ' = '''') AND OITB.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' != '''' AND OITB.');
				sql3 := CONCAT(sql3, accountLevel);
				sql3 := CONCAT(sql3, ' IS NOT NULL)');
				IF lastAcct = 'N' THEN 
					sql3 := CONCAT(sql3, ' OR ');
				END IF;
			i := :i+1;
		END WHILE;

		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2); 
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITB INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
		
		/*--------------------------------------------------------------------------------------------------*/
		j :=1;
	    
		/* Loop all posting periods */
		WHILE :j <= :TEMP3CNT DO 
				i :=1;
				/* Overall query SQL */
				sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, OITW."ItemCode", OITM."ItmsGrpCod", OITW."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

				/* 46 accounts selection */
				sql2 := '';

				/* 46 accounts condition */
				sql3 := '';

				/* Loop 46 accounts */
			SELECT "PeriodCat", "FinancYear", "Year", "PeriodName","SubType" , "PeriodNum", "F_RefDate","T_RefDate", "F_DueDate","T_DueDate","F_TaxDate","T_TaxDate"  
			INTO s_periodCat, s_financYear, s_year, s_periodName, s_subType, s_periodNum, f_RefDate, t_RefDate, f_DueDate, t_DueDate, f_TaxDate, t_TaxDate 
		    FROM #TEMP3 WHERE ID=:j;
			WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
						/* build selection */
						sql2 := CONCAT(sql2, ', ');
						accountLevelStr := CONCAT('OITW.', accountLevel);
						accountCompanyStr := CONCAT('OITB.', accountLevel);
						sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
						sql2 := CONCAT(sql2, accountLevelStr);
						sql2 := CONCAT(sql2, '), LCASE(');
						sql2 := CONCAT(sql2, accountCompanyStr);
						sql2 := CONCAT(sql2, '))');

						/* build condition */
						sql3 := CONCAT(sql3, '((OITW.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' != OITB.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' OR OITB.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' IS NULL');
						sql3 := CONCAT(sql3, ' OR OITB.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' = '''') AND OITW.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' != '''' AND OITW.');
						sql3 := CONCAT(sql3, accountLevel);
						sql3 := CONCAT(sql3, ' IS NOT NULL)');
						IF lastAcct = 'N' THEN 
							sql3 := CONCAT(sql3, ' OR ');
						END IF;
						i :=:i+1;
					END WHILE;

				/* Build overall query SQL */
				sql1 := CONCAT(sql1, sql2);
				sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITW INNER JOIN OITM ON OITW."ItemCode" = OITM."ItemCode" INNER JOIN OITB ON OITB."ItmsGrpCod" = OITM."ItmsGrpCod" WHERE OITM."GLMethod" = ''L'' AND ');
				sql1 := CONCAT(sql1, sql3);

				/* Execute SQL */
				EXEC sql1;
				--select sql1 from dummy;

				/* Fill values for PeriodCat, FinancYear ... */
				UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODCAT = s_periodCat WHERE #OGAR_TMP.PERIODCAT = '' OR #OGAR_TMP.PERIODCAT IS NULL;
				UPDATE #OGAR_TMP SET #OGAR_TMP.FINANCYEAR = s_financYear WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.FINANCYEAR = '' OR #OGAR_TMP.FINANCYEAR IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.YEAR = TO_SMALLINT(s_year)  WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND #OGAR_TMP.YEAR IS NULL;
				UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODNAME = s_periodName  WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.PERIODNAME = '' OR #OGAR_TMP.PERIODNAME IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.SUBTYPE = s_subType WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.SUBTYPE = '' OR #OGAR_TMP.SUBTYPE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.PERIODNUM = s_periodNum WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND #OGAR_TMP.PERIODNUM IS NULL;
				UPDATE #OGAR_TMP SET #OGAR_TMP.F_REFDATE = f_RefDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_REFDATE = '' OR #OGAR_TMP.F_REFDATE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.T_REFDATE = t_RefDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_REFDATE = '' OR #OGAR_TMP.T_REFDATE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.F_DUEDATE = f_DueDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_DUEDATE = '' OR #OGAR_TMP.F_DUEDATE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.T_DUEDATE = t_DueDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_DUEDATE = '' OR #OGAR_TMP.T_DUEDATE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.F_TAXDATE = f_TaxDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.F_TAXDATE = '' OR #OGAR_TMP.F_TAXDATE IS NULL);
				UPDATE #OGAR_TMP SET #OGAR_TMP.T_TAXDATE = t_TaxDate WHERE #OGAR_TMP.PERIODCAT = s_periodCat AND (#OGAR_TMP.T_TAXDATE = '' OR #OGAR_TMP.T_TAXDATE IS NULL);
				j :=:j+1;
		END WHILE;
		
		
------------------------------------------------------------------------------------------------------------------------------------------------		

		
	ELSEIF countOfByWHS > 0 AND countOfByIG > 0 THEN 
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', ''-1'', OWHS."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''W'', ''N'', NULL, NULL';
		
		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';

		/* Loop 46 accounts */	
		i := 1;
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
			/* build selection */
			sql2 := CONCAT(sql2, ', ');
			accountLevelStr := CONCAT('OWHS.', accountLevel);
			accountCompanyStr := CONCAT('OACP.', accountCompany);
			sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
			sql2 := CONCAT(sql2, accountLevelStr);
			sql2 := CONCAT(sql2, '), LCASE(');
			sql2 := CONCAT(sql2, accountCompanyStr);
			sql2 := CONCAT(sql2, '))');

			/* build condition */
			sql3 := CONCAT(sql3, '((OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' IS NULL');
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' = '''') AND OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != '''' AND OWHS.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' IS NOT NULL)');
			IF lastAcct = 'N' THEN
				sql3 := CONCAT(sql3, ' OR ');
			END IF;
		i := :i+1;
		END WHILE;
		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2);
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OWHS INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
		
		i := 1;
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType",	OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, ''!^|'', OITB."ItmsGrpCod", ''!^|'', ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''C'', ''N'', NULL, NULL';

		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
			/* build selection */
			sql2 := CONCAT(sql2, ', ');
			accountLevelStr := CONCAT('OITB.', accountLevel);
			accountCompanyStr := CONCAT('OACP.', accountCompany);
			sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
			sql2 := CONCAT(sql2, accountLevelStr);
			sql2 := CONCAT(sql2, '), LCASE(');
			sql2 := CONCAT(sql2, accountCompanyStr);
			sql2 := CONCAT(sql2, '))');
			
			/* build condition */
			sql3 := CONCAT(sql3, '((OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' IS NULL');
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' = '''') AND OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != '''' AND OITB.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' IS NOT NULL)');
			IF lastAcct = 'N' THEN 
				sql3 := CONCAT(sql3, ' OR ');
			END IF;
		i := :i+1;
		END WHILE;
		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2);
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITB INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
		
		i := 1;
		/* Overall query SQL */
		sql1 := 'INSERT INTO #OGAR_TMP SELECT NULL, OACP."PeriodCat", OACP."FinancYear", OACP."Year", OACP."PeriodName", OACP."SubType", OACP."PeriodNum", OACP."F_RefDate", OACP."T_RefDate", OACP."F_DueDate", OACP."T_DueDate", OACP."F_TaxDate", OACP."T_TaxDate", NULL, NULL, NULL, NULL, OITW."ItemCode", OITM."ItmsGrpCod", OITW."WhsCode", ''-1'', ''!^|'', ''!^|'', ''!^|'', NULL, NULL, NULL, ''A'', ''N'', NULL, NULL';

		/* 46 accounts selection */
		sql2 := '';
		
		/* 46 accounts condition */
		sql3 := '';
		
		/* Loop 46 accounts */
		WHILE :i <= :FILEMAPCNT DO
			SELECT W, A, L INTO accountLevel, accountCompany, lastAcct FROM #FIELDMAP_TMP WHERE ID = :i;
			/* build selection */
			sql2 := CONCAT(sql2, ', ');
			accountLevelStr := CONCAT('OITW.', accountLevel);
			accountCompanyStr := CONCAT('OACP.', accountCompany);
			sql2 := CONCAT(sql2, 'NULLIF(LCASE(');
			sql2 := CONCAT(sql2, accountLevelStr);
			sql2 := CONCAT(sql2, '), LCASE(');
			sql2 := CONCAT(sql2, accountCompanyStr);
			sql2 := CONCAT(sql2, '))');

			/* build condition */
			sql3 := CONCAT(sql3, '((OITW.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' IS NULL');
			sql3 := CONCAT(sql3, ' OR OACP.');
			sql3 := CONCAT(sql3, accountCompany);
			sql3 := CONCAT(sql3, ' = '''') AND OITW.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' != '''' AND OITW.');
			sql3 := CONCAT(sql3, accountLevel);
			sql3 := CONCAT(sql3, ' IS NOT NULL)');
			IF lastAcct = 'N' THEN 
				sql3 := CONCAT(sql3, ' OR ');
			END IF;
		i := :i+1;
		END WHILE;
		/* Build overall query SQL */
		sql1 := CONCAT(sql1, sql2);
		sql1 := CONCAT(sql1, ', ''Y'', ''A'', ''!^|'', ''!^|'', ''-1'', NULL, NULL, ''!^|'', ''!^|'', ''!^|'', ''!^|'', ''!^|'' FROM OITW INNER JOIN OITM ON OITW."ItemCode" = OITM."ItemCode" INNER JOIN (SELECT OACP.* FROM OACP WHERE (SELECT COUNT(*) FROM OFPR WHERE OACP."PeriodCat" = OFPR."Category") >= 1) AS OACP ON 1 = 1 WHERE OITM."GLMethod" = ''L'' AND ');
		sql1 := CONCAT(sql1, sql3);

		/* Add order by in query SQL */
		sql1 := CONCAT(sql1, ' ORDER BY OACP."PeriodCat"');

		/* Execute SQL */
		EXEC sql1;
	END IF;	
	
	-------step 4----------
	SELECT COUNT(*) INTO TABLE_EXISTS FROM M_TEMPORARY_TABLES WHERE TABLE_NAME = '#OGAR_TMP2' AND SCHEMA_NAME = CURRENT_SCHEMA;
	IF TABLE_EXISTS > 0 THEN
		EXEC ('DROP TABLE #OGAR_TMP2');
	END IF; 
	CREATE LOCAL TEMPORARY TABLE #OGAR_TMP2 AS (
	SELECT ROW_NUMBER() OVER(ORDER BY PeriodCat) AS ABSENTRY,
				PeriodCat,
				FinancYear,
				Year,
				PeriodName,
				SubType,
				PeriodNum,
				F_RefDate,
				T_RefDate,
				F_DueDate,
				T_DueDate,
				F_TaxDate,
				T_TaxDate,
				LogInstanc,
				UpdateDate,
				UserSign,
				UserSign2,
				ItemCode,
				ItmsGrpCod,
				WhsCode,
				BPGrpCod,
				LicTradNum,
				ShipCountr,
				ShipState,
				Comments,
				CreateDate,
				RuleCode,
				GLMethod,
				Transfered,
				FromDate,
				ToDate,
				DfltExpn,
				DfltIncom,
				ExmptIncom,
				StockAct,
				COGM_Act,
				AlocCstAct,
				VariancAct,
				PricDifAct,
				NegStckAct,
				DfltLoss,
				DfltProfit,
				RturnngAct,
				ECIncome,
				ECExepnses,
				ForgnIncm,
				ForgnExpn,
				PurchseAct,
				PaReturnAc,
				PaOffsetAc,
				ExDiffAct,
				BalanceAct,
				DecresGlAc,
				IncresGlAc,
				WipAcct,
				WipVarAcct,
				WipOffset,
				StockOffst,
				StockRvAct,
				StkRvOfAct,
				CostRevAct,
				CostOffAct,
				ExpClrAct,
				ExpOfstAct,
				StkInTnAct,
				ShpdGdsAct,
				VatRevAct,
				ARCMAct,
				APCMAct,
				ARCMExpAct,
				ARCMFrnAct,
				APCMFrnAct,
				ARCMEUAct,
				APCMEUAct,
				PurBalAct,
				WhICenAct,
				WhOCenAct,
				Active,
				CmpPrivate,
				VatGroup,
				CardCode,
				Usage,
				FreeChrgSA,
				FreeChrgPU,
				UDF1,
				UDF2,
				UDF3,
				UDF4,
				UDF5
				 FROM #OGAR_TMP);
	INSERT INTO OGAR SELECT * FROM #OGAR_TMP2;
	
	/* update AutoKey in ONNM table */
	UPDATE ONNM SET ONNM."AutoKey" = (SELECT MAX(ABSENTRY)+1 FROM #OGAR_TMP2) WHERE ONNM."ObjectCode" =  '1470000057';
	--EXEC ('UPDATE ONNM SET ONNM."AutoKey" = ABS_SEQUENCE.NEXTVAL WHERE ONNM."ObjectCode" =  ''1470000057''');  -- value of object GAR is 1470000057.
	
	/* In Case V: GLPickMeth should equal to GLMethod in OITM when it is set as ByWHS/ByIG */
	SELECT COUNT(1) INTO countOfByWHS FROM OITM WHERE OITM."GLMethod" = 'W';
	SELECT COUNT(1) INTO countOfByIG FROM OITM WHERE OITM."GLMethod" = 'C';
	IF countOfByIG > 0 AND countOfByWHS > 0 THEN 
		UPDATE OITM SET OITM."GLPickMeth" = OITM."GLMethod" WHERE OITM."GLMethod" = 'W' OR OITM."GLMethod" = 'C';
		UPDATE OITM SET OITM."GLPickMeth" = 'A' WHERE OITM."GLMethod" = 'L';
	END IF;
	

	-----step 5---------
	SELECT COUNT(*) INTO TABLE_EXISTS FROM M_TEMPORARY_TABLES WHERE TABLE_NAME = '#TEMP4' AND SCHEMA_NAME = CURRENT_SCHEMA;
	IF TABLE_EXISTS > 0 THEN
		EXEC ('DROP TABLE #TEMP4');
	END IF; 
	
	CREATE LOCAL TEMPORARY TABLE #TEMP4 AS (SELECT ROW_NUMBER() OVER(ORDER BY OGAR."PeriodCat") AS ID, OGAR."AbsEntry", OGAR."PeriodCat" FROM OGAR ORDER BY OGAR."PeriodCat");
	SELECT COUNT(1) INTO TEMP4CNT FROM #TEMP4;
	
	IF :TEMP4CNT > 0 THEN
		SELECT "AbsEntry", "PeriodCat" INTO absEntry, periodCatTmp FROM #TEMP4 WHERE ID=1;
		/* Initialization, make periodCatTmpPre equals to periodCatTmp. */
		ruleCodeIndex := 1;
		periodCatTmpPre := periodCatTmp;
		
		i := 1;
		WHILE :i <= :TEMP4CNT DO
	
			IF NULLIF(periodCatTmpPre, periodCatTmp) IS NOT NULL THEN 
				ruleCodeIndex := 1;
				periodCatTmpPre := periodCatTmp;
			END IF;
			ruleCode := CONCAT('R', CAST(ruleCodeIndex AS NVARCHAR(50)));
			UPDATE OGAR SET OGAR."RuleCode" = :ruleCode WHERE OGAR."AbsEntry" = absEntry;
			ruleCodeIndex := ruleCodeIndex + 1;		
			
			i := :i+1;
			IF :i <= :TEMP4CNT THEN
				SELECT "AbsEntry", "PeriodCat" INTO absEntry, periodCatTmp FROM #TEMP4 WHERE ID=:i;
			END IF;
		END WHILE;
	END IF;
	
	EXEC ('drop table #OGAR_TMP');
	EXEC ('drop table #OGAR_TMP2');
	EXEC ('drop table #FIELDMAP_TMP');
	EXEC ('drop table #temp3');
	EXEC ('drop table #temp4');
END;


