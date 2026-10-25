CREATE PROCEDURE ILD_GET_WEIGHTGOODS
(
	IN fdate date ,
	IN tdate date ,
	IN doctype nvarchar(20),
	IN posono nvarchar(254)
)
AS
BEGIN
	IF (:doctype =22 ) THEN
		select
			t1."DRIVERNAME",t1."WEIGHINGREMARK",t1."REGISTERREMARK",t1."CONTAINERREGISTEREDWEIGHT",t1."TAREWEIGHT",t1."GROSSWEIGHT",t1."DOCNO",t1."SEALNO",
			t1."SOPONO",t1."CONTAINERNO",t1."TRUCKREGISTEREDWEIGHT",t1."INWEIGHT",t1."OUTWEIGHT",t1."NETWEIGHT",t1."CARDCODE" as vendorcode,t1."CARDNAME" vendorname,t1."ALARMMESSAGE",
			t1."TRAILERPLATENO",t1."WEIGHINGTYPE",t1."TRUCKPLATENO",t1."CREATEDATE",t1."WEIGHERNAME",t1."TRAILERREGISTEREDWEIGHT",
			t1."INTIME",t1."OUTTIME",t1."CONTAINERTYPE",t1."DOCDATE"
			,t2."BAGQUANTITY",t2."DELIVERYEMPTYTRUCKWEIGHT",t2."DELIVERYGOODSWEIGHT",t2."DELIVERYTRUCKWEIGHT",t2."ITEMCODE",t2."PACKINGWEIGHT",t2."PALLETQUANTITY",t2."PALLETWEIGHT"
			,t2."ITEMTYPE" as itemname,t3."ItemDetail"
		from GOODSWEIGHT t1
		left join GOODSWEIGHT_DETAILS t2 on t2.docno = t1.docno
		left join (select max(a."DocNum") "DocNum",b."ItemCode",max(b."Dscription") as "ItemName",max(b."U_ItemDetail") as "ItemDetail"
					from opor a left join por1 b on a."DocEntry"=b."DocEntry" group by b."DocEntry",b."ItemCode" 
				) t3 on t3."ItemCode" = t2.itemcode and t3."DocNum" = t1.sopono
		where t1.sopotype = :doctype and  t1.docdate between fdate and tdate
			and ((ifnull(:posono, '') = '') or (TO_VARCHAR(t1.sopono) in (select Data from split(:posono,',')))) ;
			
	 ELSEIF (:doctype = 17) THEN
		select 
			t1."DRIVERNAME",t1."WEIGHINGREMARK",t1."REGISTERREMARK",t1."CONTAINERREGISTEREDWEIGHT",t1."TAREWEIGHT",t1."GROSSWEIGHT",t1."DOCNO",t1."SEALNO",
			t1."SOPONO",t1."CONTAINERNO",t1."TRUCKREGISTEREDWEIGHT",t1."INWEIGHT",t1."OUTWEIGHT",t1."NETWEIGHT",t1."CARDCODE" as Customercode,t1."CARDNAME" Customername,t1."ALARMMESSAGE",
			t1."TRAILERPLATENO",t1."WEIGHINGTYPE",t1."TRUCKPLATENO",t1."CREATEDATE",t1."WEIGHERNAME",t1."TRAILERREGISTEREDWEIGHT",
			t1."INTIME",t1."OUTTIME",t1."CONTAINERTYPE",t1."DOCDATE"
			,t2."BAGQUANTITY",t2."DELIVERYEMPTYTRUCKWEIGHT",t2."DELIVERYGOODSWEIGHT",t2."DELIVERYTRUCKWEIGHT",t2."ITEMCODE",t2."PACKINGWEIGHT",t2."PALLETQUANTITY",t2."PALLETWEIGHT"
			,t2."ITEMTYPE" as itemname,t3."ItemDetail"
		from GOODSWEIGHT t1
		left join GOODSWEIGHT_DETAILS t2 on t2.docno = t1.docno
		left join (select max(a."DocNum") "DocNum",b."ItemCode",max(b."Dscription") as "ItemName",max(b."U_ItemDetail") as "ItemDetail" 
					from ORDR a left join RDR1 b on a."DocEntry"=b."DocEntry" group by b."DocEntry",b."ItemCode" 
				) t3 on t3."ItemCode" = t2.itemcode and t3."DocNum" = t1.sopono
		where t1.sopotype = :doctype and  t1.docdate between fdate and tdate
			and ((ifnull(:posono, '') = '') or (TO_VARCHAR(t1.sopono) in (select Data from split(:posono,',')))) ;
	ELSE
		select N'Hiện tại chỉ hổ trợ PO/ SO' as "Message" from dummy;
	END IF;
/*
	IF (:doctype = 22) THEN
		select
			t1.docdate, t1.docno, t1.TRUCKPLATENO, t1.sopono AS PONO,t1.cardcode as VenderCode,t1.cardname as VendorName,
			t1.DRIVERNAME,t2.itemcode, t3."ItemName" as itemname,t1.NETWEIGHT,t1.REGISTERREMARK
		from GOODSWEIGHT t1
		left join GOODSWEIGHT_DETAILS t2 on t2.docno = t1.docno
		left join oitm t3 on t3."ItemCode" = t2.itemcode
		where t1.sopotype = :doctype and  t1.docdate between fdate and tdate
			and ((ifnull(:posono, '') = '') or (TO_VARCHAR(t1.sopono) in (select Data from split(:posono,',')))) ;
			
	 ELSEIF (:doctype = 17) THEN
		select
			t1.docdate, t1.docno, t1.TRUCKPLATENO, t1.sopono AS SONO,t1.cardcode AS CustomerCode,t1.cardname as CustomerName,
			t1.DRIVERNAME,t2.itemcode, t3."ItemName" as itemname,t1.NETWEIGHT,t1.REGISTERREMARK
		from GOODSWEIGHT t1
		left join GOODSWEIGHT_DETAILS t2 on t2.docno = t1.docno
		left join oitm t3 on t3."ItemCode" = t2.itemcode
		where t1.sopotype = :doctype and  t1.docdate between fdate and tdate
			and ((ifnull(:posono, '') = '') or (TO_VARCHAR(t1.sopono) in (select Data from split(:posono,',')))) ;
	ELSE
		select N'Hiện tại chỉ hổ trợ PO/ SO' as "Message" from dummy;
	END IF;

	Ngày	
	Số Phiếu	
	Số Xe	
	Số PO/SO/ Hợp Đồng	
	Mã NCC/ Khách Hàng
	Khách Hàng
	Tên Tài Xế
	Mã Hàng
	Loại Hàng Hóa	
	Net Weight (Goods Weight)	
	Ghi Chú
*/
END
