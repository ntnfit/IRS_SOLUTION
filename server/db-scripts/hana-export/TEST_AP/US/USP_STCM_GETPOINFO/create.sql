CREATE procedure USP_STCM_GetPoInfo(in docNo nvarchar(50))
as
begin

	declare tbl table ("CardCode" NVARCHAR(15), "CardName" NVARCHAR(100), "ItemCode" NVARCHAR(50), "ItemName" NVARCHAR(100));
	
	if not exists(select 1 from OPOR where cast("DocNum" as nvarchar)=:docNo and CANCELED='N')
		and not exists(select 1 from ORDR where cast("DocNum" as nvarchar)=:docNo and CANCELED='N') then
		SIGNAL SQL_ERROR_CODE 10002
		SET MESSAGE_TEXT = 'Số PO/SO không tồn tại.';
	end if;
	
	if exists(select 1 from OPOR where "DocNum"=:docNo and CANCELED='N') then
		insert into :tbl("CardCode", "CardName", "ItemCode", "ItemName")
		select a."CardCode", a."CardName", b."ItemCode", b."Dscription" "ItemName"
		from OPOR a join POR1 b on a."DocEntry"=b."DocEntry"
		where a."DocNum"=:docNo
		group by a."CardCode", a."CardName", b."ItemCode", b."Dscription";		
	else
		insert into :tbl("CardCode", "CardName", "ItemCode", "ItemName")
		select a."CardCode", a."CardName", b."ItemCode", b."Dscription" "ItemName"
		from ORDR a join RDR1 b on a."DocEntry"=b."DocEntry"
		where a."DocNum"=:docNo
		group by a."CardCode", a."CardName", b."ItemCode", b."Dscription";
	end if;	
	-- 11/06/2026: Them dong Item tong trong truong hop PO/SO co chi tiet tu 2 item tro len
	-- Itemcode la so PO/SO, ItemName se lay theo UDT "@API_ITEMDESC"
	if (select count(*) from :tbl) > 1 then
		insert into :tbl("CardCode", "CardName", "ItemCode", "ItemName")
		select top 1 t1."CardCode", t1."CardName",:docNo "ItemCode", case when t2."Code" is null then 'None' else t2."Name" end "ItemName"
		from :tbl t1
		left join "@API_ITEMDESC" t2 on t1."ItemName" like '%' || t2."Code" || '%' ;		
	end if;
	
	select * from :tbl;
end
