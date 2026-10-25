CREATE procedure ILD_DIY_SinhSoBatch_SSCC_ThanhPham_Rework
(
	in ItemCode nvarchar(50),
	in InDate date, -- Ngay dong gói : Posting date
	in SoluongTong decimal(21,6),
    in SoluongTrong1SSCC  decimal(21,6),
    in DocEntry_PO int,  -- Số Production Order để lấy Docnum và sinh số SSCC
	in Line nvarchar(2) -- nha máy số mấy: L1, L2

)
as
begin
 
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
---
declare tbl table
(	
	SoBatch nvarchar(50),
	SoSSCC nvarchar(50),
	SoNumbering_hientai int,
	SoDongSSCC smallint, -- SSCC
	SoluongTrong1SSCC float,
	SoLuongSSCC_DongCuoi float
);


select  ifnull("DocNum",0)
into DocNum_PO
from  OWOR  
where "DocEntry"  =  :docEntry_PO;
--- Sinh số lô hàng
/*
	---=== Thành phẩm ===---
	Year(1)+Julian Date(3)+ 4 last number of PO No (LSX) + Product type (1) + Line (1)
   
	Example: Batch code 20311234F1
	2 = Year 2022
	031 = Julian Date of manufacturing 31/01
	1234 = 4 last number of PO No.
	F = Freeze Coffee Power/ L = Liquid Coffee
	1 = line 1 (Current line)
	---=== Rework ===---
	Year(1)+Julian Date(3)+ RW (2) + Product type (1) + Line (1)
*/
--1 Lấy DocNum từ số PO DocEntry
select  ifnull("DocNum",0)
into DocNum_PO
from  OWOR  
where "DocEntry"  =  :docEntry_PO;

If(select count (*) from OITM where "ItemCode"  = :ItemCode and  "ItmsGrpCod"  in ( 120, 127))>0  then -- Thanh phẩm

	select  RIGHT(year(InDate),1) ||  REPLICATE(0, 3 - (LENGTH(DAYOFYEAR(InDate)))) || DAYOFYEAR(InDate)   
								||  REPLICATE(0, 4 - (LENGTH(MAX(Right (DocNum_PO,4))))) || max(Right (DocNum_PO,4))
								
								
								
								
								 -- || 'F1'
								 -- mục đich ra FFF nếu null --> để user check lại item master data có thiết lập giá trị này chưa
								 || (select ifnull("U_ProductType",'FFF') from OITM where "ItemCode"  =  ItemCode)  
								 ||  right(Line,1)	 
	into  str from dummy;

Else -- Rework, Khong can them chu RW: 01/12/2025
		select  RIGHT(year(InDate),1) ||  REPLICATE(0, 3 - (LENGTH(DAYOFYEAR(InDate)))) || DAYOFYEAR(InDate)  
									--|| 'RWF1' 
							||right(DocNum_PO,4) || (select ifnull("U_ProductType",'RWWW') from OITM where "ItemCode"  =  ItemCode)  
								||  right(Line,1)	
		into  str from dummy;

End if;

------ Sinh số Batch (Số lot)


insert into :tbl ( SoBatch, 
					SoDongSSCC, SoluongTrong1SSCC, SoLuongSSCC_DongCuoi)
select   	:str,
			case when  SUBSTR_BEFORE(SoluongTong/SoluongTrong1SSCC,'.') = '' then SoluongTong/SoluongTrong1SSCC

			else  SUBSTR_BEFORE(SoluongTong/SoluongTrong1SSCC,'.') + 1 end  as "SoDongSSCC", 
			:SoluongTrong1SSCC as "SoluongTrong1SSCC",
 
 			 mod (SoluongTong,SoluongTrong1SSCC)   as "SoLuongSSCC_DongCuoi"
 
from dummy;
--- Sinh số SSCC : NVL: Purchase Order (9 số) + Auto number (5 số)



--2 Lấy số SSCC cuối theo cấu trúc: Purchase Order (9 số) + Auto number (5 số)
select max("BatchNum")  as  "SSCC"
Into ssccNo
from OIBT
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

 
select * from  :tbl order by SoSSCC;
--drop table :tbl;
  
end;
