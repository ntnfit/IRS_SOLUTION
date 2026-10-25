CREATE function UFC_GetNewFormat
(
	in pObType nvarchar(30),
	in pDocEntry nvarchar(16)
)
RETURNS TABLE
(
	"JrnlMemo" nvarchar(254)
)
as
begin
	declare pBaseType int;
	declare pBaseRef nvarchar(254);
	declare pLineNum int ;
	DECLARE pCharNum int = 5; -- TRU 4 KY TU DAU LA THANG, NAM, LAY TU KYTU THU 5
	declare pFormat varchar(20)='';
	declare pDienGiai nvarchar(254);--Chua gioi han 150 ky tu
	declare pJrnlMemo nvarchar(254) = '';
	declare pJrnl nvarchar(254) = '';
	declare pAPInv nvarchar(254);
	declare pInvCount int;
	declare pCancel varchar(1);
	declare pSoKyTu int;
	
	if (:pObType = 18) then 
		pFormat = 'PO ';
		select  "U_APInv",CANCELED into pAPInv,pCancel default '','' from opch where CANCELED = 'N' and "DocEntry" = pDocEntry;
		if(pCancel = 'N') then
			select min("LineNum") into pLineNum from pch1 where "DocEntry" = pDocEntry;
			select "BaseType",case when ifnull("U_ItemDetail",'') = '' then "Dscription" else "U_ItemDetail" end into pBaseType,pDienGiai
			from pch1 where "LineNum" = pLineNum and "DocEntry" = pDocEntry limit 1;
			
			if (pBaseType = 20) then -- Thong qua GRPO de lay PO
				select pFormat|| string_agg(TO_INT(SUBSTRING(T1."BaseRef",pCharNum)),'-') as "JrnlMemo"
				into pJrnl
				from (SELECT distinct "BaseRef" FROM pdn1 where "DocEntry" in (select distinct "BaseEntry"
					from pch1 where "DocEntry" = :pDocEntry)) T1;
					
			elseif (pBaseType = 22) then --truc tiep tu PO
				select pFormat|| string_agg(TO_INT(SUBSTRING("BaseRef",pCharNum)),'-') as "JrnlMemo"
				into pJrnl
				from (SELECT distinct "BaseRef" FROM pch1 where "DocEntry" = :pDocEntry) T1;
			else
				pJrnlMemo := '';
			end if;	
			pSoKyTu = length(pJrnl|| '-'|| pDienGiai || '-Inv '||ifnull(:pAPInv,'')) - 241 ;
			if (pSoKyTu > 0) then -- Chieu dai chuoi Format dang > 241. Phai cat bo bot phan dien giai.Vì phải chừa ký tự khi làm Outgoing kế thừa remark sang có cộng thêm ký tự
				pJrnlMemo := pJrnl|| '-'|| left(pDienGiai,length(pDienGiai)- pSoKyTu) || '-Inv '||ifnull(:pAPInv,'');
			else
				pJrnlMemo := pJrnl|| '-'|| pDienGiai || '-Inv '||ifnull(:pAPInv,'');
			end if;
								
		end if;
		
	end if;
	if (:pObType = '204') then 
		pFormat = 'Advance-PO ';

		select IFNULL("U_APInv",''),CANCELED into pAPInv,pCancel default '','' from ODPO where CANCELED = 'N' and "DocEntry" = pDocEntry;
		if(pCancel = 'N') then
			--select min("LineNum") into pLineNum from DPO1 where "DocEntry" = pDocEntry;
			select "BaseType" into pBaseType
			from DPO1 where "DocEntry" = pDocEntry limit 1;
			
			if (pBaseType = 20) then -- Thong qua GRPO de lay PO
				select pFormat || string_agg(TO_INT(SUBSTRING(IFNULL(T1."BaseRef",''),pCharNum)),'-') as "JrnlMemo"
				into pJrnlMemo
				from (SELECT distinct "BaseRef" FROM PDN1 where "DocEntry" in (select distinct "BaseEntry"
					from DPO1 where "DocEntry" = :pDocEntry)) T1;				
					
			elseif (pBaseType = 22) then --truc tiep tu PO
				select pFormat|| string_agg(TO_INT(SUBSTRING(T1."BaseRef",pCharNum)),'-') as "JrnlMemo"
				into pJrnlMemo
				from (SELECT distinct "BaseRef" FROM DPO1 where "DocEntry" = :pDocEntry) T1;
			else
				pJrnlMemo := '';
			end if;	
			if(length(pJrnlMemo) > 241) then --- Vì phải chừa ký tự khi làm Outgoing kế thừa remark sang có cộng thêm ký tự
				pJrnlMemo := substring(pJrnlMemo,1,241);
			end if;							
		end if;

	end if;	
	if (:pObType = 46) then -- Lay dien giai dong dau tien trong truong hop payment nhieu A/P invoice
		-- Lay so luong phieu A/P trong 1 Outgoing Payment
		select count(T1."DocNum"),max(T0."Canceled") into pInvCount,pCancel default 0 ,''
		from OVPM T0
		INNER JOIN VPM2 T1 ON T0."DocEntry" = T1."DocNum" AND T1."InvType" = '18'
		WHERE  T0."Canceled" = 'N' and T0."DocEntry" = :pDocEntry ;
		if (pCancel = 'N') then
			if :pInvCount = 1 then
				pFormat = 'PU ';
				SELECT 
			   		:pFormat ||  string_agg(T2."DocNum",',') || '-' || max(T2."JrnlMemo") as "NewFormat"
				INTO pJrnlMemo
				FROM OVPM T0
				INNER JOIN VPM2 T1 ON T0."DocEntry" = T1."DocNum" AND T1."InvType" = '18'
				inner JOIN OPCH T2 ON T1."DocEntry" = T2."DocEntry"
				WHERE T0."DocEntry" = :pDocEntry
				group by T0."DocNum" ;			
			end if;
			if :pInvCount > 1 then
				pFormat = 'Payment-Inv. ';
				SELECT 
			   		:pFormat ||  string_agg(ifnull(cast(T2."U_APInv" as varchar),''),',') as "NewFormat"
				INTO pJrnlMemo
				FROM OVPM T0
				INNER JOIN VPM2 T1 ON T0."DocEntry" = T1."DocNum" AND T1."InvType" = '18'
				inner JOIN OPCH T2 ON T1."DocEntry" = T2."DocEntry"
				WHERE T0."DocEntry" = :pDocEntry
				--T0."DocNum" = 4089
				group by T0."DocNum" ;				
			end if;		
		end if;	

	end if;
	
	return	select ifnull(:pJrnlMemo,'') as "JrnlMemo" from dummy;
end
