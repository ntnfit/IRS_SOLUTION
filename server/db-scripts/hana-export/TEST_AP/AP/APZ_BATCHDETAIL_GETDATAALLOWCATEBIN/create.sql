
CREATE PROCEDURE APZ_BatchDetail_GetDataAllowcateBin(in itemCode nvarchar(50), in whsCode nvarchar(8), in qty numeric(19,6))
AS
BEGIN

	create local temporary table #temBinData ("BinAbs" int, "OnHandQty" numeric(19,6));
	---------------------- lấy dữ liệu tồn kho theo bin của mặt hàng theo kho
	insert into #temBinData select "BinAbs", "OnHandQty" from OIBQ where "ItemCode" = :itemCode and "WhsCode" = :whsCode and "OnHandQty" > 0;

	--------------- nếu số lượng bin có tồn kho lớn hơn 1 thì chỉ lấy data khi số lượng tồn kho nhỏ hơn hoặc bằng số lương allocate
	if ifnull((select count(1) from #temBinData),0) > 1
	THEN

		if (select sum("OnHandQty") from #temBinData) <= :qty
		THEN
			select a."BinAbs", b."BinCode", a."OnHandQty" "Qty" from #temBinData a inner join OBIN b ON a."BinAbs" = b."AbsEntry";
		else 
			select top 0 0 "BinAbs", '' "BinCode", 0 "Qty" from DUMMY;
		END IF;
		return;

	END IF;

	---------------- nếu chỉ có một bin có tồn thì trả về bin đó
	select a."BinAbs", b."BinCode", case when a."OnHandQty" > :qty then :qty else a."OnHandQty" end "Qty" from #temBinData a inner join OBIN b on a."BinAbs" = b."AbsEntry";

END




