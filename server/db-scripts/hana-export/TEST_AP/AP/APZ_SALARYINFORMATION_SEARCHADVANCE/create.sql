
CREATE PROCEDURE APZ_SalaryInformation_SearchAdvance(
	in branch nvarchar(40000) default null,
	in dept nvarchar(40000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(40000) default null,
	in period nvarchar(200) default null)
AS
BEGIN
	--Variable
	declare maxdate timestamp;
	--Tạo bảng tạm chứa thông tin trả về
	Create local temporary table #tmp_Advance As (
		Select Top 0 b."empID", b."ExtEmpNo", b."dept", b."branch", b."status", 0 As "empName",
		a.*
		From APZ_HSI2 a Join OHEM b On a."EmpId" = b."empID"
	);
	select ifnull((select max("ValidTo") from APZ_OHSI),'9999-12-31') into maxdate from DUMMY;
	
	--Lấy ra danh sách nhân viên
	Insert Into #tmp_Advance("empID", "ExtEmpNo", "dept", "branch", "status", "empName", "EmpId", "LineNum","Period")
		Select b."empID", b."ExtEmpNo", b."dept", b."branch", b."status", b."empID" AS "empName", b."empID", 0,:period
		From OHEM b LEFT OUTER JOIN APZ_OHSI a ON a."EmpId" = b."empID" AND CURRENT_DATE BETWEEN a."ValidFrom" AND IFNULL(a."ValidTo", cast(TO_VARCHAR(:maxdate,'yyyy-MM-dd') as timestamp))
		Where (ifnull(:branch,0)=0 or (:branch<>0 and b."branch" = :branch))
			AND (ifnull(:dept,'')='' or :dept = '0' or (:dept<>'' and :dept <> '0' and cast(b."dept" as nvarchar) in (select data from split(:dept,','))))
			AND (ifnull(:empName,'')='' or (:empName<>'' and 
				(b."firstName" like :empName
				or b."middleName" like :empName
				or b."lastName" like :empName)))
			AND (ifnull(:empNo,0)=0 or (:empNo<>0 and b."empID"=:empNo))
			AND (ifnull(:status,'')='' or (:status<>'' and cast(b."status" as nvarchar) in (select data from split(:status,','))))
			AND b."empID" Is NOT NULL;
	
	---Update lại thông tin
	--Trường hợp period đã có thông tin
	UPDATE a SET "LineNum" = APZ_HSI2."LineNum",
				"Amount" = APZ_HSI2."Amount",
				"Remain" = APZ_HSI2."Remain",
				"MonthlyPay" = APZ_HSI2."MonthlyPay",
				"Received" = APZ_HSI2."Received",
				"Remarks" = APZ_HSI2."Remarks",
				"UserSign2" = APZ_HSI2."UserSign2",
				"UpdateDate" = APZ_HSI2."UpdateDate"
	FROM #tmp_Advance a, APZ_HSI2 WHERE a."Period" = APZ_HSI2."Period" And a."empID" = APZ_HSI2."EmpId";
	--Trường hợp chưa có thông tin
	Update #tmp_Advance Set "MonthlyPay" = 'N', "Received"= 'N' Where "LineNum" = 0;
	
	Select * From #tmp_Advance;
	Return;
END




