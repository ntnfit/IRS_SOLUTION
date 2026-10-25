
CREATE PROCEDURE apz_picking_getdoc(
	in docentry INT default 0,
	in whs NVARCHAR(50) default '',
	in fromDate timestamp default NULL,
	in toDate timestamp default NULL,
	in docStatus NVARCHAR(10) default '',
	in bpCode NVARCHAR(50) default '',
	in userName NVARCHAR(50) default '',
	in branch INT default -1)
AS
BEGIN
	IF :DocEntry<>0
	THEN
		SELECT * FROM APZ_OPKL WHERE "AbsEntry"= :DocEntry ORDER BY "AbsEntry" desc;
	ELSE 
		SELECT * FROM APZ_OPKL WHERE 
		("Status"= :DocStatus OR IFNULL(:DocStatus,'')='')
		--AND (CardCode=@CardCode OR @CardCode='')
		AND "PickDate" BETWEEN IFNULL(:FromDate,'1900-01-01') AND IFNULL(:ToDate,CURRENt_DATE)	ORDER BY "AbsEntry" desc;
	END IF;
END;



