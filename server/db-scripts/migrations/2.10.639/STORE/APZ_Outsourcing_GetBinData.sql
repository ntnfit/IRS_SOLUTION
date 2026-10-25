
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetBinData');

GO

CREATE  PROCEDURE APZ_Outsourcing_GetBinData(
	in code VARCHAR(5000),
	in typeDoc Int,
	in itemCode VARCHAR(50)
	)
AS
BEGIN
	Declare ID Int;
	
	IF :typeDoc = 1
	THEN 
		Select "U_issueID" Into ID From "@TBL_OUTS_DOCNUM" Where "Code" = :code;
		
		x = SELECT a."DistNumber", "BinCode" AS "FromBinCode", "Instock" AS "AvailQty", IFNULL("Quantity",0) AS "Quantity", 0 AS "LineNum", "AbsEntryBatch" AS "BtSAbsEntry", c."U_ORIBatch",
			a."BinAbs" AS "FromBinAbs",a."MnfSerial", a."LotNumber", b."BinActivat" AS "BinActivat" 
		FROM APZ_OITW_LOCATION a 
			LEFT JOIN OWHS b ON a."WhsCode" = b."WhsCode"
			LEFT JOIN OBTN c ON a."ItemCode" = c."ItemCode" And a."DistNumber" = c."DistNumber"
		WHERE a."ItemCode" = :ItemCode;
		Create Local Temporary Table #Temp AS (SELECT  * FROM :x);	
		
		y = Select a."BatchNum" AS "DistNumber", b."BinCode" AS "FromBinCode", f."OnHandQty" AS "AvailQty", a."Quantity", "BaseLinNum" AS "LineNum", d."AbsEntry" AS "BtSAbsEntry", d."U_ORIBatch",
			b."AbsEntry" AS "FromBinAbs", d."MnfSerial", d."LotNumber", e."BinActivat" AS "BinActivat" 
		From  IBT1 a JOIN OBIN b On a."WhsCode" = b."WhsCode"
			JOIN OITW c On a."WhsCode" = c."WhsCode" And a."ItemCode" = c."ItemCode"
			LEFT JOIN OBTN d On a."ItemCode" = d."ItemCode" And a."BatchNum" = d."DistNumber"
			LEFT JOIN OBBQ f On a."ItemCode" = f."ItemCode" And f."SnBMDAbs" = d."AbsEntry"
			LEFT JOIN OWHS e On a."WhsCode" = e."WhsCode"
		Where a."BaseEntry" = :ID And a."ItemCode" = :itemCode;
		Create Local Temporary Table #TempE AS (SELECT  * FROM :y);
		
		If Exists(Select 1 From #Temp a Join #TempE b On a."DistNumber" = b."DistNumber" And  a."FromBinCode" = b."FromBinCode") Then
			Delete From #Temp a Where Exists(Select 1 From #TempE b Where a."DistNumber" = b."DistNumber" And  a."FromBinCode" = b."FromBinCode"); 
		End If;
		
		Select * From #Temp UNION Select * From #TempE Order By "DistNumber";
		Drop Table #Temp;
		Drop Table #TempE;
	ELSE 
		Select "U_receiptID" Into ID From "@TBL_OUTS_DOCNUM" Where "Code" = :code;
		
		Select a."BatchNum" AS "DistNumber", b."BinCode" AS "ToBinCode", c."OnHand" AS "AvailQty", a."Quantity", "BaseLinNum" AS "LineNum", d."AbsEntry" AS "BtSAbsEntry", d."U_ORIBatch",
			b."AbsEntry" AS "FromBinAbs", d."MnfSerial", d."LotNumber", e."BinActivat" AS "BinActivat" 
		From IBT1 a JOIN OBIN b On a."WhsCode" = b."WhsCode"
			JOIN OITW c On a."WhsCode" = c."WhsCode" And a."ItemCode" = c."ItemCode"
			LEFT JOIN OBTN d On a."ItemCode" = d."ItemCode" And a."BatchNum" = d."DistNumber"
			LEFT JOIN OWHS e On a."WhsCode" = e."WhsCode"
		Where a."BaseEntry" = :ID  And a."ItemCode" = :itemCode
		Order By "DistNumber";
	End If;
END	
