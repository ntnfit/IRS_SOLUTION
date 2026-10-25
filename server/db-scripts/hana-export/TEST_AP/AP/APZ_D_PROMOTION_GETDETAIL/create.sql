
CREATE PROCEDURE APZ_D_Promotion_GetDetail
(
	 IN code nvarchar(100) DEFAULT '',
	 IN groupCode nvarchar(100) DEFAULT '',
	 IN userCode nvarchar(50) DEFAULT''
)
AS
BEGIN
	declare userId  int = ifnull((select USERID from OUSR where USER_CODE = :userCode),0);
	declare empID int = ifnull((select top 1 a."empID" from OHEM a left join OUSR b on a."userId" = b.USERID where b.USER_CODE = :userCode),0);
	declare promotionCode nvarchar(100) = ifnull((SELECT TOP 1 "Code" FROM APZ_OPPR WHERE "DocEntry" = :code), '');
	
	--Lấy ra danh sách cửa hàng theo tuyến bán hàng
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	CALL APZ_D_FilterStores('','','S',:userCode, N'#tmpStore');
	
	-- lấy danh sách lợi ích
	SELECT a.*, b."Name" FROM APZ_PPR1 a LEFT OUTER JOIN APZ_OPBF b ON a."BenefitCode" = b."Code" 
	WHERE a."DocEntry" = :code AND (:groupCode = '' OR (:groupCode <> '' 
	AND a."PakageNo" = :groupCode)) AND IFNULL(a."BenefitCode", '') <> '';

	-- lấy danh sách điều kiện
	SELECT a.*, b."Name" FROM APZ_PPR2 a LEFT OUTER JOIN APZ_OPCN b ON a."ConditionCode" = b."Code" 
	WHERE a."DocEntry" = :code AND (:groupCode = '' OR a."PakageNo" = n'' || :groupCode) AND IFNULL(a."ConditionCode", '') <> '';
	
	-- lấy danh sách cửa hàng đăng ký
	SELECT a.*, b."CardName", b."Address" FROM APZ_D_PMO3 a LEFT OUTER JOIN OCRD b ON a."CardCode" = b."CardCode" 
	LEFT OUTER JOIN APZ_OPPR c ON a."Code" = c."Code" 
	WHERE a."Code" = :promotionCode AND a."StatusCode" = 'Y' 
	AND ((c."PakageSplit" = 'Y' AND (:groupCode = '' OR (:groupCode <> '' AND a."GroupCode" = n'' || :groupCode))) OR (c."PakageSplit" <> 'Y')) 
	AND (b."CardCode" IN (SELECT "CardCode" FROM #tmpStore));
	
END




