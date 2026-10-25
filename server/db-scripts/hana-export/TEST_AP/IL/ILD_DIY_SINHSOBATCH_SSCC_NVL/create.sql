CREATE procedure ILD_DIY_SinhSoBatch_SSCC_NVL
(
	in CardCode nvarchar(50),
	in ItemCode nvarchar(50),
	in InDate date, -- Ngay nhập kho
	in MnfDate  date,--  '2023-06-01'; --  Ngày sản xuất;
	in SoluongTong decimal(21,6),
    in SoluongTrong1SSCC  decimal(21,6),
    in DocEntry_PO int  -- Số Purchase Order để lấy Docnum và sinh số SSCC

)
as
begin
/*
	The batch number will have 9  digits which following rules:
	Example: Batch code GC2203101
	GC = Green Coffee Bean --> part1
	22 = Year 2022 --> part2
	031 = Julian Date of manufacturing 31/01 --> part3
	01 = running number, the first batch of date --> part4
-----
*/
/*
-- Tham so tư giao dien
declare cardCode nvarchar(50) = '200001999'; -- CardCode
declare itemCode nvarchar(50) = '1520100002';
declare inDate date = '2023-06-02'; -- Ngày nhập kho

declare SoluongTong decimal(21,6) = 1000; 
declare SoluongTrong1SSCC  decimal(21,6) = 400; 
declare MnfDate  date  = '2023-06-01'; --  Ngày sản xuất;
*/
----
declare batchNo nvarchar(20);
declare ssccNo nvarchar(20);
declare str nvarchar(50);
declare runNumer smallint = 0;

--- Return 
declare batchAttribute1  nvarchar(50); --  MnfSerial: Dung lam so Lot
declare distNumber  nvarchar(50); --  Batch Number: Dung lam SSCC
-- SSCC
declare DocEntry_GRPO int=0;
declare BaseLine_GRPO int=0;
declare DocNum_PO nvarchar(20);
declare tbl table
(	
	SoBatch nvarchar(50),
	SoSSCC nvarchar(50),
	SoNumbering_hientai int,
	SoDongSSCC smallint, -- SSCC
	SoluongTrong1SSCC smallint,
	SoLuongSSCC_DongCuoi smallint,
	ExpirDate date
);

--- Sinh số lô hàng
select 'GC' || RIGHT(year(CURRENT_DATE),2) ||  REPLICATE(0, 3 - (LENGTH(DAYOFYEAR(MnfDate)))) || DAYOFYEAR(MnfDate)   as "Julian Date" 
into  str from dummy;

---
select max("SuppSerial")  as  "BatchNo"
Into batchNo
from OIBT
where "ItemCode"  = :itemCode and "InDate" = :inDate  and "CardCode"  = :cardCode;
--

If (ifnull(batchNo,'') <> '') then --> Đã tồn tại
	batchAttribute1 = batchNo;
	
else -->  Chưa tồn tại 1 số batch nào vơi tham số truyền vào ( carcode, ngày nhập kho, itemcode)
	 
	runNumer =  ifnull(right  (:batchNo,2),0);
	
End If;
----

select  str || REPLICATE(0, 2 - (LENGTH(runNumer+1)))||  cast(runNumer+1  as nvarchar(2))
into  batchAttribute1 from  dummy;
------ Sinh số Batch (Số lot)
insert into :tbl ( SoBatch, 
					SoDongSSCC, SoluongTrong1SSCC, SoLuongSSCC_DongCuoi)
select   	:batchAttribute1,
			case when  SUBSTR_BEFORE(SoluongTong/SoluongTrong1SSCC,'.') = '' then SoluongTong/SoluongTrong1SSCC

			else  SUBSTR_BEFORE(SoluongTong/SoluongTrong1SSCC,'.') + 1 end  as "SoDongSSCC", 
			:SoluongTrong1SSCC as "SoluongTrong1SSCC",
 
 			 mod (SoluongTong,SoluongTrong1SSCC)   as "SoLuongSSCC_DongCuoi"
 
from dummy;
--- Sinh số SSCC : NVL: Purchase Order (9 số) + Auto number (5 số)

--1 Lấy DocNum từ số PO DocEntry
select  ifnull("DocNum",0)
into DocNum_PO
from  OPOR  
where "DocEntry"  =  :docEntry_PO;

--2 Lấy số SSCC cuối theo cấu trúc: Purchase Order (9 số) + Auto number (5 số)
select max("BatchNum")  as  "SSCC"
Into ssccNo
from IBT1
where left("BatchNum",9)  = REPLICATE(0, 9 - (LENGTH(DocNum_PO)))||DocNum_PO;
--cast(DocNum_PO as nvarchar(20));

--3 Kiểm tra và tăng số runner: -->  Đã tồn tại 1 số SSCC nào vơi tham số truyền vào ( Purchase Order (9 số) + Auto number (5 số))
If (ifnull(ssccNo,'') <> '') then
	runNumer =  ifnull(right  (:ssccNo,5),0);
	
	
End If;

select REPLICATE(0, 9 - (LENGTH(DocNum_PO)))||DocNum_PO
--|| REPLICATE(0, 5 - (LENGTH(runNumer+1)))||  cast(runNumer+1  as nvarchar(2))
into  distNumber from  dummy;

update a
set soSSCC = distNumber, 
SoNumbering_hientai  = runNumer +1
from :tbl a;
---
update a
set ExpirDate   = (select ADD_MONTHS(:MnfDate, ifnull("U_Life",0)) from OITM where "ItemCode" = :ItemCode)
from :tbl a;
 
 
select * from  :tbl order by SoSSCC;
  
end;

