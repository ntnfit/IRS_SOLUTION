CREATE procedure VAS_F_PC_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	declare listAcc varchar(500);
	
	select ifnull(x."BPLId",0) into bplID from "OPCH" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		RptName 		nvarchar(300), 
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300), 
		CompTaxID 		nvarchar(50), 
		CompTel 		nvarchar(50), 
		CompFax 		nvarchar(50),
		BPCode 			nvarchar(20), 
		BPName 			nvarchar(300), 
		BPAddr 			nvarchar(300), 
		BPTaxID 		nvarchar(50), 
		DocNo 			int, 
		DocDate 		timestamp, 
		DocTime 		nvarchar(10), 
		DueDate 		timestamp, 
		DebAcc 			nvarchar(100), 
		CredAcc 		nvarchar(100),
		ItmCode 		nvarchar(50), 
		ItmName 		nvarchar(200), 
		Qty 			numeric(21,6),
		Price	 		numeric(21,6),
		Amount 			numeric(21,6),
		TotalTax 		numeric(21,6),
		TotalAftTax 	numeric(21,6),
		SalEmpID 		int, 
		SalEmpName 		nvarchar(100), 
		PayTerm 		nvarchar(100), 
		Comments 		nvarchar(300), 
		CreatedByUser	nvarchar(100)
	);	
	create local temporary table #tblACCOUNT (Account nvarchar(20));
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (BPCode,BPName,BPAddr,BPTaxID,DocNo,DocDate,DocTime,DueDate,ItmCode,ItmName,Qty,Price,Amount,TotalTax,TotalAftTax,SalEmpID,PayTerm,Comments,CreatedByUser)
	SELECT a."CardCode", a."CardName", a."Address", a."LicTradNum", a."DocNum", a."DocDate", a."DocTime", a."DocDueDate", b."ItemCode", b."Dscription", b."Quantity", b."Price", b."LineTotal", a."VatSum", a."DocTotal", a."SlpCode", a."GroupNum", a."Comments", a."UserSign" FROM "OPCH" a, "PCH1" b WHERE a."DocEntry" = b."DocEntry" AND a."DocEntry" = :DocID;


	insert into #tblACCOUNT (Account) select "Account" from "JDT1" where "TransType"='18' and "CreatedBy"=DocID and "DebCred"='D' group by "Account";
	update #tbl set DebAcc = (SELECT STRING_AGG(Account,','ORDER BY Account) FROM #tblACCOUNT);
	delete from #tblACCOUNT;
	insert into #tblACCOUNT (Account) select "Account" from "JDT1" where "TransType"='18' and "CreatedBy"=DocID and "DebCred"='C' group by "Account";
	update #tbl set CredAcc = (SELECT STRING_AGG(Account,','ORDER BY Account) FROM #tblACCOUNT);
	
	update #tbl set SalEmpName 		= (select x."SlpName" from "OSLP" x where x."SlpCode" = SalEmpID);
	update #tbl set PayTerm 		= (select x."PymntGroup" from "OCTG" x where x."GroupNum" = PayTerm);
	update #tbl set CreatedByUser 	= (select x."U_NAME" from "OUSR" x where x."USERID"=CreatedByUser);
	
	update #tbl set 
		RptName 	= N'', 
		CompTaxID	= (select a."TaxIdNum" from "OADM" a,"ADM1" b),
		CompTel		= (select a."Phone1" from "OADM" a,"ADM1" b),
		CompFax		= (select a."Fax" from "OADM" a,"ADM1" b);
	
	if bplID=0 then
		update #tbl set 
			CompName 	= (select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAddr 	= (select b."Street" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	= (select x."BPLName" from "OBPL" x where x."BPLId"=bplID),
			CompAddr	= (select x."Street" from "OBPL" x where x."BPLId"=bplID);
	end if;	

	select 
		RptName  "RptName",
		CompName  "CompName",
		CompAddr  "CompAddr",
		CompTaxID  "CompTaxID",
		CompTel  "CompTel",
		CompFax  "CompFax",
		BPCode  "BPCode",
		BPName  "BPName",
		BPAddr  "BPAddr",
		BPTaxID  "BPTaxID",
		DocNo  "DocNo",
		DocDate  "DocDate",
		DocTime  "DocTime",
		DueDate  "DueDate",
		DebAcc  "DebAcc",
		CredAcc  "CredAcc",
		ItmCode  "ItmCode",
		ItmName  "ItmName",
		Qty  "Qty",
		Price  "Price",
		Amount  "Amount",
		TotalTax  "TotalTax",
		TotalAftTax  "TotalAftTax",
		SalEmpID  "SalEmpID",
		SalEmpName  "SalEmpName",
		PayTerm  "PayTerm",
		Comments  "Comment",
		CreatedByUser  "CreatedByUser"
	from #tbl;

	drop table #tbl;
	drop table #tblACCOUNT;
end

