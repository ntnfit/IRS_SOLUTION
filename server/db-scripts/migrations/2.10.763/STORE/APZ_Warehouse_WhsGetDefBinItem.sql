
call apz_existsproc('APZ_Warehouse_WhsGetDefBinItem');

GO

create procedure APZ_Warehouse_WhsGetDefBinItem
(
	in transType nvarchar(1),
	in whsCode nvarchar(8),
	in itemCode nvarchar(50),
	in quantity decimal(21,6)
)
as
begin	
	-- OUT
	if :transType = 'O' then
		declare autoIssMtd int = ifnull((select "AutoIssMtd" from OWHS where "WhsCode" = :whsCode), 0);
		
		-- Single Choice
		if :autoIssMtd = 0 then
			-- Nếu chỉ có 1 bin có tồn item
			if (select count("BinAbs") from OIBQ where "ItemCode"=:itemCode and "WhsCode"=:whsCode and "OnHandQty">0) = 1 then
				select b."BinCode", case when a."OnHandQty"<:quantity then a."OnHandQty" else :quantity end "Qty", a."OnHandQty" "OnHand", a."BinAbs" "AbsEntry"
				from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry" where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0;
			-- Nếu tổng tồn của item ở tất cả bin trong kho không lớn hơn số lượng yêu cầu thì lấy ra những bin đó	
			elseif (select sum("OnHandQty") from OIBQ where "ItemCode"=:itemCode and "WhsCode"=:whsCode group by "ItemCode", "WhsCode") <= :quantity then
				select b."BinCode", a."OnHandQty" "Qty", a."OnHandQty" "OnHand", a."BinAbs" "AbsEntry"
				from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry" where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0;
			end if;
		
		-- Bin Location Code Order
		elseif :autoIssMtd = 1 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by b."BinCode") from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0
				order by b."BinCode";
			while :quantity >=0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			select "AbsEntry", "BinCode", "Qty", "OnHand" from :lstBin where "Qty" > 0;
		
		-- Alternative Sort Code Order
		elseif :autoIssMtd = 2 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by b."AltSortCod") from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0
				order by b."AltSortCod";
			while :quantity >=0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			select "AbsEntry","BinCode","Qty","OnHand" from :lstBin where "Qty" > 0;
			
		-- Descending Quantity
		elseif :autoIssMtd = 3 then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by a."OnHandQty" desc) from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0
				order by a."OnHandQty" desc;
			while :quantity > 0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "Qty" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			select "AbsEntry","BinCode","Qty","OnHand" from :lstBin where "Qty" > 0;
		
		-- Ascending Quantity / Ascending Quantity – Single Bin Preferred
		elseif :autoIssMtd in (4, 7) then
			declare row_num int = 1;
			declare lstBin table ("AbsEntry" int, "BinCode" nvarchar(228), "OnHand" decimal(19,6), "Qty" decimal(19,6), row_num int);
			
			if :autoIssMtd = 7 then
				if exists(select 1 from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
									where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">=:quantity) then
					select top 1 a."BinAbs" "AbsEntry", b."BinCode", :quantity "Qty", a."OnHandQty" "OnHand" 
					from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
					where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">=:quantity
					order by a."OnHandQty";
					
					return;
				end if;
			end if;
			
			insert into :lstBin
				select a."BinAbs", b."BinCode", a."OnHandQty", 0, row_number() over(order by a."OnHandQty") from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
				where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty">0
				order by a."OnHandQty";
			while :quantity >=0 do
				update :lstBin set "Qty" = case when :quantity > "OnHand" then "OnHand" else :quantity end where row_num = :row_num;
				quantity = quantity - (select "OnHand" from :lstBin where row_num = :row_num);
				row_num := row_num + 1;
			end while;
			
			select "AbsEntry","BinCode","Qty","OnHand" from :lstBin where "Qty" > 0;
		
		-- FIFO và LIFO chưa xử lý được
		end if;
		
	-- IN
	elseif :transType = 'I' then
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
				select "AbsEntry", "BinCode", :quantity "Qty", cast(null as decimal(21,6)) "OnHand"
				from OBIN where "BinCode" = :dftBin;
			end if;
			
		-- Last Bin Location That Received Item
		elseif :autoRecvMd = 1 then
			select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", cast(null as decimal(21,6)) "OnHand"
			from OBTL a join OBIN b on a."BinAbs"=b."AbsEntry"
			join OILM c on a."MessageID"=c."MessageID"
			where c."ItemCode"=:itemCode and c."LocCode"=:whsCode and c."AccumType"=1 and c."ActionType"=1
			order by c."MessageID" desc;
		
		-- Item's Current Bin Locations
		elseif :autoRecvMd = 2 then
			select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", cast(null as decimal(21,6)) "OnHand"
			from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
			where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode and a."OnHandQty" > 0
			order by b."BinCode";
			
		-- Item's Current and Historical Bin Locations
		else
			select top 1 b."AbsEntry", b."BinCode", :quantity "Qty", cast(null as decimal(21,6)) "OnHand"
			from OIBQ a join OBIN b on a."BinAbs"=b."AbsEntry"
			where a."ItemCode"=:itemCode and a."WhsCode"=:whsCode
			order by b."BinCode";
		end if;
	end if;

end