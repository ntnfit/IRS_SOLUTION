
CREATE PROCEDURE APZ_EmployeeMasterData_exists_linked

(in empID INT,
out result table ("ObjType" VARCHAR(20),
		"DocEntry" int))
AS

BEGIN
declare hasRow int;

	CREATE local temporary table #tbl (
		"ObjType" VARCHAR(20),
		"DocEntry" int
	);

	INSERT INTO #tbl
	        ( "ObjType", "DocEntry" )
	SELECT TOP 1 "ObjType","DocEntry" FROM OCPI WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OCPV WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	
	end if;
    select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORPC WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;
    select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ODPO WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	
    end if;
	 select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORPC WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;
     select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OPCH WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;
	 select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OCIN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OCSI WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OCSV WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORIN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ODPI WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OINV WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORIN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '33',"ClgCode" FROM OCLG WHERE "AttendEmpl"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000000',"Code" FROM OADM WHERE "ChfAcc"=:empID or "CEO" =:empID  or "GTSDftChk" = :empID or "GTSDftPye" = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1250000025',"AbsID" FROM OOAT WHERE "Owner"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000001',"CpnNo" FROM OCPN WHERE "Owner"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '60150',"insID" FROM OINS WHERE "technician"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '140',"DocEntry" FROM ODLN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '3002',"DocEntry" FROM ODRF WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000002',"AbsEntry" FROM OEFDW WHERE "AccExtern" =:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OIGE WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OIGN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OPDN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORPD WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OIEI WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000003',"AbsEntry" FROM OISW WHERE "CntPersID" =:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OWTR WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OWTQ WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	 select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '4',"DocEntry" FROM OITM WHERE "Technician"=:empID or "Employee" =:empID;
	end if;

	--select COUNT(1) into hasRow FROM #tbl;
	--IF(hasRow <=0) Then 
	--	INSERT INTO #tbl
	--	        ( "ObjType", "ObjType" )
	--	SELECT TOP 1 "ObjType","ObjType" FROM ODOC WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID
	--

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OOEI WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000004',"DocEntry" FROM APJ2 WHERE "Owner" =:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000005',"DocEntry" FROM PJT2 WHERE "Owner" =:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OPOR WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OPQT WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OPRQ WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000006',"ResCode" FROM RSC4 WHERE "EmpID"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORDN WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '666',"SlpCode" FROM OSLP WHERE "EmpID"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '320',"OpprId" FROM OOPR WHERE "Owner"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM ORDR WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 "ObjType","DocEntry" FROM OQUT WHERE "OwnerCode"=:empID or "Payee" = :empID or "Checker" = :empID or "Releaser" =:empID or "Receiver"  = :empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '191',"callID" FROM OSCL WHERE "technician"=:empID;
	end if;

	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '64',"WhsCode" FROM OWHS WHERE "StorKeeper"=:empID;
	end if;
	--Thông tin lương
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000007',"EmpId" FROM APZ_OHSI WHERE "EmpId"=:empID;
	end if;
	--Bảng phụ cấp lương
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000008',"EmpId" FROM APZ_HSI1 WHERE "EmpId"=:empID;
	end if;
	--Bảng tạm ứng lương
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000009',"EmpId" FROM APZ_HSI2 WHERE "EmpId"=:empID;
	end if;
	--Bảng lưu tính lương
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000010',"empID" FROM APZ_OSAL WHERE "empID"=:empID;
	end if;
	--Bảng chấm công
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 	
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000011',"EmpId" FROM APZ_OTSH WHERE "EmpId"=:empID;
	
	end if;
	--Bảng chấm phạt
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
	
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000012',"EmpId" FROM APZ_HPT1 WHERE "EmpId"=:empID;
		
end if;
	-- Bảng OCLG(Activity)
	
	select COUNT(1) into hasRow FROM #tbl;
	IF(hasRow <=0) Then 
	
		INSERT INTO #tbl
		        ( "ObjType", "DocEntry" )
		SELECT TOP 1 '1000000013',"ClgCode" FROM APZ_OCLG WHERE COMPARESPLIT(:empID,"GroupEmployee",';') = 1;
	end if;
	
	--select * from #tbl;
	--select * into result from #tbl;
	result = SELECT * FROM #TBL; 
END




