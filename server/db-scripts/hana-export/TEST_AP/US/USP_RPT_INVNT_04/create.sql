CREATE procedure USP_RPT_INVNT_04 (
	in tdate			timestamp,
	in groupID			int,
	in warehouse		nvarchar(20),
	in point1			int,
	in point2			int,
	in point3			int,
	in point4			int,
	in itemcode			nvarchar(5000)
)
as
begin
	-- TUỔI HÀNG TỒN KHO THEO BATCH/SERIAL----------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		ManageBy		nvarchar(20),
		BatchID			int,
		SSCC			nvarchar(50),
		Batch			varchar(50),
		InputDate		timestamp,
		ExpireDate		timestamp,
		InputQty		numeric(21,6),
		AgeDays			int,
		BalQty			numeric(21,6),
		BalQtyTmp		numeric(21,6),
		Qty1			numeric(21,6),
		Qty2			numeric(21,6),
		Qty3			numeric(21,6),
		Qty4			numeric(21,6),
		Qty5			numeric(21,6),
		Note			nvarchar(100),
		
		Bin				nvarchar(300),
		CreatedTime		nvarchar(15)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" where 
	--(branch in (-1,0) or "BPLid"=branch) and 
	(warehouse='' or "WhsCode"=warehouse);
	------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblITEM (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		InventoryAcct	nvarchar(20),
		GLMethod		char(1),
		ManSerNum		char(1),
		ManBtchNum		char(1),
		GrpID			int,
		GrpName			nvarchar(100)
	);
	insert into #tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName,GLMethod,InventoryAcct,ManSerNum,ManBtchNum) 
	select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam",a."GLMethod",case when a."GLMethod"='C' then b."BalInvntAc" end,a."ManSerNum",a."ManBtchNum"
	from "OITM" a, "OITB" b 
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."InvntItem"='Y'
	and (b."ItmsGrpCod"=groupID or groupID=0)
	and
			(
			IFNULL(:itemcode, '') = '' OR cast ("ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
			--ifnull((select STRING_AGG( Code) from :itemcode), '') = '' OR cast (a."ItemCode" as nvarchar) IN (select Code from :itemcode)
			) 
	
	;
	--ITEM QUẢN LÝ THEO BATCH/SERIAL=========================================================================================
	create local temporary table #tblBATCH (
		ItemCode 	nvarchar(50),
		SysNumber 	int,
		DistNumber	nvarchar(50),
		InDate		timestamp,
		ExpireDate	timestamp		
	);
	insert into #tblBATCH (ItemCode,SysNumber,DistNumber,InDate,ExpireDate)
	select "ItemCode","SysNumber","DistNumber","InDate","ExpDate" from "OBTN";
	insert into #tblBATCH (ItemCode,SysNumber,DistNumber,InDate,ExpireDate)
	select "ItemCode","SysNumber","DistNumber","InDate","ExpDate" from "OSRN";
	--RAW DATA ---------------------------------------------------------------------------------------------------------------
	create local temporary table #tblDATA (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		ManageBy		nvarchar(20),
		BatchID			int,
		SSCC			nvarchar(50),
		InputDate		timestamp,
		ExpireDate		timestamp,
		DocType			nvarchar(20),
		DocID			int,
		Qty				numeric(21,6)
	);
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,BatchID,SSCC,InputDate,ExpireDate,DocType,DocID,Qty,ManageBy)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,a2."SysNumber",d.DistNumber,d.InDate,d.ExpireDate,a1."DocType",a1."DocEntry",a2."Quantity",case when ManSerNum='Y' then 'Serial' when ManBtchNum='Y' then 'Batch' else '' end
	from "OITL" a1, "ITL1" a2, #tblWH b, #tblITEM c, #tblBATCH d
	where a1."DocDate"<=tdate and a1."LocCode"=b.WHCode and a1."ItemCode"=c.ItemCode 
	and a1."LogEntry"=a2."LogEntry" and a1."ItemCode"=a2."ItemCode"
	and a2."ItemCode"=d.ItemCode and a2."SysNumber"=d.SysNumber
	and (c.ManSerNum='Y' or c.ManBtchNum='Y');
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,BatchID,SSCC,InputDate,ExpireDate,BalQty,ManageBy)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,BatchID,SSCC,InputDate,ExpireDate,sum(Qty),ManageBy
	from #tblDATA
	group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,BatchID,SSCC,InputDate,ExpireDate,ManageBy;
	-----------
	update #tbl set InputQty = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.BatchID=#tbl.BatchID);
	-----------
	update #tbl set AgeDays = DAYS_BETWEEN(InputDate,tdate);
	update #tbl set Qty1 = BalQty where AgeDays <= point1;
	update #tbl set Qty2 = BalQty where AgeDays > point1 and AgeDays <= point2;
	update #tbl set Qty3 = BalQty where AgeDays > point2 and AgeDays <= point3;
	update #tbl set Qty4 = BalQty where AgeDays > point3 and AgeDays <= point4;
	update #tbl set Qty5 = BalQty where AgeDays > point4;
	--ITEM KHÔNG QUẢN LÝ THEO BATCH/SERIAL=========================================================================================
	delete from #tblDATA;
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,DocID,InputDate,Qty,ManageBy)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,a."TransType",a."CreatedBy",a."DocDate",a."InQty"-a."OutQty",''
	from "OINM" a, #tblWH b, #tblITEM c
	where a."DocDate"<=tdate and a."Warehouse"=b.WHCode and a."ItemCode"=c.ItemCode 
	and (c.ManSerNum='N' and c.ManBtchNum='N');
	---
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,BalQty,BalQtyTmp,ManageBy,Note)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,sum(Qty),sum(Qty),ManageBy,'Mô phỏng theo phương pháp FIFO'
	from #tblDATA
	group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,ManageBy
	having sum(Qty)<>0;
	---
	update #tbl set InputQty = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode)
	where ManageBy='';
	---
	update #tbl set 
		Qty1 = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.InputDate between ADD_DAYS(tdate,-point1) and tdate),
		Qty2 = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.InputDate between ADD_DAYS(tdate,-point2) and ADD_DAYS(tdate,-point1-1)),
		Qty3 = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.InputDate between ADD_DAYS(tdate,-point3) and ADD_DAYS(tdate,-point2-1)),
		Qty4 = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.InputDate between ADD_DAYS(tdate,-point4) and ADD_DAYS(tdate,-point3-1)),
		Qty5 = (select SUM(b.Qty) from #tblDATA b where b.DocType in ('18','19','20','21','59','310000001') and b.ItemCode=#tbl.ItemCode and b.InputDate < ADD_DAYS(tdate,-point4))
	where ManageBy='';
	-- Trừ số còn tồn theo các khoản nhập trên
	-- Khoảng 1 
	update #tbl set Qty1 = BalQtyTmp, Qty2 = 0, Qty3 = 0, Qty4 = 0, Qty5 = 0, BalQtyTmp = 0 where BalQtyTmp<=Qty1 and BalQtyTmp>=0 and ManageBy='';
	update #tbl set BalQtyTmp = BalQtyTmp - Qty1 where BalQtyTmp>Qty1 and BalQtyTmp>=0 and ManageBy='';
	-- Khoảng 2 
	update #tbl set Qty2 = BalQtyTmp, Qty3 = 0, Qty4 = 0, Qty5 = 0, BalQtyTmp = 0 where BalQtyTmp<=Qty2 and BalQtyTmp>=0 and ManageBy='';
	update #tbl set BalQtyTmp = BalQtyTmp - Qty2 where BalQtyTmp>Qty2 and BalQtyTmp>=0 and ManageBy='';
	-- Khoảng 3 
	update #tbl set Qty3 = BalQtyTmp, Qty4 = 0, Qty5 = 0, BalQtyTmp = 0 where BalQtyTmp<=Qty3 and BalQtyTmp>=0 and ManageBy='';
	update #tbl set BalQtyTmp = BalQtyTmp - Qty3 where BalQtyTmp>Qty3 and BalQtyTmp>=0 and ManageBy='';
	-- Khoảng 4 
	update #tbl set Qty4 = BalQtyTmp, Qty5 = 0, BalQtyTmp = 0 where BalQtyTmp<=Qty4 and BalQtyTmp>=0 and ManageBy='';
	update #tbl set BalQtyTmp = BalQtyTmp - Qty4 where BalQtyTmp>Qty4 and BalQtyTmp>=0 and ManageBy='';

	-- Khoảng 5 
	update #tbl set Qty5 = BalQtyTmp, BalQtyTmp = 0 where BalQtyTmp>=0 and ManageBy='';

	update #tbl set Qty1 = ifnull(Qty1,0), Qty2 = ifnull(Qty2,0), Qty3 = ifnull(Qty3,0), Qty4 = ifnull(Qty4,0), Qty5 = ifnull(Qty5,0);
	
	--update CreatedTime
	update a 
	set CreatedTime= case when LENGTH(b."U_CreatedTime") = 3 then  '0'||left(b."U_CreatedTime",1) || ':' || right(b."U_CreatedTime",2) 
						  when LENGTH(b."U_CreatedTime") = 4 then left(b."U_CreatedTime",2) || ':' || right(b."U_CreatedTime",2)  else '' end
	from #tbl a inner join OBTN b on b."ItemCode"=a.ItemCode and b."SysNumber" = a.BatchID;
	--update Bin
/*
	update a
	set Bin  = c."BinCode"
	from #tbl a
	left join OINM a1 on a.ItemCode = a1."ItemCode"
	inner join OBBQ b on a.BatchID = b."SnBMDAbs" and a.ItemCode = b."ItemCode" and b."WhsCode" = a1."Warehouse"
	inner join OBIN c on c."AbsEntry" = b."BinAbs";	
*/

	lcsql = select c."ItemCode", e."DistNumber", T5."BinCode" 
	from OITL a inner join ITL1 b on a."LogEntry"=b."LogEntry" and a."ItemCode"=b."ItemCode" and b."Quantity" <> 0		
				left join OBTN e on e."ItemCode"=b."ItemCode" and e."SysNumber"=b."SysNumber"				
				inner JOIN OBTL T4 ON a."LogEntry"   = t4."ITLEntry" and T4."SnBMDAbs" = e."AbsEntry"
				left JOIN OBIN T5 ON T5."AbsEntry" = T4."BinAbs"				
				inner join OITM c on c."ItemCode"=b."ItemCode"
	where a."DocDate"<=:tdate	
	group by c."ItemCode", e."DistNumber", T5."BinCode"
	having sum(case when b."Quantity" > 0 then t4."Quantity" else -t4."Quantity" end) > 0 ;

	update x set Bin = y."BinCode"
	from #tbl x
	inner join :lcsql y on y."ItemCode" = x.ItemCode and y."DistNumber" = x.SSCC
	where ManageBy <> '' and BalQty > 0 ;
	---
	update a
	set a.Batch  = b."MnfSerial"
	from #tbl a
	inner join OBTN b on a.SSCC  = b."DistNumber";
	-- SSCC có BalQty=0 thì loại bỏ trong báo cáo
	delete from #tbl where ifnull(BalQty,0)  = 0;
	
	select * from #tbl a order by ItemCode;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
	drop table #tblBATCH;
	drop table #tblDATA;
end

