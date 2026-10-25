
-- apz_itemmaster_RemoveItem 'zzzzzz'
-- apz_itemmaster_linked_trans 'zzzzzz'
CREATE PROCEDURE apz_itemmaster_RemoveItem
(ItemCode NVARCHAR(50))
AS
BEGIN
DECLARE objType nvarchar(20);


CREATE LOCAL TEMPORARY TABLE #tbl
(
		ObjType VARCHAR(20)
);

INSERT INTO #tbl ( ObjType ) CALL apz_itemmaster_linked_trans (:ItemCode);
								   

select IFNULL((SELECT TOP 1 IFNULL(ObjType,'') FROM #tbl),'') into objType from DUMMY;

IF :objType <> '' THEN IF :objType = '1250000025' THEN SELECT 'You cannot remove item linked to blanket aggrement' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '202' THEN SELECT 'You cannot remove item linked to production order' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '66' THEN SELECT 'You cannot remove item linked to bill of materials' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '8000006' THEN SELECT 'You cannot remove item linked to BOM versions' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '23' THEN SELECT 'You cannot remove item linked to sales quotation' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '540000006' THEN SELECT 'You cannot remove item linked to purchase request' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '112' THEN SELECT 'You cannot remove item linked to draft document' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '10000044' THEN SELECT 'You cannot remove item linked to batch number' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '10000045' THEN SELECT 'You cannot remove item linked to serial number' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '1470000065' THEN SELECT 'You cannot remove item linked to Inventory Counting' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '198' THEN SELECT 'You cannot remove item linked to Sales Forecast' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '8000005' THEN SELECT 'You cannot remove item linked to advance Sales Forecast' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = 'LINE' THEN SELECT 'You cannot remove item linked to Line' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = 'IQA' THEN SELECT 'You cannot remove item linked to Item Quota Arrangement' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE IF :objType = '73' THEN SELECT 'You cannot remove item linked to BP catalog' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
ELSE SELECT 'You cannot remove item; linked transactions exist' FROM DUMMY;
--[Note:Modifier] Use FROM DUMMY if there is no FROM clause in the SELECT statement
RETURN;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
END IF;
DELETE FROM OITM WHERE "ItemCode" = :ItemCode;
DELETE FROM ITM1 WHERE "ItemCode" = :ItemCode;
DELETE FROM OITW WHERE "ItemCode" = :ItemCode;
DELETE FROM ITM12 WHERE "ItemCode" = :ItemCode;
END;




