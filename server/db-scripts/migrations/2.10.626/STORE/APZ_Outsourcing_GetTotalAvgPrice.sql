
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetTotalAvgPrice');

GO

CREATE PROCEDURE APZ_Outsourcing_GetTotalAvgPrice
(	
	in whs nvarchar(5000) default '',
	in item nvarchar(5000) default '',
	in qty nvarchar(5000) default ''	
)
AS
BEGIN

	CREATE LOCAL TEMPORARY TABLE #tWhs ("Id" Int, Data Nvarchar(5000));
	INSERT INTO  #tWhs SELECT ROW_NUMBER() OVER (PARTITION BY Data), Data FROM (SELECT Data FROM Split(:whs,','));
	
	CREATE LOCAL TEMPORARY TABLE #tItem ("Id" Int, Data Nvarchar(5000));
	INSERT INTO  #tItem SELECT ROW_NUMBER() OVER (PARTITION BY Data), Data FROM (SELECT Data FROM Split(:item,','));
	
	CREATE LOCAL TEMPORARY TABLE #tQty ("Id" Int, Data Nvarchar(5000));
	INSERT INTO  #tQty SELECT ROW_NUMBER() OVER (PARTITION BY Data), Data FROM (SELECT Data FROM Split(:qty,','));
	
	CREATE LOCAL TEMPORARY TABLE #tmp_Outsourcing_AvgPrice ("WhsCode" Nvarchar(5000),"ItemCode" Nvarchar(5000),"Quantity" Nvarchar(5000));
	INSERT INTO #tmp_Outsourcing_AvgPrice 
	SELECT a.Data "WhsCode",
		b.Data "ItemCode",
		c.Data "Quantity"
	FROM #tWhs a 
		INNER JOIN #tItem b on a."Id" = b."Id"
		INNER JOIN #tQty c on a."Id" = c."Id";
	
	SELECT SUM(a."Total") "Total" 
	FROM (SELECT (b."AvgPrice" * a."Quantity") AS "Total"
		FROM #tmp_Outsourcing_AvgPrice a 
		INNER JOIN OITW b on a."ItemCode"=b."ItemCode" and a."WhsCode"=b."WhsCode") a;
END