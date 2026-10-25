
Drop function APZ_Function_WhsGetDefBinItem;

GO

CREATE function APZ_Function_WhsGetDefBinItem(in tranType nvarchar(1), in whsCode nvarchar(50), in itemCode nvarchar(50), in batchEntry int, in quantity numeric(19,6))
returns table ("AbsEntry" int, "BinCode" nvarchar(50), "Qty" numeric(19,6), "OnHand" numeric(19,6))
AS
BEGIN
	
	-- OUT
	if :tranType = 'O' then
		declare autoIssMtd int = ifnull((select "AutoIssMtd" from OWHS where "WhsCode" = :whsCode), 0);
		
		-- Single Choice
		if :autoIssMtd = 0 then
			-- Nếu chỉ có 1 bin có tồn item
			if (select count("BinAbs") from OBBQ where "ItemCode"=:itemCode and "WhsCode"=:whsCode and "SnBMDAbs"=:batchEntry and "OnHandQty">0) = 1 then
				return select a."BinAbs" "AbsEntry", b."BinCode", case when a."OnHandQty"<:quantity then a."OnHandQty" else :quantity end "Qty", a."OnHandQty" "OnHand"
				from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry" where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0;
			-- Nếu tổng tồn của item ở tất cả bin trong kho không lớn hơn số lượng yêu cầu thì lấy ra những bin đó	
			elseif (select sum("OnHandQty") from OBBQ where "ItemCode"=:itemCode and "WhsCode"=:whsCode and "SnBMDAbs"=:batchEntry group by "ItemCode","WhsCode","SnBMDAbs") <= :quantity then
				return select a."BinAbs" "AbsEntry", b."BinCode", a."OnHandQty" "Qty", a."OnHandQty" "OnHand"
				from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0;
			end if;
		
		-- Bin Location Code Order
		elseif :autoIssMtd = 1 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by b."BinCode") from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0
				order by b."BinCode";
			while :quantity > 0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "Qty" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			return select "AbsEntry", "BinCode", "Qty", "OnHand" from :lstBin where "Qty" > 0;
		
		-- Alternative Sort Code Order
		elseif :autoIssMtd = 2 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by b."AltSortCod") from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0
				order by b."AltSortCod";
			while :quantity >=0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			return select "AbsEntry", "BinCode", "Qty", "OnHand" from :lstBin where "Qty" > 0;
			
		-- Descending Quantity
		elseif :autoIssMtd = 3 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by a."OnHandQty" desc) from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0
				order by a."OnHandQty" desc;
			while :quantity > 0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			return select "AbsEntry", "BinCode", "Qty", "OnHand" from :lstBin where "Qty" > 0;
		
		-- Ascending Quantity / Ascending Quantity – Single Bin Preferred
		elseif :autoIssMtd in (4, 7) then
			declare row_num int = 1;
			
			if :autoIssMtd = 7 then
				single = select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", a."OnHandQty" "OnHand" from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
									where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">=:quantity order by a."OnHandQty";
				if exists(select 1 from :single) then
					return select * from :single;
				end if;
			end if;
			
			begin
				declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
				insert into :lstBin
					select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by a."OnHandQty" desc) from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
					where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty">0
					order by a."OnHandQty";
				while :quantity >=0 do
					update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
					quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
					row_num := row_num + 1;
				end while;
				
				return select "AbsEntry", "BinCode", "Qty", "OnHand" from :lstBin where "Qty" > 0;
			end;
			
		-- FIFO và LIFO chưa xử lý được
		end if;
		
	-- IN
	elseif :tranType = 'I' then
		declare autoRecvMd int = ifnull((select "AutoRecvMd" from OWHS where "WhsCode"=:whsCode), 0);
		-- Enforce Default Bin Loc or Default Bin Location
		if exists(select 1 from OWHS where "WhsCode"=:whsCode and "DftBinEnfd" = 'Y') or :autoRecvMd = 0 then
			-- Lấy default bin theo thứ tự ưu tiên: Item > Item Group > Warehouse
			declare dftBin nvarchar(228) = ifnull((select b."BinCode" from OITW a join OBIN b on a."DftBinAbs"=b."AbsEntry" where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode), '');
			if :dftBin = '' then
				dftBin := ifnull((select d."BinCode" from OITM a join OITB b on a."ItmsGrpCod"=b."ItmsGrpCod" join OIGW c on b."ItmsGrpCod"=c."ItmsGrpCod"
									join OBIN d on c."DftBinAbs"=d."AbsEntry"
									where a."ItemCode"=:itemCode and c."WhsCode"=:whsCode), '');
				if :dftBin = '' then
					dftBin := ifnull((select b."BinCode" from OWHS a join OBIN b on a."DftBinAbs"=b."AbsEntry" where a."WhsCode"=:whsCode), '');
				end if;
			end if;
			
			if :dftBin <> '' then
				return select "AbsEntry", "BinCode", :quantity "Qty", null "OnHand" from OBIN where "BinCode" = :dftBin;
			end if;
			
		-- Last Bin Location That Received Item
		elseif :autoRecvMd = 1 then
			return select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", null "OnHand" from OBTL a join OBIN b on a."BinAbs"=b."AbsEntry"
			join OILM c on a."MessageID"=c."MessageID"
			where c."ItemCode"=:itemCode and c."LocCode"=:whsCode and c."AccumType"=1 and c."ActionType"=1
			order by c."MessageID" desc;
		
		-- Item's Current Bin Locations
		elseif :autoRecvMd = 2 then
			return select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", null "OnHand" from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
			where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry and a."OnHandQty" > 0
			order by b."BinCode";
			
		-- Item's Current and Historical Bin Locations
		else
			return select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", null "OnHand" from OBBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
			where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."SnBMDAbs"=:batchEntry
			order by b."BinCode";
		end if;
	end if;
	
END