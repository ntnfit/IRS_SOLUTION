CREATE procedure VAS_F_GRC_04 (
	in ObjType	nvarchar(20),
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DeciSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "OIGN" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo 			nvarchar(50)
		, RptName 		nvarchar(200)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, CompTel		nvarchar(50)
		, CompFax		nvarchar(50)
		, CompTaxId		nvarchar(50)
		, CompEmail		nvarchar(100)
		, CompWebSite	nvarchar(300)
		
		, WhsCode 		nvarchar(20)
		, WhsName		nvarchar(200)
		
		, ObjType		nvarchar(20)
		, DocEntry		int
		, WOID			int
		
		, SoPhieu		nvarchar(50)
		, Ngay			timestamp
		, NgayHachToan	timestamp
		, TinhTrang		nvarchar(50)
		, ChatLuong		nvarchar(50)
		, GhiChu		nvarchar(300)
		
		, NguoiLap		nvarchar(100)
		, LoaiTien		nvarchar(10)
		, TyGia			numeric(21,6)
		, PTVanTai		nvarchar(50)
		, NguoiGiao		nvarchar(100)
		, SoHieu		nvarchar(50)
		, DonHang		nvarchar(50)
		, LoHang		nvarchar(50)
		, StyleNo		nvarchar(50)
		, KhachHang		nvarchar(300)
		, HinhThuc		nvarchar(100)
		, DVSX			nvarchar(300)
		
		, MaSP			nvarchar(50)
		, TenSP			nvarchar(200)
		, ChungLoai		nvarchar(100)
		, TenMau		nvarchar(100)
		, Co			nvarchar(10)
		, DVT			nvarchar(50)
		, SLYeuCau		numeric(21,6)
		, SLNhap 		numeric(21,6)
		
		, DeciSep		nvarchar(10)
		, ThousSep		nvarchar(10)
		, QtyDec		int
		, AmtDec		int
		
		, Note			nvarchar(100)		
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (ObjType,DocEntry,SoPhieu,Ngay,NgayHachToan,GhiChu,LoaiTien,TyGia,WOID,MaSP,TenSP,DVT,WhsCode,SLNhap,NguoiLap)	
	select a."ObjType",a."DocEntry",a."U_JVNumber",a."DocDate",a."DocDate",a."Comments",a."DocCur",a."DocRate",b."BaseEntry",b."ItemCode",b."Dscription",b."unitMsr",b."WhsCode",b."Quantity",a."UserSign" 
	from OIGN a, IGN1 b where a."DocEntry"=b."DocEntry" and a."DocEntry"=:DocID;
	
	update x set WhsName = y."WhsName" from #tbl x inner join OWHS y on y."WhsCode"=x.WhsCode;
	
	update x set NguoiLap = y."U_NAME" from #tbl x inner join OUSR y on y."USERID"=x.NguoiLap;

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DeciSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set 
		RptName = N'PHIẾU NHẬP KHO SẢN PHẨM',
		DeciSep	= '.',	--DeciSep,
		ThousSep= ',',	--ThousSep,
		QtyDec	= 2, 	--DecQty,
		AmtDec	= 0;		--DecPrice;

	if bplID=0 then
		update x set CompName 	= a."CompnyName", CompAdd = b."Street" from #tbl x inner join "OADM" a on 1=1 inner join "ADM1" b on 1=1;
	end if;
	if bplID<>0 then
		update y set CompName	= x."BPLFrName", CompAdd = x."Street", CompTel = x."ZipCode" from #tbl y inner join "OBPL" x on x."BPLId"=bplID;
	end if;	

	select 
		*
	from #tbl;

	drop table #tbl;

end

