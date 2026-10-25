
CREATE PROCEDURE APZ_BOM_GetTreeData(in productCode VARCHAR(50))
AS
BEGIN

	declare maxId int = 0;
	declare i int = 1;
	declare fatherCode nvarchar(50) = '';

	---------------------------------------------------- Lấy dữ liệu phiếu gốc ---------------------------------------------------------------------------------------------------------
	create local temporary table #diagram_cte ("ItemCode" nvarchar(50), "FatherCode" nvarchar(50), ID int, "FatherId" int, "WhsCode" nvarchar(50), "Price" numeric(19,6), "Quantity" numeric(19,6));
	insert into #diagram_cte SELECT "Code", null, 1, null, "ToWH", null, "Qauntity" FROM OITT where "Code" = :productCode;
	---------------------------------------------------- Lấy dữ liệu maxid để đánh ID cho các phiếu tiếp theo trong quá trình đệ quy ----------------------------------------------------
	select ifnull((select max(ID) from #diagram_cte), 0) into maxId from DUMMY;
	--------------------------------------------------- đệ quy để lấy dữ liệu các phiếu liên quan --------------------------------------------------------------------------------------
	WHILE :i <= :maxId
	DO
		
		select ifnull((select "ItemCode" from #diagram_cte where Id = :i), '') into fatherCode from DUMMY;
		insert into #diagram_cte
			SELECT T0."Code",T0."Father", row_number() over (order by T0."Code") + :maxId, :i, "Warehouse", "Price", "Quantity" FROM itt1 T0 where t0."Father"= :fatherCode;
				 
		select ifnull((select max(ID) from #diagram_cte), 0) into maxId from DUMMY;
		i = :i + 1;
		
	END WHILE;	
		------------------------------------------------------ Kết thúc đệ quy ----------------------------------------------------------------------------------------------------------
		 
	select a."ItemCode", b."ItemName", a."Quantity", b."InvntryUom" ,b."ItmsGrpCod", a."Price", a.ID, a."FatherId", a."WhsCode", c."Qauntity" "FatherQty"
				from #diagram_cte a inner join OITM b ON a."ItemCode" = b."ItemCode"
					left join OITT c on a."FatherCode" = c."Code";

END



