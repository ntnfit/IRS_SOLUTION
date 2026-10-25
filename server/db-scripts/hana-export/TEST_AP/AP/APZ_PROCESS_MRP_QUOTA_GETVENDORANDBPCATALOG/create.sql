
CREATE procedure apz_process_MRP_QUOTA_GetVendorAndBPCatalog(in itemCode nvarchar(50) default '', in periodID int default 0)
as
BEGIN
	declare checkCount int = 0;
	select ifnull((select top 1 1 from APZ_IQA1 where "PeriodID"=:periodID AND "ItemCode"=:itemCode),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		SELECT :periodID AS "PeriodID", T0."CardCode", T1."CardName", 0.0 AS "LeadTime", 0.0 AS "Per", 0.0 AS "MaxQty", 0.0 AS "MaxLot", 0.0 AS "BaseQty", 0.0 AS "MinLot", '' AS "Remarks", 0.0 AS "OrdrIntrvl"
				, 0.0 AS "OrdrMulti", T0."Substitute" FROM OSCN T0 INNER JOIN OCRD T1 ON T0."CardCode" = T1."CardCode" WHERE T0."ItemCode" = :itemCode;
	END IF;
END;



