
CREATE PROCEDURE Apz_Commons_CheckExistsObjectByCode (
	IN 	type INT DEFAULT -1
,		code NVARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	DECLARE DsplITMDoc CHAR(1);
	DECLARE fromDate TIMESTAMP;
	DECLARE toDate TIMESTAMP;
	SELECT TOP 1 "DspIITMDoc" INTO DsplITMDoc FROM ADM1;
	SELECT ADD_DAYS(NOW(), -1) INTO fromDate FROM DUMMY;
	SELECT ADD_DAYS(NOW(), 1) INTO toDate FROM DUMMY;
	
	--List Item
	IF :type = 1 THEN 
		SELECT 1 
			FROM OITM 
			WHERE "ItemCode" = :code 
				AND ((:DsplITMDoc = 'Y' 
					OR (:DsplITMDoc = 'N' 
						AND (("validFor" = 'Y' AND IFNULL("frozenFor", 'N') = 'N' AND (IFNULL("validFrom", :fromDate) <= NOW() AND IFNULL("validTo", :toDate) >= NOW())) 
						OR (IFNULL("validFor", 'N') = 'N' AND "frozenFor" = 'Y' AND (IFNULL("frozenFrom", :fromDate) > NOW() OR IFNULL("frozenTo", :toDate) < NOW())) 
						OR ("validFor" = 'Y' 
						AND "frozenFor" = 'Y' 
						AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
						AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
					OR (IFNULL("validFor", 'N') = 'N' AND IFNULL("frozenFor", 'N') = 'N')))));
	--inventory item
	ELSEIF :type = 10000078 THEN 
		SELECT 1 FROM OITM WHERE "ItemCode" = :code AND "InvntItem" = 'Y' AND ((:DsplITMDoc = 'Y' OR (:DsplITMDoc = 'N' AND (("validFor" = 'Y' AND IFNULL("frozenFor", 'N') = 'N' AND (IFNULL("validFrom", :fromDate) <= NOW() AND IFNULL("validTo", :toDate) >= NOW())) OR (IFNULL("validFor", 'N') = 'N' AND "frozenFor" = 'Y' AND (IFNULL("frozenFrom", :fromDate) > NOW() OR IFNULL("frozenTo", :toDate) < NOW())) OR ("validFor" = 'Y' AND "frozenFor" = 'Y' AND (IFNULL("validFrom", :fromDate) <= NOW() AND IFNULL("validTo", :toDate) >= NOW()) AND (IFNULL("frozenFrom", :fromDate) > NOW() OR IFNULL("frozenTo", :toDate) < NOW())) OR (IFNULL("validFor", 'N') = 'N' AND IFNULL("frozenFor", 'N') = 'N')))));
	--Sell Item
	ELSEIF :type = 10000076 THEN 
		SELECT 1 
			FROM OITM 
			WHERE "ItemCode" = :code 
			AND "SellItem" = 'Y' 
			AND ((:DsplITMDoc = 'Y' 
					OR (:DsplITMDoc = 'N' 
						AND (("validFor" = 'Y' 
								AND IFNULL("frozenFor", 'N') = 'N' 
								AND (IFNULL("validFrom", :fromDate) <= NOW() 
									AND IFNULL("validTo", :toDate) >= NOW())) 
							OR (IFNULL("validFor", 'N') = 'N' 
								AND "frozenFor" = 'Y' 
								AND (IFNULL("frozenFrom", :fromDate) > NOW() 
									OR IFNULL("frozenTo", :toDate) < NOW())) 
							OR ("validFor" = 'Y' 
								AND "frozenFor" = 'Y' 
								AND (IFNULL("validFrom", :fromDate) <= NOW() 
									AND IFNULL("validTo", :toDate) >= NOW()) 
								AND (IFNULL("frozenFrom", :fromDate) > NOW() 
									OR IFNULL("frozenTo", :toDate) < NOW())) 
							OR (IFNULL("validFor", 'N') = 'N' 
								AND IFNULL("frozenFor", 'N') = 'N')))));
	--Purchase item
	ELSEIF :type = 10000077 THEN 
		SELECT 1 
			FROM OITM 
			WHERE "ItemCode" = :code 
			AND "PrchseItem" = 'Y' 
			AND ((:DsplITMDoc = 'Y' 
					OR (:DsplITMDoc = 'N' 
						AND (("validFor" = 'Y' 
								AND IFNULL("frozenFor", 'N') = 'N' 
								AND (IFNULL("validFrom", :fromDate) <= NOW() 
									AND IFNULL("validTo", :toDate) >= NOW())) 
							OR (IFNULL("validFor", 'N') = 'N' 
								AND "frozenFor" = 'Y' 
								AND (IFNULL("frozenFrom", :fromDate) > NOW() 
									OR IFNULL("frozenTo", :toDate) < NOW())) 
							OR ("validFor" = 'Y' 
								AND "frozenFor" = 'Y' 
								AND (IFNULL("validFrom", :fromDate) <= NOW() 
									AND IFNULL("validTo", :toDate) >= NOW()) 
								AND (IFNULL("frozenFrom", :fromDate) > NOW() 
									OR IFNULL("frozenTo", :toDate) < NOW())) 
							OR (IFNULL("validFor", 'N') = 'N' 
								AND IFNULL("frozenFor", 'N') = 'N')))));
	--List Bussiness
	ELSEIF :type = 2 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'));
	--Vendor
	ELSEIF :type = 3 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND "CardType" = 'C' 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'));
	--Vendor
	ELSEIF :type = 4 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND "CardType" = 'S' 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'));
	--Lead
	ELSEIF :type = 5 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND "CardType" = 'L' 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'))
			;
	--Customer vendor
	ELSEIF :type = 2000034 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND "CardType" IN ('C',	'S') 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'));
	--Customer Lead	
	ELSEIF :type = 2000035 THEN 
		SELECT 1 
			FROM OCRD 
			WHERE "CardCode" = :code 
			AND "CardType" IN ('C',	'L') 
			AND (("validFor" = 'Y' 
					AND IFNULL("frozenFor", 'N') = 'N' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR ("validFor" = 'Y' 
					AND "frozenFor" = 'Y' 
					AND (IFNULL("validFrom", :fromDate) <= NOW() 
						AND IFNULL("validTo", :toDate) >= NOW()) 
					AND (IFNULL("frozenFrom", :fromDate) > NOW() 
						OR IFNULL("frozenTo", :toDate) < NOW())) 
				OR (IFNULL("validFor", 'N') = 'N' 
					AND IFNULL("frozenFor", 'N') = 'N'));
	--Warehouse
	ELSEIF :type = 18 THEN 
		SELECT 1 
			FROM OWHS 
			WHERE "WhsCode" = :code 
			AND IFNULL("Locked", 'N') = 'N';
	--Project
	ELSEIF :type = 50 THEN 
		SELECT 1 FROM OPRJ WHERE "PrjCode" = :code AND IFNULL("Locked", 'N') = 'N';
	--Acount code
	ELSEIF :type = 20 THEN 
		SELECT 1 FROM OACT WHERE "Postable" = 'Y' AND "AcctCode" = :code;
	END IF;
END;




