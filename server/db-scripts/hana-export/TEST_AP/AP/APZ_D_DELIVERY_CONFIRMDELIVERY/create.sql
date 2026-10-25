

CREATE PROCEDURE Apz_D_Delivery_ConfirmDelivery (IN type NVARCHAR(50) DEFAULT 'A')
AS
BEGIN	
	
	CREATE LOCAL TEMPORARY TABLE #ConfirmLocal as (select top 0 * from APZ_D_ODLO);
	exec 'drop table #ConfirmLocal';
	exec 'create local temporary table #ConfirmLocal as (select "StartTime", "ConfirmTime", "Feedback", "ActualAmount", "DeliveredFull", "Latitude", "Longitude" from "#Temp_Confirm")';
	
	UPDATE B SET "StartTime" = a."StartTime", "ConfirmTime" = a."ConfirmTime", "Feedback" = a."Feedback", "ActualAmount" = a."ActualAmount", 
	"DeliveredFull" = a."DeliveredFull", "Latitude" = a."Latitude", "Longtitude" = a."Longtitude", "Status" = 'Done' 
	FROM #ConfirmLocal a, APZ_D_ODLO b WHERE a."DocEntry" = b."DocEntry" AND a."CardCode" = b."CardCode";
	
	SELECT 0, '' FROM DUMMY;
	
END



