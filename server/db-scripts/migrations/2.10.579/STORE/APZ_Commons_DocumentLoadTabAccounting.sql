
CALL APZ_EXISTSPROC('APZ_Commons_DocumentLoadTabAccounting');

GO

CREATE procedure APZ_Commons_DocumentLoadTabAccounting(
	in entry int default 0,
	in objType int default 0
)
AS
BEGIN
	DECLARE tblname NVARCHAR(10)='';
	declare refTableName nvarchar(10)='';
	DECLARE sqlQuery NVARCHAR(5000)='';
	
	If :objType = '1250000025' or :objType = '1250000026'
	THEN
		
		If Exists(Select 1 From OOAT Where "AbsID" = :entry) Then
			SELECT '' "JrnlMemo", IFNULL("Project",'') As "Project", IFNULL("GroupNum",-1) As "GroupNum",'' "PeyMethod",'' "Indicator", '' "LicTradNum", -1 "ImportEnt", '' "PayDuMonth",
				-1 "ExtraMonth",-1 "ExtraDays", -1 "CdcOffset", '' "UseShpdGd", 0 RefDoc
			From OOAT Where "AbsID" = :entry;
			Return;
		End If;
		
		SELECT top 0 '' "JrnlMemo", '' "Project",-1 "GroupNum", '' "PeyMethod",'' "Indicator", '' "LicTradNum", -1 "ImportEnt", '' "PayDuMonth",-1 "ExtraMonth",-1 "ExtraDays", -1 "CdcOffset",
			 '' "UseShpdGd",0 RefDoc from DUMMY;
		Return;
	END IF;
	
	select IFNULL((SELECT "TblName" FROM apz_tblobject a WHERE a."ObjType"=:objType),'') into tblname from DUMMY;
	select left(IFNULL((SELECT "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:objType),''),3)||'21'  into refTableName from DUMMY; 
	
	
	sqlQuery = N'SELECT "JrnlMemo", "Project","GroupNum","PeyMethod","Indicator", "LicTradNum", "ImportEnt", "PayDuMonth","ExtraMonth","ExtraDays", "CdcOffset","UseShpdGd",(select count(1) from '||:refTableName||' where "DocEntry"='|| cast(:entry as nvarchar) || ') RefDoc
				FROM ' || :tblname || ' WHERE "DocEntry"=' || cast(:entry as nvarchar) || ';';
			
	execute immediate :sqlQuery;

END