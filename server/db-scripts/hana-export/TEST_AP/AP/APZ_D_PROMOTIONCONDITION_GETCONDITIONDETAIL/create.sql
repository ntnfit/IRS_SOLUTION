
CREATE PROCEDURE Apz_D_PromotionCondition_GetConditionDetail (IN code nvarchar(50), IN typeName nvarchar(50))
AS
BEGIN 
	
	IF :typeName = '1' 
		THEN CALL Apz_D_PromotionCondition_GetConditionCustomer(:code);
	ELSEIF :typeName = '2' 
		THEN CALL Apz_D_PromotionCondition_GetConditionCustomerGroup(:code);
	ELSEIF :typeName = '3' 
		THEN CALL Apz_D_PromotionCondition_GetConditionItem(:code);
	ELSEIF :typeName = '4' 
		THEN CALL Apz_D_PromotionCondition_GetConditionCombo(:code);
	ELSEIF :typeName = '5' 
		THEN CALL Apz_D_PromotionCondition_GetConditionGroupAttribute(:code);
	ELSEIF :typeName = '6' 
		THEN CALL Apz_D_PromotionCondition_GetConditionDocument(:code);
	ELSEIF :typeName = '7' 
		THEN CALL Apz_D_PromotionCondition_GetConditionAccrued(:code);
	ELSEIF :typeName = '8' 
		THEN CALL Apz_D_PromotionCondition_GetConditionAccrued(:code);
	ELSEIF :typeName = '9' 
		THEN CALL Apz_D_PromotionCondition_GetConditionTerritory(:code);
	ELSEIF :typeName = '10' 
		THEN CALL Apz_D_PromotionCondition_GetConditionChannel(:code);
	END IF;
	
	execute immediate 'SELECT * FROM #tblStructure ORDER BY ID;';
	
	execute immediate 'select * from #TempData;';
	
	execute immediate 'SELECT * FROM #tblValidValue;';
END;




