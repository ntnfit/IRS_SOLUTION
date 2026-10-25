
CREATE PROCEDURE APZ_SalesOpportunity_SearchDoc(
	in FromDate timestamp default null,
	in ToDate timestamp default null,
	in NoTrans int default 0,
	in BusPartner NVARCHAR(20) default '',
	in Status NVARCHAR(20) default '',
	in Stage int default 0,
	in SaleEmp int default 0,
	in UserName NVARCHAR(50),
	in searchKey nvarchar(254) default '')
AS
BEGIN
	DECLARE OppByUser NVARCHAR(1);
	select IFNULL((SELECT TOP 1 "OppByUser" FROM IRS_CONFIG),'N') into OppByUser from DUMMY;
	
	call apz_func_OwnerOverShip(:UserName,'97',OwnerView);
	
	SELECT a.*,
		CASE "OpprType" WHEN 'R' THEN IFNULL((SELECT SUM("DocTotal") FROM OINV WHERE "CardCode" = a."CardCode"), 0) ELSE 0 END AS "TotalInvc" 
	FROM OOPR a
	WHERE 
		(
			IFNULL("Owner",-1)=-1 OR ("Owner" IN (select distinct x."OwnerId" from :OwnerView x))
		)
		AND
		(
			:NoTrans > 0 AND a."OpprId"=:NoTrans
			OR
			(
				:NoTrans <= 0
				AND (((:FromDate is not null) AND a."OpenDate">=:FromDate) OR :FromDate is null)
				AND (((:ToDate is not null) AND a."OpenDate"<=:ToDate) OR :ToDate is null)
				AND (((:BusPartner <> '') AND a."CardCode"=:BusPartner) OR :BusPartner ='')
				AND (((:Status <> '') AND a."Status"=:Status) OR :Status ='')
				AND (((:Stage <> 0) AND exists(select "Step_Id" from opr1 where "OpprId"=A."OpprId" and "Step_Id"=:Stage)) OR :Stage =0)
				AND (((:SaleEmp <> 0) AND a."SlpCode"=:SaleEmp) OR :SaleEmp = 0)
				AND (((:searchKey<>'') and (a."CardName" like N'%' || :searchKey || '%' or a."Name" like N'%'|| :searchKey || '%' or a."CardCode" like N'%' || :searchKey || '%')) or Ifnull(:searchKey,'') = '')
			)
		)
	
	ORDER BY "OpprId" DESC;
END;




