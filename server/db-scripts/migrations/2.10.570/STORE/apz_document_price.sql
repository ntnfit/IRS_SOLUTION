
CALL APZ_EXISTSPROC('apz_document_price');

GO

CREATE procedure apz_document_price
(
in CardCode VARCHAR(20),
in DocDate timestamp,
in ItemCode NVARCHAR(50),
in Quantity NUMERIC(21,6),
in uomCode nvarchar(20),
in infoPriceList int,
out result TABLE(
	"Price" NUMERIC(21,6),
	"Currency" NVARCHAR(10),
	"Discount" NUMERIC(21,6),
	"CardCode" NVARCHAR(20),
	"DocDate" timestamp,
	"ItemCode" NVARCHAR(50),
	"Quantity" NUMERIC(21,6),
	"SpecPrice" nvarchar(10),
	"IsGrossPrc" char(1)
	)
)
AS
BEGIN
	------------------------ variable ----------------------------------
	DECLARE ListNum INT = -4;
	DECLARE checkCount INT;
	declare priceUnit  int;
	declare echangeInvToPriceUnit numeric(21,6);
	declare exchangeRate numeric(21,6);
	declare existsSetupUomPrice char(1);
	declare uomGroup int;
	declare isGrossPrc char(1);
	declare tempCardCode nvarchar(20);
	------------------------- END VARIABLE ----------------------------
	
	------------------------------------------- Temporary table ------------------------------------
	------------------------------------------------------------------------------------------------
	---------------- TABLE CHỨA KẾT QUẢ DỮ LIỆU -------------------------
	create local temporary table #tbl(
				"Price" NUMERIC(21,6),
				"Currency" NVARCHAR(10),
				"Discount" NUMERIC(21,6),
				"CardCode" NVARCHAR(20),
				"DocDate" timestamp,
				"ItemCode" NVARCHAR(50),
				"Quantity" NUMERIC(21,6),
				"SpecPrice" nvarchar(10),
				"IsGrossPrc" char(1)
			);
	---------------- CÁC TABLE CHỨA DỮ LIỆU SPECIAL PRICE ---------------
	---------------- -- biến bảng chứa dữ liệu special price của BP và item
	create local temporary table #tblospp(
		"Price" NUMERIC(21,6),
		"Currency" VARCHAR(10),
		"Discount" NUMERIC(21,6)
		);
	------------------- bảng lưu thông tin setting theo ngày tháng 
	create local temporary table #tblspp1(
		"LINENUM" INT,
		"Price" NUMERIC(21,6),
		"Currency" VARCHAR(10),
		"Discount" NUMERIC(21,6)
		);
	-------------------- bảng lưu thông tin setting theo số lượng	
	create local temporary table #tblspp2(
		"Price" NUMERIC(21,6),
		"Currency" VARCHAR(10),
		"Discount" NUMERIC(21,6)
		);
	--------------------- END TABLE SPECIAL PRICE -------------------------
	-------------------- Bảng lưu thông tin setting uom price
	CREATE LOCAL TEMPORARY TABLE #tblUomPrice("Price" numeric(21,6), "Currency" nvarchar(3));
	----------------------------------------------- END CREATE TEMPORARY TABLE -------------------------

	-------------------------- dữ liệu Price unit -------------------------------------------------
	select ifnull((select "PriceUnit" from OITM where "ItemCode" = :ItemCode),-1) into priceUnit from DUMMY;

	------------------ lấy đơn vị quy đổi từ inventory unit sang Price unit
	select ifnull((select a."AltQty" / ifnull(a."BaseQty", 0) from UGP1 a INNER JOIN (select "ItemCode", "UgpEntry", "PriceUnit" from OITM where "ItemCode" = :ItemCode) b on a."UgpEntry" = b."UgpEntry" and a."UomEntry" = b."PriceUnit"),1) into echangeInvToPriceUnit from DUMMY;

	----------------------- biến lưu giá trị chuyển đổi từ đơn vị hiện tại sang đơn vị PriceUnit ---------------
	select ifnull((select a."BaseQty" * :echangeInvToPriceUnit / ifnull(a."AltQty", 0) from UGP1 a INNER JOIN (select "ItemCode", "UgpEntry" from OITM where "ItemCode" = :ItemCode) b on a."UgpEntry" = b."UgpEntry"
																			INNER JOIN (select * from OUOM where "UomCode" = :uomCode) c ON a."UomEntry" = c."UomEntry"),1) into exchangeRate from DUMMY;

	if ifnull(:infoPriceList,0) < -2 or ifnull(:infoPriceList,0) = 0
	THEN
		select (SELECT TOP 1 "ListNum" FROM OCRD WHERE "CardCode"=:CardCode) into infoPriceList from DUMMY;
	END IF;
	
	select ifnull((select "IsGrossPrc" from OPLN where "ListNum" = :infoPriceList), 'N') into isGrossPrc from DUMMY;
	
	------------------------------------------------------------------ Lấy dữ liệu setting Uom Price --------------------------------------------------------------------------------------------
	insert into #tblUomPrice
		SELECT a."Price", a."Currency" FROM ITM9 a INNER JOIN OUOM b ON a."UomEntry" = b."UomEntry" WHERE a."ItemCode" = :itemCode AND b."UomCode" = :uomCode AND a."PriceList" = :infoPriceList;

	select ifnull((select top 1 'Y' from #tblUomPrice), 'N') into existsSetupUomPrice from DUMMY;

	-- Lấy dữ liệu special price theo setting cho BP và item
	INSERT INTO #tblospp ("Price", "Currency", "Discount") 
		SELECT case when a."ListNum" = 0 then a."Price" / (1 - ifnull(a."Discount", 0) / 100) else b."Price" end, a."Currency", "Discount"
		FROM OSPP a INNER JOIN itm1 b ON a."ItemCode" = b."ItemCode" AND a."ListNum" = b."PriceList"
		WHERE a."Valid" = 'Y' and "CardCode" = :CardCode AND a."ItemCode" = :ItemCode and :DocDate between ifnull(a."ValidFrom", :DocDate) and ifnull(a."ValidTo", :DocDate);
	
	tempCardCode := CardCode;
	
	-- Không có special price thì lấy theo period and discount volume
	if not exists(select 1 from #tblospp) then
		CardCode := '*' || cast(:infoPriceList as varchar);
		INSERT INTO #tblospp ("Price", "Currency", "Discount")
		SELECT case when a."ListNum" = 0 then a."Price" / (1 - ifnull(a."Discount", 0) / 100) else b."Price" end, a."Currency", "Discount"
		FROM OSPP a INNER JOIN itm1 b ON a."ItemCode" = b."ItemCode" AND a."ListNum" = b."PriceList"
		WHERE a."Valid" = 'Y' and "CardCode" = :CardCode AND a."ItemCode" = :ItemCode and :DocDate between ifnull(a."ValidFrom", :DocDate) and ifnull(a."ValidTo", :DocDate);
	end if;
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------- TÍNH TOÁN DỮ LIỆU THEO SPECIAL PRICE ---------------------------------------------------------------
	select ifnull((select count(1) from #tblospp), 0) into checkCount from DUMMY;
	IF :checkCount > 0
	THEN	
	    ---------------------- lấy dữ liệu setting theo ngày tháng ----------------------------------------
		INSERT INTO #tblspp1 (LINENUM, "Price", "Currency", "Discount") 
			SELECT TOP 1 LINENUM, case when a."ListNum" = 0 then a."Price" / (1 - ifnull(a."Discount", 0) / 100) else c."Price" end, a."Currency", a."Discount"
			FROM SPP1 a INNER JOIN OSPP b ON a."ItemCode" = b."ItemCode" AND a."CardCode" = b."CardCode" 
				INNER JOIN ITM1 c ON b."ItemCode" = c."ItemCode" AND b."ListNum" = c."PriceList" 
			WHERE a."CardCode" = :CardCode AND a."ItemCode" = :ItemCode
			AND :DocDate BETWEEN ifnull(a."FromDate", :DocDate) AND IFNULL(a."ToDate", :DocDate);

		select ifnull((select count(1) from #tblspp1), 0) into checkCount from DUMMY;
		IF :checkCount > 0
		THEN

			------------------------ Lấy dữ liệu setting theo số lượng và uom ---------------------------------------------
			INSERT INTO  #tblspp2("Price", "Currency", "Discount") 
				SELECT TOP 1 case when a."SPP2LNum" = 0 then a."Price" / (1 - ifnull(a."Discount", 0) / 100) else d."Price" end, a."Currency", a."Discount" 
					FROM SPP2 a INNER JOIN spp1 b ON a."ItemCode" = b."ItemCode" AND a."CardCode" = b."CardCode" AND a."SPP1LNum" = b.LINENUM 
								INNER JOIN ospp c ON b."ItemCode" = c."ItemCode" AND b."CardCode" = c."CardCode" 
								INNER JOIN ITM1 d ON c."ItemCode" = d."ItemCode" AND c."ListNum" = d."PriceList" 
							WHERE a."ItemCode" = :ItemCode AND a."CardCode" = :CardCode AND "SPP1LNum" = (SELECT LINENUM FROM #tblspp1) AND :Quantity >= "Amount" 
									AND a."UomEntry" = (SELECT "UomEntry" FROM OUOM WHERE "UomCode" = :uomCode) ORDER BY "Amount" DESC;

			----- nếu có dữ liệu setting theo số lượng thì lấy giá theo setting ------------------------------------------------------
			select ifnull((select count(1) from #tblspp2), 0) into checkCount from DUMMY;
			IF :checkCount > 0
			THEN		
				result = SELECT "Price", "Currency", "Discount", :CardCode "CardCode", :DocDate "DocDate", :ItemCode "ItemCode", :Quantity "Quantity", 'Y' "SpecPrice", :isGrossPrc "IsGrossPrc" FROM #tblspp2;
				return;
			ELSEIF :existsSetupUomPrice = 'N'
			THEN
				----------------------- lấy setup giá theo khối lượng của Price unit -----------------------------------------------
				INSERT INTO #tblspp2 ("Price", "Currency", "Discount") 
					SELECT TOP 1 case when a."SPP2LNum" = 0 then a."Price" / (1 - ifnull(a."Discount", 0) / 100) else d."Price" end, a."Currency", a."Discount"
					FROM SPP2 a INNER JOIN spp1 b ON a."ItemCode" = b."ItemCode" AND a."CardCode" = b."CardCode" AND a."SPP1LNum" = b.LINENUM 
												INNER JOIN ospp c ON b."ItemCode" = c."ItemCode" AND b."CardCode" = c."CardCode" 
												INNER JOIN ITM1 d ON c."ItemCode" = d."ItemCode" AND c."ListNum" = d."PriceList" 
					WHERE a."ItemCode" = :ItemCode AND a."CardCode" = :CardCode AND "SPP1LNum" = (SELECT LineNum FROM #tblspp1) AND :Quantity * :exchangeRate >= "Amount" ORDER BY "Amount" DESC;
						 
				result = SELECT "Price", "Currency", "Discount", :CardCode "CardCode", :DocDate "DocDate", :ItemCode "ItemCode", :Quantity "Quantity", 'Y' "SpecPrice", :isGrossPrc "IsGrossPrc" FROM #tblspp1;
				return;
			END IF;
			
		ELSEIF :existsSetupUomPrice = 'N'
		THEN
			result = SELECT "Price", "Currency", "Discount", :CardCode "CardCode", :DocDate "DocDate", :ItemCode "ItemCode", :Quantity "Quantity", 'Y' "SpecPrice", :isGrossPrc "IsGrossPrc" FROM #tblospp;
			RETURN;
		END IF;
	END	IF;
	------------------------------------------------------ KẾT THÚC TÍNH TOÁN THEO SPECIAL PRICE ------------------------------------------------------------
	---------------------------------------------------------------------------------------------------------------------------------------------------------

	----------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------ TÍNH TOÁN PRICE THEO UOM PRICE --------------------------------------------------------------------
	CardCode := tempCardCode;
	IF :infoPriceList=-1
	THEN
		------------------------------------- LAST PURCHASE PRICE không có setup Uom ----------------------------------------------------
		INSERT INTO #tbl ("Price", "Currency", "Discount", "CardCode", "DocDate", "ItemCode", "Quantity", "SpecPrice", "IsGrossPrc") 
			SELECT IFNULL("LastPurPrc", 0) * :exchangeRate AS "Price", IFNULL("LastPurCur", '') AS "Currency", 0 AS "Discount", :CardCode, :DocDate, :ItemCode, :Quantity, 'R', :isGrossPrc FROM OITM WHERE "ItemCode" = :ItemCode;
			
	ELSEIF :infoPriceList=-2
	THEN
		------------------------------------- LAST Evatu PRICE không có setup Uom ----------------------------------------------------
		INSERT INTO #tbl ("Price", "Currency", "Discount", "CardCode", "DocDate", "ItemCode", "Quantity", "SpecPrice", "IsGrossPrc") 
			SELECT IFNULL("LstEvlPric", 0) * :exchangeRate AS "Price", '' AS "Currency", 0 AS "Discount", :CardCode, :DocDate, :ItemCode, :Quantity, 'R', :isGrossPrc FROM OITM WHERE "ItemCode" = :ItemCode;

	ELSE
			----------------------------------- nếu không có dữ liệu setting peice cho Uom hiện tại thì lấy theo Price unit ------------------------------------
			if :existsSetupUomPrice = 'N'
			THEN
				select (select "UgpEntry" from OITM where "ItemCode" = :itemCode) into uomGroup FROM DUMMY;
				INSERT INTO #tblUomPrice 
					SELECT "Price" * :exchangeRate, "Currency" FROM ITM1 WHERE "PriceList" = :infoPriceList AND "ItemCode" = :itemCode;

			END IF;
			
			INSERT INTO #tbl ("Price", "Currency", "Discount", "CardCode", "DocDate", "ItemCode", "Quantity", "SpecPrice", "IsGrossPrc") 
							SELECT "Price", "Currency", 0, :CardCode, :DocDate, :ItemCode, :Quantity, 'R', :isGrossPrc FROM #tblUomPrice;
	END IF;
	----------------------------------------------- KẾT THÚC TÍNH TOÁN THEO UOM PRICE ---------------------------------------------------------------------------
	-------------------------------------------------------------------------------------------------------------------------------------------------------------

	result = select * from #tbl;
END